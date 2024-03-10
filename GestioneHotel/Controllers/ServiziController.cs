using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace GestioneHotel.Controllers
{
    [Authorize]
    public class ServiziController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // SERVIZI INDEX
        // Visualizza l'elenco dei servizi con il totale delle richieste, ordinato per numero di richieste
        public ActionResult Index()
        {
            List<Servizio> servizi = new List<Servizio>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per visualizzare l'elenco dei servizi
                string sql = @"
                   SELECT s.IDServizio, s.TipoServizio, s.PrezzoServizio, COUNT(r.IDRichServizio) AS TotaleRichieste 
                   FROM Servizi s
                   LEFT JOIN RichiesteServizi r ON s.IDServizio = r.FK_IDServizio
                   GROUP BY s.IDServizio, s.TipoServizio, s.PrezzoServizio
                   ORDER BY COUNT(r.IDRichServizio) DESC";

                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Aggiunge il servizio alla lista
                        servizi.Add(new Servizio
                        {
                            IDServizio = Convert.ToInt32(reader["IDServizio"]),
                            TipoServizio = reader["TipoServizio"].ToString(),
                            PrezzoServizio = Convert.ToDecimal(reader["PrezzoServizio"]),
                            TotaleRichieste = Convert.ToInt32(reader["TotaleRichieste"])
                        });
                    }
                }
                // Gestione eccezioni 
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(servizi);
        }

        // SERVIZI EDIT
        // Ottiene i dati del servizio da modificare
        public ActionResult Edit(int id)
        {
            Servizio servizio = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare il servizio da modificare in base all'ID del servizio
                SqlCommand cmd = new SqlCommand("SELECT * FROM Servizi WHERE IDServizio = @IDServizio", conn);
                cmd.Parameters.AddWithValue("@IDServizio", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Imposta i dati del servizio
                        servizio = new Servizio
                        {
                            IDServizio = Convert.ToInt32(reader["IDServizio"]),
                            TipoServizio = reader["TipoServizio"].ToString(),
                            PrezzoServizio = Convert.ToDecimal(reader["PrezzoServizio"])
                        };
                    }
                }
                // Gestione eccezioni 
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(servizio);
        }

        // Salva i dati modificati del servizio
        [HttpPost]
        public ActionResult Edit(Servizio servizio)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per aggiornare i dati del servizio
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Servizi SET TipoServizio = @TipoServizio, PrezzoServizio = @PrezzoServizio WHERE IDServizio = @IDServizio", conn);
                cmd.Parameters.AddWithValue("@IDServizio", servizio.IDServizio);
                cmd.Parameters.AddWithValue("@TipoServizio", servizio.TipoServizio);
                cmd.Parameters.AddWithValue("@PrezzoServizio", servizio.PrezzoServizio);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Errore nell'aggiornamento del servizio");
                    }
                }
                // Gestione eccezioni 
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                }
            }
            return View(servizio);
        }

        // SERVIZI DETAILS
        // Visualizza i dettagli del servizio e l'elenco delle richieste
        public ActionResult Details(int id)
        {
            Servizio servizio = null;
            List<RichiestaServizio> richiestaServizio = new List<RichiestaServizio>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //  Query per selezionare il servizio in base all'ID del servizio
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Servizi WHERE IDServizio = @IDServizio", conn);
                cmd.Parameters.AddWithValue("@IDServizio", id);

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Imposta i dati del servizio
                        servizio = new Servizio
                        {
                            IDServizio = Convert.ToInt32(reader["IDServizio"]),
                            TipoServizio = reader["TipoServizio"].ToString(),
                            PrezzoServizio = Convert.ToDecimal(reader["PrezzoServizio"])
                        };
                    }
                    reader.Close();
                }
                // Gestione eccezioni 
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // Query per selezionare l'elenco delle richieste in base all'ID del servizio
                string sqlRichiesteServizi = @"
                            SELECT rs.*, p.AnnoPreno, p.IDPrenotazione, p.FK_IDCLiente, c.Cognome, c.Nome
                            FROM RichiesteServizi rs
                            JOIN Prenotazioni p ON rs.FK_IDPrenotazione = p.IDPrenotazione
                            JOIN Clienti c ON p.FK_IDCliente = c.IDCliente
                            WHERE rs.FK_IDServizio = @IDServizio
                            ORDER BY rs.DataServizio DESC";
                SqlCommand cmdRichiesteServizi = new SqlCommand(sqlRichiesteServizi, conn);
                cmdRichiesteServizi.Parameters.AddWithValue("@IDServizio", id);

                try
                {
                    SqlDataReader reader = cmdRichiesteServizi.ExecuteReader();
                    while (reader.Read())
                    {
                        // Aggiunge la richiesta alla lista
                        richiestaServizio.Add(new RichiestaServizio
                        {
                            IDRichServizio = Convert.ToInt32(reader["IDRichServizio"]),
                            DataServizio = Convert.ToDateTime(reader["DataServizio"]),
                            QuantitaServizio = Convert.ToInt32(reader["QuantitaServizio"]),
                            FK_IDServizio = Convert.ToInt32(reader["FK_IDServizio"]),
                            FK_IDPrenotazione = Convert.ToInt32(reader["FK_IDPrenotazione"]),
                            FK_IDCliente = Convert.ToInt32(reader["FK_IDCliente"]),
                            AnnoPreno = Convert.ToInt32(reader["AnnoPreno"]),
                            CognomeCliente = reader["Cognome"].ToString(),
                            NomeCliente = reader["Nome"].ToString(),

                        });
                    }
                    reader.Close();
                }
                // Gestione eccezioni
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            // Crea un modello con il servizio e l'elenco delle richieste
            var model = new Tuple<Servizio, List<RichiestaServizio>>(servizio, richiestaServizio);
            return View(model);
        }
    }
}