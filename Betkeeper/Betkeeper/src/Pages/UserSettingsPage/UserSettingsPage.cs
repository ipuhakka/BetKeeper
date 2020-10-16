using Betkeeper.Classes;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using Betkeeper.Actions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Betkeeper.Pages
{
    public class UserSettingsPage : IPage
    {
        private UserAction UserAction { get; set; }

        public UserSettingsPage()
        {
            UserAction = new UserAction();
        }

        public HttpResponseMessage GetResponse(string pageKey, int userId)
        {
            var passwordModal = new ModalActionButton(
                "changePassword",
                new List<Component>
                {
                    new HiddenInput("newPassword", "New password"),
                    new HiddenInput("confirmNewPassword", "Confirm new password")
                },
                "Change password",
                requireConfirm: true);

            return Http.CreateResponse(
                HttpStatusCode.OK,
                new PageResponse(
                    "usersettings", 
                    new List<Component> { passwordModal }, 
                    new Dictionary<string, object>()));
        }

        public HttpResponseMessage HandleAction(PageAction action)
        {
            switch (action.ActionName)
            {
                case "changePassword":
                    return HandlePasswordAction(action);
                default:
                    throw new NotImplementedException($"{action.ActionName} not implemented!");
            }
        }

        public HttpResponseMessage HandleDropdownUpdate(Dictionary<string, object> data, int? pageId = null)
        {
            throw new NotImplementedException();
        }

        private HttpResponseMessage HandlePasswordAction(PageAction action)
        {
            var data = action.Parameters;

            UserAction.ChangePassword(
                action.UserId,
                data["newPassword"].ToString(),
                data["confirmNewPassword"].ToString());

            return Http.CreateResponse(HttpStatusCode.OK, new PageActionResponse("Password changed successfully"));
        }
    }
}
