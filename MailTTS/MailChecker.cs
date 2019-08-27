using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace MailTTS
{
    class MailChecker
    {
        public delegate void OnMessageDelegate(string from, string subject);
        public event OnMessageDelegate OnMessage;

        private readonly FileSystemWatcher watcher;
        private readonly Timer timer;
        private readonly DBReader reader;
        private long lastMsgId = -1;
        private readonly string dbfilename = "Store.db";
        private readonly string dbpath;

        public MailChecker()
        {
            dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mailbird", "Store");
            reader = new DBReader(Path.Combine(dbpath, dbfilename));

            watcher = new FileSystemWatcher(dbpath, dbfilename + "*")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            watcher.Changed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;

            timer = new Timer(OnTimer, null, 5000, 60000);
        }

        private void CheckNewMessage()
        {
            lock (this)
            {
                var msg = reader.GetLastMessage(lastMsgId).Result;
                foreach (var item in msg)
                {
                    var (id, sub, from) = item;
                    if (id > lastMsgId)
                    {
                        lastMsgId = id;
                    }
                    OnMessage?.Invoke(from, sub);
                }
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            CheckNewMessage();
        }

        private void OnTimer(object sender)
        {
            CheckNewMessage();
        }
    }
}
