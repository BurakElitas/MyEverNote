using MyEvernote.Comman;
using MyEvernote.Core;
using MyEvernote.DataAccessLayer;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class Repository<T> : IDataAccess<T> where T : class
    {

        DatabaseContext context;
        DbSet<T> _object;
        public Repository()
        {
            context = RepositoryBase.CreateContext;
            _object = context.Set<T>();
        }


        public List<T> List()
        {
            return _object.ToList();
        }

        public List<T> List(Expression<Func<T, bool>> where)
        {
            return _object.Where(where).ToList();

        }

        public int Insert(T obj)
        {
            if (obj is MyEntityBase) //gelen nesne bir MyEntityBase tipinde ise . Bu kodun amacı hep aynı şeyleri yazmamak için
            {
                MyEntityBase o = obj as MyEntityBase;
                DateTime now = DateTime.Now; //ms farklı olmaması için ilk kayıtta böyle yapıyorum
                o.CreatedOn = now;
                o.ModifiedOn = now;
                o.ModidiedUsername = App.Comman.Username(); //Varsayılanda kullanıcı kendi kayıt olduğu için system eklemiş gözüküyor.
            }
            _object.Add(obj);
            return Save();
        }

        public int Update(T obj)
        {
            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;
                o.ModifiedOn = DateTime.Now;
                o.ModidiedUsername = App.Comman.Username();
            }
            return Save();
        }

        public int Delete(T obj)
        {
            _object.Remove(obj);
            return Save();
        }

        public T Find(Expression<Func<T, bool>> where)
        {
            return _object.FirstOrDefault(where);
        }

        public int Save()
        {
            return context.SaveChanges();
        }
       

        public IQueryable<T> ListQueryable()
        {
            return _object;
        }

        
    }
}
