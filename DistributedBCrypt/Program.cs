using System;
using System.Linq;
using Akka.Actor;
using Akka.Configuration;

namespace DistributedBCrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("Deployer", ConfigurationFactory.ParseString(@"
                akka {  
                    actor{
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    }
                    remote {
                        helios.tcp {
		                    port = 8099
		                    hostname = localhost
                        }
                    }
                }")))
            {
                var passwords = new DataLoader().GetPasswords().ToArray();
                system.ActorOf(Props.Create(() => new WorkerSupervisor(passwords)), "master");

                Console.ReadKey();
            }
        }
    }
}
