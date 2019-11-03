using DevOne.Security.Cryptography.BCrypt;
using System;

namespace slms2asp.Shared
{
    public class Hashing
    {
        /// <summary>
        /// Ammount of rounds used to generate 
        /// password hash.
        /// </summary>
        public static int Rounds { get; set; } = 12;

        /// <summary>
        /// Returns a hash generated of the passed keyword based on the
        /// Blowfish algorithm using a randomly generated salt with
        /// the set ammount of rounds.
        /// </summary>
        /// <param name="password">Clear text password string</param>
        /// <returns>Hash containing salt</returns>
        public static string CreatePasswordHash(string password)
        {
            var salt = BCryptHelper.GenerateSalt(Rounds);
            return BCryptHelper.HashPassword(password, salt);
        }

        /// <summary>
        /// Returns true if the passed clear text password matches
        /// the passed keyword hash.
        /// </summary>
        /// <param name="pw">Clear text password</param>
        /// <param name="hash">Keyword hash</param>
        /// <returns>Match success state</returns>
        public static bool CompareStringToHash(string pw, string hash)
        {
            try
            {
                return BCryptHelper.CheckPassword(pw, hash);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
