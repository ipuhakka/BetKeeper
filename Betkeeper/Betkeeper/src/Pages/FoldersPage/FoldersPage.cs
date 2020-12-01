using Betkeeper.Page;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Betkeeper.Pages.FoldersPage
{
    public class FoldersPage : IPage
    {
        public PageResponse GetPage(string pageKey, int userId)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage HandleAction(PageAction action)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage HandleDropdownUpdate(Dictionary<string, object> data, int? pageId = null)
        {
            throw new NotImplementedException();
        }
    }
}
