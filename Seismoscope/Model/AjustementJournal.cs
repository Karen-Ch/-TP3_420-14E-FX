using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Model
{
    public class AjustementJournal
    {
        public DateTime Date { get; set; }
        public int CapteurId { get; set; } 
        public string RègleAppliquée { get; set; } = null!;
        public string Détail { get; set; } = null!;
    }
}
