using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using static System.Net.Sockets.TcpClient;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Threading;
using Common;
namespace CacheClient 
{
  

    public class Client
    {
        private NetworkStream clientstream;
        private TcpClient client;
        public event EventHandler<string> Onchagne;
        public Client()
        {
            
            string serverAddr ="127.0.0.1";
            int port = int.Parse(ConfigurationManager.AppSettings["port"]);
            try {
                client = new TcpClient(serverAddr, port);
                clientstream = client.GetStream();
            }
            catch(SocketException e)
            {
                throw e;
            }
             
      

        }
        public object Add(string key, object value)
        {

            var message = new Request { Operation = "add", Key = key, Value = value };
            try
            {
                var jsonString = JsonConvert.SerializeObject(message);
                var messageBytes = Encoding.UTF8.GetBytes(jsonString);
                clientstream.Write(messageBytes, 0, messageBytes.Length);
                //get response from server
                byte[] buffer = new byte[client.ReceiveBufferSize];
                var received = clientstream.Read(buffer, 0, buffer.Length);
                var res = Encoding.UTF8.GetString(buffer, 0, received);
                Response response = JsonConvert.DeserializeObject<Response>(res);
                return response;
            }
            catch(Exception e)
            {
     
                throw e;
            }

        }
        public object Get(string key)
        {
            try { 
            var message = new Request { Operation = "get", Key = key};
            var jsonString = JsonConvert.SerializeObject(message);
            var messageBytes = Encoding.UTF8.GetBytes(jsonString);
            clientstream.Write(messageBytes, 0, messageBytes.Length);

            byte[] buffer = new byte[client.ReceiveBufferSize];
            var received = clientstream.Read(buffer, 0, buffer.Length);
            var res = Encoding.UTF8.GetString(buffer, 0, received);
            Response response = JsonConvert.DeserializeObject<Response>(res);
            return response;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public object Remove(string key)
        {
            try
            {

                var message = new Request { Operation = "remove", Key = key };
                var jsonString = JsonConvert.SerializeObject(message);
                var messageBytes = Encoding.UTF8.GetBytes(jsonString);
                clientstream.Write(messageBytes, 0, messageBytes.Length);

                byte[] buffer = new byte[client.ReceiveBufferSize];
                var received = clientstream.Read(buffer, 0, buffer.Length);
                var res = Encoding.UTF8.GetString(buffer, 0, received);
                Response response = JsonConvert.DeserializeObject<Response>(res);
                return response;
            }
            catch(Exception e)
            {
     
                throw e;
            }

}
        public object Dispose()
        {
            try { 
            var message = new Request { Operation = "dispose"};
            var jsonString = JsonConvert.SerializeObject(message);
            var messageBytes = Encoding.UTF8.GetBytes(jsonString);
            clientstream.Write(messageBytes, 0, messageBytes.Length);

            byte[] buffer = new byte[client.ReceiveBufferSize];
            var received = clientstream.Read(buffer, 0, buffer.Length);
            var res = Encoding.UTF8.GetString(buffer, 0, received);
            Response response = JsonConvert.DeserializeObject<Response>(res);
            return response;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
     
        public void Register()
        {
           
            try
            {
                var message = new Request { Operation = "register" };
                var jsonString = JsonConvert.SerializeObject(message);
                var messageBytes = Encoding.UTF8.GetBytes(jsonString);
                clientstream.Write(messageBytes, 0, messageBytes.Length);
                byte[] buffer = new byte[client.ReceiveBufferSize];
                var received = clientstream.Read(buffer, 0, buffer.Length);
                var res = Encoding.UTF8.GetString(buffer, 0, received);
                Response response = JsonConvert.DeserializeObject<Response>(res);
                Console.WriteLine(response);
                Thread rec = new Thread(() =>
                {
                    Listen();



                });
                rec.Start();
                //CommonClass common = new CommonClass();
                //common.Common += Listen;

                //    new Thread(() => {
                //        buffer = new byte[client.ReceiveBufferSize];
                //        received = clientstream.Read(buffer, 0, buffer.Length);
                //        res = Encoding.UTF8.GetString(buffer, 0, received);
                //        response = JsonConvert.DeserializeObject<Response>(res);

                //        Onchagne?.Invoke(this, response.MsgResponse);
                //    }

                //).Start();
 }
            catch (Exception e)
            {

                throw e;
            }

        }
        //public void Listen(object sender, string st)
        public void Listen()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    var received = clientstream.Read(buffer, 0, buffer.Length);
                    var res = Encoding.UTF8.GetString(buffer, 0, received);
                    Response response = JsonConvert.DeserializeObject<Response>(res);
                    if (response.Value != null && response.Value.Equals("Notification"))
                    {
                        //Console.WriteLine("Cacheclient Listen here "+ st);
                        Onchagne?.Invoke(this, response.MsgResponse);
                    }
                }
            }catch(Exception e)
            {
                throw e;
            }
        }
        public object Clear()
        {
            try { 
           var message = new Request { Operation = "clear"};
            var jsonString = JsonConvert.SerializeObject(message);
            var messageBytes = Encoding.UTF8.GetBytes(jsonString);
            clientstream.Write(messageBytes, 0, messageBytes.Length);

            byte[] buffer = new byte[client.ReceiveBufferSize];
            var received = clientstream.Read(buffer, 0, buffer.Length);
            var res = Encoding.UTF8.GetString(buffer, 0, received);
            Response response = JsonConvert.DeserializeObject<Response>(res);
            return response;
            }
            catch (Exception e)
            {

                throw e;
            }


        }

    }



}
