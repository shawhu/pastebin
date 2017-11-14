using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace web
{
    public class PasteBinModel
    {
        public static List<PasteBinItem> codesnippets { get; set; }

        public static IEnumerable<PasteBinItem> GetAll()
        {
            if (codesnippets == null)
            {
                codesnippets = new List<PasteBinItem>();
                codesnippets.Add(new PasteBinItem(){
                    codeid = "0",
                    body = "printf('hello world')",
                    timestamp = Common.TimestampNow()
                });
            }
            return codesnippets.OrderByDescending(p=>p.timestamp);
        }
        public static IEnumerable<PasteBinItem> Push(string pastedcode)
        {
            if (codesnippets == null)
            {
                codesnippets = new List<PasteBinItem>();
                codesnippets.Add(new PasteBinItem(){
                    codeid = "0",
                    body = "printf('hello world')",
                    timestamp = Common.TimestampNow()
                });
            }
            codesnippets.Add(new PasteBinItem()
            {
                codeid = (int.Parse(codesnippets[codesnippets.Count-1].codeid)+1).ToString(),
                body = pastedcode,
                timestamp = Common.TimestampNow(),
            });
            return codesnippets.OrderByDescending(p=>p.timestamp);
        }
        public static IEnumerable<PasteBinItem> Clear()
        {
            if (codesnippets==null)
            {
                codesnippets = new List<PasteBinItem>();
                codesnippets.Add(new PasteBinItem(){
                    codeid = "0",
                    body = "printf('hello world')",
                    timestamp = Common.TimestampNow()
                });
            }
            else
            {
                codesnippets.Clear();
                codesnippets.Add(new PasteBinItem(){
                    codeid = "0",
                    body = "printf('hello world')",
                    timestamp = Common.TimestampNow()
                });
            }
            return codesnippets.OrderByDescending(p=>p.timestamp);
        }
    }

    public class PasteBinItem
    {
        public string codeid { get; set; }
        public string body { get; set; }
        public long timestamp { get; set; }
    }
}
