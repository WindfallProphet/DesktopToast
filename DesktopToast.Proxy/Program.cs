using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DesktopToast.Proxy
{
	class HttpServer
	{
		public static HttpListener listener;
		public static string url = "http://localhost:8000/";
	
		public static async Task HandleIncomingConnections()
		{
			bool runServer = true;

			// While a user hasn't visited the `shutdown` url, keep on handling requests
			while (runServer)
			{
				// Will wait here until we hear from a connection
				HttpListenerContext ctx = await listener.GetContextAsync();

				// Peel out the requests and response objects
				HttpListenerRequest req = ctx.Request;
				HttpListenerResponse resp = ctx.Response;

				//Receive Json
				var context = listener.GetContext();
				var request = context.Request;
				string json;
				using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
				{
					json = reader.ReadToEnd();
				}

				Console.WriteLine(json);
				ToastManager.ShowAsync(json)
				.ContinueWith(result => Console.WriteLine(result.Result))
				.Wait();

			}
		}
		class Program
		{
			static void Main(string[] args)
			{
				// Create a Http server and start listening for incoming connections
				listener = new HttpListener();
				listener.Prefixes.Add(url);
				listener.Start();
				Console.WriteLine("Listening for connections on {0}", url);

				// Handle requests
				Task listenTask = HandleIncomingConnections();
				listenTask.GetAwaiter().GetResult();

				// Close the listener
				listener.Close();
				var requestString = args.Any() ? args[0] : null;
				if (requestString == null)
				{
				#if DEBUG
					requestString = @"{
					""ShortcutFileName"":""DesktopToast.Proxy.lnk"",
					""ShortcutTargetFilePath"":""C:\\DesktopToast.Proxy.exe"",
					""ToastTitle"":""DesktopToast Proxy Sample"",
					""ToastBody"":""This is a toast test."",
					""AppId"":""DesktopToast.Proxy"",
					}";
				#endif
				#if !DEBUG
					return;
				#endif
				}

				ToastManager.ShowAsync(requestString)
				.ContinueWith(result => Console.WriteLine(result.Result))
				.Wait();
			}
		}
	}
}