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
    public delegate void Edelegate(string st);
    public delegate void Rdelegate(string st);

    public class Client:ICache
    {
        public NetworkStream clientstream;
        public TcpClient client; 
        public event Edelegate Onchange;
        public List<Response> ResponseList;
        public Client()
        {

            Initialize();


        }
        public void Initialize()
        {
            string serverAddr = "127.0.0.1";
            int port = int.Parse(ConfigurationManager.AppSettings["port"]);
            try
            {
                client = new TcpClient(serverAddr, port);
                clientstream = client.GetStream();
                new Thread(() =>
                {
                    while (true)
                    {

                        byte[] buffer = new byte[client.ReceiveBufferSize];
                        var received = clientstream.Read(buffer, 0, buffer.Length);
                        var res = Encoding.UTF8.GetString(buffer, 0, received);
                        Response response = JsonConvert.DeserializeObject<Response>(res);
                        if (response != null && response.Value != null && response.Value.Equals("Notification"))
                        {
                            Onchange?.Invoke(response.MsgResponse);
                        }
                        else
                        {
                            ResponseList = new List<Response>
                            {
                                response
                            };
                        }

                    }
                }
                    ).Start();

            }
            catch (SocketException e)
            {
                throw e;
            }
        }

       public  void Add(string key, object value)
        {
            var message = new Request { Operation = "add", Key = key, Value = value };
            try
            {
                GenerateAndWriteBytes(message);
                if (ResponseList != null) {
                    Response response = ResponseList.FirstOrDefault();
                    if (response != null && response.Value != null && response.Value.Equals("Exception"))
                    {
                        throw new Exception(response.MsgResponse);
                    }
                    ResponseList.Remove(ResponseList.FirstOrDefault());

                }
               



            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public object Get(string key)
        {
            var message = new Request { Operation = "get", Key = key };

            try
            {
                GenerateAndWriteBytes(message);
                if (ResponseList != null)
                {
                    Response response = ResponseList.FirstOrDefault();
                    ResponseList.Remove(ResponseList.FirstOrDefault());
                    return response;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public void Remove(string key)
        {
            var message = new Request { Operation = "remove", Key = key };

            try
            {
                GenerateAndWriteBytes(message);
                Response response = ResponseList.FirstOrDefault();
                ResponseList.Remove(ResponseList.FirstOrDefault());

                if (response.Value != null && response.Value.Equals("Exception"))
                {
                    throw new Exception(response.MsgResponse);
                }
            }
            catch(Exception e)
            {
     
                throw e;
            }

}
        public void Dispose()
        {
            var message = new Request { Operation = "dispose" };

            try
            {
                GenerateAndWriteBytes(message);
            }
            catch (Exception e)
            {

                throw e;
            }

        }
     
        
        public void Clear()
        {
           
           var message = new Request { Operation = "clear"};
            try {

                GenerateAndWriteBytes(message);
                Response response = ResponseList.FirstOrDefault();
                ResponseList.Remove(ResponseList.FirstOrDefault());

                if (response.Value != null && response.Value.Equals("Exception"))
                {
                    throw new Exception(response.MsgResponse);
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

        public void AddEvent(Action<string> Message)
        {
           
            Onchange += new Edelegate(Message);

            var message = new Request { Operation = "register" };
                GenerateAndWriteBytes(message);
                //Response response = GetMsg();

               
                //Thread rec = new Thread(() =>
                //{
                //    Listen();
                //});
                //rec.Start();

           
        }
        public void Listen()
        {
            //try
            //{
            //    while (true)
            //    {
            //        Response response = GetMsg();
            //        if (response.Value != null && response.Value.Equals("Notification"))
            //        {
            //            Onchange?.Invoke(response.MsgResponse);
            //        }


                    
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
        }
    }



}
