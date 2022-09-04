using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker
{
    [Serializable]
    public abstract class Entita
    {
        public Entita()
        {
            Carte = new List<Carta>();
        }

        public List<Carta> Carte { get; set; }
        public decimal Credito { get; set; }


        public Entita Pesca(Mazzo mazzo, int numCarte = 1, int brucia = 0)
        {
            if (brucia > 0)
                mazzo.Carte.RemoveRange(0, brucia); //Brucia carte

            var carte = mazzo.Carte.Take(numCarte);
            Carte.AddRange(carte);
            mazzo.Carte.RemoveRange(0, numCarte);

            if (mazzo.Carte.Count == 0)
                mazzo.CreaMazzo(true);

            return this;
        }

    }

}
