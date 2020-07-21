

using System.Collections.Generic;
using System.Linq;
using DataSystem.Models;
using DataSystem.Models.ViewModels.chart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System;

namespace DataSystem.Controllers
{
    [Authorize]
    public class DashboardmController : Controller
    {
        private readonly WebNutContext _db;
        public DashboardmController(WebNutContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var items = _db.Nmr.Select(s=>new
            {
                Year =  s.mYear,
                Des = s.mYear.ToString()
            }).Distinct().ToList();
            items.OrderBy(m => m.Year);
            items.Insert(0, new {Year=0, Des = "All" });
            ViewData["yearm"] = new SelectList(items, "Year", "Des");
            return View();
        }
        [HttpGet("/dashboardm/dashStat/{year}")]
        public IActionResult dashStat(int year)
        {
            IQueryable<TblOtp> get = _db.TblOtp;
            IQueryable<TblMam> getm = _db.TblMam;
            IQueryable<TblOtptfu> get2 = _db.TblOtptfu;

            if (year != 0)
            {
                get = _db.TblOtp.Where(m => m.Nmr.mYear == year);
                getm = _db.TblMam.Where(m => m.Nmr.mYear == year);
                get2 = _db.TblOtptfu.Where(m => m.Nmr.mYear == year);
            }
            var samin = get.AsNoTracking().Select(m => new submission()
            {
                ProvCode = m.Nmr.Facility.DistNavigation.ProvCode,
                DistCode = m.Nmr.Facility.DistCode,
                Implementer = m.Nmr.Implementer,
                FacilityCode = m.Nmr.Nmrid.Substring(0, m.Nmr.Nmrid.IndexOf("-"))
            }).ToList();

            var samout = get2.AsNoTracking().Select(m => new submission()
            {
                ProvCode = m.Nmr.Facility.DistNavigation.ProvCode,
                DistCode = m.Nmr.Facility.DistCode,
                Implementer = m.Nmr.Implementer,
                FacilityCode = m.Nmr.Nmrid.Substring(0, m.Nmr.Nmrid.IndexOf("-"))

            }).ToList();

            var mam = getm.AsNoTracking().Select(m => new submission()
            {
                ProvCode = m.Nmr.Facility.DistNavigation.ProvCode,
                DistCode = m.Nmr.Facility.DistCode,
                Implementer = m.Nmr.Implementer,
                FacilityCode = m.Nmr.Nmrid.Substring(0, m.Nmr.Nmrid.IndexOf("-"))

            }).ToList();

            List<submission> data = new List<submission>();

            data.AddRange(samin);
            data.AddRange(samout);
            data.AddRange(mam);
            submissionRes res = new submissionRes();
            res.ExDistricts = _db.Districts.Count();
            res.ExProvinces = _db.Provinces.Count();
            res.ExFaciliteis = _db.FacilityInfo.Count();
            res.SubProvinces = data.Select(m => m.ProvCode).Distinct().Count();
            res.SubDistricts = data.Select(m => m.DistCode).Distinct().Count();
            res.SubFacilities = data.Select(m => m.FacilityCode).Distinct().Count();
            res.SubOrgs = data.Select(m => m.Implementer).Distinct().Count();

            return Ok(res);
        }
        [HttpGet("/dashboardm/provAll/{year}")]
        public IActionResult provAll(int year)
        {
            IQueryable<TblOtp> get = _db.TblOtp.Where(m => !m.Otp.AgeGroup.ToLower().Contains("total"));
            IQueryable<TblMam> getm = _db.TblMam.Where(m => !m.Mam.AgeGroup.ToLower().Contains("total"));;
            IQueryable<TblOtptfu> get2 = _db.TblOtptfu.Where(m => !m.Otptfu.AgeGroup.ToLower().Contains("total"));
            if (year != 0)
            {
                get = _db.TblOtp.Where(m =>m.Nmr.mYear == year && !m.Otp.AgeGroup.ToLower().Contains("total"));
                getm = _db.TblMam.Where(m =>m.Nmr.mYear == year && !m.Mam.AgeGroup.ToLower().Contains("total"));
                get2 = _db.TblOtptfu.Where(m =>m.Nmr.mYear == year && !m.Otptfu.AgeGroup.ToLower().Contains("total"));
            }
            var samin = get.AsNoTracking().Select(m => new allReports()
            {
                PROV_32_ID = m.Nmr.Facility.DistNavigation.ProvCode,
                Pname = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName,
                Male = m.TMale.GetValueOrDefault(),
                Female = m.TFemale.GetValueOrDefault(),
                Cured = m.Cured.GetValueOrDefault(),
                Deaths = m.Death.GetValueOrDefault(),
                Defaulter = m.Default.GetValueOrDefault(),
                Age = m.Otp.AgeGroup,

                Odema = m.Odema.GetValueOrDefault(),
                Zscore23 = m.Z3score.GetValueOrDefault(),
                NonCured = m.NonCured.GetValueOrDefault(),
                ReferOuts = m.RefOut.GetValueOrDefault(),
                MUAC115 = m.Muac115.GetValueOrDefault(),
                ReferIn = m.Fromsfp.GetValueOrDefault() + m.Fromscotp.GetValueOrDefault() + m.Defaultreturn.GetValueOrDefault(),

            }).ToList();

            var samout = get2.AsNoTracking().Select(m => new allReports()
            {
                PROV_32_ID = m.Nmr.Facility.DistNavigation.ProvCode,
                Pname = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName,
                Male = m.TMale.GetValueOrDefault(),
                Female = m.TFemale.GetValueOrDefault(),
                Cured = m.Cured.GetValueOrDefault(),
                Deaths = m.Death.GetValueOrDefault(),
                Defaulter = m.Default.GetValueOrDefault(),
                Age = m.Otptfu.AgeGroup,

                Odema = m.Odema.GetValueOrDefault(),
                Zscore23 = m.Z3score.GetValueOrDefault(),
                NonCured = m.NonCured.GetValueOrDefault(),
                ReferOuts = m.RefOut.GetValueOrDefault(),
                MUAC115 = m.Muac115.GetValueOrDefault(),
                ReferIn = m.Fromsfp.GetValueOrDefault() + m.Fromscotp.GetValueOrDefault() + m.Defaultreturn.GetValueOrDefault(),
            }).ToList();

            var mam = getm.AsNoTracking().Select(m => new allReports()
            {
                PROV_32_ID = m.Nmr.Facility.DistNavigation.ProvCode,
                Pname = m.Nmr.Facility.DistNavigation.ProvCodeNavigation.ProvName,
                Male = m.TMale.GetValueOrDefault(),
                Female = m.TFemale.GetValueOrDefault(),
                Cured = m.Cured.GetValueOrDefault(),
                Deaths = m.Deaths.GetValueOrDefault(),
                Defaulter = m.Defaulters.GetValueOrDefault(),
                Age = m.Mam.AgeGroup,

                Zscore23 = m.Zscore23.GetValueOrDefault(),
                NonCured = m.NonCured.GetValueOrDefault(),
                MUAC12 = m.Muac12.GetValueOrDefault(),
                MUAC23 = m.Muac23.GetValueOrDefault(),
                ReferIn = m.ReferIn.GetValueOrDefault(),
                ReferOuts = m.Transfers.GetValueOrDefault(),

            }).Where(m=>m.Age.Contains("month")).ToList();

            List<allReports> data = new List<allReports>();

            data.AddRange(samin);
            data.AddRange(samout);
            data.AddRange(mam);

            List<allProvince> res = new List<allProvince>();

            foreach (var item in data.Select(m => m.PROV_32_ID).Distinct())
            {
                var row = new allProvince();

                row.PROV_32_ID =item;

                row.Pname = data.Where(m => m.PROV_32_ID.Equals(item)).Select(m => m.Pname).FirstOrDefault();

                row.Value = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.Male + m.Female);

                row.Children6m = data.Where(m => m.PROV_32_ID.Equals(item) && m.Age.ToLower().Contains("6 month")).Sum(m => m.Male + m.Female);
                row.Children6mp = ((double)row.Children6m / row.Value) * 100;

                row.Children23m = data.Where(m => m.PROV_32_ID.Equals(item) && m.Age.ToLower().Contains("6-23 months")).Sum(m => m.Male + m.Female);
                row.Children23mp = ((double)row.Children23m / row.Value) * 100;

                row.Children59m = data.Where(m => m.PROV_32_ID.Equals(item) && m.Age.ToLower().Contains("24-59 months")).Sum(m => m.Male + m.Female);
                row.Children59mp = ((double)row.Children59m / row.Value) * 100;

                row.Discharge = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.Cured + m.Deaths + m.Defaulter);
                row.DischargeP = ((double)row.Discharge / row.Value) * 100;
                if (row.Discharge != 0)
                {
                    row.Cured = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.Cured);
                    row.CuredP = ((double)row.Cured / row.Discharge) * 100;

                    row.Deaths = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.Deaths);
                    row.DeathP = ((double)row.Deaths / row.Discharge) * 100;

                    row.Defaulter = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.Defaulter);
                    row.DefaultP = ((double)row.Defaulter / row.Discharge) * 100;

                    row.NonCured = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.NonCured);
                    row.NonCuredP = ((double)row.NonCured / row.Discharge) * 100;
                }

                row.Male = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.Male);
                row.MaleP = ((double)row.Male / row.Value) * 100;

                row.Female = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.Female);
                row.Femalep = ((double)row.Female / row.Value) * 100;

                row.Odema = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.Odema);
                row.Zscore23 = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.Zscore23);
                row.ReferIn = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.ReferIn);
                row.ReferOuts = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.ReferOuts);
                row.MUAC115 = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.MUAC115);
                row.MUAC12 = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.MUAC12);
                row.MUAC23 = data.Where(m => m.PROV_32_ID.Equals(item)).Sum(m => m.MUAC23);
                
                res.Add(row);

            }


            return Ok(res);
        }

    }

}