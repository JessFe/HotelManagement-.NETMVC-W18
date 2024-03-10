using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestioneHotel.Models
{
    public class Camera
    {
        [Display(Name = "# Camera")]
        public int NrCamera { get; set; }
        [Display(Name = "Tipo Camera")]
        public string TipoCamera { get; set; }
        [Display(Name = "Prezzo")]
        public decimal PrezzoCamera { get; set; }

        public int TotalePrenotazioni { get; set; }

        // Relazione uno-a-molti con Prenotazioni
        public virtual ICollection<Prenotazione> Prenotazioni { get; set; }
    }

}