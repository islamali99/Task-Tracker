# TaskTraker CLI

A minimal command-line Task Tracker that stores tasks in a JSON file in the current working directory.

Features
- Add, update, delete tasks
- Mark tasks as in-progress or done
- List tasks (all / todo / in-progress / done)
- Tasks persisted to `tasks.json`

Usage (dotnet run during development)

```bash
# Add a task
dotnet run --project /Users/mac/TaskTraker/TaskTraker.csproj -- add "Buy groceries"

# Update a task
dotnet run --project /Users/mac/TaskTraker/TaskTraker.csproj -- update 1 "Buy groceries and cook dinner"

# Delete a task
dotnet run --project /Users/mac/TaskTraker/TaskTraker.csproj -- delete 1

# Mark as in-progress or done
dotnet run --project /Users/mac/TaskTraker/TaskTraker.csproj -- mark-in-progress 1
dotnet run --project /Users/mac/TaskTraker/TaskTraker.csproj -- mark-done 1

# List tasks
dotnet run --project /Users/mac/TaskTraker/TaskTraker.csproj -- list
dotnet run --project /Users/mac/TaskTraker/TaskTraker.csproj -- list done
dotnet run --project /Users/mac/TaskTraker/TaskTraker.csproj -- list todo
dotnet run --project /Users/mac/TaskTraker/TaskTraker.csproj -- list in-progress
```

Notes
- The `tasks.json` file is created in the directory where you run the command.
- Timestamps are saved in UTC ISO-8601 format.
- No external libraries are used; the implementation uses `System.Text.Json` and the native file system APIs.

Development
- Build: `dotnet build`
- Run: see examples above

License: MIT
# Task-Tracker
# Task-Tracker
