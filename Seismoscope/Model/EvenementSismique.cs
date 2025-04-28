using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seismoscope.Enums;

namespace Seismoscope.Model
{
    public class EvenementSismique
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Station")]
        public int StationId { get; set; }

        [Required]
        public DateTime DateEvenement { get; set; }

        [Required]
        public double Amplitude { get; set; }

        [Required]
        public TypeOnde TypeOnde { get; set; } 

        [Required]
        public double SeuilAtteint { get; set; }

        public Station? Station { get; set; }
    }
}
