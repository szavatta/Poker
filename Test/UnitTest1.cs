using NUnit.Framework;
using Poker;
using System;
using System.Collections.Generic;

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
            Assert.AreEqual(52, mazzo.ListaCarte?.Count);
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

            var v = g1.IsVincitore(g2, t);

            Assert.AreEqual(52, mazzo.ListaCarte?.Count,v.ToString(),null);
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

            Assert.IsTrue(g2.IsVincitore(g1, tavolo));
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

            Poker.Partita.PartitaCorrente = new Partita
            {
                Tavolo = tavolo
            };

            List<Giocatore> listag = new List<Giocatore> { g1, g2, g3 };
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

    }
}