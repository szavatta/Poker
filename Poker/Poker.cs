using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
            string path = $"carte\\{(int)Seme}-{(int)Numero}.png";
            return File.ReadAllBytes(path);
        }
        public string GetBase64Immagine()
        {
            return Convert.ToBase64String(GetImmagine());
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
                carta.ImmagineBase64 = carta.GetBase64Immagine();
                Carte.Add(carta);
                carta.Usata = true;
            }
        }

    }

    public class Giocatore : Entita
    {
        public Punteggio Punteggio { get; set; }
        public Giocatore SetPunteggio(Tavolo tavolo)
        {
            Punteggio p = new Punteggio();
            List<Carta> carte = new List<Carta>(Carte);
            carte.AddRange(tavolo.Carte);
            p.GetPunteggio(carte);
            Punteggio = p;
            return this;
        }

        public bool? IsVincitore(Giocatore g2, Tavolo tavolo)
        {
            this.SetPunteggio(tavolo);
            g2.SetPunteggio(tavolo);
            bool? ret = null;
            if (this.Punteggio.Tipo > g2.Punteggio.Tipo)
                ret = true;
            else if (this.Punteggio.Tipo < g2.Punteggio.Tipo)
                ret = false;
            else
            {
                if (this.Punteggio.Numero1 > g2.Punteggio.Numero1)
                    ret = true;
                else if (this.Punteggio.Numero1 < g2.Punteggio.Numero1)
                    ret = false;
                else
                {
                    if (this.Punteggio.Numero2 > g2.Punteggio.Numero2)
                        ret = true;
                    else if (this.Punteggio.Numero2 < g2.Punteggio.Numero2)
                        ret = false;
                    else
                    {
                        if (this.Punteggio.Seme > g2.Punteggio.Seme)
                            ret = true;
                        else if (this.Punteggio.Seme < g2.Punteggio.Seme)
                            ret = false;
                        else
                        {
                            //guarda le altre carte
                        }
                    }    
                }
            }

            return ret;
        }
    }

    public class Tavolo : Entita
    {

    }

    public class Partita
    {
        public static Partita PartitaCorrente = null;

        public Mazzo Mazzo { get; set; }
        public Tavolo Tavolo { get; set; }
        public List<Giocatore> Giocatori { get; set; }
        public int Mano { get; set; }
    }

    public class Punteggio
    {
        private EnumTipo? tipo { get; set; }
        public EnumTipo? Tipo {
            get 
            { 
                return tipo; 
            }
            set
            {
                tipo = value;
                TipoString = tipo.ToString();
            }
        }
        public string TipoString { get; private set; }
        public Carta.NumeroCarta? Numero1 { get; set; }
        public Carta.NumeroCarta? Numero2 { get; set; }
        public Carta.SemeCarta? Seme { get; set; }

        public enum EnumTipo
        {
            Coppia = 1,
            DoppiaCoppia = 2,
            Tris = 3,
            ScalaSemplice = 4,
            Colore = 5,
            Full = 6,
            Poker = 7,
            ScalaColore = 8
        }


        public void GetPunteggio(List<Carta> carte)
        {
            if (Tipo == null) // ScalaColore
            {
                carte.Where(q => q.Numero == Carta.NumeroCarta.Asso).ToList().ForEach(q => q.Numero = Carta.NumeroCarta.Asso14);
                int diff = 0;
                Carta.NumeroCarta? num = null;

                var aa = carte.GroupBy(q => q.Seme).ToList().Where(q => q.Count() >= 5).FirstOrDefault();
                if (aa != null)
                {
                    List<Carta> carte2 = new List<Carta>(carte);
                    carte2 = carte2.Where(q => q.Seme == aa.Key).ToList();
                    for (int i = 0; i < 2; i++)
                    {
                        //Con i==0 testa Asso dopo il Re
                        if (diff != 4 && i == 1)
                            carte2.Where(q => q.Numero == Carta.NumeroCarta.Asso14).ToList().ForEach(q => q.Numero = Carta.NumeroCarta.Asso);

                        var a = carte2.GroupBy(q => q.Numero).ToList().OrderByDescending(q => q.Key).ToList();
                        if (a.Count() >= 5)
                        {
                            for (int ii = 0; ii < a.Count() - 4; ii++)
                            {
                                diff = (int)a[ii].Key - (int)a[ii + 4].Key;
                                num = a[ii].Key;
                                if (diff == 4)
                                    break;
                            }
                        }
                        if (diff == 4)
                        {
                            Tipo = EnumTipo.ScalaColore;
                            Numero1 = num;
                        }
                    }
                }
            }
            if (Tipo == null) // Poker
            {
                carte.Where(q => q.Numero == Carta.NumeroCarta.Asso).ToList().ForEach(q => q.Numero = Carta.NumeroCarta.Asso14);
                var a = carte.GroupBy(q => q.Numero).Where(q => q.Count() == 4).OrderByDescending(q => q.Key)?.FirstOrDefault();
                if (a != null)
                {
                    Tipo = EnumTipo.Poker;
                    Numero1 = a.Key;
                }
            }
            if (Tipo == null) // Full
            {
                var a = carte.GroupBy(q => q.Numero);
                var a1 = a.Where(q => q.Count() == 3).OrderByDescending(q => q.Key).FirstOrDefault();
                if (a1 != null)
                {
                    var a2 = a.Where(q => q.Key != a1.Key && q.Count() >= 2).OrderByDescending(q => q.Key).FirstOrDefault();
                    if (a2 != null)
                    {
                        Tipo = EnumTipo.Full;
                        Numero1 = a1.Key;
                        Numero2 = a2.Key;
                    }
                }
            }
            if (Tipo == null) // Colore
            {
                var a = carte.GroupBy(q => q.Seme).Where(q => q.Count() >= 5).FirstOrDefault();
                if (a != null)
                {
                    Tipo = EnumTipo.Colore;
                    Seme = a.Key;
                }
            }
            if (Tipo == null) // ScalaSemplice
            {
                carte.Where(q => q.Numero == Carta.NumeroCarta.Asso).ToList().ForEach(q => q.Numero = Carta.NumeroCarta.Asso14);
                int diff = 0;
                Carta.NumeroCarta? num = null;
                for (int i = 0; i < 2; i++)
                {
                    //Con i==1 testa Asso dopo il Re
                    if (diff != 4 && i == 1)
                        carte.Where(q => q.Numero == Carta.NumeroCarta.Asso14).ToList().ForEach(q => q.Numero = Carta.NumeroCarta.Asso);

                    var a = carte.GroupBy(q => q.Numero).ToList().OrderByDescending(q => q.Key).ToList();
                    if (a.Count() >= 5)
                    {
                        for (int ii = 0; ii < a.Count() - 4; ii++)
                        {
                            diff = (int)a[ii].Key - (int)a[ii + 4].Key;
                            num = a[ii].Key;
                            if (diff == 4)
                                break;
                        }
                    }
                    if (diff == 4)
                    {
                        Tipo = EnumTipo.ScalaSemplice;
                        Numero1 = num;
                    }
                }
            }
            carte.Where(q => q.Numero == Carta.NumeroCarta.Asso).ToList().ForEach(q => q.Numero = Carta.NumeroCarta.Asso14);
            if (Tipo == null) // Tris
            {
                var a = carte.GroupBy(q => q.Numero).Where(q => q.Count() == 3).OrderByDescending(q => q.Key)?.FirstOrDefault();
                if (a != null)
                {
                    Tipo = EnumTipo.Tris;
                    Numero1 = a.Key;
                }
            }
            if (Tipo == null) //Doppia Coppia
            {
                var a = carte.GroupBy(q => q.Numero).Where(q => q.Count() == 2).OrderByDescending(q => q.Key);
                if (a.Count() >= 2)
                {
                    Tipo = EnumTipo.DoppiaCoppia;
                    Numero1 = a.First().Key;
                    Numero2 = a.Skip(1).First().Key;
                }
            }
            if (Tipo == null) //Coppia
            {
                var a = carte.GroupBy(q => q.Numero).Where(q => q.Count() == 2).OrderByDescending(q => q.Key)?.FirstOrDefault();
                if (a != null)
                {
                    Tipo = EnumTipo.Coppia;
                    Numero1 = a.Key;
                }
            }
        }


    }

}
