using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Entities
{
    [Table("Notes")]
   public class Note:MyEntityBase
    {
        [Required,StringLength(60),DisplayName("Not Başlığı")]
        public string Title { get; set; }

        [Required,StringLength(3000),DisplayName("Not Metni")]
        public string Text { get; set; }

        [DisplayName("Taslak")]
        public bool IsDraft { get; set; }

        [DisplayName("Beğenilme")]
        public int LikeCount { get; set; }
        public int CategoryId { get; set; }

        [DisplayName("Resim Seç")]
        public string ImageName { get; set; }
        public virtual EvernoteUser Owner { get; set; }
        public virtual Category Category { get; set; }
        public virtual List<Comment> Comments{ get; set; }
        public virtual List<Liked> Likes { get; set; }

        public Note()
        {
            Comments = new List<Comment>();
            Likes = new List<Liked>();
        }

    }
}
