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
            FourSquareClient client = new FourSquareClient("client_id", "EBBJTZ1HN2VCGSDAG4JDFYN514T1Z4KD122FNAZSMJMJRK51");
            AuthenticationResult result = client.VerifyAuthentication(this.HttpContext, new Uri("http://localhost:56709/Login"));
            if (!result.IsSuccessful)
                client.RequestAuthentication(this.HttpContext,new Uri(Request.Url.AbsoluteUri));
            else{
                ViewBag.Message = JsonConvert.SerializeObject(result.ExtraData);
            }
            return View();
        }
	}
}