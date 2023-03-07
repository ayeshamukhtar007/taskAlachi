using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace Client
{
    internal class CacheClient
    {
        public static async Task Main(string[] args)
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP  socket.
            Socket client = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            await  client.ConnectAsync(remoteEP);
            while (true)
            {
                Console.Write("Enter message here: ");
                string val = Console.ReadLine();

                // Send message.
                //var message = "hi friends client is here!<|eom|>";
                var message = val;

                var messageBytes = Encoding.UTF8.GetBytes(message);
                _ = await  client.SendAsync(messageBytes, SocketFlags.None);
                Console.WriteLine($"Socket client sent message: \"{message}\"");

                // Receive ack.
                var buffer = new byte[1024];
                var received = await client.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);
                if (response == "<|ACK|>")
                {
                    Console.WriteLine(
                        $"Socket client received acknowledgment: \"{response}\"");
                    break;
                }
                else
                {
                    Console.WriteLine(
                       $"Socket client received acknowledgment: \"{response}\"");
                }
            }
        }
    }
}
