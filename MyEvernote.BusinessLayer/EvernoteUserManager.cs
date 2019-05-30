using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.BusinessLayer.Results;
using MyEvernote.Comman.Helpers;
using MyEvernote.Entities;
using MyEvernote.Entities.Messages;
using MyEvernote.Entities.ObjectValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
   public class EvernoteUserManager:ManagerBase<EvernoteUser>
    {
        
      
        public BusinessLayerResult<EvernoteUser> RegisterUser(RegisterModel model) //gelen model Register Actionundaki model ile aynı, olduğu gibi buraya geliyor
        {
            BusinessLayerResult<EvernoteUser> layerresult = new BusinessLayerResult<EvernoteUser>();//içerik olarak gelen tipte bir property ve list<string> tipinde property var
            EvernoteUser user =Find(x => x.Email == model.Email || x.Username == model.Username); //kullanıcı adı veya e mail sistemde var mı sorguluyor
            if(user!=null)
            {
                if(user.Email==model.Email)
                {
                    layerresult.AddError(MessageCodes.UsedUsername, "Bu e-posta adresi kullanılmaktadır.");
                }
                if(user.Username==model.Username)
                {
                    layerresult.AddError(MessageCodes.UsedEmail, "Bu kullanıcı adı kullanılmaktadır.");
                }
 
            }
            else
            {
              int dbResult=base.Insert(new EvernoteUser() //kullanıcı adı veya e posta sistemde kayıtlı değilse kullanıcı eklenebilir
                {
                    Email = model.Email,
                    Username = model.Username,
                    Password=model.Password,
                    ProfileImageFileName="user_boy.png",
                    ActivateGuid=Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false,

              });


                if(dbResult>0) //eğer ekleme işlemi başarılı ise
                {
                    layerresult.Result =Find(x => x.Email == model.Email && x.Username == model.Username);
                    
                    //Send Mail
                    string SiteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{SiteUri}/MyEver/UserActivate/{layerresult.Result.ActivateGuid}";
                    string body=$"Merhaba {layerresult.Result.Username};<br/><br/> Hesabınızı Aktifleştirmek İçin <a href='{activateUri}' target='_blank'>Tıklayınız..</a>";

                    MailHelper.SendMail(body, layerresult.Result.Email, "MyEvernote Hesap Aktifleştirme");

                 
                }

            }
            return  layerresult; //içinde Son eklediğim kullanıcı ya da hata mesajları var
        }
        public BusinessLayerResult<EvernoteUser> LoginUser(LoginViewModel model)
        {
            BusinessLayerResult<EvernoteUser> user = new BusinessLayerResult<EvernoteUser>();
            user.Result = Find(x => x.Username == model.Username && x.Password == model.Password);
            if(user.Result!=null)
            {
                if(user.Result.IsActive)
                {
                    return user;
                }
                else
                {
                    user.AddError(MessageCodes.UserIsNotActive,"Bu kullanıcı Aktifleştirilmemiş! Lütfen e-posta adresinizi kontrol ediniz.");
                    return user;
                }
            }
            else
            {
                user.AddError(MessageCodes.UsernameOrPassWrong,"Hatalı kullanıcı adı veya şifre.Lütfen tekrar deneyiniz.");
                return user;
            }
        }
        public BusinessLayerResult<EvernoteUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser user =Find(x => x.ActivateGuid == activateId);
            if(user!=null)
            {
                if(user.IsActive!=true)
                {
                    user.IsActive = true;
                    Update(user);
                    res.Result = user;
                    
                }
                else
                {
                    res.AddError(MessageCodes.UserAlreadyActivated, "Bu kullanıcı zaten aktiftir.");
                   
                }
            }
            else
            {
                res.AddError(MessageCodes.UserDoesNotExist, "Aktifleştirilecek kullanıcı bulunamıyor.");
            }
            return res;
        }

        public BusinessLayerResult<EvernoteUser> GetUserById(int Id)
        {
            BusinessLayerResult<EvernoteUser> layerResult = new BusinessLayerResult<EvernoteUser>();
            layerResult.Result =Find(x => x.Id == Id);
            if(layerResult.Result==null)
            {
                layerResult.AddError(MessageCodes.UserNotFound, "Kullanıcı Bulunamadı");
            }
            return layerResult;
        }

        public BusinessLayerResult<EvernoteUser> UpdateUser(EvernoteUser data)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser user =Find(x => x.Email == data.Email || x.Username == data.Username);
            if(user != null && user.Id != data.Id)
            {
                if(user.Username== data.Username)
                {
                    res.AddError(MessageCodes.UsernameAlreadyExists, "Bu kullanıcı adı daha önceden alınmış.");

                }
                if(user.Email== data.Email)
                {
                    res.AddError(MessageCodes.EmailAlreadyExists, "Bu Email adresi daha önceden alınmış.");
                }
                return res;
                
            }

            res.Result = Find(x => x.Id == data.Id);

            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Email = data.Email;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            

            if(string.IsNullOrEmpty(data.ProfileImageFileName)==false)
            {
                res.Result.ProfileImageFileName = data.ProfileImageFileName;
            }

            if(base.Update(res.Result)==0)
            {
                res.AddError(MessageCodes.ProfileCouldNotUpdated, "Profil Güncellenemedi");
            }

            res.Result =Find(x => x.Id == data.Id);
            return res;
        }

        public BusinessLayerResult<EvernoteUser> DeleteProfile(int ıd)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser currentUser =Find(x => x.Id == ıd);
            if(currentUser==null)
            {
                res.AddError(MessageCodes.UserCouldNotFind, "Kullanıcı Bulunamadı");
            }

            if(Delete(currentUser)==0)
            {
                res.AddError(MessageCodes.UserCouldNotDelete, "Kullanıcı Silinirken Hata Oluştu");
            }
            return res;
        }

        public new BusinessLayerResult<EvernoteUser> Insert(EvernoteUser model)
        {
            BusinessLayerResult<EvernoteUser> layerresult = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser user = Find(x => x.Email == model.Email || x.Username == model.Username);
            layerresult.Result = model;
            if (user != null)
            {
                if (user.Email == model.Email)
                {
                    layerresult.AddError(MessageCodes.UsedUsername, "Bu e-posta adresi kullanılmaktadır.");
                }
                if (user.Username == model.Username)
                {
                    layerresult.AddError(MessageCodes.UsedEmail, "Bu kullanıcı adı kullanılmaktadır.");
                }

            }
            else
            {
                layerresult.Result.ActivateGuid = Guid.NewGuid();
                layerresult.Result.ProfileImageFileName = "user_boy.png";

               if(base.Insert(layerresult.Result)==0)
                {
                    layerresult.AddError(MessageCodes.UserCouldNotInserted, "Kullanıcı Eklenemedi");
                }
            }
            return layerresult;
        }

        public new BusinessLayerResult<EvernoteUser> Update(EvernoteUser data)
        {
            BusinessLayerResult<EvernoteUser> res = new BusinessLayerResult<EvernoteUser>();
            EvernoteUser user = Find(x => x.Email == data.Email || x.Username == data.Username);
            res.Result = data;
            if (user != null && user.Id != data.Id)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(MessageCodes.UsernameAlreadyExists, "Bu kullanıcı adı daha önceden alınmış.");

                }
                if (user.Email == data.Email)
                {
                    res.AddError(MessageCodes.EmailAlreadyExists, "Bu Email adresi daha önceden alınmış.");
                }
                return res;

            }

            res.Result = Find(x => x.Id == data.Id);

            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Email = data.Email;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;


        

            if (base.Update(res.Result) == 0)
            {
                res.AddError(MessageCodes.UserCouldNotUpdated, "Profil Güncellenemedi");
            }

            res.Result = Find(x => x.Id == data.Id);
            return res;
        }
    }
}
