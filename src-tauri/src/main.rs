#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]
mod sql;
mod workbook;

#[tauri::command]
async fn import_workbook(path: String) -> Result<workbook::Workbook, String> {
    tauri::async_runtime::spawn_blocking(move || workbook::read(&path))
        .await
        .map_err(|e| e.to_string())?
}

#[tauri::command]
async fn generate_sql(
    table: sql::Table,
    name: String,
    transaction: bool,
    include_select: bool,
    drop_existing: bool,
) -> Result<String, String> {
    tauri::async_runtime::spawn_blocking(move || {
        sql::generate(&table, &name, transaction, include_select, drop_existing)
    })
    .await
    .map_err(|e| e.to_string())?
}

fn main() {
    tauri::Builder::default()
        .plugin(tauri_plugin_dialog::init())
        .plugin(tauri_plugin_clipboard_manager::init())
        .invoke_handler(tauri::generate_handler![import_workbook, generate_sql])
        .run(tauri::generate_context!())
        .expect("Impossibile avviare Excel to SQL");
}
