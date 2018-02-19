using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ice.lang
{
    /// <summary>
    /// Lexer
    /// </summary>
    public class Lexer
    {
        private TextReader Reader = null;
        private int BufNext = -1;

        private Token CurToken = Token.EOF;
        private double CurDigit = 0;
        private string CurId = String.Empty;
        private string CurIdLower = String.Empty;   // format string to lower
        private int position = 0;
        private int line = 1;
        private int row = 0;

        /// <summary>
        /// current Token
        /// </summary>
        public Token CurrentToken
        {
            get
            {
                return CurToken;
            }
        }

        /// <summary>
        /// Current Digit
        /// </summary>
        public double CurrentDigit
        {
            get
            {
                return CurDigit;
            }
        }

        /// <summary>
        /// Current Identify
        /// </summary>
        public string CurrentIdentify
        {
            get
            {
                return CurId;
            }
        }

        /// <summary>
        /// Current Identify Lower
        /// </summary>
        public string CurrentIdentifyLower
        {
            get
            {
                return CurIdLower;
            }
        }

        /// <summary>
        /// get position
        /// </summary>
        public int Position
        {
            get
            {
                return position;
            }
        }

        /// <summary>
        /// get line
        /// </summary>
        public int Line
        {
            get
            {
                return line;
            }
        }

        /// <summary>
        /// get row
        /// </summary>
        public int Row
        {
            get
            {
                return row;
            }
        }

        /// <summary>
        /// enum Token
        /// </summary>
        public enum Token
        {
            EOF,             // EOF
            Semico,          // ;
            LeftBracket,     // (
            RightBracket,    // )
            LeftBrace,       // {
            RightBrace,      // }
            Comma,           // ,
            DigitLiteral,    // digit
            Identifier,      // identifier

            // operator
            Plus = 50,       // +
            Minus,           // -
            Mul,             // *
            Div,             // /
            Power,           // **

            // keyword
            Is = 100,
            For,
            From,
            To,
            Step
        }

        /// keyword list
        private static readonly string[] KeywordList = new string[]
        {
            "is", "for", "from", "to", "step"
        };

        /// <summary>
        /// format token
        /// </summary>
        /// <param name="TokenToFormat">token need to be formatted.</param>
        /// <returns>string</returns>
        public static string FormatToken(Token TokenToFormat)
        {
            switch (TokenToFormat)
            {
                case Token.EOF:
                    return "<EOF>";
                case Token.Semico:
                    return "';'";
                case Token.LeftBracket:
                    return "'('";
                case Token.RightBracket:
                    return "')'";
                case Token.LeftBrace:
                    return "'{'";
                case Token.RightBrace:
                    return "'}'";
                case Token.Comma:
                    return "','";
                case Token.Plus:
                    return "'+'";
                case Token.Minus:
                    return "'-'";
                case Token.Mul:
                    return "'*'";
                case Token.Div:
                    return "'/'";
                case Token.Power:
                    return "'**'";
                case Token.DigitLiteral:
                    return "<digit>";
                case Token.Identifier:
                    return "<identifier>";
                default:
                    if (TokenToFormat >= Token.Is && (int)TokenToFormat < (int)Token.Is + KeywordList.Length)
                        return String.Format("<{0}>", KeywordList[TokenToFormat - Token.Is]);
                    else
                        return "<unknown>";
            }
        }

        public string FormatCurrentToken()
        {
            switch (CurToken)
            {
                case Token.EOF:
                    return "<EOF>";
                case Token.Semico:
                    return "';'";
                case Token.LeftBracket:
                    return "'('";
                case Token.RightBracket:
                    return "')'";
                case Token.LeftBrace:
                    return "'{'";
                case Token.RightBrace:
                    return "'}'";
                case Token.Comma:
                    return "','";
                case Token.Plus:
                    return "'+'";
                case Token.Minus:
                    return "'-'";
                case Token.Mul:
                    return "'*'";
                case Token.Div:
                    return "'/'";
                case Token.Power:
                    return "'**'";
                case Token.DigitLiteral:
                    return CurDigit.ToString();
                case Token.Identifier:
                    return "\"" + CurId.ToString() + "\"";
                default:
                    if (CurToken >= Token.Is && (int)CurToken < (int)Token.Is + KeywordList.Length)
                        return String.Format("<{0}>", KeywordList[CurToken - Token.Is]);
                    else
                        return "<unknown>";
            }
        }

        private static string FormatCharacter(int c)
        {
            if (c == -1)
                return "\\0";
            else if (Char.IsControl((char)c))
                return "\\x" + ((short)c).ToString("X");
            else
                return ((char)c).ToString();
        }


        private static bool IsIdentifyStart(int c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || c >= 128;
        }

        private static bool IsIdentifyCharacter(int c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || (c >= '0' && c <= '9') || c >= 128;
        }

        private int ReadNext()
        {
            int t = BufNext;
            if(t != -1)
            {
                if(t == '\n')
                {
                    line++;
                    row = 0;
                }
                position++;
            }
            BufNext = Reader.Read();
            return t;
        }

        /// <summary>
        /// parse digit
        /// </summary>
        /// <returns>digit(double)</returns>
        private double ParseDigit()
        {
            int c;
            double ret = 0;

            while(true)
            {
                c = ReadNext();
                ret = ret * 10;
                ret += c - '0';
                c = BufNext;
                if (!(c >= '0' && c <= '9'))
                    break;
            }

            if(BufNext == '.')
            {
                ReadNext();
                double dec = 0;
                double pow = 1;
                while(true)
                {
                    c = BufNext;
                    if (!(c >= '0' && c <= '9'))
                        break;
                    c = ReadNext();
                    pow *= 0.1;
                    dec += pow * (c - '0');
                }
                ret += dec;
            }
            c = BufNext;
            if (IsIdentifyCharacter(c))
                throw new LexcialException(position, line, row,
                    String.Format("not a valid following charcater '{0}'.", FormatCharacter(c)));
            return ret;
        }

        /// <summary>
        /// parse next one character
        /// </summary>
        public void Next()
        {
            while (true)
            {
                int c = BufNext;
                if (c == ' ' || c == '\n' || c == '\r' || c == '\t')
                {
                    ReadNext();
                }
                // parse annotation
                else if ((c == '/' && Reader.Peek() == '/') || (c == '-' && Reader.Peek() == '-'))
                {
                    while (!(c == '\n' || c == -1))
                        c = ReadNext();
                }
                else
                    break;
            }

            int ct = BufNext;
            switch (ct)
            {
                case -1:
                    CurToken = Token.EOF;
                    return;
                case ';':
                    ReadNext();
                    CurToken = Token.Semico;
                    return;
                case '(':
                    ReadNext();
                    CurToken = Token.LeftBracket;
                    return;
                case ')':
                    ReadNext();
                    CurToken = Token.RightBracket;
                    return;
                case '{':
                    ReadNext();
                    CurToken = Token.LeftBrace;
                    return;
                case '}':
                    ReadNext();
                    CurToken = Token.RightBrace;
                    return;
                case ',':
                    ReadNext();
                    CurToken = Token.Comma;
                    return;
                case '+':
                    ReadNext();
                    CurToken = Token.Plus;
                    return;
                case '-':
                    ReadNext();
                    CurToken = Token.Minus;
                    return;
                case '*':
                    ReadNext();
                    if (BufNext == '*')
                    {
                        ReadNext();
                        CurToken = Token.Power;
                    }
                    else
                        CurToken = Token.Mul;
                    return;
                case '/':
                    ReadNext();
                    CurToken = Token.Div;
                    return;
                default:
                    if(ct >= '0' && ct <= '9')
                    {
                        CurToken = Token.DigitLiteral;
                        CurDigit = ParseDigit();
                        return;
                    }
                    else if(IsIdentifyStart(ct))
                    {
                        CurToken = Token.Identifier;

                        StringBuilder t = new StringBuilder();
                        while(IsIdentifyCharacter(ct))
                        {
                            ReadNext();
                            t.Append((char)ct);
                            ct = BufNext;
                        }
                        CurId = t.ToString();
                        CurIdLower = CurId.ToLower();

                        for(int i = 0; i < KeywordList.Length; ++i)
                        {
                            if(KeywordList[i] == CurIdLower)
                            {
                                CurToken = Token.Is + i;
                                break;
                            }
                        }
                        return;
                    }
                    else
                    {
                        throw new LexcialException(position, line, row + 1,
                            String.Format("unexpected character '{0}'.", FormatCharacter(ct)));
                    }
            }
        }

        public Lexer(TextReader Reader)
        {
            this.Reader = Reader;
            ReadNext();
        }
    }

    public class LexcialException : Exception
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

        public LexcialException(int Position, int Line, int Row, string Description)
            : base(CombineException(Position, Line, Row, Description))
        {
            position = Position;
            line = Line;
            row = Row;
            description = Description;
        }
    }
}
