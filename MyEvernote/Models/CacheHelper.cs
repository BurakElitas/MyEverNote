using MyEvernote.BusinessLayer;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace MyEvernote.Models
{
    public class CacheHelper
    {
        public static List<Category> GetCategoriesFromCache()
        {
            var result = WebCache.Get("Category-cache");
            if(result==null)
            {
                CategoryManager categoryManager = new CategoryManager();
                result = categoryManager.List();
                WebCache.Set("Category-cache", result, 20, true);
            }
            return result;
        }

        public static void RemoveCategoriesFromCache()
        {
            Remove("Category-cache");
        }
        public static void Remove(string key)
        {
            WebCache.Remove(key);
        }
    }
}