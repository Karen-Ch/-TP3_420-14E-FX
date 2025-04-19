using Seismoscope.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Model.DAL
{
    public class EvenementDAL : IEvenementDAL
    {
        private readonly ApplicationDbContext _context;

        public EvenementDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<EvenementSismique> ObtenirEvenementsParStation(int stationId)
        {
            try
            {
                return _context.Evenements
                               .Where(e => e.StationId == stationId)
                               .OrderByDescending(e => e.DateEvenement)
                               .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des événements.", ex);
            }
        }

        public void AjouterEvenement(EvenementSismique evenement)
        {
            try
            {
                _context.Evenements.Add(evenement);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'ajout de l'événement.", ex);
            }
        }
    }
}
