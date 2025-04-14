using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Model.Interfaces
{
    public interface ICapteurDAL
    {
        Capteur? FindById(int id);
        List<Capteur> GetAll();
        List<Capteur> FindAll(Station station);
        void DeleteCapteur(Capteur? capteur);
        void UpdateCapteur(Capteur? capteur);
        void CreateCapteur(Capteur capteur);
        
    }
}
