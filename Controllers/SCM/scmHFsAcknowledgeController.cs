using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.SCM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;

namespace DataSystem.Controllers.SCM
{
    public class scmHFsAcknowledgeController : Controller
    {
        private readonly WebNutContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public scmHFsAcknowledgeController(WebNutContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmHFsAcknowledgement.ToList();
            IEnumerable DataSource = data;
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<scmHFsAcknowledgement>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }

        public async Task<IActionResult> BatchUpdate([FromBody]CRUDModel value)
        {
            var Crrentuser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (value.Changed != null)
            {
                for (var i = 0; i < value.Changed.Count(); i++)
                {
                    try
                    {
                        var req = value.Changed[i];
                        scmHFsAcknowledgement hfreqs = _context.scmHFsAcknowledgement.Where(or => or.id == req.id && or.userName == Crrentuser.UserName).FirstOrDefault();
                        hfreqs.distributionId = req.distributionId;
                        hfreqs.facilityId = req.facilityId;
                        hfreqs.acknowledgeBy = req.acknowledgeBy;
                        hfreqs.dateOfAcknoledge = DateTime.Now.Date;
                        hfreqs.acknowledge = req.acknowledge;
                        hfreqs.message = req.message;
                        hfreqs.waybillNumber = req.waybillNumber;
                        hfreqs.updateDate = DateTime.Now.Date;
                        hfreqs.userName = Crrentuser.UserName;
                        _context.Update(hfreqs);
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
            }
            if (value.Deleted != null)
            {
                for (var i = 0; i < value.Deleted.Count(); i++)
                {
                    _context.scmHFsAcknowledgement.Remove(_context.scmHFsAcknowledgement.Where(or => or.id == value.Deleted[i].id).FirstOrDefault());
                    _context.SaveChanges();
                }
            }
            if (value.Added != null)
            {
                for (var i = 0; i < value.Added.Count(); i++)
                {
                    _context.scmHFsAcknowledgement.Add(value.Added[i]);
                    _context.SaveChanges();
                }
            }
            var data = _context.scmHFsAcknowledgement.ToList();
            return Json(data);
        }

        public class CRUDModel
        {
            public List<scmHFsAcknowledgement> Added { get; set; }
            public List<scmHFsAcknowledgement> Changed { get; set; }
            public List<scmHFsAcknowledgement> Deleted { get; set; }
            public scmHFsAcknowledgement Value { get; set; }
            public int key { get; set; }
            public string action { get; set; }
        }
    }
}