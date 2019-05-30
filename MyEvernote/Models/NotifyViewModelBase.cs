using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyEvernote.Models
{
    public class NotifyViewModelBase<T>
    {
        public List<T> items { get; set; }
        public string Header { get; set; }
        public string Title { get; set; }
        public bool isRedirecting { get; set; }
        public string RedirectingUrl { get; set; }
        public int RedirectingTimeout { get; set; }

        public NotifyViewModelBase()
        {
            items = new List<T>();
            Header = "Yönlendiriliyorsunuz";
            Title = "Geçersiz İşlem";
            isRedirecting = true;
            RedirectingUrl = "/MyEver/Index";
            RedirectingTimeout = 10000;

            
        }
    }
}