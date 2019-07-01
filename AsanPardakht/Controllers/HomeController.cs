using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AsanPardakht.Token;
using Fluentx.Mvc;

namespace AsanPardakht.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RedirectToBank()
        {
            var client = new TokensClient();

            long TotalPrice = 150000;

            var bankid = "Create Random Text To trace Invoice";
            var tokenResp = client.MakeSpecialToken(TotalPrice.ToString(), "Your Merch Id", bankid, "", "", "Call back url", "Custom Text like Buy from IranKish", "", "", "", "ASANSHP;LC;3");

            var datacollection = new Dictionary<string, object>
            {
                {"token",tokenResp.token },
                {"merchantId","Your Merch Id" },
                {"Amount",TotalPrice },
                {"invocreNo",bankid.ToString() }

            };
            return this.RedirectAndPost("https://ikc.shaparak.ir/TPayment/Payment/Index", datacollection);
        }

         [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Verify(string token, string merchantId, string resultCode, string paymentId, string referenceId, string InvoiceNumber)
        {
            try
            {
                var k = "";
                var sha1Key = "Your sha Key";

                using (var verifyService = new Verify.VerifyClient())
                {
                    if (!string.IsNullOrEmpty(resultCode) && resultCode == "100")
                    {
                        var res = verifyService.KicccPaymentsVerification(token, merchantId, referenceId, sha1Key);
                        if (res > 0)
                        {
                            
                            var x1 = verifyService.getTransaction("Your Merch Id", InvoiceNumber, "");
                            
                            ViewData["result"] = x1.RESULTCODE;
                            ViewData["paymentid"] = paymentId; // کد پیگیری
                            ViewData["referid"] = x1.REFERENCENUMBER; // رسید دیجیتال
                           
                        }
                        else
                        {
                            //  Verification Failed , your statements Goes here
                            ViewData["result"] = "خطا";
                            ViewData["paymentid"] = "";
                            ViewData["referid"] = "";
                        }
                    }
                }

               
            }
            catch (Exception e)
            {
               
                ViewData["err"] = "بروز خطا . لطفا دوباره تلاش کنید";

            }
           


            return View();
        }


        public ActionResult About()
        {
            return View();
        }

       
    }
}