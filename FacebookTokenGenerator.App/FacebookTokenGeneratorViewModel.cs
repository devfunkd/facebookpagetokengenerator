using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Facebook;

namespace FacebookTokenGenerator.App
{
    public class FacebookTokenGeneratorViewModel : INotifyPropertyChanged
    {
        private string _appId;

        public string AppId
        {
            get { return _appId; }
            set
            {
                if (value != _appId)
                {
                    _appId = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _appSecret;

        public string AppSecret
        {
            get { return _appSecret; }
            set
            {
                if (value != _appSecret)
                {
                    _appSecret = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _appToken;

        public string AppToken
        {
            get { return _appToken; }
            set
            {
                if (value != _appToken)
                {
                    _appToken = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _pageToken;

        public string PageToken
        {
            get { return _pageToken; }
            set
            {
                if (value != _pageToken)
                {
                    _pageToken = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _pageName;

        public string PageName
        {
            get { return _pageName; }
            set
            {
                if (value != _pageName)
                {
                    _pageName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _pageId;

        public string PageId
        {
            get { return _pageId; }
            set
            {
                if (value != _pageId)
                {
                    _pageId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TokenGenerator()
        {
            var tokenUri = new Uri(
                string.Format(
                    "https://graph.facebook.com/v2.4/oauth/access_token?grant_type=fb_exchange_token&client_id={0}&client_secret={1}&fb_exchange_token={2}", _appId, _appSecret, _appToken));

            HttpClient client = new HttpClient();
            var response = Task.Run(async () =>
            {
                var res = client.GetAsync(tokenUri).Result;
                return res;
            });

            response.Result.EnsureSuccessStatusCode();

            var responseString = response.Result.Content.ReadAsStringAsync();
            var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString.Result);
            var accessToken = jsonObject.access_token.ToString();

            return GetAccessToken(accessToken);
        }

        private string GetAccessToken(string longLivedAccessToken)
        {
            var tokenUri = new Uri(
                string.Format(
                    "https://graph.facebook.com/v2.4/me?access_token={0}", longLivedAccessToken));

            HttpClient client = new HttpClient();
            Task<HttpResponseMessage> response = Task.Run(async () =>
            {
                var res = client.GetAsync(tokenUri).Result;
                return res;
            });
            response.Result.EnsureSuccessStatusCode();

            var responseString = response.Result.Content.ReadAsStringAsync();
            var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString.Result);
            var accountId = jsonObject.id.ToString();

            return GetPermanentPageAccessToken(accountId, longLivedAccessToken);
        }

        private string GetPermanentPageAccessToken(string accountId, string longLivedAccessToken)
        {
            var pageTokenUri = new Uri(string.Format("https://graph.facebook.com/v2.4/{0}/accounts?access_token={1}", accountId, longLivedAccessToken));

            HttpClient client = new HttpClient();
            Task<HttpResponseMessage> response = Task.Run(async () =>
            {
                var res = client.GetAsync(pageTokenUri).Result;
                return res;
            });
            response.Result.EnsureSuccessStatusCode();

            var responseString = response.Result.Content.ReadAsStringAsync();
            var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString.Result);
            var dataObject = jsonObject.data.ToString();
            List<FacebookModel> jsonObjectData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FacebookModel>>(dataObject);

            var facebookPage = jsonObjectData.FirstOrDefault(x => x.id == _pageId);
            if (facebookPage != null)
            {
                var permanentPageToken = string.Format("[{0}]: {1}", facebookPage.name, facebookPage.access_token);
                return permanentPageToken;
            }

            return "COULD NOT RETRIEVE PERMANENT PAGE TOKEN";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
