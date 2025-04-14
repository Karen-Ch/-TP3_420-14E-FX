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
            string hash = Securite.HasherMotDePasse(motDePasse);
            return _context.Users
                .FirstOrDefault(u => u.NomUtilisateur == nomUtilisateur && u.MotDePasse == hash);
        }
    }
}
