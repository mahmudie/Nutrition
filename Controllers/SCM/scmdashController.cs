using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataSystem.Models;
using DataSystem.Models.SCM;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;

namespace DataSystem.Controllers.SCM
{
    public class scmdashController : Controller
    {
        private readonly WebNutContext _context;

        public scmdashController(WebNutContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var status_height = _context.vscmRequeststatus.Count()<10 ? _context.vscmRequeststatus.Count()*28+130:300;
            var dist_height = _context.scmdashdistmain.Count() < 10 ? _context.scmdashdistmain.Count() * 28+ 130 : 300;
            var notificaiton_height = _context.scmdashsubmission.Count() < 10 ? _context.scmdashsubmission.Count() * 28+ 80 : 300;
            var wastetype_height = _context.vscmstockwastages.Count() < 10 ? _context.vscmstockwastages.Count() * 28+ 130 : 300;

            ViewBag.statusheight = status_height;
            ViewBag.distheight = dist_height;
            ViewBag.notificaitonheight = notificaiton_height;
            ViewBag.wastetypeheight = wastetype_height;

            List<LineData> chartData = new List<LineData>
            {
                new LineData { xValue = 1392, yValue = 21, yValue1 = 28 },
                new LineData { xValue = 1393, yValue = 24, yValue1 = 44 },
                new LineData { xValue = 1394, yValue = 36, yValue1 = 48 },
                new LineData { xValue = 1395, yValue = 38, yValue1 = 50 },
                new LineData { xValue =1396, yValue = 54, yValue1 = 66 },
                new LineData { xValue = 1397, yValue = 57, yValue1 = 78 },
                new LineData { xValue = 1398, yValue = 70, yValue1 = 84 },
            };

            List<LineDataDistribution> chartDatadist = new List<LineDataDistribution>
            {
                new LineDataDistribution { xValue = 1397, yValue = 88, yValue1 = 78 },
                new LineDataDistribution { xValue = 1398, yValue = 56, yValue1 = 84 },
            };

            var nmrdata = _context.monthlysubmission
                .Where(m => (m.M1 + m.M2 + m.M3 + m.M4 + m.M5 + m.M6 + m.M7 + m.M8 + m.M9 + m.M10 + m.M11 + m.M12) > 1)
                .GroupBy(y => y.Year).Select(m => new
                {
                    xValue = m.Key,
                    yValue = m.Count()
                }).OrderBy(m=>m.xValue).ToList();

            var chartHeight = nmrdata.Max(m => m.yValue);

            ViewBag.dataSource = chartData;
            ViewBag.dataSourcedist = chartDatadist;
            ViewBag.dataSourceFacilities = nmrdata;
            ViewBag.chartheight = chartHeight;

            return View();
        }
        public class LineData
        {
            public int xValue;
            public double yValue;
            public double yValue1;
        }

        public class LineDataDistribution
        {
            public int xValue;
            public double yValue;
            public double yValue1;
        }

        public IActionResult ipl()
        {
            return View();
        }

        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
        {
            var data = _context.scmdashrequestip.ToList();
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
            int count = DataSource.Cast<scmdashrequestip>().Count();
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
    }
}