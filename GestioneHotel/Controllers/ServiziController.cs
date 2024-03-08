using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace GestioneHotel.Controllers
{
    public class ServiziController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // GET: Servizi
        public ActionResult Index()
        {
            List<Servizio> servizi = new List<Servizio>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Servizi", conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        servizi.Add(new Servizio
                        {
                            IDServizio = Convert.ToInt32(reader["IDServizio"]),
                            TipoServizio = reader["TipoServizio"].ToString(),
                            PrezzoServizio = Convert.ToDecimal(reader["PrezzoServizio"])
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(servizi);
        }

        public ActionResult Edit(int id)
        {
            Servizio servizio = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Servizi WHERE IDServizio = @IDServizio", conn);
                cmd.Parameters.AddWithValue("@IDServizio", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        servizio = new Servizio
                        {
                            IDServizio = Convert.ToInt32(reader["IDServizio"]),
                            TipoServizio = reader["TipoServizio"].ToString(),
                            PrezzoServizio = Convert.ToDecimal(reader["PrezzoServizio"])
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(servizio);
        }

        [HttpPost]
        public ActionResult Edit(Servizio servizio)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
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
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                }
            }
            return View(servizio);
        }

        public ActionResult Details(int id)
        {
            Servizio servizio = null;
            List<RichiestaServizio> richiestaServizio = new List<RichiestaServizio>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Servizi WHERE IDServizio = @IDServizio", conn);
                cmd.Parameters.AddWithValue("@IDServizio", id);

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        servizio = new Servizio
                        {
                            IDServizio = Convert.ToInt32(reader["IDServizio"]),
                            TipoServizio = reader["TipoServizio"].ToString(),
                            PrezzoServizio = Convert.ToDecimal(reader["PrezzoServizio"])
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                string sqlRichiesteServizi = "SELECT * FROM RichiesteServizi WHERE FK_IDServizio = @IDServizio ORDER BY DataServizio DESC";
                SqlCommand cmdRichiesteServizi = new SqlCommand(sqlRichiesteServizi, conn);
                cmdRichiesteServizi.Parameters.AddWithValue("@IDServizio", id);

                try
                {
                    SqlDataReader reader = cmdRichiesteServizi.ExecuteReader();
                    while (reader.Read())
                    {
                        richiestaServizio.Add(new RichiestaServizio
                        {
                            IDRichServizio = Convert.ToInt32(reader["IDRichServizio"]),
                            DataServizio = Convert.ToDateTime(reader["DataServizio"]),
                            QuantitaServizio = Convert.ToInt32(reader["QuantitaServizio"]),
                            FK_IDServizio = Convert.ToInt32(reader["FK_IDServizio"]),
                            FK_IDPrenotazione = Convert.ToInt32(reader["FK_IDPrenotazione"])
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            var model = new Tuple<Servizio, List<RichiestaServizio>>(servizio, richiestaServizio);
            return View(model);
        }
    }
}