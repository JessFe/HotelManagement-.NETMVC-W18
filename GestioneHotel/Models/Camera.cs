using System.Collections.Generic;

namespace GestioneHotel.Models
{
    public class Camera
    {
        public int NrCamera { get; set; }
        public string TipoCamera { get; set; }
        public decimal PrezzoCamera { get; set; }

        // Relazione uno-a-molti con Prenotazioni
        public virtual ICollection<Prenotazione> Prenotazioni { get; set; }
    }

}