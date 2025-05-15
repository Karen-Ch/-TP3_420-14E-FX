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
            return BCrypt.Net.BCrypt.HashPassword(motDePasse);
        }
        public static bool VerifierMotDePasse(string motDePasse, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(motDePasse, hash);
        }
    }
}
