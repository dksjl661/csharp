# C# Todo Web API

A minimal .NET 10 Todo API using EF Core and SQL Server.

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
dotnet run --project CsharpTodo.Api
```

Open <http://localhost:5261/swagger> to test the API. The seeded data is available at:

```bash
curl http://localhost:5261/api/todos
```

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/todos` | List Todo items |
| GET | `/api/todos/{id}` | Get one Todo item |
| POST | `/api/todos` | Create a Todo item |
| PUT | `/api/todos/{id}` | Update a Todo item |
| DELETE | `/api/todos/{id}` | Delete a Todo item |

POST and PUT accept this JSON body:

```json
{
  "title": "Write documentation",
  "description": "Explain how to run the Todo API",
  "isCompleted": false
}
```

## Development commands

```bash
dotnet test
dotnet tool run dotnet-ef migrations add MigrationName --project CsharpTodo.Api --startup-project CsharpTodo.Api
dotnet tool run dotnet-ef database update --project CsharpTodo.Api --startup-project CsharpTodo.Api
```
