using System;
using System.Linq;
using Akka.Actor;

namespace DistributedBCrypt.Shared
{
    public class WorkerActor : ReceiveActor
    {
        private bool _processing = false;
        private DateTime _lastActivity = DateTime.MinValue;

        public WorkerActor(string masterAddress)
        {
            var master = Context.ActorSelection(masterAddress);
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), Context.Self, new CheckLastActivity(), ActorRefs.NoSender);

            Console.WriteLine($"Worker name: {Self.Path}");

            master.Tell(new ReadyForWork());

            Receive<UserPasswordBatch>(batch =>
            {
                _lastActivity = DateTime.UtcNow;
                Console.WriteLine($"Processing batch {batch.BatchId}");
                _processing = true;

                var newPasswords = batch.UserAnswers.AsParallel().Select(x => x.HashPasswordWithBCrypt()).ToList();
                Sender.Tell(new BatchFinished(newPasswords));

                _processing = false;
                Console.WriteLine("Batch finished");
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
