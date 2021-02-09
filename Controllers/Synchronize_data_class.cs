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
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
namespace SHOP.Controllers
{
    [Authorize]

    public class Synchronize_data : Controller
    {
        private readonly ApplicationDBContext_online _context_online;
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string today = DateTime.Now.ToString("dd/MM/yyyy");


        public Synchronize_data(ApplicationDBContext context, ApplicationDBContext_online context_o, IWebHostEnvironment webHostEnvironment)
        {
            _context_online = context_o;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "2")]
        public IActionResult sync_data(string JSON_signal)
        {

            //if (IsConnectedToInternet())
            //{
            //DELETE LOG IN
            var _data = _context_online.Log_in.ToList();
            _data.ForEach(x =>
            {
                _context_online.Entry(x).State = EntityState.Deleted;
            });
            _context_online.SaveChanges();
            //DELETE SHOP ITEMS
            var _data1 = _context_online.Shop_items.ToList();
            _data1.ForEach(x =>
            {
                _context_online.Entry(x).State = EntityState.Deleted;
            });
            _context_online.SaveChanges();
            //DELETE SHOP RESTOCK HISTORY
            var _data2 = _context_online.Restock_history.ToList();
            _data2.ForEach(x =>
            {
                _context_online.Entry(x).State = EntityState.Deleted;
            });
            _context_online.SaveChanges();

            //DELETE SHOP SOLD ITEMS
            var _data3 = _context_online.sold_items.ToList();
            _data3.ForEach(x =>
            {
                _context_online.Entry(x).State = EntityState.Deleted;
            });
            _context_online.SaveChanges();

            //DELETE SHOP CREDITORS
            var _data4 = _context_online.Creditors.ToList();
            _data4.ForEach(x =>
            {
                _context_online.Entry(x).State = EntityState.Deleted;
            });
            _context_online.SaveChanges();

            //DELETE SHOP CREDITS
            var _data5 = _context_online.Credits.ToList();
            _data5.ForEach(x =>
            {
                _context_online.Entry(x).State = EntityState.Deleted;
            });
            _context_online.SaveChanges();

            //DELETE SHOP PAYMENT HISTORY
            var _data6 = _context_online.Payment_history.ToList();
            _data6.ForEach(x =>
            {
                _context_online.Entry(x).State = EntityState.Deleted;
            });
            _context_online.SaveChanges();

            //DELETE SHOP EXPIRIES
            var _data7 = _context_online.Expiries.ToList();
            _data7.ForEach(x =>
            {
                _context_online.Entry(x).State = EntityState.Deleted;
            });
            _context_online.SaveChanges();

            //______________________________________________________________
            //COPYING DATA FROM LOCAL DATABASE TO ONLINE DATABASE


            List<log_in> log_in_add = _context.Log_in.ToList();
            List<shop_items> shop_items_add = _context.Shop_items.ToList();
            List<Restock_history> restock_history_add = _context.Restock_history.ToList();
            List<sold_items> sold_items_add = _context.sold_items.ToList();
            List<Creditors_account> creditors_add = _context.Creditors.ToList();
            List<Credits> credit_add = _context.Credits.ToList();
            List<Payment_history> payment_history_add = _context.Payment_history.ToList();
            List<Expiries> Expiries_add = _context.Expiries.ToList();

            //______________________________________________________________
            //SYNCHRONIZING DATA TO ONLINE DATABASE

            foreach (var i in log_in_add)
            {
                _context_online.Log_in.Add(i);
            }
            _context_online.SaveChanges();

            foreach (var i in shop_items_add)
            {
                _context_online.Shop_items.Add(i);
            }
            _context_online.SaveChanges();

            foreach (var i in restock_history_add)
            {
                _context_online.Restock_history.Add(i);
            }
            _context_online.SaveChanges();

            foreach (var i in sold_items_add)
            {
                _context_online.sold_items.Add(i);
            }
            _context_online.SaveChanges();
            foreach (var i in creditors_add)
            {
                _context_online.Creditors.Add(i);
            }
            _context_online.SaveChanges();
            foreach (var i in credit_add)
            {
                _context_online.Credits.Add(i);
            }
            _context_online.SaveChanges();
            foreach (var i in Expiries_add)
            {
                _context_online.Expiries.Add(i);
            }
            _context_online.SaveChanges();


            TempData["popup"] = "1";
            //TempData["popup"] = "2";
            //TempData["popup"] = "Successfully working!";
            TempData["message"] = "Data has been synchronized successfully";
            return Redirect("~/Home/attendant");


        }

    



       
        public class JSON_reply
        {

            public string message { get; set; }
        }
        public bool IsConnectedToInternet()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
        private bool ping()
        {
            System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingReply reply = pingSender.Send("http://www.google.com");
            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}