using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj2
{
    class KProizvod
    {
        int id;
        int kat;
        string ime;
        int cena;

        public KProizvod(int id, int kat, string ime, int cena)
        {
            this.id = id;
            this.kat = kat;
            this.ime = ime;
            this.cena = cena;
        }

        public KProizvod()
        {
            this.id = 0;
            this.kat = 0;
            this.ime = "";
            this.cena = 0;
        }

        public int Id { get => id; set => id = value; }
        public string Ime { get => ime; set => ime = value; }
        public int Cena { get => cena; set => cena = value; }
        internal int Kat { get => kat; set => kat = value; }

        public override string ToString()
        {
            return ime + ", Cena: " + cena + " din";
        }
    }
}
