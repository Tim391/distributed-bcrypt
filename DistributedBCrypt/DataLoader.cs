using System;
using System.Collections.Generic;
using DistributedBCrypt.Shared;

namespace DistributedBCrypt
{
    public class DataLoader
    {
        public  UserPassword[] GetPasswords()
        {
            var passwords = new List<UserPassword>();

            for (var i = 0; i < 1001; i++)
            {
                passwords.Add(new UserPassword(Guid.NewGuid(), $"VerySecurePassword{i}"));
            }
            return passwords.ToArray();
        }
    }
}