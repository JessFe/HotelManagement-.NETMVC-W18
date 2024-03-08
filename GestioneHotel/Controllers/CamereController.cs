using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace GestioneHotel.Controllers
{
    public class CamereController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // GET: Camere
        public ActionResult Index()
        {
            List<Camera> camere = new List<Camera>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Camere ORDER BY NrCamera ASC", conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        camere.Add(new Camera
                        {
                            NrCamera = Convert.ToInt32(reader["NrCamera"]),
                            TipoCamera = reader["TipoCamera"].ToString(),
                            PrezzoCamera = Convert.ToDecimal(reader["PrezzoCamera"])
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(camere);
        }

        public ActionResult Edit(int id)
        {
            Camera camera = new Camera();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Camere WHERE NrCamera = @NrCamera", conn);
                cmd.Parameters.AddWithValue("@NrCamera", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        camera = new Camera
                        {
                            NrCamera = Convert.ToInt32(reader["NrCamera"]),
                            TipoCamera = reader["TipoCamera"].ToString(),
                            PrezzoCamera = Convert.ToDecimal(reader["PrezzoCamera"])
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(camera);
        }

        [HttpPost]
        public ActionResult Edit(Camera camera)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Camere SET TipoCamera = @TipoCamera, PrezzoCamera = @PrezzoCamera WHERE NrCamera = @NrCamera", conn);
                cmd.Parameters.AddWithValue("@NrCamera", camera.NrCamera);
                cmd.Parameters.AddWithValue("@TipoCamera", camera.TipoCamera);
                cmd.Parameters.AddWithValue("@PrezzoCamera", camera.PrezzoCamera);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Errore nell'aggiornamento della camera");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                }
            }
            return View(camera);
        }

        public ActionResult Details(int id)
        {
            Camera camera = null;
            List<Prenotazione> prenotazioni = new List<Prenotazione>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Camere WHERE NrCamera = @NrCamera", conn);
                cmd.Parameters.AddWithValue("@NrCamera", id);

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        camera = new Camera
                        {
                            NrCamera = Convert.ToInt32(reader["NrCamera"]),
                            TipoCamera = reader["TipoCamera"].ToString(),
                            PrezzoCamera = Convert.ToDecimal(reader["PrezzoCamera"])
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                string sqlPrenotazioni = "SELECT * FROM Prenotazioni WHERE FK_NrCamera = @NrCamera ORDER BY DataCheckIn DESC";
                SqlCommand cmdPrenotazioni = new SqlCommand(sqlPrenotazioni, conn);
                cmdPrenotazioni.Parameters.AddWithValue("@NrCamera", id);

                try
                {
                    SqlDataReader reader = cmdPrenotazioni.ExecuteReader();
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
                            FormulaPreno = reader["FormulaPreno"].ToString()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                var model = new Tuple<Camera, List<Prenotazione>>(camera, prenotazioni);
                return View(model);
            }
        }
    }
}