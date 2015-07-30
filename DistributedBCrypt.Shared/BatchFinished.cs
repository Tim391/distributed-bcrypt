using System.Collections.Generic;

namespace DistributedBCrypt.Shared
{
    public class BatchFinished
    {
        public List<HashedPasswordEntry> NewPasswords { get; }

        public BatchFinished(List<HashedPasswordEntry> newPasswords)
        {
            NewPasswords = newPasswords;
        }
    }
}