using System;
using System.ComponentModel.DataAnnotations;

namespace GestioneHotel.Models
{
    public class RichiestaServizio
    {
        [Display(Name = "# Richiesta")]
        public int IDRichServizio { get; set; }
        [Display(Name = "Data Servizio")]
        public DateTime DataServizio { get; set; }
        [Display(Name = "Quantità")]
        public int QuantitaServizio { get; set; }
        [Display(Name = "Servizio")]
        public int FK_IDServizio { get; set; }

        // Relazione molti-a-uno con Servizi
        public virtual Servizio Servizio { get; set; }
        public int AnnoPreno { get; set; }
        [Display(Name = "Prenotazione")]
        public int FK_IDPrenotazione { get; set; }
        public string NomeCliente { get; set; }
        public string CognomeCliente { get; set; }
        [Display(Name = "Servizio")]
        public string TipoServizio { get; set; }

        public int FK_IDCliente { get; set; }
        public decimal PrezzoServizio { get; internal set; }
        public decimal TotServizio { get; internal set; }
    }

}