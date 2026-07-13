# Keyboard Analysis

A WinUI 3 desktop foundation for locally capturing keyboard activity and evolving it into analytics features.

## Structure

- `Domain` — business model (`KeyEvent`)
- `Application` — use-case coordination and abstractions
- `Infrastructure` — Windows keyboard capture and SQLite adapters
- `Presentation` — WinUI MVVM view models

`KeyEvent` defines the required persisted data: timestamp, key code, duration, and active application. The current adapters are deliberate placeholders; future work can implement the Windows low-level hook, foreground-process resolution, SQLite schema and queries, and dashboard views without changing the application-facing contracts.
