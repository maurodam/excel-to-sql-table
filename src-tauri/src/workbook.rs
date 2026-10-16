use crate::sql::Table;
use calamine::{open_workbook_auto, Data, Reader};
use serde::Serialize;

#[derive(Serialize)]
pub struct Sheet {
    name: String,
    headers: Vec<String>,
    rows: Vec<Vec<Option<String>>>,
    warnings: Vec<String>,
}
#[derive(Serialize)]
pub struct Workbook {
    sheets: Vec<Sheet>,
}

fn cell(value: &Data) -> Result<Option<String>, String> {
    match value {
        Data::Empty => Ok(None),
        Data::Error(error) => Err(format!("Errore Excel: {error:?}")),
        Data::DateTime(value) => Ok(Some(
            value
                .as_datetime()
                .map(|d| d.to_string())
                .unwrap_or_else(|| value.to_string()),
        )),
        _ => Ok(Some(value.to_string())),
    }
}

pub fn read(path: &str) -> Result<Workbook, String> {
    let extension = std::path::Path::new(path)
        .extension()
        .and_then(|v| v.to_str())
        .unwrap_or("")
        .to_lowercase();
    if !matches!(extension.as_str(), "xls" | "xlsx") {
        return Err("Seleziona un file .xls o .xlsx.".into());
    }
    let mut workbook =
        open_workbook_auto(path).map_err(|e| format!("Impossibile leggere il file: {e}"))?;
    let mut sheets = Vec::new();
    for name in workbook.sheet_names().to_vec() {
        let range = workbook
            .worksheet_range(&name)
            .map_err(|e| format!("Foglio {name}: {e}"))?;
        let mut records = range.rows();
        let mut warnings = Vec::new();
        let headers = match records.next() {
            None => vec![],
            Some(row) => row
                .iter()
                .enumerate()
                .map(|(i, value)| {
                    let header = cell(value)?.unwrap_or_default();
                    if header.trim().is_empty() {
                        warnings.push(format!(
                            "Colonna {} senza intestazione: assegnato Colonna_{}.",
                            i + 1,
                            i + 1
                        ));
                        Ok(format!("Colonna_{}", i + 1))
                    } else {
                        Ok(header)
                    }
                })
                .collect::<Result<Vec<_>, String>>()?,
        };
        let rows = records
            .enumerate()
            .map(|(i, row)| {
                row.iter()
                    .map(cell)
                    .collect::<Result<Vec<_>, _>>()
                    .map_err(|e| format!("Foglio {name}, riga {}: {e}", i + 2))
            })
            .collect::<Result<Vec<_>, _>>()?;
        if !headers.is_empty() {
            crate::sql::generate(
                &Table {
                    headers: headers.clone(),
                    rows: vec![],
                },
                "#temp",
                false,
            )
            .map_err(|e| format!("Foglio {name}: {e}"))?;
        }
        sheets.push(Sheet {
            name,
            headers,
            rows,
            warnings,
        });
    }
    Ok(Workbook { sheets })
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn imports_multiple_sheets_and_cached_formula() {
        let workbook = read(concat!(
            env!("CARGO_MANIFEST_DIR"),
            "/tests/fixtures/sample.xlsx"
        ))
        .unwrap();
        assert_eq!(workbook.sheets.len(), 2);
        let sheet = &workbook.sheets[0];
        assert_eq!(sheet.headers, ["Nome", "Totale", "Colonna_3"]);
        assert_eq!(
            sheet.rows[0],
            vec![Some("  L'雪  ".into()), Some("3".into()), None]
        );
        assert_eq!(sheet.rows[1][1], Some("12.5".into()));
        assert_eq!(sheet.warnings.len(), 1);
        assert!(workbook.sheets[1].headers.is_empty());
        let sql = crate::sql::generate(
            &Table {
                headers: sheet.headers.clone(),
                rows: sheet.rows.clone(),
            },
            "",
            true,
        )
        .unwrap();
        assert_eq!(sql.matches("INSERT INTO").count(), 2);
        assert!(sql.contains("N'  L''雪  '"));
    }
    #[test]
    fn empty_and_error_cells_remain_distinct() {
        assert_eq!(cell(&Data::Empty).unwrap(), None);
        assert_eq!(
            cell(&Data::String(String::new())).unwrap(),
            Some(String::new())
        );
        assert!(cell(&Data::Error(calamine::CellErrorType::Div0)).is_err());
    }
    #[test]
    fn formats_excel_dates_and_booleans() {
        let date = Data::DateTime(calamine::ExcelDateTime::new(
            45292.5,
            calamine::ExcelDateTimeType::DateTime,
            false,
        ));
        assert_eq!(cell(&date).unwrap(), Some("2024-01-01 12:00:00".into()));
        assert_eq!(cell(&Data::Bool(true)).unwrap(), Some("true".into()));
    }
    #[test]
    fn reports_invalid_files() {
        assert!(read("missing.xlsx").is_err());
        assert!(read("file.csv").is_err());
    }
}

#[cfg(test)]
mod legacy_tests {
    use super::*;
    #[test]
    fn imports_xls_without_excel() {
        let workbook = read(concat!(
            env!("CARGO_MANIFEST_DIR"),
            "/tests/fixtures/sample.xls"
        ))
        .unwrap();
        assert_eq!(workbook.sheets.len(), 2);
        assert_eq!(workbook.sheets[0].headers, ["Nome", "Totale", "Vuoto"]);
        assert_eq!(
            workbook.sheets[0].rows[0],
            vec![Some("  L'雪  ".into()), Some("12.5".into()), None]
        );
    }
}
