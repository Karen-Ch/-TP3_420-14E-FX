using Seismoscope.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Seismoscope.Model
{
    public class Station
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Localisation { get; set; }
        public string Code { get; set; }
        public string Responsable { get; set; }
        public DateTime DateInstallation { get; set; }
        public Etat Etat { get; set; }

        public ICollection<Capteur> Capteurs { get; set; }
    }
}
