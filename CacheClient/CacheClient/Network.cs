using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CacheClient
{
    public class Network
    {
        public event EventHandler<string> Onchange;
        public NetworkStream clientstream;
        public TcpClient client;
        public Network()
        {
            string serverAddr = "127.0.0.1";
            int port = int.Parse(ConfigurationManager.AppSettings["port"]);
            try
            {
                client = new TcpClient(serverAddr, port);
                clientstream = client.GetStream();
            }
            catch (SocketException e)
            {
                throw e;
            }


        }
        public void Register()
        {
            var message = new Request { Operation = "register" };

            try
            {
                GenerateAndWriteBytes(message);
                Response response = GetMsg();

                if (response.Value != null && response.Value.Equals("Exception"))
                {
                    throw new Exception(response.MsgResponse);
                }
                Thread rec = new Thread(() =>
                {
                    Listen();
                });
                rec.Start();

            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public void Listen()
        {
            try
            {
                while (true)
                {
                    Response response = GetMsg();
                    if (response.Value != null && response.Value.Equals("Notification"))
                    {
                        Onchange?.Invoke(this, response.MsgResponse);
                    }
                   

                    //if (response.Value != null && response.Value.Equals("Exception"))
                    //{
                    //    throw new Exception(response.MsgResponse);
                    //}
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void GenerateAndWriteBytes(Request message)
        {
            
                var jsonString = JsonConvert.SerializeObject(message);
                var messageBytes = Encoding.UTF8.GetBytes(jsonString);
                clientstream.Write(messageBytes, 0, messageBytes.Length);
           
        }
        public Response GetMsg()
        {
            

                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    var received = clientstream.Read(buffer, 0, buffer.Length);
                    var res = Encoding.UTF8.GetString(buffer, 0, received);
                    Response response = JsonConvert.DeserializeObject<Response>(res);
                    return response;
             
        }
    }
}
