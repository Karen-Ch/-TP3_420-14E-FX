using Seismoscope.Model.Interfaces;
using Seismoscope.Model;
using Seismoscope.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Utils.Services
{
    public class EvenementService : IEvenementService
    {
        private readonly IEvenementDAL _evenementDal;

        public EvenementService(IEvenementDAL evenementDal)
        {
            _evenementDal = evenementDal;
        }

        public List<EvenementSismique> ObtenirParStation(int stationId)
        {
            return _evenementDal.ObtenirEvenementsParStation(stationId);
        }

        public void AjouterEvenement(EvenementSismique evenement)
        {
            _evenementDal.AjouterEvenement(evenement);
        }
    }
}
