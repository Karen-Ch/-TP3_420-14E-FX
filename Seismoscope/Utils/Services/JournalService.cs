using Seismoscope.Model;
using Seismoscope.Utils.Services.Interfaces.Seismoscope.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Utils.Services
{
    public class JournalService : IJournalService
    {
        public ObservableCollection<AjustementJournal> Journaux { get; } = new();

        public void Enregistrer(int capteurId, string regle, string detail)
        {
            var entree = new AjustementJournal
            {
                Date = DateTime.Now,
                CapteurId = capteurId,
                RègleAppliquée = regle,
                Détail = detail
            };

            Journaux.Add(entree);
        }
    }
}
