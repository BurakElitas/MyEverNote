using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.DynamicData;

namespace MyEvernote.Entities
{
    [TableName("Categories")]
   public class Category:MyEntityBase
    {
        [DisplayName("Kategori"),Required(ErrorMessage ="{0} alanı boş geçilemez."),StringLength(50,ErrorMessage ="{0} alanı max. {1} karakterden oluşmalı.")]
        public string Title { get; set; }

        [DisplayName("Açıklama"),StringLength(150,ErrorMessage ="{0} alanı max. {1} karakterden oluşmalı.")]
        public string Description { get; set; }

        public virtual List<Note> Notes { get; set; }

        public Category()
        {
            Notes = new List<Note>();
        
        }


    }
}
