using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace GestioneHotel.Controllers
{
    public class RichiesteServiziController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // GET: RichiesteServizi
        public ActionResult Index()
        {
            List<RichiestaServizio> richiesteServizi = new List<RichiestaServizio>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM RichiesteServizi";

                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        richiesteServizi.Add(new RichiestaServizio
                        {
                            IDRichServizio = Convert.ToInt32(reader["IDRichServizio"]),
                            DataServizio = Convert.ToDateTime(reader["DataServizio"]),
                            QuantitaServizio = Convert.ToInt32(reader["QuantitaServizio"]),
                            FK_IDServizio = Convert.ToInt32(reader["FK_IDServizio"]),
                            FK_IDPrenotazione = Convert.ToInt32(reader["FK_IDPrenotazione"])

                        });

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(richiesteServizi);
        }

        public ActionResult Create()
        {
            PopolaServizio();
            PopolaPrenotazioni();
            return View();
        }

        [HttpPost]
        public ActionResult Create(RichiestaServizio richiestaServizio)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
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

        public ActionResult Edit(int id)
        {
            RichiestaServizio richiestaServizio = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM RichiesteServizi WHERE IDRichServizio = @IDRichServizio";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@IDRichServizio", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            PopolaServizio(richiestaServizio.FK_IDServizio);
            PopolaPrenotazioni(richiestaServizio.FK_IDPrenotazione);
            return View(richiestaServizio);
        }

        [HttpPost]
        public ActionResult Edit(RichiestaServizio richiestaServizio)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
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

        private void PopolaServizio(object selectedServizio = null)
        {
            var servizi = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
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
            ViewBag.FK_IDServizio = new SelectList(servizi, "Value", "Text", selectedServizio);
        }

        private void PopolaPrenotazioni(object selectedPrenotazione = null)
        {
            var prenotazioniList = new List<SelectListItem>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
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
            ViewBag.FK_IDPrenotazione = new SelectList(prenotazioniList, "Value", "Text", selectedPrenotazione);
        }
    }
}