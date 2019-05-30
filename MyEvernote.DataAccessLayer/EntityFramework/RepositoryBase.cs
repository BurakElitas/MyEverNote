using MyEvernote.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class RepositoryBase
    {
        private static DatabaseContext context;
        protected RepositoryBase()
        {

        }
        public static DatabaseContext CreateContext
        {
            get
            { 
                if(context==null)
                {
                    context = new DatabaseContext();
                }
                return context;
            }
        }
        
    }
}
