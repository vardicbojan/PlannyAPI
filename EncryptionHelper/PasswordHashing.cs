using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Planny.EncryptionHelper
{
    internal class PasswordHashing
    {
        internal static byte[] CreateSalt()
        {
            var buffer = new byte[32];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            return buffer;
        }

        public static byte[] HashPassword(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 4; // number = number of cores * 2, example 1: 16 =  8 cores, example 2: 8 = 4 cores
            argon2.Iterations = 2;
            argon2.MemorySize = 1024 * 1024; // 1 GB

            return argon2.GetBytes(64);
        }

        private static bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }
    }
}
