using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.ViewModels
{
    public class scmAttachment
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "File Attachment")]
        public IFormFile Attachment { get; set; }

        public string AttachmentName { get; set; }
    }
}
