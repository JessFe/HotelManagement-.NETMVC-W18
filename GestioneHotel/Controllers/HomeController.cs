using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Security;

namespace GestioneHotel.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // HOME INDEX
        // Visualizza la home page
        public ActionResult Index()
        {
            return View();
        }

        // LOGIN
        // Visualizza il form di login
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // Esegue il login
        [AllowAnonymous]
        [HttpPost]

        public ActionResult Login(Login model, string returnUrl)
        {
            // Verifica che il modello sia valido
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Query per verificare se esiste un utente con le credenziali inserite
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Admin WHERE Username = @Username AND Password = @Password", conn);
                    cmd.Parameters.AddWithValue("@Username", model.Username);
                    cmd.Parameters.AddWithValue("@Password", model.Password);

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        // Se esiste un utente con le credenziali inserite, autentica l'utente
                        if (reader.HasRows)
                        {
                            FormsAuthentication.SetAuthCookie(model.Username, false);

                            // Reindirizza l'utente alla pagina richiesta, se è stato tentato l'accesso ad una pagina specifica
                            if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl); // Assicurati che questo reindirizzi correttamente
                            }
                            // altrimenti rimanda alla home
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        // se non esiste un utente coi dati inseriti mostra errore
                        else
                        {
                            ModelState.AddModelError("", "Il nome utente o la password non sono corretti.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return View(model);
        }

        // LOGOUT
        // Esegue il logout
        public ActionResult Logout()
        {
            // Cancella cookie di autenticazione
            FormsAuthentication.SignOut();
            Session.Clear();

            // Reindirizza alla pagina di login
            return RedirectToAction("Login", "Home");
        }


        // METODI

        // Visualizza la lista delle prenotazioni effettuate da un cliente in base al codice fiscale
        public JsonResult CercaPrenotazioniPerCF(string codiceFiscale)
        {
            List<Prenotazione> prenotazioni = new List<Prenotazione>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Query per selezionare le prenotazioni in base al codice fiscale, ordinate per data di check-in
                string sql = @"SELECT p.* FROM Prenotazioni p 
                        INNER JOIN Clienti c ON p.FK_IDCliente = c.IDCliente 
                        WHERE c.CodFiscale = @CodFiscale
                        ORDER BY p.datacheckin DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@CodFiscale", codiceFiscale);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Aggiunge la prenotazione alla lista
                        prenotazioni.Add(new Prenotazione
                        {
                            IDPrenotazione = (int)reader["IDPrenotazione"],
                            AnnoPreno = (int)reader["AnnoPreno"],
                            DataPreno = (DateTime)reader["DataPreno"],
                            DataCheckIn = (DateTime)reader["DataCheckIn"],
                            DataCheckOut = (DateTime)reader["DataCheckOut"],
                            Anticipo = (decimal)reader["Anticipo"],
                            FK_IDCliente = (int)reader["FK_IDCliente"],
                            FK_NrCamera = (int)reader["FK_NrCamera"],
                            FormulaPreno = reader["FormulaPreno"].ToString()
                        });
                    }
                }
            }
            return Json(prenotazioni, JsonRequestBehavior.AllowGet);
        }

        // Visualizza il numero di prenotazioni effettuate in base alla formula Pensione Completa
        public JsonResult NumeroPrenotazioniPensioneCompleta()
        {
            int numeroPrenotazioni = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Query per selezionare il numero di prenotazioni in base alla formula Pensione Completa
                string sql = "SELECT COUNT(*) FROM Prenotazioni WHERE FormulaPreno = 'Pensione Completa'";
                SqlCommand cmd = new SqlCommand(sql, conn);

                numeroPrenotazioni = (int)cmd.ExecuteScalar();
            }
            return Json(numeroPrenotazioni, JsonRequestBehavior.AllowGet);
        }

    }
}