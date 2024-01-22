using Microsoft.AspNetCore.Mvc;
using OnlineMarket.Models;
using OnlineMarket.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMarket.Controllers
{
    public class SitemapController : Controller
    {
        private readonly OnlineShopContext _context;
        public SitemapController(OnlineShopContext context)
        {
            _context = context;
        }
        public string GetHost()
        {
            return $"{(Request.IsHttps ? "https":"http")}://{Request.Host.ToString()}";
        }
        public string baseUrl = "http://kvandijk04-001-site1.dtempurl.com/";
        [Route("Sitemap.xml")]
        public IActionResult Index()
        {
            string baseUrl = GetHost();
            List<string> ls = new List<string>();
            //thêm các danh sách sitemap
            ls.Add(baseUrl + "/Sitemap-product.xml");
            ls.Add(baseUrl + "/Sitemap-posts.xml");
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<?xml version=\'1.0\' encoding=\'UTF-8\'?>");
            stringBuilder.AppendLine("<sitemapindex xmlns =\'http://www.sitemaps.org/schemas/sitemap/0.9'>");
            foreach(var item in ls)
            {
                string link = "<loc>" + item + "</loc>";
                stringBuilder.AppendLine("<sitemap>");
                stringBuilder.AppendLine(link);
                stringBuilder.AppendLine("<lastmod>" + DateTime.Now.ToString("MMMM-dd-yyyy HH:mm:ss tt") + "</lastmod>");
                stringBuilder.AppendLine("</sitemap>");
            }
            stringBuilder.AppendLine("</sitemapindex>");
            return Content(stringBuilder.ToString(),"text/xml" , Encoding.UTF8);
        }
        [Route("/Sitemap-product.xml")]
        public IActionResult SiteMapProduct()
        {
            var lsproduct = _context.Products.Where(x => x.Active == true).ToList();
            var sitemapBuilder = new SitemapBuilder();
            sitemapBuilder.AddUrl(baseUrl, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 1.0);
            foreach( var p in lsproduct)
            {
                sitemapBuilder.AddUrl(GetHost() + p.Alias, modified:DateTime.UtcNow, changeFrequency: null, priority: 0.9);
            }
            string xml = sitemapBuilder.ToString();
            return Content(xml, "text/xml");
        }
        [Route("/Sitemap-posts.xml")]
        public IActionResult SiteMapPost()
        {
            var lsproduct = _context.Posts.Where(x => x.Published == true).ToList();
            var sitemapBuilder = new SitemapBuilder();
            sitemapBuilder.AddUrl(baseUrl, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 1.0);
            foreach (var p in lsproduct)
            {
                sitemapBuilder.AddUrl(GetHost() + p.Alias, modified: DateTime.UtcNow, changeFrequency: null, priority: 0.8);
            }
            string xml = sitemapBuilder.ToString();
            return Content(xml, "text/xml");
        }
    }
}
