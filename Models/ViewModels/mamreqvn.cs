
using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DataSystem.Models.ViewModels
{
    public class MamreqVm
    {
        public string FacType { get; set; }
        public int? Muac12 { get; set; }

        public int? Muac23 { get; set; }

        public int? Zscore23 { get; set; }

        public int? refIn { get; set; }


        public string AgeGroup { get; set; }




    }
}
