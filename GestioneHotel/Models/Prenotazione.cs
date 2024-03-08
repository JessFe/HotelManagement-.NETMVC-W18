using GestioneHotel.CustomValidation;
using System;

namespace GestioneHotel.Models
{
    public class Prenotazione
    {
        public int IDPrenotazione { get; set; }
        public int AnnoPreno { get; set; }
        public DateTime DataPreno { get; set; }
        public DateTime DataCheckIn { get; set; }
        public DateTime DataCheckOut { get; set; }
        public decimal Anticipo { get; set; }
        public int FK_IDCliente { get; set; }
        public int FK_NrCamera { get; set; }

        //public int FK_IDTipoPreno { get; set; }

        [CheckFormulaPreno(ErrorMessage = "Scegli tra: 'Colazione', 'Mezza Pensione', 'Pensione Completa'")]
        public string FormulaPreno { get; set; }
        //public int FK_IDRichServizio { get; set; }


    }
}