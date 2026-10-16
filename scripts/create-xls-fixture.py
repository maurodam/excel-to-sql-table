"""Optional fixture generation: install xlwt, then run this script."""
from pathlib import Path
import xlwt
book = xlwt.Workbook()
sheet = book.add_sheet('Dati')
for col, name in enumerate(['Nome', 'Totale', 'Vuoto']):
    sheet.write(0, col, name)
sheet.write(1, 0, "  L'雪  ")
sheet.write(1, 1, 12.5)
book.add_sheet('Vuoto')
book.save(str(Path(__file__).resolve().parents[1] / 'src-tauri/tests/fixtures/sample.xls'))
