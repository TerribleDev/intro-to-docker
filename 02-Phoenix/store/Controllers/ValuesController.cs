using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace store.Controllers
{
    public class ValuesController : Controller
    {
        static ConcurrentDictionary<string, int> db = new ConcurrentDictionary<string, int>();
        [RouteAttribute("")]
        public IEnumerable<Result>  Get()
        {
            return Enumerable
                .Range(0, 60)
                .Select(a=>DateTimeOffset.UtcNow.AddSeconds(-a))
                .Select(a=>db.ContainsKey(a.ToDbKey()) ? new Result(a, db[a.ToDbKey()]) : new Result(a, 0));
        }
        [RouteAttribute("store")]
        public IEnumerable<Result> Store()
        {
            var td = DateTimeOffset.UtcNow;
            if(db.ContainsKey(td.ToDbKey()))
            {
                db[td.ToDbKey()]++;
            }
            else
            {
                db[td.ToDbKey()] = 1;
            }
            return Get();
        }
        [RouteAttribute("db")]
        public object dbResult() => db;
    }
    
    public class Result
    {
        [JsonProperty("date")]
        public string Date { get; } 

        [JsonProperty("value")]
        public int Value { get; }
        public Result(DateTimeOffset time, int valueCount)
        {
            Date = time.ToString("yyyy-MM-dd HH:mm:ss zz");
            this.Value = valueCount;
        }
    }

    public static class ext
    {
        public static string ToDbKey(this DateTimeOffset dt) => dt.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
