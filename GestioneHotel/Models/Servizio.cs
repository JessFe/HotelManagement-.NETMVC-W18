using System.Collections.Generic;

namespace GestioneHotel.Models
{
    public class Servizio
    {
        public int IDServizio { get; set; }
        public string TipoServizio { get; set; }
        public decimal PrezzoServizio { get; set; }

        // Relazione uno-a-molti con RichiesteServizi
        public virtual ICollection<RichiestaServizio> RichiesteServizi { get; set; }
    }

}