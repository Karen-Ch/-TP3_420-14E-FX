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
    public class CapteurService : ICapteurService
    {
        private readonly ICapteurDAL _capteurDal;

        public CapteurService(ICapteurDAL capteurDal)
        {
            _capteurDal = capteurDal;
        }

        public Capteur? ObtenirParId(int id)
        {
            return _capteurDal.FindById(id);
        }

        public List<Capteur> ObtenirTous()
        {
            return _capteurDal.GetAll();
        }

        public List<Capteur> ObtenirParStation(Station station)
        {
            return _capteurDal.FindAll(station);
        }

        public void AjouterCapteur(Capteur capteur)
        {
            _capteurDal.CreateCapteur(capteur);
        }

        public void SupprimerCapteur(Capteur capteur)
        {
            _capteurDal.DeleteCapteur(capteur);
        }

        public void ModifierCapteur(Capteur capteur)
        {
            _capteurDal.UpdateCapteur(capteur);
        }
    }
}
