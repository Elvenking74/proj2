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
    public partial class FProiz : Form
    {
        Baza baza;
        List<KKategorija> kategorije;
        int brinc = 1;
        int ca;
        public FProiz()
        {
            InitializeComponent();
            baza = new Baza();
            kategorije = new List<KKategorija>();
        }

        public void chkBrinc()
        {
            try
            {
                baza.otvoriKonekciju();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = baza.Kon;

                cmd.CommandText = @"
                                    SELECT IdProiz
                                    FROM PROIZVOD
                                    WHERE IdProiz = 
                                    (
                                        SELECT MAX(IdProiz)
                                        FROM PROIZVOD
                                    )
                                    ";

                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int tmp = int.Parse(reader["IdProiz"].ToString());
                    if (tmp >= brinc)
                        brinc = tmp + 1;
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
        }

        private void FProiz_Load(object sender, EventArgs e)
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

            comboBox1.DataSource = kategorije;

            chkBrinc();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && Int32.TryParse(textBox2.Text, out ca))
            {
                try
                {
                    baza.otvoriKonekciju();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = baza.Kon;

                    cmd.CommandText = @"
                                    INSERT INTO PROIZVOD
                                    VALUES(@IdProiz, @ImeProiz, @IdKat, @Cena)
                                    ";
                    cmd.Parameters.AddWithValue("IdProiz", brinc);
                    cmd.Parameters.AddWithValue("ImeProiz", textBox1.Text);
                    cmd.Parameters.AddWithValue("IdKat", kategorije[comboBox1.SelectedIndex].Id);
                    cmd.Parameters.AddWithValue("Cena", Int32.Parse(textBox2.Text));

                    int rez = cmd.ExecuteNonQuery();
                    if (rez == 0)
                    {
                        MessageBox.Show("Doslo je do greske");
                    }
                    if (rez != 0)
                    {
                        MessageBox.Show("Uspešno dodavanje proizvoda");
                        textBox1.Clear();
                        textBox2.Clear();
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
            }
            else
                MessageBox.Show("Morate popuniti sva polja i cena mora biti broj");
            

        }
    }
}
