using Microsoft.EntityFrameworkCore;
using Seismoscope.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Model.DAL
{
    public class StationDAL : IStationDAL
    {
        private readonly ApplicationDbContext _context;

        public StationDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public Station? FindById(int id)
        {
            return _context.Stations
                .Include(s => s.Capteurs)
                .FirstOrDefault(s => s.Id == id);
        }
        public List<Station> FindAll()
        {
            return _context.Stations.Include(s => s.Capteurs).ToList(); 
        }

        public void Add(Station station)
        {
            _context.Stations.Add(station);
            _context.SaveChanges();
        }

        public List<Station> GetAll()
        {
            return _context.Stations.ToList();
        }

        public void UpdateStation(Station? station)
        {
            if (station != null)
            {
                _context.Stations.Update(station);
                _context.SaveChanges();
            }
            else
            {
                throw new NullReferenceException();
            }
        }
    }
}
