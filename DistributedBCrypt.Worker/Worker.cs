using System;
using System.Threading;
using Akka.Actor;
using Akka.Configuration;
using DistributedBCrypt.Shared;

namespace DistributedBCrypt.Worker
{
    public class Worker
    {
        private ActorSystem _system;

        public void Start()
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
            Thread.Sleep(2000);
            _system = ActorSystem.Create("Worker", ConfigurationFactory.ParseString(hocon));

            _system.ActorOf(Props.Create(() => new RegistrationActor("akka.tcp://Supervisor@localhost:8099/user/supervisor")), "worker");
        }

        public void Stop()
        {
            _system.Terminate();
        }
    }
}