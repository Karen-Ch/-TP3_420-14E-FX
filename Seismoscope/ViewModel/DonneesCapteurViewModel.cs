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
using LiveCharts;
using LiveCharts.Wpf;
using Seismoscope.Utils.Services.Interfaces.Seismoscope.Services.Interfaces;

namespace Seismoscope.ViewModel
{
    public class DonneesCapteurViewModel : BaseViewModel, IParameterReceiver
    {
        private readonly IUserSessionService _userSession;
        private readonly ICapteurService _capteurService;
        private readonly IDialogService _dialogService;
        private readonly IEvenementService _evenementService;
        private readonly IAjustementService _ajustementService;
        private readonly IJournalService _journalService;
        private readonly INavigationService _navigationService;
        public SeriesCollection Series { get; set; } = new SeriesCollection();
        public ChartValues<double> ValeursAmplitude { get; set; } = new ChartValues<double>();
        public ObservableCollection<string> LabelsTemps { get; set; } = new ObservableCollection<string>();


        public ObservableCollection<Capteur> Capteurs { get; set; }
        public Capteur CapteurSelectionne { get; set; }
        public Station NouvelleStation { get; set; }
        public int NouveauStationId { get; set; }
        public ICommand LireCSVCommand { get; }
        public ICommand CommencerLectureCommand { get; }
        public ICommand ArreterLectureCommand { get; }


        public const int Correction = 1000;
        public string Nom { get; set; }
        public string Type { get; set; }
        public double Amplitude { get; set; }
        private double Intervalle { get; set; }
        public ObservableCollection<Tuple<string, double>> CsvDonnees { get; set; }
        public bool PeutCommencerLecture => CsvDonnees != null && CsvDonnees.Count > 0;
        private bool _estLectureEnCours;
        private readonly List<double> _lecturesRecues = new();



        public DonneesCapteurViewModel(IUserSessionService userSession,
            ICapteurService capteurService,
            IDialogService dialogService,
            IEvenementService evenementService,
            IAjustementService ajustementService,
            IJournalService journalService,
            INavigationService navigationService)
        {
            _userSession = userSession;
            _capteurService = capteurService;
            _dialogService = dialogService;
            _evenementService= evenementService;   
            _ajustementService= ajustementService;
            _journalService = journalService;   
            _navigationService = navigationService;
            Capteurs = new ObservableCollection<Capteur>(_capteurService.ObtenirTous());
            ArreterLectureCommand = new RelayCommand(ArreterLecture);
            LireCSVCommand = new RelayCommand(LireCsv);
            CommencerLectureCommand = new RelayCommand(async () => await LancerLectureAsync());
            InitialiserGraphique();
        }
        
        public bool EstLectureEnCours
        {
            get => _estLectureEnCours;
            set
            {
                _estLectureEnCours = value;
                OnPropertyChanged(nameof(EstLectureEnCours));
                OnPropertyChanged(nameof(PeutCommencerLecture)); 
            }
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
            int compteur = 0;
            EstLectureEnCours = true;
            ValeursAmplitude.Clear();
            LabelsTemps.Clear();
            foreach (var tuple in CsvDonnees)
            {
                if (!EstLectureEnCours)
                    break;
                Type = tuple.Item1;
                Amplitude = tuple.Item2;
                _lecturesRecues.Add(Amplitude);
                _ajustementService.AppliquerRegles(CapteurSelectionne, _lecturesRecues);

                OnPropertyChanged(nameof(Type));
                OnPropertyChanged(nameof(Amplitude));
                ValeursAmplitude.Add(Amplitude);
                LabelsTemps.Add($"t{compteur++}");

                OnPropertyChanged(nameof(LabelsTemps));
                OnPropertyChanged(nameof(ValeursAmplitude));

                DetecterEvenement();
                await Task.Delay(temps); 
            }
            EstLectureEnCours = false;
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
        private void InitialiserGraphique()
        {
            Series.Add(new LineSeries
            {
                Title = "Amplitude",
                Values = ValeursAmplitude,
                PointGeometry = DefaultGeometries.Circle,
                LineSmoothness = 0,
                StrokeThickness = 2
            });
        }
        public ObservableCollection<AjustementJournal> Journaux => _journalService.Journaux;

        public void ArreterLecture()
        {
            EstLectureEnCours = false;
        }


    }
}
