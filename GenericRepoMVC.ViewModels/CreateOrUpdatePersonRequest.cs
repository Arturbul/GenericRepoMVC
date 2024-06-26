namespace GenericRepoMVC.ViewModels
{
    public class CreateOrUpdatePersonRequest
    {
        public int? Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
    }
}
