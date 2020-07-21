using System;
using System.ComponentModel.DataAnnotations;


namespace DataSystem.Models.ViewModels
{
    public class IycfDto
    {
        public int Iycfid { get; set; }
        public int? MChildU5months { get; set; }
        public int? MChild524months { get; set; }
        public int? Pregnanatwomen { get; set; }
        public int? Firstvisit { get; set; }
        public int? Revisit { get; set; }
        public int? ReferIn { get; set; }
        public int? ReferOut { get; set; }
        public string UserName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? UpdateDate { get; set; }
        public string CauseShortName { get; set; }

    }
}
