using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CacheClient;
using System.Configuration;
using System.Threading;

namespace TestApplication
{
     class Program
    {

        static void Main(string[] args)
        {
            Client client=null;
            while (true)
            {
                try
                {
                    client = new Client();
                    if (client != null) break;

                }
                catch (Exception e)
                {
                    Console.WriteLine("No Connection made");
                    Console.WriteLine("Enter A for Try Again or Press any Key for Exit");

                    string res = Console.ReadLine().ToUpper();
                    if (res == "A") client = new Client();
                    else
                        Environment.Exit(0);

                }
            }
            //try
            //{
            //    client.Register();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            //Thread rec = new Thread(() => {
            //    client.Onchagne += Message;
            //    client.listen();

            //});
            //rec.Start();
            while (true)
            {
                Console.WriteLine("Cache Operation Menu");
                Console.WriteLine("1: Add item to cache");
                Console.WriteLine("2: Get item from cache");
                Console.WriteLine("3: Remove item from cache");
                Console.WriteLine("4: Clear cache");
                Console.WriteLine("5: Dispose cache");
                Console.WriteLine("6: Register an Event on server");
                Console.WriteLine("7: Exit");
                Console.Write("Enter operation number here: ");
                var operation = Console.ReadLine();
                string key;
                object value;
                Response res;
                switch (operation)
                {
                    case "1":
                        Console.Write("Enter key: ");
                        key = Console.ReadLine();
                        while(key == "")
                        {
                            Console.Write("Key is required or enter X for cancelation:");
                            key = Console.ReadLine();
                        }
                        if (key.ToUpper() == "X") break;
                        Console.Write("Enter value: ");
                        value = Console.ReadLine();
                        while (value == "")
                        {
                            Console.Write("Value is required or enter X for cancelation:");
                            value = Console.ReadLine().ToUpper();
                        }
                        if (value.Equals("X")) break;
                        try {
                            res = (Response)client.Add(key, value);
                            Console.WriteLine(res.MsgResponse);

                        }
                        catch (Exception e){
                            Console.WriteLine(e.Message);
                        }
                        break;

                    case "2":
                        Console.Write("Enter key: ");
                        key = Console.ReadLine();
                        while (key == "")
                        {
                            Console.Write("Key is required or enter X for cancelation:");
                            key = Console.ReadLine();
                        }
                        if (key.ToUpper() == "X") break;
                        try
                        {
                            res = (Response)client.Get(key);
                        Console.WriteLine(res.MsgResponse+"  " +res.Value);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "3":
                        Console.Write("Enter key: ");
                        key = Console.ReadLine();
                        while (key == "")
                        {
                            Console.Write("Key is required or enter X for cancelation:");
                            key = Console.ReadLine();
                        }
                        if (key.ToUpper() == "X") break;
                        try { 
                        res = (Response)client.Remove(key);
                        Console.WriteLine(res.MsgResponse);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "4":
                        try { 
                        res=(Response)client.Clear();
                        Console.WriteLine(res.MsgResponse);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "5":
                        try { 
                        res=(Response)client.Dispose();
                        Console.WriteLine(res.MsgResponse);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "6":
                        try
                        {
                           
                            client.Onchagne += Message;


                            client.Register();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                      
                        break;  
                    case "7":

                        Environment.Exit(0); 
                        break;
                }

            

           }
            
          
        }

        public static void Message(object sender, string msg)
        {
            Console.WriteLine(msg);

        }
    }
}
