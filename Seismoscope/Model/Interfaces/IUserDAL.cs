using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Model.Interfaces
{
    public interface IUserDAL
    {
        User? Authentifier(string nomUtilisateur, string motDePasse);
    }
}
