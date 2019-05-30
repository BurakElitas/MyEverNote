using MyEvernote.Comman;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyEvernote.Models
{
    public class WebComman : IComman
    {
        public string Username()
        {
            if(HttpContext.Current.Session["login"]!=null)
            {
                EvernoteUser user = HttpContext.Current.Session["login"] as EvernoteUser;
                return user.Username;
            }
            return "system";
        }
    }
}