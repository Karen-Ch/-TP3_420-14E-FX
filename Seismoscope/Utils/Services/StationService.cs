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
    public class StationService : IStationService
    {
        private readonly IStationDAL _stationDal;

        public StationService(IStationDAL stationDal)
        {
            _stationDal = stationDal;
        }

        public Station? ObtenirParId(int id)
        {
            return _stationDal.FindById(id);
        }

        public List<Station> ObtenirToutesAvecCapteurs()
        {
            return _stationDal.FindAll();
        }
    }
}
