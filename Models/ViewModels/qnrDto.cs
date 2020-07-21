using System;
namespace DataSystem.Models.ViewModels
{
    public partial class qnrDto
    {

     public int Qnrid{get;set;}
     public string Province {get;set;}
     public String Implementer{get;set;}
     public int? ReportMonth{get;set;}
     public int? ReportYear{get;set;}
     public DateTime? ReportingDate{get;set;}
     public DateTime? Updated{get;set;}
     public string Message{get;set;}
     public int? status{get;set;}


    }
}
