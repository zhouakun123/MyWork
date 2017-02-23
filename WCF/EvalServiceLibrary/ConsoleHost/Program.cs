using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHost
{
    using System.ServiceModel;

    using EvalServiceLibrary;

    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(EvalService));
            
            try
            {
                host.Open();
                PrintServiceInfo(host);
                Console.ReadLine();
                host.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                host.Abort();
            }
        }

        static public void PrintServiceInfo(ServiceHost host)
        {
            Console.WriteLine("{0} is up and running with these endpoints:", host.Description.ServiceType);
            foreach (var endpoint in host.Description.Endpoints)
            {
                Console.WriteLine(endpoint.Address);
            }
        }
    }
}
