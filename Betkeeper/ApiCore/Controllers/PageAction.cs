using Api.Classes;
using Betkeeper.Actions;
using Betkeeper.Classes;
using Betkeeper.Page;
using Betkeeper.Services;
using Betkeeper.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Api.Controllers
{
    public class PageActionController : ControllerBase
    {
        /// <summary>
        /// Executes a page action.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [Route("api/pageaction/{page}/{pageaction}")]
        public IActionResult Action(string page, string pageAction)
        {
            if (string.IsNullOrEmpty(page))
            {
                return BadRequest();
            }

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Unauthorized();
            }

            var parameters = Http.GetRequestBody<Dictionary<string, object>>(Request);

            try
            {
                return ExecutePageAction(new PageAction((int)userId, page, pageAction, parameters));
            }
            catch (ActionException actionException)
            {
                // Create page action response from action exception
                return ToIActionResult(
                    new PageActionResponse(
                        actionException.ActionExceptionType,
                        actionException.ErrorMessage));
            }
        }

        /// <summary>
        /// Executes a page action.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        [Route("api/pageaction/{page}/{pageId}/{pageAction}")]
        public IActionResult Action(string page, int pageId, string pageAction)
        {
            if (string.IsNullOrEmpty(page))
            {
                return BadRequest();
            }

            var userId = TokenLog.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Unauthorized();
            }

            var parameters = Http.GetRequestBody<Dictionary<string, object>>(Request);

            try
            {
                return ExecutePageAction(new PageAction((int)userId, page, pageAction, parameters, pageId));
            }
            catch (ActionException actionException)
            {
                // Create page action response from action exception
                return ToIActionResult(new PageActionResponse(
                    actionException.ActionExceptionType,
                    actionException.ErrorMessage));
            }
        }

        /// <summary>
        /// Executes a page action.
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="action">Action name.</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IActionResult ExecutePageAction(PageAction action)
        {
            var pageInstance = PageService.GetPageInstance(action.Page);

            if (pageInstance == null)
            {
                return NotFound();
            }

            return ToIActionResult(pageInstance.HandleAction(action));
        }

        /// <summary>
        /// Return page action response as IActionResult
        /// </summary>
        /// <returns></returns>
        private IActionResult ToIActionResult(PageActionResponse response)
        {
            return StatusCode((int)response.ActionResultType, response);
        }
    }
}
