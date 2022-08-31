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
                mazzo.ListaCarte.RemoveRange(0, brucia); //Brucia carte

            var carte = mazzo.ListaCarte.Take(numCarte);
            Carte.AddRange(carte);
            mazzo.ListaCarte.RemoveRange(0, numCarte);

            return this;
        }

    }

}
