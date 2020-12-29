using Betkeeper.Extensions;
using System.Collections.Generic;

namespace Betkeeper.Page
{
    public class ListGroupItemExpandParameters
    {
        public int UserId { get; }

        public object ItemIdentifier { get; }

        public string ComponentKey { get; }

        public ListGroupItemExpandParameters(int userId, Dictionary<string, object> requestParameters)
        {
            UserId = userId;
            ItemIdentifier = requestParameters["itemIdentifier"];
            ComponentKey = requestParameters.GetString("componentKey");
        }
    }
}
