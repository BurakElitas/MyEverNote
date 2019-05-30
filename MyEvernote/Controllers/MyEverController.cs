using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ObjectValues;
using MyEvernote.Filters;
using MyEvernote.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace MyEvernote.Controllers
{
    public class MyEverController : Controller
    {


        CategoryManager categoryManager = new CategoryManager();
        EvernoteUserManager evernoteuserManager = new EvernoteUserManager();
        NoteManager noteManager = new NoteManager();
        // GET: MyEver
        public ActionResult Index()
        {
            return View(noteManager.ListQueryable().Where(x=>x.IsDraft==false).OrderByDescending(x=>x.ModifiedOn).ToList());
        }
        public ActionResult CategoryById(int? id)
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            
            Category cat = categoryManager.Find(x=>x.Id==id.Value);

            if (cat == null)
            {
                return HttpNotFound();
            }

            if(cat.Notes.Count==0)
            {
                ViewBag.Result = "Bu kategoriye ait not bulunamadı.";
                return View("Index", cat.Notes);
            }

            return View("Index", cat.Notes.Where(x => x.IsDraft == false).OrderByDescending(x=>x.ModifiedOn).ToList());
        }

        public ActionResult MostLiked()
        {
            List<Note> mostliked = noteManager.ListQueryable().Where(x => x.IsDraft == false).OrderByDescending(x => x.LikeCount).ToList();
            return View("Index", mostliked);
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
              BusinessLayerResult<EvernoteUser> res=evernoteuserManager.LoginUser(model);
              if(res.Errors.Count>0)
                {
                   
                    foreach (MessageObject item in res.Errors)
                    {
                        ModelState.AddModelError("", item.Error);
                        
                    }
                    return View(model);
                }
              
                CurrentSession.Set<EvernoteUser>("login", res.Result);

                return RedirectToAction("Index");
            }
            return View(model);
        }


        public ActionResult Register()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if(ModelState.IsValid)
            {
               
                BusinessLayerResult<EvernoteUser> buser= evernoteuserManager.RegisterUser(model);

                if(buser.Errors.Count>0) //eğer hata var ise ekrana basıyorum ve view a geri döndürüyorum
                {
                    foreach (MessageObject item in buser.Errors)
                    {
                        ModelState.AddModelError("", item.Error);
                    }
                    return View(model);
                }

                OkViewModel ok = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl = "/MyEver/Login",
                    
                };
                ok.items.Add("Lütfen e-posta adresinize gönderdiğimiz Aktivasyon link'ine tıklayarak hesabınızı aktive ediniz.Hesabınızı active etmeden not ekleme ve not beğenme işlemleri yapamazsınız.");
                return View("Ok",ok);
            }
            return View(model);
        }

       
        public ActionResult UserActivate(Guid id)
        {
            BusinessLayerResult<EvernoteUser> res = evernoteuserManager.ActivateUser(id);
            if(res.Errors.Count>0)
            {
                TempData["errors"] = res.Errors;
                ErrorViewModel e = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    items = res.Errors
                };

                return View("Error", e);
            }


            OkViewModel ok = new OkViewModel()
            {
                Title = "Hesap Aktifleştirildi",
                RedirectingUrl = "/MyEver/Login",

            };
            ok.items.Add("Hesabınız aktifleştirildi.Artık Not paylaşımı ve Beğenme işlemleri yapabilirsiniz..");
            
            return View("Ok", ok); //hata yoksa kullanıcı kayıt olmuştur.
        }

        public ActionResult Singout()
        {
            CurrentSession.Clear();
            return RedirectToAction("Index");
        }

        [Auth]
        public ActionResult ShowProfile()
        {
            EvernoteUser currentUser = Session["login"] as EvernoteUser;
            BusinessLayerResult<EvernoteUser> res = evernoteuserManager.GetUserById(currentUser.Id);

            if(res.Errors.Count>0)
            {
                ErrorViewModel NotifyError = new ErrorViewModel()
                {
                    items = res.Errors,
                    Title = "Kullanıcı Bulunamadı",
                    RedirectingUrl = "/MyEver/Index"
                };

                return View("Error", NotifyError);
            }
            
            
            return View(res.Result);
        }
        [Auth]
        public ActionResult EditProfile()
        {
            EvernoteUser currentUser = Session["login"] as EvernoteUser;
            BusinessLayerResult<EvernoteUser> res = evernoteuserManager.GetUserById(currentUser.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel NotifyError = new ErrorViewModel()
                {
                    items = res.Errors,
                    Title = "Kullanıcı Bulunamadı",
                    RedirectingUrl = "/MyEver/ShowProfile"
                };

                return View("Error", NotifyError);
            }
            return View(res.Result); 
        }
        
        [HttpPost]
        [Auth]
        public ActionResult EditProfile(EvernoteUser model,HttpPostedFileBase ProfileImage)
        {
            ModelState.Remove("ModidiedUsername");
            if(ModelState.IsValid)
            {
                if (ProfileImage != null && (ProfileImage.ContentType == "image/jpeg" ||
               ProfileImage.ContentType == "image/jpg" ||
               ProfileImage.ContentType == "image/png"))
                {
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";//resmin ismi
                    ProfileImage.SaveAs(Server.MapPath($"~/Images/{filename}"));//Images klasörüne kaydediyor resmi.
                    model.ProfileImageFileName = filename;
                }


                BusinessLayerResult<EvernoteUser> res = evernoteuserManager.UpdateUser(model);

                if (res.Errors.Count > 0)
                {
                    ErrorViewModel NotifyError = new ErrorViewModel()
                    {
                        items = res.Errors,
                        RedirectingUrl = "/MyEver/EditProfile",
                        Title = "Kullanıcı Güncellenemedi"
                    };
                    return View("Error", NotifyError);
                }

                Session["login"] = res.Result;
                return RedirectToAction("ShowProfile");
            }
            return View(model);
           
        }
        [Auth]
        public ActionResult DeleteProfile()
        {
            EvernoteUser currentUser=Session["login"] as EvernoteUser;
            BusinessLayerResult<EvernoteUser> res = evernoteuserManager.DeleteProfile(currentUser.Id);

            if(res.Errors.Count>0)
            {
                ErrorViewModel NotifyErrorObj = new ErrorViewModel()
                {
                    items = res.Errors,
                    Title = "Kullanıcı Silinemedi",
                    RedirectingUrl = "/MyEver/ShowProfile"
                };
                return View("Error", NotifyErrorObj);
            }

            Session.Clear();
            return RedirectToAction("Index");

        }

        public ActionResult Access()
        {
            return View();
        }

     

    }
}