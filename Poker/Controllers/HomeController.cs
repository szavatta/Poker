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
            if (Partita.PartitaCorrente == null || Partita.PartitaCorrente.Giocatori.Count <= 4)
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionId")))
                    HttpContext.Session.SetString("SessionId", HttpContext.Session.Id);

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
            Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo, 3, 1);
            Partita.PartitaCorrente.Giocatori.ForEach(q => { q.Carte = new List<Carta>(); q.Pesca(Partita.PartitaCorrente.Mazzo, 2); });
            Partita.PartitaCorrente.Logs.Add(new Log("Distribuite 3 carte sul tavolo e 2 per ogni giocatore"));

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult PescaCartaTavolo()
        {
            if (Partita.PartitaCorrente.Tavolo.Carte.Count < 5)
            {
                int num = Partita.PartitaCorrente.Tavolo.Carte?.Count < 3 ? 3 : 1;
                Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo, num, 1);
                Partita.PartitaCorrente.Giocatori.ForEach(q => q.SetPunteggio(Partita.PartitaCorrente.Tavolo));
                Partita.PartitaCorrente.Logs.Add(new Log($"Pescata {num} carta sul tavolo"));
            }

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult AssegnaSoldi(decimal importo)
        {
            Partita.PartitaCorrente.Giocatori.ForEach(q => { q.Credito = importo; q.Puntata = 0; });
            Partita.PartitaCorrente.Logs.Add(new Log($"Assegnata una quota di {importo} a ciascun giocatore"));

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult ModificaNomeGiocatore(int id, string nome)
        {
            string old = Partita.PartitaCorrente.Giocatori[id].Nome;
            Partita.PartitaCorrente.Giocatori[id].Nome = nome;
            Partita.PartitaCorrente.Logs.Add(new Log($"Il giocatore {old} ha cambiato il nome in {nome}"));

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult Puntata(int id, decimal importo)
        {
            var giocatore = Partita.PartitaCorrente.Giocatori[id];
            if (giocatore.Uscito)
                throw new Exception("Puntata non valida. Il giocatore non è più in gioco");

            decimal min = Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata);
            if (giocatore.Puntata + importo < min)
                throw new Exception("Puntata errata. Il minimo è " + min);

            if (importo > Partita.PartitaCorrente.Giocatori[id].Credito)
                throw new Exception("Puntata errata. Non hai credito sufficiente");

            giocatore.Credito -= importo;
            giocatore.Puntata += importo;
            Partita.PartitaCorrente.Tavolo.Credito += importo;

            Partita.PartitaCorrente.Logs.Add(new Log($"Il giocatore {giocatore.Nome} ha puntato {importo}"));

            string messaggio = VerificaPuntate();
            if (!string.IsNullOrEmpty(messaggio))
                Partita.PartitaCorrente.Logs.Add(new Log(messaggio));

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
                    Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo, 1, 1);
                    Partita.PartitaCorrente.Mano = Partita.PartitaCorrente.GetNextMano();
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
            Partita.PartitaCorrente.Logs.Add(new Log($"Il giocatore {Partita.PartitaCorrente.Giocatori[id].Nome} è passato"));

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
            Partita p = Partita.PartitaCorrente.Clone();
            p.Logs = p.Logs.OrderByDescending(q => q.Data).ToList();
            return Json(p);
        }

        public Partita SetNuovaPartita(string sessionId = null)
        {
            if (Partita.PartitaCorrente == null)
            {
                Partita.PartitaCorrente = new Partita();
                Partita.PartitaCorrente.Logs.Add(new Log("Iniziata nuova partita"));
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
                Partita.PartitaCorrente.Logs.Add(new Log($"Il giocatore {g.Nome} si è aggiunto al gioco"));
            }

            return Partita.PartitaCorrente;
        }
    }
}
