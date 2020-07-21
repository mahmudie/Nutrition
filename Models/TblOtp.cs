using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace DataSystem.Models
{
    [OtpAttribute]
    public partial class TblOtp
    {
        [Range(1, int.MaxValue, ErrorMessage = "Enter a valid number")]
        public int Otpid { get; set; }
        [Required]
        public string Nmrid { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Totalbegin { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Odema { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Z3score { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Muac115 { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Fromscotp { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Fromsfp { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Defaultreturn { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Cured { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? Death { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        [JsonProperty("defaulters")]
        public int? Default { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? RefOut { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? NonCured { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? TMale { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Invalid number")]
        public int? TFemale { get; set; }
        [StringLength(50)]
        public string UserName { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual Nmr Nmr { get; set; }
        public virtual TlkpOtptfu Otp { get; set; }


    }
}
