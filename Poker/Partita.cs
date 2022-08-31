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
    }

}
