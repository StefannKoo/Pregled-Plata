using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pregled_Plata
{
    public partial class IstorijatPromjenePlate : Form
    {
        DB db = null;
        public IstorijatPromjenePlate()
        {
            InitializeComponent();
            db= new DB();
            fillDataGrid();
        }

        private void fillDataGrid()
        {
            db.openConnection();

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            dataGridView1.DataSource = db.showIstorijat();
            try
            {
                dataGridView1.Columns[0].HeaderCell.Value = "Ime radnika";
                dataGridView1.Columns[1].HeaderCell.Value = "Prezime radnika";
                dataGridView1.Columns[2].HeaderCell.Value = "Pozicija";
                dataGridView1.Columns[3].HeaderCell.Value = "Iznos";
                dataGridView1.Columns[4].HeaderCell.Value = "Datum promjene plate";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nema podataka u bazi", ex.Message);
            }
        }
    }
}
