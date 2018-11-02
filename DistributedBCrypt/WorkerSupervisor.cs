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

                    //Context.System.Terminate();

                    return;
                }

                Console.WriteLine($"Batch complete. {remainingBatches} batches remaining");

                if (batches.Count > 0) 
                    Sender.Tell(batches.Dequeue());
            });
        }

        private Queue<UserPasswordBatch> CreateBatches(UserPassword[] passwords)
        {
            var total = passwords.Length;

            Console.WriteLine($"Total answers to hash: {total}");
            var chunkSize = 200;
            Console.WriteLine($"Chunk size: {chunkSize}");
            var chunks = (int)Math.Ceiling(total / (double)chunkSize);

            var results = new Queue<UserPasswordBatch>();

            for (int i = 0; i < chunks; i++)
            {
                var inChunk = passwords.Skip(i * chunkSize).Take(chunkSize).ToArray();
                results.Enqueue(new UserPasswordBatch(inChunk, i+1));
            }
            return results;
        } 
    }

}