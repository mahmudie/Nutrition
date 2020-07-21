using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.HP
{
    public class HpMonitoring
    {
        [Key]
        public int HpmId { get; set; }
        [Required]
        public DateTime DateOfMonitoring { get; set; }
        [Required]
        public string DataCollectorName { get; set; }
        [Required]
        public string RespondentName { get; set; }
        public string RespondentContactNo { get; set; }
        [Required]
        public string ProvinceId { get; set; }
        [Required]
        public string DistrictId { get; set; }
        [Required]
        public int FacilityId { get; set; }
        public int FacilityTypeId { get; set; }
        [Required]
        public string HPName { get; set; }
        public string HPCode { get; set; }
        [Required]
        public int ImpId { get; set; }
        public int OtherImpId { get; set; }
        public string UserName { get; set; }
        public int TenantId { get; set; }
        public DateTime UpdateDate { get; set; }
    }

    public class vHpMonitoring
    {
        [Key]
        public int HpmId { get; set; }
        public DateTime DateOfMonitoring { get; set; }
        public string DataCollectorName { get; set; }
        public string RespondentName { get; set; }
        public string RespondentContactNo { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string HPName { get; set; }
        public string HPCode { get; set; }
        public string Implementer { get; set; }
        public string UserName { get; set; }
        public int TenantId { get; set; }
    }
}
