using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionId")))
                HttpContext.Session.SetString("SessionId", HttpContext.Session.Id);

            if (Partita.PartitaCorrente == null || Partita.PartitaCorrente.Giocatori.Count < 4)
            {
                SetNuovaPartita(HttpContext.Session.GetString("SessionId"));
                ViewBag.IdGiocatore = Partita.PartitaCorrente.Giocatori.Where(q => q.SessionId == HttpContext.Session.GetString("SessionId")).FirstOrDefault()?.Id;
            }
            else
                ViewBag.IdGiocatore = -1;

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
            SetNuovaPartita();

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult DistribuisciCarte()
        {
            Partita.PartitaCorrente.Stato = Partita.EnumStato.Iniziata;
            Partita.PartitaCorrente.Tavolo.Carte = new List<Carta>();
            Partita.PartitaCorrente.Mazzo = new Mazzo();
            Partita.PartitaCorrente.Mazzo.CreaMazzo(true);
            Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo, 3);
            Partita.PartitaCorrente.Giocatori.ForEach(q => { q.Carte = new List<Carta>(); q.Pesca(Partita.PartitaCorrente.Mazzo, 2); });

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult PescaCartaTavolo()
        {
            if (Partita.PartitaCorrente.Tavolo.Carte.Count < 5)
            {
                int num = Partita.PartitaCorrente.Tavolo.Carte?.Count < 3 ? 3 : 1;
                Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo, num);
                Partita.PartitaCorrente.Giocatori.ForEach(q => q.SetPunteggio(Partita.PartitaCorrente.Tavolo));
            }

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult AssegnaSoldi(decimal importo)
        {
            Partita.PartitaCorrente.Giocatori.ForEach(q => { q.Credito = importo; q.Puntata = 0; });

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult ModificaNomeGiocatore(int id, string nome)
        {
            Partita.PartitaCorrente.Giocatori[id].Nome = nome;

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult Puntata(int id, decimal importo)
        {
            if (Partita.PartitaCorrente.Giocatori[id].Uscito)
                throw new Exception("Puntata non valida. Il giocatore non è più in gioco");

            decimal min = Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata);
            if (Partita.PartitaCorrente.Giocatori[id].Puntata + importo < min)
                throw new Exception("Puntata errata. Il minimo è " + min);

            if (importo > Partita.PartitaCorrente.Giocatori[id].Credito)
                throw new Exception("Puntata errata. Non hai credito sufficiente");

            Partita.PartitaCorrente.Giocatori[id].Credito -= importo;
            Partita.PartitaCorrente.Giocatori[id].Puntata += importo;
            Partita.PartitaCorrente.Tavolo.Credito += importo;

            string messaggio = VerificaPuntate();

            return Json(new { partita = Partita.PartitaCorrente, messaggio = messaggio });
        }

        private string VerificaPuntate()
        {
            string messaggio = string.Empty;
            //Verifica se hanno puntato tutti uguale
            if (Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata) == Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Min(q => q.Puntata))
            {
                Partita.PartitaCorrente.Giocatori.ForEach(q => q.Puntata = 0);
                if (Partita.PartitaCorrente.Tavolo.Carte.Count < 5 && Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Count() > 1)
                {
                    Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo);
                }
                else
                {
                    Partita.PartitaCorrente.Stato = Partita.EnumStato.CambioMano;
                    List<Giocatore> lista = new List<Giocatore>(Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito));
                    lista.ForEach(q => q.SetPunteggio(Partita.PartitaCorrente.Tavolo));
                    lista.Sort();
                    var vincitore = lista.FirstOrDefault();

                    messaggio = $"Mano vinta da {vincitore.Nome} con {vincitore.Punteggio?.Tipo} di {(vincitore.Punteggio?.Seme != null ? vincitore.Punteggio.Seme.ToString() : "")} {vincitore.Punteggio?.Numero1} {(vincitore.Punteggio?.Numero2 != null ? " e " + vincitore.Punteggio.Numero2.ToString() : "")}";

                    vincitore.Credito += Partita.PartitaCorrente.Tavolo.Credito;
                    Partita.PartitaCorrente.Tavolo.Credito = 0;

                    Partita.PartitaCorrente.Tavolo = new Tavolo();
                    Partita.PartitaCorrente.Giocatori.ForEach(q => { q.Uscito = false; q.Carte = new List<Carta>(); q.Puntata = 0; });
                }
            }

            return messaggio;
        }

        public JsonResult Passa(int id)
        {
            Partita.PartitaCorrente.Giocatori[id].Uscito = true;
            string messaggio = VerificaPuntate();

            return Json(new { partita = Partita.PartitaCorrente, messaggio = messaggio });
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
            return Json(Partita.PartitaCorrente);
        }

        public Partita SetNuovaPartita(string sessionId = null)
        {
            if (Partita.PartitaCorrente == null)
            {
                Partita.PartitaCorrente = new Partita
                { 
                    Stato = Partita.EnumStato.DaIniziare
                };
                Partita.PartitaCorrente.Tavolo = new Tavolo();
                AggiungiGiocatore(sessionId);
            }
            else if (Partita.PartitaCorrente.Giocatori.Count < 4)
            {
                AggiungiGiocatore(sessionId);
            }
            else
                ViewBag.IdGiocatore = -1;

            return Partita.PartitaCorrente;
        }

        public Partita SetNuovaPartita2(int NumGiocatori = 4)
        {
            Mazzo mazzo = new Mazzo();
            mazzo.CreaMazzo(true);

            Tavolo t = new Tavolo();
            t.Pesca(mazzo, 3);

            Partita partita = new Partita
            {
                Mazzo = mazzo,
                Tavolo = t,
                Giocatori = new List<Giocatore>(),
                Mano = 0
            };


            for (int i = 0; i < NumGiocatori; i++)
            {
                Giocatore g = new Giocatore
                {
                    Nome = $"Giocatore{i + 1}",
                    Id = i
                };
                ((Giocatore)g.Pesca(mazzo, 2)).SetPunteggio(t);
                partita.Giocatori.Add(g);
            }

            Partita.PartitaCorrente = partita;

            return partita;
        }

        public Partita AggiungiGiocatore(string sessionId = null)
        {
            if (Partita.PartitaCorrente.Giocatori == null)
                Partita.PartitaCorrente.Giocatori = new List<Giocatore>();

            Giocatore g = null;
            if (!string.IsNullOrEmpty(sessionId))
                g = Partita.PartitaCorrente.Giocatori.Where(q => q.SessionId == sessionId).FirstOrDefault();

            if (g == null)
            {
                int i = Partita.PartitaCorrente.Giocatori.Count();
                g = new Giocatore
                {
                    Nome = $"Giocatore{i + 1}",
                    Id = i,
                    SessionId = sessionId
                };

                if (Partita.PartitaCorrente.Mazzo != null)
                    ((Giocatore)g.Pesca(Partita.PartitaCorrente.Mazzo, 2)).SetPunteggio(Partita.PartitaCorrente.Tavolo);
                Partita.PartitaCorrente.Giocatori.Add(g);
            }

            return Partita.PartitaCorrente;
        }
    }
}
