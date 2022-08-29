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
            if (Partita.PartitaCorrente == null)
            {
                SetNuovaPartita(1);
                ViewBag.IdGiocatore = Partita.PartitaCorrente.Giocatori.Count() - 1;
            }
            else if (Partita.PartitaCorrente.Giocatori.Count < 4)
            {
                AggiungiGiocatore();
                ViewBag.IdGiocatore = Partita.PartitaCorrente.Giocatori.Count() - 1;
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
            SetNuovaPartita(4);

            return Json(Partita.PartitaCorrente);
        }

        public JsonResult PescaCartaTavolo()
        {
            if (Partita.PartitaCorrente.Tavolo.Carte.Count < 5)
            {
                Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo);
                Partita.PartitaCorrente.Giocatori.ForEach(q => q.SetPunteggio(Partita.PartitaCorrente.Tavolo));
            }

            return Json(Partita.PartitaCorrente);
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

        public Partita SetNuovaPartita(int NumGiocatori = 4)
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

        public Partita AggiungiGiocatore()
        {
            int i = Partita.PartitaCorrente.Giocatori.Count();
            Giocatore g = new Giocatore
            {
                Nome = $"Giocatore{i + 1}",
                Id = i
            };
            ((Giocatore)g.Pesca(Partita.PartitaCorrente.Mazzo, 2)).SetPunteggio(Partita.PartitaCorrente.Tavolo);
            Partita.PartitaCorrente.Giocatori.Add(g);

            return Partita.PartitaCorrente;
        }
    }
}
