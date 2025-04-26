using Seismoscope.Enums;
using Seismoscope.Model;
using Seismoscope.Utils.Commands;
using Seismoscope.Utils.Services.Interfaces;
using Seismoscope.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Seismoscope.ViewModel
{
    public class HistoriqueEvenementsViewModel : BaseViewModel, IParameterReceiver
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IEvenementService _evenementService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;

        public ObservableCollection<EvenementSismique> Evenements { get; private set; } = new();
        public ObservableCollection<EvenementSismique> TousEvenements { get; private set; } = new();

        public ICommand FiltrerCommand { get; }
        public ICommand ReinitialiserFiltreCommand { get; }

        public TypeOnde? TypeOndeSelectionne { get; set; }

        public HistoriqueEvenementsViewModel(
            IUserSessionService userSessionService,
            IEvenementService evenementService,
            INavigationService navigationService,
            IDialogService dialogService)
        {
            _userSessionService = userSessionService;
            _evenementService = evenementService;
            _navigationService = navigationService;
            _dialogService = dialogService;

            FiltrerCommand = new RelayCommand(FiltrerEvenements);
            ReinitialiserFiltreCommand = new RelayCommand(ReinitialiserFiltre);
        }

        public void Receive(object parameter)
        {
            ChargerEvenements();
        }

        private void ChargerEvenements()
        {
            if (_userSessionService.ConnectedUser == null || _userSessionService.ConnectedUser.StationId == null)
                return;

            TousEvenements.Clear();
            Evenements.Clear();

            var evenements = _evenementService.ObtenirParStation(_userSessionService.ConnectedUser.StationId.Value);

            foreach (var evenement in evenements)
            {
                TousEvenements.Add(evenement);
                Evenements.Add(evenement);
            }
        }


        private void FiltrerEvenements()
        {
            if (TypeOndeSelectionne == null)
                return;

            var filtres = TousEvenements
                .Where(e => e.TypeOnde == TypeOndeSelectionne)
                .ToList();

            Evenements.Clear();
            foreach (var evenement in filtres)
            {
                Evenements.Add(evenement);
            }
        }

        private void ReinitialiserFiltre()
        {
            TypeOndeSelectionne = null;
            OnPropertyChanged(nameof(TypeOndeSelectionne));

            Evenements.Clear();
            foreach (var evenement in TousEvenements)
            {
                Evenements.Add(evenement);
            }
        }
    }
}
