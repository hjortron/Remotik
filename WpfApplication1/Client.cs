using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace WpfApplication1
{
    class Client
    {
        public static HubConnection Connection { get; set; }
        public static IHubProxy HubProxy { get; set; }

        public static async void ConnectAsync(string serverName)
        {
            Connection = new HubConnection($"http://{serverName}:8080");
            Connection.Closed += Connection_Closed;
            HubProxy = Connection.CreateHubProxy("MyHub");
            //Handle incoming event from server: use Invoke to write to console from SignalR's thread
            HubProxy.On<byte>("AddMessage", Press);
            try
            {
                await Connection.Start();
            }
            catch (HttpRequestException)
            {
                ((MainWindow)Application.Current.MainWindow).WriteToConsole("Unable to connect to server: Start server before connecting clients.");
                //No connection: Don't enable Send button or show chat UI
                return;
            }

            //Show chat UI; hide login UI           
            ((MainWindow)Application.Current.MainWindow).WriteToConsole("Connected to server at " + serverName + "\r");
        }

        private static void Connection_Closed()
        {
            //Hide chat UI; show login UI
            var dispatcher = Application.Current.Dispatcher;
            dispatcher.Invoke(() => ((MainWindow)Application.Current.MainWindow).WriteToConsole("You have been disconnected."));
        }
    }
}
