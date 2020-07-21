using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.ViewModels
{
    public class ErnmrVm
    {
        public int Ernmrid { get; set; }
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int? Biweekly { get; set; }
        public string Implementer { get; set; }
        public string FacilityType { get; set; }
        public int? Screens { get; set; }
        public string UserName { get; set; }
        public int TenantId { get; set; }
    }
}
