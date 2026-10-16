# Excel to SQL

Applicazione desktop locale: Rust + Tauri 2, TypeScript e Tailwind CSS 4. Non richiede Microsoft Excel né .NET.

## Avvio

Prerequisiti: Node.js 20.19+ o 22.12+, Rust stable e [prerequisiti Tauri della piattaforma](https://v2.tauri.app/start/prerequisites/).

```sh
npm ci
npm run tauri dev
```

## Verifica e distribuzione

```sh
npm run build
cargo test --locked --manifest-path src-tauri/Cargo.toml
npm run tauri build
```

I pacchetti vengono generati in `src-tauri/target/release/bundle`. Compilare sulla piattaforma di destinazione. Firma e notarizzazione richiedono credenziali del distributore.

`npm run dev` mostra soltanto l'interfaccia nel browser; importazione, generazione e appunti richiedono l'app desktop.

## Utilizzo

1. Apri `.xlsx` o `.xls` e seleziona il foglio.
2. Controlla intestazioni e celle. L'anteprima mostra 100 righe per pagina; la conversione include tutte le righe del foglio.
3. Inserisci un nome tabella oppure lascia `#temp`.
4. Genera e copia lo script SQL Server.

La prima riga dell'intervallo usato contiene le intestazioni. Le intestazioni vuote ricevono `Colonna_N`; nomi duplicati o non validi producono un errore. I nomi sono identificatori singoli: `dbo.Clienti` indica una tabella con quel nome letterale, non uno schema.

Tutte le colonne sono `NVARCHAR(MAX) NULL`. Il testo conserva spazi, apostrofi e Unicode. Celle vuote Excel diventano `NULL`; una cella modificata e svuotata diventa stringa vuota. Tasto destro o Alt+Canc sulla cella imposta `NULL`. Le modifiche invalidano lo script precedente e non modificano il file Excel.

Le formule usano il risultato salvato nel file: il programma non ricalcola Excel. Date riconosciute vengono convertite in testo data/ora; formattazioni personalizzate, macro e motore di calcolo Excel non vengono riprodotti. Errori nelle celle vengono segnalati. La transazione opzionale termina con `ROLLBACK TRANSACTION`. Nessuna connessione database, nessuna query eseguita e nessun upload.

## Regressione

`src-tauri/tests/fixtures/sample.xlsx` copre fogli multipli, foglio vuoto, intestazione mancante, Unicode, spazi, formula con risultato salvato, numeri e celle vuote. Rigenerazione: `python3 scripts/create-fixtures.py`.

La versione WinForms precedente resta recuperabile nella cronologia Git.

## Verifica eseguita

- TypeScript e build Vite/Tailwind completati.
- Otto test Rust passati, inclusa importazione XLS/XLSX e generazione SQL.
- Bundle macOS Intel di debug creato e processo avviato senza errori.
- Flusso UI verificato nel browser con IPC Tauri simulato (`scripts/ui-smoke.html`): importazione, modifica, generazione, copia, invalidazione, paginazione su 101 righe e foglio vuoto. La simulazione non prova dialoghi e appunti nativi.
- Nessuna verifica su Windows/Linux, firma, notarizzazione o SQL Server reale.

Il lockfile Cargo mantiene `zip 8.6.0`, richiesto da calamine 0.36.1 ma ritirato dal registro. Conservare `Cargo.lock` e usare `--locked`; un aggiornamento di calamine con supporto a una serie zip disponibile permetterà di rimuovere questo vincolo. Nessun sorgente di dipendenze modificato.
