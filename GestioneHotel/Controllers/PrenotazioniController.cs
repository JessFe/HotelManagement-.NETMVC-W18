using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace GestioneHotel.Controllers
{
    public class PrenotazioniController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // GET: Prenotazioni
        public ActionResult Index()
        {
            List<Prenotazione> prenotazioni = new List<Prenotazione>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Prenotazioni ORDER BY DataCheckIn DESC", conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
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
                            FormulaPreno = reader["FormulaPreno"].ToString(),
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(prenotazioni);
        }

        public ActionResult Create()
        {
            PopolaTipoPrenotazione();

            return View();
        }

        [HttpPost]
        public ActionResult Create(Prenotazione prenotazione)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
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
            return View(prenotazione);
        }

        public ActionResult Edit(int id)
        {
            Prenotazione prenotazione = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Prenotazioni WHERE IDPrenotazione = @IDPrenotazione", conn);
                cmd.Parameters.AddWithValue("@IDPrenotazione", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            PopolaTipoPrenotazione(prenotazione.FormulaPreno);
            return View(prenotazione);
        }

        [HttpPost]
        public ActionResult Edit(Prenotazione prenotazione)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
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
            PopolaTipoPrenotazione(prenotazione.FormulaPreno);
            return View(prenotazione);
        }

        public ActionResult Details(int id)
        {
            Prenotazione prenotazione = null;
            List<RichiestaServizio> richiestaServizio = new List<RichiestaServizio>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Prenotazioni WHERE IDPrenotazione = @IDPrenotazione", conn);
                cmd.Parameters.AddWithValue("@IDPrenotazione", id);

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
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
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                string sqlRichiesteServizi = "SELECT * FROM RichiesteServizi WHERE FK_IDPrenotazione = @IDPrenotazione ORDER BY DataServizio DESC";
                SqlCommand cmdRichiesteServizi = new SqlCommand(sqlRichiesteServizi, conn);
                cmdRichiesteServizi.Parameters.AddWithValue("@IDPrenotazione", id);

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
            var model = new Tuple<Prenotazione, List<RichiestaServizio>>(prenotazione, richiestaServizio);

            return View(model);


        }

        // METODI

        private void PopolaTipoPrenotazione(string selectedFormula = null)
        {
            var tipoPrenotazioni = new List<SelectListItem>
            {
                new SelectListItem { Text = "Colazione", Value = "Colazione" },
                new SelectListItem { Text = "Mezza Pensione", Value = "Mezza Pensione" },
                new SelectListItem { Text = "Pensione Completa", Value = "Pensione Completa" }
            };

            ViewBag.FormulaPreno = new SelectList(tipoPrenotazioni, "Value", "Text", selectedFormula);
        }
    }
}