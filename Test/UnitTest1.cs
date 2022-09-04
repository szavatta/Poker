using NUnit.Framework;
using Poker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Mischia()
        {
            Mazzo mazzo = new Mazzo();
            mazzo.CreaMazzo(true);
            Assert.AreEqual(52, mazzo.Carte?.Count);
        }

        [Test]
        public void Partita()
        {
            Mazzo mazzo = new Mazzo();
            mazzo.CreaMazzo(true);

            Giocatore g1 = new Giocatore();
            g1.Pesca(mazzo, 2);

            Giocatore g2 = new Giocatore();
            g2.Pesca(mazzo, 2);

            Tavolo t = new Tavolo();
            t.Pesca(mazzo, 5);

            g1.SetPunteggio(t);
            g2.SetPunteggio(t);


            var v = g1.IsVincitore(g2, t);

            Assert.AreEqual(52 - t.Carte.Count - g1.Carte.Count - g2.Carte.Count , mazzo.Carte?.Count,v.ToString(),null);
        }

        [Test]
        public void IsCoppia()
        {
            List<Carta> carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Quadri)
            };

            Punteggio p = new Punteggio();
            p.GetPunteggio(carte);

            Assert.IsTrue(p.Tipo == Punteggio.EnumTipo.Coppia && p.Numero1 == Carta.NumeroCarta.Asso14);
        }

        [Test]
        public void IsTris()
        {
            List<Carta> carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri)
            };

            Punteggio p = new Punteggio();
            p.GetPunteggio(carte);

            Assert.IsTrue(p.Tipo == Punteggio.EnumTipo.Tris && p.Numero1 == Carta.NumeroCarta.Re);
        }


        [Test]
        public void IsDoppiaCoppia()
        {
            List<Carta> carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Quadri)
            };

            Punteggio p = new Punteggio();
            p.GetPunteggio(carte);

            Assert.IsTrue(p.Tipo == Punteggio.EnumTipo.DoppiaCoppia && p.Numero1 == Carta.NumeroCarta.Re && p.Numero2 == Carta.NumeroCarta.Sei);
        }


        [Test]
        public void IsFull()
        {
            List<Carta> carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Cuori)
            };

            Punteggio p = new Punteggio();
            p.GetPunteggio(carte);

            Assert.IsTrue(p.Tipo == Punteggio.EnumTipo.Full && p.Numero1 == Carta.NumeroCarta.Re && p.Numero2 == Carta.NumeroCarta.Cinque);
        }

        [Test]
        public void IsColore()
        {
            List<Carta> carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Cuori)
            };

            Punteggio p = new Punteggio();
            p.GetPunteggio(carte);

            Assert.IsTrue(p.Tipo == Punteggio.EnumTipo.Colore && p.Seme == Carta.SemeCarta.Cuori);
        }

        [Test]
        public void IsPoker()
        {
            List<Carta> carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Cuori)
            };

            Punteggio p = new Punteggio();
            p.GetPunteggio(carte);

            Assert.IsTrue(p.Tipo == Punteggio.EnumTipo.Poker && p.Numero1 == Carta.NumeroCarta.Asso14);
        }
        [Test]

        public void IsScalaSemplice()
        {
            List<Carta> carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Cuori)
            };

            Punteggio p = new Punteggio();
            p.GetPunteggio(carte);

            Assert.IsTrue(p.Tipo == Punteggio.EnumTipo.Scala && p.Numero1 == Carta.NumeroCarta.Cinque);
        }

        [Test]
        public void IsScalaSempliceAsso14()
        {
            List<Carta> carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Cuori)
            };

            Punteggio p = new Punteggio();
            p.GetPunteggio(carte);

            Assert.IsTrue(p.Tipo == Punteggio.EnumTipo.Scala && p.Numero1 == Carta.NumeroCarta.Asso14);
        }

        [Test]
        public void IsScalaColore()
        {
            List<Carta> carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Cuori)
            };

            Punteggio p = new Punteggio();
            p.GetPunteggio(carte);

            Assert.IsTrue(p.Tipo == Punteggio.EnumTipo.ScalaColore && p.Numero1 == Carta.NumeroCarta.Cinque);
        }

        [Test]
        public void IsScalaColoreAsso14()
        {
            List<Carta> carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Cuori)
            };

            Punteggio p = new Punteggio();
            p.GetPunteggio(carte);

            Assert.IsTrue(p.Tipo == Punteggio.EnumTipo.ScalaColore && p.Numero1 == Carta.NumeroCarta.Asso14);
        }


        [Test]
        public void Vincitore1()
        {
            Tavolo tavolo = new Tavolo();
            tavolo.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Picche)
            };

            Giocatore g1 = new Giocatore();
            g1.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri)
            };

            Giocatore g2 = new Giocatore();
            g2.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori)
            };

            g1.SetPunteggio(tavolo);
            g2.SetPunteggio(tavolo);

            Assert.IsTrue(g1.IsVincitore(g2,tavolo));
        }

        [Test]
        public void Vincitore2()
        {
            Tavolo tavolo = new Tavolo();
            tavolo.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Picche)
            };

            Giocatore g1 = new Giocatore();
            g1.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri)
            };

            Giocatore g2 = new Giocatore();
            g2.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Quadri)
            };

            g1.SetPunteggio(tavolo);
            g2.SetPunteggio(tavolo);

            Assert.IsTrue(g2.IsVincitore(g1, tavolo));
        }

        [Test]
        public void Vincitore3()
        {
            Tavolo tavolo = new Tavolo();
            tavolo.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Picche)
            };

            Giocatore g1 = new Giocatore();
            g1.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri)
            };

            Giocatore g2 = new Giocatore();
            g2.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori)
            };
            
            g1.SetPunteggio(tavolo);
            g2.SetPunteggio(tavolo);

            Assert.IsTrue(g2.IsVincitore(g1, tavolo));
        }

        [Test]
        public void PunteggioPari()
        {
            Tavolo tavolo = new Tavolo();
            tavolo.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Cuori)
            };
            Poker.Partita.PartitaCorrente = new Partita
            {
                Tavolo = tavolo
            };

            Giocatore g1 = new Giocatore(); //tris di assi
            g1.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Quadri)
            };
            g1.SetPunteggio(tavolo);

            Giocatore g2 = new Giocatore(); //tris di 3
            g2.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori)
            };
            g2.SetPunteggio(tavolo);

            Giocatore g3 = new Giocatore(); //scala
            g3.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori)
            };
            g3.SetPunteggio(tavolo);

            List<Giocatore> listag = new List<Giocatore> { g1, g2, g3 };
            listag.Sort();

            Assert.False(listag[0].Punteggio.Equals(listag[1].Punteggio));
        }

        [Test]
        public void SortGiocatori()
        {
            Tavolo tavolo = new Tavolo();
            tavolo.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Picche)
            };

            Giocatore g1 = new Giocatore(); //tris di assi
            g1.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri)
            };
            g1.SetPunteggio(tavolo);

            Giocatore g2 = new Giocatore(); //tris di 3
            g2.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori)
            };
            g2.SetPunteggio(tavolo);

            Giocatore g3 = new Giocatore(); //scala
            g3.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori)
            };
            g3.SetPunteggio(tavolo);

            Giocatore g4 = new Giocatore(); //carta alta
            g4.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori)
            };
            g4.SetPunteggio(tavolo);

            Poker.Partita.PartitaCorrente = new Partita
            {
                Tavolo = tavolo
            };

            List<Giocatore> listag = new List<Giocatore> { g1, g2, g3, g4 };
            listag.Sort();

            Assert.AreEqual(g3, listag[0]);
        }

        [Test]
        public void GetNextMano()
        {
            Poker.Partita.PartitaCorrente = new Partita
            {
                Giocatori = new List<Giocatore> { 
                    new Giocatore { Uscito = true }, 
                    new Giocatore { Uscito = false },
                    new Giocatore { Uscito = true },
                    new Giocatore { Uscito = true },
                    new Giocatore { Uscito = true },
                    new Giocatore { Uscito = false },
                    new Giocatore { Uscito = true },
                    new Giocatore { Uscito = true },
                    new Giocatore { Uscito = false },
                },
                Tavolo = new Tavolo(),
                Mano = 1
            };

            Assert.AreEqual(5, Poker.Partita.PartitaCorrente.GetNextMano());

            Poker.Partita.PartitaCorrente.Mano = 5;
            Assert.AreEqual(8, Poker.Partita.PartitaCorrente.GetNextMano());

            Poker.Partita.PartitaCorrente.Mano = 8;
            Assert.AreEqual(1, Poker.Partita.PartitaCorrente.GetNextMano());

        }

        [Test]
        public void PiuVincitori()
        {
            Partita partita = new Partita(1000, 100);
            Poker.Partita.PartitaCorrente = partita;
            partita.Tavolo = new Tavolo();
            partita.Tavolo.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Picche)
            };
            Giocatore g1 = new Giocatore
            {
                Carte = new List<Carta>
                {
                    new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Quadri),
                    new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori),
                }
            };

            Giocatore g2 = new Giocatore
            {
                Carte = new List<Carta>
                {
                    new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori),
                    new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Quadri),
                }
            };

            Giocatore g3 = new Giocatore
            {
                Carte = new List<Carta>
                {
                    new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori),
                    new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Quadri),
                }
            };

            partita.Giocatori = new List<Giocatore> { g1, g2, g3 };
            List<Giocatore> vincitori = partita.GetVincitori();

            Assert.AreEqual(2, vincitori.Count);
        }

        [Test]
        [Order(10)]
        public void SimulazionePartita()
        {
            Poker.Partita.NuovaPartita();
            Poker.Partita.NuovaPartita();
            Poker.Partita.NuovaPartita();
            Poker.Partita.NuovaPartita();
            Partita partita = Poker.Partita.PartitaCorrente;
            partita.Mazzo = new Mazzo();
            partita.Mazzo.Carte = new List<Carta>
            {
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Quattro, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Donna, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Quadri),
                new Carta(Carta.NumeroCarta.Jack, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Sette, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Cinque, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Re, Carta.SemeCarta.Picche),
                new Carta(Carta.NumeroCarta.Otto, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Asso, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Sei, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Nove, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Due, Carta.SemeCarta.Cuori),
                new Carta(Carta.NumeroCarta.Tre, Carta.SemeCarta.Fiori),
                new Carta(Carta.NumeroCarta.Dieci, Carta.SemeCarta.Fiori),
            };

            partita.DistribuisciCarte();
            try
            {
                partita.Giocatori[partita.Mano].Punta(300);
                Assert.True(false);
            }
            catch { }
            partita.Giocatori[partita.Mano].Punta(500); //g4
            partita.Giocatori[partita.Mano].Vedi(); //g1
            partita.Giocatori[partita.Mano].Vedi(); //g2
            partita.Giocatori[partita.Mano].Vedi(); //g3
            Assert.AreEqual(3, partita.Tavolo.Carte.Count);
            Assert.AreEqual(2000, partita.Tavolo.Credito);

            partita.Giocatori[partita.Mano].Check(); //g4
            try
            {
                partita.Giocatori[partita.Mano].Punta(100);
                Assert.True(false);
            }
            catch { }
            partita.Giocatori[partita.Mano].Punta(200); //g1 - 200
            partita.Giocatori[partita.Mano].Punta(400); //g2 - 400
            partita.Giocatori[partita.Mano].Vedi(); //g3 - 400
            Assert.AreEqual(3, partita.Mano);
            partita.Giocatori[partita.Mano].Passa(); //g4 -----
            partita.Giocatori[partita.Mano].Punta(400); //g1 - 600
            partita.Giocatori[partita.Mano].Punta(200); //g2 - 600
            partita.Giocatori[partita.Mano].Vedi(); //g3 - 600
            Assert.AreEqual(4, partita.Tavolo.Carte.Count);
            Assert.AreEqual(3800, partita.Tavolo.Credito);
            Assert.AreEqual(0, partita.Mano);
            Assert.AreEqual(partita.SoldiIniziali * partita.Giocatori.Count(), partita.Giocatori.Sum(q => q.Credito) + partita.Tavolo.Credito);

            partita.Giocatori[partita.Mano].Punta(200); //g1 - 200
            partita.Giocatori[partita.Mano].Passa(); //g2 ----
            partita.Giocatori[partita.Mano].Punta(800); //g3 - 800
            partita.Giocatori[partita.Mano].Punta(600); //g1 - 800
            Assert.AreEqual(5, partita.Tavolo.Carte.Count);
            Assert.AreEqual(5400, partita.Tavolo.Credito);
            Assert.AreEqual(2, partita.Mano);
            Assert.AreEqual(partita.SoldiIniziali * partita.Giocatori.Count(), partita.Giocatori.Sum(q => q.Credito) + partita.Tavolo.Credito);

            partita.Giocatori[partita.Mano].Check(); //g3
            partita.Giocatori[partita.Mano].Punta(200); //g1 - 200
            var vincitore = partita.GetVincitori().First();
            Assert.IsTrue(vincitore.Nome == "Giocatore1");
            Assert.IsTrue(vincitore.Punteggio.Tipo == Punteggio.EnumTipo.Coppia && vincitore.Punteggio.Numero1 == Carta.NumeroCarta.Otto);
            partita.Giocatori[partita.Mano].Vedi(); //g3 - 200
            Assert.IsTrue(partita.Tavolo.Credito == 0);
            Assert.IsTrue(partita.Giocatori[0].Credito == 13700);
            Assert.AreEqual(partita.SoldiIniziali * partita.Giocatori.Count(), partita.Giocatori.Sum(q => q.Credito) + partita.Tavolo.Credito);

            partita.DistribuisciCarte();
            Assert.IsTrue(partita.Mazzo.Carte.Count == 28);
            Assert.IsTrue(partita.IdMazziere == 1);
            Assert.IsTrue(partita.Mano == 0);
            Assert.IsTrue(partita.Giocatori[3].Puntata == partita.Puntata);
            partita.Giocatori[partita.Mano].Vedi(); //g1 - 
            partita.Giocatori[partita.Mano].Vedi(); //g2 - 
            partita.Giocatori[partita.Mano].Vedi(); //g3 - 
            partita.Giocatori[partita.Mano].Check(); //g4 - 
            Assert.IsTrue(partita.Giocatori.Sum(q => q.Puntata) == 0);
            Assert.IsTrue(partita.Tavolo.Carte.Count == 3);
            partita.Giocatori[partita.Mano].Punta(400); //g1 - 5000
            partita.Giocatori[partita.Mano].Vedi(); //g2 - 5000
            partita.Giocatori[partita.Mano].Vedi(); //g3 - 5000
            partita.Giocatori[partita.Mano].Vedi(); //g4 - 5000
            Assert.IsTrue(partita.Giocatori.Sum(q => q.Puntata) == 0);
            Assert.IsTrue(partita.Tavolo.Carte.Count == 4);
            partita.Giocatori[partita.Mano].Punta(800); //g1 - 5000
            partita.Giocatori[partita.Mano].Vedi(); //g2 - 5000
            partita.Giocatori[partita.Mano].Vedi(); //g3 - 5000
            partita.Giocatori[partita.Mano].Vedi(); //g4 - 5000
            Assert.IsTrue(partita.Giocatori.Sum(q => q.Puntata) == 0);
            Assert.IsTrue(partita.Tavolo.Carte.Count == 5);
            partita.Giocatori[partita.Mano].Punta(7000); //g1 - 7000
            try
            {
                partita.Giocatori[partita.Mano].AllIn(); //g3 - 6500
                Assert.True(false);
            }
            catch { }
            partita.Giocatori[partita.Mano].Vedi(); //g2 - 7000
            try
            {
                partita.Giocatori[partita.Mano].Vedi(); //g3 - 7000
                Assert.True(false);
            }
            catch { }
            partita.Giocatori[partita.Mano].AllIn(); //g3 - 7000
            Assert.IsTrue(partita.Giocatori[2].Credito == 0);
            partita.Giocatori[partita.Mano].Vedi(); //g4 - 7000
            Assert.IsTrue(partita.Stato == Poker.Partita.EnumStato.CambioMazziere);
            Assert.AreEqual(partita.SoldiIniziali * partita.Giocatori.Count(), partita.Giocatori.Sum(q => q.Credito) + partita.Tavolo.Credito);

            partita.DistribuisciCarte();
            partita.Giocatori[partita.Mano].Vedi(); 
            partita.Giocatori[partita.Mano].Vedi(); 
            partita.Giocatori[partita.Mano].Vedi(); 
            Assert.IsTrue(partita.Giro == 0);
            Assert.IsTrue(partita.Giocatori[partita.Mano].Posizione == Giocatore.EnumPosizione.GrandeBuio);
            partita.Giocatori[partita.Mano].Check();
            Assert.IsTrue(partita.Giro == 1);
            partita.Giocatori[partita.Mano].Punta(1000); //mano=1
            partita.Giocatori[partita.Mano].Punta(1000); //mano=2
            partita.Giocatori[partita.Mano].Punta(1000); //mano=3
            partita.Giocatori[partita.Mano].Punta(3000); //mano=0
            partita.Giocatori[partita.Mano].AllIn(); //mano=1
            partita.Giocatori[partita.Mano].Vedi(); //mano=2
            partita.Giocatori[partita.Mano].Vedi(); //mano=3
            partita.Giocatori[partita.Mano].Punta(200); //mano=0
            partita.Giocatori[partita.Mano].Vedi(); //mano=2
            partita.Giocatori[partita.Mano].Vedi(); //mano=3
            partita.Giocatori[partita.Mano].Punta(200); //mano=0
            partita.Giocatori[partita.Mano].Vedi(); //mano=2
            partita.Giocatori[partita.Mano].Passa(); //mano=2
            var v = partita.GetVincitori();
            Assert.IsTrue(v.Count == 1 && v[0].Punteggio.Tipo == Punteggio.EnumTipo.Coppia && v[0].Punteggio.Numero1 == Carta.NumeroCarta.Sette);
            Assert.IsTrue(partita.Giocatori[1].Terminato);

            partita.DistribuisciCarte();

        }

    }
}