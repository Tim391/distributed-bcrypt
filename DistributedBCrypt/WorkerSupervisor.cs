using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Akka.Actor;
using DistributedBCrypt.Shared;
using ServiceStack;

namespace DistributedBCrypt
{
    public class WorkerSupervisor : ReceiveActor
    {
        public WorkerSupervisor(UserPassword[] answers)
        {
            var batches = CreateBatches(answers);
            var processedAnswers = new List<HashedPasswordEntry>();
            int remainingBatches = batches.Count;
            Console.WriteLine($"Total Batches: {remainingBatches}");


            Receive<ReadyForWork>(worker =>
            {
                Console.WriteLine($"Worker ready: {Sender.Path}");
                if (batches.Count > 0)
                {
                    Sender.Tell(batches.Dequeue());
                }
            });

            Receive<BatchFinished>(b =>
            {
                remainingBatches--;
                processedAnswers.AddRange(b.NewPasswords);
                if (remainingBatches <= 0)
                {
                    File.WriteAllText("D:\\PasswordHashes.csv", processedAnswers.ToCsv());
                    Console.WriteLine("Processing complete!");

                    return;
                }

                Console.WriteLine($"Batch complete. {remainingBatches} batches remaining");

                if (batches.Count > 0) 
                    Sender.Tell(batches.Dequeue());
            });
        }

        private Queue<UserPasswordBatch> CreateBatches(UserPassword[] passwords)
        {
            var batches = passwords.BatchesOf(200)
                .Select((pb, i) => new UserPasswordBatch(pb, i + 1));

            return new Queue<UserPasswordBatch>(batches);
        }
    }

}