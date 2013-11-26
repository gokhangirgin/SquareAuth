using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SquareAuth.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {
            FourSquareClient client = new FourSquareClient("client_id", "client_secret");
            AuthenticationResult result = client.VerifyAuthentication(this.HttpContext, new Uri(Request.Url.AbsoluteUri));
            if (!result.IsSuccessful)
                client.RequestAuthentication(this.HttpContext,new Uri(Request.Url.AbsoluteUri));
            else{
                ViewBag.Message = JsonConvert.SerializeObject(result.ExtraData);
            }
            return View();
        }
	}
}