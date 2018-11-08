using System;
using System.IO;
using System.Linq;
using Akka.Actor;
using Akka.Configuration;
using Microsoft.Extensions.Configuration;

namespace DistributedBcrypt.Supervisor
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            var hocon = @"
                akka {  
                    actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                        serializers {
                            hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                        }
                        serialization-bindings {
                            ""System.Object"" = hyperion
                        }
                    }
                    remote {
                        dot-netty.tcp {
		                    port = #port#
		                    hostname = #hostname#
                        }
                    }
                }";

            hocon = hocon.Replace("#port#", configuration["port"])
                .Replace("#hostname#", configuration["hostname"]);

            using (var system = ActorSystem.Create("Supervisor", ConfigurationFactory.ParseString(hocon)))
            {
                var passwords = new DataLoader().GetPasswords().ToArray();
                system.ActorOf(Props.Create(() => new WorkerSupervisor(passwords)), "supervisor");

                Console.ReadKey();
            }
        }
    }
}
