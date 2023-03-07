using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Pregled_Plata
{
    public partial class IzmjenaPozicija : Form
    {
        private string ime, prezime;
        private int id;
        DB db = null;

        List<RadniKPozicija> lista = new List<RadniKPozicija>();
        public IzmjenaPozicija(string ime, string prezime, int id)
        {
            InitializeComponent();

            label2.Text = ime;
            label5.Text = prezime;
            this.id = id;

            db=new DB();

            db.openConnection();

            fillData();

            //db.closeConnection();
        }

        private void fillData()
        {
            fillPozicije();
            fillBonusi();
            fillOdbitci();
            getPozicijeRadnik();
        }

        private void fillBonusi()
        {
            List<string> list = db.getRadnikBonusi(id);

            list.ForEach(b =>
            {
                listBox1.Items.Add(b);
            });
        }

        private void getPozicijeRadnik()
        {
            lista = db.getPozicijeRadnik(id);

            if(lista.Count == 0)
            {
                panel1.Enabled = false;
                return;
            }
            comboBox1.Items.Clear();
            lista.ForEach(b => {
               
                comboBox1.Items.Add(b.naziv);
            });
        }

        private void fillOdbitci()
        {
            List<string> list = db.getRadnikOdbitci(id);

            list.ForEach(b =>
            {
                listBox2.Items.Add(b);
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime od_datum = dateTimePicker4.Value;
            DateTime do_datum = dateTimePicker3.Value;

            int pozicija_id = db.getPozicijaId(comboBox2.Text);

            if(od_datum.Date < do_datum.Date)
            {
                if (db.addNewPozicijaToRadnik(id, pozicija_id, od_datum, do_datum) > -1)
                {
                    MessageBox.Show("Uspjesno ste dodali poziciju za radnika", pozicija_id.ToString());
                }
                else MessageBox.Show("Neuspjesno dodavanje");

            }
            else MessageBox.Show("Nepravilni datumi");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                int radnik_pozicija_id ;
                string stara_pozicija = comboBox1.Text;
                string nova_pozicija = comboBox3.Text;

                RadniKPozicija rp = new RadniKPozicija();

                lista.ForEach(l =>
                {
                    if(l.naziv == stara_pozicija)
                    {
                        radnik_pozicija_id = l.id;
                        rp.id = l.id;
                        rp.naziv = nova_pozicija;
                    }
                });
                updateRadnikPozicija(rp);
            }
            else
                MessageBox.Show("Izaberite poziciju");
        }

        private void updateRadnikPozicija(RadniKPozicija rp)
        {
            int pozicija_id = db.getPozicijaId(rp.naziv);
            int update = db.updateRadnikPozicija(pozicija_id, rp.id);

            db.updateRadnikPozicija(pozicija_id, rp.id);

            getPozicijeRadnik();
            
        }
        private void fillPozicije()
        {

            comboBox2.DataSource = db.searchPozicije();
            comboBox2.ValueMember = "naziv";

            comboBox3.DataSource = db.searchPozicije();
            comboBox3.ValueMember = "naziv";

        }
    }
}
