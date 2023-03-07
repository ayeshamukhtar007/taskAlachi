using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Text;
using Common;
namespace Task1
{
    class CacheServer
    {

        Cache cache;
        List<TcpClient> ClientList;
        public CacheServer()
        {
            this.cache = new Cache();
            ClientList = new List<TcpClient>();
        }

        public static void Main(string[] args)
        {
            CacheServer server = new CacheServer();


            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            int port = int.Parse(ConfigurationManager.AppSettings["port"]);
            TcpListener listener = new TcpListener(localAddr, port);
            listener.Start();
            int counter = 0;
            while (true)
            {
                counter += 1;
                Console.WriteLine("Waiting for a connection...");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine(counter + "): Client Connected");
                Thread ctThread = new(() => server.DoChat(client));
                ctThread.Start();

            }
        }
        private void Notify(string message)
        {
            foreach (TcpClient client in ClientList)
            {
                var Notifymessage = new Response { Value= "Notification", MsgResponse = message, };


                var jsonString = JsonConvert.SerializeObject(Notifymessage);
                var messageBytes = Encoding.UTF8.GetBytes(jsonString);

                client.GetStream().Write(messageBytes, 0, messageBytes.Length);
            }
            
        }
        private void DoChat(TcpClient handler)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[handler.ReceiveBufferSize];
                    var received = handler.GetStream().Read(buffer, 0, buffer.Length);
                    var res = Encoding.UTF8.GetString(buffer, 0, received);
                    Request response = JsonConvert.DeserializeObject<Request>(res);
                    Response message;

                    String jsonString;
                    Byte[] messageBytes;
                    switch (response.Operation)
                    {
                        case "add":
                            try
                            {
                                cache.Add(response.Key, response.Value);
                                message = new Response { MsgResponse = "added successfully" };

                                jsonString = JsonConvert.SerializeObject(message);
                                messageBytes = Encoding.UTF8.GetBytes(jsonString);

                                handler.GetStream().Write(messageBytes, 0, messageBytes.Length);
                                //Thread notifyAddThread = new(() => Notify("Add Operation exceuted on cache"));
                                //notifyAddThread.Start();
                                //CommonClass common = new CommonClass();
                                //common.InvokeMethod("Added");
                                Notify("Add Operation exceuted on cache");
                            }
                            catch (InvalidOperationException e)
                            {
                                Console.Write(e.Message);
                                message = new Response { MsgResponse = e.Message };

                                jsonString = JsonConvert.SerializeObject(message);
                                messageBytes = Encoding.UTF8.GetBytes(jsonString);

                                handler.GetStream().Write(messageBytes, 0, messageBytes.Length);
                            }


                            break;
                        case "get":
                            try
                            {
                                var get = cache.Get(response.Key);
                                message = new Response { Value = get, MsgResponse = "Found value" };
                                jsonString = JsonConvert.SerializeObject(message);
                                messageBytes = Encoding.UTF8.GetBytes(jsonString);
                                handler.GetStream().Write(messageBytes, 0, messageBytes.Length);
                            }
                            catch (InvalidOperationException e)
                            {
                                Console.Write(e.Message);
                                message = new Response { MsgResponse = e.Message };

                                jsonString = JsonConvert.SerializeObject(message);
                                messageBytes = Encoding.UTF8.GetBytes(jsonString);

                                handler.GetStream().Write(messageBytes, 0, messageBytes.Length);
                            }
                            break;
                        case "remove":
                            try
                            {
                                cache.Remove(response.Key);
                                message = new Response { MsgResponse = "remove successfully" };

                                jsonString = JsonConvert.SerializeObject(message);
                                messageBytes = Encoding.UTF8.GetBytes(jsonString);

                                handler.GetStream().Write(messageBytes, 0, messageBytes.Length);
                                Thread notifyRemoveThread = new(() => Notify("Remove Operation exceuted on cache"));
                                notifyRemoveThread.Start();
                            }
                            catch (InvalidOperationException e)
                            {
                                Console.Write(e.Message);
                                message = new Response { MsgResponse = e.Message };

                                jsonString = JsonConvert.SerializeObject(message);
                                messageBytes = Encoding.UTF8.GetBytes(jsonString);

                                handler.GetStream().Write(messageBytes, 0, messageBytes.Length);
                            }
                            break;
                        case "clear":
                            cache.Clear();
                            message = new Response { MsgResponse = "clear successfully" };
                            jsonString = JsonConvert.SerializeObject(message);
                            messageBytes = Encoding.UTF8.GetBytes(jsonString);
                            handler.GetStream().Write(messageBytes, 0, messageBytes.Length);
                            Thread notifyThread = new(() => Notify("Clear Operation exceuted on cache"));
                            notifyThread.Start();
                            break;
                        case "register":
                            ClientList.Add(handler);
                            message = new Response { MsgResponse = "Registered"};

                            jsonString = JsonConvert.SerializeObject(message);
                            messageBytes = Encoding.UTF8.GetBytes(jsonString);

                            handler.GetStream().Write(messageBytes, 0, messageBytes.Length);
                            break;
                        case "dispose":
                            cache.Dispose();//here we when made cache object also pass client thread so when they dispose close that client thread 
                            break;
                    }



                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}
