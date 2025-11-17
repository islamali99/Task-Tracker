using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

// Simple TaskTraker CLI
// Commands: add, update, delete, mark-in-progress, mark-done, list

const string TasksFileName = "tasks.json";

if (args.Length == 0)
{
    PrintUsage();
    return;
}

var command = args[0].ToLowerInvariant();

try
{
    switch (command)
    {
        case "add":
            AddCommand(args);
            break;
        case "update":
            UpdateCommand(args);
            break;
        case "delete":
            DeleteCommand(args);
            break;
        case "mark-in-progress":
            MarkCommand(args, "in-progress");
            break;
        case "mark-done":
            MarkCommand(args, "done");
            break;
        case "list":
            ListCommand(args);
            break;
        default:
            Console.WriteLine($"Unknown command: {command}");
            PrintUsage();
            break;
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
}

static void PrintUsage()
{
    Console.WriteLine("Usage:\n");
    Console.WriteLine("task-cli add \"description\"");
    Console.WriteLine("task-cli update <id> \"new description\"");
    Console.WriteLine("task-cli delete <id>");
    Console.WriteLine("task-cli mark-in-progress <id>");
    Console.WriteLine("task-cli mark-done <id>");
    Console.WriteLine("task-cli list [all|todo|in-progress|done]");
}

static void AddCommand(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Missing description. Usage: task-cli add \"description\"");
        return;
    }

    var description = string.Join(' ', args.Skip(1));
    var tasks = LoadTasks();
    var nextId = tasks.Count == 0 ? 1 : tasks.Max(t => t.Id) + 1;
    var now = DateTime.UtcNow;
    var task = new TaskItem
    {
        Id = nextId,
        Description = description,
        Status = "todo",
        CreatedAt = now,
        UpdatedAt = now
    };
    tasks.Add(task);
    SaveTasks(tasks);
    Console.WriteLine($"Task added successfully (ID: {task.Id})");
}

static void UpdateCommand(string[] args)
{
    if (args.Length < 3)
    {
        Console.WriteLine("Usage: task-cli update <id> \"new description\"");
        return;
    }

    if (!int.TryParse(args[1], out var id))
    {
        Console.WriteLine("Invalid id.");
        return;
    }

    var newDescription = string.Join(' ', args.Skip(2));
    var tasks = LoadTasks();
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task == null)
    {
        Console.WriteLine($"Task with ID {id} not found.");
        return;
    }

    task.Description = newDescription;
    task.UpdatedAt = DateTime.UtcNow;
    SaveTasks(tasks);
    Console.WriteLine($"Task {id} updated.");
}

static void DeleteCommand(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: task-cli delete <id>");
        return;
    }

    if (!int.TryParse(args[1], out var id))
    {
        Console.WriteLine("Invalid id.");
        return;
    }

    var tasks = LoadTasks();
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task == null)
    {
        Console.WriteLine($"Task with ID {id} not found.");
        return;
    }

    tasks.Remove(task);
    SaveTasks(tasks);
    Console.WriteLine($"Task {id} deleted.");
}

static void MarkCommand(string[] args, string newStatus)
{
    if (args.Length < 2)
    {
        Console.WriteLine($"Usage: task-cli mark-{(newStatus == "done" ? "done" : "in-progress")} <id>");
        return;
    }

    if (!int.TryParse(args[1], out var id))
    {
        Console.WriteLine("Invalid id.");
        return;
    }

    var tasks = LoadTasks();
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task == null)
    {
        Console.WriteLine($"Task with ID {id} not found.");
        return;
    }

    task.Status = newStatus;
    task.UpdatedAt = DateTime.UtcNow;
    SaveTasks(tasks);
    Console.WriteLine($"Task {id} marked as {newStatus}.");
}

static void ListCommand(string[] args)
{
    var tasks = LoadTasks();
    if (args.Length < 2 || args[1].ToLowerInvariant() == "all")
    {
        PrintTasks(tasks);
        return;
    }

    var filter = args[1].ToLowerInvariant();
    if (filter == "todo" || filter == "done" || filter == "in-progress")
    {
        var filtered = tasks.Where(t => t.Status == filter).ToList();
        PrintTasks(filtered);
        return;
    }

    Console.WriteLine("Unknown list option. Use: all, todo, in-progress, done");
}

static List<TaskItem> LoadTasks()
{
    try
    {
        if (!File.Exists(TasksFileName))
        {
            // create empty file
            SaveTasks(new List<TaskItem>());
        }

        var json = File.ReadAllText(TasksFileName);
        if (string.IsNullOrWhiteSpace(json)) return new List<TaskItem>();

        var opts = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var items = JsonSerializer.Deserialize<List<TaskItem>>(json, opts);
        return items ?? new List<TaskItem>();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Failed to read tasks file: {ex.Message}");
        return new List<TaskItem>();
    }
}

static void SaveTasks(List<TaskItem> tasks)
{
    var opts = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    var json = JsonSerializer.Serialize(tasks, opts);
    File.WriteAllText(TasksFileName, json);
}

static void PrintTasks(IEnumerable<TaskItem> tasks)
{
    var list = tasks.ToList();
    if (!list.Any())
    {
        Console.WriteLine("No tasks found.");
        return;
    }

    foreach (var t in list)
    {
        Console.WriteLine($"ID: {t.Id}");
        Console.WriteLine($"Description: {t.Description}");
        Console.WriteLine($"Status: {t.Status}");
        Console.WriteLine($"CreatedAt: {t.CreatedAt:o}");
        Console.WriteLine($"UpdatedAt: {t.UpdatedAt:o}");
        Console.WriteLine(new string('-', 20));
    }
}

class TaskItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = "todo";

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}


