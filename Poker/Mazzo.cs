using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker
{
    [Serializable]
    public class Mazzo
    {
        public List<Carta> Carte { get; set; }

        public void CreaMazzo(bool mischia = false)
        {
            Carte = new List<Carta>();
            for (int i = 1; i <= 4; i++)
            {
                for (int ii = 1; ii <= 13; ii++)
                {
                    Carte.Add(new Carta((Carta.NumeroCarta)ii, (Carta.SemeCarta)i));
                }
            }
            if (mischia)
            {
                var rnd = new Random();
                Carte = Carte.OrderBy(item => rnd.Next()).ToList();
            }
        }
    }

}
