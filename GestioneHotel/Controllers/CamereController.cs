using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace GestioneHotel.Controllers
{
    [Authorize]
    public class CamereController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // CAMERE INDEX
        // Visualizza l'elenco delle camere con il totale delle prenotazioni, ordinato per numero di camera
        public ActionResult Index()
        {
            List<Camera> camere = new List<Camera>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per visualizzare l'elenco delle camere
                SqlCommand cmd = new SqlCommand(
            "SELECT c.*, (SELECT COUNT(*) FROM Prenotazioni WHERE FK_NrCamera = c.NrCamera) AS TotalePrenotazioni FROM Camere c ORDER BY c.NrCamera ASC",
            conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Aggiunge la camera alla lista
                        camere.Add(new Camera
                        {
                            NrCamera = Convert.ToInt32(reader["NrCamera"]),
                            TipoCamera = reader["TipoCamera"].ToString(),
                            PrezzoCamera = Convert.ToDecimal(reader["PrezzoCamera"]),
                            TotalePrenotazioni = Convert.ToInt32(reader["TotalePrenotazioni"])
                        });
                    }
                }
                // Gestione eccezioni                
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(camere);
        }

        // CAMERE EDIT
        // Ottiene i dati della camera da modificare
        public ActionResult Edit(int id)
        {
            Camera camera = new Camera();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare la camera da modificare in base al numero di camera
                SqlCommand cmd = new SqlCommand("SELECT * FROM Camere WHERE NrCamera = @NrCamera", conn);
                cmd.Parameters.AddWithValue("@NrCamera", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Imposta i dati della camera
                        camera = new Camera
                        {
                            NrCamera = Convert.ToInt32(reader["NrCamera"]),
                            TipoCamera = reader["TipoCamera"].ToString(),
                            PrezzoCamera = Convert.ToDecimal(reader["PrezzoCamera"])
                        };
                    }
                }
                // Gestione eccezioni
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(camera);
        }

        // Salva i dati modificati della camera
        [HttpPost]
        public ActionResult Edit(Camera camera)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Query per aggiornare i dati della camera
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
                // Gestione eccezioni
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                }
            }
            return View(camera);
        }

        // CAMERE DETAILS
        // Visualizza i dettagli della camera e l'elenco delle prenotazioni
        public ActionResult Details(int id)
        {
            Camera camera = null;
            List<Prenotazione> prenotazioni = new List<Prenotazione>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Query per selezionare i dati della camera in base al numero di camera
                SqlCommand cmd = new SqlCommand("SELECT * FROM Camere WHERE NrCamera = @NrCamera", conn);
                cmd.Parameters.AddWithValue("@NrCamera", id);

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Imposta i dati della camera
                        camera = new Camera
                        {
                            NrCamera = Convert.ToInt32(reader["NrCamera"]),
                            TipoCamera = reader["TipoCamera"].ToString(),
                            PrezzoCamera = Convert.ToDecimal(reader["PrezzoCamera"])
                        };
                    }
                    reader.Close();
                }
                // Gestione eccezioni
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // Query per selezionare l'elenco delle prenotazioni in base al numero di camera
                string sqlPrenotazioni = "SELECT * FROM Prenotazioni WHERE FK_NrCamera = @NrCamera ORDER BY DataCheckIn DESC";
                SqlCommand cmdPrenotazioni = new SqlCommand(sqlPrenotazioni, conn);
                cmdPrenotazioni.Parameters.AddWithValue("@NrCamera", id);

                try
                {
                    SqlDataReader reader = cmdPrenotazioni.ExecuteReader();
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
                            FormulaPreno = reader["FormulaPreno"].ToString()
                        });
                    }
                    reader.Close();
                }
                // Gestione eccezioni
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                // Crea un modello con i dati della camera e l'elenco delle prenotazioni
                var model = new Tuple<Camera, List<Prenotazione>>(camera, prenotazioni);
                return View(model);
            }
        }
    }
}