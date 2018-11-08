using System;

namespace DistributedBCrypt.Shared
{
    using BCrypt.Net;
    public class UserPassword
    {
        public Guid UserId { get; }
        public string Password { get; }

        public UserPassword(Guid userId, string password) 
        {
            UserId = userId;
            Password = password;
        }

        public HashedPasswordEntry HashPasswordWithBCrypt(int workfactor)
        {
            var newPassword = BCrypt.HashPassword(Password, workfactor); 

            return new HashedPasswordEntry(UserId, newPassword);
        }
    }
}