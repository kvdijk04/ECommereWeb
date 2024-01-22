using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Models;
using PagedList.Core;
using System.Collections.Generic;
using System.Linq;
using System;
using OnlineMarket.Others;
using Newtonsoft.Json.Linq;
using OnlineMarket.ModelViews;
using OnlineMarket.Extension;

namespace OnlineMarket.Controllers
{
    public class MoMoController : Controller
    {
        private readonly OnlineShopContext _context;
        public INotyfService _notyfService { get; }
        public MoMoController(OnlineShopContext context, INotyfService notifyfService)
        {
            _context = context;
            _notyfService = notifyfService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Payment(int id)
        {
            //HttpContext.Session.Set<List<CartItem>>("GioHang", cart);

            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOF2FF20220326";
            string accessKey = "5B9WlRDysPguI40l";
            string serectkey = "8egsq1LLx17aOpOIYpm5jfV4W0FaMdWV";
            string orderInfo = "webshoe";
            string returnUrl = "https://localhost:44379/success-momo.html";
            string notifyurl = "http://ba1adf48beba.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test
            string amount = "1000";
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "amount",amount},
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i
        public ActionResult ConfirmPaymentClient()
        {
            //hiển thị thông báo cho người dùng
            return View();
        }


        [HttpPost]
        public void SavePayment()
        {
            

        }
    }
}
