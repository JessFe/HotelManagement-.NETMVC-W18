using GestioneHotel.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace GestioneHotel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connStringDb"].ConnectionString;




        public JsonResult CercaPrenotazioniPerCF(string codiceFiscale)
        {
            List<Prenotazione> prenotazioni = new List<Prenotazione>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT p.* FROM Prenotazioni p 
                        INNER JOIN Clienti c ON p.FK_IDCliente = c.IDCliente 
                        WHERE c.CodFiscale = @CodFiscale";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@CodFiscale", codiceFiscale);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        prenotazioni.Add(new Prenotazione
                        {
                            IDPrenotazione = (int)reader["IDPrenotazione"],
                            AnnoPreno = (int)reader["AnnoPreno"],
                            DataPreno = (DateTime)reader["DataPreno"],
                            DataCheckIn = (DateTime)reader["DataCheckIn"],
                            DataCheckOut = (DateTime)reader["DataCheckOut"],
                            Anticipo = (decimal)reader["Anticipo"],
                            FK_IDCliente = (int)reader["FK_IDCliente"],
                            FK_NrCamera = (int)reader["FK_NrCamera"],
                            FormulaPreno = reader["FormulaPreno"].ToString()
                        });
                    }
                }
            }
            return Json(prenotazioni, JsonRequestBehavior.AllowGet);
        }


        public JsonResult NumeroPrenotazioniPensioneCompleta()
        {
            int numeroPrenotazioni = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM Prenotazioni WHERE FormulaPreno = 'Pensione Completa'";
                SqlCommand cmd = new SqlCommand(sql, conn);

                numeroPrenotazioni = (int)cmd.ExecuteScalar();
            }
            return Json(numeroPrenotazioni, JsonRequestBehavior.AllowGet);
        }

    }
}