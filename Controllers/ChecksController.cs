using System.Collections.Generic;
using System.Linq;
using DataSystem.Models;
using DataSystem.Models.ViewModels.chart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using DataTables.AspNet.AspNetCore;
using System;
using DataSystem.Data;
using System.Threading.Tasks;

namespace DataSystem.Controllers
{
    [Authorize]
    public class ChecksController:Controller
    {
        private readonly WebNutContext _db;
        private readonly ApplicationDbContext _app;
        public ChecksController(WebNutContext db, ApplicationDbContext App)
        {
           _db=db;
           _app=App;
        }

        public  IActionResult Index()
        {
            return View();
        }

        public IActionResult plist()
        {
            return View();
        }

        public IActionResult hlistm()
        {
            return View();
        }

        public IActionResult hlistmsection()
        {
            return View();
        }
        public IActionResult plistdata()
        {
            var data =_db.provincemonthly.FromSql("SELECT * FROM provincemonthly");

            return Ok(new {data=data});
        }
        public int tenant(string user_name)
        {
           var item =  _app.Users.Include(m => m.TenantId).SingleOrDefault(m => m.UserName == user_name);
            return item.TenantId;
        }
          public IActionResult reportstat()
          {
            // ProvId ="20-1396";
            string UserName;
            UserName=User.Identity.Name;
            var data = _db.monthlysubmission.FromSql("Select * from monthlysubmission").ToList();

            if(!data.Any())
                    {
                        return NotFound();
                    }

            if(User.IsInRole("administrator")|| UserName==null)
            {
                data=data.ToList();
                //data = _db.monthlysubmission.FromSql("Select * from monthlysubmission WHERE UserName= {0} and Tenant= {1}",UserName,tenant(UserName)).ToList();                   
            } 
             else if(User.IsInRole("administrator")&&tenant(UserName)!=1)
            {
                data = _db.monthlysubmission.FromSql("Select * from monthlysubmission WHERE UserName= {0} and Tenant= {1}",UserName,tenant(UserName)).ToList();                   
            }
            else if (User.IsInRole("dataentry"))
            {
                data = _db.monthlysubmission.FromSql("Select * from monthlysubmission WHERE UserName= {0}",UserName).ToList();                   
            }

             return Ok(new {data=data});
          }

 public IActionResult hmonthtrend(string ProvId)
          {
            string UserName;
            UserName=User.Identity.Name;
            var data = _db.monthlysubmission.FromSql("Select * from monthlysubmission WHERE ProvId = {0}",ProvId).ToList();

            if(!data.Any())
                    {
                        return NotFound();
                    }

             return Ok(new {data=data});
          }



            [Authorize(Roles = "dataentry,administrator")]
          public IActionResult completeness(string MyId)
          {
            bool role = false;

            if (User.IsInRole("dataentry"))
            {
                role = true;
            }
            var tenants = _app.Users.Where(m => m.UserName.Equals(User.Identity.Name)).SingleOrDefault();
            int tenantId = 0;
            if (tenants != null)
            {
                tenantId = tenants.TenantId;
            }

            string UserName;
            UserName=User.Identity.Name;
            var data = _db.checkcompleteness.Where(m =>m.MyId.Equals(MyId)).Select(s =>new{
                        NMRID=s.NMRID,
                        Province=s.Province,
                        District=s.District,
                        Implementer=s.Implementer,
                        FacilityID=s.FacilityID,
                        FacilityName=s.FacilityName,
                        message=s.message,
                        StatusId=s.StatusId,
                        Year =s.Year,
                        Month=s.Month,
                        IPDSAM_submission=s.IPDSAM_submission,
                        OPDSAM_submission=s.OPDSAM_submission,
                        OPDMAM_submission=s.OPDMAM_submission,
                        MNS_submission=s.MNS_submission,
                        OPDMAM_stock_submission=s.OPDMAM_stock_submission,
                        IPDSAM_stock_submission =s.IPDSAM_stock_submission,
                        OPDSAM_stock_submission=s.OPDSAM_stock_submission,
                        UserName=s.UserName,
                        Tenant=s.Tenant,
                        MyId=s.MyId,
                        role=role
            }).ToList();

            if (tenantId != 1 && role != true)
            {
                data = data.Where(m => m.Tenant == tenantId).ToList();
            }
            else if (role == true)
            {
                data = data.Where(m => m.UserName.Equals(UserName)).ToList();
            }

            if(!data.Any())
                    {
                        return NotFound();
                    }
             return Ok(new {data=data});
          }
    }
}