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
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.Filter = "Excel Files|*.xlsx;*.xls";

                DialogResult result = openFileDialog1.ShowDialog();

                if (result == DialogResult.OK)
                {
                    Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

                    excel.Visible = false;

                    Workbook workbook = excel.Workbooks.Open(openFileDialog1.FileName);

                    Worksheet worksheet = workbook.Sheets[1];

                    Range range = worksheet.UsedRange;

                    DataTable dataTable = new DataTable();

                    for (int row = 1; row <= range.Rows.Count; row++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        for (int col = 1; col <= range.Columns.Count; col++)
                        {
                            if (row == 1)
                                dataTable.Columns.Add(range.Cells[row, col].Value2.ToString());
                            else
                                dataRow[col - 1] = range.Cells[row, col].Value2;
                        }
                        if (row != 1)
                            dataTable.Rows.Add(dataRow);
                    }

                    workbook.Close(false);
                    excel.Quit();

                    dataGridView1.DataSource = dataTable;
                    FillCheckListBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Il file non è valido. Errore: " + ex.ToString());
                return;
            }
            MessageBox.Show("File caricato.");
        }

        void FillCheckListBox()
        {
            checkedListBox1.Items.Clear();
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                checkedListBox1.Items.Add(col.Name);
            }
        }

        private String CreateTable(string tableName)
        {
            if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Seleziona almeno una colonna.", "Error");
                return null;
            }

            DataTable dataTable = new DataTable(dataGridView1.Name);

            foreach (DataGridViewColumn dataGridViewColumn in dataGridView1.Columns)
            {
                dataTable.Columns.Add(dataGridViewColumn.Name);
            }

            var checkedColumns = checkedListBox1.CheckedItems.Cast<string>();
            var columnNames = string.Join(", ", dataTable.Columns.Cast<DataColumn>()
                                            .Where(c => checkedColumns.Contains(c.ColumnName))
                                            .Select(c => $"[{c.ColumnName}] NVARCHAR(MAX)"));
            var createTableScript = $"CREATE TABLE {tableName} ({columnNames})";

            return createTableScript;
        }


        void generateScript()
        {
            var tableName = string.IsNullOrEmpty(textBox1.Text.Trim()) ? "#temp" : textBox1.Text.Trim();

            DataTable dataTable = new DataTable(dataGridView1.Name);

            foreach (var item in checkedListBox1.CheckedItems)
            {
                DataGridViewColumn column = dataGridView1.Columns.Cast<DataGridViewColumn>().FirstOrDefault(c => c.Name == item.ToString());
                if (column != null)
                {
                    dataTable.Columns.Add(column.Name);
                }
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                object[] rowData = new object[dataTable.Columns.Count];
                int i = 0;
                foreach (DataColumn column in dataTable.Columns)
                {
                    DataGridViewCell cell = row.Cells.Cast<DataGridViewCell>().FirstOrDefault(c => c.OwningColumn.Name == column.ColumnName);
                    if (cell != null)
                    {
                        rowData[i] = cell.Value;
                    }
                    i++;
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
                var sqlQuery = $"INSERT INTO {tableName} VALUES ('{rowValues}'){Environment.NewLine}";

                sqlOutput += sqlQuery;
            }

                sqlOutput = (chkTran.Checked ? $"BEGIN TRAN test{Environment.NewLine}{Environment.NewLine}" : "") +
                    CreateTable(tableName) +
                    Environment.NewLine + Environment.NewLine +
                    sqlOutput +
                    Environment.NewLine + Environment.NewLine +
                    (chkSelect.Checked ? $"SELECT * FROM {tableName}{Environment.NewLine}{Environment.NewLine}" : "") +
                    (chkTran.Checked ? "ROLLBACK TRAN test" : "");

            txtOutput.Text = sqlOutput;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            generateScript();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtOutput.Text = "";
            checkedListBox1.Items.Clear();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            txtOutput.Text = "";
        }

        private void btnCopia_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtOutput.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //TO DO: aggiungere selezione foglio in caso di fogli multipli in Excel
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (var i = 0; i <= checkedListBox1.Items.Count-1; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
        }

    }
}
