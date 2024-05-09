namespace GenericRepoMVC.Domain.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Position { get; set; } = null!;
        public Person EmployeeDetails { get; set; } = null!;
    }
}
