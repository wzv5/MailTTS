using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MailTTS
{
    public partial class Form1 : Form
    {
        private MailChecker checker;
        private delegate void LogDelegate(string msg);
        private LogDelegate AddLog;

        public Form1()
        {
            InitializeComponent();

            AddLog = Log;
            checker = new MailChecker();
            checker.OnMessage += Checker_OnMessage;
        }

        private void Checker_OnMessage(string from, string subject)
        {
            var s = string.Format("{0} 说：{1}", from, subject);
            this.Invoke(AddLog, s);
            var synthes = new SpeechSynthesizer();
            synthes.Rate = 2;
            synthes.SpeakAsync(s);
        }

        private void Log(string msg)
        {
            if (textBox1.TextLength > 60000)
            {
                textBox1.Text = "";
            }
            textBox1.AppendText(string.Format("[{0}] {1}\r\n", DateTime.Now.ToString("MM/dd HH:mm:ss"), msg));
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }
        }
    }
}
