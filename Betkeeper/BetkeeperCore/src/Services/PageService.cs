using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betkeeper.Page;

namespace Betkeeper.Services
{
    public static class PageService
    {
        private static Dictionary<string, Type> PageTypeDictionary { get; set; }

        /// <summary>
        /// Initialize different page types
        /// </summary>
        public static void InitializePageTypes()
        {
            PageTypeDictionary = new Dictionary<string, Type>();

            foreach (var type in Assembly.GetAssembly(typeof(PageBase)).GetTypes()
                .Where(pageType => 
                    pageType.IsClass && 
                    !pageType.IsAbstract && 
                    pageType.IsSubclassOf(typeof(PageBase))))
            {
                var pageKey = ((PageBase)Activator.CreateInstance(type)).PageKey;
                PageTypeDictionary.Add(pageKey, type);
            }
        }

        /// <summary>
        /// Returns a new page instance based on key
        /// </summary>
        /// <param name="pageKey"></param>
        /// <returns></returns>
        public static PageBase GetPageInstance(string pageKey)
        {
            if (PageTypeDictionary == null || !PageTypeDictionary.ContainsKey(pageKey))
            {
                return null;
            }

            return (PageBase)Activator.CreateInstance(PageTypeDictionary[pageKey]);
        }
    }
}
