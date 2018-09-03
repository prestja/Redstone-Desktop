using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace prestja {
    public partial class MainWindow : Window {
        private async void AttemptConnection()
        {
            Log("Connecting to server at " + Client.Client.RemoteEndPoint, Minecraft.warning);
            bool success = await ConnectAsync(field_password.Password);
            if (success)
            {
                Log("Connection successful", Minecraft.success);
                GetPlayers();
            }
            else
            {
                Log("Failed to connect to server", Minecraft.error);
            }
        }

        private async void GetPlayers()
        {
            string response = await SendCommandAsync("list");
            int index = response.IndexOf(':');
            response = response.Remove(0, index + 1);
            ConnectedPlayers = response.Split(',');
            for (int i = 0; i < ConnectedPlayers.Length; i++)
            {
                ConnectedPlayers[i] = ConnectedPlayers[i].Trim();
            }
            UpdatePlayerList();
        }

        private async void Command(String message)
        {
            string response = await SendCommandAsync(message);
            Log(response, Minecraft.standard);
        }

        static async Task<bool> ConnectAsync(string password)
        {
            NetworkStream stream = Client.GetStream();
            Packet login = Packet.FormatLogin(password);
            byte[] outgoing = login.Serialize();
            byte[] incoming = new byte[1234];
            if (stream == null)
                return false;
            stream.Write(outgoing, 0, outgoing.Length);
            await stream.ReadAsync(incoming, 0, incoming.Length);
            Packet response = Packet.DeSerialize(incoming);
            if (response.requestID == login.requestID)
            {
                Console.WriteLine("Connection successful");
                Connected = true;
                return Connected;
            }
            else
            {
                Console.WriteLine("Connection failed");
                Connected = false;
                return Connected;
            }
        }

        static async Task<string> SendCommandAsync(string message)
        {
            NetworkStream stream = Client.GetStream();
            Packet request = Packet.Format(PacketType.command, message);
            byte[] outgoing = request.Serialize();
            byte[] incoming = new byte[1234];
            if (stream == null)
                return "";
            stream.Write(outgoing, 0, outgoing.Length);
            await stream.ReadAsync(incoming, 0, incoming.Length);
            Packet response = Packet.DeSerialize(incoming);
            return Encoding.ASCII.GetString(response.payload);
        }
    }
}
// special thanks to https://wiki.vg/RCON