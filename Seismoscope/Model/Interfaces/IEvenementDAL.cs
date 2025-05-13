using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Model.Interfaces
{
    public interface IEvenementDAL
    {
        List<EvenementSismique> ObtenirEvenementsParStation(int stationId);

        void AjouterEvenement(EvenementSismique evenement);
    }
}
