using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace proj2
{
    class KRacun
    {
        int id;
        KProizvod proiz;
        DateTime datIzd;
        int ukCena;
        int brArt;
        public int brAr;

        public KRacun(int id, KProizvod proiz, DateTime datIzd, int brArt, int ukCena)
        {
            this.id = id;
            this.proiz = proiz;
            this.datIzd = datIzd;
            this.brArt = brArt;
            this.ukCena = ukCena;
            brAr = brArt;
        }

        public KRacun()
        {
            this.id = 0;
            this.proiz = new KProizvod();
            this.datIzd = DateTime.Now;
            this.brArt = 0;
            this.ukCena = 0;
        }

        public int Id { get => id; set => id = value; }
        public KProizvod Proiz { get => proiz; set => proiz = value; }
        public DateTime DatIzd { get => datIzd; set => datIzd = value; }
        public int BrArt { get => brArt; set => brArt = value; }
        public int UkCena { get => ukCena; set => ukCena = value; }

        public override string ToString()
        {
            return proiz.Ime + ", Broj artikala: " + brArt + ", Ukupna Cena: " + proiz.Cena*brAr;
        }
    }
}
