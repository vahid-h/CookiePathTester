using System;
using System.Net;

namespace CookiePathTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var listenerA = new HttpListener();
            listenerA.Prefixes.Add("http://localhost:54545/A/");
            var listenerB = new HttpListener();
            listenerB.Prefixes.Add("http://localhost:54545/B/");
            var listenerC = new HttpListener();
            listenerC.Prefixes.Add("http://localhost:54545/C/");

            Console.WriteLine("Starting listener A...");
            listenerA.Start();

            Console.WriteLine("Starting listener B...");
            listenerB.Start();

            Console.WriteLine("Starting listener C...");
            listenerC.Start();

            ListenA(listenerA);
            ListenB(listenerB);
            ListenC(listenerC);

            Console.WriteLine("---CallA---");
            CallA();
            Console.WriteLine("---CallB---");
            CallB();
            Console.WriteLine("---CallC---");
            CallC();


            Console.WriteLine("Hit enter to exit.");
            Console.ReadLine();

            listenerA.Stop();
            listenerB.Stop();
            listenerC.Stop();
        }

        // Sets a cookie with path /A/ and returns 200.
        static async void ListenA(HttpListener listener)
        {
            while (true)
            {
                var context = await listener.GetContextAsync();
                Console.WriteLine(@"[ListenerA] Received a request. Setting cookie with path /A/.");
                context.Response.AppendCookie(new Cookie
                {
                    Name = "testNameA",
                    Value = "testValueA",
                    Domain = "localhost",
                    Path = "/A/",
                    Expires = DateTime.UtcNow.AddHours(1)
                });
                context.Response.StatusCode = 200;
                context.Response.Close();
            }
        }

        // Sets a cookie with path /C/ and returns 200.
        static async void ListenB(HttpListener listener)
        {
            while (true)
            {
                var context = await listener.GetContextAsync();
                Console.WriteLine("[ListenerB] received a request. Setting cookie with path /C/.");
                context.Response.AppendCookie(new Cookie
                {
                    Name = "testNameC",
                    Value = "testValueC",
                    Domain = "localhost",
                    Path = "/C/",
                    Expires = DateTime.UtcNow.AddHours(1)
                });
                context.Response.StatusCode = 200;
                context.Response.Close();
            }
        }

        // Prints all cookies received and returns 200.
        static async void ListenC(HttpListener listener)
        {
            while (true)
            {
                var context = await listener.GetContextAsync();
                Console.WriteLine("[ListenerC] Received a request. Printing cookies and returning 200.");
                foreach (Cookie cook in context.Request.Cookies)
                {
                    Console.WriteLine("{0}:{1}", cook.Name, cook.Value);
                }
                context.Response.StatusCode = 200;
                context.Response.Close();
            }
        }

        static void CallA()
        {
            var cookCont = new CookieContainer();
            var req = (HttpWebRequest)WebRequest.Create("http://localhost:54545/A/");
            req.CookieContainer = cookCont;

            Console.WriteLine("[Client] Sending a request to ListenerA.");
            var resp = (HttpWebResponse)req.GetResponse();

            Console.WriteLine("[Client] Got response back. Dumping cookie container name/value pairs for uri http://localhost:54545/A/:");
            var cookies = cookCont.GetCookies(new Uri("http://localhost:54545/A/"));
            foreach (Cookie cook in cookies)
            {
                Console.WriteLine("{0}:{1}", cook.Name, cook.Value);
            }
        }

        static void CallB()
        {
            var cookCont = new CookieContainer();
            var req = (HttpWebRequest)WebRequest.Create("http://localhost:54545/B/");
            req.CookieContainer = cookCont;

            Console.WriteLine("[Client] Sending a request to ListenerB");
            var resp = (HttpWebResponse)req.GetResponse();

            Console.WriteLine("[Client] Got response back. Dumping cookie container name/value pairs for uri http://localhost:54545/C/:");
            var cookies = cookCont.GetCookies(new Uri("http://localhost:54545/C/"));
            foreach (Cookie cook in cookies)
            {
                Console.WriteLine("{0}:{1}", cook.Name, cook.Value);
            }
        }

        static void CallC()
        {
            var cookCont = new CookieContainer();
            var req = (HttpWebRequest)WebRequest.Create("http://localhost:54545/C/");
            req.CookieContainer = cookCont;

            Console.WriteLine("[Client] Sending a request to ListenerB");
            var resp = (HttpWebResponse)req.GetResponse();
            Console.WriteLine("[Client] Got response back.");
        }
    }
}
