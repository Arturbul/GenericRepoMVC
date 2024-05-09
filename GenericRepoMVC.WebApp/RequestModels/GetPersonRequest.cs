namespace GenericRepoMVC.WebApp.RequestModels
{
    public class GetPersonRequest
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DateOfBirthFrom { get; set; }
        public DateOnly? DateOfBirthTo { get; set; }
    }
}
