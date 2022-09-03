using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Poker
{
    [Serializable]
    public class Partita : ICloneable
    {
        public Partita(decimal soldiIniziali = 0, decimal puntata = 0)
        {
            Stato = Partita.EnumStato.DaIniziare;
            Tavolo = new Tavolo();
            Logs = new List<Log>();
            SoldiIniziali = soldiIniziali;
            Puntata = puntata;
        }
            
        public static Partita PartitaCorrente = null;

        public Mazzo Mazzo { get; set; }
        public Tavolo Tavolo { get; set; }
        public List<Giocatore> Giocatori { get; set; }
        public int Mano { get; set; }
        public int IdMazziere { get; set; }
        public decimal SoldiIniziali { get; set; }
        public decimal Puntata { get; set; }
        public EnumStato Stato { get; set; }
        public List<Log> Logs { get; set; }

        public enum EnumStato
        {
            DaIniziare = 0,
            InSvolgimento = 1,
            CambioMazziere = 2,
            Terminata = 3
        }

        public int GetNextMano(int? attuale = null)
        {
            if (attuale == null)
                attuale = Partita.PartitaCorrente.Mano;
            int id = -1;
            bool trovato = false;
            for (int i = 0; i < Partita.PartitaCorrente.Giocatori.Count - 1; i++)
            {
                id = (attuale.Value + i + 1) % (Partita.PartitaCorrente.Giocatori.Count);
                if (!Partita.PartitaCorrente.Giocatori[id].Uscito)
                {
                    trovato = true;
                    break;
                }
            }

            if (!trovato)
                throw new Exception("Non ci sono altri giocatori da passare la mano");

            return id;
        }

        public void SetNextMano(int? attuale = null) => Partita.PartitaCorrente.Mano = GetNextMano(attuale);

        public int GetNextMazziere(int? attuale = null)
        {
            if (attuale == null)
                attuale = Partita.PartitaCorrente.IdMazziere;

            int id = -1;
            for (int i = 0; i < Partita.PartitaCorrente.Giocatori.Count - 1; i++)
            {
                id = (attuale.Value + i + 1) % (Partita.PartitaCorrente.Giocatori.Count);
                break;
            }

            return id;
        }

        public void SetNextMazziere(int? attuale = null) => Partita.PartitaCorrente.IdMazziere = GetNextMazziere(attuale);

        public object Clone()
        {
            var partita = (Partita)MemberwiseClone();
            return partita;
        }

        public static void AggiungiLog(string testo)
        {
            Partita.PartitaCorrente.Logs.Insert(0, new Log(testo));
        }

        public static Partita NuovaPartita(string sessionId = null, decimal soldiIniziali = 1000, decimal puntata = 100)
        {
            if (Partita.PartitaCorrente == null)
            {
                Partita.PartitaCorrente = new Partita(soldiIniziali, puntata);
                Partita.AggiungiLog("Iniziata nuova partita");
                Partita.PartitaCorrente.AggiungiGiocatore(sessionId);
            }
            else if (Partita.PartitaCorrente.Giocatori.Count < 4)
            {
                Partita.PartitaCorrente.AggiungiGiocatore(sessionId);
            }

            return Partita.PartitaCorrente;
        }

        public Partita AggiungiGiocatore(string sessionId = null)
        {
            if (Giocatori == null)
                Giocatori = new List<Giocatore>();

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
                    SessionId = sessionId,
                    Credito = Partita.PartitaCorrente.SoldiIniziali
                };

                if (Partita.PartitaCorrente.Mazzo != null)
                    ((Giocatore)g.Pesca(Partita.PartitaCorrente.Mazzo, 2)).SetPunteggio(Partita.PartitaCorrente.Tavolo);
                Partita.PartitaCorrente.Giocatori.Add(g);
                Partita.AggiungiLog($"Il giocatore {g.Nome} si è aggiunto al gioco");
            }

            return Partita.PartitaCorrente;
        }

        public List<Giocatore> GetVincitori()
        {
            List<Giocatore> lista = new List<Giocatore>(Giocatori.Where(q => !q.Uscito));
            lista.ForEach(q => q.SetPunteggio(Partita.PartitaCorrente.Tavolo));
            lista.Sort();
            var vincitore = lista.FirstOrDefault();

            List<Giocatore> vincitori = new List<Giocatore> { vincitore };
            foreach (var g in lista.Skip(1))
            {
                if (!g.Punteggio.Equals(vincitore.Punteggio))
                    break;
                vincitori.Add(g);
            }

            return vincitori;
        }

        public IEnumerable<Giocatore> GiocatoriAttivi() => Giocatori.Where(q => !q.Uscito);

    }

}
