using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Seismoscope.Utils
{
    public static class Securite
    {
        public static string HasherMotDePasse(string motDePasse)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] motBytes = Encoding.UTF8.GetBytes(motDePasse);
                byte[] hashBytes = sha.ComputeHash(motBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
