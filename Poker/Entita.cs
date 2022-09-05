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
            if (brucia > 0) //Brucia carte
                mazzo.Carte.RemoveRange(0, brucia);

            Carte.AddRange(mazzo.Carte.Take(numCarte));
            mazzo.Carte.RemoveRange(0, numCarte);

            return this;
        }

    }

}
