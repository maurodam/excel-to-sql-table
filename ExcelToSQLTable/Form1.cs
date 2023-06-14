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
        private Workbook workbook;
        Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

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
                    excel.Visible = false;

                    workbook = excel.Workbooks.Open(openFileDialog1.FileName);

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
                    FillComboBox(workbook);

                    dataGridView1.DataSource = dataTable;
                    FillCheckListBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Il file non è valido. Errore: " + ex.ToString());
                return;
            }
        }

        void FillCheckListBox()
        {
            checkedListBox1.Items.Clear();
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                checkedListBox1.Items.Add(col.Name);
            }
        }

        private void FillComboBox(Workbook workbook)
        {
            comboBox1.Items.Clear();
            foreach (Worksheet sheet in workbook.Sheets)
            {
                comboBox1.Items.Add(sheet.Name);
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
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

            if (CreateTable(tableName) == null)
                return;

                sqlOutput = (chkTran.Checked ? $"BEGIN TRAN test{Environment.NewLine}{Environment.NewLine}" : "") +
                            (chkDrop.Checked ? $"IF(OBJECT_ID('TEMPDB..{tableName}', 'U') IS NOT NULL) DROP TABLE {tableName}{ Environment.NewLine}" : "") +
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
            try
            {
                string selectedSheetName = comboBox1.SelectedItem.ToString();
                Worksheet selectedSheet = workbook.Sheets[selectedSheetName];

                Range range = selectedSheet.UsedRange;
                DataTable dataTable = new DataTable();

                for (int col = 1; col <= range.Columns.Count; col++)
                {
                    dataTable.Columns.Add(range.Cells[1, col].Value2.ToString());
                }

                for (int row = 2; row <= range.Rows.Count; row++)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int col = 1; col <= range.Columns.Count; col++)
                    {
                        dataRow[col - 1] = range.Cells[row, col].Value2;
                    }
                    dataTable.Rows.Add(dataRow);
                }

                dataGridView1.DataSource = dataTable;
                FillCheckListBox(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errore: " + ex.ToString());
                return;
            }
        }

        private void FillCheckListBox(DataTable dataTable)
        {
            checkedListBox1.Items.Clear();
            foreach (DataColumn column in dataTable.Columns)
            {
                checkedListBox1.Items.Add(column.ColumnName);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            for (var i = 0; i <= checkedListBox1.Items.Count-1; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
                workbook?.Close(false);
                excel?.Quit();
        }
    }
}
