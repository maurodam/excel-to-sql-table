import './style.css';
import { invoke, isTauri } from '@tauri-apps/api/core';
import { open } from '@tauri-apps/plugin-dialog';
import { writeText } from '@tauri-apps/plugin-clipboard-manager';

type Sheet = { name: string; headers: string[]; rows: (string | null)[][]; warnings: string[] };
let sheets: Sheet[] = [];
let selected = 0;
let included: boolean[][] = [];
let page = 0;
let busy = false;
const pageSize = 100;
const app = document.querySelector<HTMLDivElement>('#app')!;
app.innerHTML = `
<header class="border-b border-slate-200 bg-white"><div class="mx-auto flex max-w-[1600px] items-center justify-between px-8 py-5"><div class="flex items-center gap-3"><span class="flex h-10 w-10 items-center justify-center rounded-xl bg-teal-800 font-mono text-lg text-white">SQL</span><div><h1 class="text-lg font-semibold tracking-tight">Excel to SQL</h1><p class="text-xs text-slate-500">Da foglio di calcolo a script SQL Server</p></div></div><span class="rounded-full bg-teal-50 px-3 py-1 text-xs font-medium text-teal-800">Locale · nessun upload</span></div></header>
<main class="mx-auto max-w-[1600px] px-8 py-8">
<div class="mb-7 flex flex-wrap items-end justify-between gap-4"><div><p class="mb-2 text-xs font-semibold uppercase tracking-[.16em] text-teal-700">Conversione</p><h2 class="text-3xl font-semibold tracking-tight">I tuoi dati, pronti per SQL.</h2><p class="mt-2 text-sm text-slate-500">Importa Excel, controlla le celle e genera lo script.</p></div><button id="import" class="primary">Apri file Excel <span aria-hidden="true">＋</span></button></div>
<p id="status" role="status" aria-live="polite" class="mb-5 min-h-5 text-sm text-slate-600">Seleziona un file .xlsx o .xls per iniziare.</p>
<div class="grid gap-6 xl:grid-cols-[minmax(0,1.15fr)_minmax(0,1fr)]">
<section class="panel flex min-w-0 flex-col"><div class="flex flex-wrap items-center justify-between gap-3 border-b border-slate-200 p-5"><div><h3 class="font-semibold">01 <span class="ml-2">Anteprima dati</span></h3><p id="filename" class="mt-1 max-w-80 truncate text-xs text-slate-500">Nessun file selezionato</p></div><label class="text-xs text-slate-500">Foglio <select id="sheet" disabled class="ml-2 max-w-48 rounded-lg border border-slate-200 bg-white p-2 text-sm text-slate-800"><option>—</option></select></label></div>
<fieldset id="columns" hidden class="border-b border-slate-200 px-5 py-4"><legend class="sr-only">Colonne da includere nello script</legend><div class="mb-3 flex items-center justify-between gap-3"><span class="text-xs font-medium text-slate-600">Colonne da includere</span><div class="flex gap-3 text-xs text-teal-800"><button id="all-columns" type="button">Tutte</button><button id="no-columns" type="button">Nessuna</button></div></div><div id="column-list" class="flex max-h-28 flex-wrap gap-x-4 gap-y-2 overflow-auto"></div><p class="mt-3 text-[11px] text-slate-400">Usa i pulsanti nelle intestazioni per riordinare le colonne, anche nello script.</p></fieldset>
<div id="warnings" class="hidden border-b border-amber-200 bg-amber-50 p-4 text-xs text-amber-900"></div>
<div id="grid" class="h-[420px] overflow-auto"><div class="flex h-full flex-col items-center justify-center gap-3 p-8 text-center"><span class="text-4xl text-teal-700" aria-hidden="true">▦</span><p class="font-medium">Il tuo foglio apparirà qui</p><p class="max-w-64 text-sm text-slate-500">Prima riga come intestazione. Celle modificabili prima della conversione.</p></div></div>
<div class="flex items-center justify-between border-t border-slate-200 px-5 py-3 text-xs text-slate-500"><span id="count">0 righe · 0 colonne</span><div class="flex items-center gap-3"><button id="previous" class="secondary" disabled aria-label="Pagina precedente">‹</button><span id="page"></span><button id="next" class="secondary" disabled aria-label="Pagina successiva">›</button></div></div>
<p class="border-t border-slate-100 px-5 py-3 text-xs text-slate-500">Cella vuota Excel = NULL. Modifica una cella per usare testo, anche vuoto. Tasto destro o Alt+Canc: imposta NULL.</p></section>
<section class="panel flex min-w-0 flex-col"><div class="border-b border-slate-200 p-5"><h3 class="font-semibold">02 <span class="ml-2">Script SQL</span></h3><p class="mt-1 text-xs text-slate-500">CREATE TABLE + INSERT · colonne NVARCHAR(MAX)</p></div>
<div class="grid gap-4 p-5 sm:grid-cols-2"><label class="text-xs font-medium text-slate-600">Nome tabella<input id="name" placeholder="#temp" class="mt-2 w-full rounded-lg border border-slate-200 px-3 py-2.5 text-sm text-slate-900" maxlength="128"><span class="mt-1 block text-[11px] font-normal text-slate-400">Vuoto: #temp. Nome singolo, senza schema.</span></label><label class="flex items-start gap-2 pt-7 text-sm"><input id="transaction" type="checkbox" checked class="mt-1 accent-teal-700"><span>Includi transazione<span class="mt-1 block text-xs text-slate-500">Termina con ROLLBACK</span></span></label></div>
<div class="flex flex-wrap gap-x-5 gap-y-3 px-5 pb-4 text-sm"><label class="flex items-center gap-2"><input id="include-select" type="checkbox" checked class="accent-teal-700">Includi SELECT finale</label><label class="flex items-center gap-2"><input id="drop-existing" type="checkbox" checked class="accent-teal-700">Includi DROP se esiste</label><p class="w-full text-xs text-slate-500">DROP elimina la tabella esistente quando esegui lo script. Con transazione, ROLLBACK annulla le modifiche. Con DROP attivo, usa SSMS o sqlcmd (batch GO).</p></div>
<div class="px-5 pb-5"><button id="generate" disabled class="primary w-full">Genera script SQL</button></div>
<div class="mx-5 flex items-center justify-between rounded-t-lg bg-slate-900 px-4 py-3 text-xs text-slate-400"><span>SQL SERVER</span><div class="flex gap-4"><button id="clear" disabled class="text-slate-300 hover:text-white">Svuota</button><button id="copy" disabled class="text-teal-300 hover:text-teal-100">Copia script</button></div></div>
<textarea id="output" aria-label="Script SQL generato" readonly spellcheck="false" placeholder="-- Lo script generato apparirà qui" class="mx-5 mb-5 min-h-64 flex-1 resize-y rounded-b-lg border-0 bg-slate-900 p-4 font-mono text-xs leading-6 text-teal-100 outline-none"></textarea></section></div>
<footer class="mt-6 text-xs text-slate-400">Le formule usano valori salvati nel file. Nessuna query viene eseguita sul database.</footer></main>`;
const el = <T extends HTMLElement>(id: string) => document.getElementById(id) as T;
const status = (text: string, error = false) => { el('status').textContent = text; el('status').className = `mb-5 min-h-5 text-sm ${error ? 'text-red-700' : 'text-slate-600'}`; };
const output = el<HTMLTextAreaElement>('output');
function invalidate() { output.value = ''; buttons(); }
function buttons() {
  for (const id of ['import', 'sheet', 'name', 'transaction', 'include-select', 'drop-existing']) (el(id) as HTMLInputElement).disabled = busy || (id === 'sheet' && !sheets.length);
  el<HTMLButtonElement>('generate').disabled = busy || !included[selected]?.some(Boolean);
  for (const id of ['copy', 'clear']) el<HTMLButtonElement>(id).disabled = busy || !output.value;
  el('grid').querySelectorAll('input').forEach(input => { input.disabled = busy; });
  el('columns').querySelectorAll('input, button').forEach(control => { (control as HTMLInputElement).disabled = busy; });
  el('grid').querySelectorAll<HTMLButtonElement>('button').forEach(button => { button.disabled = busy || button.dataset.boundary === 'true'; });
  const total = sheets[selected]?.rows.length ?? 0;
  el<HTMLButtonElement>('previous').disabled = busy || page === 0;
  el<HTMLButtonElement>('next').disabled = busy || (page + 1) * pageSize >= total;
}
function renderColumns() {
  const sheet = sheets[selected];
  el('columns').hidden = !sheet?.headers.length;
  const list = el('column-list'); list.replaceChildren();
  sheet?.headers.forEach((header, index) => {
    const label = document.createElement('label'); label.className = 'flex items-center gap-2 text-xs text-slate-600';
    const check = document.createElement('input'); check.type = 'checkbox'; check.className = 'accent-teal-700';
    check.checked = included[selected][index];
    check.setAttribute('aria-label', `Includi colonna ${index + 1}: ${header}`);
    check.addEventListener('change', () => { included[selected][index] = check.checked; invalidate(); });
    const text = document.createElement('span'); text.textContent = header;
    label.append(check, text); list.append(label);
  });
}
function moveColumn(index: number, offset: number) {
  const sheet = sheets[selected]; const next = index + offset;
  if (busy || next < 0 || next >= sheet.headers.length) return;
  [sheet.headers[index], sheet.headers[next]] = [sheet.headers[next], sheet.headers[index]];
  [included[selected][index], included[selected][next]] = [included[selected][next], included[selected][index]];
  sheet.rows.forEach(row => { [row[index], row[next]] = [row[next], row[index]]; });
  invalidate(); renderGrid();
}
function renderGrid() {
  const sheet = sheets[selected];
  const grid = el('grid'); grid.replaceChildren();
  const table = document.createElement('table'); table.className = 'w-full border-collapse text-left text-xs';
  const head = table.createTHead().insertRow();
  const number = document.createElement('th'); number.textContent = '#'; head.append(number);
  sheet.headers.forEach((header, index) => {
    const th = document.createElement('th'); const input = document.createElement('input');
    input.value = header; input.setAttribute('aria-label', `Intestazione colonna ${index + 1}`);
    input.addEventListener('input', () => { sheet.headers[index] = input.value; renderColumns(); invalidate(); });
    const controls = document.createElement('div'); controls.className = 'flex justify-end gap-2 pt-1';
    for (const [offset, symbol, direction] of [[-1, '‹', 'sinistra'], [1, '›', 'destra']] as const) {
      const button = document.createElement('button'); button.type = 'button'; button.className = 'secondary'; button.textContent = symbol;
      button.setAttribute('aria-label', `Sposta ${header} a ${direction}`);
      button.dataset.boundary = String(index + offset < 0 || index + offset >= sheet.headers.length);
      button.addEventListener('click', () => moveColumn(index, offset)); controls.append(button);
    }
    th.append(input, controls); head.append(th);
  });
  const body = table.createTBody();
  sheet.rows.slice(page * pageSize, (page + 1) * pageSize).forEach((row, offset) => {
    const tr = body.insertRow(); tr.insertCell().textContent = String(page * pageSize + offset + 2);
    row.forEach((value, index) => {
      const td = tr.insertCell(); const input = document.createElement('input');
      input.value = value ?? ''; input.placeholder = value === null ? 'NULL' : '';
      input.setAttribute('aria-label', `Riga ${page * pageSize + offset + 2}, ${sheet.headers[index]}`);
      input.addEventListener('input', () => { row[index] = input.value; input.placeholder = ''; invalidate(); });
      const setNull = () => { row[index] = null; input.value = ''; input.placeholder = 'NULL'; invalidate(); };
      input.addEventListener('contextmenu', event => { event.preventDefault(); if (!busy) setNull(); });
      input.addEventListener('keydown', event => { if (event.altKey && event.key === 'Delete') { event.preventDefault(); setNull(); } });
      td.append(input);
    });
  });
  if (sheet.headers.length) grid.append(table);
  else { const empty = document.createElement('p'); empty.className = 'p-8 text-sm text-slate-500'; empty.textContent = 'Foglio vuoto. Seleziona un altro foglio.'; grid.append(empty); }
  el('count').textContent = `${sheet.rows.length} righe · ${sheet.headers.length} colonne`;
  el('page').textContent = `${page + 1} / ${Math.max(1, Math.ceil(sheet.rows.length / pageSize))}`;
  el('warnings').textContent = sheet.warnings.join(' ');
  el('warnings').classList.toggle('hidden', !sheet.warnings.length);
  renderColumns(); buttons();
}
el('import').addEventListener('click', async () => {
  if (!isTauri()) { status('Apri l’app desktop con npm run tauri dev per importare file Excel.', true); return; }
  busy = true; buttons();
  try {
    const path = await open({ multiple: false, filters: [{name: 'Excel', extensions: ['xlsx', 'xls']}] });
    if (!path) return;
    status('Lettura del file Excel…');
    const workbook = await invoke<{sheets: Sheet[]}>('import_workbook', {path});
    if (!workbook.sheets.length) throw new Error('Il file non contiene fogli.');
    sheets = workbook.sheets; included = sheets.map(sheet => sheet.headers.map(() => true)); selected = 0; page = 0;
    const select = el<HTMLSelectElement>('sheet'); select.replaceChildren();
    sheets.forEach((sheet, i) => select.add(new Option(sheet.name, String(i))));
    el('filename').textContent = path.split(/[\\/]/).pop()!;
    invalidate(); renderGrid(); status('File caricato. Controlla i dati prima di generare SQL.');
  } catch (error) { status(String(error), true); }
  finally { busy = false; buttons(); }
});
el('sheet').addEventListener('change', () => { selected = Number(el<HTMLSelectElement>('sheet').value); page = 0; invalidate(); renderGrid(); });
el('previous').addEventListener('click', () => { page--; renderGrid(); });
el('next').addEventListener('click', () => { page++; renderGrid(); });
el('name').addEventListener('input', invalidate);
for (const id of ['transaction', 'include-select', 'drop-existing']) el(id).addEventListener('change', invalidate);
for (const [id, value] of [['all-columns', true], ['no-columns', false]] as const) {
  el(id).addEventListener('click', () => { included[selected].fill(value); renderColumns(); invalidate(); });
}
el('generate').addEventListener('click', async () => {
  busy = true; buttons(); status('Generazione SQL…');
  try {
    const sheet = sheets[selected];
    const columns = sheet.headers.flatMap((_, index) => included[selected][index] ? [index] : []);
    if (!columns.length) throw new Error('Seleziona almeno una colonna.');
    output.value = await invoke<string>('generate_sql', {
      table: { headers: columns.map(index => sheet.headers[index]), rows: sheet.rows.map(row => columns.map(index => row[index])) },
      name: el<HTMLInputElement>('name').value,
      transaction: el<HTMLInputElement>('transaction').checked,
      includeSelect: el<HTMLInputElement>('include-select').checked,
      dropExisting: el<HTMLInputElement>('drop-existing').checked,
    });
    status(`Script generato: ${sheet.rows.length} INSERT. Nessuna query eseguita.`);
  } catch (error) { status(String(error), true); }
  finally { busy = false; buttons(); }
});
el('clear').addEventListener('click', () => { invalidate(); status('Output svuotato.'); });
el('copy').addEventListener('click', async () => {
  try { await writeText(output.value); status('Script copiato negli appunti.'); }
  catch (error) { status(String(error), true); }
});
