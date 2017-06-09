using DesktopToast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;

namespace HttpListenerExample
{
	class Proxy
	{
		public static HttpListener listener;
		public static string url = "http://localhost:8000/";

		public static async Task HandleIncomingConnections()
		{
			bool runServer = true;
			// While a user hasn't visited the `shutdown` url, keep on handling requests
			while (runServer)
			{

				var context = listener.GetContext();
				var request = context.Request;
				string text;

				using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
				{
					text = reader.ReadToEnd();
				}

				Console.WriteLine(text);
				ToastManager.ShowAsync(text)
				.ContinueWith(result => Console.WriteLine(result.Result))
				.Wait();

				// Will wait here until we hear from a connection
				//HttpListenerContext ctx = await listener.GetContextAsync();

				// Peel out the requests and response objects
				//HttpListenerRequest req = ctx.Request;
				//HttpListenerResponse resp = ctx.Response;

				// Print out some info about the request
				//Console.WriteLine("Request #: {0}", ++requestCount);
				//Console.WriteLine(req.Url.ToString());
				//Console.WriteLine(req.HttpMethod);
				//Console.WriteLine(req.UserHostName);
				//Console.WriteLine(req.UserAgent);
				//Console.WriteLine();

				// If `shutdown` url requested w/ POST, then shutdown the server after serving the page
				//if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
				//{
				//	Console.WriteLine("Shutdown requested");
				//	runServer = false;
				//}

				// Make sure we don't increment the page views counter if `favicon.ico` is requested
				//if (req.Url.AbsolutePath != "/favicon.ico")
				//	pageViews += 1;

				// Write the response info
				//string disableSubmit = !runServer ? "disabled" : "";
				//byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
				//resp.ContentType = "text/html";
				//resp.ContentEncoding = Encoding.UTF8;
				//resp.ContentLength64 = data.LongLength;

				// Write out to the response stream (asynchronously), then close it
				//await resp.OutputStream.WriteAsync(data, 0, data.Length);
				//resp.Close();
			}
		}
		static void Main(string[] args)
		{
			var requestString = args.Any() ? args[0] : null;

			//if (requestString == null)
			//{ 
			//#if DEBUG
			//	requestString = @"{
			//""ShortcutFileName"":""DesktopToast.Proxy.lnk"",
			//""ShortcutTargetFilePath"":""C:\\DesktopToast.Proxy.exe"",
			//""ToastTitle"":""DesktopToast Proxy Sample"",
			//""ToastBody"":""This is a toast test."",
			//""AppId"":""DesktopToast.Proxy"",
			//}";
			//#endif
			//#if !DEBUG
			//	return;
			//#endif
			//}
		{
		// Create a Http server and start listening for incoming connections
		listener = new HttpListener();
		listener.Prefixes.Add(url);
		listener.Start();
		Console.WriteLine("Listening for connections on {0}", url);

		// Handle requests
		Task listenTask = HandleIncomingConnections();
		listenTask.GetAwaiter().GetResult();


		ToastManager.ShowAsync(requestString)
		.ContinueWith(result => Console.WriteLine(result.Result))
		.Wait();
		// Close the listener
		listener.Close();
		}
		}
	}
}
