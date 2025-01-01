using System.ComponentModel.DataAnnotations;

namespace TaskListApp.Models
{
    public class User
    {
        [Key] //Mark this as the primary key
        public int Id { get; set; } //Primary Key
        [Required] //Ensure a value is entered
        public string Name { get; set; }
        [Required] //Ensure a value is entered
        public string Role { get; set; }
        [Required] //Ensure a value is entered
        public string Email { get; set; }
    }
}
