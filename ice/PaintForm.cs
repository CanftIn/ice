using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ice
{
    public partial class PaintForm : Form
    {
        public PaintForm()
        {
            InitializeComponent();
        }

        private string _DocumentTitle = String.Empty;
        private string _SourceCode = String.Empty;

        // 编译执行线程
        private Thread _WorkThread = null;

        // AST树
        private lang.ASTNode_StatementList _AST = null;

        // 初始化控件状态
        private void initState()
        {
            pictureBox_result.Width = pictureBox_result.BackgroundImage.Width;
            pictureBox_result.Height = pictureBox_result.BackgroundImage.Height;
        }

        private void writeLog(string str)
        {
            Console.WriteLine(str);
        }

        // 工作线程
        private void workThreadJob()
        {
            Stopwatch tWatch = new Stopwatch();

            // 初始化状态
            this.Invoke((Action)initState);

            // 检查AST树是否已生成
            if (_AST != null)
                writeLog("忽略解析过程");
            else
            {
                writeLog("正在执行解析过程...");

                // 解析代码
                bool bCompileSucceed = false;
                using (StringReader tReader = new StringReader(_SourceCode))
                {
                    tWatch.Start();
                    try
                    {
                        lang.Lexer tLexer = new lang.Lexer(tReader);  // 初始化Lexer
                        _AST = lang.Syntax.Parse(tLexer);  // 解析
                        bCompileSucceed = true;
                    }
                    catch (lang.LexcialException e)
                    {
                        writeLog("词法错误");
                    }
                    catch (lang.SyntaxException e)
                    {
                        writeLog("语法错误");
                    }
                    catch (Exception e)
                    {
                        writeLog("一般性错误");
                    }
                    tWatch.Stop();
                    if (bCompileSucceed)
                        writeLog("解析成功，耗时：{0} 秒");
                    else
                        writeLog("解析失败，耗时：{0} 秒");
                }
            }

            if (_AST != null)
            {
                // 执行语法树
                PaintRuntime tRT = new PaintRuntime();
                tRT.OnOutputText += delegate (PaintRuntime sender, string Content)
                {
                    //writeOutputText(Content + "\r\n");
                };
                tRT.OnRuntimeException += delegate (PaintRuntime sender, lang.RuntimeException e)
                {
                    writeLog("运行时错误");
                };

                // 执行
                writeLog("正在执行...");
                tWatch.Start();
                tRT.RunAST(_AST);
                tWatch.Stop();
                writeLog("执行完毕，耗时秒");

                // 设置图片
                this.Invoke((Action)delegate () {
                    pictureBox_result.BackgroundImage = Resource.Icon_Busy;//tRT.TargetBuffer;
                    pictureBox_result.Width = pictureBox_result.BackgroundImage.Width;
                    pictureBox_result.Height = pictureBox_result.BackgroundImage.Height;
                });
            }
        }


        /// <summary>
        /// 提交源代码
        /// </summary>
        /// <param name="SourceCode">源代码</param>
        public void SubmitSourceCode(string SourceCode)
        {
            if (_WorkThread != null && _WorkThread.IsAlive)
            {
                MessageBox.Show(this, "请先结束当前任务然后继续。", "正忙", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (SourceCode != _SourceCode)
            {
                _SourceCode = SourceCode;
                _AST = null;
            }

            _WorkThread = new Thread(new ThreadStart(workThreadJob));
            _WorkThread.IsBackground = true;
            _WorkThread.Start();
        }
    }
}
