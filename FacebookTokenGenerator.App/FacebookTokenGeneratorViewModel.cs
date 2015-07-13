using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Facebook;

namespace FacebookTokenGenerator.App
{
    public class FacebookTokenGeneratorViewModel : INotifyPropertyChanged
    {
        #region Xaml Bindings

        private string _appId;

        public string AppId
        {
            get { return _appId; }
            set
            {
                if (value != _appId)
                {
                    _appId = value;
                    OnPropertyChanged("AppId");
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
                    OnPropertyChanged("AppSecret");
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
                    OnPropertyChanged("AppToken");
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
                    OnPropertyChanged("PageToken");
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
                    OnPropertyChanged("PageName");
                }
            }
        }

        private string _page;

        public string Page
        {
            get { return _page; }
            set
            {
                if (value != _page)
                {
                    _page = value;
                    OnPropertyChanged("Page");
                }
            }
        }

        private ObservableCollection<ComboBoxItem> _pages;

        public ObservableCollection<ComboBoxItem> Pages
        {
            get { return _pages; }
            set
            {
                if (value != _pages)
                {
                    _pages = value;
                    OnPropertyChanged("Pages");
                }
            }
        }

        private bool _showFacebookPages;

        public bool ShowFacebookPages
        {
            get { return _showFacebookPages; }
            set
            {
                if (value != _showFacebookPages)
                {
                    _showFacebookPages = value;
                    OnPropertyChanged("ShowFacebookPages");
                }
            }
        }


        #endregion

        private string AccountId { get; set; }
        private string LongLivedAccessToken { get; set; }

        private List<FacebookModel> FacbookPages { get; set; }

        public void TokenGenerator()
        {
            _showFacebookPages = false;

            var tokenUri = new Uri(
                string.Format(
                    "https://graph.facebook.com/v2.4/oauth/access_token?grant_type=fb_exchange_token&client_id={0}&client_secret={1}&fb_exchange_token={2}", _appId, _appSecret, _appToken));

            HttpClient client = new HttpClient();
            var response = Task.Run(async () =>
            {
                var res = client.GetAsync(tokenUri).Result;
                return res;
            });

            if (response.Result.StatusCode != HttpStatusCode.OK)
            {
                var responseError = response.Result.Content.ReadAsStringAsync();
                var messageText = string.Format("{0}", responseError.Result);
                MessageBox.Show(messageText, response.Result.StatusCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var responseString = response.Result.Content.ReadAsStringAsync();
            var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString.Result);
            LongLivedAccessToken = jsonObject.access_token.ToString();

            GetAccessToken();
        }

        private void GetAccessToken()
        {
            if (string.IsNullOrEmpty(LongLivedAccessToken)) return;

            var tokenUri = new Uri(
                string.Format(
                    "https://graph.facebook.com/v2.4/me?access_token={0}", LongLivedAccessToken));

            HttpClient client = new HttpClient();
            Task<HttpResponseMessage> response = Task.Run(async () =>
            {
                var res = client.GetAsync(tokenUri).Result;
                return res;
            });

            if (response.Result.StatusCode != HttpStatusCode.OK)
            {
                var responseError = response.Result.Content.ReadAsStringAsync();
                var messageText = string.Format("{0}", responseError.Result);
                MessageBox.Show(messageText, response.Result.StatusCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var responseString = response.Result.Content.ReadAsStringAsync();
            var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString.Result);
            AccountId = jsonObject.id.ToString();

            GetFacebookPages();
        }

        private void GetFacebookPages()
        {
            if (string.IsNullOrEmpty(AccountId)) return;
            if (string.IsNullOrEmpty(LongLivedAccessToken)) return;

            var pageTokenUri = new Uri(string.Format("https://graph.facebook.com/v2.4/{0}/accounts?access_token={1}", AccountId, LongLivedAccessToken));

            HttpClient client = new HttpClient();

            Task<HttpResponseMessage> response = Task.Run(async () =>
            {
                var res = client.GetAsync(pageTokenUri).Result;
                return res;
            });

            if (response.Result.StatusCode != HttpStatusCode.OK)
            {
                var responseError = response.Result.Content.ReadAsStringAsync();
                var messageText = string.Format("{0}", responseError.Result);
                MessageBox.Show(messageText, response.Result.StatusCode.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var responseString = response.Result.Content.ReadAsStringAsync();
            var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseString.Result);
            var dataObject = jsonObject.data.ToString();

            FacbookPages = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FacebookModel>>(dataObject);

            _pages = new ObservableCollection<ComboBoxItem>(FacbookPages.Select(facebookPage => new ComboBoxItem() { Content = facebookPage.name }).ToList());
            _showFacebookPages = true;

            OnPropertyChanged("Pages");
            OnPropertyChanged("ShowFacebookPages");
        }

        public void GetPermanentPageAccessToken(string pageName)
        {
            if(pageName == null) return;

            var facebookPage = FacbookPages.FirstOrDefault(o => o.name == pageName);
            if (facebookPage != null)
            {
                var permanentPageToken = string.Format("[{0}]: {1}", facebookPage.name, facebookPage.access_token);
                _pageToken = facebookPage.access_token;
                OnPropertyChanged("PageToken");
            }
            else
            {
                MessageBox.Show("An unknown error occurred, please try again.");
            }           
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
