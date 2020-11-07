using InsstagramTool.ObjectData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ControllerInstagram
{
    public delegate void ActionInvoke();
    public class Action
    {
        public static String PATH_INFOR = "inforLogin.ini";
        public static void SaveInfor( String cookie, String id,String username,String CsrfToken)
        {
            try
            {
                File.WriteAllLines(PATH_INFOR, new String[] { cookie ,id,username,CsrfToken});
            }catch(Exception ex)
            {
                throw ex;
            }
        }
        public static String[] ReadInfor()
        {
            try
            {
                return File.ReadAllLines(PATH_INFOR);
            }catch(Exception ex)
            {
                throw ex;
            }
        }
        public static void InvokeRun(Control control, ActionInvoke action)
        {
            control.Invoke(new MethodInvoker(action));
        }
        public static void Download(CustomResourcePostData postData, String path) 
        {
            try
            {
                if (postData.is_video)
                    RequestCustom.DOWNLOAD(postData.video_url,path+".mp4","", "", "");
                else
                    RequestCustom.DOWNLOAD(postData.display_desources[postData.display_desources.Length-1].src, path+".jpg", "", "", "");
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

    }
}
