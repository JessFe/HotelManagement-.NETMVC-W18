using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestioneHotel.Models
{
    public class Servizio
    {
        [Display(Name = "# Servizio")]
        public int IDServizio { get; set; }
        [Display(Name = "Servizio")]
        public string TipoServizio { get; set; }
        [Display(Name = "Prezzo")]
        public decimal PrezzoServizio { get; set; }

        public int TotaleRichieste { get; set; }

        // Relazione uno-a-molti con RichiesteServizi
        public virtual ICollection<RichiestaServizio> RichiesteServizi { get; set; }
    }

}