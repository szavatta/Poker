using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker
{
    public class Mazzo
    {
        public List<Carta> ListaCarte { get; set; }

        public void CreaMazzo(bool mischia = false)
        {
            ListaCarte = new List<Carta>();
            for (int i = 1; i <= 4; i++)
            {
                for (int ii = 1; ii <= 13; ii++)
                {
                    ListaCarte.Add(new Carta((Carta.NumeroCarta)ii, (Carta.SemeCarta)i));
                }
            }
            if (mischia)
            {
                var rnd = new Random();
                ListaCarte = ListaCarte.OrderBy(item => rnd.Next()).ToList();
            }
        }
    }

}
