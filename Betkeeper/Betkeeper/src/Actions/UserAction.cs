using System.Collections.Generic;
using System.Linq;
using Betkeeper.Models;

namespace Betkeeper.Actions
{
    public class UserAction
    {
        private UserRepository UserRepository { get; set; }

        public UserAction()
        {
            UserRepository = new UserRepository();
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newPassword"></param>
        /// <param name="newPasswordConfirmed"></param>
        public void ChangePassword(int userId, string newPassword, string newPasswordConfirmed)
        {
            if (!newPassword.Equals(newPasswordConfirmed))
            {
                throw new ActionException(ActionExceptionType.InvalidInput, "Passwords do not match");
            }

            var previousPassword = UserRepository
                .GetUsersById(new List<int> { userId })
                .Single()
                .Password;

            if (previousPassword.Equals(newPassword))
            {
                throw new ActionException(ActionExceptionType.InvalidInput, "Password cannot be same as previous one");
            }

            UserRepository.ChangePassword(userId, newPassword);
        }
    }
}
