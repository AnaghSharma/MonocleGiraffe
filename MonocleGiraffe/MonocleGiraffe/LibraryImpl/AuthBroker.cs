﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using XamarinImgur.Interfaces;

namespace MonocleGiraffe.LibraryImpl
{
    public class AuthBroker : IAuthBroker
    {
        public async Task<AuthResult> AuthenticateAsync(Uri requestUri, Uri callbackUri)
        {
            var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, requestUri, callbackUri);
            var responseData = ParseAuthResult(result.ResponseData);
            AuthResult ret = new AuthResult(responseData, (AuthResponseStatus)Enum.Parse(typeof(AuthResponseStatus), result.ResponseStatus.ToString()), result.ResponseErrorDetail);
            return ret;
        }

        public static Dictionary<string, string> ParseAuthResult(string result)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            Uri uri = new Uri(result.Replace('#', '&'));
            string query = uri.Query;
            string[] frags = query.Split('&');
            foreach (var frag in frags)
            {
                string[] splits = frag.Split('=');
                ret.Add(splits[0], splits[1]);
            }
            return ret;
        }
    }
}