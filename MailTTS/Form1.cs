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
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.Caching;

namespace MailTTS
{
    public partial class Form1 : Form
    {
        private MailChecker checker = new MailChecker();
        private delegate void LogDelegate(string msg);
        private LogDelegate AddLog;
        private ConcurrentQueue<string> msgQueue = new ConcurrentQueue<string>();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Task ttsTask;
        private MemoryCache cache;

        public Form1()
        {
            InitializeComponent();

            cache = MemoryCache.Default;

            ttsTask = Task.Run(() => {
                var synthes = new SpeechSynthesizer();
                synthes.Rate = 2;
                while (!cts.Token.IsCancellationRequested)
                {
                    if (msgQueue.TryDequeue(out var msg))
                    {
                        if (!cache.Contains(msg))
                        {
                            // 跳过 10 秒内出现的重复消息
                            cache.Add(msg, msg, DateTimeOffset.Now.AddSeconds(10));
                            synthes.SpeakAsync(msg);
                        }
                    }
                    else
                    {
                        cts.Token.WaitHandle.WaitOne(1000);
                    }
                }
                synthes.SpeakAsyncCancelAll();
            });

            AddLog = Log;
            checker.OnMessage += Checker_OnMessage;
        }

        private void Checker_OnMessage(string from, string subject)
        {
            var s = string.Format("{0} 说：{1}", from, subject);
            this.Invoke(AddLog, s);
            msgQueue.Enqueue(s);
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            cts.Cancel();
            ttsTask.Wait();
        }
    }
}
