using System.Collections.Generic;
using System.Linq;
using Seismoscope.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using Seismoscope.Utils;

namespace Seismoscope.Model.DAL
{
    public class UserDAL : IUserDAL
    {
        private readonly ApplicationDbContext _context;

        public UserDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public User? Authentifier(string nomUtilisateur, string motDePasse)
        {
            var user = _context.Users
                .FirstOrDefault(user => user.NomUtilisateur == nomUtilisateur);

            if (user == null)
                return null;

            bool motDePasseValide = Securite.VerifierMotDePasse(motDePasse, user.MotDePasse);

            return motDePasseValide ? user : null;
        }

    }
}
