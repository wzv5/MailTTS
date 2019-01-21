using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace MailTTS
{
    class DBReader
    {
        private SQLiteConnection db;

        public DBReader(string dbfile)
        {
            db = new SQLiteConnection("Data Source=" + dbfile + ";Read Only=True;");
            db.Open();
        }

        // id, subject, from
        public async Task<IList<Tuple<long, string, string>>> GetLastMessage(long lastid)
        {
            var result = new List<Tuple<long, string, string>>();
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "SELECT Id, Subject, DraftChecksum FROM Messages WHERE IsRead = 0 AND Id > @lastid ORDER BY Id DESC LIMIT 100";
                cmd.Parameters.AddWithValue("lastid", lastid);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var id = reader.GetInt64(0);
                        var sub = reader.GetString(1);

                        // 跳过草稿
                        var draft = reader.GetValue(2);
                        if (draft is Guid)
                        {
                            continue;
                        }

                        string name = await GetFromName(id);
                        result.Add(new Tuple<long, string, string>(id, sub, name));
                    }
                }
            }
            return result;
        }

        private async Task<string> GetFromName(long id)
        {
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "SELECT Name FROM Messages_Contacts WHERE MessageId = @id AND Type = 0 ORDER BY Id DESC";
                cmd.Parameters.AddWithValue("id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        // 姓名字段可能为 NULL
                        if (!await reader.IsDBNullAsync(0))
                        {
                            return reader.GetFieldValue<string>(0);
                        }
                    }
                }
            }
            return "未知姓名";
        }
    }
}
