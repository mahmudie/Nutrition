using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class scmDocs
    {
        [Key]
        public int id { get; set; }
        public int distributionId { get; set; }
        public string documentName { get; set; }
        public string message { get; set; }
        public DateTime? dateSent { get; set; }
        public DateTime? updateDate { get; set; }
        public string userName { get; set; }
    }

    public class scmDoctypes
    {
        [Key]
        public int DocId { get; set; }
        public string DocumentType { get; set; }
    }

    //public class File
    //{
    //    public string name { get; set; }
    //    public long size { get; set; }
    //    public string type { get; set; }

    //    public string onlinePath { get; set; }
    //}
}
