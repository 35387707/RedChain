using RelexBarBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RelaxBarWeb_MVC.Controllers
{
    public class VerifycodeController : BaseController
    {
        public void getVerifyCode(string t, string r, string b)
        {
            if (string.IsNullOrEmpty(t) || t == "1")//图片验证码
            {
                var q = ImgVerifyCode();

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    Response.ClearContent();
                    Response.ContentType = "image/jpeg";
                    q.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    Response.BinaryWrite(ms.ToArray());
                    q.Dispose();
                    Response.End();
                }
            }
            else if (t == "2")//手机验证码
            {
                if (Common.IsPhone(r))
                {
                    b = b.Replace(" ", "").Replace("+", "");
                    if (string.IsNullOrEmpty(b))
                        b = "86";
#if !DEBUG
                    if (Session["smstime"] != null)
                    {
                        var dt = DateTime.Parse(Session["smstime"].ToString());
                        if (dt.AddMinutes(1) > DateTime.Now)//时间还未过期，一分钟之内不允许发第二次验证码
                        {
                            WriteFaild();
                            return;
                        }
                    }
#endif

                    Session["smstime"] = DateTime.Now;
                    Session["before"] = b;
                    Session["Phone"] = r;
                    SmsVerifyCode(b, r);
                    WriteSuccess();
                }
                else
                {
                    WriteFaild();
                }
            }
            else if (t == "3")//邮箱验证码
            {
                if (Common.IsEmail(r))
                {
                    //EmailVerifyCode(r);
                    LogsBLL.InsertAPILog("getVerifyCode", Guid.Empty, "验证码邮箱：" + r);
                    Session["email"] = r;
                    WriteSuccess();
                }
                else
                {
                    WriteFaild();
                }
            }
        }

        public System.Drawing.Bitmap ImgVerifyCode()
        {
            string code;
            var q = Common.SendImgVerify(out code, 4, true);
            VerifyCode = code;

            VerifyCodesBLL verbll = new VerifyCodesBLL();
            verbll.InsertCode(UserInfo == null ? Guid.Empty : UserInfo.ID, code, Common.enCodeType.Img);

            return q;
        }
        public void SmsVerifyCode(string before, string recphone)
        {
            string code;
            Common.SendSmsVerify(out code, before, recphone);
            VerifyCode = code;

            VerifyCodesBLL verbll = new VerifyCodesBLL();
            verbll.InsertCode(UserInfo != null ? UserInfo.ID : Guid.Empty, code, Common.enCodeType.SMS);
        }
        public void EmailVerifyCode(string recEmail)
        {
            string code;

            Common.SendEmailVerify(out code, recEmail);
            VerifyCode = code;
            VerifyCodesBLL verbll = new VerifyCodesBLL();
            verbll.InsertCode(UserInfo != null ? UserInfo.ID : Guid.Empty, code, Common.enCodeType.Email);
        }
        public void WriteSuccess()
        {
            Response.Write("1");
        }
        public void WriteFaild()
        { Response.Write("0"); }
    }
}