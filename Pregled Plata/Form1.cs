using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
namespace Pregled_Plata
{
    public partial class Form1 : Form
    {
      
        DB db = null;
        public Form1()
        {
            InitializeComponent();

            db = new DB();

            readAllRadnici();
            fillComboBox();
        }
        private void readAllRadnici()
        {
            db.openConnection();

            dataGridView1.DataSource = db.searchAllData();

            setDataGridHeader();

            db.closeConnection();
        }
        private void fillComboBox()
        {
            db.openConnection();

            pozicijaCmb.DataSource = db.searchPozicije();
            pozicijaCmb.ValueMember = "naziv";


            db.closeConnection();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["radnik_id"].Value);
            MessageBox.Show("ID", id.ToString());
            string ime = dataGridView1.Rows[e.RowIndex].Cells["ime"].Value.ToString();
            string prezime = dataGridView1.Rows[e.RowIndex].Cells["prezime"].Value.ToString();
        
            IzmjenaPozicija ip = new IzmjenaPozicija(ime, prezime, id);

            ip.Show();
        }

 
        private void setDataGridHeader()
        {
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            try
            {
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[0].HeaderCell.Value = "ID";
                dataGridView1.Columns[1].HeaderCell.Value = "Ime";
                dataGridView1.Columns[2].HeaderCell.Value = "Prezime";
                dataGridView1.Columns[3].HeaderCell.Value = "Preostali dani odmora";
                dataGridView1.Columns[4].HeaderCell.Value = "Rasporedjen od";
                dataGridView1.Columns[5].HeaderCell.Value = "Rasporedjen do";
                dataGridView1.Columns[6].HeaderCell.Value = "Pozicija";
                dataGridView1.Columns[7].HeaderCell.Value = "Ime nadredjenog";
                dataGridView1.Columns[8].HeaderCell.Value = "Prezime nadredjenog";
                dataGridView1.Columns[9].HeaderCell.Value = "Plata";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nema podataka u bazi", ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            db.openConnection();
            IstorijatPromjenePlate ip = new IstorijatPromjenePlate();
            ip.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();

            string pozicija = pozicijaCmb.Text;
            DateTime datum = dateTimePicker1.Value;

            db.openConnection();

            dataGridView1.DataSource = db.searchByDatumPozicija(pozicija, datum);
            setDataGridHeader();

            db.closeConnection();

        }
    }
}
