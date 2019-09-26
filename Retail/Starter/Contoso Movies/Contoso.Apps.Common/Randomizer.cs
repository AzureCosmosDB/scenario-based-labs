using System;
using System.Text;

namespace Contoso.Apps.Common
{
    /// <summary>
    /// Provides thread-safe character and numerical randomized values.
    /// </summary>
    public class Randomizer
    {
        public int GetRandomNumber(int maxNumber)
        {
            if (maxNumber < 1)
            {
                throw new ArgumentOutOfRangeException("maxNumber", "The maxNumber value should be greater than 1");
            }

            var b = new byte[4];

            using (var crypto = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                crypto.GetBytes(b);
            }

            int seed = (b[0] & 0x7f) << 24 | b[1] << 16 | b[2] << 8 | b[3];

            var rnd = new System.Random(seed);

            return rnd.Next(1, maxNumber);
        }

        public string GetRandomString(int length)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(
                    _array[GetRandomNumber(53)]
                );
            }

            return sb.ToString();
        }

        private static string[] _array = new string[54]
        {
            "0","2","3","4","5","6","8","9",
            "a","b","c","d","e","f","g","h","j","k","m","n","p","q","r","s","t","u","v","w","x","y","z",
            "A","B","C","D","E","F","G","H","J","K","L","M","N","P","R","S","T","U","V","W","X","Y","Z"
        };
    }
}
