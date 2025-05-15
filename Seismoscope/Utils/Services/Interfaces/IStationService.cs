using Seismoscope.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Utils.Services.Interfaces
{
    public interface IStationService
    {
        Station? ObtenirParId(int id);
        List<Station> ObtenirToutesAvecCapteurs();
        void ModifierStation(Station station);
    }
}
