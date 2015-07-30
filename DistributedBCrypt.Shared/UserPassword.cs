using System;

namespace DistributedBCrypt.Shared
{
    public class UserPassword
    {
        public Guid UserId { get; }
        public string Password { get; }

        public UserPassword(Guid userId, string password)
        {
            UserId = userId;
            Password = password;
        }

        public HashedPasswordEntry HashPasswordWithBCrypt()
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt(10); //Normally 12+. Reduced for speed during tests
            var newPassword = BCrypt.Net.BCrypt.HashPassword(Password, salt);

            return new HashedPasswordEntry(UserId, newPassword);
        }
    }
}