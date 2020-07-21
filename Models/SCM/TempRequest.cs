using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class TempRequest
    {
        [Key]
    public int Id { get; set; }
    public int RequestId { get; set; }
    public int FacilityId { get; set; }
    public int FacilityTypeId { get; set; }
    public int SupplyId { get; set; }
    public int Children  { get; set; }
    public double Buffer  { get; set; }
    public double Factor  { get; set; }
    public int CurrentBalance  { get; set; }
    public int Adjustment  { get; set; }
    public string AdjComment { get; set; }
    public int TenantId { get; set; }
    public string UserName { get; set; }
    public DateTime UpdateDate  { get; set; }
    public int Year  { get; set; }
    public int Month { get; set; }
     public int StockForChildren { get; set; }
     public string Program { get; set; }
    }
}
