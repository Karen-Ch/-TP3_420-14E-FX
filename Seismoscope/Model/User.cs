using Seismoscope.Model;
using Seismoscope.Enums;

public class User
{
    public int Id { get; set; } 

    public string Prenom { get; set; } = null!;

    public string Nom { get; set; } = null!;

    public string NomUtilisateur { get; set; } = null!;

    public string MotDePasse { get; set; } = null!;

    public Role Role { get; set; } 

    public int? StationId { get; set; } = null!;

    public User() { }

    public User (int id, string prenom, string nom, string nomUtilisateur, string motDePasse, Role role, int? stationId)
    {
        Id = id;
        Prenom = prenom;
        Nom = nom;
        NomUtilisateur = nomUtilisateur;
        MotDePasse = motDePasse;
        Role = role;
        StationId = stationId;
    }
}
