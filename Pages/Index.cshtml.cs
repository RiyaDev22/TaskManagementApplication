using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskListApp.Models;
using Task = TaskListApp.Models.Task; //Ensure the correct namespace for TaskDbContext

public class IndexModel : PageModel
{
    private readonly TaskDbContext _context;

    public IndexModel(TaskDbContext context)
    {
        _context = context; //Assign the injected context
    }

    /*Fetch all of the tasks from the database*/
    public IActionResult OnGetTasks()
    {
        var tasks = _context.Tasks
            .Include(t => t.Assignee) //Include Assignee navigation property
            .Include(t => t.Reviewer) //Include Reviewer navigation property
            .Select(t => new
            {
                t.Id,
                t.TaskType,
                t.Title,
                t.Description,
                AssigneeId = t.AssigneeId,
                ReviewerId = t.ReviewerId,
                AssigneeName = t.Assignee.Name, //Map Assignee name
                ReviewerName = t.Reviewer.Name, //Map Reviewer name
                DueDate = t.DueDate.ToString("dd-MMM-yyyy"), //Format the date
                t.Status,
                Actions = $"<button class='btn btn-sm btn-primary edit-task' data-task-id='{t.Id}'>Edit</button>" +
                $"<button class='btn btn-sm btn-danger delete-task' data-task-id='{t.Id}'>Delete</button>"
            })
            .ToList();

        return new JsonResult(new { data = tasks });
    }

    /*Fetch a specific task from the database using task id*/
    public IActionResult OnGetTaskById(int id)
    {
        var task = _context.Tasks
            .Include(t => t.Assignee)
            .Include(t => t.Reviewer)
            .Where(t => t.Id == id)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                DueDate = t.DueDate.ToString("yyyy-MM-dd"), //Correct format for <input type="date">
                t.AssigneeId,
                t.ReviewerId,
                t.Status,
                t.TaskType
            })
            .FirstOrDefault();

        if (task == null)
        {
            return new JsonResult(new { success = false, message = "Task not found." });
        }

        return new JsonResult(new { success = true, data = task });
    }

    /*Fetch Assignees from the database*/
    public IActionResult OnGetAssignees()
    {
        var assignees = _context.Users
            .Where(u => u.Role == "Assignee")
            .Select(u => new { u.Id, u.Name, u.Email })
            .ToList();

        return new JsonResult(assignees);
    }
        
    /*Fetch Reviewers from the database*/
    public IActionResult OnGetReviewers()
    {
        var reviewers = _context.Users
            .Where(u => u.Role == "Reviewer")
            .Select(u => new { u.Id, u.Name, u.Email })
            .ToList();

        return new JsonResult(reviewers);
    }

    /*Update the database when a new task is created*/
    public IActionResult OnPostCreateTask([FromBody] Task newTask)
    {
        //Check if the newTask object is null
        if (newTask == null)
        {
            Console.WriteLine("The newTask object is null.");
            return new JsonResult(new { success = false, message = "The task data is null." });
        }

        //Parse and validate the DueDate explicitly
        if (!DateTime.TryParse(newTask.DueDate.ToString(), out var parsedDueDate))
        {
            Console.WriteLine("Invalid DueDate format.");
            return new JsonResult(new { success = false, message = "Invalid DueDate format." });
        }

        //Replace the unparsed DueDate with the parsed value
        newTask.DueDate = parsedDueDate;

        //Validate the entire model
        if (!ModelState.IsValid)
        {
            Console.WriteLine("ModelState is invalid.");
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                }
            }
            return new JsonResult(new { success = false, message = "Validation failed." });
        }

        try
        {
            //Add the task to the database
            _context.Tasks.Add(newTask);
            _context.SaveChanges();

            return new JsonResult(new { success = true, message = "Task added successfully." });
        }
        catch (Exception ex)
        {
            //Log any exceptions
            Console.WriteLine($"Error adding task: {ex.Message}");
            return new JsonResult(new { success = false, message = "Failed to add task." });
        }
    }

    /*Update the database when a task is updated*/
    public IActionResult OnPostUpdateTask([FromBody] Task updatedTask)
    {
        if (updatedTask == null)
        {
            return new JsonResult(new { success = false, message = "Task data is null." });
        }

        try
        {
            //Retrieve the existing task from the database
            var existingTask = _context.Tasks.FirstOrDefault(t => t.Id == updatedTask.Id);
            if (existingTask == null)
            {
                return new JsonResult(new { success = false, message = "Task not found." });
            }

            //Update the task fields
            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.DueDate = updatedTask.DueDate;
            existingTask.AssigneeId = updatedTask.AssigneeId;
            existingTask.ReviewerId = updatedTask.ReviewerId;
            existingTask.Status = updatedTask.Status;
            existingTask.TaskType = updatedTask.TaskType;

            //Save changes to the database
            _context.SaveChanges();

            return new JsonResult(new { success = true, message = "Task updated successfully." });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating task: {ex.Message}");
            return new JsonResult(new { success = false, message = "Failed to update task." });
        }
    }

    /*Update the database when a task is deleted*/
    public IActionResult OnPostDeleteTask([FromBody] Task deleteTask)
    {
        if (deleteTask == null || deleteTask.Id == 0)
        {
            return new JsonResult(new { success = false, message = "Invalid task data." });
        }

        try
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == deleteTask.Id);
            if (task == null)
            {
                return new JsonResult(new { success = false, message = "Task not found." });
            }

            _context.Tasks.Remove(task);
            _context.SaveChanges();

            return new JsonResult(new { success = true, message = "Task deleted successfully." });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting task: {ex.Message}");
            return new JsonResult(new { success = false, message = "Failed to delete task." });
        }
    }
}
