using System.Collections.Generic;

namespace GestioneHotel.Models
{
    public class Cliente
    {
        public int IDCliente { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string CodFiscale { get; set; }
        public string Citta { get; set; }
        public string Prov { get; set; }
        public string Tel { get; set; }
        public string Cell { get; set; }
        public string Email { get; set; }

        // Relazione uno-a-molti con Prenotazioni
        public virtual ICollection<Prenotazione> Prenotazioni { get; set; }
    }

}