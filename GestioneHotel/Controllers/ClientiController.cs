using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace GestioneHotel.Controllers
{
    public class ClientiController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // GET: Clienti
        public ActionResult Index()
        {
            List<Cliente> clienti = new List<Cliente>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Clienti ORDER BY Cognome ASC", conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        clienti.Add(new Cliente
                        {
                            IDCliente = Convert.ToInt32(reader["IDCliente"]),
                            Cognome = reader["Cognome"].ToString(),
                            Nome = reader["Nome"].ToString(),
                            CodFiscale = reader["CodFiscale"].ToString(),
                            Citta = reader["Citta"].ToString(),
                            Prov = reader["Prov"].ToString(),
                            Tel = reader["Tel"].ToString(),
                            Cell = reader["Cell"].ToString(),
                            Email = reader["Email"].ToString()
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(clienti);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Clienti (Cognome, Nome, CodFiscale, Citta, Prov, Tel, Cell, Email) VALUES (@Cognome, @Nome, @CodFiscale, @Citta, @Prov, @Tel, @Cell, @Email)", conn);
                cmd.Parameters.AddWithValue("@Cognome", cliente.Cognome);
                cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
                cmd.Parameters.AddWithValue("@CodFiscale", cliente.CodFiscale);
                cmd.Parameters.AddWithValue("@Citta", cliente.Citta);
                cmd.Parameters.AddWithValue("@Prov", cliente.Prov);
                cmd.Parameters.AddWithValue("@Tel", string.IsNullOrEmpty(cliente.Tel) ? (object)DBNull.Value : cliente.Tel);
                cmd.Parameters.AddWithValue("@Cell", string.IsNullOrEmpty(cliente.Cell) ? (object)DBNull.Value : cliente.Cell);
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(cliente.Email) ? (object)DBNull.Value : cliente.Email);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Errore nell'inserimento del cliente");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                }
            }
            return View(cliente);
        }

        public ActionResult Edit(int id)
        {
            Cliente cliente = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Clienti WHERE IDCliente = @IDCliente", conn);
                cmd.Parameters.AddWithValue("@IDCliente", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        cliente = new Cliente
                        {
                            IDCliente = (int)reader["IDCliente"],
                            Cognome = reader["Cognome"].ToString(),
                            Nome = reader["Nome"].ToString(),
                            CodFiscale = reader["CodFiscale"].ToString(),
                            Citta = reader["Citta"].ToString(),
                            Prov = reader["Prov"].ToString(),
                            Tel = reader["Tel"].ToString(),
                            Cell = reader["Cell"].ToString(),
                            Email = reader["Email"].ToString()
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(cliente);
        }

        [HttpPost]
        public ActionResult Edit(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Clienti SET Cognome = @Cognome, Nome = @Nome, CodFiscale = @CodFiscale, Citta = @Citta, Prov = @Prov, Tel = @Tel, Cell = @Cell, Email = @Email WHERE IDCliente = @IDCliente", conn);
                cmd.Parameters.AddWithValue("@IDCliente", cliente.IDCliente);
                cmd.Parameters.AddWithValue("@Cognome", cliente.Cognome);
                cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
                cmd.Parameters.AddWithValue("@CodFiscale", cliente.CodFiscale);
                cmd.Parameters.AddWithValue("@Citta", cliente.Citta);
                cmd.Parameters.AddWithValue("@Prov", cliente.Prov);
                cmd.Parameters.AddWithValue("@Tel", string.IsNullOrEmpty(cliente.Tel) ? (object)DBNull.Value : cliente.Tel);
                cmd.Parameters.AddWithValue("@Cell", string.IsNullOrEmpty(cliente.Cell) ? (object)DBNull.Value : cliente.Cell);
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(cliente.Email) ? (object)DBNull.Value : cliente.Email);

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Errore nell'aggiornamento del cliente");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                }
            }
            return View(cliente);
        }

        public ActionResult Details(int id)
        {
            Cliente cliente = null;
            List<Prenotazione> prenotazioni = new List<Prenotazione>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Clienti WHERE IDCliente = @IDCliente", conn);
                cmd.Parameters.AddWithValue("@IDCliente", id);

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        cliente = new Cliente
                        {
                            IDCliente = Convert.ToInt32(reader["IDCliente"]),
                            Cognome = reader["Cognome"].ToString(),
                            Nome = reader["Nome"].ToString(),
                            CodFiscale = reader["CodFiscale"].ToString(),
                            Citta = reader["Citta"].ToString(),
                            Prov = reader["Prov"].ToString(),
                            Tel = reader["Tel"].ToString(),
                            Cell = reader["Cell"].ToString(),
                            Email = reader["Email"].ToString()
                        };
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                string sqlPrenotazioni = "SELECT * FROM Prenotazioni WHERE FK_IDCliente = @IDCliente ORDER BY DataCheckIn DESC";
                SqlCommand cmdPrenotazioni = new SqlCommand(sqlPrenotazioni, conn);
                cmdPrenotazioni.Parameters.AddWithValue("@IDCliente", id);

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
            }
            var model = new Tuple<Cliente, List<Prenotazione>>(cliente, prenotazioni);

            return View(model);
        }
    }
}