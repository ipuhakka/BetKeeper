using System.Collections.Generic;

namespace Betkeeper.Page
{
    public class DropdownUpdateParameters
    {
        public Dictionary<string, object> Data { get; }

        public int UserId { get; }

        public int? PageId { get; }

        public DropdownUpdateParameters(int userId, Dictionary<string, object> data, int? pageId = null)
        {
            UserId = userId;
            Data = data;
            PageId = pageId;
        }
    }
}
