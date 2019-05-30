using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyEvernote.Entities.Messages;

namespace MyEvernote.Models
{
    public class ErrorViewModel:NotifyViewModelBase<MessageObject>
    {
        public ErrorViewModel()
        {
            Header = "Hata";

        }
    }
}