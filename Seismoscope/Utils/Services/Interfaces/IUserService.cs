using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Utils.Services.Interfaces
{
    public interface IUserService
    {
        User? Authentifier(string nomUtilisateur, string motDePasse);
    }
}
