using Betkeeper.Page.Components;
using Newtonsoft.Json.Linq;

namespace Betkeeper.Page
{
    /// <summary>
    /// Generic type-based component actions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComponentTools<T> where T : Component
    {
        /// <summary>
        /// Get component from action.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="componentKey"></param>
        /// <returns></returns>
        public static T GetComponentFromAction(
            PageAction action,
            string componentKey)
        {
            var asJObject = JObject.Parse(action.Parameters["components"].ToString())[componentKey];

            return ComponentParser
                .ParseComponent(asJObject.ToString()) as T;
        }
    }
}
