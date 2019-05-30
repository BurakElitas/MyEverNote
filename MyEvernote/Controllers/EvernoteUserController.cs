using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyEvernote.BusinessLayer;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Filters;
using MyEvernote.Models;

namespace MyEvernote.Controllers
{
    [Auth]
    [AuthAdmin]
    public class EvernoteUserController : Controller
    {
        private EvernoteUserManager evernoteUserManager = new EvernoteUserManager();

        
        public ActionResult Index()
        {
            return View(evernoteUserManager.List());
        }

        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvernoteUser evernoteUser = evernoteUserManager.Find(x=>x.Id==id.Value);
            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }

        
        public ActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EvernoteUser evernoteUser)
        {
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModidiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res=evernoteUserManager.Insert(evernoteUser);
                if(res.Errors.Count>0)
                {
                    foreach (MessageObject item in res.Errors)
                     {
                        ModelState.AddModelError("", item.Error);
                     }
                    return View(evernoteUser);
                }


                return RedirectToAction("Index");
            }

            return View(evernoteUser);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvernoteUser evernoteUser = evernoteUserManager.Find(x => x.Id == id.Value);
            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EvernoteUser evernoteUser)
        {
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModidiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<EvernoteUser> res = evernoteUserManager.Update(evernoteUser);
                if (res.Errors.Count > 0)
                {
                    foreach (MessageObject item in res.Errors)
                    {
                        ModelState.AddModelError("", item.Error);
                    }
                    return View(evernoteUser);
                }


                return RedirectToAction("Index");
            }

            return View(evernoteUser);

        }

        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvernoteUser evernoteUser = evernoteUserManager.Find(x => x.Id == id.Value);
            if (evernoteUser == null)
            {
                return HttpNotFound();
            }
            return View(evernoteUser);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EvernoteUser evernoteUser = evernoteUserManager.Find(x => x.Id == id);
            evernoteUserManager.Delete(evernoteUser);
            return RedirectToAction("Index");
        }

       
    }
}
