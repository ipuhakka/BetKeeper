using Betkeeper.Actions;
using Betkeeper.Classes;
using Betkeeper.Page;
using Betkeeper.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Api.Controllers
{
    public class PageController : ControllerBase
    {
        [Route("api/page/{page}")]
        public IActionResult Get(string page)
        {
            if (string.IsNullOrEmpty(page))
            {
                return BadRequest();
            }

            var userId = SessionAction.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Unauthorized();
            }

            return GetPageResponse(page, (int)userId);
        }

        [Route("api/page/{page}/{id}")]
        public IActionResult Get(string page, int id)
        {
            if (string.IsNullOrEmpty(page))
            {
                return BadRequest();
            }

            var userId = SessionAction.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Unauthorized();
            }

            return GetPageResponse(page, (int)userId, id);
        }

        [Route("api/page/handleDropdownUpdate/{page}")]
        public IActionResult HandleDropdownUpdate(string page)
        {
            return DropdownUpdate(page);
        }

        [Route("api/page/handleDropdownUpdate/{page}/{id}")]
        public IActionResult HandleDropdownUpdate(string page, int id)
        {
            return DropdownUpdate(page, id);
        }

        [Route("api/page/expandListGroupItem/{page}")]
        public IActionResult ExpandListGroupItem(string page)
        {
            return HandleExpandListGroupItem(page);
        }

        private IActionResult DropdownUpdate(string page, int? id = null)
        {
            if (string.IsNullOrEmpty(page))
            {
                return BadRequest();
            }

            var userId = SessionAction.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Unauthorized();
            }

            var parameters = Http.GetRequestBody<Dictionary<string, object>>(Request);

            return Ok(PageService
                .GetPageInstance(page)
                .HandleDropdownUpdate(new DropdownUpdateParameters((int)userId, parameters, id)));
        }
        
        private IActionResult HandleExpandListGroupItem(string page)
        {
            if (string.IsNullOrEmpty(page))
            {
                return BadRequest();
            }

            var userId = SessionAction.GetUserIdFromRequest(Request);

            if (userId == null)
            {
                return Unauthorized();
            }

            return Ok(PageService
                    .GetPageInstance(page)
                    .ExpandListGroupItem(new ListGroupItemExpandParameters(
                        (int)userId, 
                        Http.GetRequestBody<Dictionary<string, object>>(Request))));
        }

        /// <summary>
        /// Return page response IActionResult
        /// </summary>
        /// <param name="pageKey"></param>
        /// <param name="userId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        private IActionResult GetPageResponse(
            string pageKey,
            int userId,
            int? pageId = null)
        {
            var pageInstance = PageService.GetPageInstance(pageKey);

            if (pageInstance == null)
            {
                return NotFound();
            }

            // Use page id by default, if it is null use pageKey
            var pageResponse = pageInstance.GetPage(pageId?.ToString() ?? pageKey, userId);

            if (pageResponse.Redirect)
            {
                return Redirect(pageResponse.RedirectTo);
            }

            return Ok(pageResponse);
        }
    }
}
