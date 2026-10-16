use serde::{Deserialize, Serialize};
use std::collections::HashSet;
use std::fmt::Write;

#[derive(Debug, Serialize, Deserialize)]
pub struct Table {
    pub headers: Vec<String>,
    pub rows: Vec<Vec<Option<String>>>,
}

fn identifier(value: &str) -> Result<String, String> {
    if value.is_empty() || value.encode_utf16().count() > 128 || value.chars().any(char::is_control)
    {
        return Err("Nome SQL vuoto, troppo lungo o con caratteri di controllo.".into());
    }
    Ok(format!("[{}]", value.replace(']', "]]")))
}

pub fn generate(
    table: &Table,
    name: &str,
    transaction: bool,
    include_select: bool,
    drop_existing: bool,
) -> Result<String, String> {
    if table.headers.is_empty() {
        return Err("Nessuna colonna da convertire.".into());
    }
    let name = if name.trim().is_empty() {
        "#temp"
    } else {
        name.trim()
    };
    if name.starts_with('#') && name.encode_utf16().count() > 116 {
        return Err("Nome tabella temporanea: massimo 116 caratteri.".into());
    }
    let lookup = if name.starts_with('#') {
        format!("tempdb..{}", identifier(name)?)
    } else {
        identifier(name)?
    }
    .replace('\'', "''");
    let name = identifier(name)?;
    let mut seen = HashSet::new();
    let columns: Vec<String> = table
        .headers
        .iter()
        .map(|header| {
            if !seen.insert(header.to_lowercase()) {
                return Err(format!("Colonna duplicata: {header}"));
            }
            identifier(header)
        })
        .collect::<Result<_, String>>()?;
    let mut output = String::new();
    if transaction {
        output.push_str("BEGIN TRANSACTION;\n\n");
    }
    if drop_existing {
        writeln!(
            output,
            "IF OBJECT_ID(N'{lookup}', N'U') IS NOT NULL DROP TABLE {name};\nGO\n"
        )
        .unwrap();
    }
    writeln!(
        output,
        "CREATE TABLE {name} (\n  {}\n);\n",
        columns
            .iter()
            .map(|c| format!("{c} NVARCHAR(MAX) NULL"))
            .collect::<Vec<_>>()
            .join(",\n  ")
    )
    .unwrap();
    if drop_existing {
        output.push_str("GO\n\n");
    }
    for row in &table.rows {
        if row.len() != columns.len() {
            return Err("Numero celle diverso dal numero colonne.".into());
        }
        let values = row
            .iter()
            .map(|cell| match cell {
                None => "NULL".to_string(),
                Some(value) => format!("N'{}'", value.replace('\'', "''")),
            })
            .collect::<Vec<_>>()
            .join(", ");
        writeln!(
            output,
            "INSERT INTO {name} ({}) VALUES ({values});",
            columns.join(", ")
        )
        .unwrap();
    }
    if include_select {
        writeln!(output, "\nSELECT * FROM {name};").unwrap();
    }
    if transaction {
        output.push_str("\nROLLBACK TRANSACTION;\n");
    }
    Ok(output)
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn preserves_data_and_quotes_identifiers() {
        let table = Table {
            headers: vec!["a]b".into(), "vuoto".into(), "nullo".into()],
            rows: vec![vec![Some("  l'雪  ".into()), Some("".into()), None]],
        };
        let sql = generate(&table, "#t]; DROP TABLE x;--", true, false, false).unwrap();
        assert!(sql.contains("[#t]]; DROP TABLE x;--]"));
        assert!(sql.contains("[a]]b]"));
        assert!(sql.contains("N'  l''雪  ', N'', NULL"));
        assert!(sql.ends_with("ROLLBACK TRANSACTION;\n"));
    }
    #[test]
    fn rejects_invalid_shapes_and_headers() {
        let mut table = Table {
            headers: vec!["A".into(), "a".into()],
            rows: vec![],
        };
        assert!(generate(&table, "", false, false, false).is_err());
        table.headers = vec!["".into()];
        assert!(generate(&table, "", false, false, false).is_err());
        table.headers = vec!["ok".into()];
        table.rows = vec![vec![]];
        assert!(generate(&table, "", false, false, false).is_err());
    }
    #[test]
    fn defaults_to_temp_without_transaction() {
        let table = Table {
            headers: vec!["id".into()],
            rows: vec![],
        };
        let sql = generate(&table, " ", false, false, false).unwrap();
        assert!(sql.starts_with("CREATE TABLE [#temp]"));
        assert!(!sql.contains("TRANSACTION"));
        assert!(!sql.contains("INSERT"));
    }
    #[test]
    fn optional_statements_follow_execution_order() {
        let table = Table {
            headers: vec!["id".into()],
            rows: vec![vec![Some("1".into())]],
        };
        for transaction in [false, true] {
            for include_select in [false, true] {
                for drop_existing in [false, true] {
                    let sql = generate(&table, "#temp", transaction, include_select, drop_existing)
                        .unwrap();
                    assert_eq!(sql.contains("BEGIN TRANSACTION"), transaction);
                    assert_eq!(sql.contains("ROLLBACK TRANSACTION"), transaction);
                    assert_eq!(sql.contains("SELECT * FROM [#temp];"), include_select);
                    assert_eq!(sql.contains("DROP TABLE"), drop_existing);
                    let create = sql.find("CREATE TABLE").unwrap();
                    let insert = sql.find("INSERT INTO").unwrap();
                    assert!(create < insert);
                    if drop_existing {
                        assert!(sql.find("DROP TABLE").unwrap() < create);
                        assert!(sql.contains("OBJECT_ID(N'tempdb..[#temp]', N'U')"));
                        assert!(sql[..create].contains("\nGO\n"));
                        assert!(sql[create..insert].contains("\nGO\n"));
                    }
                    if include_select {
                        assert!(insert < sql.find("SELECT *").unwrap());
                        if transaction {
                            assert!(sql.find("SELECT *").unwrap() < sql.find("ROLLBACK").unwrap());
                        }
                    }
                }
            }
        }
    }
    #[test]
    fn drop_checks_current_database_for_permanent_tables_and_escapes_names() {
        let table = Table {
            headers: vec!["id".into()],
            rows: vec![],
        };
        let sql = generate(&table, "L'anno]", false, true, true).unwrap();
        assert!(sql.contains("OBJECT_ID(N'[L''anno]]]', N'U')"));
        assert!(sql.contains("DROP TABLE [L'anno]]];"));
        assert!(!sql.contains("tempdb"));
        assert!(generate(
            &Table {
                headers: vec![],
                rows: vec![]
            },
            "#temp",
            false,
            true,
            true
        )
        .is_err());
    }
}
