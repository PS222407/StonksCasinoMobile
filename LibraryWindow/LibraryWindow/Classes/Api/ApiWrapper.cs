using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LibraryWindow.classes.Api.Models;
using LibraryWindow.classes.Main;

namespace LibraryWindow.classes.Api
{
    public static class ApiWrapper
    {
        private const string BaseUrl = "http://192.168.2.4/stonkscasino_api";

        public static async Task<string> Login(LoginCredentials loginCredentials)
        {
            string url = $@"{BaseUrl}/login.php";
            Uri request = new Uri(url);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "StonksCasino");


            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(loginCredentials), Encoding.UTF8, "application/json");
            HttpResponseMessage respons = await client.PostAsync(request, httpContent);
            if (respons.IsSuccessStatusCode == false)
            {
                return null;
            }

            string content = await respons.Content.ReadAsStringAsync();
            LoginResult loginResult = JsonConvert.DeserializeObject<LoginResult>(content);

            if(loginResult.Result == "succes")
            {
                GlobalApiToken.AccessToken = loginResult.AccessToken;
                GlobalApiToken.UserId = loginResult.UserId;
            }

            return loginResult.Result;
        }

        public static async Task<bool> Logout()
        {
            string url = $@"{BaseUrl}/Logout.php";
            Uri request = new Uri(url);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "StonksCasino");

            ApiAccessToken apiToken = new ApiAccessToken();
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(apiToken), Encoding.UTF8, "application/json");
            HttpResponseMessage respons = await client.PostAsync(request, httpContent);
            return true;
        }

        public static async Task<bool> GetUserInfo()
        {
            string url = $@"{BaseUrl}/GetUserInfo.php";
            Uri request = new Uri(url);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "StonksCasino");

            ApiAccessToken token = new ApiAccessToken();
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(token), Encoding.UTF8, "application/json");
            HttpResponseMessage respons = await client.PostAsync(request, httpContent);
            if (respons.IsSuccessStatusCode)
            {
                string content = await respons.Content.ReadAsStringAsync();
                UserInfoResult infoResult = JsonConvert.DeserializeObject<UserInfoResult>(content);
                if (infoResult.Result)
                {
                    User.Username = infoResult.UserName;
                    User.Tokens = infoResult.Tokens;
                    User.SelectedCardskin = infoResult.SelectedCardSkin;
                    return true;
                }
                else
                {
                    await Logout();
                    return false;
                }
            }
            else
            {
                User.Username = "";
                User.Tokens = 0;
                return false;
            }
        }

        public static async Task<bool> UpdateTokens(int tokens, string sender)
        {
            string url = $@"{BaseUrl}/UpdateUserTokens.php";
            Uri request = new Uri(url);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "StonksCasino");

            TokenUpdate tokenUpdate = new TokenUpdate(tokens, sender);
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(tokenUpdate), Encoding.UTF8, "application/json");
            HttpResponseMessage respons = await client.PostAsync(request, httpContent);

            if (respons.IsSuccessStatusCode)
            {
                string content = await respons.Content.ReadAsStringAsync();
                ApiResult result = JsonConvert.DeserializeObject<ApiResult>(content);
                return result.Result;
            }
            else
            {
                await Logout();
                return false;
            }
        }

        public static async Task<bool> CheckLogin()
        {
            string url = $@"{BaseUrl}/CheckAccesskey.php";
            Uri request = new Uri(url);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "StonksCasino");

            ApiAccessToken token = new ApiAccessToken();
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(token), Encoding.UTF8, "application/json");
            HttpResponseMessage respons = await client.PostAsync(request, httpContent);

            if (respons.IsSuccessStatusCode)
            {
                string content = await respons.Content.ReadAsStringAsync();
                ApiResult result = JsonConvert.DeserializeObject<ApiResult>(content);
                return result.Result;
            }
            else
            {
                return false;
            }
        }
    }
}
