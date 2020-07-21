//using DataSystem.Models;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Features;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.FileProviders;
//using Syncfusion.EJ2.Base;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Http.Headers;

//namespace DataSystem.Controllers.SCM
//{

//    public class scmFilesController : Controller
//    {
//        private readonly WebNutContext _context;
//        private readonly UserManager<ApplicationUser> _userManager;
//        public scmFilesController(WebNutContext context, UserManager<ApplicationUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }
//        public IActionResult Index()
//        {
//            return View();
//        }
//        public IActionResult UrlDatasource([FromBody]DataManagerRequest dm)
//        {
//            var data = _context.scmFiles.ToList();
//            IEnumerable DataSource = data;
//            DataOperations operation = new DataOperations();
//            if (dm.Search != null && dm.Search.Count > 0)
//            {
//                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
//            }
//            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
//            {
//                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
//            }
//            if (dm.Where != null && dm.Where.Count > 0) //Filtering
//            {
//                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
//            }
//            int count = DataSource.Cast<scmFiles>().Count();
//            if (dm.Skip != 0)
//            {
//                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
//            }
//            if (dm.Take != 0)
//            {
//                DataSource = operation.PerformTake(DataSource, dm.Take);
//            }
//            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
//        }

//        public IActionResult Update(int OrderID, string CustomerID, IList<IFormFile> chunkFile, IList<IFormFile> file, string fileName, double fileSize, string fileType)
//        {
//            OrdersDetails val = OrdersDetails.GetAllRecords().Where(or => or.OrderID == OrderID).FirstOrDefault();
//            val.OrderID = OrderID;
//            val.CustomerID = CustomerID;
//            foreach (var files in file)
//                {
//                    var filename = ContentDispositionHeaderValue
//                                    .Parse(files.ContentDisposition)
//                                    .FileName
//                                    .Trim('"');
//                    var folders = filename.Split('/');
//                    var uploaderFilePath = hostingEnv.WebRootPath;
//                val.file = new OrdersDetails.File() { name = filename, onlinePath = uploaderFilePath + $@"\{filename}", type = '.' + filename.Split('.')[1] };
//                filename = uploaderFilePath + $@"\{filename}";
//                    if (!System.IO.File.Exists(filename))
//                    {
//                        using (FileStream fs = System.IO.File.Create(filename))
//                        {
//                            files.CopyTo(fs);
//                            fs.Flush();
//                        }
//                    }
//                }
//            return Json(new {val.CustomerID, val.OrderID, val.file });
//        }

//        private IHostingEnvironment hostingEnv;
//        public scmFilesController(IHostingEnvironment env)
//        {
//            this.hostingEnv = env;
//        }
//        [AcceptVerbs("Post")]
//        // Upload method for chunk-upload and normal upload

//        public IActionResult Save(IList<IFormFile> file)
//        {
//            try
//            {
//                foreach (var files in file)
//                {
//                    if (files != null)
//                    {
//                        var filename = ContentDispositionHeaderValue.Parse(files.ContentDisposition).FileName.Trim('"');
//                        filename = hostingEnv.WebRootPath + $@"\{filename}";
//                        if (!System.IO.File.Exists(filename))
//                        {
//                            using (FileStream fs = System.IO.File.Create(filename))
//                            {
//                                files.CopyTo(fs);
//                                fs.Flush();
//                            }
//                        }
//                        else
//                        {
//                            Response.Clear();
//                            Response.StatusCode = 204;
//                            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File already exists.";
//                        }
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                Response.Clear();
//                Response.ContentType = "application/json; charset=utf-8";
//                Response.StatusCode = 204;
//                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "No Content";
//                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
//            }
//            return Content("");
//        }
//        [AcceptVerbs("Post")]
//        public IActionResult Remove(IList<IFormFile> UploadFiles)
//        {
//            try
//            {
//                foreach (var file in UploadFiles)
//                {
//                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
//                    var filePath = Path.Combine(hostingEnv.WebRootPath);
//                    var fileSavePath = filePath + "\\" + fileName;
//                    if (!System.IO.File.Exists(fileSavePath))
//                    {
//                        System.IO.File.Delete(fileSavePath);
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                Response.Clear();
//                Response.StatusCode = 200;
//                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File removed successfully";
//                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
//            }
//            return Content("");
//        }
//        [HttpGet]
//        public FileResult Download(string filename)
//        {
//            var filePath = Path.Combine(
//                      Directory.GetCurrentDirectory(),
//                              "wwwroot");
//            IFileProvider provider = new PhysicalFileProvider(filePath);
//            IFileInfo fileInfo = provider.GetFileInfo(filename);
//            var readStream = fileInfo.CreateReadStream();
//            var mimeType = "application/pdf";
//            return File(readStream, mimeType, filename);
//        }

