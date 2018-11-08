using System;
using Akka.Actor;
using Akka.Configuration;
using DistributedBCrypt.Shared;

namespace DistributedBcrypt.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
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
                system.ActorOf(Props.Create(() => new RegistrationActor("akka.tcp://Supervisor@localhost:8099/user/supervisor")), "worker");

                Console.ReadKey();
            }
        }
    }
}
