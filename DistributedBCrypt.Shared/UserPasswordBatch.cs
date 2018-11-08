namespace DistributedBCrypt.Shared
{
    public class UserPasswordBatch
    {
        public int BatchId { get; }
        public int WorkFactor { get; }
        public UserPassword[] UserPasswords { get; }

        public UserPasswordBatch(UserPassword[] userPasswords, int batchId, int workFactor)
        {
            UserPasswords = userPasswords;
            BatchId = batchId;
            WorkFactor = workFactor;
        }
    }
}