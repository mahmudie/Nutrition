namespace DataSystem.Models.ViewModels
{
    public class FeedbackVm
    {
        public int Id { get; set; }
        public string Nmrid { get; set; }
        public string Initiator { get; set; }
        public string Respondent { get; set; }

        public string Problem { get; set; }
        public string Respose { get; set; }
    }
}