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

pub fn generate(table: &Table, name: &str, transaction: bool) -> Result<String, String> {
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
        let sql = generate(&table, "#t]; DROP TABLE x;--", true).unwrap();
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
        assert!(generate(&table, "", false).is_err());
        table.headers = vec!["".into()];
        assert!(generate(&table, "", false).is_err());
        table.headers = vec!["ok".into()];
        table.rows = vec![vec![]];
        assert!(generate(&table, "", false).is_err());
    }
    #[test]
    fn defaults_to_temp_without_transaction() {
        let table = Table {
            headers: vec!["id".into()],
            rows: vec![],
        };
        let sql = generate(&table, " ", false).unwrap();
        assert!(sql.starts_with("CREATE TABLE [#temp]"));
        assert!(!sql.contains("TRANSACTION"));
        assert!(!sql.contains("INSERT"));
    }
}
