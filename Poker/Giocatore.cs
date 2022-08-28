using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poker
{

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

}
