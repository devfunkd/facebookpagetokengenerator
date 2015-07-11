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
            var appId = "205296092988057";
            var appSecret = "b69186a963a70879d50e934896c80804";
            var accessToken = "CAAC6tziyQpkBAGZCqmnNr3CYW4OQLpjp1hA9MoiKPoDJqnG8dcfG6OphKmi6YyYVZBzhvXgaMtE1ZC717YIrNqgVS8Nd50ZBBWNghp6aSDpg8oazex7Cspzqse5UkKAZA16JRkf0fZAUfhrFEex2CuT4BSTA7RXIcdPZATBHnLHxKmQeFzc8xhSOiZAAa7SyoQdmFw1ZCRYKepMG1VAgUykUoHc4b8VpolFl3xn7s8MR2nQZDZD";

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
