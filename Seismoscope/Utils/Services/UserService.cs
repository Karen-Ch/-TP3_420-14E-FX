using Seismoscope.Model.Interfaces;
using Seismoscope.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Utils.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDAL _userDal;

        public UserService(IUserDAL userDal)
        {
            _userDal = userDal;
        }

        public User? Authentifier(string nomUtilisateur, string motDePasse)
        {
            return _userDal.Authentifier(nomUtilisateur, motDePasse);
        }
    }
}
