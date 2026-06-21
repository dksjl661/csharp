# C# Todo Workspace

A .NET 8 CQRS Todo API with SQL Server and a Next.js Todo workspace.

## Start SQL Server

Docker Desktop must be running. Create a local password file, then start the database:

```bash
cp .env.example .env
# Edit .env and replace SQL_SA_PASSWORD with a strong password.
docker compose --env-file .env up -d
```

## Run the API

The application automatically applies EF Core migrations and adds mock Todo data on startup. Load the SQL Server password and connection string into your shell first:

```bash
set -a
source .env
set +a
export ConnectionStrings__TodoDatabase="Server=localhost,1433;Database=CsharpTodo;User Id=sa;Password=${SQL_SA_PASSWORD};TrustServerCertificate=True;Encrypt=False"
/opt/homebrew/opt/dotnet@8/bin/dotnet run --project CsharpTodo.Api --launch-profile http
```

Open <http://localhost:5261/swagger> to test the API. The seeded data is available at:

```bash
curl http://localhost:5261/api/todos
```

The API allows the local Next.js app at `http://localhost:3000` through its `Frontend` CORS policy.

## Run the frontend

In a second terminal:

```bash
cd todo-web
cp .env.example .env.local
npm run dev
```

Open <http://localhost:3000>. The workspace fetches Todo and Label data from the API, lets you create Todo items, toggle completion, and remove items.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/todos` | List Todo items |
| GET | `/api/todos/{id}` | Get one Todo item |
| POST | `/api/todos` | Create a Todo item |
| PUT | `/api/todos/{id}` | Update a Todo item |
| DELETE | `/api/todos/{id}` | Delete a Todo item |
| GET | `/api/labels` | List the seeded labels |

POST and PUT accept this JSON body:

```json
{
  "title": "Write documentation",
  "description": "Explain how to run the Todo API",
  "isCompleted": false,
  "labelId": 1
}
```

`labelId` is optional. Todo responses include the assigned Label (`name`, `description`, and `color`), or `null` when no label is assigned.

## Development commands

```bash
/opt/homebrew/opt/dotnet@8/bin/dotnet test CsharpTodo.Tests/CsharpTodo.Tests.csproj
/opt/homebrew/opt/dotnet@8/bin/dotnet tool run dotnet-ef migrations add MigrationName --project CsharpTodo.Api --startup-project CsharpTodo.Api
/opt/homebrew/opt/dotnet@8/bin/dotnet tool run dotnet-ef database update --project CsharpTodo.Api --startup-project CsharpTodo.Api
npm --prefix todo-web run lint
npm --prefix todo-web run build
```
