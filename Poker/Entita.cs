﻿using System.Collections.Generic;
using System.Linq;

namespace Poker
{
    public class Entita
    {
        public Entita()
        {
            Carte = new List<Carta>();
        }

        public List<Carta> Carte { get; set; }

        public void Pesca(Mazzo mazzo, int numCarte = 1)
        {
            for (int i = 0; i < numCarte; i++)
            {
                Carta carta = mazzo.ListaCarte.Where(q => q.Usata == false).FirstOrDefault();
                //carta.ImmagineBase64 = carta.GetBase64Immagine();
                Carte.Add(carta);
                carta.Usata = true;
            }
        }

    }

}
