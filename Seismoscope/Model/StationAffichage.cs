using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Seismoscope.Model
{
    public class StationAffichage
    {
        public int Id { get; set; }

        public SolidColorBrush CouleurEtat { get; set; } = new SolidColorBrush(Colors.Gray);

        public string EtatMessage { get; set; } = "État inconnu";

        public bool Clignote { get; set; } = false;

        public Color CouleurEtatColor => CouleurEtat.Color;
    }

}
