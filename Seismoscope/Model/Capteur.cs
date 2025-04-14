using Seismoscope.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Model
{
    public class Capteur
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Type { get; set; }
        public Etat Statut { get; set; }
        public double FrequenceCollecte { get; set; }
        public double SeuilAlerte { get; set; }
        public DateTime DateInstallation { get; set; }
        public bool EstDesactive { get; set; }
        public bool EstLivre { get; set; }
        public int? StationId { get; set; }
        public Station Station { get; set; }
    }
}
