using System;
using Akka.Actor;

namespace DistributedBCrypt.Shared
{
    public class RegistrationActor : ReceiveActor
    {
        private bool _processing = false;
        private DateTime _lastActivity = DateTime.MinValue;

        public RegistrationActor(string masterAddress)
        {
            var master = Context.ActorSelection(masterAddress);
            var worker = Context.ActorOf(Props.Create(() => new WorkerActor()), "remoteworker");

            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), Context.Self, new CheckLastActivity(), ActorRefs.NoSender);

            Console.WriteLine($"Worker name: {Self.Path}");

            master.Tell(new ReadyForWork());

            Receive<UserPasswordBatch>(batch =>
            {
                _lastActivity = DateTime.UtcNow;
                _processing = true;

                worker.Tell(batch);
            });

            Receive<BatchFinished>(batch =>
            {
                _processing = false;
                master.Tell(batch);
            });

            Receive<CheckLastActivity>(x =>
            {
                if (!_processing && DateTime.UtcNow >= _lastActivity.AddSeconds(10))
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
    }
}