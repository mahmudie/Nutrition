
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public partial class nmrsubmission
{
    [System.ComponentModel.DataAnnotations.Key]
    public string NMRID{get;set;}
    public int IPDSAM_submission { get; set; }
    public int OPDSAM_submission { get; set; }
    public int OPDMAM_submission { get; set; }
    public int MNS_submission { get; set; }
    public int OPDMAM_stock_submission { get; set; }
    public int IPDSAM_stock_submission { get; set; }
    public int OPDSAM_stock_submission { get; set; }


    public string District { get; set; }
    public string Implementer { get; set; }
    public int FacilityID { get; set; }
    public string FacilityName { get; set; }
    public string FacilityType { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int mYear { get; set; }
    public int mMonth { get; set; }
    public string Province { get; set; }
    public string ProvCode { get; set; }


}


[Table("pvtSubmission")]
public partial class reportsubmission
{
    [Key]
    public string NMRID { get; set; }
    public int FacilityID { get; set; }
    public string FacilityName { get; set; }
    public string TypeAbbrv { get; set; }
    public string DistCode { get; set; }
    public string District { get; set; }
    public string ProvCode { get; set; }
    public string Province { get; set; }
    public string Implementer { get; set; }
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
}
