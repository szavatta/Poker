using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Poker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Poker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration iConfig)
        {
            _logger = logger;
            configuration = iConfig;
        }

        public IActionResult Index()
        {
            if (Partita.PartitaCorrente == null || Partita.PartitaCorrente.Giocatori.Count < 4)
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionId")))
                    HttpContext.Session.SetString("SessionId", HttpContext.Session.Id);

                decimal soldiIniziali = 0;
                decimal puntata = 0;
                try { soldiIniziali = Convert.ToDecimal(configuration.GetSection("SoldiIniziali").Value); }
                catch { }
                try { puntata = Convert.ToDecimal(configuration.GetSection("Puntata").Value); }
                catch { }

                Partita.NuovaPartita(HttpContext.Session.GetString("SessionId"), soldiIniziali, puntata);
                ViewBag.IdGiocatore = Partita.PartitaCorrente.Giocatori.Where(q => q.SessionId == HttpContext.Session.GetString("SessionId")).FirstOrDefault()?.Id;
            }
            else
            {
                ViewBag.IdGiocatore = -1;
                Partita.AggiungiLog("Un visitatore si è unito alla partita");
            }

            return View(Partita.PartitaCorrente);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public JsonResult NuovaPartita()
        {
            Partita.PartitaCorrente = null;
            Partita.NuovaPartita();

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult DistribuisciCarte()
        {
            Partita.PartitaCorrente.DistribuisciCarte();

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult PescaCartaTavolo()
        {
            Partita.PartitaCorrente.PescaCartaTavolo();

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult ModificaNomeGiocatore(int id, string nome)
        {
            string old = Partita.PartitaCorrente.Giocatori[id].Nome;
            Partita.PartitaCorrente.Giocatori[id].Nome = nome;
            Partita.AggiungiLog($"Il giocatore {old} ha cambiato il nome in {nome}");

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult Puntata(int id, decimal importo)
        {
            Partita.PartitaCorrente.Giocatori[id].Punta(importo);

            return Json(new { partita = Partita.PartitaCorrente });
        }

        //private string VerificaPuntate()
        //{
        //    string messaggio = string.Empty;
        //    //Verifica se hanno puntato tutti uguale oppure se hanno fatto tutti check
        //    if (Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata) == Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Min(q => q.Puntata) && Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata) > 0
        //        || Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Count() == Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito && q.Check).Count())
        //    {
        //        Partita.PartitaCorrente.Giocatori.ForEach(q => { q.Puntata = 0; q.Check = false; });
        //        if (Partita.PartitaCorrente.Tavolo.Carte.Count < 5 && Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Count() > 1)
        //        {
        //            int num = Partita.PartitaCorrente.Tavolo.Carte.Count == 0 ? 3 : 1;
        //            Partita.AggiungiLog($"Pescat{(num > 0 ? "e" : "a")} {num} cart{(num > 0 ? "e" : "a")} sul tavolo");
        //            Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo, num, 1);
        //        }
        //        else
        //        {
        //            Partita.PartitaCorrente.Stato = Partita.EnumStato.CambioMazziere;
        //            List<Giocatore> vincitori = Partita.PartitaCorrente.GetVincitori();

        //            string sep = "Mano vinta da ";
        //            foreach (Giocatore v in vincitori)
        //            {
        //                messaggio += sep + v.Nome;
        //                v.Credito += Partita.PartitaCorrente.Tavolo.Credito / vincitori.Count;
        //                sep = " e da ";
        //            }
        //            messaggio += $" con {vincitori[0].Punteggio?.Tipo}{(vincitori[0].Punteggio?.Seme != null ? " di " + vincitori[0].Punteggio.Seme.ToString() : "")} {(vincitori[0].Punteggio?.Numero1 != null ? " di " + vincitori[0].Punteggio.Numero1.ToString() : "")} {(vincitori[0].Punteggio?.Numero2 != null ? " e " + vincitori[0].Punteggio.Numero2.ToString() : "")}";
        //            Partita.AggiungiLog(messaggio);


        //            Partita.PartitaCorrente.Tavolo.Credito = 0;
        //            Partita.PartitaCorrente.Stato = Partita.EnumStato.CambioMazziere;
        //            Partita.PartitaCorrente.SetNextMazziere();
        //        }
        //    }

        //    return messaggio;
        //}

        public JsonResult Passa(int id)
        {
            Partita.PartitaCorrente.Giocatori[id].Passa();

            return Json(new { partita = Partita.PartitaCorrente });
        }

        public JsonResult Check(int id)
        {
            Partita.PartitaCorrente.Giocatori[id].Check();

            return Json(new { partita = Partita.PartitaCorrente });
        }

        public JsonResult Vedi(int id)
        {
            Partita.PartitaCorrente.Giocatori[id].Vedi();

            return Json(new { partita = Partita.PartitaCorrente });
        }


        public JsonResult GetVincitore()
        {
            List<Giocatore> lista = new List<Giocatore>(Partita.PartitaCorrente.Giocatori);
            lista.Sort();

            return Json(lista.FirstOrDefault()?.Id);
        }

        public JsonResult GetPartita()
        {
            return Json(Partita.PartitaCorrente);
        }

    }
}
