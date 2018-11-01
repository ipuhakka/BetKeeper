using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Globalization;
using API.Models;
using BetKeeper.Exceptions;
using Newtonsoft.Json;

namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BetsController: ApiController
    {
        /// <summary>
        /// Returns users bets. Request can specify whether the bets need to be finished or not, and a certain folder
        /// from which the bets are searched.
        /// 
        /// Responses:
        ///     200 OK
        ///     401 Unauthorized
        /// </summary>
        public HttpResponseMessage Get([FromUri] bool? finished = null, [FromUri] string folder = null)
        {
            if (Request.Headers.Authorization == null || !TokenLog.ContainsToken(Request.Headers.Authorization.ToString()))
            {
                return HttpMessage.ResponseMessage("Request did not contain a valid token", HttpStatusCode.Unauthorized);
            }

            try
            {
                BetsModel model = new BetsModel();
                string data = model.GetBets(Request.Headers.Authorization.ToString(), finished, folder);
                return HttpMessage.ResponseMessage(data, HttpStatusCode.OK, "application/json");
            }
            catch (AuthenticationError e)
            {
                return HttpMessage.ResponseMessage(e.ErrorMessage, HttpStatusCode.Unauthorized);
            }        
        }

        /// <summary>
        /// Deletes a bet from the user. If there are any folders specified, bet is deleted only from those folders, not from
        /// table 'bets'. 
        /// 
        /// Responses:
        ///     204 Deleted
        ///     401 Unauthorized
        ///     404 Not found: When folders are specified, 404 will be sent back if bet is not found at all, or from any of the 
        ///         specified folders.
        /// </summary>
        public HttpResponseMessage Delete(int id, [FromUri] List<string> folders = null)
        {
            if (Request.Headers.Authorization == null || !TokenLog.ContainsToken(Request.Headers.Authorization.ToString()))
            {
                return HttpMessage.ResponseMessage("Request did not contain a valid token", HttpStatusCode.Unauthorized);
            }

            try
            {
                BetsModel model = new BetsModel();
                int res = model.DeleteBet(Request.Headers.Authorization.ToString(), id, folders);
                if (res == 0)
                {
                    return HttpMessage.ResponseMessage("Bet not found", HttpStatusCode.NotFound);
                }
                return HttpMessage.ResponseMessage("", HttpStatusCode.NoContent);
            }
            catch (AuthenticationError e)
            {
                return HttpMessage.ResponseMessage(e.ErrorMessage, HttpStatusCode.Unauthorized);
            }
            catch (UnknownBetError)
            {
                return HttpMessage.ResponseMessage("Bet not found", HttpStatusCode.NotFound);
            }
        }

        /// <summary>
        /// Modifies a bet with bet_id = id.
        /// 
        /// returns 
        ///     204 No content: On successful modification.
        ///     400 Bad request: If converting request body to dynamic fails, double format
        ///         is incorrect or bet_won parameter is missing.   
        ///     401 Unauthorized: if request doesn't have a valid token or user
        ///         tries to modify a bet that doesn't belong to them.
        ///     404 Not found: If bet trying to be modified does not exist.
        ///     409 Conflict: If modification affected 0 rows.     
        /// </summary>
        public HttpResponseMessage Put(int id)
        {
            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            dynamic data;
            string name = null;
            double? odd = null;
            double? bet = null;
            int result = -1;
            BetsModel model = new BetsModel();

            if (Request.Headers.Authorization == null || !TokenLog.ContainsToken(Request.Headers.Authorization.ToString()))
            {
                return HttpMessage.ResponseMessage("Request did not contain a valid token", HttpStatusCode.Unauthorized);
            }

            try
            {
                HttpContent requestContent = Request.Content;
                string jsonContent = requestContent.ReadAsStringAsync().Result;
                data = JsonConvert.DeserializeObject(jsonContent);

                if (data["bet_won"] == null)
                    return HttpMessage.ResponseMessage("Request body was missing parameter bet_won", HttpStatusCode.BadRequest);

                if (data["bet"] != null)
                    bet = Convert.ToDouble(data["bet"].ToString());
                if (data["odd"] != null)
                    odd = Convert.ToDouble(data["odd"].ToString());

                if (data["name"] != null)
                    name = data["name"].ToString();

                result = model.ModifyBet(Request.Headers.Authorization.ToString(), id, Convert.ToBoolean(data["bet_won"]), odd, bet, name);
            }
            catch (JsonReaderException)
            {
                return HttpMessage.ResponseMessage("Request body was malformed", HttpStatusCode.BadRequest);
            }
            catch (FormatException)
            {
                return HttpMessage.ResponseMessage("Invalid double format", HttpStatusCode.BadRequest);
            }
            catch (UnknownBetError)
            {
                return HttpMessage.ResponseMessage("", HttpStatusCode.NotFound);
            }
            catch (AuthenticationError)
            {
                return HttpMessage.ResponseMessage("Bet did not belong to user trying to modify it", HttpStatusCode.Unauthorized);
            }

            if (result == 0)
                return HttpMessage.ResponseMessage("Bet with id " + id + " could not be modified ", HttpStatusCode.Conflict);

            return HttpMessage.ResponseMessage("", HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Creates a new Bet into bets-table.
        /// 
        /// Responses:
        ///     201 Created
        ///     400 Bad request (Invalid request body or missing arguments)
        ///     401 Unauthorized 
        /// </summary>
        public HttpResponseMessage Post()
        {
            double bet = 0;
            double odd = 0;
            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            if (Request.Headers.Authorization == null || !TokenLog.ContainsToken(Request.Headers.Authorization.ToString()))
            {
                return HttpMessage.ResponseMessage("Request did not contain a valid token", HttpStatusCode.Unauthorized);
            }

            try
            {
                dynamic data;
                HttpContent requestContent = Request.Content;
                string jsonContent = requestContent.ReadAsStringAsync().Result;
                data = JsonConvert.DeserializeObject(jsonContent);

                if (data["bet_won"] == null || data["odd"] == null || data["bet"] == null || 
                    data["name"] == null || data["datetime"] == null || data["folders"] == null)
                    return HttpMessage.ResponseMessage("Missing arguments in request body", HttpStatusCode.BadRequest);

                bet = Convert.ToDouble(data["bet"].ToString());
                odd = Convert.ToDouble(data["odd"].ToString());
                BetsModel model = new BetsModel();
                int id = model.CreateBet(Request.Headers.Authorization.ToString(), data["datetime"].ToString(), Conversions.ToNullableBool(data["bet_won"].ToString()), odd, bet, data["name"].ToString(), JsonConvert.DeserializeObject<List<string>>(data["folders"].ToString()));
                return HttpMessage.ResponseMessage(JsonConvert.SerializeObject(id), HttpStatusCode.Created, "application/json");
            }
            catch (JsonReaderException)
            {
                return HttpMessage.ResponseMessage("Request body was malformed", HttpStatusCode.BadRequest);
            }
            catch (FormatException)
            {
                return HttpMessage.ResponseMessage("Invalid double format", HttpStatusCode.BadRequest);
            }
            catch (ArgumentException)
            {
                return HttpMessage.ResponseMessage("Invalid date format", HttpStatusCode.BadRequest);
            }
        }

    }
}
