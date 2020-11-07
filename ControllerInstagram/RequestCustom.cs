using System;
using System.Collections.Generic;
using System.Text;
using xNet;
namespace ControllerInstagram
{
    class RequestCustom
    {

        public static HttpRequest GetRequest(String cookie, String token, String user_agent )
        {
            HttpRequest http = new HttpRequest();
            http.Cookies = new CookieDictionary();
            if (!String.IsNullOrEmpty(cookie))
                AddCookie(http, cookie);
            if (!String.IsNullOrEmpty(token))
                http.AddHeader("x-csrftoken",token);
            if (!String.IsNullOrEmpty(user_agent))
                http.UserAgent = user_agent;
            else
                http.UserAgent = Http.ChromeUserAgent();
            return http;
        }
        public static void AddCookie(HttpRequest http, string cookie)
        {
            var temp = cookie.Split(';');
            foreach (var item in temp)
            {
                var temp2 = item.Trim().Split('=');
                if (temp2.Length > 1)
                {
                    try
                    {
                        http.Cookies.Add(temp2[0], temp2[1]);
                    }
                    catch(Exception ex)
                    {

                    }
                    
                }
            }
        }

        public static String GET(String url, String cookie, String token, String user_agent)
        {
            try
            {
                HttpRequest http = GetRequest(cookie, token, user_agent);
                return http.Get(url).ToString();
            }catch(Exception ex)
            {
                throw ex;
            }
        }
        public static void DOWNLOAD(String url,String path, String cookie, String token, String user_agent)
        {
            try
            {
                HttpRequest http = GetRequest(cookie, token, user_agent);
                http.Get(url).ToFile(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static String POST(String url, String data,String content_type ,String cookie, String token, String user_agent)
        {
            try
            {
                HttpRequest http = GetRequest(cookie, token, user_agent);
                if(String.IsNullOrEmpty(data))
                    return http.Post(url).ToString();
                return http.Post(url,data,content_type).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
