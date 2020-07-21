using System.ComponentModel.DataAnnotations.Schema;

[Table("NMR_checkcompleteness")]
public partial class checkcompleteness
{
    [System.ComponentModel.DataAnnotations.Key]
    public string NMRID{get;set;}
    public string Province { get; set; }
    public string District { get; set; }
    public string Implementer { get; set; }
    public int FacilityID { get; set; }
    public string FacilityName { get; set; }
    public string message { get; set; }
    public int? StatusId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int IPDSAM_submission { get; set; }
    public int OPDSAM_submission { get; set; }
    public int OPDMAM_submission { get; set; }
    public int MNS_submission { get; set; }
    public int OPDMAM_stock_submission { get; set; }
    public int IPDSAM_stock_submission { get; set; }
    public int OPDSAM_stock_submission { get; set; }
    public string UserName { get; set; }
    public int Tenant {get;set;}
    public string MyId{get;set;}
}
