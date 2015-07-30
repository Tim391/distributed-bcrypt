namespace DistributedBCrypt.Shared
{
    public class UserPasswordBatch
    {
        public int BatchId { get; }
        public UserPassword[] UserAnswers { get; }

        public UserPasswordBatch(UserPassword[] userAnswers, int batchId)
        {
            UserAnswers = userAnswers;
            BatchId = batchId;
        }
    }
}