//        public IActionResult Insert(int OrderID, string CustomerID, IList<IFormFile> chunkFile, IList<IFormFile> file, string fileName, double fileSize, string fileType)
//        {
//          foreach (var files in file)
//                {
//                    var filename = ContentDispositionHeaderValue
//                                    .Parse(files.ContentDisposition)
//                                    .FileName
//                                    .Trim('"');
//                    var folders = filename.Split('/');
//                    var uploaderFilePath = hostingEnv.WebRootPath;
//                    OrdersDetails.GetAllRecords().Insert(0, new OrdersDetails() { OrderID = OrderID, CustomerID = CustomerID, file = new OrdersDetails.File() { name = filename, onlinePath = uploaderFilePath + $@"\{filename}", type = '.' + filename.Split('.')[1] } });
//                    filename = uploaderFilePath + $@"\{filename}";
//                    if (!System.IO.File.Exists(filename))
//                    {
//                        using (FileStream fs = System.IO.File.Create(filename))
//                        {
//                        files.CopyTo(fs);
//                            fs.Flush();
//                        }
//                    }
//                }
//                return Content("");
//        }

//        [HttpGet]
//        //Delete the record
//        public ActionResult Delete([FromBody]CRUDModel<OrdersDetails> value)
//        {
//            OrdersDetails.GetAllRecords().Remove(OrdersDetails.GetAllRecords().Where(or => or.OrderID == int.Parse(value.key.ToString())).FirstOrDefault());
//            return Json(value);
//        }

//        public class Data
//        {

//            public bool requiresCounts { get; set; }
//            public int skip { get; set; }
//            public int take { get; set; }
//        }
//        public class CRUDModel<T> where T : class
//        {
//            public string action { get; set; }

//            public string table { get; set; }

//            public string keyColumn { get; set; }

//            public object key { get; set; }

//            public T value { get; set; }

//            public List<T> added { get; set; }

//            public List<T> changed { get; set; }

//            public List<T> deleted { get; set; }

//            public IDictionary<string, object> @params { get; set; }
//        }
//    }
//    public class OrdersDetails
//    {
//        public static List<OrdersDetails> order = new List<OrdersDetails>();
//        public OrdersDetails()
//        {

//        }
//        public OrdersDetails(int OrderID, string CustomerId, int EmployeeId, double Freight, bool Verified, DateTime OrderDate, string ShipCity, string ShipName, string ShipCountry, DateTime ShippedDate, string ShipAddress, File file, string fileName, double filesize,string filetype)
//        {
//            this.OrderID = OrderID;
//            this.CustomerID = CustomerId;
//            this.EmployeeID = EmployeeId;
//            this.Freight = Freight;
//            this.ShipCity = ShipCity;
//            this.Verified = Verified;
//            this.OrderDate = OrderDate;
//            this.ShipName = ShipName;
//            this.ShipCountry = ShipCountry;
//            this.ShippedDate = ShippedDate;
//            this.ShipAddress = ShipAddress;
//            this.file = file;
//            this.fileName = filetype;
//            this.fileSize = filesize;
//            this.fileType = filetype;
//        }

//        public class File
//        {
//            public string name { get; set; }
//            public long size { get; set; }
//            public string type { get; set; }

//            public string onlinePath { get; set; }
//        }
//        public static List<OrdersDetails> GetAllRecords()
//        {
//            if (order.Count() == 0)
//            {
//                int code = 10000;
//                for (int i = 1; i < 2; i++)
//                {
//                    order.Add(new OrdersDetails(code + 1, "ALFKI", i + 0, 2.3 * i, false, new DateTime(1991, 05, 15), "101", "New York", "1", new DateTime(1996, 7, 16), "Kirchgasse 6", new File(),"",0,""));
                   
//                }
//            }
//            return order;
//            //if (order.Count() == 0)
//            //{
//            //    int code = 10000;
//            //    int i = 1;
//            //    order.Add(new OrdersDetails(code + 1, "ALFKI", i + 0, 2.3 * i, false, new DateTime(1991, 05, 15), "101", "New York", "1", new DateTime(1996, 7, 16), "Kirchgasse 6", new File()));

//            //}
//            //return order;
//        }

//        public int? OrderID { get; set; }
//        public string CustomerID { get; set; }
//        public int? EmployeeID { get; set; }
//        public double? Freight { get; set; }
//        public string ShipCity { get; set; }
//        public bool Verified { get; set; }
//        public DateTime OrderDate { get; set; }

//        public string ShipName { get; set; }

//        public string ShipCountry { get; set; }

//        public DateTime ShippedDate { get; set; }
//        public string ShipAddress { get; set; }

//        public File file { get; set; }

//        public string fileName { get; set; }

//        public double fileSize { get; set; }
//        public string fileType { get; set; }


//    }
//}