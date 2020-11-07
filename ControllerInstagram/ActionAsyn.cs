using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ControllerInstagram
{
    public delegate void ErrorEvent(Object ex);
    public class ActionAsyn
    {
        public static async Task RunInit(Instagram instagram, ErrorEvent error)
        {
            await Task.Run(() => {
                try
                {
                    instagram.Init();
                }
                catch(Exception ex)
                {
                    error(ex);
                }
                
            });
        }
        public static async Task RunReadInfor(Instagram instagram, ErrorEvent error )
        {
            await Task.Run(() => {
                try
                {
                    String[] infor = Action.ReadInfor();
                    instagram.Cookie = infor[0];
                    instagram.Id = infor[1];
                    instagram.Username = infor[2];
                    instagram.CsrfToken1 = infor[3];
                }
                catch(Exception ex)
                {
                    error(ex);
                }
            });
        }
        public static async Task RunSaveInfor(Instagram instagram,ErrorEvent error)
        {
            await Task.Run(() => {
                try
                {
                    Action.SaveInfor(instagram.Cookie, instagram.Id, instagram.Username, instagram.CsrfToken1);
                }
                catch (Exception ex)
                {
                    error(ex);
                }
            });
        }
        public static async Task RunDownloadFile(Instagram instagram, String id, ErrorEvent error)
        {
            Task.Run(() =>
            {
                instagram.GetResourcePostUser(id, "");
            });
        }
    }
}
