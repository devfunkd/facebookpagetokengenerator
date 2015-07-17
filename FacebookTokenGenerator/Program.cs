using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FacebookTokenGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            TokenGenerator();
        }

        private static void TokenGenerator()
        {
            Console.WriteLine("AppId: ");
            //appId = Console.ReadLine();

            Console.WriteLine("AppSecret: ");
            //appSecret = Console.ReadLine();

            Console.WriteLine("AccessToken: ");
            //accessToken = Console.ReadLine();


            // Long-lived Access Token
            var tokenUri = new Uri(
                string.Format(
                    "https://graph.facebook.com/v2.2/oauth/access_token?grant_type=fb_exchange_token&client_id={0}&client_secret={1}&fb_exchange_token={2}", appId, appSecret, accessToken));

            HttpClient client = new HttpClient();
            Task<HttpResponseMessage> response = Task.Run(async () =>
            {
                var res = client.GetAsync(tokenUri).Result;
                return res;
            });
            response.Result.EnsureSuccessStatusCode();

            var test = response.Result.Content.ReadAsStringAsync();
            var output = test.Result.Split(new char[] { '=', '&' })[1];




            Console.WriteLine(output);
        }
    }
}
