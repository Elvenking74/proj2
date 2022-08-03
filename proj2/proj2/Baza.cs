using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace proj2
{
    class Baza
    {
        OleDbConnection kon;
        public Baza()
        {
            kon = new OleDbConnection();
            kon.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=database\proj2.accdb";

        }

        public OleDbConnection Kon { get => kon; set => kon = value; }

        public void otvoriKonekciju()
        {
            if (kon != null)
                kon.Open();
        }
        public void zatvoriKonekciju()
        {
            if (kon != null)
                kon.Close();
        }
    }
}
