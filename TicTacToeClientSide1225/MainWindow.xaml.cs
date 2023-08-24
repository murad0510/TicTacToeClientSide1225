using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TicTacToeClientSide1225
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int port = 27001;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            ConnectToServer();
            RequestLoop();
        }

        private void RequestLoop()
        {
            var receiver = Task.Run(() =>
            {
                while (true)
                {
                    ReceiveResponse();
                }
            });
        }

        public bool IsEnable { get; set; }
        private void ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);
            App.Current.Dispatcher.Invoke(() =>
            {
                if (text == "True")
                {
                    //MessageBox.Show("Sira x de di");

                    IsEnable = true;
                    b1.IsEnabled = IsEnable;
                    b2.IsEnabled = IsEnable;
                    b3.IsEnabled = IsEnable;
                    b4.IsEnabled = IsEnable;
                    b5.IsEnabled = IsEnable;
                    b6.IsEnabled = IsEnable;
                    b7.IsEnabled = IsEnable;
                    b8.IsEnabled = IsEnable;
                    b9.IsEnabled = IsEnable;
                }
                else if (text == "False")
                {
                    //MessageBox.Show("Sira o de di");

                    IsEnable = false;
                    b1.IsEnabled = IsEnable;
                    b2.IsEnabled = IsEnable;
                    b3.IsEnabled = IsEnable;
                    b4.IsEnabled = IsEnable;
                    b5.IsEnabled = IsEnable;
                    b6.IsEnabled = IsEnable;
                    b7.IsEnabled = IsEnable;
                    b8.IsEnabled = IsEnable;
                    b9.IsEnabled = IsEnable;
                }
                if (text == "O")
                {
                    MessageBox.Show("Start game");
                }
            });

            //if (text != "O" && text != "True" && text != "False")
            //{
            IntegrateToView(text);
            //}
        }

        private void IntegrateToView(string text)
        {
            try
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var data = text.Split('\n');
                    var row1 = data[0].Split('\t');
                    var row2 = data[1].Split('\t');
                    var row3 = data[2].Split('\t');

                    b1.Content = row1[0];
                    b2.Content = row1[1];
                    b3.Content = row1[2];

                    b4.Content = row2[0];
                    b5.Content = row2[1];
                    b6.Content = row2[2];

                    b7.Content = row3[0];
                    b8.Content = row3[1];
                    b9.Content = row3[2];
                });
            }
            catch (Exception)
            {
            }
        }
        public static string Text { get; set; }
        private void ConnectToServer()
        {
            while (!ClientSocket.Connected)
            {
                try
                {
                    ClientSocket.Connect(IPAddress.Parse("192.168.0.110"), port);
                }
                catch (Exception)
                {
                }
            }

            MessageBox.Show("Connected to game");

            //Task.Run(() =>
            //{
            var buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer);
            if (received == 0) return;

            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);

            //MessageBox.Show(text);


            if (text == "O")
            {
                MessageBox.Show("Start game");
            }


            Text = text;


            //});

            this.Title = "Player : " + Text;

            this.player.Text = this.Title;

            //try
            //{
            //    this.Title = "Player : " + texts[2];
            //    this.player.Text = this.Title;
            //}
            //catch (Exception)
            //{
            //}

        }

        private void b1_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var bt = sender as Button;
                    string request = bt.Content.ToString() + player.Text.Split(' ')[2];
                    SendString(request);
                });
            });
        }

        private void SendString(string request)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(request);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }
    }
}
