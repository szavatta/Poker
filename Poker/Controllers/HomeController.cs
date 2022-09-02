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
            Partita.PartitaCorrente.Stato = Partita.EnumStato.InSvolgimento;
            Partita.PartitaCorrente.Tavolo.Carte = new List<Carta>();
            Partita.PartitaCorrente.Mazzo = new Mazzo();
            Partita.PartitaCorrente.Mazzo.CreaMazzo(true);
            //Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo, 3, 1);
            Partita.PartitaCorrente.Giocatori.ForEach(q => { q.Carte = new List<Carta>(); q.Uscito = false; q.Pesca(Partita.PartitaCorrente.Mazzo, 2); });
            Partita.AggiungiLog("Distribuite 3 carte sul tavolo e 2 per ogni giocatore");

            Partita.PartitaCorrente.SetNextMano(Partita.PartitaCorrente.IdMazziere);
            if (Partita.PartitaCorrente.Giocatori.Count > 2)
            {
                //Piccolo buio
                Partita.PartitaCorrente.Giocatori[Partita.PartitaCorrente.Mano].Punta(Partita.PartitaCorrente.Puntata / 2);
                //Grande buio
                Partita.PartitaCorrente.SetNextMano();
                Partita.PartitaCorrente.Giocatori[Partita.PartitaCorrente.Mano].Punta(Partita.PartitaCorrente.Puntata);
            }
            else
            {
                //Piccolo buio
                Partita.PartitaCorrente.Giocatori[Partita.PartitaCorrente.IdMazziere].Punta(Partita.PartitaCorrente.Puntata / 2);
                //Grande buio
                Partita.PartitaCorrente.Giocatori[Partita.PartitaCorrente.Mano].Punta(Partita.PartitaCorrente.Puntata);
            }

            Partita.PartitaCorrente.SetNextMano();
            Partita.AggiungiLog($"La mano è passata al giocatore {Partita.PartitaCorrente.Giocatori[Partita.PartitaCorrente.Mano].Nome}");

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult PescaCartaTavolo()
        {
            if (Partita.PartitaCorrente.Tavolo.Carte.Count < 5)
            {
                int num = Partita.PartitaCorrente.Tavolo.Carte?.Count < 3 ? 3 : 1;
                Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo, num, 1);
                Partita.PartitaCorrente.Giocatori.ForEach(q => q.SetPunteggio(Partita.PartitaCorrente.Tavolo));
                Partita.AggiungiLog($"Pescata {num} carta sul tavolo");
            }

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
            var giocatore = Partita.PartitaCorrente.Giocatori[id];
            if (DiffPuntata(importo, giocatore.Puntata) > 0 && DiffPuntata(importo, giocatore.Puntata) < Partita.PartitaCorrente.Puntata)
                throw new Exception("Puntata non sufficiente");

            giocatore.Punta(importo);
            Partita.PartitaCorrente.SetNextMano();

            VerificaPuntate();

            return Json(new { partita = Partita.PartitaCorrente });
        }

        private static decimal DiffPuntata(decimal importo, decimal puntata)
        {
            return importo + puntata - Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata);
        }


        private void VerificaPuntate()
        {
            string messaggio = string.Empty;
            //Verifica se hanno puntato tutti uguale
            if (Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata) == Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Min(q => q.Puntata))
            {
                Partita.PartitaCorrente.Giocatori.ForEach(q => q.Puntata = 0);
                if (Partita.PartitaCorrente.Tavolo.Carte.Count < 5 && Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Count() > 1)
                {
                    int num = Partita.PartitaCorrente.Tavolo.Carte.Count == 0 ? 3 : 1;
                    Partita.AggiungiLog($"Pescat{(num > 0 ? "e" : "a")} {num} cart{(num > 0 ? "e" : "a")} sul tavolo");
                    Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo, num, 1);
                }
                else
                {
                    Partita.PartitaCorrente.Stato = Partita.EnumStato.CambioMazziere;
                    List<Giocatore> lista = new List<Giocatore>(Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito));
                    lista.ForEach(q => q.SetPunteggio(Partita.PartitaCorrente.Tavolo));
                    lista.Sort();
                    var vincitore = lista.FirstOrDefault();

                    List<Giocatore> vincitori = new List<Giocatore> { vincitore };
                    foreach(var g in lista.Skip(1))
                    {
                        if (g.Punteggio != vincitore.Punteggio)
                            break;
                        vincitori.Add(g);
                    }

                    foreach (Giocatore v in vincitori)
                    {
                        messaggio = $"Mano vinta da {vincitore.Nome} con {vincitore.Punteggio?.Tipo} di {(vincitore.Punteggio?.Seme != null ? vincitore.Punteggio.Seme.ToString() : "")} {vincitore.Punteggio?.Numero1} {(vincitore.Punteggio?.Numero2 != null ? " e " + vincitore.Punteggio.Numero2.ToString() : "")}";
                        Partita.AggiungiLog(messaggio);
                        v.Credito += Partita.PartitaCorrente.Tavolo.Credito / vincitori.Count;
                    }

                    Partita.PartitaCorrente.Tavolo.Credito = 0;
                    Partita.PartitaCorrente.Stato = Partita.EnumStato.CambioMazziere;
                    Partita.PartitaCorrente.SetNextMazziere();

                    //Partita.PartitaCorrente.Tavolo = new Tavolo();
                    //Partita.PartitaCorrente.Giocatori.ForEach(q => { q.Uscito = false; q.Carte = new List<Carta>(); q.Puntata = 0; });
                }
            }

        }

        public JsonResult Passa(int id)
        {
            Partita.PartitaCorrente.Giocatori[id].Uscito = true;
            Partita.PartitaCorrente.SetNextMano();
            Partita.AggiungiLog($"Il giocatore {Partita.PartitaCorrente.Giocatori[id].Nome} è passato");
            VerificaPuntate();

            return Json(new { partita = Partita.PartitaCorrente });
        }

        public JsonResult ImportoVedi(int id)
        {
            decimal min = Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata);
            decimal diff = min - Partita.PartitaCorrente.Giocatori[id].Puntata;

            return Json(diff);
        }


        public JsonResult GetVincitore()
        {
            List<Giocatore> lista = new List<Giocatore>(Partita.PartitaCorrente.Giocatori);
            lista.Sort();

            return Json(lista.FirstOrDefault()?.Id);
        }

        public JsonResult GetPartita()
        {
            List<Giocatore> lista = new List<Giocatore>(Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito));
            lista.ForEach(q => q.SetPunteggio(Partita.PartitaCorrente.Tavolo));
            lista.Sort();
            var vincitore = lista.FirstOrDefault();

            return Json(Partita.PartitaCorrente);
        }

    }
}
