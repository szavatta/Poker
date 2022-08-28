using System.Collections.Generic;
using System.Linq;

namespace Poker
{
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
        public List<Carta> Carte { get; set; }

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
                    Carte = carte.Where(q => q.Numero == a.Key).ToList();
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
                        Carte = carte.Where(q => q.Numero == a1.Key).ToList();
                        Carte.AddRange(carte.Where(q => q.Numero == a2.Key).Take(2).ToList());
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
                    Carte = carte.Where(q => q.Seme == a.Key).ToList();
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
                    Carte = carte.Where(q => q.Numero == a.Key).ToList();
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
                    Carte = carte.Where(q => q.Numero == Numero1).ToList();
                    Carte.AddRange(carte.Where(q => q.Numero == Numero2));
                }
            }
            if (Tipo == null) //Coppia
            {
                var a = carte.GroupBy(q => q.Numero).Where(q => q.Count() == 2).OrderByDescending(q => q.Key)?.FirstOrDefault();
                if (a != null)
                {
                    Tipo = EnumTipo.Coppia;
                    Numero1 = a.Key;
                    Carte = carte.Where(q => q.Numero == a.Key).ToList();
                }
            }
        }


    }

}
