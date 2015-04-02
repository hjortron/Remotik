using System;
using System.Net;
using System.Net.Http;
using System.Windows;
using Microsoft.AspNet.SignalR.Client;

namespace WPFClient
{   
    /// <summary>
    /// SignalR client hosted in a WPF application. The client
    /// lets the user pick a user name, connect to the server asynchronously
    /// to not block the UI thread, and send chat messages to all connected 
    /// clients whether they are hosted in WinForms, WPF, or a web application.
    /// For simplicity, MVVM will not be used for this sample.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// This name is simply added to sent messages to identify the user; this 
        /// sample does not include authentication.
        /// </summary>

        private const byte VkVolumeMute = 0xAD; //Volume Mute key
        private const byte VkVolumeUp = 0xAF; //Volume Up key
        private const byte VkVolumeDown = 0xAE; //Volume Down key
        private const byte VkMediaNextTrack = 0xB0; //Next Track key
        private const byte VkMediaPrevTrack = 0xB1; //Previous Track key
        private const byte VkMediaStop = 0xB2; //Stop Media key
        private const byte VkMediaPlayPause = 0xB3; //Play/Pause Media key

        public string UserName { get; set; }
        public IHubProxy HubProxy { get; set; }
     
        public HubConnection Connection { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("Send", UserName, TextBoxMessage.Text);
            TextBoxMessage.Text = string.Empty;
            TextBoxMessage.Focus();
        }

        /// <summary>
        /// Creates and connects the hub connection and hub proxy. This method
        /// is called asynchronously from SignInButton_Click.
        /// </summary>
        private async void ConnectAsync(string serverIp)
        {          
            Connection = new HubConnection($"http://{serverIp}:8080");
            Connection.Closed += Connection_Closed;
            HubProxy = Connection.CreateHubProxy("MyHub");
            //Handle incoming event from server: use Invoke to write to console from SignalR's thread
            HubProxy.On<string, string>("AddMessage", (name, message) =>
                Dispatcher.Invoke(() =>
                    RichTextBoxConsole.AppendText($"{name}: {message}\r")
                )
            );
            try
            {
                await Connection.Start();
            }
            catch (HttpRequestException)
            {
                StatusText.Content = "Unable to connect to server: Start server before connecting clients.";
                //No connection: Don't enable Send button or show chat UI
                return;
            }

            //Show chat UI; hide login UI
            SignInPanel.Visibility = Visibility.Collapsed;
            ChatPanel.Visibility = Visibility.Visible;
            ButtonSend.IsEnabled = true;
            TextBoxMessage.Focus();
            RichTextBoxConsole.AppendText("Connected to server at " + serverIp + "\r");
        }

        /// <summary>
        /// If the server is stopped, the connection will time out after 30 seconds (default), and the 
        /// Closed event will fire.
        /// </summary>
        void Connection_Closed()
        {
            //Hide chat UI; show login UI
            var dispatcher = Application.Current.Dispatcher;
            dispatcher.Invoke(() => ChatPanel.Visibility = Visibility.Collapsed);
            dispatcher.Invoke(() => ButtonSend.IsEnabled = false);
            dispatcher.Invoke(() => StatusText.Content = "You have been disconnected.");
            dispatcher.Invoke(() => SignInPanel.Visibility = Visibility.Visible);
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            var hostEntry = Dns.GetHostEntry(UserNameTextBox.Text);
            if (hostEntry.AddressList.Length == 0) return;
            var ip = hostEntry.AddressList[2].ToString();        
            //Connect to server (use async method to avoid blocking UI thread)
            if (string.IsNullOrEmpty(ip)) return;
            StatusText.Visibility = Visibility.Visible;
            StatusText.Content = "Connecting to server...";
            ConnectAsync(ip);
        }

        private void WPFClient_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Connection == null) return;
            Connection.Stop();
            Connection.Dispose();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("Send", VkMediaPrevTrack);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("Send", VkMediaStop);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("Send", VkMediaPlayPause);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            HubProxy.Invoke("Send", VkMediaNextTrack);
        }
    }
}
