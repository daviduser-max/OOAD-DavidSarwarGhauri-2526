# CLAUDE.md - Agent Instructions

## Commands

- **Build solution**: `dotnet build SlnExamen.slnx`
- **Run WPF app**: `dotnet run --project WpfDierenarts/WpfDierenarts.csproj`
- **Clean build artifacts**: `dotnet clean SlnExamen.slnx`

## Architecture & Code Guidelines

- **Multi-Tier Design**: Separation of concerns. Do not mix SQL logic in the presentational WPF project, and do not reference WPF classes or controls in the Class Library (`CLDierenarts`).
- **Database**: SQLite database `dieren.db` queried via `Microsoft.Data.Sqlite` ADO.NET classes (`SqliteConnection`, `SqliteCommand`, `SqliteDataReader`).
- **OOP Inheritance**:
  - `Dier` is the abstract base class.
  - `Kat` inherits from `Dier` and adds `IsGevaccineerd`.
  - `Hond` inherits from `Dier` and adds `Ras`.
- **Validation**: `DierValidator` handles rules for `IsGeldigNaam` and `IsGeldigRas`.
