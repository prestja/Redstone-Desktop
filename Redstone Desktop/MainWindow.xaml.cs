using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Documents;
using prestja;
using System.Windows.Controls;
using System.IO;

namespace prestja
{
    public partial class MainWindow : Window
    {
        private static TcpClient Client;
        private static bool Connected = false;
        private static string[] ConnectedPlayers;
        private static string Address = "127.0.0.1";
        private static int Port = 25566;

        public MainWindow()
        {
            // initializing the user interface
            InitializeComponent();
            localConsole.IsReadOnly = true;
            // load credentials from file, if they exist
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Redstone Desktop\\login.dat");
            FileInfo file = new FileInfo(fileName);
            try
            {
                file.Directory.Create();
                if (file.Exists)
                {
                    FileStream stream = file.OpenRead();
                    string line = File.ReadAllLines(fileName)[0];
                    string[] sep = line.Split(' ');
                    Address = sep[0];
                    int.TryParse(sep[1], out Port);
                    field_password.Password = sep[2];
                }
                else
                {
                    file.Create();
                }
            }
            catch (Exception e)
            {
                Log(e.Message, Minecraft.error);
            }
            field_port.Text = "" + Port;
            field_address.Text = Address;
        }

        private void Log(string line, Color color) 
        {
            Paragraph para = new Paragraph(new Run(line));
            para.Foreground = new SolidColorBrush(color);
            para.Margin = new Thickness(0);
            localConsole.Document.Blocks.Add(para);
            localConsole.ScrollToEnd();
        }

        private void UpdatePlayerList() 
        {
            playerList.Items.Clear();
            for (int i = 0; i < ConnectedPlayers.Length; i++) {
                playerList.Items.Add(ConnectedPlayers[i]);
            }
        }

        private async void send_Click(object sender, RoutedEventArgs e) 
        {
            try
            {
                string response = await SendCommandAsync(input.Text);
                Console.WriteLine(response);
            } catch (Exception ex)
            {
                
            }
        }

        private void EnterKeyHandler(object sender, KeyEventArgs e) 
        {
            if (e.Key == Key.Return) {
                Command(input.Text);
                input.Text = "";
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void button_connect_Click(object sender, RoutedEventArgs e)
        {
            Address = field_address.Text;
            try
            {
                Int32.TryParse(field_port.Text, out Port);
            } catch (Exception ex)
            {
                Log("Malformed port", Minecraft.error);
                return;
            }
            try
            {
                Client = new TcpClient(Address, Port);
            } catch (SocketException ex)
            {
                Log(ex.Message, Minecraft.error);
                return;
            }
            AttemptConnection();
        }
    }
}
