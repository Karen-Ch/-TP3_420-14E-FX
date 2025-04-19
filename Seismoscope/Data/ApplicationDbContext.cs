using Microsoft.EntityFrameworkCore;
using Seismoscope.Enums;
using Seismoscope.Model;
using Seismoscope.Utils;
using Seismoscope.Utils.Services;
using System.IO;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Station> Stations { get; set; } = null!;
    public DbSet<Capteur> Capteurs { get; set; } = null!;
    public DbSet<EvenementSismique> Evenements { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Seismoscope", "Seismoscope.db");

            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            var connectionString = $"Data Source={dbPath}";
            optionsBuilder.UseSqlite(connectionString);

        }
            
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<Station>()
            .Property(s => s.Etat)
            .HasConversion<string>();

        modelBuilder.Entity<Capteur>()
            .Property(c => c.Statut)
            .HasConversion<string>();
    }

    public void SeedData()
{
    if (!Stations.Any())
    {
        Stations.AddRange(
            new Station
            {
                Id = 1,
                Nom = "Québec",
                Localisation = "Québec, QC",
                Code = "QC001",
                Responsable = "Martin Tremblay",
                DateInstallation = new DateTime(2020, 5, 10),
                Etat = Etat.Actif
            },
            new Station
            {
                Id = 2,
                Nom = "Montréal",
                Localisation = "Montréal, QC",
                Code = "MTL001",
                Responsable = "Sophie Laroche",
                DateInstallation = new DateTime(2021, 3, 15),
                Etat = Etat.Actif
            },
            new Station
            {
                Id = 3,
                Nom = "Toronto",
                Localisation = "Toronto, ON",
                Code = "TOR001",
                Responsable = "David Chan",
                DateInstallation = new DateTime(2019, 11, 20),
                Etat = Etat.Maintenance
            },
            new Station
            {
                Id = 4,
                Nom = "Calgary",
                Localisation = "Calgary, AB",
                Code = "CAL001",
                Responsable = "Emily White",
                DateInstallation = new DateTime(2022, 2, 5),
                Etat = Etat.Actif
            },
            new Station
            {
                Id = 5,
                Nom = "Vancouver",
                Localisation = "Vancouver, BC",
                Code = "VAN001",
                Responsable = "Alexandre Gagnon",
                DateInstallation = new DateTime(2023, 6, 1),
                Etat = Etat.HorsService
            }
        );
    }

    if (!Users.Any())
    {
        Users.AddRange(
            new User
            {
                Prenom = "John",
                Nom = "Doe",
                NomUtilisateur = "johndoe",
                MotDePasse = Securite.HasherMotDePasse("password123"),
                Role = Role.Employe,
                StationId = 1 
            },
            new User
            {
                Prenom = "Jane",
                Nom = "Doe",
                NomUtilisateur = "janedoe",
                MotDePasse = Securite.HasherMotDePasse("password123"),
                Role = Role.Administrateur,
                StationId = null
            }
        );
    }
    if (!Capteurs.Any())
{
    Capteurs.AddRange(
        new Capteur
        {
            Nom = "Sismographe A1",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 10.0,
            SeuilAlerte = 5.0,
            DateInstallation = new DateTime(2021, 6, 15),
            EstDesactive = false,
            EstLivre = true,
            StationId = 1
        },
        new Capteur
        {
            Nom = "Sismographe A2",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 10.0,
            SeuilAlerte = 5.0,
            DateInstallation = new DateTime(2021, 5, 10),
            EstDesactive = false,
            EstLivre = true,
            StationId = 1
        },
        new Capteur
        {
            Nom = "Sismographe A3",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 10.0,
            SeuilAlerte = 5.0,
            DateInstallation = new DateTime(2023, 6, 13),
            EstDesactive = false,
            EstLivre = true,
            StationId = 1
        },
        new Capteur
        {
            Nom = "Sismographe M1",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 5.0,
            SeuilAlerte = 50.0,
            DateInstallation = new DateTime(2021, 7, 10),
            EstDesactive = false,
            EstLivre = true,
            StationId = 2
        },
        new Capteur
        {
            Nom = "Sismographe M2",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 5.0,
            SeuilAlerte = 50.0,
            DateInstallation = new DateTime(2021, 8, 12),
            EstDesactive = false,
            EstLivre = true,
            StationId = 2
        },
        new Capteur
        {
            Nom = "Sismographe T2",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 15.0,
            SeuilAlerte = 4.2,
            DateInstallation = new DateTime(2020, 9, 22),
            EstDesactive = false,
            EstLivre = true,
            StationId = 3
        },
        new Capteur
        {
            Nom = "Sismographe T3",
            Type = "Sismographe",
            Statut = Etat.Maintenance,
            FrequenceCollecte = 15.0,
            SeuilAlerte = 4.2,
            DateInstallation = new DateTime(2019, 7, 14),
            EstDesactive = false,
            EstLivre = true,
            StationId = 3
        },
        new Capteur
        {
            Nom = "Sismographe T4",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 15.0,
            SeuilAlerte = 4.2,
            DateInstallation = new DateTime(2023, 11, 21),
            EstDesactive = false,
            EstLivre = true,
            StationId = 3
        },
        new Capteur
        {
            Nom = "Capteur de pression C1",
            Type = "Baromètre",
            Statut = Etat.HorsService,
            FrequenceCollecte = 8.0,
            SeuilAlerte = 12.0,
            DateInstallation = new DateTime(2022, 3, 5),
            EstDesactive = true,
            EstLivre = true,
            StationId = 4
        },
        new Capteur
        {
            Nom = "Capteur de pression C2",
            Type = "Baromètre",
            Statut = Etat.Actif,
            FrequenceCollecte = 8.0,
            SeuilAlerte = 12.0,
            DateInstallation = new DateTime(2024, 9, 5),
            EstDesactive = false,
            EstLivre = true,
            StationId = 4
        },
        new Capteur
        {
            Nom = "Sismographe V3",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 20.0,
            SeuilAlerte = 3.8,
            DateInstallation = new DateTime(2023, 1, 12),
            EstDesactive = false,
            EstLivre = true,
            StationId = 5
        },
        new Capteur
        {
            Nom = "Sismographe V8",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 20.0,
            SeuilAlerte = 3.8,
            DateInstallation = new DateTime(2023, 11, 12),
            EstDesactive = false,
            EstLivre = true,
            StationId = 5
        },
        new Capteur
        {
            Nom = "Sismographe T",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 20.0,
            SeuilAlerte = 3.8,
            DateInstallation = new DateTime(2023, 11, 12),
            EstDesactive = false,
            EstLivre = true,
            StationId = 5
        },
        new Capteur
        {
            Nom = "Sismographe V9",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 20.0,
            SeuilAlerte = 3.8,
            DateInstallation = new DateTime(2023, 11, 12),
            EstDesactive = false,
            EstLivre = true,
            StationId = 5
        },
        new Capteur
        {
            Nom = "Sismographe T3",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 10.0,
            SeuilAlerte = 5.0,
            DateInstallation = default,
            EstLivre = true,
            StationId = null
        },
        new Capteur
        {
            Nom = "Capteur de pression B1",
            Type = "Baromètre",
            Statut = Etat.Actif,
            FrequenceCollecte = 7.5,
            SeuilAlerte = 3.0,
            DateInstallation = default,
            EstLivre = true,
            StationId = null
        },
        new Capteur
        {
            Nom = "Sismographe SL",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 12.0,
            SeuilAlerte = 4.5,
            DateInstallation = default,
            EstLivre = true,
            StationId = null
        },
        new Capteur
        {
            Nom = "Capteur de pression Bn",
            Type = "Baromètre",
            Statut = Etat.Actif,
            FrequenceCollecte = 9.0,
            SeuilAlerte = 2.5,
            DateInstallation = default,
            EstLivre = true,
            StationId = null
        },
        new Capteur
        {
            Nom = "Sismographe S6",
            Type = "Sismographe",
            Statut = Etat.Actif,
            FrequenceCollecte = 11.0,
            SeuilAlerte = 6.0,
            DateInstallation = default,
            EstLivre = true,
            StationId = null
        }

    );
}
        
        SaveChanges();
}

}
