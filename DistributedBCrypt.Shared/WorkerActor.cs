using System;
using System.Linq;
using Akka.Actor;

namespace DistributedBCrypt.Shared
{
    public class WorkerActor : ReceiveActor
    {
        public WorkerActor()
        {
            Receive<UserPasswordBatch>(batch =>
            {
                Console.WriteLine($"Processing batch {batch.BatchId}");
                var newPasswords = batch.UserPasswords.AsParallel().Select(x => x.HashPasswordWithBCrypt()).ToList();
                Console.WriteLine("Batch finished");

                Sender.Tell(new BatchFinished(newPasswords, batch.BatchId));
            });
        }
    }
}
