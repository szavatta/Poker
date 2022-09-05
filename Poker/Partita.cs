using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Poker
{
    [Serializable]
    public class Partita : ICloneable
    {
        public Partita(decimal soldiIniziali = 0, decimal puntata = 0)
        {
            Stato = Partita.EnumStato.DaIniziare;
            Tavolo = new Tavolo();
            Logs = new List<Log>();
            SoldiIniziali = soldiIniziali;
            Puntata = puntata;
        }
            
        public static Partita PartitaCorrente = null;

        public Mazzo Mazzo { get; set; }
        public Tavolo Tavolo { get; set; }
        public List<Giocatore> Giocatori { get; set; }
        public int Mano { get; set; }
        public int Giro { get; set; }
        public int IdMazziere { get; set; }
        public decimal SoldiIniziali { get; set; }
        public decimal Puntata { get; set; }
        public EnumStato Stato { get; set; }
        public List<Log> Logs { get; set; }

        public enum EnumStato
        {
            DaIniziare = 0,
            InSvolgimento = 1,
            CambioMazziere = 2,
            Terminata = 3
        }

        public int GetNextMano(int? attuale = null)
        {
            if (attuale == null)
                attuale = Mano;
            int id = -1;
            bool trovato = false;
            for (int i = 0; i < Giocatori.Count - 1; i++)
            {
                id = (attuale.Value + i + 1) % (Giocatori.Count);
                if (!Giocatori[id].Uscito && !Giocatori[id].Terminato && !Giocatori[id].IsAllIn)
                {
                    trovato = true;
                    break;
                }
            }

            if (!trovato)
                throw new Exception("Non ci sono altri giocatori da passare la mano");

            return id;
        }

        public void SetNextMano(int? attuale = null) => Mano = GetNextMano(attuale);

        public int GetNextMazziere(int? attuale = null)
        {
            if (attuale == null)
                attuale = Partita.PartitaCorrente.IdMazziere;

            int id = -1;
            for (int i = 0; i < Partita.PartitaCorrente.Giocatori.Count - 1; i++)
            {
                id = (attuale.Value + i + 1) % (Partita.PartitaCorrente.Giocatori.Count);
                break;
            }

            return id;
        }

        public void SetNextMazziere(int? attuale = null) => Partita.PartitaCorrente.IdMazziere = GetNextMazziere(attuale);

        public object Clone()
        {
            var partita = (Partita)MemberwiseClone();
            return partita;
        }

        public static void AggiungiLog(string testo)
        {
            Partita.PartitaCorrente.Logs.Insert(0, new Log(testo));
        }

        public static Partita NuovaPartita(string sessionId = null, decimal soldiIniziali = 10000, decimal puntata = 200)
        {
            if (Partita.PartitaCorrente == null)
            {
                Partita.PartitaCorrente = new Partita(soldiIniziali, puntata);
                Partita.AggiungiLog("Iniziata nuova partita");
                Partita.PartitaCorrente.AggiungiGiocatore(sessionId);
            }
            else if (Partita.PartitaCorrente.Giocatori.Count < 4)
            {
                Partita.PartitaCorrente.AggiungiGiocatore(sessionId);
            }

            return Partita.PartitaCorrente;
        }

        public Partita AggiungiGiocatore(string sessionId = null)
        {
            if (Giocatori == null)
                Giocatori = new List<Giocatore>();

            Giocatore g = null;
            if (!string.IsNullOrEmpty(sessionId))
                g = Partita.PartitaCorrente.Giocatori.Where(q => q.SessionId == sessionId).FirstOrDefault();

            if (g == null)
            {
                int i = Partita.PartitaCorrente.Giocatori.Count();
                g = new Giocatore
                {
                    Nome = $"Giocatore{i + 1}",
                    Id = i,
                    SessionId = sessionId,
                    Credito = Partita.PartitaCorrente.SoldiIniziali
                };

                if (Partita.PartitaCorrente.Mazzo != null)
                    ((Giocatore)g.Pesca(Partita.PartitaCorrente.Mazzo, 2)).SetPunteggio(Partita.PartitaCorrente.Tavolo);
                Partita.PartitaCorrente.Giocatori.Add(g);
                Partita.AggiungiLog($"Il giocatore {g.Nome} si è aggiunto al gioco");
            }

            return Partita.PartitaCorrente;
        }

        public List<Giocatore> GetVincitori()
        {
            List<Giocatore> lista = new List<Giocatore>(Giocatori.Where(q => !q.Uscito && !q.Terminato));
            lista.ForEach(q => q.SetPunteggio(Partita.PartitaCorrente.Tavolo));
            lista.Sort();
            var vincitore = lista.FirstOrDefault();

            List<Giocatore> vincitori = new List<Giocatore> { vincitore };
            foreach (var g in lista.Skip(1))
            {
                if (!g.Punteggio.Equals(vincitore.Punteggio))
                    break;
                vincitori.Add(g);
            }

            return vincitori;
        }

        public IEnumerable<Giocatore> GiocatoriAttivi() => Giocatori.Where(q => !q.Uscito);

        public string VerificaPuntate()
        {
            string messaggio = string.Empty;
            //Verifica se hanno puntato tutti uguale oppure se hanno fatto tutti check
            var ingiocon = PartitaCorrente.GiocatoriInGioco().Where(q => !q.IsAllIn).ToList();
            var maxPuntata = ingiocon.Max(q => q.Puntata);
            if ((maxPuntata == ingiocon.Min(q => q.Puntata) && maxPuntata > 0 || (maxPuntata == 0 && ingiocon.Where(q => q.IsCheck).Count() == ingiocon.Count()))
                && 
                //al primo giro da la possibile ad grande buio di poter effettuare la puntata
                (!(Giro == 0 && Giocatori.FirstOrDefault(q => q.Posizione == Giocatore.EnumPosizione.GrandeBuio).Id == Mano && maxPuntata == Puntata)))
            {
                if (Tavolo.Carte.Count < 5 && Giocatori.Where(q => !q.Uscito && !q.Terminato && !q.IsAllIn).Count() > 1)
                {
                    //Giro finito si pesca una carta sul tavolo
                    int num = Tavolo.Carte.Count == 0 ? 3 : 1;
                    Partita.AggiungiLog($"Pescat{(num > 0 ? "e" : "a")} {num} cart{(num > 0 ? "e" : "a")} sul tavolo");
                    Tavolo.Pesca(Mazzo, num, 1);
                    Giro++;
                }
                else
                {
                    //Mano finita si decreta il vincitore
                    Stato = Partita.EnumStato.CambioMazziere;
                    List<Giocatore> vincitori = GetVincitori();

                    var puntataok = ingiocon.FirstOrDefault()?.Puntata;

                    string sep = "Mano vinta da ";
                    foreach (Giocatore v in vincitori)
                    {
                        decimal vincita = 0;
                        if (vincitori.Count == 1 && !v.IsAllIn) //un vincitore non allin
                            vincita = Tavolo.Credito;
                        else if (vincitori.Count > 0 && vincitori.Where(q => q.IsAllIn).Count() == 0) //più vincitori senza allin
                            vincita = Math.Round(Tavolo.Credito / vincitori.Count());
                        else
                        {
                            if (v.IsAllIn)
                                vincita = Math.Round(v.PuntataAllIn * PartitaCorrente.GiocatoriInGioco().Count() / vincitori.Count());
                            else
                                vincita = Math.Round(v.Puntata * PartitaCorrente.GiocatoriInGioco().Count() / vincitori.Count());
                        }

                        Tavolo.Credito -= vincita;
                        v.Credito += vincita;
                        messaggio += sep + v.Nome;
                        sep = " e da ";
                    }
                    messaggio += $" con {vincitori[0].Punteggio?.Tipo}" +
                        $"{(vincitori[0].Punteggio?.Seme != null ? " di " + vincitori[0].Punteggio.Seme.ToString() : "")} " +
                        $"{(vincitori[0].Punteggio?.Numero1 != null ? " di " + (vincitori[0].Punteggio.Numero1 == Carta.NumeroCarta.Asso14 ? Carta.NumeroCarta.Asso.ToString() : vincitori[0].Punteggio.Numero1.ToString()) : "")} " +
                        $"{(vincitori[0].Punteggio?.Numero2 != null ? " e " + (vincitori[0].Punteggio.Numero2 == Carta.NumeroCarta.Asso14 ? Carta.NumeroCarta.Asso.ToString() : vincitori[0].Punteggio.Numero2.ToString()) : "")}";
                    Partita.AggiungiLog(messaggio);

                    if (Tavolo.Credito > 0)
                    {
                        //i soldi rimasti vengono divisi tra i giocatori rimasti in gioco
                        var v1 = PartitaCorrente.GiocatoriInGioco().Select(q => q.Id).ToList();
                        var v2 = vincitori.Select(q => q.Id).ToList();
                        var v3 = v1.Except(v2).ToList();
                        var v4 = PartitaCorrente.GiocatoriInGioco().Where(q => v3.Contains(q.Id)).ToList();
                        decimal vincita = Math.Round(Tavolo.Credito / v4.Count());
                        foreach (var vva in v4)
                        {
                            vva.Credito += vincita;
                            Tavolo.Credito -= vincita;
                        }
                        v4[0].Credito += Tavolo.Credito;
                    }

                    Tavolo.Credito = 0;
                    Giro = 0;
                    Stato = Partita.EnumStato.CambioMazziere;
                    SetNextMazziere();
                    Giocatori.Where(q => q.Credito == 0 && !q.Terminato).ToList().ForEach(q => q.Terminato = true);
                    if (Giocatori.Where(q => !q.Terminato).Count() <= 1)
                        Stato = EnumStato.Terminata;
                }

                //azzera i valori nei giocatori
                Giocatori.ForEach(q => { 
                    q.Puntata = 0;
                    q.PuntataAllIn = 0;
                    q.IsCheck = false; 
                    q.IsAllInAbilitato = false;
                });
            }

            return messaggio;
        }

        public static decimal DiffPuntata(decimal importo, decimal puntata)
        {
            return importo + puntata - Partita.PartitaCorrente.Giocatori.Where(q => !q.Uscito).Max(q => q.Puntata);
        }

        public void VerificaFlagsGiocatori() => Giocatori.ForEach(q => q.VerificaFlags());

        public void PescaCartaTavolo()
        {
            if (Tavolo.Carte.Count < 5)
            {
                int num = Tavolo.Carte?.Count < 3 ? 3 : 1;
                Tavolo.Pesca(Mazzo, num, 1);
                Giocatori.ForEach(q => q.SetPunteggio(Tavolo));
                Partita.AggiungiLog($"Pescata {num} carta sul tavolo");
            }
        }

        public void DistribuisciCarte(bool nuovoMazzo = true)
        {
            Tavolo.Carte = new List<Carta>();
            if (Mazzo == null)
                Mazzo = new Mazzo();
            if (nuovoMazzo)
                Mazzo.CreaMazzo(true);
            //Partita.PartitaCorrente.Tavolo.Pesca(Partita.PartitaCorrente.Mazzo, 3, 1);
            Giocatori.Where(q => !q.Terminato).ToList().ForEach(q => { 
                q.Carte = new List<Carta>(); 
                q.Uscito = false;
                q.Posizione = Giocatore.EnumPosizione.Altro;
                q.IsAllIn = false;
                q.PuntataAllIn = 0;
                q.Pesca(Mazzo, 2); 
            });
            Partita.AggiungiLog("Distribuite 2 per ogni giocatore");

            if (Giocatori.Count > 2)
                SetNextMano(IdMazziere);
            else
                PartitaCorrente.Mano = IdMazziere;

            //Piccolo buio
            Giocatori[PartitaCorrente.Mano].Posizione = Giocatore.EnumPosizione.PiccoloBuio;
            Giocatori[PartitaCorrente.Mano].Punta(Puntata / 2);

            //Grande buio
            Giocatori[PartitaCorrente.Mano].Posizione = Giocatore.EnumPosizione.GrandeBuio;
            Giocatori[PartitaCorrente.Mano].Punta(Puntata);

            Partita.AggiungiLog($"La mano è passata al giocatore {Giocatori[PartitaCorrente.Mano].Nome}");
            Stato = Partita.EnumStato.InSvolgimento;

        }

        public List<Giocatore> GiocatoriInGioco() => Giocatori.Where(q => !q.Uscito && !q.Terminato).ToList();

    }

}

