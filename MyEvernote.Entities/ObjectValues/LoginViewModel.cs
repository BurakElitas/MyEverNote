﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyEvernote.Entities.ObjectValues
{
    public class LoginViewModel
    {
        [DisplayName("Kullanıcı adı") ,Required(ErrorMessage ="{0} alanı boş geçilemez"),StringLength(25,ErrorMessage ="{0} alanı max. {1} karakter olmalı")]
        public string Username { get; set; }

        [DisplayName("Şifre"),Required(ErrorMessage ="{0} alanı boş geçilemez"),StringLength(25,ErrorMessage ="{0} alanı max. {1} karakter olmalı"),DataType(DataType.Password)]
        public string Password { get; set; }
    }
}