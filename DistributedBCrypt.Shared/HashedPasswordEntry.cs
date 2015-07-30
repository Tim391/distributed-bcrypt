using System;

namespace DistributedBCrypt.Shared
{
    public class HashedPasswordEntry
    {
        public string UserId { get; }
        public string Password { get; }

        public HashedPasswordEntry(Guid userId, string password)
        {
            UserId = userId.ToString("B").ToUpper();
            Password = password;
        }
    }
}