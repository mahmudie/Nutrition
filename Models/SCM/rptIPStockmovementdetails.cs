using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.SCM
{
    public class rptIPStockmovementdetails
    {
        [Key]
        public int id { get; set; }
        public int rndId { get; set; }
        public int distributionId { get; set; }
        public string roundId { get; set; }
        public string implementer { get; set; }
        public string location { get; set; }
        public string item { get; set; }
        public string batchNumber { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public DateTime issueDate { get; set; }
        public int quantity { get; set; }
        public double dispatch { get; set; }
        public DateTime expiryDate { get; set; }
        public double loss { get; set; }
        public double damage { get; set; }
        public double expiration { get; set; }
        public double totalWaste { get; set; }
        public int tenantId { get; set; }
        public int whId { get; set; }
        public double balance
        {
            get
            {
                return quantity - dispatch;
            }
        }
    }
}
