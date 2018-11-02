using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using DistributedBCrypt.Shared;

namespace DistributedBCrypt.Worker2
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
		                    port = 8092
		                    hostname = #MachineName#
                        }
                    }
                }";

            hocon = hocon.Replace("#MachineName#", Environment.MachineName);
            Thread.Sleep(2000);
            _system = ActorSystem.Create("Worker", ConfigurationFactory.ParseString(hocon));

            _system.ActorOf(Props.Create(() => new WorkerActor("akka.tcp://Deployer@localhost:8099/user/master")), "worker");
        }

        public void Stop()
        {
            _system.Terminate();
        }
    }
}
