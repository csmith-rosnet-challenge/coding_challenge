# TodoApi (ASP.NET Core)

This repository contains a minimal ASP.NET Core Web API implementing basic CRUD operations for a `TodoItem` model, backed by SQLite via Entity Framework Core.

Quick start

Windows PowerShell:

```powershell
./packages/backend/build/build.ps1
./packages/backend/build/run.ps1
```

macOS / Linux:

```bash
./packages/backend/build/build.sh
./packages/backend/build/run.sh
```

API endpoints (after running):
- GET    /api/TodoItems
- GET    /api/TodoItems/{id}
- POST   /api/TodoItems
- PUT    /api/TodoItems/{id}
- DELETE /api/TodoItems/{id}

By default the SQLite file is `todo.db` in the project root. The app will create it automatically.

Frontend note: A React application will be created later to consume these endpoints.
