using DataSystem.Models.GLM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.GLM
{
    public class FieldViewModel
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public long QuestionId { get; set; }

        [Required]
        public long ColumnId { get; set; }

        [Required]
        [Display(Name = "Data Type")]
        public string DataType { get; set; }

        [Display(Name = "Input Type")]
        public string InputType { get; set; }

        [Required]
        [Display(Name = "Is Required")]
        public bool IsRequired { get; set; } = true;

        [Display(Name = "Yes/No Default Caption (Optional)")]
        public string YesNoDefaultCaption { get; set; } = null;

        [Required]
        [Display(Name = "Is Expiry Date")]
        public bool IsExpiryDate { get; set; } = false;

        [Display(Name = "Expiry Warning Period")]
        public int? ExpiryWarningPeriod { get; set; }

        public IEnumerable<FieldOption> FieldOptions { get; set; }

        public Dictionary<string, string> DataTypes
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "number", "Number" },
                    { "text", "Text" },
                    { "date", "Date" },
                    { "yesno", "Yes/No" }
                };
            }
        }

        public Dictionary<string, string> InputTypes
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "textbox", "Text Box" },
                    { "textarea", "Textarea" },
                    { "dropdown", "Dropdown" },
                    { "percentage", "Percentage" }
                };
            }
        }

        public Dictionary<bool, string> BooleanOptions
        {
            get
            {
                return new Dictionary<bool, string>
                {
                    { true, "Yes" },
                    { false, "No" }
                };
            }
        }

    }
}
