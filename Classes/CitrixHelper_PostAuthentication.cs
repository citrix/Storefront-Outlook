using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StorefrontApps_Outlook.Classes.CitrixHelper
{
    public partial class Storefront
    {
        public async static Task<CitrixAuthCredential> AuthenticateWithPost(string WebURL, string Username, string Password, string Domain)
        {
            string SFURL = WebURL;
            bool IsSSL = false;
            if (SFURL.ToLower().IndexOf("https:") != -1)
            {
                IsSSL = true;
            }
            else
            {
                IsSSL = false;
            }

            var _creds = await GetCredentialsFromStoreFront(SFURL, Username, Password, Domain, IsSSL);

            return _creds;
        }
        public async static Task<CitrixAuthCredential> AuthenticateWithPost(string Server, string WebLocation, string Username, string Password, string Domain, bool IsSSL)
        {
            string SFURL = null;

            if ( WebLocation.StartsWith("/"))
            {
                WebLocation = WebLocation.Substring(1, WebLocation.Length - 1);
            }
            if (IsSSL)
            {
                SFURL = string.Format("https://{0}/{1}", Server, WebLocation);
            }
            else
            {
                SFURL = string.Format("http://{0}/{1}", Server, WebLocation);
            }

            var _creds = await GetCredentialsFromStoreFront(SFURL, Username, Password, Domain, IsSSL);

            return _creds;
        }
        private async static Task<CitrixAuthCredential> GetCredentialsFromStoreFront(string SFURL, string Username, string Password, string Domain, bool IsSSL)
        {
            CitrixAuthCredential _sfCredential = null;

            Dictionary<string, string> _returnedValues = new Dictionary<string, string>();
            string _csrfToken = Guid.NewGuid().ToString();
            string _aspnetSessionID = Guid.NewGuid().ToString();

            string _username = Username;
            string _password = Password;
            string _domain = Domain;

            string _authenticationBody = string.Format(@"username={0}\{1}&password={2}", _domain, _username, _password);

            CookieContainer _cookieContainer = new CookieContainer();
            HttpClientHandler _clientHandler = new HttpClientHandler();
            _clientHandler.CookieContainer = _cookieContainer;

            System.Net.Http.HttpClient _client = new System.Net.Http.HttpClient(_clientHandler);

            _client.BaseAddress = new Uri(SFURL);

            string _postAuthUrl = (SFURL.EndsWith("/")) ? "PostCredentialsAuth/Login" : "/PostCredentialsAuth/Login";

            if (IsSSL)
            {
                _client.DefaultRequestHeaders.Add("X-Citrix-IsUsingHTTPS", "Yes");
            }
            else
            {
                _client.DefaultRequestHeaders.Add("X-Citrix-IsUsingHTTPS", "No");
            }
            _client.DefaultRequestHeaders.Add("Csrf-Token", _csrfToken);
            

            StringContent _bodyContent = new StringContent(_authenticationBody, Encoding.UTF8, "application/x-www-form-urlencoded");
            _bodyContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            //HttpResponseMessage _authResp = await _client.PostAsync(string.Format("{0}{1}",SFURL ,_postAuthUrl), _bodyContent);
            HttpResponseMessage _authResp = await _client.PostAsync( _postAuthUrl, _bodyContent);

            if (_authResp.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(String.Format("Error: {0}", _authResp.ReasonPhrase));
            }
            else
            {
                /*
                <?xml version="1.0" encoding="UTF-8"?>
                <AuthenticationStatus xmlns="http://citrix.com/deliveryservices/webAPI/2-6/authStatus">
                <Result>success</Result>
                <AuthType>Certificate</AuthType>
                </AuthenticationStatus>
                */
                string _returnedContent = await _authResp.Content.ReadAsStringAsync();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(_returnedContent);

                XmlNamespaceManager _namespaceMgr = new XmlNamespaceManager(doc.NameTable);
                _namespaceMgr.AddNamespace("mockns", @"http://citrix.com/delivery-services/webAPI/2-6/authStatus");
                XmlNode _resultAuthNode = doc.SelectSingleNode("//mockns:Result", _namespaceMgr);
                if (_resultAuthNode != null)
                {
                    if (_resultAuthNode.InnerText.ToLower() == "success")
                    {
                        string _cookiePath = "/";
                        string _cookieHost = _authResp.RequestMessage.RequestUri.Host;

                        foreach (var header in _authResp.Headers.Where(i => i.Key == "Set-Cookie"))
                        {
                            foreach (string cookieValue in header.Value)
                            {
                                string[] cookieElements = cookieValue.Split(';');
                                string[] keyValueElements = cookieElements[0].Split('=');

                                _returnedValues.Add(keyValueElements[0], keyValueElements[1]);
                            }
                        }

                        _sfCredential = new CitrixAuthCredential
                        {
                            AuthToken = _returnedValues["CtxsAuthId"].ToString(),
                            CSRFToken = _returnedValues["CsrfToken"].ToString(),
                            SessionID = _returnedValues["ASP.NET_SessionId"].ToString(),
                            CookiePath = _cookiePath,
                            CookieHost = _cookieHost
                        };
                    }
                }
                else
                {
                    _sfCredential = null;
                }
            }
            return _sfCredential;
        }

    }


}
