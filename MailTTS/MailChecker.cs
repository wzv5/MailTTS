using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MailTTS
{
    class MailChecker
    {
        public delegate void OnMessageDelegate(string from, string subject);
        public event OnMessageDelegate OnMessage;

        private FileSystemWatcher watcher;
        private DBReader reader;
        private long lastMsgId = -1;
        private string dbfilename = "Store.db";
        private string dbpath;

        public MailChecker()
        {
            dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mailbird", "Store");
            reader = new DBReader(Path.Combine(dbpath, dbfilename));
            watcher = new FileSystemWatcher(dbpath, dbfilename+"*");
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            watcher.Changed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;
        }

        private async void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var msg = await reader.GetLastMessage();
            if (msg != null)
            {
                var (id, sub, from) = msg;
                if (lastMsgId != id)
                {
                    OnMessage(from, sub);
                }
                lastMsgId = id;
            }
        }
    }
}
