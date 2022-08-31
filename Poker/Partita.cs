using System;
using System.Collections.Generic;

namespace Poker
{
    public class Partita
    {
        public static Partita PartitaCorrente = null;

        public Mazzo Mazzo { get; set; }
        public Tavolo Tavolo { get; set; }
        public List<Giocatore> Giocatori { get; set; }
        public int Mano { get; set; }
        public EnumStato Stato { get; set; }

        public enum EnumStato
        {
            DaIniziare,
            Iniziata,
            CambioMano,
            Terminata
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



    }

}
