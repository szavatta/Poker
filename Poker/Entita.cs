using System.Collections.Generic;
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

        public Entita Pesca(Mazzo mazzo, int numCarte = 1)
        {
            for (int i = 0; i < numCarte; i++)
            {
                Carta carta = mazzo.ListaCarte.FirstOrDefault(q => q.Usata == false);
                //carta.ImmagineBase64 = carta.GetBase64Immagine();
                Carte.Add(carta);
                carta.Usata = true;
            }
            return this;
        }

    }

}
