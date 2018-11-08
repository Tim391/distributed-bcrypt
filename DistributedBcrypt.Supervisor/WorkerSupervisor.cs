using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Akka.Actor;
using DistributedBCrypt.Shared;
using ServiceStack;

namespace DistributedBcrypt.Supervisor
{
    public class WorkerSupervisor : ReceiveActor
    {
        private class SupervisorState
        {
            private IImmutableList<UserPasswordBatch> PasswordBatches { get; }
            private Queue<int> BatchesToProcess { get;  }
            private Dictionary<int, string> BatchesInProgess { get; }
            public List<HashedPasswordEntry> ProcessedPasswords { get; }

            public SupervisorState(IImmutableList<UserPasswordBatch> passwordBatches)
            {
                PasswordBatches = passwordBatches;
                BatchesToProcess = new Queue<int>(passwordBatches.Select(b => b.BatchId));
                BatchesInProgess = new Dictionary<int, string>();
                ProcessedPasswords = new List<HashedPasswordEntry>();
            }

            public UserPasswordBatch NextBatch(string actorPath)
            {
                var nextBatchId = BatchesToProcess.Dequeue();
                var nextBatch = PasswordBatches.First(b => b.BatchId == nextBatchId);
                BatchesInProgess.Add(nextBatchId, actorPath);

                return nextBatch;
            }

            public void CompleteBatch(BatchFinished batch)
            {
                BatchesInProgess.Remove(batch.BatchId);
                ProcessedPasswords.AddRange(batch.NewPasswords);
            }

            public int RemainingBatches()
            {
                return BatchesToProcess.Count;
            }

            public bool ProcessingComplete()
            {
                return BatchesToProcess.Count <= 0 &&
                       BatchesInProgess.Count <= 0;
            }

            public void BatchFailed(string actorPath)
            {
                var inProgress = BatchesInProgess
                    .Where(bd => bd.Value == actorPath)
                    .Select(b => b.Key)
                    .ToList();

                if (!inProgress.Any()) return;

                foreach (var batchId in inProgress)
                {
                    BatchesInProgess.Remove(batchId);
                    BatchesToProcess.Enqueue(batchId);
                }
            }
        }

        public WorkerSupervisor(UserPassword[] answers)
        {
            var state = new SupervisorState(CreateBatches(answers));
            Console.WriteLine($"Total Batches: {state.RemainingBatches()}");

            Receive<ReadyForWork>(worker =>
            {
                Context.Watch(Sender);
                Console.WriteLine($"Worker ready: {Sender.Path}");

                if (state.RemainingBatches() > 0)
                {
                    var nextbatch = state.NextBatch(Sender.Path.ToStringWithAddress());
                    Console.WriteLine($"Sending batch {nextbatch.BatchId} to {Sender.Path}");
                    Sender.Tell(nextbatch);
                }
            });

            Receive<BatchFinished>(b =>
            {
                state.CompleteBatch(b);
                Console.WriteLine($"Batch {b.BatchId} complete. {state.RemainingBatches()} batches remaining");

                if (state.RemainingBatches() > 0)
                {
                    var nextbatch = state.NextBatch(Sender.Path.ToStringWithAddress());
                    Console.WriteLine($"Sending batch: {nextbatch.BatchId} to {Sender.Path}");
                    Sender.Tell(nextbatch);
                    return;
                }

                if (state.ProcessingComplete())
                {
                    File.WriteAllText("D:\\PasswordHashes.csv", state.ProcessedPasswords.ToCsv());
                    Console.WriteLine("Processing complete!");
                }
            });

            Receive<Terminated>(t => {
                Console.WriteLine($"Termination received from {t.ActorRef.Path}");
                state.BatchFailed(t.ActorRef.Path.ToStringWithAddress());
            });
        }

        private IImmutableList<UserPasswordBatch> CreateBatches(UserPassword[] passwords)
        {
            return passwords.BatchesOf(200)
                .Select((pb, i) => new UserPasswordBatch(pb, i + 1, 12)) //work factor normally 12+.
                .ToImmutableList();
        }
    }

}