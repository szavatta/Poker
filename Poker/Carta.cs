using System;
using System.IO;

namespace Poker
{
    public class Carta
    {
        public Carta(NumeroCarta numero, SemeCarta seme)
        {
            Numero = numero;
            Seme = seme;
        }

        private byte[] _Immagine { get; set; }
        public NumeroCarta Numero { get; set; }
        public SemeCarta Seme { get; set; }
        public bool Usata { get; set; }
        public byte[] Immagine { get; set; }
        public string ImmagineBase64 { get; set; }

        public byte[] GetImmagine()
        {
            byte[] ret = null;
            try
            {
                string path = $"carte\\{(int)Seme}-{(int)Numero}.png";
                ret = File.ReadAllBytes(path);
            }
            catch { }
            return ret;
        }
        public string GetBase64Immagine()
        {
            string ret = "";
            try
            {
                ret = Convert.ToBase64String(GetImmagine());
            }
            catch { }
            return ret;
        }


        public enum SemeCarta
        {
            Cuori = 1,
            Quadri = 2,
            Fiori = 3,
            Picche = 4
        }

        public enum NumeroCarta
        {
            Asso = 1,
            Due = 2,
            Tre = 3,
            Quattro = 4,
            Cinque = 5,
            Sei = 6,
            Sette = 7,
            Otto = 8,
            Nove = 9,
            Dieci = 10,
            Jack = 11,
            Donna = 12,
            Re = 13,
            Asso14 = 14
        }


    }

}
