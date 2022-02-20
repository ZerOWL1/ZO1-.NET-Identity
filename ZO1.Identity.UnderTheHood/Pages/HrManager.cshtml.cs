using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ZO1.Identity.UnderTheHood.Authorizations;
using ZO1.Identity.UnderTheHood.Models;
using ZO1.Identity.UnderTheHood.Models.ViewModels;

namespace ZO1.Identity.UnderTheHood.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HrManagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public List<WeatherForecastViewModel> WeatherForecastViewModels { get; set; }


        public HrManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task OnGet()
        {
            WeatherForecastViewModels =
                await InvokeEndpointTask<List<WeatherForecastViewModel>>("OurWebAPI", "WeatherForecast");
        }


        /// <summary>
        /// This method use to invoke endpoint with a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clientName"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<T> InvokeEndpointTask<T>(string clientName, string url)
        {
            JwtToken token;

            // get token from session

            // get from key
            var strTokenObj = HttpContext.Session.GetString("access_token");

            // if null
            if (string.IsNullOrWhiteSpace(strTokenObj))
            {
                //create session
                token = await AuthenticateTask();
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj);
            }

            if (token == null ||
                string.IsNullOrWhiteSpace(token.AccessToken) ||
                token.ExpiresAt <= DateTime.UtcNow)
            {
                token = await AuthenticateTask();
            }
            // get client with uri
            var httpClient = _httpClientFactory.CreateClient(clientName);

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.AccessToken);

            // use this line to trigger endpoint
            return await httpClient.GetFromJsonAsync<T>(url);
        }

        /// <summary>
        /// This method use to authenticate and save token to session
        /// </summary>
        /// <returns></returns>
        private async Task<JwtToken> AuthenticateTask()
        {
            // authentication and getting the token
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");

            // 401-Authenticate
            // consume the endPoint protected by JWT Token
            var response = await httpClient.PostAsJsonAsync("auth",
                new Credential { UserName = "admin", Password = "123478@Kid" });

            response.EnsureSuccessStatusCode();

            // json str. should content token
            var strJwt = await response.Content.ReadAsStringAsync();

            // save token into session
            HttpContext.Session.SetString("access_token", strJwt);

            return JsonConvert.DeserializeObject<JwtToken>(strJwt);
        }
    }
}
