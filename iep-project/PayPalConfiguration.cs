using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iep_project
{
    public class PayPalConfiguration
    {

        public static APIContext ApiContext { get {
                var config = ConfigManager.Instance.GetProperties();
                var accessToken = new OAuthTokenCredential(config).GetAccessToken();
                return new APIContext(accessToken);
            }
            }

        public static void ConfigurePayPal()
        {
            
        }
    }
}