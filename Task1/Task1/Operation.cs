using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;
namespace Task1
{
    public class Operation
    {
        Cache cache;
        List<TcpClient> ClientList;
        public Operation()
        {
            this.cache = new Cache();
            ClientList = new List<TcpClient>();
        }
        public void DoChat(TcpClient handler)
        {
            try
            {
                while (true)
                {
                    Request request;
                    
                       request = GetMsg(handler);

                  

                        Response message;
                        switch (request.Operation)
                        {
                            case "add":
                                try
                                {
                                    cache.Add(request.Key, request.Value);
                                message = new Response { MsgResponse = "Added" };
                                GenerateAndWriteBytes(message, handler);
                                Thread notifyRemoveThread = new(() => Notify("Add Operation exceuted on cache"));
                                notifyRemoveThread.Start();
                            }
                                catch (InvalidOperationException e)
                                {
                                    message = new Response { MsgResponse = e.Message, Value = "Exception" };
                                    GenerateAndWriteBytes(message, handler);
                                }


                                break;
                            case "get":
                                try
                                {
                                    var get = cache.Get(request.Key);
                                    message = new Response { Value = get, MsgResponse = "Found value" };
                                    GenerateAndWriteBytes(message, handler);
                                }
                                catch (InvalidOperationException e)
                                {
                                    Console.Write(e.Message);
                                    message = new Response { MsgResponse = e.Message, Value = "Exception" };
                                    GenerateAndWriteBytes(message, handler);
                                }
                                break;
                            case "remove":
                                try
                                {
                                    cache.Remove(request.Key);
                                    message = new Response { MsgResponse = "remove successfully" };
                                    GenerateAndWriteBytes(message, handler);
                                    Thread notifyRemoveThread = new(() => Notify("Remove Operation exceuted on cache"));
                                    notifyRemoveThread.Start();
                                }
                                catch (InvalidOperationException e)
                                {
                                    message = new Response { MsgResponse = e.Message, Value = "Exception" };
                                    GenerateAndWriteBytes(message, handler);
                                }
                                break;
                            case "clear":
                                cache.Clear(); 
                                message = new Response { MsgResponse = "added successfully" };
                                GenerateAndWriteBytes(message, handler);
                                Thread notifyThread = new(() => Notify("Clear Operation exceuted on cache"));
                                notifyThread.Start();
                                break;
                            case "register":
                                ClientList.Add(handler);
                                message = new Response { MsgResponse = "Registered" };
                                GenerateAndWriteBytes(message, handler);
                                break;
                            case "dispose":
                                handler.Dispose();//here we when made cache object also pass client thread so when they dispose close that client thread 
                                break;
                        }


                    
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void GenerateAndWriteBytes(Response message, TcpClient handler)
        {
            
               
               
                    var jsonString = JsonConvert.SerializeObject(message);
                var messageBytes = Encoding.UTF8.GetBytes(jsonString);
                handler.GetStream().Write(messageBytes, 0, messageBytes.Length);
            handler.GetStream().Flush();


        }
        public Request GetMsg(TcpClient handler)
        {
            byte[] buffer;
            try
            {

                buffer = new byte[handler.ReceiveBufferSize];
            }
            catch (NullReferenceException e)
            {
                throw e;
            }
            var received = handler.GetStream().Read(buffer, 0, buffer.Length);
                var res = Encoding.UTF8.GetString(buffer, 0, received);
                Request request = JsonConvert.DeserializeObject<Request>(res);

                return request;
           
        }

        private void Notify(string message)
        {
            foreach (TcpClient client in ClientList)
            {
                
                    var Notifymessage = new Response { Value = "Notification", MsgResponse = message, };


                    var jsonString = JsonConvert.SerializeObject(Notifymessage);
                    var messageBytes = Encoding.UTF8.GetBytes(jsonString);

                    
                client.GetStream().Write(messageBytes, 0, messageBytes.Length);
                
            }

        }
    }

}
