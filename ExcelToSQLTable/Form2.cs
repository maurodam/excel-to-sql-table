using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using DataTable = System.Data.DataTable;

namespace ExcelToSQLTable
{
    public partial class Form2 : Form
    {
        private DataTable dataTable;
        private string jsonString;

        public Form2()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string jsonString = txtJson.Text;

            try
            {
                DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(jsonString);
                dataGridView.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il caricamento del JSON: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Salva le modifiche nel DataTable
                dataTable = (DataTable)dataGridView.DataSource;

                // Serializza il DataTable in una stringa JSON
                jsonString = JsonConvert.SerializeObject(dataTable, Formatting.Indented);

                // Verifica se la stringa JSON non ha i tag di apertura e chiusura "[ ]"
                if (!jsonString.StartsWith("[") || !jsonString.EndsWith("]"))
                {
                    // Aggiungi i tag di apertura e chiusura "[ ]"
                    jsonString = "[" + jsonString + "]";
                }

                // Scrivi la stringa JSON nel file di testo
                //File.WriteAllText("data.json", jsonString);

                //MessageBox.Show("Modifiche salvate correttamente.", "Salvataggio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtJson.Text = jsonString;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore durante il salvataggio del JSON: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Inizializza il DataTable e la DataGridView
            dataTable = new DataTable();
            dataGridView.DataSource = dataTable;
        }
    }
}
