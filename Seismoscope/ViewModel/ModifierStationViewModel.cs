using Seismoscope.Enums;
using Seismoscope.Model;
using Seismoscope.Utils;
using Seismoscope.Utils.Commands;
using Seismoscope.Utils.Services;
using Seismoscope.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Seismoscope.ViewModel
{
    public class ModifierStationViewModel : BaseViewModel, IParameterReceiver
    {
        private readonly IUserSessionService _userSession;
        private readonly IStationService _stationService;
        private readonly INavigationService _navigation;
        private readonly IDialogService _dialogService;

        public ICommand SauvegarderModificationsCommand => new RelayCommand(SauvegarderModifications);
        

        public int Id { get; set; }
        public string Nom { get; set; }
        public string Localisation { get; set; }
        public string Code { get; set; }
        public string Responsable { get; set; }
        public Etat Etat { get; set; }
        public DateTime DateInstallation { get; set; }
        public Station NouvelleStation {  get; set; }

        public ObservableCollection<Station> Stations{ get; set; } = new();

        public ModifierStationViewModel(
            IUserSessionService userSession,
            IStationService stationService,
            INavigationService navigation,
            IDialogService dialogService)
        {
            _userSession = userSession;
            _stationService = stationService;
            _navigation = navigation;
            _dialogService = dialogService;
        }

        public void Receive(object parameter)
        {
            if (parameter is int stationId)
            {
                Station? station = _stationService.ObtenirParId(stationId);
                NouvelleStation = station;
                Nom = station.Nom;
                Localisation = station.Localisation;
                Code = station.Code;
                Responsable = station.Responsable;
                Etat = station.Etat;
                DateInstallation = station.DateInstallation;
   
                OnPropertyChanged(nameof(Nom));
                OnPropertyChanged(nameof(Localisation));
                OnPropertyChanged(nameof(Code));
                OnPropertyChanged(nameof(Responsable));
                OnPropertyChanged(nameof(Etat));
                OnPropertyChanged(nameof(DateInstallation));
            }
        }
   
        private void SauvegarderModifications()
        {
             var utilisateur = _userSession.ConnectedUser;
            Console.WriteLine("Début SauvegarderModifications");
            if (utilisateur.Role is Role.Administrateur) {
                try
                {
                    Console.WriteLine($"Rôle OK: {utilisateur.Role}");
                    
                    if (NouvelleStation != null)
                    {
                        NouvelleStation.Responsable = Responsable;
                        NouvelleStation.DateInstallation = DateInstallation;
                        NouvelleStation.Etat = Etat;
                        Console.WriteLine("Avant ModifierStation");
                        Console.WriteLine("Après ModifierStation");
                        _stationService.ModifierStation(NouvelleStation);
                        _dialogService.ShowMessage("Paramètres de la station mis à jour avec succès.", "Succès");
                        _navigation.NavigateTo<StationViewModel>();

                    }
                   else
                    {
                        _dialogService.ShowMessage("Erreur inattendue, essayer à nouveau .", "Erreur");
                    }  
                }
                catch (Exception ex) {
                    Console.WriteLine($"Erreur inattendue: {ex.Message}");
                   
                }
                
            }
            else
            {
                _dialogService.ShowMessage("Accès non autorisé .", "Erreur");
            }
        }
    } 
}
