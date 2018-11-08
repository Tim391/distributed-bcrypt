using System;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using DistributedBCrypt.Shared;
using Microsoft.Extensions.Configuration;

namespace DistributedBcrypt.Worker
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
                    actor.provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                    remote {
                        dot-netty.tcp {
		                    port = 0 # bound to a dynamic port assigned by the OS
		                    hostname = #MachineName#
                        }
                    }
                }";

            hocon = hocon.Replace("#MachineName#", Environment.MachineName);

            using (var system = ActorSystem.Create("Worker", ConfigurationFactory.ParseString(hocon)))
            {
                var supervisorPath = configuration["supervisorPath"];
                system.ActorOf(Props.Create(() => new RegistrationActor(supervisorPath)), "worker");

                Console.ReadKey();
            }
        }
    }
}
