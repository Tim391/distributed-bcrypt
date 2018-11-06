using System.Collections.Generic;

namespace DistributedBCrypt.Shared
{
    public class BatchFinished
    {
        public List<HashedPasswordEntry> NewPasswords { get; }
        public int BatchId { get; }

        public BatchFinished(List<HashedPasswordEntry> newPasswords, int batchId)
        {
            NewPasswords = newPasswords;
            BatchId = batchId;
        }
    }
}