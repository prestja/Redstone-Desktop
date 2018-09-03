using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace prestja {
    public partial class MainWindow : Window {
        private async void AttemptConnection() {
            Log("Connecting to server at " + client.Client.RemoteEndPoint, GameColors.warning);
            bool success = await ConnectAsync(field_password.Password);
            if (success) {
                Log("Connection successful", GameColors.green);
                GetPlayers();
            }
            else {
                Log("Failed to connect to server", GameColors.error);
            }
        }
        private async void Reconnect() {
            throw new NotImplementedException(); // todo: implement
        }
        private async void GetPlayers() {
            string response = await SendCommandAsync("list");
            int index = response.IndexOf(':');
            response = response.Remove(0, index + 1);
            connectedPlayers = response.Split(',');
            for (int i = 0; i < connectedPlayers.Length; i++) {
                connectedPlayers[i] = connectedPlayers[i].Trim();
            }
            UpdatePlayerList();
        }
        private async void GetPlayerNBT(string username) {
            // todo: implement
            throw new NotImplementedException();
        }
        private async void Command(String message) {
            string response = await SendCommandAsync(message);
            Log(response, GameColors.standard);
        }

        static async Task<bool> ConnectAsync(string password) {
            NetworkStream stream = client.GetStream();
            Packet login = Packet.FormatLogin(password);
            byte[] outgoing = login.Serialize();
            byte[] incoming = new byte[1234];
            if (stream == null)
                return false;
            stream.Write(outgoing, 0, outgoing.Length);
            await stream.ReadAsync(incoming, 0, incoming.Length);
            Packet response = Packet.DeSerialize(incoming);
            if (response.requestID == login.requestID) {
                Console.WriteLine("Connection successful");
                connected = true;
                return connected;
            }
            else {
                Console.WriteLine("Connection failed");
                connected = false;
                return connected;
            }
        }
        static async Task<string> SendCommandAsync(string message) {
            NetworkStream stream = client.GetStream();
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