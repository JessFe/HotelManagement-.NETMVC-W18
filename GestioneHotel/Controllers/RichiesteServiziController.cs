using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace GestioneHotel.Controllers
{
    [Authorize]
    public class RichiesteServiziController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // RICHIESTE SERVIZI INDEX
        // Visualizza l'elenco delle richieste di servizi con i dettagli del cliente e del servizio richiesto, ordinato per data del servizio
        public ActionResult Index()
        {
            List<RichiestaServizio> richiesteServizi = new List<RichiestaServizio>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per visualizzare l'elenco delle richieste di servizi
                string sql = @"
            SELECT rs.*, c.Cognome AS CognomeCliente, c.Nome AS NomeCliente, s.TipoServizio, p.FK_IDCliente
            FROM RichiesteServizi rs
            JOIN Servizi s ON rs.FK_IDServizio = s.IDServizio
            JOIN Prenotazioni p ON rs.FK_IDPrenotazione = p.IDPrenotazione
            JOIN Clienti c ON p.FK_IDCliente = c.IDCliente
            ORDER BY rs.DataServizio DESC";

                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Aggiunge la richiesta di servizio alla lista
                        richiesteServizi.Add(new RichiestaServizio
                        {
                            IDRichServizio = Convert.ToInt32(reader["IDRichServizio"]),
                            DataServizio = Convert.ToDateTime(reader["DataServizio"]),
                            QuantitaServizio = Convert.ToInt32(reader["QuantitaServizio"]),
                            FK_IDServizio = Convert.ToInt32(reader["FK_IDServizio"]),
                            NomeCliente = reader["NomeCliente"].ToString(),
                            CognomeCliente = reader["CognomeCliente"].ToString(),
                            TipoServizio = reader["TipoServizio"].ToString(),
                            FK_IDCliente = Convert.ToInt32(reader["FK_IDCliente"])

                        });

                    }
                }
                // Gestione eccezioni 
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(richiesteServizi);
        }

        // RICHIESTE SERVIZI CREATE
        // Visualizza il form per aggiungere una nuova richiesta di servizio
        public ActionResult Create()
        {
            // Popola la lista dei servizi
            PopolaServizio();
            // Popola la lista delle prenotazioni
            PopolaPrenotazioni();
            return View();
        }


        // Aggiunge una nuova richiesta di servizio
        [HttpPost]
        public ActionResult Create(RichiestaServizio richiestaServizio)
        {
            // Verifica la validità del modello
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Query per aggiungere una nuova richiesta di servizio
                    conn.Open();
                    string sql = "INSERT INTO RichiesteServizi (DataServizio, QuantitaServizio, FK_IDServizio, FK_IDPrenotazione) VALUES (@DataServizio, @QuantitaServizio, @FK_IDServizio, @FK_IDPrenotazione)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Connection = conn;

                    cmd.Parameters.AddWithValue("@DataServizio", richiestaServizio.DataServizio);
                    cmd.Parameters.AddWithValue("@QuantitaServizio", richiestaServizio.QuantitaServizio);
                    cmd.Parameters.AddWithValue("@FK_IDServizio", richiestaServizio.FK_IDServizio);
                    cmd.Parameters.AddWithValue("@FK_IDPrenotazione", richiestaServizio.FK_IDPrenotazione);

                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Non è stato possibile aggiungere la richiesta del servizio");
                        }
                    }
                    // Gestione eccezioni 
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                    }
                }
            }
            PopolaServizio(richiestaServizio.FK_IDServizio);
            PopolaPrenotazioni(richiestaServizio.FK_IDPrenotazione);
            return View(richiestaServizio);
        }

        // RICHIESTE SERVIZI EDIT
        // Visualizza il form per modificare i dati di una richiesta di servizio
        public ActionResult Edit(int id)
        {
            RichiestaServizio richiestaServizio = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare i dati della richiesta di servizio in base all'ID
                string sql = "SELECT * FROM RichiesteServizi WHERE IDRichServizio = @IDRichServizio";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@IDRichServizio", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Imposta i dati della richiesta di servizio
                        richiestaServizio = new RichiestaServizio
                        {
                            IDRichServizio = Convert.ToInt32(reader["IDRichServizio"]),
                            DataServizio = Convert.ToDateTime(reader["DataServizio"]),
                            QuantitaServizio = Convert.ToInt32(reader["QuantitaServizio"]),
                            FK_IDServizio = Convert.ToInt32(reader["FK_IDServizio"]),
                            FK_IDPrenotazione = Convert.ToInt32(reader["FK_IDPrenotazione"])
                        };
                    }
                    reader.Close();
                }
                // Gestione eccezioni 
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Popola la lista dei servizi
            PopolaServizio(richiestaServizio.FK_IDServizio);
            // Popola la lista delle prenotazioni
            PopolaPrenotazioni(richiestaServizio.FK_IDPrenotazione);
            return View(richiestaServizio);
        }

        // Modifica i dati di una richiesta di servizio
        [HttpPost]
        public ActionResult Edit(RichiestaServizio richiestaServizio)
        {
            // Verifica la validità del modello
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //  Query per modificare i dati della richiesta di servizio
                    string sql = "UPDATE RichiesteServizi SET DataServizio = @DataServizio, QuantitaServizio = @QuantitaServizio, FK_IDServizio = @FK_IDServizio, FK_IDPrenotazione = @FK_IDPrenotazione WHERE IDRichServizio = @IDRichServizio";
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@IDRichServizio", richiestaServizio.IDRichServizio);
                    cmd.Parameters.AddWithValue("@DataServizio", richiestaServizio.DataServizio);
                    cmd.Parameters.AddWithValue("@QuantitaServizio", richiestaServizio.QuantitaServizio);
                    cmd.Parameters.AddWithValue("@FK_IDServizio", richiestaServizio.FK_IDServizio);
                    cmd.Parameters.AddWithValue("@FK_IDPrenotazione", richiestaServizio.FK_IDPrenotazione);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Non è stato possibile modificare la richiesta del servizio.");
                        }
                    }
                    // Gestione eccezioni 
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                    }
                }
            }
            PopolaServizio(richiestaServizio.FK_IDServizio);
            PopolaPrenotazioni(richiestaServizio.FK_IDPrenotazione);
            return View(richiestaServizio);
        }


        // METODI

        // Popola la lista dei servizi
        private void PopolaServizio(object selectedServizio = null)
        {
            var servizi = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare l'elenco dei servizi
                string sql = "SELECT IDServizio, TipoServizio, PrezzoServizio FROM Servizi";
                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var descrizione = $"ID: {reader["IDServizio"]} - {reader["TipoServizio"]} - {reader["PrezzoServizio"]} €";
                        servizi.Add(new SelectListItem { Value = reader["IDServizio"].ToString(), Text = descrizione });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Imposta la lista dei servizi
            ViewBag.FK_IDServizio = new SelectList(servizi, "Value", "Text", selectedServizio);
        }

        // Popola la lista delle prenotazioni
        private void PopolaPrenotazioni(object selectedPrenotazione = null)
        {
            var prenotazioniList = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare l'elenco delle prenotazioni con i dati del cliente
                string sql = @"
            SELECT 
                p.IDPrenotazione, 
                c.Cognome, 
                c.Nome 
            FROM Prenotazioni p
            JOIN Clienti c ON p.FK_IDCliente = c.IDCliente";
                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Aggiorna la descrizione per includere l'ID della prenotazione e il nome e cognome del cliente
                        var descrizione = $"IDPreno: {reader["IDPrenotazione"]} - Cliente: {reader["Cognome"]} {reader["Nome"]}";
                        prenotazioniList.Add(new SelectListItem { Value = reader["IDPrenotazione"].ToString(), Text = descrizione });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Imposta la lista delle prenotazioni
            ViewBag.FK_IDPrenotazione = new SelectList(prenotazioniList, "Value", "Text", selectedPrenotazione);
        }
    }
}