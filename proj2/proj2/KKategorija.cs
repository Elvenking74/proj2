using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj2
{
    class KKategorija
    {
        int id;
        string ime;

        public KKategorija(int id, string ime)
        {
            this.id = id;
            this.ime = ime;
        }

        public KKategorija()
        {
            this.id = 0;
            this.ime = "";
        }

        public int Id { get => id; set => id = value; }
        public string Ime { get => ime; set => ime = value; }

        public override string ToString()
        {
            return ime;
        }


    }
}
