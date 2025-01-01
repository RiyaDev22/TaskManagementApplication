using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskListApp.Models
{
    public class Task
    {
        [Key] //Mark this as the primary key
        public int Id { get; set; } //Primary Key
        [Required] //Ensure a value is entered
        public string Title { get; set; }
        [Required] //Ensure a value is entered
        public string Description { get; set; }
        
        [ForeignKey("Assignee")]
        public int AssigneeId { get; set; } //Foreign Key which links to the Users Table
        [ValidateNever] //Exclude from validation
        public User Assignee { get; set; } //Navigation property for Assignee

        [ForeignKey("Reviewer")]
        public int ReviewerId { get; set; } //Foreign Key which links to the Users Table
        [ValidateNever] //Exclude from validation
        public User Reviewer { get; set; } //Navigation property for Reviewer

        [Required] //Ensure a value is entered
        public DateTime DueDate { get; set; }
        [Required] //Ensure a value is entered
        public string Status { get; set; } //New, In Progress, Completed
        [Required] //Ensure a value is entered
        public string TaskType { get; set; } //Milestone, Compliance, Task
    }
}
