using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyEvernote.Entities.ObjectValues
{
    public class RegisterModel
    {
        [DisplayName("Kullanıcı adı"), Required(ErrorMessage = "{0} alanı boş geçilemez"), StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalı")]
        public string Username { get; set; }

        [DisplayName("E-posta"),Required(ErrorMessage ="{0} alanı boş geçilemez"),StringLength(70,ErrorMessage ="{0} alanı max. {1} karakter olmalı"),EmailAddress(ErrorMessage ="{0} alanı içim geçerli bir e-posta giriniz")]
        public string Email { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı boş geçilemez"), StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalı"), DataType(DataType.Password)]
        public string Password { get; set; }


        [DisplayName("Şifre Tekrar"), Required(ErrorMessage = "{0} alanı boş geçilemez"), StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalı"),
            DataType(DataType.Password),
            Compare(nameof(Password),ErrorMessage ="{0} ile {1} alanı uyuşmuyor")]
        public string RePassword { get; set; }
    }
}