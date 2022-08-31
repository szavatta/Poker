using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System;

namespace Poker
{

    public class Giocatore : Entita, IComparable<Giocatore>
    {
        public Punteggio Punteggio { get; set; }
        public string Nome { get; set; }
        public int? Id { get; set; }
        public decimal Puntata { get; set; }
        public bool Uscito { get; set; }
        public string SessionId { get; set; }
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
                            if (this.Punteggio.Carte.Count < 5)
                            {
                                List<Carta> carte1 = new List<Carta>(this.Carte.OrderByDescending(q => q.Numero));
                                carte1.AddRange(tavolo.Carte);
                                carte1.RemoveAll(q => this.Punteggio.Carte.Contains(q));

                                List<Carta> carte2 = new List<Carta>(g2.Carte.OrderByDescending(q => q.Numero));
                                carte2.AddRange(tavolo.Carte);
                                carte2.RemoveAll(q => g2.Punteggio.Carte.Contains(q));

                                for (int i = 0; i < carte1.Count() - 1; i++)
                                {
                                    if (carte1[i].Numero > carte2[i].Numero)
                                        ret = true;
                                    else if (carte1[i].Numero < carte2[i].Numero)
                                        ret = false;

                                    if (ret.HasValue)
                                        break;
                                }
                            }
                        }
                    }    
                }
            }

            return ret;
        }

        public int CompareTo(Giocatore other)
        {
            int ret = 0;
            if (this.IsVincitore(other, Partita.PartitaCorrente.Tavolo) == true)
                ret = -1;
            else if (this.IsVincitore(other, Partita.PartitaCorrente.Tavolo) == false)
                ret = 1;

            return ret;
        }
    }

}
