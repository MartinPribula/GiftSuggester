using MvcApplication4.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to the place witch will help you please your beloved ones.";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public String user;

        public ActionResult Login()
        {
            if ((Session["LogedUserId"] == null))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LogingModel u)
        {        
            if((Session["LogedUserId"] == null))
            {   
                if (ModelState.IsValid) 
                {
                    using (UserEntities dc = new UserEntities())
                    {
                        u.password = HashPassword(u.password);
                        var v = dc.Users.Where(a => a.userName.Equals(u.userName) && a.password.Equals(u.password)).FirstOrDefault();
                        if (v != null)
                        {
                            Session["LogedUserID"] = v.id.ToString();
                            Session["LogedUsername"] = v.userName.ToString();
                            Session["LogedPassword"] = v.password.ToString();
                            return RedirectToAction("AfterLogin");
                        }
                    }
                }
            }
            return View(u);
        }


        public ActionResult AfterLogin()
        {
            if (Session["LogedUserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Register()
        {
            return View();
            
        }

        public string HashPassword(string password)
        {
            byte[] passwordInByte = StringToByte(password);
            byte[] hashedPasswordInByte;
            using (SHA256 shaM = new SHA256Managed())
            {
                hashedPasswordInByte = shaM.ComputeHash(passwordInByte);
            }
            return BytesToString(hashedPasswordInByte); 
        }

        static byte[] StringToByte(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string BytesToString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(MvcApplication4.Models.User u)
        {
            if (ModelState.IsValid)
            {
                using (UserEntities dc = new UserEntities())
                {                  
                    Session["LogedUserID"] = u.id.ToString();
                    Session["LogedUsername"] = u.userName.ToString();
                    u.password = HashPassword(u.password);
                    Session["LogedPassword"] = u.password.ToString();
                    dc.Users.Add(u);
                    dc.SaveChanges();
                    ModelState.Clear();
                    u = null;
                    return RedirectToAction("AfterLogin");
                }              
            }
            return View(u);
        }

        public ActionResult LogOff()
        {
            if (Session["LogedUserId"] != null)
            {
                Session.RemoveAll();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult Manage()
        {
            if (Session["LogedUserId"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ManageModel u)
        {
            if (ModelState.IsValid)
            {
                if (Session["LogedUserID"] != null)
                {
                    using (UserEntities dc = new UserEntities())
                    {
                        u.password = HashPassword(u.password);
                        if (u.password.ToString().Equals((String)Session["LogedPassword"]))
                        {
                            int id = Convert.ToInt32(Session["LogedUserId"]);
                            var oldUser = dc.Users.Where(model => model.id.Equals(id)).FirstOrDefault();
                            u.NewPassword = HashPassword(u.NewPassword);
                            oldUser.password = u.NewPassword.ToString();
                            dc.SaveChanges();
                            ModelState.Clear();
                            u = null;
                            ViewBag.Message = "Your password has been updated.";
                            return View();
                        }
                        else
                        {
                            ViewBag.Message = "Wrong password for user " + Session["LogedUsername"];
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(u);
        }


        public List<Gift> GetPossibleGifts(Suggestion s)
        {
            GifterEntities entities = new GifterEntities();
            List<Gift> possibleGifts = entities.Gifts.Where(model => model.ageFrom < s.age && model.ageTo > s.age && model.occasion.Contains(s.occasion) && model.relationship.Contains(s.relationship) && model.relationshipLengthFrom < s.relationshipLength && model.relationshipLengthTo > s.relationshipLength).ToList();
            return possibleGifts;
        }

        public List<Suggestion> GetSimilarSuggestions(Suggestion s)
        {
            GifterEntities entities = new GifterEntities();
            List<Suggestion> similarSuggestions = entities.Suggestions.Where(model => model.age.Equals(s.age) && model.occasion.Equals(s.occasion) && model.relationship.Equals(s.relationship) && model.relationshipLength.Equals(s.relationshipLength) && !(model.rating.Equals(null))).ToList();
            return similarSuggestions;
        }

        public List<Gift> RemoveGiftsWithRatingLowerThenFive(List<Suggestion> similarSuggestions, List<Gift> possibleGifts)
        {
            foreach (Suggestion similarSuggestion in similarSuggestions)
            {
                foreach (Gift possibleGift in possibleGifts)
                {
                    if (similarSuggestion.idGift.Equals(possibleGift.id))
                    {
                        if(similarSuggestion.rating < 5)
                        {
                            possibleGifts.Remove(possibleGift);
                        }
                    }
                }
            }
            return possibleGifts;
        }


        public List<Gift> GetGiftstWithHighestRating(List<Suggestion> similarSuggestions)
        {
            int maxRating = 0;
            GifterEntities entities = new GifterEntities();
            List<Gift> highestRatingGifts = new List<Gift>();
            foreach(Suggestion suggestion in similarSuggestions)
            {
                if (suggestion.rating == maxRating)
                {
                        highestRatingGifts.Add(entities.Gifts.Where(model => model.id.Equals(suggestion.idGift)).FirstOrDefault());
                }
                if (maxRating < suggestion.rating)
                {
                    maxRating = (int)suggestion.rating;
                    highestRatingGifts.Clear();
                    highestRatingGifts.Add(entities.Gifts.Where(model => model.id.Equals(suggestion.idGift)).FirstOrDefault());

                }
            }
            return highestRatingGifts;
        }

        public ActionResult Suggest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Suggest(Suggestion s)
        {
            GifterEntities entities = new GifterEntities();
            Gift gift = entities.Gifts.Where(model => model.ageFrom < s.age && model.ageTo > s.age && model.occasion.Contains(s.occasion) && model.relationship.Contains(s.relationship) && model.relationshipLengthFrom < s.relationshipLength && model.relationshipLengthTo > s.relationshipLength).FirstOrDefault();
            List<Gift> possibleGifts = GetPossibleGifts(s);
            if (possibleGifts == null)
            {
                return AfterSuggest(s);
            }
            else
            {
                List<Suggestion> similarSuggestions = GetSimilarSuggestions(s);
                List<Gift> highestRatingGifts = GetGiftstWithHighestRating(similarSuggestions);
                possibleGifts = RemoveGiftsWithRatingLowerThenFive(similarSuggestions, possibleGifts);      
                
                s.idGift = gift.id;
                if (Session["LogedUserID"] != null)
                {
                    s.idUser = Convert.ToInt32(Session["LogedUserID"]);                 
                }
                entities.Suggestions.Add(s);
                try
                {
                    entities.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
                return RedirectToAction("AfterSuggest", s);
            }
        }

        [HttpGet]
        public ActionResult SuggestToRecipient(int id)
        {
            if (Session["LogedUserID"] != null)
            {
                GifterEntities entities = new GifterEntities();
                Suggestion s = new Suggestion();
                Recipient r = entities.Recipients.Where(model => model.id.Equals(id)).FirstOrDefault();
                if (r.idUser == Convert.ToInt32(Session["LogedUserID"]))
                {
                    s.age = r.age;
                    s.relationship = r.relationship;
                    s.relationshipLength = r.relationshipLength;
                    s.idRecipient = r.id;
                    return View(s);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuggestToRecipient(Suggestion s)
        {
            GifterEntities entities = new GifterEntities();
            Gift gift = entities.Gifts.Where(model => model.ageFrom < s.age && model.ageTo > s.age && model.occasion.Contains(s.occasion) && model.relationship.Contains(s.relationship) && model.relationshipLengthFrom < s.relationshipLength && model.relationshipLengthTo > s.relationshipLength).FirstOrDefault();
            if (gift == null)
            {
                return View(s);
            }
            else
            {
                s.idGift = gift.id;
                if (Session["LogedUserID"] != null)
                {
                    s.idUser = Convert.ToInt32(Session["LogedUserID"]);
                }
                if (s.idRecipient == null)
                {
                    return RedirectToAction("Index");
                }
                s.timeStamp = DateTime.Now;
                entities.Suggestions.Add(s);         
                entities.SaveChanges();
                return RedirectToAction("AfterSuggest", s);
            }
        }

        [HttpGet]
        public ActionResult AfterSuggest(Suggestion s)
        {
            GifterEntities entities = new GifterEntities();
            Gift gift;
            if (s.relationship == "girlfriend" && s.relationshipLength >= 3)
            {
                gift = entities.Gifts.Where(model => model.id.Equals(2)).FirstOrDefault();
                ViewBag.Message = "Dating for that long? Time to put the ring on it.";
            }
            else
            {
                gift = entities.Gifts.Where(model => model.id.Equals(1)).FirstOrDefault();
                ViewBag.Message = "Flowers, you shuld buy her flowers, that never spoiled anything";
            }
            ViewBag.Title = "You should buy her " + gift.name;
            return View();
        }


        public ActionResult MyRecipients()
        {
            if (Session["LogedUserID"] != null)
            {

                var entities = new RecipientEntities();

                return View(entities.Recipients.ToList());
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult CreateRelationship()
        {
            if (Session["LogedUserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRelationship(Models.Recipient r)
        {
            if (ModelState.IsValid)
            {
                if (Session["LogedUserID"] != null)
                {
                    using (var entities = new RecipientEntities())
                    {
                        r.idUser = Convert.ToInt32(Session["LogedUserId"]);
                        entities.Recipients.Add(r);
                        entities.SaveChanges();
                        ModelState.Clear();
                        r = null;
                        return RedirectToAction("MyRecipients");
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeleteRelationship(int id)
        {
            if (Session["LogedUserID"] != null)
            {
                using (var entities = new RecipientEntities())
                {
                    
                    var r = entities.Recipients.Where(model => model.id.Equals(id)).FirstOrDefault();
                    if (r != null)
                    {
                        entities.Recipients.Remove(r);
                        entities.SaveChanges();
                        r = null;
                        return RedirectToAction("MyRecipients");
                    }
                    else
                    {
                        return RedirectToAction("MyRecipients");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]    
        public ActionResult EditRelationship(int id)
        {
            if (Session["LogedUserID"] != null)
            {
                using (var entities = new RecipientEntities())
                {
                    var r = entities.Recipients.Where(model => model.id.Equals(id)).FirstOrDefault();
                    if (r != null)
                    {
                        return View(r);
                    }
                    else
                    {
                        return RedirectToAction("MyRecipients");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRelationship(Models.Recipient r)
        {
            if (Session["LogedUserID"] != null)
            {
                if (r != null)
                {
                    using (var entities = new RecipientEntities())
                    {
                        var oldEntry = entities.Recipients.Where(model => model.id.Equals(r.id)).FirstOrDefault();
                        oldEntry.name = r.name;
                        oldEntry.surname = r.surname;
                        oldEntry.relationship = r.relationship;
                        oldEntry.relationshipLength = r.relationshipLength;
                        oldEntry.age = r.age;
                        entities.SaveChanges();
                        return RedirectToAction("MyRecipients");
                    }
                }
                else
                {
                    return RedirectToAction("MyRecipients");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult SheDesires()
        {
            ViewBag.Message = "What she desires";
            return View();
        }

        public ActionResult MySuggestions()
        {
            if (Session["LogedUserID"] != null)
            {
                var entities = new GifterEntities();
                return View(entities.Suggestions.ToList());
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public ActionResult AddFeedback(int id)
        {
            if (Session["LogedUserID"] != null)
            {
                GifterEntities entities = new GifterEntities();
                Suggestion sugg = entities.Suggestions.Where(model => model.id.Equals(id)).FirstOrDefault();
                if (sugg.rating == null)
                {
                    return View(sugg);
                }
                else
                {
                    return RedirectToAction("MySuggestions");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddFeedback(Suggestion s)
        {
            if (Session["LogedUserID"] != null)
            {
                    GifterEntities entities = new GifterEntities();
                    Suggestion oldSugg = entities.Suggestions.Where(model => model.id.Equals(s.id)).FirstOrDefault();
                    oldSugg.rating = s.rating;
                    entities.SaveChanges();
                    ModelState.Clear();
                    return RedirectToAction("MySuggestions");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public JsonResult doesUserNameExist(string userName)
        {
            var entities = new UserEntities();
            User user = entities.Users.Where(model => model.userName.Equals(userName)).FirstOrDefault();
            return Json(user == null);
        }

        [HttpPost]
        public JsonResult doesPasswordMatch(string password)
        {
            var entities = new UserEntities();
            int userId = Convert.ToInt32(Session["LogedUserID"]);
            password = HashPassword(password);
            User user = entities.Users.Where(model => model.id.Equals(userId) && model.password.Equals(password)).FirstOrDefault();
            return Json(user != null);
        }

        [HttpGet]
        public ActionResult SuggestionsForRecipient(int id)
        {
            if (Session["LogedUserID"] != null)
            {
                int uId = Convert.ToInt32(Session["LogedUserID"]);
                var entities = new GifterEntities();

                Recipient r = entities.Recipients.Where(m => m.id.Equals(id) && m.idUser.Equals(uId)).FirstOrDefault();
                if (r != null)
                {
                    ViewBag.Id = id;
                    ViewBag.Name = r.name;
                    ViewBag.Surname = r.surname;
                    ViewBag.Relationship = r.relationship;
                    ViewBag.Age = r.age;
                    return View(entities.Suggestions.ToList());
                }
                else
                {
                    return RedirectToAction("MyRecipients");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }            
            
        }
    }
}
