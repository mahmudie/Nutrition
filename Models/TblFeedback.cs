namespace DataSystem.Models
{
    public partial class TblFeedback
    {
        public int FedId { get; set; }
        public int? FormId { get; set; }
        public string Nmrid { get; set; }
        public string Comments { get; set; }

        public virtual TlkpForms Form { get; set; }
        public virtual Nmr Nmr { get; set; }
    }
}
