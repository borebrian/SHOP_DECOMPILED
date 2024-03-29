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
        //private readonly string today = DateTime.Now.ToString("dd/MM/yyyy");
        private static TimeZoneInfo E_Africa_standard_time = TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time");
        private static  DateTime today1 = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, E_Africa_standard_time);
        string today = today1.ToString("dd/MM/yyyy");

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
            ViewBag.time = today;
            string printer = HttpContext.Request.Cookies["printer_name"];
            ViewBag.printer = printer;
            expiries_set();
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
            ViewBag.count_all = _context.Shop_items.Sum(x => x.Quantity);



            var sold = _context.sold_items.ToList();
            if (sold.Count() == 0)
            {

                ViewBag.sold = _context.sold_items.Where(x => x.DateTime == today).Sum(x => x.quantity_sold);



            }
            else
            {
                ViewBag.sold = _context.sold_items.Where(x => x.DateTime == today).Sum(x => x.quantity_sold);

            }

            ViewBag.shop_name = HttpContext.Session.GetString("shop_name");
            ViewBag.name = HttpContext.Session.GetString("Name");
            ViewBag.phone = HttpContext.Session.GetString("phone");
            ViewBag.id = HttpContext.Session.GetString("id");

            var phone = HttpContext.Session.GetString("phone");
            var pass = _context.Log_in.SingleOrDefault(z => z.Phone == phone);
           
            Random r = new Random();
            ViewBag.Random = r.Next(1000000, 9999999);
            return View();
        }
        public IActionResult log_out()
        {
            HttpContext.Session.Clear();
            return Redirect("~/log_in/log_in");
        }
        //[AllowAnonymous]

        public IActionResult Expiries()
        {

            ViewBag.expiry_details = _context.Expiries.ToList();
            ViewBag.expired_count = _context.Expiries.ToList();






            DateTime _date;



            string _date_today;
            _date_today = DateTime.Now.ToString("dd-MM-yyyy");


            var all_items = _context.Expiries.ToList();

            List<Expiries> expire = new List<Expiries>();
            foreach (var x in all_items)
            {
                int day;
                int month;
                int year;
                int current_day;
                int current_month;
                int current_year;
                _date = DateTime.Parse(x.Expiry_date);
                day = int.Parse(_date.ToString("dd"));
                month = int.Parse(_date.ToString("MM"));
                year = int.Parse(_date.ToString("yyyy"));

                current_day = int.Parse(DateTime.Now.ToString("dd"));
                current_month = int.Parse(DateTime.Now.ToString("MM"));
                current_year = int.Parse(DateTime.Now.ToString("yyyy"));
                DateTime start = new DateTime(current_year, current_month, current_day);
                DateTime end = new DateTime(year, month, day);
                int diff = (start - end).Days;
                if (diff > 0)
                {
                    expire.Add(x);
                }


            }

            ViewBag.count_expired = expire.Count();
            ViewBag.count_all = _context.Expiries.Count();
            ViewBag.expired_list = expire;






            return View();
        }
        public void expiries_set()
        {
            DateTime _date;



            string _date_today;
            _date_today = DateTime.Now.ToString("dd-MM-yyyy");


            var all_items = _context.Expiries.ToList();

            List<Expiries> expire = new List<Expiries>();
            foreach (var x in all_items)
            {
                int day;
                int month;
                int year;
                int current_day;
                int current_month;
                int current_year;
                _date = DateTime.Parse(x.Expiry_date);
                day = int.Parse(_date.ToString("dd"));
                month = int.Parse(_date.ToString("MM"));
                year = int.Parse(_date.ToString("yyyy"));

                current_day = int.Parse(DateTime.Now.ToString("dd"));
                current_month = int.Parse(DateTime.Now.ToString("MM"));
                current_year = int.Parse(DateTime.Now.ToString("yyyy"));
                DateTime start = new DateTime(current_year, current_month, current_day);
                DateTime end = new DateTime(year, month, day);
                int diff = (start - end).Days;
                if (diff > 0)
                {
                    expire.Add(x);
                }


            }


            ViewBag.expired_list = expire.Count();
        }

        public IActionResult dispose_single_stock(int id)
        {

            var itemToRemove = _context.Expiries.SingleOrDefault(x => x.id == id); //returns a single item.
            var nameOfVictim = itemToRemove.Item_name;
            if (itemToRemove != null)
            {
                _context.Expiries.Remove(itemToRemove);
                _context.SaveChanges();
                TempData["popup"] = "1";
                TempData["message"] = "You have successfully disposed: " + nameOfVictim;
            }
            return Redirect("~/home/expiries");
        }
        public IActionResult Add_expiry_details(string item_name, string Expiry_date)
        {
            Expiries x = new Expiries()
            {
                Item_name = item_name,
                Date_created = DateTime.Now.ToString("dd/mm/yyyy"),
                Expiry_date = Expiry_date
            };
            _context.Add(x);
            _context.SaveChanges();
            return Redirect("~/home/expiries");
        }
        public IActionResult Debtors()
        {
            ViewBag.allBrands_0 = _context.Shop_items.ToList();
            ViewBag.all_debtors = _context.Creditors.ToList();
            ViewBag.count_debtors = _context.Creditors.Count();
            ViewBag.sum_of_debts = _context.Creditors.Sum(x => x.Credit);
            return View();
        }

        public IActionResult Creditors(int id)
        {
            ViewBag.history = _context.Credits.Where(x => x.Client_id == id).ToList();
            var x = _context.Creditors.FirstOrDefault(x => x.id == id);
            ViewBag.debtors_name = x.Customer_name;
            ViewBag.id = id;
            ViewBag.credit = x.Credit;
            ViewBag.count_debtors = _context.Creditors.Count();
            ViewBag.p_history = _context.Payment_history.ToList();



            return View();
        }
        [HttpPost]

        public IActionResult add_supliers(string Company_name,string Phone,string location)
        {
            Suppliers x = new Suppliers() { 
            
            Company_name=Company_name,
            Phone=Phone,
            Location=location
            
            };

            _context.Add(x);
            _context.SaveChanges();
            TempData["popup"] = "1";
            TempData["message"] = "Supplier successfully added!";
            return Redirect("~/home/supliers");

        } 
        public IActionResult supliers()
        {
            ViewBag.suppliers = _context.Supliers.ToList();
            ViewBag.supliers_count = _context.Supliers.Count();
            return View();
        }
        public IActionResult delete_sup(int id)
        {
            var item_to_remove = _context.Supliers.SingleOrDefault(x => x.id == id); //returns a single item.
            var nameOfVictim = item_to_remove.Company_name;
            if (item_to_remove != null)
            {
                _context.Supliers.Remove(item_to_remove);
                _context.SaveChanges();
                TempData["popup"] = "1";
                TempData["message"] = nameOfVictim + "  successfully removed from the system!";
                return Redirect("~/home/supliers");
            }
            else
            {
                TempData["popup"] = "2";
                TempData["message"] = "Not found";
                return Redirect("~/home/supliers");
            }
          
        }
        [HttpPost]
        public IActionResult mod_account(string f_names, string phone,string new_pass, string shop_name, int id)
        {
            var records = _context.Log_in.FirstOrDefault(x => x.id == id);
            records.Full_name = f_names;
            records.Password = new_pass;
            records.Phone = phone;
            records.Shop_name = shop_name;
            _context.Entry(records).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            TempData["popup"] = "1";
            TempData["message"] = "Account details successfully updated!";
            return Redirect("~/log_in/log_in");


        }
        public IActionResult payment(float ammount, int id)
        {
            var initial_total = _context.Creditors.FirstOrDefault(x => x.id == id);
            var init = initial_total.Credit;
            var new_total = init - ammount;
            initial_total.Credit = new_total;
            _context.Entry(initial_total).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            TempData["popup"] = "1";
            TempData["message"] = "Payment updated successfully";
            var date = DateTime.Now;
            Payment_history x = new Payment_history
            {
                Client_id = id,
                Ammount_paid = ammount,
                Balance = new_total,
                Date_created = date.ToString()
            };
            _context.Add(x);
            _context.SaveChanges();

            return RedirectToAction("creditors", "Home", new { id = id });


        }
        public IActionResult dele_sold(int id)
        {
            var itemToRemove = _context.sold_items.SingleOrDefault(x => x.id == id); //returns a single item.
          
            if (itemToRemove != null)
            {
                _context.sold_items.Remove(itemToRemove);
                _context.SaveChanges();
            }
            TempData["popup"] = "1";
            TempData["message"] = "Sales deleted successfully! NB:Remember to update your stock!";
            return Redirect("~/home/admin");

        }
        public IActionResult delete_account(int id)
        {
            var itemToRemove = _context.Creditors.SingleOrDefault(x => x.id == id); //returns a single item.
            var nameOfVictim = itemToRemove.Customer_name;
            if (itemToRemove != null)
            {
                _context.Creditors.Remove(itemToRemove);
                _context.SaveChanges();
            }
            var itemToRemove2 = _context.Credits.Where(x => x.Client_id == id); //returns a single item.

            if (itemToRemove != null)
            {
                _context.Credits.RemoveRange(itemToRemove2);
                _context.SaveChanges();
            }
            var itemToRemove3 = _context.Payment_history.Where(x => x.Client_id == id); //returns a single item.

            if (itemToRemove3 != null)
            {
                _context.Payment_history.RemoveRange(itemToRemove3);
                _context.SaveChanges();
            }




            TempData["popup"] = "1";
            //TempData["popup"] = "2";
            //TempData["popup"] = "Successfully working!";
            TempData["message"] = nameOfVictim + " has been successfully removed from the system!";

            return Redirect("~/home/debtors");

        }

        public IActionResult _add_debt(int id, String item, String quantity, string date, float total)
        {
            var xx = _context.Creditors.FirstOrDefault(x => x.id == id);

            Credits x = new Credits
            {
                Item = item,
                Quantity = quantity,
                Date_created = date,
                Client_id = id,
                Total = total,


            };
            //var yy = _context.Creditors.FirstOrDefault(x => x.id == id);
            _context.Add(x);
            _context.SaveChanges();
            //UPDATE INITIAL CASH
            var before_ = _context.Creditors.FirstOrDefault(x => x.id == id);
            float initial_cash = before_.Credit;
            float new_cash = initial_cash + total;
            before_.Credit = new_cash;
            _context.Entry(before_).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            TempData["popup"] = "1";
            TempData["message"] = "You have successfully top upd credit for :" + xx.Customer_name + " from the system";

            return RedirectToAction("creditors", "Home", new { id = id });
        }

        [HttpPost]
        public IActionResult sell_Item(int id_finish, float quantity_sold, float submit_price, float Total_cash_made, string date)
        {
            DateTime date_ = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, E_Africa_standard_time);

         

            var check_if_exists = _context.sold_items.FirstOrDefault(x => x.Item_id.ToString() == id_finish.ToString() && x.DateTime == today);
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
                    check_if_exists.DateTime =today;
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
                    total = "Sales have been done successfully sent to your printer."
                };
                //LETS FIND TOTAL COST PRICE PER ITEM
                var check_if_exists1 = _context.sold_items.FirstOrDefault(x => x.Item_id.ToString() == id_finish.ToString() && x.DateTime == date);
                float final_cost_price;
                if (check_if_exists1 == null)
                {
                    float initial_cost_price_total = 0;
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
                    DateTime = today


                };
                _context.Add(x);
                _context.SaveChanges();
                return Json(person);

            }




        }
        public IActionResult set_printer(string installedPrinterName) { 

 CookieOptions option = new CookieOptions();  

 
   option.Expires = DateTime.Now.AddMonths(300);  
   
   Response.Cookies.Append("printer_name", installedPrinterName, option);
            return Redirect("/home/attendant");
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

        public IActionResult modify_stock(int mod_id, float mod_stock,string mod_name,float mod_stock_price,float mod_item_price)
        {
            var rest = _context.Shop_items.FirstOrDefault(x => x.id == mod_id);

            
            rest.Quantity = mod_stock;
            rest.Cost_price = mod_stock_price;
            rest.DateTime = today;
            rest.Item_name = mod_name;
            rest.Item_price = mod_item_price;
            //db.Entry(payment).State = EntityState.Modified;
            _context.Entry(rest).State = EntityState.Modified;
            _context.SaveChanges();
            TempData["popup"] = "8";
            //TempData["popup"] = "2";
            //TempData["popup"] = "Successfully working!";
          
            return Redirect("~/home/admin");
            

        }
        public IActionResult refresh()
        {
            TempData["popup"] = "11";
            //TempData["popup"] = "2";
            //TempData["popup"] = "Successfully working!";

            return Redirect("~/home/attendant");
        }
     public IActionResult restock(int id, int ammount,float cost_price_restock,string supplier)
        {

            var rest = _context.Shop_items.FirstOrDefault(x => x.id == id);
            float initialStock = rest.Quantity;
            var new_stock = initialStock + ammount;
            rest.Quantity = new_stock;
            rest.Cost_price = cost_price_restock;
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
                quantity = ammount,
                Supplier=supplier,
            };
            _context.Add(insert_new);
            _context.SaveChanges();
            TempData["popup"] = "1";
            TempData["message"] = "You have successfully added stock to " + rest.Item_name + " from: " + initialStock + " to:" + new_stock;
            return Redirect("~/home/admin");
        }
        public IActionResult change_price(int change_price_id, int new_price)
        {
            var rest = _context.Shop_items.FirstOrDefault(x => x.id == change_price_id);
            var initial_price = rest.Item_price;
            rest.Item_price = new_price;
            _context.Entry(rest).State = EntityState.Modified;
            _context.SaveChanges();
            var date = DateTime.Now.ToString();
            TempData["popup"] = "1";
            TempData["message"] = "You have successfully changed price of:" + rest.Item_name + " from: " + initial_price + " to:" + new_price;
            return Redirect("~/home/admin");
        }



        public class Total_cash_made
        {

            public string total { get; set; }
        }


        public IActionResult pdf_test()
        {
            return View();
        } 
        public IActionResult Print_command()
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

            ViewBag.supliers_count = _context.Supliers.Count();

            List<Suppliers> x = new List<Suppliers>();
            //bind dropdown
            x = _context.Supliers.ToList();
            x.Insert(0, new Suppliers { id = 0, Company_name = "--Select supplier--" });
            ViewBag.Drop_sulier = x;

            expiries_set();
            //LETS CALCULATE SHOP NET WORTH
            var get_all = _context.Shop_items.Where(x=>x.Quantity>0);
            int i, j;
            float sum=0, total;
            j = get_all.Count();
            foreach(var item in get_all)
                                        {

                sum = sum + (item.Quantity * item.Item_price);
                ViewBag.Net_worth = sum;
            }

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
                                pd.id,
                                pd.Item_id
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
                JoinObject.Item_id = item.Item_id.ToString();
                JoinObject.id = item.id;
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
            ViewBag.id = HttpContext.Session.GetString("id");

            ViewBag.count_below = _context.Shop_items.Count(x => x.Quantity <= 0);
            ViewBag.to_restock = _context.Shop_items.Where(x => x.Quantity <= 0);
            ViewBag.count_all = _context.Shop_items.Sum(x => x.Quantity);
            var sold = _context.sold_items.ToList();
            if (sold.Count() == 0)
            {

                ViewBag.sold = _context.sold_items.Where(x => x.DateTime == today).Sum(x => x.quantity_sold);



            }
            else
            {
                ViewBag.sold = _context.sold_items.Where(x => x.DateTime == today).Sum(x => x.quantity_sold);

            }

            ViewBag.sold_general = _context.sold_items;
            ViewBag.shop_name = HttpContext.Session.GetString("shop_name");
            ViewBag.name = HttpContext.Session.GetString("Name");
            ViewBag.id = HttpContext.Session.GetString("id");
            var phone = HttpContext.Session.GetString("phone");
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
                               pd.quantity,
                               od.id,
                               pd.Supplier,
                           }).ToList();

            foreach (var item in results)
            {
                join_tables JoinObject = new join_tables();
                JoinObject.Item_id = item.id.ToString();
                JoinObject.Item_name = item.Item_name;
                JoinObject.Item_price = item.Item_price;
                JoinObject.new_quanity = item.new_quanity;
                JoinObject.Prev_quantity = item.Prev_quantity;
                JoinObject.quantity = item.quantity;
                JoinObject.Supplier = item.Supplier;
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
           
            var count_below = _context.Shop_items.Where(x => x.Quantity < 0).Count();
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


        public IActionResult Filter([Optional] string date)
        {

            DateTime _date;
            string day;
          
            if (date== null)
            {
                day = today1.ToString();
            }
            else {


                _date = DateTime.Parse(date);
                day = _date.ToString("dd/MM/yyyy");
            }



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
                                     pd.Total_Cost_cash
                                 }).ToList();

            foreach (var item in results_third)
            {
                join_sold_ite_filtered JoinObject_third = new join_sold_ite_filtered();

                JoinObject_third.Item_name = item.Item_name;
                JoinObject_third.DateTime = item.DateTime;
                JoinObject_third.quantity_sold = item.quantity_sold;
                JoinObject_third.Total_cash_made = item.Total_cash_made;
                JoinObject_third.Total_Cost_cash = item.Total_Cost_cash;
                JoinObject_third.Item_price = item.Item_price;
                JoinObject_third.Item_name = item.Item_name;
                joinList_third.Add(JoinObject_third);
            }
            var JoinListToViewbag_third = joinList_third.ToList();
            var count = joinList_third.Count();
            var sum_of_cash = JoinListToViewbag_third.Sum(x => x.Total_cash_made);

            //TempData["popup"] = "2";
         
        ViewBag.message ="Date: "+day+" Found:" +count + " records, total cash made: " + sum_of_cash+"/=.";
            //TempData["total"] = sum_of_cash;


            ViewBag.JoinList_general_third = JoinListToViewbag_third;
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
        public IActionResult add_debtor(String Full_name, int Phone,float credit)
        {
            //LETS JOIN TABLES















            var check_if_item_exists1 = _context.Creditors.SingleOrDefault(x => x.Customer_name == Full_name || x.Phone_number==Phone);
            if (check_if_item_exists1 != null)
            {

                TempData["popup"] = "2";
                //TempData["popup"] = "2";
                //TempData["popup"] = "Successfully working!";
                TempData["message"] = Full_name + " already exists in the system!";
                return Redirect("~/home/creditors_account");

            }
            else
            {
                var shop = _context.Log_in.FirstOrDefault(x => x.strRole == 1);

                Creditors_account client = new Creditors_account
                {
                    Customer_name = Full_name,
                    Phone_number = Phone,
                    Credit = credit,
                    Date_created = DateTime.Now.ToString(),

                };
                _context.Add(client);
                _context.SaveChanges();

                TempData["popup"] = "1";
                //TempData["popup"] = "2";
                //TempData["popup"] = "Successfully working!";
                TempData["message"] = Full_name + " successfully added to the system!";
            }

            return Redirect("~/home/debtors");

        }
        public IActionResult add_item(String Item_name,float cost_price, float Item_price, int Quantity, String date)
        {
            var check_if_item_exists = _context.Shop_items.SingleOrDefault(x => x.Item_name == Item_name);
            if (check_if_item_exists == null)
            {
                shop_items add_new_product = new shop_items
                {
                    Item_name = Item_name,
                    Item_price = Item_price,
                    Quantity = Quantity,
                    DateTime = date,
                    Cost_price=cost_price
                    

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
