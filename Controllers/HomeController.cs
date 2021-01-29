using Lubes.DBContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOP.Models;
using SHOP_DECOMPILED.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
namespace SHOP.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string today = DateTime.Now.ToString("dd/MM/yyyy");


        public HomeController(ApplicationDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }






        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "2")]

        public IActionResult attendant()
        {
            List<sold_items> list_of_sold = _context.sold_items.Where(x => x.DateTime == today).ToList();
            List<shop_items> list_of_brands = _context.Shop_items.ToList();
            List<join_sold_item> joinList = new List<join_sold_item>();
            var results = (from pd in list_of_sold
                           join od in list_of_brands on pd.Item_id equals od.id
                           select new
                           {
                               pd.DateTime,
                               pd.quantity_sold,
                               pd.Total_cash_made,
                               od.Item_price,
                               od.Item_name,


                           }).ToList();

            foreach (var item in results)
            {
                join_sold_item JoinObject = new join_sold_item();

                JoinObject.Item_name = item.Item_name;
                JoinObject.Item_price = item.Item_price;

                JoinObject.DateTime = item.DateTime;
                JoinObject.quantity_sold = item.quantity_sold;
                JoinObject.Total_cash_made = item.Total_cash_made;
                JoinObject.Item_price = item.Item_price;
                JoinObject.Item_name = item.Item_name;
                joinList.Add(JoinObject);

            }
            var JoinListToViewbag = joinList.ToList();

            ViewBag.JoinList = JoinListToViewbag;

            ViewBag.allBrands = _context.Shop_items.Where(x => x.Quantity > 0).ToList();
            ViewBag.allBrands_0 = _context.Shop_items.ToList();
            ViewBag.count_below = _context.Shop_items.Count(x => x.Quantity <= 0);
            ViewBag.to_restock = _context.Shop_items.Where(x => x.Quantity <= 0);
            ViewBag.count_all = _context.Shop_items.Count();
            ViewBag.sold = _context.sold_items.Where(x => x.DateTime == today).Sum(x => x.quantity_sold);
            ViewBag.shop_name = HttpContext.Session.GetString("shop_name");
            ViewBag.name = HttpContext.Session.GetString("Name");
            ViewBag.phone = HttpContext.Session.GetString("phone");
            Random r = new Random();
            ViewBag.Random = r.Next(1000000, 9999999);
            return View();
        }
        public IActionResult log_out()
        {
            HttpContext.Session.Clear();
            return Redirect("~/log_in/log_in");
        }
        [HttpPost]
        public IActionResult sell_Item(int id_finish, float quantity_sold, float submit_price, float Total_cash_made, string date)
        {
            var check_if_exists = _context.sold_items.FirstOrDefault(x => x.Item_id.ToString() == id_finish.ToString() && x.DateTime == date);
            var shop_itemss = _context.Shop_items.FirstOrDefault(x => x.id == id_finish);

            //LETS CHECK IF WE SOLD SIMILAR ITEM TODAY
            if (check_if_exists != null)
            {
                //WE HAVE SOLD SIMILAR ITEM TODAY

                var initial_quantity = shop_itemss.Quantity;
                var initial_sold_quantity = check_if_exists.quantity_sold;
                if (initial_quantity <= 0)
                {
                    Total_cash_made person = new Total_cash_made
                    {
                        total = "Unable to sell the item.Stock running low"
                    };
                    return Json(person);

                }
                else
                {
                    //LETS FIND TOTAL COST PRICE PER ITEM
                    float initial_cost_price_total = check_if_exists.Total_Cost_cash;
                    float total_cost_price = shop_itemss.Cost_price * quantity_sold;
                    float final_cost_price = initial_cost_price_total + total_cost_price;
                    

                    //

                    //LETS SUBTRACT ITEMS
                    float new_quantity = initial_quantity + quantity_sold;
                    //LETS ADD SOLD ITEMS
                    float new_sold_quantity = initial_sold_quantity + quantity_sold;
                    //LETS GET INITIAL TOTAL CAS
                    float initial_total_cash = check_if_exists.Total_cash_made;
                    //LETS ADD TOTAL CASH
                    var new_total_cash = initial_total_cash + Total_cash_made;

                    //LETS UPDATE FIELDS
                    check_if_exists.DateTime = date;
                    check_if_exists.Total_cash_made = new_total_cash;
                    check_if_exists.quantity_sold = new_sold_quantity;
                    check_if_exists.Total_Cost_cash = final_cost_price;
                    _context.Entry(check_if_exists).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();

                    //LETS UPDATE  REMAINING QUANTITY
                    var change_r = _context.Shop_items.FirstOrDefault(x => x.id == id_finish);
                    float initial_q = change_r.Quantity;
                    float new_Q = initial_q - quantity_sold;
                    change_r.Quantity = new_Q;
                    _context.Entry(change_r).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();
                    Total_cash_made person = new Total_cash_made
                    {

                        total = new_sold_quantity.ToString()
                    };
                    return Json(person);

                }

            }
            else
            {
                //WE HAVENT SOLD ANY ITEM
                //LETS UPDATE QUANTITY
                //LETS UPDATE  REMAINING QUANTITY
                var change_r = _context.Shop_items.FirstOrDefault(x => x.id == id_finish);
                float initial_q = change_r.Quantity;
                float new_Q = initial_q - quantity_sold;
                change_r.Quantity = new_Q;
                _context.Entry(change_r).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
                Total_cash_made person = new Total_cash_made
                {
                    total = "Sales have been done successfully and receipt downloaded and stored in your local machine"
                };
                //LETS FIND TOTAL COST PRICE PER ITEM
                var check_if_exists1 = _context.sold_items.FirstOrDefault(x => x.Item_id.ToString() == id_finish.ToString() && x.DateTime == date);
                float final_cost_price;
                if (check_if_exists1 == null)
                {
                    float initial_cost_price_total =0;
                    float total_cost_price = shop_itemss.Cost_price * quantity_sold;
                     final_cost_price = initial_cost_price_total + total_cost_price;
                }
                else
                {
                    float initial_cost_price_total = check_if_exists1.Total_Cost_cash;
                    float total_cost_price = shop_itemss.Cost_price * quantity_sold;
                    final_cost_price = initial_cost_price_total + total_cost_price;

                }
             


                //LETS CREATE NEW DOCUMENT FOR NEW DATES
                sold_items x = new sold_items
                {
                    Item_id = id_finish,
                    quantity_sold = quantity_sold,
                    Total_cash_made = Total_cash_made,
                    Total_Cost_cash = final_cost_price,
                    DateTime = date


                };
                _context.Add(x);
                _context.SaveChanges();
                return Json(person);

            }




        }
        public IActionResult delete_item(int id)
        {
            var itemToRemove = _context.Shop_items.SingleOrDefault(x => x.id == id); //returns a single item.
            var item_name = itemToRemove.Item_name;
            if (itemToRemove != null)
            {
                _context.Shop_items.Remove(itemToRemove);
                _context.SaveChanges();
                TempData["popup"] = "1";
                //TempData["popup"] = "2";
                //TempData["popup"] = "Successfully working!";
                TempData["message"] = "You have successfully removed :" + item_name + " from the system";
            }
            else
            {
                TempData["popup"] = "2";
                //TempData["popup"] = "2";
                //TempData["popup"] = "Successfully working!";
                TempData["message"] = "Error! item does not exists!";
            }
            return Redirect("~/home/admin");


        }

        public IActionResult restock(int id, int ammount)
        {

            var rest = _context.Shop_items.FirstOrDefault(x => x.id == id);
            float initialStock = rest.Quantity;
            var new_stock = initialStock + ammount;
            rest.Quantity = new_stock;
            //db.Entry(payment).State = EntityState.Modified;
            _context.Entry(rest).State = EntityState.Modified;
            _context.SaveChanges();
            var date = DateTime.Now.ToString();
            Restock_history insert_new = new Restock_history
            {

                Item_id = id,
                Date_restock = date,
                new_quanity = new_stock,
                Prev_quantity = initialStock,
                quantity = ammount


            };
            _context.Add(insert_new);
            _context.SaveChanges();

            TempData["popup"] = "1";
            //TempData["popup"] = "2";
            //TempData["popup"] = "Successfully working!";
            TempData["message"] = "You have successfully added stock to " + rest.Item_name + " from: " + initialStock + " to:" + new_stock;

            return Redirect("~/home/admin");
        }
        public IActionResult change_price(int change_price_id, int new_price)
        {

            var rest = _context.Shop_items.FirstOrDefault(x => x.id == change_price_id);

            var initial_price = rest.Item_price;
            rest.Item_price = new_price;
            //db.Entry(payment).State = EntityState.Modified;
            _context.Entry(rest).State = EntityState.Modified;
            _context.SaveChanges();
            var date = DateTime.Now.ToString();
            //Restock_history insert_new = new Restock_history { 

            //    Item_id=id,
            //    Date_restock= date,
            //    new_quanity=new_stock,
            //    Prev_quantity=initialStock,
            //    quantity=new_stock


            //}; 
            //_context.Add(insert_new);
            //_context.SaveChanges();

            TempData["popup"] = "1";
            //TempData["popup"] = "2";
            //TempData["popup"] = "Successfully working!";
            TempData["message"] = "You have successfully changed price of:" + rest.Item_name + " from: " + initial_price + " to:" + new_price;

            return Redirect("~/home/admin");
        }

        //public async Task<IActionResult> Upload_system_imagesAsync(Dynami_images log)
        //{
        //public async Task<IActionResult> Upload_system_images( model)
        //{

        //    if (log.popup_icon_url != null)
        //    {
        //        string folder = "Dynamic_images/";
        //       string item_icon= folder+Guid.NewGuid().ToString() + log.item_icon.FileName;
        //        string item_icon_server_folder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
        //        await log.item_icon.CopyToAsync(new FileStream(item_icon_server_folder, FileMode.Create));
        //        //model.SaveAs(Server.MapPath(filePath));
        //        //var filename = Path.GetFileName(file.FileName);
        //        //var path = Path.Combine(Server.MapPath("~/Uploads/Photo/"), filename);
        //        //file.SaveAs(path);
        //        //tyre.Url = filename;


        //        Item_category itemC = new Item_category
        //        {
        //            Category_name = model.Category_name,
        //            ImageURL = folder,

        //        };
        //        _context.Add(itemC);
        //        await _context.SaveChangesAsync();
        //    }
        //}


        //}


        public class Total_cash_made
        {

            public string total { get; set; }
        }


        public IActionResult pdf_test()
        {
            return View();
        }
        public IActionResult generatePDF()
        {
            PdfDocument doc = new PdfDocument();
            //Add a page.
            PdfPage page = doc.Pages.Add();
            //Create a PdfGrid.
            PdfGrid pdfGrid = new PdfGrid();
            PdfGraphics graphics = page.Graphics;
            //Load the image as stream.
            FileStream imageStream = new FileStream("wwwroot/images/logo2.png", FileMode.Open, FileAccess.Read);
            PdfBitmap image = new PdfBitmap(imageStream);
            graphics.DrawImage(image, 0, 0);
            PdfGridCellStyle headerStyle = new PdfGridCellStyle();
            headerStyle.Borders.All = new PdfPen(new PdfColor(126, 151, 173));
            headerStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(126, 151, 173));
            headerStyle.TextBrush = PdfBrushes.White;
            headerStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 14f, PdfFontStyle.Regular);
            //Add values to list
            //List<object> data = new List<object>();
            var list_all = _context.Shop_items.ToList();
            //Object row1 = new { ID = "E01", Name = "Clay" };
            //Object row2 = new { ID = "E02", Name = "Thomas" };
            //Object row3 = new { ID = "E03", Name = "Andrew" };
            //Object row4 = new { ID = "E04", Name = "Paul" };
            //Object row5 = new { ID = "E05", Name = "Gray" };
            //data.Add(row1);
            //data.Add(row2);
            //data.Add(row3);
            //data.Add(row4);
            //data.Add(row5);
            //Add list to IEnumerable
            IEnumerable<object> dataTable = list_all;
            //Assign data source.
            pdfGrid.DataSource = dataTable;
            //Draw grid to the page of PDF document.
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(10, 10));
            //Save the PDF document to stream
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            //If the position is not set to '0' then the PDF will be empty.
            stream.Position = 0;
            //Close the document.
            doc.Close(true);
            //Defining the ContentType for pdf file.
            string contentType = "application/pdf";
            //Define the file name.
            string fileName = "Output.pdf";
            //Creates a FileContentResult object by using the file contents, content type, and file name.
            return File(stream, contentType, fileName);

        }
        [HttpPost]
        public JsonResult setTempData(string name)
        {
            //ViewData.Remove("calculated");
            //var a = "'";
            //var b = "'";
            var c = name;

            Total_cash_made person = new Total_cash_made
            {

                total = c

            };
            //ViewData["calculated"] = name;

            return Json(person);

        }
        [HttpPost]

        public JsonResult AjaxMethod(string name)
        {
            //var rol = User.Identity.Name;
            //if (rol == "1")
            //{
            Total_cash_made person = new Total_cash_made
            {

                total = name

            };

            return Json(person);

            //}


        }

        [Authorize(Roles = "1")]
        public IActionResult admin(log_in log, [Optional] String date)
        {
            DateTime _date;
            string day;

            //SOLD ITEMS FOR TODAY
            List<sold_items> list_of_sold = _context.sold_items.Where(x => x.DateTime == today).ToList();
            List<shop_items> list_of_brandss = _context.Shop_items.ToList();
            List<join_sold_item> joinList1 = new List<join_sold_item>();
            var results1 = (from pd in list_of_sold
                            join od in list_of_brandss on pd.Item_id equals od.id
                            select new
                            {
                                pd.DateTime,
                                pd.quantity_sold,
                                pd.Total_cash_made,
                                od.Item_price,
                                od.Item_name,
                                pd.Total_Cost_cash,
                            }).ToList();

            foreach (var item in results1)
            {
                join_sold_item JoinObject = new join_sold_item();

                JoinObject.Item_name = item.Item_name;
                JoinObject.Item_price = item.Item_price;

                JoinObject.DateTime = item.DateTime;
                JoinObject.quantity_sold = item.quantity_sold;
                JoinObject.Total_cash_made = item.Total_cash_made;

                JoinObject.Item_price = item.Item_price;
                JoinObject.Item_name = item.Item_name;
                JoinObject.Total_Cost_cash = item.Total_Cost_cash;
                joinList1.Add(JoinObject);
            }
            var JoinListToViewbag1 = joinList1.ToList();
            ViewBag.JoinList1 = JoinListToViewbag1;


            if (date == null)
            {
                //SOLD ITEMS FOR THE FIRST MODAL
                List<sold_items> list_of_sold_second = _context.sold_items.ToList();

                List<shop_items> list_of_brands_second = _context.Shop_items.ToList();
                List<join_sold_item> joinList_second = new List<join_sold_item>();
                var results_second = (from pd in list_of_sold_second
                                      join od in list_of_brands_second on pd.Item_id equals od.id
                                      select new
                                      {
                                          pd.DateTime,
                                          pd.quantity_sold,
                                          pd.Total_cash_made,
                                          od.Item_price,
                                          od.Item_name,
                                          pd.Total_Cost_cash


                                      }).ToList();

                foreach (var item in results_second)
                {
                    join_sold_item JoinObject_second = new join_sold_item();

                    JoinObject_second.Item_name = item.Item_name;
                    JoinObject_second.Item_price = item.Item_price;
                    JoinObject_second.Total_Cost_cash = item.Total_Cost_cash;

                    JoinObject_second.DateTime = item.DateTime;
                    JoinObject_second.quantity_sold = item.quantity_sold;
                    JoinObject_second.Total_cash_made = item.Total_cash_made;
                    JoinObject_second.Item_price = item.Item_price;
                    JoinObject_second.Item_name = item.Item_name;
                    joinList_second.Add(JoinObject_second);
                }
                var JoinListToViewbag_second = joinList_second.ToList();
                ViewBag.JoinList12 = JoinListToViewbag_second;

            }
            else
            {
                _date = DateTime.Parse(date);
                day = _date.ToString("dd/MM/yyyy");





                //LETS COMPUTE IF COMMAND IS FILTER
                List<sold_items> list_of_sold_third = _context.sold_items.Where(x => x.DateTime == day).ToList();

                List<shop_items> list_of_brands_third = _context.Shop_items.ToList();
                List<join_sold_ite_filtered> joinList_third = new List<join_sold_ite_filtered>();
                var results_third = (from pd in list_of_sold_third
                                     join od in list_of_brands_third on pd.Item_id equals od.id
                                     select new
                                     {
                                         pd.DateTime,
                                         pd.quantity_sold,
                                         pd.Total_cash_made,
                                         od.Item_price,
                                         od.Item_name,


                                     }).ToList();

                foreach (var item in results_third)
                {
                    join_sold_ite_filtered JoinObject_third = new join_sold_ite_filtered();

                    JoinObject_third.Item_name = item.Item_name;
                    JoinObject_third.DateTime = item.DateTime;
                    JoinObject_third.quantity_sold = item.quantity_sold;
                    JoinObject_third.Total_cash_made = item.Total_cash_made;
                    JoinObject_third.Item_price = item.Item_price;
                    JoinObject_third.Item_name = item.Item_name;
                    joinList_third.Add(JoinObject_third);
                }
                var JoinListToViewbag_third = joinList_third.ToList();
                var count = joinList_third.Count();


                var sum_of_cash = joinList_third.Sum(x => x.Total_cash_made);
                TempData["popup"] = 4;
                //TempData["popup"] = "2";
                //TempData["popup"] = "Successfully working!";
                TempData["message"] = count + " records found totaling to Ksh. " + sum_of_cash;
                TempData["total"] = sum_of_cash;


                ViewBag.JoinList_general_third = JoinListToViewbag_third;
            }
            ViewBag.allBrands = _context.Shop_items.Where(x => x.Quantity > 0).ToList();
            ViewBag.allBrands_0 = _context.Shop_items.ToList();
            ViewBag.count_below = _context.Shop_items.Count(x => x.Quantity <= 0);
            ViewBag.to_restock = _context.Shop_items.Where(x => x.Quantity <= 0);
            ViewBag.count_all = _context.Shop_items.Count();
            ViewBag.sold = _context.sold_items.Where(x => x.DateTime == today).Sum(x => x.quantity_sold);
            ViewBag.sold_general = _context.sold_items;
            //HttpContext.Session.SetString("roles", user_id.strRole.ToString());
            //HttpContext.Session.SetString("Name", user_id.Full_name);
            //HttpContext.Session.SetString("shop_name", user_id.Shop_name);
            ViewBag.shop_name = HttpContext.Session.GetString("shop_name");
            ViewBag.name = HttpContext.Session.GetString("Name");
            //ViewBag.name=HttpContext.Session.GetString("user_id");


            //RESTOCKING ITEMS HISTORY
            List<Restock_history> list_of_restocked = _context.Restock_history.ToList();
            List<shop_items> list_of_brands = _context.Shop_items.ToList();
            List<join_tables> joinList = new List<join_tables>();
            var shop_items = _context.Shop_items;
            var restock_item = _context.Restock_history;
            var results = (from pd in list_of_restocked
                           join od in list_of_brands on pd.Item_id equals od.id
                           select new
                           {
                               pd.Date_restock,
                               pd.Prev_quantity,
                               pd.new_quanity,
                               od.Item_name,
                               od.Item_price,
                               od.Quantity,
                               od.id,

                           }).ToList();

            foreach (var item in results)
            {
                join_tables JoinObject = new join_tables();
                JoinObject.Item_id = item.id.ToString();
                JoinObject.Item_name = item.Item_name;
                JoinObject.Item_price = item.Item_price;
                JoinObject.new_quanity = item.new_quanity;
                JoinObject.Prev_quantity = item.Prev_quantity;
                JoinObject.quantity = item.Quantity;
                JoinObject.Date_restock = item.Date_restock;
                joinList.Add(JoinObject);

            }
            var JoinListToViewbag = joinList.ToList();

            ViewBag.JoinList = JoinListToViewbag;
            //TempData["message"] = "success you have deleted the attendant successfully!";
            //GETTING ALL BRANDS
            ViewBag.allBrands = _context.Shop_items.ToList();
            // GETTING ALL ATTENDANTS
            //var phone = @User.Claims.FirstOrDefault(c => c.Type == "User_id").Value;
            ViewBag.all_attendants = _context.Log_in.Where(item => item.strRole == 2).ToList();
            //var getting_quanity = _context.Shop_items.FirstOrDefault();
            //ViewBag.stock=getting_quanity.
            var count_brand = _context.Shop_items.Count();
            if (count_brand == null)
            {
                ViewBag.count_all = 0;
            }
            else
            {
                ViewBag.count_all = count_brand;

            }
            var count_below = _context.Shop_items.Where(x => x.Quantity < 5).Count();
            if (count_below == null)
            {
                ViewBag.count_below = 0;
            }
            else
            {
                ViewBag.count_below = count_below;

            }
            //ViewBag.manager_name = _context.Log_in.Where(item=>item.id.ToString()==phone).ToList();
            //TempData["popup"] = "2";

            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult delete_attwndant(String id)
        {
            var itemToRemove = _context.Log_in.SingleOrDefault(x => x.id.ToString() == id); //returns a single item.
            var nameOfVictim = itemToRemove.Full_name;
            if (itemToRemove != null)
            {
                _context.Log_in.Remove(itemToRemove);
                _context.SaveChanges();
            }
            TempData["popup"] = "3";
            //TempData["popup"] = "2";
            //TempData["popup"] = "Successfully working!";
            TempData["message"] = nameOfVictim + " has been successfully removed from the system!";
            TempData["total"] = nameOfVictim + " has been successfully removed from the system!";
            return Redirect("~/home/admin");

        }

        public IActionResult add_new_attendant(String Full_name, String Phone)
        {
            //LETS JOIN TABLES















            var check_if_item_exists1 = _context.Log_in.SingleOrDefault(x => x.Phone == Phone);
            if (check_if_item_exists1 != null)
            {

                TempData["popup"] = "2";
                //TempData["popup"] = "2";
                //TempData["popup"] = "Successfully working!";
                TempData["message"] = Full_name + " already exists in the system!";
                return Redirect("~/home/admin");

            }
            else
            {
                var shop = _context.Log_in.FirstOrDefault(x => x.strRole == 1);

                log_in add_new_attendant = new log_in
                {
                    Full_name = Full_name,
                    Phone = Phone,
                    Password = Phone,
                    Shop_name = shop.Shop_name,
                    strRole = 2,

                };
                _context.Add(add_new_attendant);
                _context.SaveChanges();

                TempData["popup"] = "1";
                //TempData["popup"] = "2";
                //TempData["popup"] = "Successfully working!";
                TempData["message"] = Full_name + " successfully added to the list!";
            }

            return Redirect("~/home/admin");

        }
        public IActionResult add_item(String Item_name, float Item_price, int Quantity, String date)
        {
            var check_if_item_exists = _context.Shop_items.SingleOrDefault(x => x.Item_name == Item_name);
            if (check_if_item_exists == null)
            {
                shop_items add_new_product = new shop_items
                {
                    Item_name = Item_name,
                    Item_price = Item_price,
                    Quantity = Quantity,
                    DateTime = date


                };

                _context.Add(add_new_product);
                _context.SaveChanges();

                //TempData["popup"] = "1";
                ////TempData["popup"] = "2";
                ////TempData["popup"] = "Successfully working!";
                //TempData["message"] = Item_name + " successfully added to the list!"; 
                TempData["popup"] = "5";
                //TempData["popup"] = "2";
                //TempData["popup"] = "Successfully working!";
                TempData["message"] = Item_name + " successfully added to the list!";
            }
            else
            {
                TempData["popup"] = "5";
                //TempData["popup"] = "2";
                //TempData["popup"] = "Successfully working!";
                TempData["message"] = Item_name + " already exists in the system!";
            }

            return Redirect("~/home/admin");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult error_page()
        {

            return View();
        }
    }
}
