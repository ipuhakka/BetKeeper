using System;
using System.Collections.Generic;

namespace Betkeeper.Page
{
    public class PageResponse
    {
        public string Key { get; set; }

        public Structure Structure { get; set; }

        public Dictionary<string, object> Data { get; set; }

        public PageResponse(string pageKey)
        {
            Key = pageKey;
        }
    }
}
