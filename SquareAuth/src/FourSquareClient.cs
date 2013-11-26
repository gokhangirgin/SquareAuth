using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace DotNetOpenAuth.AspNet.Clients
{
    public class FourSquareClient : OAuth2Client
    {
        /// <summary>
        /// Authorization End Point
        /// </summary>
        private const string AuthorizationEndpoint = "https://foursquare.com/oauth2/authenticate";
        /// <summary>
        /// Access Token End Point
        /// </summary>
        private const string TokenEndPoint = "https://foursquare.com/oauth2/access_token";
        /// <summary>
        /// Consumer Key
        /// </summary>
        private string ClientId;
        /// <summary>
        /// Consumer Secret
        /// </summary>
        private string ClientSecret;
        public FourSquareClient(string _clientId, string _clientSecret) : base("FourSquare")
        {
            ClientId = _clientId;
            ClientSecret = _clientSecret;
        }
        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("client_id", ClientId);
            query.Add("redirect_uri", returnUrl.ToString());
            query.Add("response_type", "code");
            return new UriBuilder(AuthorizationEndpoint) { Query = query.ToString() }.Uri;
        }
        private Dictionary<string, string> _userData = new Dictionary<string, string>();
        private const string profileBase = "https://api.foursquare.com/v2/users/self?oauth_token={0}";
        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(profileBase,accessToken));
            request.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using(Stream stream = response.GetResponseStream())
            using(StreamReader sr = new StreamReader(stream))
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(sr.ReadToEnd());
                dynamic user = obj.response.user;
                _userData.Add("id",Convert.ToString(user.id));
                _userData.Add("firstName",Convert.ToString(user.firstName));
                _userData.Add("lastName",Convert.ToString(user.lastName));
                /*
                 Have a look at for getting more information https://developer.foursquare.com/docs/responses/user
                 */
            }
            return _userData;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("client_id", ClientId);
            query.Add("client_secret", ClientSecret);
            query.Add("code", authorizationCode);
            query.Add("grant_type", "authorization_code");
            query.Add("redirect_uri", returnUrl.ToString());
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(TokenEndPoint);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (Stream rstream = request.GetRequestStream())
            using (StreamWriter sw = new StreamWriter(rstream))
            {
                sw.Write(query.ToString());
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(stream))
            {
                //basic user info through access_token request that maps to extraData
                //http://instagram.com/developer/authentication/
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(sr.ReadToEnd());
                _userData.Add("access_token", Convert.ToString(obj.access_token));
            }
            return _userData["access_token"];
        }
    }
}