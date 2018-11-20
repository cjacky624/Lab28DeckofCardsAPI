using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CreateDeck()
        {
            string deck_id;
            HttpWebRequest request = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:64.0) Gecko/20100101 Firefox/64.0";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string output = reader.ReadToEnd();
                JObject Jparser = JObject.Parse(output);

                if (TempData["deck_id"] == null)
                {
                    TempData["deck_id"] = Jparser["deck_id"];
                    deck_id = Jparser["deck_id"].ToString();
                }
                else
                {
                    deck_id = TempData["deck_id"].ToString();
                }
                ViewBag.Deck = deck_id;
                reader.Close();
                response.Close();
            }
            return View("Index");
        }
        public ActionResult DrawCards(string deck_id)
        {
            HttpCookie cookie;
            if (Request.Cookies["deck_id"] == null)
            {
                cookie = new HttpCookie("deck_id");
                cookie.Value = deck_id;
                Response.Cookies.Add(cookie);
            }
            else
            {
                cookie = Request.Cookies["deck_id"];
                cookie.Value = deck_id;
            }

            deck_id = cookie.Value.ToString();

            HttpWebRequest request = WebRequest.CreateHttp("https://deckofcardsapi.com/api/deck/" + deck_id + "/draw/?count=5");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:47.0) Gecko/20100101 Firefox/47.0";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string output = reader.ReadToEnd();

                JObject Jparser = JObject.Parse(output);

                Card[] cards = new Card[Jparser["cards"].Count()];
                int i = 0;
                foreach (var x in Jparser["cards"])
                {
                    cards[i] = new Card(x["image"].ToString(), x["value"].ToString(), x["suit"].ToString());
                    i++;
                }

                ViewBag.CardsInHand = cards;
                reader.Close();
                response.Close();
                return View("Index");
            }

            return View("Index");
        }
    }
}