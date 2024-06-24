namespace GenericRepoMVC.ViewModels
{
    public class PersonOrderByRequest
    {
        public int OrderById { get; set; }
        public int OrderByFirstName { get; set; }
        public int OrderByLastName { get; set; }
        public int OrderByDateOfBirth { get; set; }
    }
}
