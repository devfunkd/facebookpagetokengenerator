# facebookpagetokengenerator
Quickly and easily generate permanent page tokens for Facebook Pages

HOW TO POST TO YOUR PAGE USING FACEBOOK C# SDK

1) Create a Facebook App at: developers.facebook.com and get yourself an APPID and APPSECRET. (there are a lot of tutorials online for doing this so I will skip repeating it)
2) Go to: http://developers.facebook.com/tools/explorer and choose your app from the dropdown and click "generate access token".
3) Now take the access token, appid and appsecret you have and enter them into the tool and click GENERATE PERMANENT PAGE TOKEN


=======================================================================
Ok at this point you should have your APPID, APPSECRET and a PERMANENT PAGE TOKEN.
=======================================================================
In your Visual Studio solution:
4) Using Nuget:Install-Package Facebook
5) Implement the Facebook client:
public void PostMessage(string message)
        {
            try
            {
                var fb = new FacebookClient
                {
                    AppId = ConfigurationManager.AppSettings.Get("FacebookAppID"),
                    AppSecret = ConfigurationManager.AppSettings.Get("FacebookAppSecret"),
                    AccessToken = ConfigurationManager.AppSettings.Get("FacebookAccessToken")
                };

                dynamic result = fb.Post("me/feed", new
                {
                    message = message
                });
            }
            catch (Exception exception)
            {
                // Handle your exception
            }
        }  
I hope this helps anyone who is struggling to figure this out.
