using Betkeeper.Enums;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using Betkeeper.Actions;
using System;
using System.Collections.Generic;

namespace Betkeeper.Pages
{
    public class UserSettingsPage : PageBase
    {
        public override string PageKey => "usersettings";

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

        public override PageActionResponse HandleAction(PageAction action)
        {
            switch (action.ActionName)
            {
                case "changePassword":
                    return HandlePasswordAction(action);
                default:
                    throw new NotImplementedException($"{action.ActionName} not implemented!");
            }
        }

        private PageActionResponse HandlePasswordAction(PageAction action)
        {
            var data = action.Parameters;

            UserAction.ChangePassword(
                action.UserId,
                data["newPassword"].ToString(),
                data["confirmNewPassword"].ToString());

            return new PageActionResponse(
                ActionResultType.OK,
                "Password changed successfully");
        }
    }
}
