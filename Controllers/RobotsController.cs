using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMarket.Controllers
{
    public class RobotsController : Controller
    {


        [Route("/robots.txt")]

        public ContentResult RobotsTxt()
        {

            var sb = new StringBuilder();
            sb.AppendLine("User-agent: *")
                .AppendLine("Disallow: /")
                .AppendLine("Disallow: /Admin")
                .AppendLine("Disallow: /cart.html")
                .AppendLine("Disallow: /checkout.html")
                .AppendLine("Disallow: /tai-khoan-cua-toi.html")
                .AppendLine("Disallow: /collections/*+*")
                .AppendLine("Disallow: /collections/*%2B*")
                .AppendLine("Disallow: /collections/*%2b*")
                .AppendLine("Disallow: /Blogs/*+*")
                .AppendLine("Disallow: /Blogs/*%2B*")
                .AppendLine("Disallow: /Blogs/*%2b*")
                .Append("Sitemap: ")
                .Append(this.Request.Scheme)
                .Append("://")
                .Append(this.Request.Host)
                .AppendLine("/Sitemap.xml")
                .AppendLine("User-agent: adsbot-google")
                .AppendLine("Disallow: /cart.html")
                .AppendLine("Disallow: /checkout.html")
                .AppendLine("User-agent: Nutch")
                .AppendLine("Disallow: /")
                .AppendLine("User-agent: MJ12bot")
                .AppendLine("Crawl-delay: 10")
                .AppendLine("User-agent: Twitterbot")
                .AppendLine("Disallow: ")
                .AppendLine("User-agent: Pinterest")
                .AppendLine("Crawl-delay: 1");

            


            return this.Content(sb.ToString(), "text/plain", Encoding.UTF8);
        }
    }
}
