using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Model.Interfaces
{
    public interface IStationDAL
    {
        Station? FindById(int id);
        List<Station> FindAll();
        //Station? FindById(Station? nouvelleStation);
    }
}
