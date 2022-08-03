using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proj2
{
    public partial class FStati : Form
    {
        Baza baza;
        List<KRacun> racuni;
        List<KRacun> racSta;
        List<KProizvod> proizvodi;
        DateTime dt1, dt2;
        int brPArt;
        int brArtS;
        public FStati()
        {
            InitializeComponent();
            baza = new Baza();
            racuni = new List<KRacun>();
            racSta = new List<KRacun>();
            proizvodi = new List<KProizvod>();
        }

        public void updProiz()
        {
            try
            {
                proizvodi.Clear();
                baza.otvoriKonekciju();


                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Kon;
                cmd.CommandText = @"SELECT * 
                                    FROM PROIZVOD
                                    ORDER BY ImeProiz
                                    ";
                OleDbDataReader reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    KProizvod p = new KProizvod();
                    p.Id = int.Parse(reader["IdProiz"].ToString());
                    p.Kat = int.Parse(reader["IdKat"].ToString());
                    p.Ime = reader["ImeProiz"].ToString();
                    p.Cena = int.Parse(reader["cena"].ToString());

                    proizvodi.Add(p);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                baza.zatvoriKonekciju();
            }
        }

        public void updRacPro()
        {
            racSta.Clear();
            if (lb_Racuni.SelectedIndex != -1)
            {
                try
                {
                    baza.otvoriKonekciju();

                    int idRac = racuni[lb_Racuni.SelectedIndex].Id;
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = baza.Kon;

                    cmd.CommandText = @"
                                    SELECT * 
                                    FROM RACUN 
                                    WHERE IdRacuna = " + idRac + " ";

                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        KRacun r = new KRacun();
                        r.Id = int.Parse(reader["IdRacuna"].ToString());
                        r.DatIzd = DateTime.Parse(reader["DatVreRac"].ToString());
                        r.UkCena = int.Parse(reader["UkupnaCena"].ToString());
                        r.BrArt = int.Parse(reader["brArt"].ToString());
                        foreach (KProizvod p in proizvodi)
                        {
                            if (p.Id == int.Parse(reader["IdProiz"].ToString()))
                                r.Proiz = p;
                        }

                        racSta.Add(r);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    baza.zatvoriKonekciju();
                }

                lb_stavkeRac.DataSource = null;
                lb_stavkeRac.DataSource = racSta;
            }

        }

        public void updRacP()
        {
            racuni.Clear();
            try
            {
                baza.otvoriKonekciju();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Kon;
                cmd.CommandText = @"
                                    SELECT IdRacuna, MIN(DatVreRac) AS Dat, SUM(UkupnaCena) AS ukCena, SUM(brArt) AS brArt
                                    FROM RACUN
                                    WHERE DatVreRac >= #" + dateTimePicker1.Value + @"# AND DatVreRac <= #" + dateTimePicker2.Value + @"# 
                                    GROUP BY IdRacuna
                                    ";
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    KRacun r = new KRacun();
                    r.Id = int.Parse(reader["IdRacuna"].ToString());
                    r.DatIzd = DateTime.Parse(reader["Dat"].ToString());
                    r.UkCena = int.Parse(reader["ukCena"].ToString());
                    r.BrArt = int.Parse(reader["brArt"].ToString());
                    r.Proiz = new KProizvod();
                    r.Proiz.Ime = r.Id.ToString();
                    r.Proiz.Cena = int.Parse(reader["ukCena"].ToString());
                    r.brAr = 1;

                    
                    racuni.Add(r);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                baza.zatvoriKonekciju();
            }

            lb_Racuni.DataSource = null;
            lb_Racuni.DataSource = racuni;
        }

        public void brProdArt()
        {
            try
            {
                brPArt = 0;
                baza.otvoriKonekciju();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Kon;
                cmd.CommandText = @"
                                SELECT SUM(brArt) AS suma 
                                FROM RACUN
                                WHERE IdProiz = " + racSta[lb_stavkeRac.SelectedIndex].Proiz.Id + @" AND
                                DatVreRac >= #"+ dt1.ToString() + @"# AND 
                                DatVreRac <= #" + dt2.ToString() + @"#
                                ";
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    brPArt += int.Parse(reader["suma"].ToString());
                }
            }
            catch
            {
                
            }
            finally
            {
                baza.zatvoriKonekciju();
            }
        }

        public void crtaj(object sender, PaintEventArgs e)
        {
            if (brPArt != 0 && brArtS != 0)
            {
                int ugao = 0;
                e.Graphics.FillPie(Brushes.Azure, new Rectangle(500, 150, 200, 200), ugao, (brPArt * 360) / brArtS);
                ugao += (brPArt * 360) / brArtS;
                e.Graphics.FillPie(Brushes.Gray, new Rectangle(500, 150, 200, 200), ugao, ((brArtS - brPArt) * 360) / brArtS);
            }
            else
            {
                e.Graphics.FillPie(Brushes.Gray, new Rectangle(500, 150, 200, 200), 0, 360);
            }
        }

        public void brArt()
        {
            try
            {
                brArtS = 0;
                baza.otvoriKonekciju();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Kon;

                cmd.CommandText = @"
                                    SELECT SUM(BrArt) AS suma
                                    FROM RACUN
                                    WHERE DatVreRac >= #" + dt1.ToString() + @"# AND 
                                    DatVreRac <= #" + dt2.ToString() + @"#
                                    ";
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    brArtS += int.Parse(reader["suma"].ToString());
                }
            }
            catch
            {
                
            }
            finally
            {
                baza.zatvoriKonekciju();
            }
        }

        public void odbrMesec()
        {
            dt1 = new DateTime(dateTimePicker3.Value.Year, dateTimePicker3.Value.Month, 1, 0, 0, 0);
            dt2 = new DateTime(dateTimePicker3.Value.Year, dateTimePicker3.Value.Month, 1, 0, 0, 0);
            dt2 = dt2.AddMonths(1);
            dt2 = dt2.AddDays(-1);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.MinDate = dateTimePicker1.Value;

            updRacP();

            updRacPro();
        }

        private void FStati_Load(object sender, EventArgs e)
        {
            dateTimePicker3.CustomFormat = ("MM/yyyy");

            odbrMesec();


            updProiz();


            updRacP();

            updRacPro();


        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            updRacP();

            updRacPro();
        }

        private void lb_Racuni_SelectedIndexChanged(object sender, EventArgs e)
        {
            updRacPro();
        }

        private void lb_stavkeRac_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lb_stavkeRac.SelectedIndex != -1)
            {

                label5.Location = new Point(498, 100);
                label5.Text = racSta[lb_stavkeRac.SelectedIndex].Proiz.Ime.ToString();

                brProdArt();

                label6.Text = brPArt.ToString();

                brArt();

                label7.Text = brArtS.ToString();


                this.Paint += crtaj;
                this.Invalidate();
            }
            else
            {
                MessageBox.Show("Morate da izaberete stavku iz liste proizvodi");
            }
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            odbrMesec();
        }
    }
}
