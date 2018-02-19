using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ice.lang
{
    /// <summary>
    /// binary operator
    /// </summary>
    /// <remarks>In order with Lexer.Token</remarks>
    public enum BinaryOp
    {
        Plus,
        Minus,
        Mul,
        Div,
        Power
    }

    /// <summary>
    /// unary operator
    /// </summary>
    public enum UnaryOp
    {
        Negative
    }

    public class ASTNode
    {
        /// <summary>
        /// type of abstract syntax tree 
        /// </summary>
        public enum ASTType
        {
            NotImpl,
            StatementList,
            ArgList,

            Assignment,
            Call,
            ForStatement,

            BinaryExpression,
            UnaryExpression,
            DigitLiteral,
            CallExpression,
            SymbolExpression,
            TupleExpression
        }

        private ASTType mType = ASTType.NotImpl;
        private int mLineNumber = 0;

        public ASTType Type
        {
            get { return mType; }
        }

        public int LineNumber
        {
            get { return mLineNumber; }
        }

        public ASTNode(ASTType AType)
        {
            mType = AType;
        }

        public ASTNode(ASTType AType, int LineNum)
        {
            mType = AType;
            mLineNumber = LineNum;
        }
    }

    public class ASTNode_StatementList : ASTNode
    {
        private List<ASTNode_Statement> mStatements = new List<ASTNode_Statement>();

        public IList<ASTNode_Statement> Statements
        {
            get
            {
                return mStatements;
            }
        }

        public ASTNode_StatementList()
            : base(ASTNode.ASTType.StatementList) { }
    }

    public class ASTNode_Statement : ASTNode
    {
        public ASTNode_Statement(ASTNode.ASTType AType, int LineNum)
            : base(AType, LineNum) { }
    }

    public class ASTNode_Assignment : ASTNode_Statement
    {
        private string mIdentifier = String.Empty;
        private string mIdentifierLower = String.Empty;
        private ASTNode_Expression mExpression = null;

        public string Identifier
        {
            get
            {
                return mIdentifier;
            }
            set
            {
                mIdentifier = value;
                mIdentifierLower = mIdentifier.ToLower();
            }
        }

        public string IdentifierLower
        {
            get
            {
                return mIdentifierLower;
            }
        }

        public ASTNode_Expression Expression
        {
            get
            {
                return mExpression;
            }
            set
            {
                mExpression = value;
            }
        }

        public ASTNode_Assignment(int LineNum, string Id, ASTNode_Expression Expr)
            : base(ASTNode.ASTType.Assignment, LineNum)
        {
            Identifier = Id;
            Expression = Expr;
        }
    }

    public class ASTNode_Expression : ASTNode
    {
        public ASTNode_Expression(ASTNode.ASTType AType, int LineNum)
            : base(AType, LineNum) { }
    }

    public class ASTNode_Call : ASTNode_Statement
    {
        private string mIdentifier = String.Empty;
        private string mIdentifierLower = String.Empty;
        private ASTNode_ArgList mArgList = null;

        public string Identifier
        {
            get
            {
                return mIdentifier;
            }
            set
            {
                mIdentifier = value;
                mIdentifierLower = mIdentifier.ToLower();
            }
        }

        public string IdentifierLower
        {
            get
            {
                return mIdentifierLower;
            }
        }

        public ASTNode_ArgList ArgList
        {
            get
            {
                return mArgList;
            }
            set
            {
                mArgList = value;
            }
        }

        public ASTNode_Call(int LineNum, string Id, ASTNode_ArgList AL)
            : base(ASTNode.ASTType.Call, LineNum)
        {
            Identifier = Id;
            ArgList = AL;
        }
    }

    public class ASTNode_CallExpression : ASTNode_Expression
    {
        private string mIdentifier = String.Empty;
        private string mIdentifierLower = String.Empty;
        private ASTNode_ArgList mArgList = null;

        public string Identifier
        {
            get
            {
                return mIdentifier;
            }
            set
            {
                mIdentifier = value;
                mIdentifierLower = mIdentifier.ToLower();
            }
        }

        public string IdentifierLower
        {
            get
            {
                return mIdentifierLower;
            }
        }

        public ASTNode_ArgList ArgList
        {
            get
            {
                return mArgList;
            }
            set
            {
                mArgList = value;
            }
        }

        public ASTNode_CallExpression(int LineNum, string Id, ASTNode_ArgList AL)
            : base(ASTNode.ASTType.Call, LineNum)
        {
            Identifier = Id;
            ArgList = AL;
        }
    }

    public class ASTNode_ForStatement : ASTNode_Statement
    {
        private string mIdentifier = String.Empty;
        private string mIdentifierLower = String.Empty;
        private ASTNode_Expression mFromExpression = null;
        private ASTNode_Expression mToExpression = null;
        private ASTNode_Expression mStepExpression = null;
        private ASTNode_StatementList mExecBlock = null;

        public string Identifier
        {
            get
            {
                return mIdentifier;
            }
            set
            {
                mIdentifier = value;
                mIdentifierLower = mIdentifier.ToLower();
            }
        }

        public string IdentifierLower
        {
            get
            {
                return mIdentifierLower;
            }
        }

        public ASTNode_Expression FromExpression
        {
            get
            {
                return mFromExpression;
            }
            set
            {
                mFromExpression = value;
            }
        }

        public ASTNode_Expression ToExpression
        {
            get
            {
                return mToExpression;
            }
            set
            {
                mToExpression = value;
            }
        }

        public ASTNode_Expression StepExpression
        {
            get
            {
                return mStepExpression;
            }
            set
            {
                mStepExpression = value;
            }
        }

        public ASTNode_StatementList ExecBlock
        {
            get
            {
                return mExecBlock;
            }
            set
            {
                mExecBlock = value;
            }
        }

        public ASTNode_ForStatement(int LineNum)
            : base(ASTNode.ASTType.ForStatement, LineNum) { }
    }

    public class ASTNode_ArgList : ASTNode
    {
        private List<ASTNode_Expression> mArgs = new List<ASTNode_Expression>();

        public IList<ASTNode_Expression> Args
        {
            get
            {
                return mArgs;
            }
        }

        public ASTNode_ArgList()
            : base(ASTNode.ASTType.ArgList) { }
    }

    public class ASTNode_BinaryExpression : ASTNode_Expression
    {
        private BinaryOp _BinaryOperator;
        private ASTNode_Expression _Left;
        private ASTNode_Expression _Right;

        public BinaryOp BinaryOperator
        {
            get
            {
                return _BinaryOperator;
            }
            set
            {
                _BinaryOperator = value;
            }
        }

        public ASTNode_Expression Left
        {
            get
            {
                return _Left;
            }
            set
            {
                _Left = value;
            }
        }

        public ASTNode_Expression Right
        {
            get
            {
                return _Right;
            }
            set
            {
                _Right = value;
            }
        }

        public ASTNode_BinaryExpression(int LineNum, BinaryOp Opt, ASTNode_Expression LeftNode, ASTNode_Expression RightNode)
            : base(ASTNode.ASTType.BinaryExpression, LineNum)
        {
            BinaryOperator = Opt;
            Left = LeftNode;
            Right = RightNode;
        }
    }

    public class ASTNode_UnaryExpression : ASTNode_Expression
    {
        private UnaryOp _UnaryOperator;
        private ASTNode_Expression _Right;

        public UnaryOp UnaryOperator
        {
            get
            {
                return _UnaryOperator;
            }
            set
            {
                _UnaryOperator = value;
            }
        }

        public ASTNode_Expression Right
        {
            get
            {
                return _Right;
            }
            set
            {
                _Right = value;
            }
        }

        public ASTNode_UnaryExpression(int LineNum, UnaryOp Opt, ASTNode_Expression RightNode)
            : base(ASTNode.ASTType.UnaryExpression, LineNum)
        {
            UnaryOperator = Opt;
            Right = RightNode;
        }
    }


    public class ASTNode_DigitLiteral : ASTNode_Expression
    {
        private double _Value;

        public double Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        public ASTNode_DigitLiteral(int LineNum, double Val)
            : base(ASTNode.ASTType.DigitLiteral, LineNum)
        {
            Value = Val;
        }
    }

    public class ASTNode_SymbolExpression : ASTNode_Expression
    {
        private string _Identifier = String.Empty;
        private string _IdentifierLower = String.Empty;

        public string Identifier
        {
            get
            {
                return _Identifier;
            }
            set
            {
                _Identifier = value;
                _IdentifierLower = _Identifier.ToLower();
            }
        }

        public string IdentifierLower
        {
            get
            {
                return _IdentifierLower;
            }
        }

        public ASTNode_SymbolExpression(int LineNum, string Id)
            : base(ASTNode.ASTType.SymbolExpression, LineNum)
        {
            Identifier = Id;
        }
    }

    public class ASTNode_TupleExpression : ASTNode_Expression
    {
        private List<ASTNode_Expression> _Args = new List<ASTNode_Expression>();

        public IList<ASTNode_Expression> Args
        {
            get
            {
                return _Args;
            }
        }

        public ASTNode_TupleExpression(int LineNum)
            : base(ASTNode.ASTType.TupleExpression, LineNum) { }
    }

    class AST
    {
    }
}
