using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System;

namespace Poker
{
    [Serializable]
    public class Giocatore : Entita, IComparable<Giocatore>
    {
        public Punteggio Punteggio { get; set; }
        public string Nome { get; set; }
        public int? Id { get; set; }
        public decimal Puntata { get; set; }
        public bool Uscito { get; set; }
        public bool IsCheck { get; set; }
        public string SessionId { get; set; }
        public EnumPosizione Posizione { get; set; }
        public Giocatore SetPunteggio(Tavolo tavolo = null)
        {
            Punteggio p = new Punteggio();
            List<Carta> carte = new List<Carta>(Carte);
            if (tavolo != null)
                carte.AddRange(tavolo.Carte);
            p.GetPunteggio(carte);
            Punteggio = p;
            return this;
        }

        public bool? IsVincitore(Giocatore g2, Tavolo tavolo)
        {
            //this.SetPunteggio(tavolo);
            //g2.SetPunteggio(tavolo);
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

        public void Vedi()
        {
            decimal min = Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata);
            Punta(min - Puntata);
        }

        public void Punta(decimal? importo = null)
        {
            if (!importo.HasValue)
                importo = Partita.PartitaCorrente.Puntata;

            if (Partita.PartitaCorrente.Stato == Partita.EnumStato.InSvolgimento && Partita.DiffPuntata(importo.Value, Puntata) > 0 && Partita.DiffPuntata(importo.Value, Puntata) < Partita.PartitaCorrente.Puntata)
                throw new Exception("Puntata non sufficiente");

            if (Uscito)
                throw new Exception("Puntata non valida. Il giocatore non è più in gioco");

            decimal min = Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata);
            if (Puntata + importo < min)
                throw new Exception("Puntata errata. Il minimo è " + min);

            if (importo > Credito)
                throw new Exception("Puntata errata. Non hai credito sufficiente");

            Credito -= importo.Value;
            Puntata += importo.Value;
            Partita.PartitaCorrente.Tavolo.Credito += importo.Value;

            Partita.AggiungiLog($"Il giocatore {Nome} ha puntato {importo.Value}");

            Partita.PartitaCorrente.SetNextMano();
            Partita.VerificaPuntate();
        }

        public void Passa()
        {
            if (Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Sum(q => q.Puntata) == 0 && Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito && q.IsCheck).Count() == 0)
                throw new Exception("Non è possibile passare");

            Uscito = true;
            Partita.AggiungiLog($"Il giocatore {Nome} è passato");
            Partita.PartitaCorrente.SetNextMano();
            Partita.VerificaPuntate();
        }

        public void Check()
        {
            if (Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata) != Puntata)
                throw new Exception("Non è possibile effettuare il check");

            IsCheck = true;
            Partita.PartitaCorrente.SetNextMano();
            Partita.AggiungiLog($"Il giocatore {Nome} ha effettuato il check");
            Partita.VerificaPuntate();
        }

        public enum EnumPosizione
        {
            Altro,
            Dealer,
            PiccoloBuio,
            GrandeBuio
        }

    }

}
