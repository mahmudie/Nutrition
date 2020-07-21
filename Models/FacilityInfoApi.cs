using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    public class FacilityInfoApi
    {
        [Key]   
        public Int32 FacilityId
        {
            get;
            set;
        }

        public String DistrictCode
        {
            get;
            set;
        }

        public String FacilityName
        {
            get;
            set;
        }

        public String FacilityNameDari
        {
            get;
            set;
        }

        public String FacilityNamePashto
        {
            get;
            set;
        }

        public String Location
        {
            get;
            set;
        }

        public String LocationDari
        {
            get;
            set;
        }

        public String LocationPashto
        {
            get;
            set;
        }


        public double? Latitude
        {
            get;
            set;
        }

        public double? Longitude
        {
            get;
            set;
        }

        public DateTime? DateEstablished
        {
            get;
            set;
        }

        public String Implementer
        {
            get;
            set;
        }

        public Int32? FacilityTypeId
        {
            get;
            set;
        }

        public String IsActive
        {
            get;
            set;
        }
    }
}
