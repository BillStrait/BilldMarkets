using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using BilldMarkets.Models;
using IEXSharp;
using Nito.AsyncEx;

namespace BilldMarkets
{
    class Program
    {
        private static Profile _profile;
        private static IEXCloudClient _iexClient;

        static void Main(string[] args)
        {

            AsyncContext.Run(() => MainAsync(args));
        }


        private static async Task MainAsync(string[] args)
        {
            var program = new Program();

            if (!Initialize(args))
            {
                return;
            }
            var timer = new Timer(1000);

            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        public static async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var output = new List<OutputObject>();
            
            foreach(var stonk in _profile.Stocks)
            {
                var response = await _iexClient.StockPrices.QuoteAsync(stonk.Name);
                stonk.LastValue = stonk.NewValue;
                stonk.NewValue = (decimal)response.Data.latestPrice;
                var oo = new OutputObject();
                if (stonk.LastValue > stonk.NewValue)
                {
                    oo.Color = ConsoleColor.Red;
                }
                else
                {
                    oo.Color = ConsoleColor.Green;
                }
                oo.Text = $"{stonk.Name} - {stonk.LastValue} ";
                output.Add(oo);
            }

            foreach(var crypto in _profile.Cryptos)
            {
                var response = await _iexClient.Crypto.QuoteAsync(crypto.Name);
                crypto.LastValue = crypto.NewValue;
                crypto.NewValue = response.Data.latestPrice;

                var oo = new OutputObject();
                if(crypto.LastValue > crypto.NewValue)
                {
                    oo.Color = ConsoleColor.Red;
                }
                else
                {
                    oo.Color = ConsoleColor.Green;
                }
                oo.Text = $"{crypto.Name} - {crypto.LastValue} ";
                output.Add(oo);
            }

            WriteDashboard(output);

            return;
        }


        public static void WriteDashboard(List<OutputObject> oo)
        {
            Console.Write("\r");
            foreach (var o in oo)
            {
                Console.ForegroundColor = o.Color;
                Console.Write(o.Text);
            }
            Console.ResetColor();
        }




        public static bool Initialize(string[] args)
        {
            bool apiKeyFound = false;
            bool apiSecretFound = false;
            string apiKey = "";
            string apiSecret = "";

            for(var i = 0; i<args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "/apikey":
                        apiKey = args[++i];
                        apiKeyFound = true;
                        break;
                    case "/apisecret":
                        apiSecret = args[++i];
                        apiSecretFound = true;
                        break;
                    default:
                        break;
                }
            }
            
            if(!apiKeyFound || !apiSecretFound)
            {
                Console.WriteLine("You must provide an API Key and API Secret from IEX Cloud. You can sign up for an account for free. If you choose a paid tier, please use my referral link: https://iexcloud.io/s/72dc2734 \n The command must inclide the flags /apiSecret <api secret> /apiKey <api key>");
                Console.ReadLine();
                return false;
            }

            _profile = new Profile();

            _profile.Cryptos.Add(new Crypto { Name = "btcusd", LastValue = 0.00m });
            _profile.Cryptos.Add(new Crypto { Name = "ethusd", LastValue = 0.00m });
            _profile.Cryptos.Add(new Crypto { Name = "bchusd", LastValue = 0.00m });

            _profile.Stocks.Add(new Stock { Name = "gme", LastValue = 0.00m });
            _profile.Stocks.Add(new Stock { Name = "amc", LastValue = 0.00m });

            
            _iexClient = new IEXCloudClient(apiKey, apiSecret, false, false);

            return true;
        }

    }
}
