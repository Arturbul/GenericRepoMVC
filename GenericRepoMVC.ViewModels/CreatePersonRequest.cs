namespace GenericRepoMVC.ViewModels
{
    public class CreatePersonRequest
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
    }
}
