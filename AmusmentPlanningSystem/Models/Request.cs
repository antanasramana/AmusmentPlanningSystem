namespace AmusmentPlanningSystem.Models
{
    public class Request
    {
        public string DocumentFile { get; set; }
        public DateTime ConfirmationDate  { get; set; }

        public RequestStatus Status { get; set; }
        public Administrator Administrator { get; set; }
        public Company Company { get; set; }

    }
}
