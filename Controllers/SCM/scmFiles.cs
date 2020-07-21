using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Controllers.SCM
{
    public class scmFiles
    {
        [Key]
        public int id { get; set; }
        public string documentName { get; set; }
        public int impId { get; set; }
        public int provinceId { get; set; }
        public int roundId { get; set; }
        public double? fileSize { get; set; }
        public string fileName { get; set; }
        public string fileType { get; set; }
        public string filePath { get; set; }
        public string userName { get; set; }
        public int? tenantId { get; set; }
        public DateTime? updateDate { get; set; }
    }
}
