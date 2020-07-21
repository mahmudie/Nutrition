using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.Checklist_subs
{
    public class ChkCGM
    {      
        [Key]
        public int ChkId { get; set; }
        public int IndId { get; set; }
        public int Response { get; set; }
        public int NResponse { get; set; }
        public int Tenant { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Update")]
        public DateTime? UpdateDate { get; set; }

        public virtual Checklist GetChecklist { get; set; }
        public virtual lkpChecklist GetLkpChecklist { get; set; }
    }
    public class ChkGM
    {
        [Key]
        public int ChkId { get; set; }
        public int IndId { get; set; }
        public int Response { get; set; }
        public int NResponse { get; set; }
        public string UserName { get; set; }
        public int Tenant { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Update")]
        public DateTime? UpdateDate { get; set; }

        public virtual Checklist GetChecklist { get; set; }
        public virtual lkpChecklist GetLkpChecklist { get; set; }
    }

    public class ChkIYCFHF
    {
        [Key]
        public int ChkId { get; set; }
        public int IndId { get; set; }
        public int Response { get; set; }
        public int NResponse { get; set; }
        public string UserName { get; set; }
        public int Tenant { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Update")]
        public DateTime? UpdateDate { get; set; }

        public virtual Checklist GetChecklist { get; set; }
        public virtual lkpChecklist GetLkpChecklist { get; set; }
    }

    public class ChkMN
    {
        [Key]
        public int ChkId { get; set; }
        public int IndId { get; set; }
        public int Response { get; set; }
        public int NResponse { get; set; }
        public int Tenant { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Update")]
        public DateTime? UpdateDate { get; set; }

        public virtual Checklist GetChecklist { get; set; }
        public virtual lkpChecklist GetLkpChecklist { get; set; }
    }

    public class ChkOPT
    {
        [Key]
        public int ChkId { get; set; }
        public int IndId { get; set; }
        public int Response { get; set; }
        public int NResponse { get; set; }
        public int Tenant { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Update")]
        public DateTime? UpdateDate { get; set; }

        public virtual Checklist GetChecklist { get; set; }
        public virtual lkpChecklist GetLkpChecklist { get; set; }
    }
    
    public class ChkSAMHF
    {
        [Key]
        public int ChkId { get; set; }
        public int IndId { get; set; }
        public int Response { get; set; }
        public int NResponse { get; set; }
        public int Tenant { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Update")]
        public DateTime? UpdateDate { get; set; }

        public virtual Checklist GetChecklist { get; set; }
        public virtual lkpChecklist GetLkpChecklist { get; set; }
    }
        
    public class ChkSFP
    {
        [Key]
        public int ChkId { get; set; }
        public int IndId { get; set; }
        public int Response { get; set; }
        public int NResponse { get; set; }
        public int Tenant { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Last Update")]
        public DateTime? UpdateDate { get; set; }

        public virtual Checklist GetChecklist { get; set; }
        public virtual lkpChecklist GetLkpChecklist { get; set; }
    }

}
