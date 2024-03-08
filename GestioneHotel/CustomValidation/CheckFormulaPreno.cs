using System;
using System.ComponentModel.DataAnnotations;

namespace GestioneHotel.CustomValidation
{
    public class CheckFormulaPreno : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var formulaPreno = value as string;

            // Formule di prenotazione valide
            string[] valideFormulePreno = { "Colazione", "Mezza Pensione", "Pensione Completa" };

            if (Array.Exists(valideFormulePreno, element => element == formulaPreno))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessage ?? "Scegli tra: 'Colazione', 'Mezza Pensione', 'Pensione Completa'");
            }
        }
    }
}