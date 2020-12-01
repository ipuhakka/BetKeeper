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
    public class UserSettingsPage : PageBase
    {
        private UserAction UserAction { get; set; }

        public UserSettingsPage()
        {
            UserAction = new UserAction();
        }

        public override PageResponse GetPage(string pageKey, int userId)
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

            return new PageResponse(
                    "usersettings",
                    new List<Component> { passwordModal },
                    new Dictionary<string, object>());
        }

        public override HttpResponseMessage HandleAction(PageAction action)
        {
            switch (action.ActionName)
            {
                case "changePassword":
                    return HandlePasswordAction(action);
                default:
                    throw new NotImplementedException($"{action.ActionName} not implemented!");
            }
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
