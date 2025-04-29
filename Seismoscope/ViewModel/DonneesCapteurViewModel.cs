using Microsoft.Win32;
using Seismoscope.Enums;
using Seismoscope.Model;
using Seismoscope.Model.Interfaces;
using Seismoscope.Utils;
using Seismoscope.Utils.Commands;
using Seismoscope.Utils.Services;
using Seismoscope.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Seismoscope.ViewModel
{
    public class DonneesCapteurViewModel : BaseViewModel, IParameterReceiver
    {
        private readonly IUserSessionService _userSession;
        private readonly INavigationService _navigationService;
        private readonly ICapteurService _capteurService;
        private readonly IStationService _stationService;
        private readonly IDialogService _dialogService;
        private readonly IEvenementService _evenementService;



        public ObservableCollection<Capteur> Capteurs { get; set; }
        public Capteur CapteurSelectionne { get; set; }
        public Station NouvelleStation { get; set; }
        public int NouveauStationId { get; set; }
        public ICommand LireCSVCommand { get; }
        public ICommand CommencerLectureCommand { get; }

        public const int Correction = 1000;
        public string Nom { get; set; }
        public string Type { get; set; }
        public double Amplitude { get; set; }
        private double Intervalle { get; set; }
        public ObservableCollection<Tuple<string, double>> CsvDonnees { get; set; }
        public bool PeutCommencerLecture => CsvDonnees != null && CsvDonnees.Count > 0;


        public DonneesCapteurViewModel(IUserSessionService userSession,
            INavigationService navigationService,
            ICapteurService capteurService,
            IStationService stationService,
            IDialogService dialogService,
            IEvenementService evenementService)
        {
            _userSession = userSession;
            _navigationService = navigationService;
            _capteurService = capteurService;
            _stationService = stationService;
            _dialogService = dialogService;
            _evenementService= evenementService;    


            Capteurs = new ObservableCollection<Capteur>(_capteurService.ObtenirTous());

            LireCSVCommand = new RelayCommand(LireCsv);
            CommencerLectureCommand = new RelayCommand(async () => await LancerLectureAsync());
        }
        public void LireCsv()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Fichiers CSV (*.csv)|*.csv",
                Title = "Sélectionnez un fichier CSV"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    CsvDonnees = new ObservableCollection<Tuple<string, double>>(LireFichierCsv(openFileDialog.FileName));
                    OnPropertyChanged(nameof(CsvDonnees));
                    OnPropertyChanged(nameof(PeutCommencerLecture));
                }
                catch (Exception ex)
                {
                    _dialogService.ShowMessage("Erreur", $"Impossible de lire le fichier CSV : {ex.Message}");
                }
            }
        }

        private List<Tuple<string, double>> LireFichierCsv(string filename)
        {
            var donnees = new List<Tuple<string, double>>();
            using (var lire = new StreamReader(filename)) { 
                lire.ReadLine();
                while (!lire.EndOfStream) {
                    var line = lire.ReadLine();
                    var values = line.Split(',');

                    if (values != null && values[0] is string) {
                        donnees.Add(Tuple.Create(
                        values[0].Trim(),
                       double.Parse(values[1].Trim(), System.Globalization.CultureInfo.InvariantCulture)));
                    }
                }
            }
            return donnees;
        }
        public void Receive(object parameter)
        {
            if (parameter is int capteurId)
            {
                var capteur = _capteurService.ObtenirParId(capteurId);
                if (capteur != null)
                {
                    CapteurSelectionne = capteur;
                    Nom = capteur.Nom;
                    Intervalle = capteur.FrequenceCollecte;
                    NouveauStationId = capteur.StationId ?? 0;

                    OnPropertyChanged(nameof(Nom));
                }
            }
        }
        public async Task LancerLectureAsync()
        {
            var temps = (int)Intervalle * Correction;
            foreach (var tuple in CsvDonnees)
            {
                Type = tuple.Item1;
                Amplitude = tuple.Item2;

                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(Amplitude));

                DetecterEvenement();
                await Task.Delay(temps); 
            }
        }
        private void DetecterEvenement()
        {
            if (CapteurSelectionne != null && Amplitude > CapteurSelectionne.SeuilAlerte)
            {
                CreerEtEnregistrerEvenement();
            }
        }

        private void CreerEtEnregistrerEvenement()
        {
            _dialogService.ShowMessage(
                "Un événement sismique a été détecté (seuil dépassé) !",
                "Alerte");

            var evenement = new EvenementSismique
            {
                DateEvenement = DateTime.Now,
                TypeOnde = (TypeOnde)Enum.Parse(typeof(TypeOnde), Type, true), 
                Amplitude = Amplitude,
                SeuilAtteint = CapteurSelectionne.SeuilAlerte,
                StationId = CapteurSelectionne.StationId ?? 0
            };

            _evenementService.AjouterEvenement(evenement);
            if (_navigationService.CurrentView is HistoriqueEvenementsViewModel historiqueVm)
                historiqueVm.Rafraichir();
        }
    }
}
