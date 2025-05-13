using Seismoscope.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Utils.Services.Interfaces
{
    namespace Seismoscope.Services.Interfaces
    {
        public interface IJournalService
        {
            void Enregistrer(int capteurId, string regle, string detail);
            ObservableCollection<AjustementJournal> Journaux { get; }
        }
    }

}
