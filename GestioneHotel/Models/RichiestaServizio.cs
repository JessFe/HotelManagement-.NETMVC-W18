using System;

namespace GestioneHotel.Models
{
    public class RichiestaServizio
    {
        public int IDRichServizio { get; set; }
        public DateTime DataServizio { get; set; }
        public int QuantitaServizio { get; set; }
        public int FK_IDServizio { get; set; }

        // Relazione molti-a-uno con Servizi
        public virtual Servizio Servizio { get; set; }
        public int FK_IDPrenotazione { get; set; }
    }

}