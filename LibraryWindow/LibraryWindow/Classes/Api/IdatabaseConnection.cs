using LibraryWindow.classes.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWindow.classes.Api
{
    interface IdatabaseConnection
    {
        Task<LoginResult> CheckLogin(LoginCredentials loginCredentials);

        Task<UserInfoResult> GetUserTokens();

        void UpdateTokens(int tokens);

        void Logout();
    }
}
