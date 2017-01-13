using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
namespace Worker
{
    public class Program
    {
        static Random rand = new Random();
        static HttpClient httpclient = new HttpClient();
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
        public static async Task MainAsync(string[] args)
        {
            while(true)
            {
                try
                {
                    var bytes = await httpclient.GetByteArrayAsync("http://gen/8");
                    var results = await httpclient.PostAsync("http://hashr/hashme",  new ByteArrayContent(bytes));
                    results.EnsureSuccessStatusCode();
                    var hashResults = await results.Content.ReadAsStringAsync();
                    await httpclient.GetStringAsync($"http://store/store");
                    await Task.Delay(rand.Next(100, 1000)); //emulate some level of processing time
                }
                catch(Exception e)
                {
                    Console.WriteLine("WORKER FAILED");
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
