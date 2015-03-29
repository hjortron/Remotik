using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

namespace Server
{
    class Server
    {
        public static IDisposable SignalR { get; set; }

        public static void StartServer(string serverName)
        {
            try
            {
                SignalR = WebApp.Start($"http://{serverName}:8080");
            }
            catch (TargetInvocationException)
            {
                //Dispatcher.Invoke(() =>
                //((MainWindow)Application.Current.MainWindow).WriteToConsole("A server is already running at " + serverName);
                //Dispatcher.Invoke(() => ((MainWindow)Application.Current.MainWindow).buttonStart.IsEnabled = true);
                return;
            }
            //Dispatcher.Invoke(() => .buttonStop.IsEnabled = true));
            //Dispatcher.Invoke(() =>
            //((MainWindow)Application.Current.MainWindow).WriteToConsole("Server started at " + serverName));
        }             
    }
    /// <summary>
    /// Used by OWIN's startup process. 
    /// </summary>
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
    /// <summary>
    /// Echoes messages sent using the Send message by calling the
    /// addMessage method on the client. Also reports to the console
    /// when clients connect and disconnect.
    /// </summary>
    public class MyHub : Hub
    {
        public void Send(byte message)
        {
            Clients.All.addMessage(message);
        }

        public void SendKey(byte message)
        {
            Clients.All.addMessage(message);
        }
        public override Task OnConnected()
        {
            //Use Application.Current.Dispatcher to access UI thread from outside the MainWindow class
            Console.WriteLine("Client connected: " + Context.ConnectionId);

            return base.OnConnected();
        }
        public void OnDisconnected()
        {
            //Use Application.Current.Dispatcher to access UI thread from outside the MainWindow class
            Console.WriteLine("Client disconnected: " + Context.ConnectionId);
        }
    }
}
