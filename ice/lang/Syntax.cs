﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ice.lang
{
    /// <summary>
    /// 语法分析器
    /// </summary>
    public abstract class Syntax
    {
        /// <summary>
        /// 二元运算符优先级表
        /// </summary>
        /// <remarks>与Lexer.Token具有相同顺序</remarks>
        private static readonly int[] BinaryOperatorPriorityTable = new int[]
        {
            3, // Plus
            3, // Minus
            2, // Mul
            2, // Div
            1  // Power
        };

        /// <summary>
        /// 匹配一个Token
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <param name="TokenToMatch">需要匹配的Token</param>
        private static void MatchToken(Lexer Lex, Lexer.Token TokenToMatch)
        {
            if (Lex.CurrentToken != TokenToMatch)
                throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                    String.Format("expect {0}, but found {1}.", Lexer.FormatToken(TokenToMatch), Lex.FormatCurrentToken()));

            Lex.Next();
        }

        /// <summary>
        /// 匹配一个Identifier
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>ID（大小写）</returns>
        private static string MatchIdentifier(Lexer Lex)
        {
            if (Lex.CurrentToken != Lexer.Token.Identifier)
                throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                    String.Format("expect {0}, but found {1}.", Lexer.FormatToken(Lexer.Token.Identifier), Lex.FormatCurrentToken()));

            string tRet = Lex.CurrentIdentify;
            Lex.Next();
            return tRet;
        }

        /// <summary>
        /// 尝试匹配一个Token，若成功则向后推进
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <param name="TokenToMatch">需要匹配的Token</param>
        /// <returns>是否成功匹配</returns>
        private static bool TryMatchToken(Lexer Lex, Lexer.Token TokenToMatch)
        {
            if (Lex.CurrentToken != TokenToMatch)
                return false;

            Lex.Next();
            return true;
        }

        /// <summary>
        /// 尝试匹配一个Identifier，若成功则向后推进
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <param name="Id">匹配到的Id</param>
        /// <returns>是否成功匹配</returns>
        private static bool TryMatchIdentifier(Lexer Lex, out string Id)
        {
            Id = String.Empty;

            if (Lex.CurrentToken != Lexer.Token.Identifier)
                return false;

            Id = Lex.CurrentIdentify;
            Lex.Next();
            return true;
        }

        /// <summary>
        /// 解析语法树
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>语法树</returns>
        public static ASTNode_StatementList Parse(Lexer Lex)
        {
            Lex.Next();  // 预读取第一个词法单元

            ASTNode_StatementList tRet = ParseStatementList(Lex);
            if (Lex.CurrentToken != Lexer.Token.EOF)
                throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                    String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));

            return tRet;
        }

        /// <summary>
        /// 解析Statement
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <param name="Result">解析到的语句</param>
        /// <returns>是否解析成功</returns>
        private static bool ParseStatement(Lexer Lex, out ASTNode_Statement Result)
        {
            if (Lex.CurrentToken == Lexer.Token.For)
                Result = ParseForStatement(Lex);  // for_statement
            else if (Lex.CurrentToken == Lexer.Token.Identifier)  // assignment or call
                Result = ParseAssignmentOrCall(Lex);
            else
            {
                Result = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 解析一个块中的StatementList
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode_StatementList ParseStatementList(Lexer Lex)
        {
            ASTNode_StatementList tRet = new ASTNode_StatementList();
            ASTNode_Statement tStatement;
            while (ParseStatement(Lex, out tStatement))
            {
                tRet.Statements.Add(tStatement);
            }
            return tRet;
        }

        /// <summary>
        /// 解析一个Block
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode_StatementList ParseBlock(Lexer Lex)
        {
            ASTNode_StatementList tRet;

            if (TryMatchToken(Lex, Lexer.Token.LeftBrace))  // {
            {
                tRet = ParseStatementList(Lex);

                MatchToken(Lex, Lexer.Token.RightBrace);  // }

                return tRet;
            }
            else  // 单条语句
            {
                ASTNode_Statement tStatement;
                if (ParseStatement(Lex, out tStatement))
                {
                    tRet = new ASTNode_StatementList();
                    tRet.Statements.Add(tStatement);
                    return tRet;
                }
                else
                    throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                        String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));
            }
        }

        /// <summary>
        /// 解析一个ArgList
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode_ArgList ParseArgList(Lexer Lex)
        {
            ASTNode_ArgList tRet = new ASTNode_ArgList();
            if (Lex.CurrentToken != Lexer.Token.RightBracket) // 非空arglist
            {
                while (true)
                {
                    tRet.Args.Add(ParseExpression(Lex));
                    if (TryMatchToken(Lex, Lexer.Token.Comma))  // ','
                        continue;
                    else if (Lex.CurrentToken == Lexer.Token.RightBracket)  // peek ')'
                        break;
                    else
                        throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                            String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));
                }
            }
            return tRet;
        }

        /// <summary>
        /// 解析For语句
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode_ForStatement ParseForStatement(Lexer Lex)
        {
            ASTNode_ForStatement tRet = new ASTNode_ForStatement(Lex.Line);

            // for <identifier> from <expression> to <expression> {step <expression>} <block>
            MatchToken(Lex, Lexer.Token.For);
            tRet.Identifier = MatchIdentifier(Lex);
            MatchToken(Lex, Lexer.Token.From);
            tRet.FromExpression = ParseExpression(Lex);
            MatchToken(Lex, Lexer.Token.To);
            tRet.ToExpression = ParseExpression(Lex);
            if (TryMatchToken(Lex, Lexer.Token.Step))
                tRet.StepExpression = ParseExpression(Lex);
            tRet.ExecBlock = ParseBlock(Lex);

            return tRet;
        }

        /// <summary>
        /// 解析一条赋值或者调用指令
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode_Statement ParseAssignmentOrCall(Lexer Lex)
        {
            string tIdentifier = MatchIdentifier(Lex);
            if (TryMatchToken(Lex, Lexer.Token.LeftBracket))  // call
            {
                ASTNode_Call tCall = new ASTNode_Call(Lex.Line, tIdentifier, ParseArgList(Lex));

                MatchToken(Lex, Lexer.Token.RightBracket);  // ')'
                MatchToken(Lex, Lexer.Token.Semico);  // ';'
                return tCall;
            }
            else if (TryMatchToken(Lex, Lexer.Token.Is))  // assignment
            {
                // 读取表达式
                ASTNode_Assignment tAssign = new ASTNode_Assignment(Lex.Line, tIdentifier, ParseExpression(Lex));

                MatchToken(Lex, Lexer.Token.Semico);  // ';'
                return tAssign;
            }
            else
                throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                    String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));
        }

        /// <summary>
        /// 解析表达式
        /// 
        /// 保证后续Token能产生一个表达式，否则抛出异常
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode_Expression ParseExpression(Lexer Lex)
        {
            return ParseBinaryExpression(Lex, BinaryOperatorPriorityTable.Length - 1);
        }

        /// <summary>
        /// 解析二元表达式
        /// 
        /// 当优先级为0时退化到一元表达式
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <param name="Priority">优先级</param>
        /// <returns>解析结果</returns>
        private static ASTNode_Expression ParseBinaryExpression(Lexer Lex, int Priority)
        {
            // 退化
            if (Priority == 0)
                return ParseUnaryExpression(Lex);

            // 递归解析左侧的表达式
            ASTNode_Expression tRet = ParseBinaryExpression(Lex, Priority - 1);

            // 检查是否为二元运算符
            if (Lex.CurrentToken >= Lexer.Token.Plus && (int)Lex.CurrentToken < (int)Lexer.Token.Plus + BinaryOperatorPriorityTable.Length)
            {
                int tPriority = BinaryOperatorPriorityTable[Lex.CurrentToken - Lexer.Token.Plus];
                if (tPriority > Priority) // 优先级不符，返回
                    return tRet;
                else
                    Priority = tPriority;
            }
            else
                return tRet;

            // 循环解析右侧的表达式
            while (true)
            {
                // 检查下一个算符的优先级
                Lexer.Token tOpt = Lex.CurrentToken;
                if (!(tOpt >= Lexer.Token.Plus && (int)tOpt < (int)Lexer.Token.Plus + BinaryOperatorPriorityTable.Length &&
                    BinaryOperatorPriorityTable[Lex.CurrentToken - Lexer.Token.Plus] == Priority))
                {
                    break;
                }

                // 吃掉运算符
                Lex.Next();

                // 获取算符右侧
                ASTNode_Expression tRight = ParseBinaryExpression(Lex, Priority - 1);

                // 组合成二元AST树
                tRet = new ASTNode_BinaryExpression(Lex.Line, BinaryOp.Plus + (tOpt - Lexer.Token.Plus), tRet, tRight);
            }

            return tRet;
        }

        /// <summary>
        /// 解析一元表达式
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode_Expression ParseUnaryExpression(Lexer Lex)
        {
            // 判断是否为一元表达式
            if (TryMatchToken(Lex, Lexer.Token.Minus))  // 一元负号
                return new ASTNode_UnaryExpression(Lex.Line, UnaryOp.Negative, ParseAtomExpression(Lex));
            else
                return ParseAtomExpression(Lex);
        }

        /// <summary>
        /// 解析原子表达式
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode_Expression ParseAtomExpression(Lexer Lex)
        {
            ASTNode_Expression tRet;
            if (Lex.CurrentToken == Lexer.Token.DigitLiteral)  // digit_literal
            {
                tRet = new ASTNode_DigitLiteral(Lex.Line, Lex.CurrentDigit);
                Lex.Next();
                return tRet;
            }
            else if (Lex.CurrentToken == Lexer.Token.Identifier)  // symbol or call_expression
            {
                string tIdentifier = MatchIdentifier(Lex);

                // 检查下一个符号
                if (TryMatchToken(Lex, Lexer.Token.LeftBracket))  // '(' -- call_expression
                {
                    ASTNode_ArgList tArgList = ParseArgList(Lex);
                    MatchToken(Lex, Lexer.Token.RightBracket);  // ')'
                    return new ASTNode_CallExpression(Lex.Line, tIdentifier, tArgList);
                }
                else  // symbol
                    return new ASTNode_SymbolExpression(Lex.Line, tIdentifier);
            }
            else if (TryMatchToken(Lex, Lexer.Token.LeftBracket))  // '(' -- bracket_expression
            {
                tRet = ParseBracketExpression(Lex);

                MatchToken(Lex, Lexer.Token.RightBracket);  // ')'
                return tRet;
            }
            else
                throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                    String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));
        }

        /// <summary>
        /// 解析括号表达式
        /// 
        /// 括号表达式不可为空，当仅有一个元素时表达为单值，当有多个元素时表达为向量
        /// </summary>
        /// <param name="Lex">词法分析器</param>
        /// <returns>解析结果</returns>
        private static ASTNode_Expression ParseBracketExpression(Lexer Lex)
        {
            // 先读取第一个表达式
            ASTNode_Expression tFirstExpr = ParseExpression(Lex);

            // 检查后续元素
            if (Lex.CurrentToken != Lexer.Token.RightBracket)
            {
                ASTNode_TupleExpression tList = new ASTNode_TupleExpression(Lex.Line);
                tList.Args.Add(tFirstExpr);

                while (true)
                {
                    MatchToken(Lex, Lexer.Token.Comma);  // ','
                    tList.Args.Add(ParseExpression(Lex));

                    if (Lex.CurrentToken == Lexer.Token.Comma)  // peek ','
                        continue;
                    if (Lex.CurrentToken == Lexer.Token.RightBracket)  // peek ')'
                        break;
                    else
                        throw new SyntaxException(Lex.Position, Lex.Line, Lex.Row,
                            String.Format("unexpected token {0}.", Lex.FormatCurrentToken()));
                }
                return tList;
            }
            else
                return tFirstExpr;
        }
    }

    public class SyntaxException : Exception
    {
        private int position;
        private int line;
        private int row;
        private string description;

        private static string CombineException(int position, int line, int row, string description)
        {
            return String.Format("({0},{1},{2}): {3}", line, row, position, description);
        }

        public int Position
        {
            get
            {
                return position;
            }
        }

        public int Line
        {
            get
            {
                return line;
            }
        }

        public int Row
        {
            get
            {
                return row;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }

        public SyntaxException(int Position, int Line, int Row, string Description)
            : base(CombineException(Position, Line, Row, Description))
        {
            position = Position;
            line = Line;
            row = Row;
            description = Description;
        }
    }
}
