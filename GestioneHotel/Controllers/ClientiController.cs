using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace GestioneHotel.Controllers
{
    [Authorize]
    public class ClientiController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;

        // CLIENTI INDEX
        // Visualizza l'elenco dei clienti ordinato per cognome
        public ActionResult Index()
        {
            List<Cliente> clienti = new List<Cliente>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per visualizzare l'elenco dei clienti
                SqlCommand cmd = new SqlCommand("SELECT * FROM Clienti ORDER BY Cognome ASC", conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Aggiunge il cliente alla lista
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
                // Gestione eccezioni 
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(clienti);
        }

        // CLIENTI CREATE
        // Visualizza il form per l'inserimento di un nuovo cliente
        public ActionResult Create()
        {
            return View();
        }

        // Salva i dati del nuovo cliente nel database
        [HttpPost]
        public ActionResult Create(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Query per inserire un nuovo cliente
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
                // Gestione eccezioni
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                }
            }
            return View(cliente);
        }

        // CLIENTI EDIT
        // Visualizza il form per la modifica dei dati del cliente
        public ActionResult Edit(int id)
        {
            Cliente cliente = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query per selezionare i dati del cliente in base all'ID
                SqlCommand cmd = new SqlCommand("SELECT * FROM Clienti WHERE IDCliente = @IDCliente", conn);
                cmd.Parameters.AddWithValue("@IDCliente", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Imposta i dati del cliente
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
                // Gestione eccezioni
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return View(cliente);
        }

        // Salva i dati modificati del cliente
        [HttpPost]
        public ActionResult Edit(Cliente cliente)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Query per aggiornare i dati del cliente
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
                // Gestione eccezioni
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Si è verificato un errore: {ex.Message}");
                }
            }
            return View(cliente);
        }

        // CLIENTI DETAILS
        // Visualizza i dettagli del cliente e l'elenco delle prenotazioni
        public ActionResult Details(int id)
        {
            Cliente cliente = null;
            List<Prenotazione> prenotazioni = new List<Prenotazione>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                // Query per selezionare i dati del cliente in base all'ID
                SqlCommand cmd = new SqlCommand("SELECT * FROM Clienti WHERE IDCliente = @IDCliente", conn);
                cmd.Parameters.AddWithValue("@IDCliente", id);

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        // Imposta i dati del cliente
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
                // Gestione eccezioni
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // Query per selezionare l'elenco delle prenotazioni in base all'ID del cliente
                string sqlPrenotazioni = @"
                                  SELECT P.*, C.TipoCamera
                                  FROM Prenotazioni P
                                  JOIN Camere C ON P.FK_NrCamera = C.NrCamera
                                  WHERE P.FK_IDCliente = @IDCliente
                                  ORDER BY P.DataCheckIn DESC";
                SqlCommand cmdPrenotazioni = new SqlCommand(sqlPrenotazioni, conn);
                cmdPrenotazioni.Parameters.AddWithValue("@IDCliente", id);

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
                            TipoCamera = reader["TipoCamera"].ToString(),
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
            }
            // Crea un modello con i dati del cliente e l'elenco delle prenotazioni
            var model = new Tuple<Cliente, List<Prenotazione>>(cliente, prenotazioni);

            return View(model);
        }
    }
}