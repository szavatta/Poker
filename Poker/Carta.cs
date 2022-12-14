using System;
using System.ComponentModel;
using System.IO;

namespace Poker
{
    [Serializable]
    public class Carta
    {
        public Carta(NumeroCarta numero, SemeCarta seme)
        {
            Numero = numero;
            Seme = seme;
            PathImage = $"carte/{(int)seme}-{(numero == NumeroCarta.Asso14 ? 1 : (int)numero)}.png";
        }

        private byte[] _Immagine { get; set; }
        public NumeroCarta Numero
        {
            get
            {
                return _numero;
            }
            set
            {
                _numero = value;
                NumeroString = _numero == NumeroCarta.Asso14 ? NumeroCarta.Asso.ToString() : _numero.ToString();
            }
        }
        private NumeroCarta _numero { get; set; }
        public string NumeroString { get; set; }
        public SemeCarta Seme
        {
            get
            {
                return _seme;
            }
            set
            {
                _seme = value;
                SemeString = _seme.ToString();
            }
        }
        private SemeCarta _seme { get; set; }
        public string SemeString { get; set; }
        public string PathImage { get; set; }
        public byte[] Immagine { get; set; }
        public string ImmagineBase64 { get; set; }

        public byte[] GetImmagine()
        {
            byte[] ret = null;
            try
            {
                string path = $"wwwroot\\carte\\{(int)Seme}-{(int)Numero}.png";
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
            [Description("Asso")]
            Asso14 = 14
        }

        public override bool Equals(object obj)
        {
            bool ret = false;

            if (obj is Carta)
            {
                ret = ((Carta)obj).Seme == Seme && ((Carta)obj).Numero == Numero;
            }

            return ret;
        }

    }

}
