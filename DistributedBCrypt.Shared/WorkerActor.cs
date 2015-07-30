using System;
using System.Collections.Generic;
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

                var newPasswords = batch.UserAnswers.AsParallel().Select(x => x.HashPasswordWithBCrypt()).ToList();
                Console.WriteLine("Batch finished");
                Sender.Tell(new BatchFinished(newPasswords), Context.Parent);
            });
        }
    }
}
