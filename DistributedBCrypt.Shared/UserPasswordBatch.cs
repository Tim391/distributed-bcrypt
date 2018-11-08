namespace DistributedBCrypt.Shared
{
    public class UserPasswordBatch
    {
        public int BatchId { get; }
        public UserPassword[] UserPasswords { get; }

        public UserPasswordBatch(UserPassword[] userPasswords, int batchId)
        {
            UserPasswords = userPasswords;
            BatchId = batchId;
        }
    }
}