using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSystem.Models
{
    [Table("SurveyInfo")]
    public partial class SurveyInfo
    {
        public SurveyInfo()
        {
            SurveyResults = new HashSet<SurveyResults>();
        }
        [Key]
        [Display(Name ="Survey Id")]
        public Int32 SurveyId { get; set; }
        [Display(Name = "Survey Acronym")]
        [Required(ErrorMessage="Survey Abreviation is required.")]
        public string SurveyAccro { get; set; }
        [Display(Name = "Survey Full Name")]
        [Required(ErrorMessage = "Survey Full Name is required.")]
        public string SurveyFull { get; set; }
        [Required(ErrorMessage = "Leady is required.")]
        [Display(Name = "Lead By")]
        public string LeadBy { get; set; }
        [Display(Name = "Implementer")]
        public string ImpBy { get; set; }
        [Display(Name = "Survey Year")]
        public int SurveyYear { get; set; }        
        [Display(Name = "Month")]
        public int? Month { get; set; }
        [Display(Name = "Survey Abstract")]
        public string Abstract { get; set; }
        [Display(Name = "User Name")]
        public String UserName { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name ="Date Last Updated")]
        public DateTime? UpdateDate { get; set; }
        [Display(Name = "Writer Names")]
        public String Writers { get; set; }
        public int? TenantId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Attachment { get; set; }


        public virtual ICollection<SurveyResults> SurveyResults { get; set; }
    }

    public class SurveyInfovm
    {
        [Display(Name = "Survey Id")]
        public Int32 SurveyId { get; set; }
        [Display(Name = "Survey Acronym")]
        [Required(ErrorMessage = "Survey Abreviation is required.")]
        public string SurveyAccro { get; set; }
        [Display(Name = "Survey Full Name")]
        [Required(ErrorMessage = "Survey Full Name is required.")]
        public string SurveyFull { get; set; }
        [Required(ErrorMessage = "Leady is required.")]
        [Display(Name = "Lead By")]
        public string LeadBy { get; set; }
        [Required()]
        [Display(Name = "Implementer")]
        public string ImpBy { get; set; }
        [Required()]
        [Display(Name = "Survey Year")]
        public int SurveyYear { get; set; }
        [Display(Name = "Month")]
        public int? Month { get; set; }
        [Display(Name = "Survey Abstract")]
        public string Abstract { get; set; }
        [Display(Name = "User Name")]
        public String UserName { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Date Last Updated")]
        public DateTime? UpdateDate { get; set; }
        [Display(Name = "Writer Names")]
        public String Writers { get; set; }
        //[Required(ErrorMessage = "No file selected.")]
        //[RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.pdf)$", ErrorMessage = "Only PDF files are allowed.")]
        public IFormFile Attachment { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool DeleteAttachement { get; set; }
    }

    public class SurveyAttachmentViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Attachment")]
        public IFormFile Attachment { get; set; }
    }

    public class IdDto
    {
        public int Id { get; set; }
    }
}
