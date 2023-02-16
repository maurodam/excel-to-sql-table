using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace ExcelToSQLTable
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Crea un'istanza del controllo OpenFileDialog
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Imposta il filtro di selezione file su Excel
            openFileDialog1.Filter = "Excel Files|*.xlsx;*.xls";

            // Mostra la finestra di dialogo per la selezione del file
            DialogResult result = openFileDialog1.ShowDialog();

            // Se l'utente ha selezionato un file
            if (result == DialogResult.OK)
            {
                // Crea un'istanza dell'applicazione Excel
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

                // Imposta l'opzione di visualizzazione dell'applicazione Excel su false
                excel.Visible = false;

                // Apri il file Excel selezionato
                Workbook workbook = excel.Workbooks.Open(openFileDialog1.FileName);

                // Seleziona il foglio di lavoro desiderato
                Worksheet worksheet = workbook.Sheets[1];

                // Ottieni l'intervallo di celle utilizzato dal foglio di lavoro
                Range range = worksheet.UsedRange;

                // Crea un oggetto DataTable per contenere i dati dal foglio di lavoro
                DataTable dataTable = new DataTable();

                // Leggi i dati dall'intervallo di celle e inseriscili nella DataTable
                for (int row = 1; row <= range.Rows.Count; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int col = 1; col <= range.Columns.Count; col++)
                    {
                        if (row == 1)
                        {
                            // Aggiungi le intestazioni delle colonne alla DataTable
                            dataTable.Columns.Add(range.Cells[row, col].Value2.ToString());
                        }
                        else
                        {
                            // Aggiungi i dati delle celle alla DataTable
                            dataRow[col - 1] = range.Cells[row, col].Value2;
                        }
                    }
                    if (row != 1)
                    {
                        dataTable.Rows.Add(dataRow);
                    }
                }

                // Chiudi il file Excel
                workbook.Close(false);
                excel.Quit();

                // Mostra i dati nella DataGridView
                dataGridView1.DataSource = dataTable;
            }
        }

        private String CreateTable()
        {
            // Crea un oggetto DataTable vuoto con il nome della DataGridView
            DataTable dataTable = new DataTable(dataGridView1.Name);

            // Aggiungi le colonne alla DataTable
            foreach (DataGridViewColumn dataGridViewColumn in dataGridView1.Columns)
            {
                dataTable.Columns.Add(dataGridViewColumn.Name);
            }

            // Ottieni i nomi delle colonne come stringa separate da virgole
            string columnNames = string.Join(", ", dataTable.Columns.Cast<DataColumn>().Select(c => $"[{c.ColumnName}] NVARCHAR(MAX)"));

            // Costruisci lo script CREATE TABLE
            string createTableScript = $"CREATE TABLE #temp ({columnNames})";

            // Mostra lo script nella casella di testo
            return createTableScript;
        }


        void generateScript()
        {
            var tableName = string.IsNullOrEmpty(textBox1.Text.Trim()) ? "#temp" : textBox1.Text.Trim();

            DataTable dataTable = new DataTable(dataGridView1.Name);
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                dataTable.Columns.Add(column.Name);
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                object[] rowData = new object[row.Cells.Count];
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    rowData[i] = row.Cells[i].Value;
                }
                dataTable.Rows.Add(rowData);
            }

            string sqlOutput = "";
            foreach (DataRow row in dataTable.Rows)
            {
                string rowValues = "";
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    rowValues += row[i].ToString().Replace("'", "''").Trim();

                    if (i < dataTable.Columns.Count - 1)
                    {
                        rowValues += "','";
                    }
                }
                var sqlQuery = "INSERT INTO " + tableName + $" VALUES ('{rowValues}'){Environment.NewLine}";

                sqlOutput += sqlQuery;
            }

            textBox2.Text = CreateTable() + Environment.NewLine + Environment.NewLine + sqlOutput;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            generateScript();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }
    }
}
