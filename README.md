# TaskLists API ✅

TaskLists API is a REST API for managing task collections and tasks, including sharing features.

## Prerequisites 🛠️

Before running the TaskLists API, make sure the following tools are installed on your machine:

* An IDE or code editor:
  * Visual Studio 2022 [![Visual Studio](https://img.shields.io/badge/Visual%20Studio-2022-blue?logo=visual-studio&logoColor=white)](https://visualstudio.microsoft.com/)
  * VS Code with the C# extension [![VS Code](https://img.shields.io/badge/VS%20Code-blue?logo=visual-studio-code&logoColor=white)](https://code.visualstudio.com/)
* .NET 9 SDK [![.NET](https://img.shields.io/badge/.NET-9.0-blue?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* PostgreSQL [![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17.6-blue?logo=postgresql&logoColor=white)](https://www.postgresql.org/download/)

## Setup

1. **Clone the repository** 📂

```bash
git clone https://github.com/vzaiats/TaskLists-API
cd TaskListsAPI
```

2. **Configure your PostgreSQL connection** ⚙️

Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=TaskListsDb;Username=<your-username>;Password=<your-password>;"
}
```

> Replace 👤 `<your-username>` and 🔑 `<your-password>` with your PostgreSQL credentials.

3. **Apply migrations** 🗄️

Open Package Manager Console in Visual Studio or a terminal and run:

```powershell
# Create initial migration
Add-Migration InitialCreate -Project TaskListsAPI.Infrastructure -StartupProject TaskListsAPI.Api

# Apply migration to database
Update-Database -Project TaskListsAPI.Infrastructure -StartupProject TaskListsAPI.Api
```

* `-Project` → the project containing your `DbContext` (`TaskListsAPI.Infrastructure`)
* `-StartupProject` → the project that runs the application (`TaskListsAPI.Api`)

## Running the API 🚀

Run the API using Visual Studio or with the CLI:

```bash
dotnet run --project TaskListsAPI.Api
```

🌐 The API can be explored in Swagger UI at the following URL:

```
https://localhost:44352/swagger/index.html
```
