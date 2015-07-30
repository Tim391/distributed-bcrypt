using System;
using Akka.Actor;

namespace DistributedBCrypt.Shared
{
    public class RegistrationActor : ReceiveActor
    {
        private DateTime _lastActivity;

        public RegistrationActor(string masterAddress)
        {
            var master = Context.ActorSelection(masterAddress);
            var worker = Context.ActorOf(Props.Create(() => new WorkerActor()), "remoteworker");
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), Context.Self, new CheckLastActivity(), ActorRefs.NoSender);

            _lastActivity = DateTime.MinValue;

            Console.WriteLine($"Worker name: {worker.Path}");

            master.Tell(new ReadyForWork());

            Receive<UserPasswordBatch>(x =>
            {
                _lastActivity = DateTime.UtcNow;
                worker.Forward(x);
            });

            Receive<CheckLastActivity>(x =>
            {
                if (DateTime.UtcNow - _lastActivity > TimeSpan.FromSeconds(20))
                {
                    master.Tell(new ReadyForWork());
                }
            });
        }
    }

    public class CheckLastActivity
    {
    }

    public class ReadyForWork
    {
        public ReadyForWork()
        {
        }
    }

}
