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
          
            ICache client = null;


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
                    Console.WriteLine("Trying Again");

                    //string res = Console.ReadLine().ToUpper();
                    //if (res == "A") connection = new Network();
                    //else
                    //    Environment.Exit(0);

                }
            }
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
                var operation = "";
               
                    Console.Write("Enter operation number here: ");
                     operation = Console.ReadLine();

                
                
                string key;
                object value;
                Response res;
                switch (operation)
                {
                    case "1":
                        Console.Write("Enter key: ");
                        key = Console.ReadLine();
                        while (key == "")
                        {
                            Console.Write("Key is required or enter X for cancelation:");
                            key = Console.ReadLine();
                        }
                        if (key.ToUpper() == "X") break;
                        Console.Write("Enter value: ");
                        value = Console.ReadLine();
                        while (value == null)
                        {
                            Console.Write("Value is required or enter X for cancelation:");
                            value = Console.ReadLine().ToUpper();
                        }
                        if (value.Equals("X")) break;
                        try
                        {
                            client.Add(key, value);


                        }
                        catch (Exception e)
                        {
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
                            if(res.Equals(""))
                            Console.WriteLine(res.MsgResponse + "  " + res.Value);
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
                        try
                        {
                            client.Remove(key);
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "4":
                        try
                        {
                           client.Clear();
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "5":
                        try
                        {
                            client.Dispose();
                         
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case "6":
                        try
                        {
                            client.AddEvent(Message);



                            
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


    

    public static void Message(string msg)
    {
        Console.WriteLine("Notificatio "+msg);

    }
}
}
