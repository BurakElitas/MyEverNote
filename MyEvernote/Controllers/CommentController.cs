using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using MyEvernote.Filters;
using MyEvernote.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MyEvernote.Controllers
{
    public class CommentController : Controller
    {
        NoteManager noteManager = new NoteManager();
        CommentManager commentManager = new CommentManager();
        // GET: Comment
        public ActionResult ShowNoteComments(int? id)
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = noteManager.ListQueryable().Include("Comments").FirstOrDefault(x => x.Id == id);
            if(note==null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialComments", note.Comments);
        }

        [HttpPost]
        [Auth]
        public ActionResult Edit(int? id,string text)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Comment comment = commentManager.Find(x => x.Id == id.Value);
            if (comment == null)
                return HttpNotFound();

            comment.Text = text;
            if(commentManager.Update(comment)>0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Comment comment = commentManager.Find(x => x.Id == id.Value);
            if (comment == null)
                return HttpNotFound();

            if (commentManager.Delete(comment) > 0)
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);

            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Auth]
        public ActionResult AddComment(Comment com,int? noteid)
        {
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModidiedUsername");
            ModelState.Remove("CreatedOn");
            if (ModelState.IsValid)
            {
                if (noteid == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                Note note = noteManager.Find(x => x.Id == noteid.Value);
                if (note == null)
                    return HttpNotFound();

                com.Owner = CurrentSession.User;
                com.Note = note;
               

                if (commentManager.Insert(com) > 0)
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);

                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
    }
}