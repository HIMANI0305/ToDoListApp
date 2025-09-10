using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

delegate void TaskAction(string message);

class ToDoList
{
    private List<string> tasks = new List<string>();
    private string filePath = "tasks.txt";

    // Event for task added/removed
    public event TaskAction OnTaskChanged;

    public async Task LoadTasksAsync()
    {
        if (File.Exists(filePath))
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            string[] lines = await File.ReadAllLinesAsync(filePath);
#else
            string[] lines = File.ReadAllLines(filePath);
#endif
            tasks = new List<string>(lines);
        }
    }

    public async Task SaveTasksAsync()
    {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        await File.WriteAllLinesAsync(filePath, tasks);
#else
        await Task.Run(() => File.WriteAllLines(filePath, tasks));
#endif
    }

    public void AddTask(string task)
    {
        tasks.Add(task);
        OnTaskChanged?.Invoke($"Task Added: {task}");
    }

    public void ViewTasks()
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks yet!");
            return;
        }
        Console.WriteLine("\nYour Tasks:");
        for (int i = 0; i < tasks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {tasks[i]}");
        }
    }

    public void DeleteTask(int index)
    {
        if (index < 0 || index >= tasks.Count)
        {
            Console.WriteLine("Invalid task number!");
            return;
        }
        string removed = tasks[index];
        tasks.RemoveAt(index);
        OnTaskChanged?.Invoke($"Task Removed: {removed}");
    }
}

class Program
{
    static async Task Main()
    {
        ToDoList todo = new ToDoList();
        todo.OnTaskChanged += msg => Console.WriteLine($"[Event] {msg}");

        await todo.LoadTasksAsync();

        while (true)
        {
            Console.WriteLine("\n===== To-Do List App =====");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. View Tasks");
            Console.WriteLine("3. Delete Task");
            Console.WriteLine("4. Exit");
            Console.Write("Choose: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter task: ");
                    string task = Console.ReadLine();
                    todo.AddTask(task);
                    await todo.SaveTasksAsync();
                    break;
                case "2":
                    todo.ViewTasks();
                    break;
                case "3":
                    Console.Write("Enter task number to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int num))
                        todo.DeleteTask(num - 1);
                    await todo.SaveTasksAsync();
                    break;
                case "4":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }
    }
}
