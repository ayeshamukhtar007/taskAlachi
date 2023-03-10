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
using System.IO;

namespace CacheClient 
{
    public delegate void Edelegate(string st);
    public delegate void Rdelegate(string st);
    public class Client:ICache
    {
        public NetworkStream clientstream;
        public TcpClient client; 
        public event Edelegate Onchange;
        public Response ResponseList;
        private static ManualResetEvent mre = new ManualResetEvent(false);
        private static ManualResetEvent ex = new ManualResetEvent(false);
        static readonly object Lock = new object();
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
                byte[] buffer;
                int received=0;
                string res;
                new Thread(() =>
                {
                    while (true)
                    {
                        
                            buffer = new byte[client.ReceiveBufferSize];

                        try
                        {
                            received = clientstream.Read(buffer, 0, buffer.Length);
                        }
                        catch (IOException e)
                        {


                            while (true)
                            {
                                try
                                {
                                    client = new TcpClient(serverAddr, port);
                                    clientstream = client.GetStream();
                                    break;
                                }catch(Exception ex) { }
                            }

                        }

                        clientstream.Flush();
                        res = Encoding.UTF8.GetString(buffer, 0, received);
                        Response response = JsonConvert.DeserializeObject<Response>(res);
                        res = "";
                       
                        lock(Lock)
                        {
                            if (response != null && response.Value != null && response.Value.Equals("Notification"))
                            {
                                Onchange?.Invoke(response.MsgResponse);


                            }
                            else if ((response != null && response.Value != null && response.Value.Equals("Exception")))
                            {
                                ResponseList = (Response)response;
                                Monitor.Pulse(Lock);
                                //mre.Set();

                            }
                            else
                            {
                                //mre.Set();
                                ResponseList = (Response)response;
                                Monitor.Pulse(Lock);
                            }

                        }

                    }
                }
                    ).Start();
               
            }
            catch (SocketException e)
            {
                throw e;
            }
            catch (IOException e)
            {
                throw ;
            }
        }

       public  void Add(string key, object value)
        {
            var message = new Request { Operation = "add", Key = key, Value = value };
           
            try
            {
                GenerateAndWriteBytes(message);
                lock (Lock)
                {
                    //mre.WaitOne();
                    Monitor.Wait(Lock);

                    if (ResponseList != null && ResponseList.Value != null && ResponseList.Value.Equals("Exception"))
                    {

                        string ex = ResponseList.MsgResponse;
                        ResponseList = new Response();
                        throw new Exception(ex);

                    }


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
            
            Response ex1=new Response();
            try
            {
                GenerateAndWriteBytes(message);
      
                lock (Lock)
                {
                    //mre.WaitOne();
                    Monitor.Wait(Lock);
                    if (ResponseList != null&& ResponseList.Value != null)
                {
                        ex1 = ResponseList;
                        ResponseList = new Response();
                      
                }
                }
                return ex1; 
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
                lock (Lock)
                {
                    //mre.WaitOne();
                    Monitor.Wait(Lock);

                    if (ResponseList != null && ResponseList.Value != null && ResponseList.Value.Equals("Exception"))
                    {

                        string ex = ResponseList.MsgResponse;
                        ResponseList = new Response();
                        throw new Exception(ex);

                    }


                }
            }
            catch (Exception e)
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
                //Console.Read();
                Environment.Exit(0);
            }
            catch (Exception e)
            {

                throw e;
            }

        }
     
        
        public void Clear()
        {

            var message = new Request { Operation = "clear" };
            try
            {

                GenerateAndWriteBytes(message);
                lock (Lock)
                {
                    //mre.WaitOne();
                    Monitor.Wait(Lock);

                    if (ResponseList != null && ResponseList.Value != null && ResponseList.Value.Equals("Exception"))
                    {

                        string ex = ResponseList.MsgResponse;
                        ResponseList = new Response();
                        throw new Exception(ex);

                    }


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
            try
            {
                GenerateAndWriteBytes(message);
            }catch(Exception e)
            {

                throw e;
            }
           
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
