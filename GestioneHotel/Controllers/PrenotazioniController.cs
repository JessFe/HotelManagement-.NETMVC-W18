using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace GestioneHotel.Controllers
{
    [Authorize]
    public class PrenotazioniController : Controller
    {
        // Connessione al database
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // PRENOTAZIONI INDEX
        // Visualizza l'elenco delle prenotazioni, ordinato per data di check-in
        public ActionResult Index()
        {
            List<Prenotazione> prenotazioni = new List<Prenotazione>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per visualizzare l'elenco delle prenotazioni
                SqlCommand cmd = new SqlCommand("SELECT p.*, c.Cognome, c.Nome, ca.TipoCamera FROM Prenotazioni p JOIN Clienti c ON p.FK_IDCliente = c.IDCliente JOIN Camere ca ON p.FK_NrCamera = ca.NrCamera ORDER BY p.DataCheckIn DESC", conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Aggiunge la prenotazione alla lista
                        prenotazioni.Add(new Prenotazione
                        {
                            IDPrenotazione = Convert.ToInt32(reader["IDPrenotazione"]),
                            AnnoPreno = Convert.ToInt32(reader["AnnoPreno"]),
                            DataPreno = Convert.ToDateTime(reader["DataPreno"]),
                            DataCheckIn = Convert.ToDateTime(reader["DataCheckIn"]),
                            DataCheckOut = Convert.ToDateTime(reader["DataCheckOut"]),
                            Anticipo = Convert.ToDecimal(reader["Anticipo"]),
                            FK_IDCliente = Convert.ToInt32(reader["FK_IDCliente"]),
                            FK_NrCamera = Convert.ToInt32(reader["FK_NrCamera"]),
                            TipoCamera = reader["TipoCamera"].ToString(),
                            FormulaPreno = reader["FormulaPreno"].ToString(),
                            Cognome = reader["Cognome"].ToString(),
                            Nome = reader["Nome"].ToString()
                        });
                    }
                }
                // Gestione eccezioni 
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(prenotazioni);
        }

        // PRENOTAZIONI CREATE
        // Visualizza il form per l'inserimento di una nuova prenotazione
        public ActionResult Create()
        {
            // Popola le liste dropdown Cliente, Camera e TipoPrenotazione
            PopolaCliente();
            PopolaCamera();
            PopolaTipoPrenotazione();

            return View();
        }

        // Salva i dati della nuova prenotazione nel database
        [HttpPost]
        public ActionResult Create(Prenotazione prenotazione)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Query per l'inserimento di una nuova prenotazione
                SqlCommand cmd = new SqlCommand("INSERT INTO Prenotazioni (AnnoPreno, DataPreno, DataCheckIn, DataCheckOut, Anticipo, FK_IDCliente, FK_NrCamera, FormulaPreno) VALUES (@AnnoPreno, @DataPreno, @DataCheckIn, @DataCheckOut, @Anticipo, @FK_IDCliente, @FK_NrCamera, @FormulaPreno)", conn);
                cmd.Parameters.AddWithValue("@AnnoPreno", prenotazione.AnnoPreno);
                cmd.Parameters.AddWithValue("@DataPreno", prenotazione.DataPreno);
                cmd.Parameters.AddWithValue("@DataCheckIn", prenotazione.DataCheckIn);
                cmd.Parameters.AddWithValue("@DataCheckOut", prenotazione.DataCheckOut);
                cmd.Parameters.AddWithValue("@Anticipo", prenotazione.Anticipo);
                cmd.Parameters.AddWithValue("@FK_IDCliente", prenotazione.FK_IDCliente);
                cmd.Parameters.AddWithValue("@FK_NrCamera", prenotazione.FK_NrCamera);
                cmd.Parameters.AddWithValue("@FormulaPreno", prenotazione.FormulaPreno);


                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Errore nell'inserimento della prenotazione");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                }
            }
            PopolaTipoPrenotazione(prenotazione.FormulaPreno);
            PopolaCliente(prenotazione.FK_IDCliente);
            PopolaCamera(prenotazione.FK_NrCamera);
            return View(prenotazione);
        }

        // PRENOTAZIONI EDIT
        // Visualizza il form per la modifica di una prenotazione
        public ActionResult Edit(int id)
        {
            Prenotazione prenotazione = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare i dati della prenotazione in base all'ID
                SqlCommand cmd = new SqlCommand("SELECT * FROM Prenotazioni WHERE IDPrenotazione = @IDPrenotazione", conn);
                cmd.Parameters.AddWithValue("@IDPrenotazione", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Imposta i dati della prenotazione
                        prenotazione = new Prenotazione
                        {
                            IDPrenotazione = Convert.ToInt32(reader["IDPrenotazione"]),
                            AnnoPreno = Convert.ToInt32(reader["AnnoPreno"]),
                            DataPreno = Convert.ToDateTime(reader["DataPreno"]),
                            DataCheckIn = Convert.ToDateTime(reader["DataCheckIn"]),
                            DataCheckOut = Convert.ToDateTime(reader["DataCheckOut"]),
                            Anticipo = Convert.ToDecimal(reader["Anticipo"]),
                            FK_IDCliente = Convert.ToInt32(reader["FK_IDCliente"]),
                            FK_NrCamera = Convert.ToInt32(reader["FK_NrCamera"]),
                            FormulaPreno = reader["FormulaPreno"].ToString(),
                            //FK_IDRichServizio = Convert.ToInt32(reader["FK_IDRichServizio"])
                        };
                    }
                }
                // Gestione eccezioni 
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Popola le liste dropdown Cliente, Camera e TipoPrenotazione
            PopolaTipoPrenotazione(prenotazione.FormulaPreno);
            PopolaCliente(prenotazione.FK_IDCliente);
            PopolaCamera(prenotazione.FK_NrCamera);
            return View(prenotazione);
        }

        // Salva le modifiche ai dati della prenotazione nel database
        [HttpPost]
        public ActionResult Edit(Prenotazione prenotazione)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per l'aggiornamento dei dati della prenotazione
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Prenotazioni SET AnnoPreno = @AnnoPreno, DataPreno = @DataPreno, DataCheckIn = @DataCheckIn, DataCheckOut = @DataCheckOut, Anticipo = @Anticipo, FK_IDCliente = @FK_IDCliente, FK_NrCamera = @FK_NrCamera, FormulaPreno = @FormulaPreno WHERE IDPrenotazione = @IDPrenotazione", conn);
                cmd.Parameters.AddWithValue("@IDPrenotazione", prenotazione.IDPrenotazione);
                cmd.Parameters.AddWithValue("@AnnoPreno", prenotazione.AnnoPreno);
                cmd.Parameters.AddWithValue("@DataPreno", prenotazione.DataPreno);
                cmd.Parameters.AddWithValue("@DataCheckIn", prenotazione.DataCheckIn);
                cmd.Parameters.AddWithValue("@DataCheckOut", prenotazione.DataCheckOut);
                cmd.Parameters.AddWithValue("@Anticipo", string.IsNullOrEmpty(prenotazione.Anticipo.ToString()) ? (object)DBNull.Value : prenotazione.Anticipo);
                cmd.Parameters.AddWithValue("@FK_IDCliente", prenotazione.FK_IDCliente);
                cmd.Parameters.AddWithValue("@FK_NrCamera", prenotazione.FK_NrCamera);
                cmd.Parameters.AddWithValue("@FormulaPreno", prenotazione.FormulaPreno);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Errore nell'aggiornamento della prenotazione");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                }
            }
            PopolaCliente(prenotazione.FK_IDCliente);
            PopolaCamera(prenotazione.FK_NrCamera);
            PopolaTipoPrenotazione(prenotazione.FormulaPreno);
            return View(prenotazione);
        }

        // PRENOTAZIONI DETAILS
        // Visualizza i dettagli della prenotazione e l'elenco dei servizi richiesti
        public ActionResult Details(int id)
        {
            Prenotazione prenotazione = null;
            List<RichiestaServizio> richiestaServizio = new List<RichiestaServizio>();
            CheckoutDetails checkoutDetails = new CheckoutDetails();
            decimal costoSoggiorno = 0m, costoServizi = 0m, importoDovuto = 0m;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Recupero dei dati della prenotazione
                conn.Open();
                SqlCommand cmdPrenotazione = new SqlCommand(
                    "SELECT p.*, c.Cognome, c.Nome, ca.TipoCamera, ca.PrezzoCamera FROM Prenotazioni p " +
                    "INNER JOIN Clienti c ON p.FK_IDCliente = c.IDCliente " +
                    "INNER JOIN Camere ca ON p.FK_NrCamera = ca.NrCamera " +
                    "WHERE p.IDPrenotazione = @IDPrenotazione", conn);
                cmdPrenotazione.Parameters.AddWithValue("@IDPrenotazione", id);

                using (SqlDataReader reader = cmdPrenotazione.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Imposta i dati della prenotazione
                        prenotazione = new Prenotazione
                        {
                            IDPrenotazione = Convert.ToInt32(reader["IDPrenotazione"]),
                            AnnoPreno = Convert.ToInt32(reader["AnnoPreno"]),
                            DataPreno = Convert.ToDateTime(reader["DataPreno"]),
                            DataCheckIn = Convert.ToDateTime(reader["DataCheckIn"]),
                            DataCheckOut = Convert.ToDateTime(reader["DataCheckOut"]),
                            Anticipo = Convert.ToDecimal(reader["Anticipo"]),
                            FK_IDCliente = Convert.ToInt32(reader["FK_IDCliente"]),
                            FK_NrCamera = Convert.ToInt32(reader["FK_NrCamera"]),
                            FormulaPreno = reader["FormulaPreno"].ToString(),
                            Cognome = reader["Cognome"].ToString(),
                            Nome = reader["Nome"].ToString(),
                            TipoCamera = reader["TipoCamera"].ToString()
                        };

                        // Calcolo del costo soggiorno
                        int durataSoggiorno = (prenotazione.DataCheckOut - prenotazione.DataCheckIn).Days;
                        decimal prezzoCamera = Convert.ToDecimal(reader["PrezzoCamera"]);
                        string formulaPreno = prenotazione.FormulaPreno;

                        // Calcolo del costo del soggiorno in base alla formula di prenotazione
                        //  (colazione = base, mezza pensione +10%, pensione completa +20%)
                        switch (formulaPreno)
                        {
                            case "Colazione":
                                costoSoggiorno = prezzoCamera * durataSoggiorno * 1;
                                break;
                            case "Mezza Pensione":
                                costoSoggiorno = prezzoCamera * durataSoggiorno * 1.1m;
                                break;
                            case "Pensione Completa":
                                costoSoggiorno = prezzoCamera * durataSoggiorno * 1.2m;
                                break;
                            default:
                                costoSoggiorno = prezzoCamera * durataSoggiorno;
                                break;
                        }
                    }
                }

                // Recupero e calcolo del costo dei servizi richiesti
                if (prenotazione != null)
                {
                    // Recupero dei dati dei servizi richiesti
                    SqlCommand cmdServizi = new SqlCommand(
                        "SELECT rs.*, s.PrezzoServizio, s.TipoServizio FROM RichiesteServizi rs " +
                        "INNER JOIN Servizi s ON rs.FK_IDServizio = s.IDServizio " +
                        "WHERE rs.FK_IDPrenotazione = @IDPrenotazione ORDER BY rs.DataServizio DESC", conn);
                    cmdServizi.Parameters.AddWithValue("@IDPrenotazione", prenotazione.IDPrenotazione);

                    using (SqlDataReader readerServizi = cmdServizi.ExecuteReader())
                    {
                        while (readerServizi.Read())
                        {
                            // Aggiunge il servizio alla lista
                            RichiestaServizio servizio = new RichiestaServizio
                            {
                                IDRichServizio = Convert.ToInt32(readerServizi["IDRichServizio"]),
                                QuantitaServizio = Convert.ToInt32(readerServizi["QuantitaServizio"]),
                                DataServizio = Convert.ToDateTime(readerServizi["DataServizio"]),
                                FK_IDServizio = Convert.ToInt32(readerServizi["FK_IDServizio"]),
                                TipoServizio = Convert.ToString(readerServizi["TipoServizio"]),
                                PrezzoServizio = Convert.ToDecimal(readerServizi["PrezzoServizio"]),
                                TotServizio = (Convert.ToDecimal(readerServizi["PrezzoServizio"]) * Convert.ToInt32(readerServizi["QuantitaServizio"]))
                            };
                            richiestaServizio.Add(servizio);
                            costoServizi += servizio.QuantitaServizio * servizio.PrezzoServizio;
                        }
                    }
                }

                // Calcolo dell'importo dovuto
                // (costo soggiorno + costo servizi - anticipo)
                importoDovuto = costoSoggiorno + costoServizi - prenotazione.Anticipo;

                // Impostazione dei dettagli del checkout
                checkoutDetails.CostoSoggiorno = costoSoggiorno;
                checkoutDetails.CostoServizi = costoServizi;
                checkoutDetails.ImportoDovuto = importoDovuto;
            }

            // Creazione del modello per la View
            var model = new Tuple<Prenotazione, List<RichiestaServizio>, CheckoutDetails>(prenotazione, richiestaServizio, checkoutDetails);

            return View(model);
        }


        // METODI

        // Popola la lista dropdown Cliente
        private void PopolaCliente(object selectedCliente = null)
        {
            var clienti = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare l'elenco dei clienti
                string sql = "SELECT IDCliente, Cognome, Nome FROM Clienti";
                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var descrizione = $"{reader["IDCliente"]} - {reader["Cognome"]} {reader["Nome"]}";
                        clienti.Add(new SelectListItem { Value = reader["IDCliente"].ToString(), Text = descrizione });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Imposta la lista dropdown
            ViewBag.FK_IDCliente = new SelectList(clienti, "Value", "Text", selectedCliente);

        }

        // Popola la lista dropdown Camera
        private void PopolaCamera(object selectedCamera = null)
        {
            var camere = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare l'elenco delle camere
                string sql = "SELECT NrCamera, TipoCamera FROM Camere";
                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var descrizione = $"{reader["NrCamera"]} - {reader["TipoCamera"]}";
                        camere.Add(new SelectListItem { Value = reader["NrCamera"].ToString(), Text = descrizione });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Imposta la lista dropdown
            ViewBag.FK_NrCamera = new SelectList(camere, "Value", "Text", selectedCamera);

        }

        //  Popola la lista dropdown TipoPrenotazione
        private void PopolaTipoPrenotazione(string selectedFormula = null)
        {
            var tipoPrenotazioni = new List<SelectListItem>
            {
                new SelectListItem { Text = "Colazione", Value = "Colazione" },
                new SelectListItem { Text = "Mezza Pensione", Value = "Mezza Pensione" },
                new SelectListItem { Text = "Pensione Completa", Value = "Pensione Completa" }
            };
            // Imposta la lista dropdown
            ViewBag.FormulaPreno = new SelectList(tipoPrenotazioni, "Value", "Text", selectedFormula);
        }
    }
}

