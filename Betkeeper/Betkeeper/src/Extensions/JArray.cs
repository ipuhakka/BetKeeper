using Newtonsoft.Json.Linq;

namespace Betkeeper.Extensions
{
    public static class JArrayExtensions
    {

        /// <summary>
        /// Converts a JArray into JObject.
        /// </summary>
        /// <param name="jArray"></param>
        /// <returns></returns>
        public static JObject ToJObject(this JArray jArray)
        {
            var jObj = new JObject();

            for (var i = 0; i < jArray.Count; i++)
            {
                jObj.Add($"bet-target-{i}", jArray[i]);
            }

            return jObj;
        }
    }
}
