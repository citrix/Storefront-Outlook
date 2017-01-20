using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StorefrontApps_Outlook.Classes.CitrixHelper
{
    public partial class Storefront
    {
        public async static Task<string> RetreiveICA(string WebURL, CitrixAuthCredential Creds, CitrixApplicationInfo AppInfo)
        {
            string SFURL = string.Format("{0}/{1}", WebURL, AppInfo.AppLaunchURL);
            bool IsSSL = false;

            if (SFURL.ToLower().IndexOf("https:") != -1)
            {
                IsSSL = true;
            }
            else
            {
                IsSSL = false;
            }

            var _ica = await GetICAFromStoreFront(SFURL, Creds, IsSSL);

            return _ica;
        }
        public async static Task<string> RetreiveICA(string Server, string WebLocation, CitrixAuthCredential Creds, CitrixApplicationInfo AppInfo, bool UseSSL)
        {
            string SFURL = null;

            if (WebLocation.StartsWith("/"))
            {
                WebLocation = WebLocation.Substring(1, WebLocation.Length - 1);
            }

            if (UseSSL)
            {
                SFURL = string.Format("https://{0}/{1}/{2}", Server, WebLocation, AppInfo.AppLaunchURL);
            }
            else
            {
                SFURL = string.Format("http://{0}/{1}/{2}", Server, WebLocation, AppInfo.AppLaunchURL);
            }
            var _ica = await GetICAFromStoreFront(SFURL, Creds, UseSSL);

            return _ica;
        }
        private async static Task<string> GetICAFromStoreFront(string SFURL, CitrixAuthCredential Creds, bool UseSSL)
        {
            CookieContainer _cookieContainer = new CookieContainer();
			Uri _cookieUri = new Uri(SFURL);
            Cookie _aspnetSessionIdCookie = new Cookie("ASP.NET_SessionId", Creds.SessionID, Creds.CookiePath, Creds.CookieHost);
            Cookie _csrfTokenCookie = new Cookie("CsrfToken", Creds.CSRFToken, Creds.CookiePath, Creds.CookieHost);
            Cookie _authIDCookie = new Cookie("CtxsAuthId", Creds.AuthToken, Creds.CookiePath, Creds.CookieHost);
            _cookieContainer.Add(_cookieUri,_aspnetSessionIdCookie);
            _cookieContainer.Add(_cookieUri,_csrfTokenCookie);
            _cookieContainer.Add(_cookieUri,_authIDCookie);

            HttpClientHandler _clientHandler = new HttpClientHandler();
            _clientHandler.CookieContainer = _cookieContainer;

            System.Net.Http.HttpClient _client = new System.Net.Http.HttpClient(_clientHandler);
            //_client.BaseAddress = new Uri(SFURL);

            if (UseSSL)
            {
                _client.DefaultRequestHeaders.Add("X-Citrix-IsUsingHTTPS", "Yes");
            }
            else
            {
                _client.DefaultRequestHeaders.Add("X-Citrix-IsUsingHTTPS", "No");
            }

            _client.DefaultRequestHeaders.Add("Csrf-Token", Creds.CSRFToken);
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/octet-stream"));

            StringContent _bodyContent = new StringContent("");

            HttpResponseMessage _icaResponse = await _client.GetAsync(SFURL);

            string _icaFile = null;

            if ( _icaResponse.StatusCode == HttpStatusCode.OK)
            {
                _icaFile = await _icaResponse.Content.ReadAsStringAsync();
            }

            return _icaFile;
        }
    }
}
