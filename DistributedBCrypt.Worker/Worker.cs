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
                        helios.tcp {
		                    port = 8090
		                    hostname = #MachineName#
                        }
                    }
                }";

            hocon = hocon.Replace("#MachineName#", Environment.MachineName);
            Thread.Sleep(2000);
            _system = ActorSystem.Create("Worker", ConfigurationFactory.ParseString(hocon));

            _system.ActorOf(Props.Create(() => new RegistrationActor("akka.tcp://Deployer@localhost:8099/user/master")), "worker");
            _system.AwaitTermination();
        }

        public void Stop()
        {
            _system.Shutdown();
        }
    }
}