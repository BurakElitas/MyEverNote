using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using MyEvernote.Filters;
using MyEvernote.Models;

namespace MyEvernote.Controllers
{
   
    public class NoteController : Controller
    {
        NoteManager noteManager = new NoteManager();
        CategoryManager categoryManager = new CategoryManager();
        LikedManager likedManager = new LikedManager();
        [Auth]
        public ActionResult Index()
        {
            List<Note> myNotes = noteManager.ListQueryable().Include("Category").Include("Owner").Where(
                x => x.Owner.Id == CurrentSession.User.Id).OrderByDescending(x => x.ModifiedOn).ToList();
            ViewBag.baslik = "Notlarım";
            ViewBag.state = false;
            return View(myNotes);
        }
        [Auth]
        public ActionResult MyLikes()
        {
            List<Note> myNotes = likedManager.ListQueryable().Include("LikedUser").Include("Note").Where(
               x => x.LikedUser.Id == CurrentSession.User.Id).Select(x => x.Note).Include("Category").Include("Owner").OrderByDescending(x => x.ModifiedOn).ToList();
            ViewBag.baslik = "Beğendiklerim";
            ViewBag.state = true;
            return View("Index", myNotes);
        }

        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }
        [Auth]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title");
            return View();
        }

        public PartialViewResult ShowDetail(int id)
        {
            Note note = noteManager.Find(x => x.Id == id);
            return PartialView("_PartialShowDetail", note);
        }

        [HttpPost]
        [Auth]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Note note, HttpPostedFileBase Resim)
        {
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModidiedUsername");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("LikeCount");
            if (ModelState.IsValid)
            {
                if (Resim != null && (Resim.ContentType == "image/jpeg" || Resim.ContentType == "image/png" || Resim.ContentType == "image/jpg"))
                {
                   
                     string filename = Resim.FileName;

                     Resim.SaveAs(Server.MapPath($"~/Images/Note/{filename}"));
                    note.ImageName = filename;
                 
                }
                else
                    note.ImageName =null;

                note.Owner = CurrentSession.User;
                noteManager.Insert(note);


                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }


        [HttpPost]
        [Auth]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Note note)
        {

            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModidiedUsername");
            if (ModelState.IsValid)
            {
                Note nt = noteManager.Find(x => x.Id == note.Id);
                nt.IsDraft = note.IsDraft;
                nt.Title = note.Title;
                nt.Text = note.Text;
                nt.CategoryId = note.CategoryId;
                noteManager.Update(nt);

                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.Id == id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Auth]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = noteManager.Find(x => x.Id == id);
            noteManager.Delete(note);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetLiked(int[] ids)
        {
            List<int> LikedNoteids = new List<int>();

            if(CurrentSession.User!=null && ids!=null)
            {
               LikedNoteids = likedManager.List(
                x => x.LikedUser.Id == CurrentSession.User.Id && ids.Contains(x.Note.Id)).Select(
                x => x.Note.Id).ToList();
            }
             

            if(LikedNoteids!=null)
            {
                return Json(new { sonuc=LikedNoteids,state=true });

            }
            return Json(new { state = false });
            
        }

        [HttpPost]
        public ActionResult SetlikedUser(int noteid,bool state)
        {
            Note note = noteManager.Find(x => x.Id == noteid);
            if(note==null)
            {
                return HttpNotFound();
            }
            if (CurrentSession.User != null)
            {
                Liked like = likedManager.Find(x => x.LikedUser.Id == CurrentSession.User.Id && x.Note.Id == noteid);

                if (like == null && state == true)
                {
                    Liked liked = new Liked();
                    liked.LikedUser = CurrentSession.User;
                    liked.Note = note;
                    if (likedManager.Insert(liked) > 0)
                    {
                        note.LikeCount++;
                        noteManager.Update(note);
                        return Json(new { hasError = false, likeCount = note.LikeCount, style = "fas", newstate = true });
                    }

                    return Json(new { hasError = true, errorMessage = "Like işlemi sırasında hata meydana geldi." });
                }

                else if (like != null && state == false)
                {
                    if (likedManager.Delete(like) > 0)
                    {
                        note.LikeCount--;
                        noteManager.Update(note);
                        return Json(new { hasError = false, likeCount = note.LikeCount, style = "far", newstate = false });
                    }
                    return Json(new { hasError = true, errorMessage = "Like kaldırma işlemi sırasında hata meydana geldi." });

                }
            }
            return Json(new { hasError = true, errorMessage = "Beğeni işlemi yapabilmek için lütfen Giriş yapınız." });
        }
    }
}
