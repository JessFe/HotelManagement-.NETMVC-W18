using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestioneHotel.Models
{
    public class Cliente
    {

        [Display(Name = "# Cliente")]
        public int IDCliente { get; set; }
        [Required]
        public string Cognome { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        [RegularExpression(@"^[A-Za-z]{6}[0-9]{2}[A-Za-z][0-9]{2}[A-Za-z][0-9]{3}[A-Za-z]$", ErrorMessage = "Formato Codice Fiscale non valido.")]
        [Display(Name = "Cod. Fiscale")]
        public string CodFiscale { get; set; }
        [Required]
        [Display(Name = "Città")]
        public string Citta { get; set; }
        [Required]
        [Display(Name = "Prov.")]
        public string Prov { get; set; }
        [Display(Name = "Telefono")]
        public string Tel { get; set; }
        [Display(Name = "Cellulare")]
        public string Cell { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        // Relazione uno-a-molti con Prenotazioni
        public virtual ICollection<Prenotazione> Prenotazioni { get; set; }
    }

}