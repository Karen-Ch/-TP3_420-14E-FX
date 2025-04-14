using Seismoscope.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Utils.Services.Interfaces
{
    public interface ICapteurService
    {
        Capteur? ObtenirParId(int id);
        List<Capteur> ObtenirTous();
        List<Capteur> ObtenirParStation(Station station);
        void AjouterCapteur(Capteur capteur);
        void SupprimerCapteur(Capteur capteur);
        void ModifierCapteur(Capteur capteur);
    }
}
