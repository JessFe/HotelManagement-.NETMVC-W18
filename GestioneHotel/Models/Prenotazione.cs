using GestioneHotel.CustomValidation;
using System;
using System.ComponentModel.DataAnnotations;

namespace GestioneHotel.Models
{
    public class Prenotazione
    {
        [Display(Name = "# Pr.")]
        public int IDPrenotazione { get; set; }
        [Display(Name = "Anno Pr.")]
        public int AnnoPreno { get; set; }
        [Display(Name = "Data Pr.")]
        public DateTime DataPreno { get; set; }
        [Display(Name = "Check-In")]
        public DateTime DataCheckIn { get; set; }
        [Display(Name = "Check-Out")]
        public DateTime DataCheckOut { get; set; }
        public decimal Anticipo { get; set; }
        [Display(Name = "# Cliente")]
        public int FK_IDCliente { get; set; }
        [Display(Name = "Camera")]
        public int FK_NrCamera { get; set; }


        [CheckFormulaPreno(ErrorMessage = "Scegli tra: 'Colazione', 'Mezza Pensione', 'Pensione Completa'")]
        [Display(Name = "Formula")]
        public string FormulaPreno { get; set; }

        public string Cognome { get; set; }
        public string Nome { get; set; }

        public string TipoCamera { get; set; }

        public string PrezzoServizio { get; set; }
        public string TotServizio { get; set; }


    }
}