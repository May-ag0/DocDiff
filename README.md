# DocDiff

[![CI](https://github.com/May-ag0/DocDiff/actions/workflows/ci.yml/badge.svg)](https://github.com/May-ag0/DocDiff/actions/workflows/ci.yml)

DocDiff is a small web application for comparing two versions of a text document. Upload an original and an updated `.txt` or `.md` file, and DocDiff shows line by line what was added, removed and left unchanged. Every comparison is logged to a history page.

![Comparing two versions of a contract](docs/screenshots/compare.png)

## Why I built it

I wanted to strengthen my practical .NET/C# experience by building a small full-stack application where application logic, persistence, testing and deployment work together.

With DocDiff I wanted to work through the full .NET workflow end to end: structuring a Blazor application, keeping business logic out of the UI with dependency injection, persisting data with Entity Framework Core, writing unit and integration tests with xUnit, and running it all through CI.

Comparing document versions is a common task wherever people work with contracts, policies and other text-heavy documents. That makes it a useful, well-defined problem for a portfolio project. DocDiff is a technical project and does not provide any legal functionality.

## Features

- **Upload two documents**: `.txt` and `.md`, up to 1 MB each
- **Line-by-line comparison** based on the Longest Common Subsequence (LCS) algorithm
- **Clear diff view** with added, removed and unchanged lines, line numbers for both versions and a change summary
- **Validation with clear error messages** for unsupported file types, files that are too large, binary content and files with too many lines
- **Comparison history** stored in SQLite, showing file names, timestamp and number of changes
- **Privacy by design**: only metadata is stored, never the contents of the documents
- **Handles different line endings**: files with Windows (`\r\n`) and Unix (`\n`) line endings compare correctly

| History                                       | Validation                                                                   |
| --------------------------------------------- | ---------------------------------------------------------------------------- |
| ![History page](docs/screenshots/history.png) | ![Validation error for an unsupported file](docs/screenshots/validation.png) |

## Tech stack

| Area          | Technology                                        |
| ------------- | ------------------------------------------------- |
| Language      | C# 14 / .NET 10                                   |
| Web framework | ASP.NET Core, Blazor Web App (Interactive Server) |
| Data access   | Entity Framework Core 10                          |
| Database      | SQLite                                            |
| Testing       | xUnit                                             |
| UI            | Bootstrap 5 with custom CSS                       |
| CI            | GitHub Actions                                    |                  |

## Architecture

The solution has two projects. The application logic is organised in folders instead of separate class libraries, which keeps the structure simple for an application of this size.

```
DocDiff/
├── src/DocDiff.Web/
│   ├── Components/
│   │   ├── Layout/            # MainLayout, NavMenu
│   │   ├── Pages/             # Home (compare), History
│   │   ├── DiffView.razor     # Renders a list of diff lines
│   │   └── DocumentPicker.razor  # File input + reading + validation
│   ├── Models/                # DiffLine, DiffLineType, ComparisonResult, DocumentComparison
│   ├── Services/              # Comparison logic, validation, history
│   ├── Data/                  # AppDbContext + EF Core migrations
│   └── Program.cs             # Dependency injection and app setup
├── tests/DocDiff.Tests/       # xUnit tests
└── samples/                   # Example documents to try the app with
```

### Key design decisions

- **The UI contains no business logic.** Razor components handle user interaction only. The diff algorithm lives in `DocumentComparisonService`, the validation rules in `UploadValidator` and data access in `ComparisonHistoryService`. Because of this separation, the logic can be tested without the UI.
- **Dependency injection throughout.** Components depend on interfaces (`IDocumentComparisonService`, `IComparisonHistoryService`), not on concrete classes. The comparison service is stateless and registered as a singleton.
- **`IDbContextFactory` for Blazor Server.** In Blazor Server a DI scope lives as long as the user's connection, which is far too long for a `DbContext`. `ComparisonHistoryService` therefore creates a short-lived context for each operation, which is the pattern Microsoft recommends for Blazor Server.
- **No repository layer.** EF Core's `DbContext` already implements the repository and unit-of-work patterns. A thin service with the two queries the app needs is enough.
- **Timestamps are stored in UTC.** SQLite has no date type, so a value converter marks values as UTC when they are read back. An integration test covers this conversion.

### How the diff works

`DocumentComparisonService` splits both documents into lines and builds an LCS table using dynamic programming. `table[i, j]` holds the length of the longest common subsequence of the remaining lines from position `i` in the original and `j` in the updated document. It then walks through both documents:

- Lines that are equal become **Unchanged**.
- Otherwise the table decides whether skipping the original line (**Removed**) or the updated line (**Added**) preserves the longest common subsequence.

A simpler approach that compares line 1 with line 1, line 2 with line 2 and so on breaks down as soon as a single line is inserted, because every following line then appears changed. LCS avoids this. The table needs O(n × m) memory, so uploads are limited to 5,000 lines per document.

## Running locally

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

```bash
git clone https://github.com/May-ag0/DocDiff.git
cd DocDiff
dotnet tool restore
dotnet run --project src/DocDiff.Web
```

`dotnet tool restore` installs the local `dotnet-ef` tool, which you only need if you create new migrations.

Open the URL printed in the terminal. The SQLite database (`docdiff.db`) is created automatically on first startup, because pending migrations are applied when the app starts.

To try the app, compare `samples/contract-v1.txt` with `samples/contract-v2.txt`.

## Testing

```bash
dotnet test
```

The test project contains 25 tests:

| Test class                       | What it covers                                                                                                 |
| -------------------------------- | -------------------------------------------------------------------------------------------------------------- |
| `DocumentComparisonServiceTests` | Identical documents, added, removed and changed lines, empty documents, line-ending differences, change counts |
| `UploadValidatorTests`           | Allowed file types, size limit boundaries, binary content, line limit                                          |
| `ComparisonHistoryServiceTests`  | Integration tests against an in-memory SQLite database: ordering, limits, UTC round-tripping                   |

The history tests run against a real SQLite database in memory instead of mocks, so the migrations, the query and the UTC conversion are all exercised. The tests focus on application logic rather than on UI components.

## Deployment

### Continuous integration

A [GitHub Actions workflow](.github/workflows/ci.yml) restores, builds and tests the solution on every push to `main` and on every pull request. The badge at the top of this README shows the result of the latest run.



## Possible future improvements

- **Word-level highlighting** within changed lines, so small edits are easier to spot
- **Side-by-side view** as an alternative to the unified diff
- **More efficient algorithm** (such as Myers' diff, or trimming common prefixes and suffixes) to support larger documents
- **Optional storage of document contents**, so past comparisons can be reopened from the history page
- **PDF and Word support** by extracting text before comparing
- **AI-generated summary** of the changes, for example with Azure OpenAI
- **Component and end-to-end tests** with bUnit and Playwright
