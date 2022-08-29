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
            Partita partita = SetNuovaPartita();

            return View(partita);
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
            Partita partita = SetNuovaPartita();

            return Json(partita);
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

        public JsonResult GetPartita()
        {
            return Json(Partita.PartitaCorrente);
        }

        public Partita SetNuovaPartita()
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


            for (int i = 0; i < 4; i++)
            {
                Giocatore g = new Giocatore();
                g.Nome = $"Giocatore{i + 1}";
                g.Pesca(mazzo, 2);
                g.SetPunteggio(t);
                partita.Giocatori.Add(g);
            }

            Partita.PartitaCorrente = partita;

            return partita;
        }
    }
}
