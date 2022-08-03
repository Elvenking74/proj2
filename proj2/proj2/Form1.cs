using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace proj2
{
    public partial class Form1 : Form
    {
        Baza baza;
        List<KKategorija> kategorije;
        List<KProizvod> proizvodi;
        List<KRacun> racuni;
        int ukcena  = 0;
        int brinc = 1;

        public Form1()
        {
            InitializeComponent();

            baza = new Baza();
            kategorije = new List<KKategorija>();
            proizvodi = new List<KProizvod>();
            racuni = new List<KRacun>();
        }

        public void updProizLi()
        {
            int id1 = kategorije[lb_kat.SelectedIndex].Id;
            proizvodi.Clear();

            try
            {
                baza.otvoriKonekciju();


                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Kon;
                cmd.CommandText = @"SELECT * 
                                    FROM PROIZVOD 
                                    WHERE IdKat = " + id1 +
                                    @"
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

        public void updKatLi()
        {
            try
            {
                baza.otvoriKonekciju();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Kon;
                cmd.CommandText = @"SELECT *
                                    FROM KATEGORIJA
                                    ORDER BY ImeKat
                                    ";
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    KKategorija k = new KKategorija();
                    k.Id = int.Parse(reader["IdKategorije"].ToString());
                    k.Ime = reader["imeKat"].ToString();

                    kategorije.Add(k);
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

        public void updRacLi()
        {
            ukcena = Convert.ToInt32(textBox1.Text);
            int prv = -1;
            DateTime dt = DateTime.Now;
            try
            {
                baza.otvoriKonekciju();

                foreach (KRacun r in racuni)
                {
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = baza.Kon;

                    cmd.CommandText = @"
                            INSERT INTO RACUN
                            VALUES (@IdRacuna, @DatVreRac, @UkupnaCena, @IdProiz, @BrArt)";
                    cmd.Parameters.AddWithValue("IdRacuna", brinc);
                    cmd.Parameters.AddWithValue("DatVreRac", DateTime.Parse(dt.ToString()));
                    cmd.Parameters.AddWithValue("UkupnaCena", ukcena);
                    cmd.Parameters.AddWithValue("IdProiz", r.Proiz.Id);
                    cmd.Parameters.AddWithValue("BrArt", r.BrArt);

                    int rez = cmd.ExecuteNonQuery();
                    if (rez == 0)
                    {
                        MessageBox.Show("Doslo je do greske");
                        prv = 1;
                    }
                    else
                    {
                        prv = 0;
                    }
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
            brinc++;
            if (prv == 0)
            {
                MessageBox.Show("Uspešno dodavanje racuna");

                racuni.Clear();
                updRac();
            }
        }

        public void updRac()
        {
            ukcena = 0;
            lb_rac.DataSource = null;
            lb_rac.DataSource = racuni;

            foreach (KRacun r in racuni)
            {
                ukcena += r.Proiz.Cena * r.BrArt;
            }

            textBox1.Text = ukcena.ToString();
        }

        public void updProiz()
        {
            lb_pro.DataSource = null;
            lb_pro.DataSource = proizvodi;
        }

        public void chcBrinc()
        {
            try
            {
                baza.otvoriKonekciju();

                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Kon;
                
                cmd.CommandText = @"
                                SELECT IdRacuna
                                FROM RACUN
                                WHERE IdRacuna = 
                                (
                                    SELECT MAX(IdRacuna)
                                    FROM RACUN
                                )
                                ";
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int tmp = int.Parse(reader["IdRacuna"].ToString());
                    if (tmp >= brinc)
                        brinc = tmp + 1;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            updKatLi();

            lb_kat.DataSource = kategorije;

            chcBrinc();

        }

        private void lb_kat_SelectedIndexChanged(object sender, EventArgs e)
        {

            updProizLi();
            updProiz();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (lb_pro.SelectedIndex != -1)
            {
                KRacun r = new KRacun();
                int prov = 0;
                r.BrArt = Convert.ToInt32(numericUpDown1.Value);
                r.Proiz = proizvodi[lb_pro.SelectedIndex];
                foreach (KRacun ra in racuni)
                {
                    if (r.Proiz.Id == ra.Proiz.Id)
                    {
                        prov = 1;
                        ra.BrArt += r.BrArt;
                        break;
                    }
                }
                if (prov == 0)
                {
                    racuni.Add(r);
                }
                updRac();
            }
            else
            {
                MessageBox.Show("Morate izabrati stavku iz liste proizvodi koju bi ste dodali na račun");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (lb_rac.SelectedIndex != -1)
            {
                racuni.RemoveAt(lb_rac.SelectedIndex);

                updRac();
            }
            else
            {
                MessageBox.Show("Morate izabrati stavku sa računa koju bi ste obrisali");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lb_rac.SelectedIndex != -1)
            {
                updRacLi();
            }
            else
            {
                MessageBox.Show("Vaš račun je prazan");
            }
        }

        private void src_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                proizvodi.Clear();
                try
                {
                    baza.otvoriKonekciju();

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = baza.Kon;

                    cmd.CommandText = @"
                                        SELECT *
                                        FROM PROIZVOD
                                        WHERE ImeProiz LIKE '%" + textBox2.Text + @"%'
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
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    baza.zatvoriKonekciju();
                }
                updProiz();

            }
            else if (radioButton2.Checked)
            {
                proizvodi.Clear();
                try
                {
                    baza.otvoriKonekciju();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = baza.Kon;

                    cmd.CommandText = @"
                                        SELECT *
                                        FROM PROIZVOD
                                        WHERE IdKat = 
                                        (
                                            SELECT IdKategorije
                                            FROM KATEGORIJA
                                            WHERE ImeKat = '" + textBox2.Text + @"'
                                        )
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
                updProiz();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (lb_rac.SelectedIndex != -1)
            {
                racuni.Clear();
                updRac();
            }
            else
                MessageBox.Show("Vaša lista je već prazna");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FProiz f = new FProiz();
            f.ShowDialog();
            updProizLi();
            updProiz();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FStati f = new FStati();
            f.Show();
        }
    }
}
