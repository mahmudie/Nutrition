namespace DataSystem.Models
{
[System.ComponentModel.DataAnnotations.Schema.Table("monthlysubmission")]
public partial class monthlysubmission
{
    [System.ComponentModel.DataAnnotations.Key]
    public string ID { get; set; }
    public string Province { get; set; }
    public string District { get; set; }
    public int FacilityID { get; set; }
    public string FacilityName { get; set; }
    public string UserName { get; set; }
    public int Year { get; set; }
    public int M1 { get; set; }
    public int M2 { get; set; }
    public int M3 { get; set; }
    public int M4 { get; set; }
    public int M5 { get; set; }
    public int M6 { get; set; }
    public int M7 { get; set; }
    public int M8 { get; set; }
    public int M9 { get; set; }
    public int M10 { get; set; }
    public int M11 { get; set; }
    public int M12 { get; set; }
    public int Tenant{get;set;}
    public string ProvId{get;set;}
}
}