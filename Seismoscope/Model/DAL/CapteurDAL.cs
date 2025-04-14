using Microsoft.EntityFrameworkCore;
using Seismoscope.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Seismoscope.Model.DAL
{
    public class CapteurDAL: ICapteurDAL
    {
        private readonly ApplicationDbContext _context;

        public CapteurDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public Capteur? FindById(int id)
        {
            return _context.Capteurs
                .FirstOrDefault(s => s.Id == id);
        }

        public List<Capteur> FindAll(Station station)
        {
            List<Capteur> selectione = [];
            if (station != null)
            {
                List<Capteur> capteurs = new(_context.Capteurs);
                foreach (var capteur in capteurs)
                {
                    if (station.Id == capteur.StationId)
                    {
                        selectione.Add(capteur);
                    }
                }
            }
            return selectione;
        }

        public void DeleteCapteur(Capteur? capteur) {
            try
            { 
                 if (capteur.EstDesactive)
                 {
                     _context.Capteurs.Remove(capteur);
                     _context.SaveChanges();
                 }
                 else
                 {
                     MessageBox.Show("Seuls les capteurs désactivés peuvent être supprimés", "Le capteur ne peut pas être supprimé",
                     MessageBoxButton.OK, MessageBoxImage.Information);
                 }   
            }
            catch (NullReferenceException) {
                MessageBox.Show("Capteur introuvable. ", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CreateCapteur(Capteur capteur)
        {
            try
            {
                if (capteur.EstLivre)
                {
                    _context.Capteurs.Add(capteur);
                    _context.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Seuls les capteurs livrés peuvent être ajoutés", "Le capteur ne peut pas être ajouté",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Capteur introuvable. ", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void UpdateCapteur(Capteur? capteur)
        {
            if (capteur != null)
            {   
                _context.Capteurs.Update(capteur);
                _context.SaveChanges();
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public List<Capteur> GetAll() {
            return _context.Capteurs.ToList();
        }
    }


}
