using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RelexBarBLL
{
    public class Common
    {
        #region Fields

        /// <summary>
        /// 最顶层的用户ID，也就是为空的
        /// </summary>
        public const string TOPUSER = "00000000-0000-0000-0000-000000000000";

        #endregion

        #region Verify

        /// <summary>
        /// 是否手机号码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsPhone(string phone)
        {
            //return !string.IsNullOrWhiteSpace(phone) && phone.Length == 11 && phone.StartsWith("1");
            return !string.IsNullOrWhiteSpace(phone);
        }

        /// <summary>
        /// 是否邮箱号码
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email, "^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$");
            //return new System.Text.RegularExpressions.Regex("^[a-z0-9_-\\.]+@[a-z0-9_-]+(\\.[a-z0-9_-]+)+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase).IsMatch(email);
        }

        #endregion

        #region Function

        public static int GetPageIndex(int pageinex)
        {
            return pageinex > 0 ? (pageinex - 1) : 0;
        }

        public static string MD5(string source)
        {
            return CommonClass.EncryptDecrypt.GetMd5Hash(source + SysConfigBLL.MD5Key);
        }

        public static string Encrypt(string source)
        {
            return CommonClass.EncryptDecrypt.DESEncrypt(source, SysConfigBLL.MD5Key);
        }

        public static string Decrypt(string source)
        {
            return CommonClass.EncryptDecrypt.DESDecrypt(source, SysConfigBLL.MD5Key);
        }

        /// <summary>
        /// 加密，并加上时间戳
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string EncryptWithTime(string source)
        {
            return Encrypt(source + "||ts=" + GetTimeStamp(DateTime.Now));
        }

        /// <summary>
        /// 加密，并加上时间戳
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string EncryptWithTime(string source, DateTime dtVal)
        {
            return Encrypt(source + "||ts=" + GetTimeStamp(dtVal));
        }

        /// <summary>
        /// 解密，并判断时间戳
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string DecryptWithTime(string source, out DateTime dtVal)
        {
            string result = Decrypt(source);
            dtVal = DateTime.MinValue;
            if (!string.IsNullOrEmpty(result))
            {
                int ls = result.LastIndexOf("||ts=");
                if (ls > 0)
                {
                    string ts = result.Substring(ls + 5);
                    if (!string.IsNullOrEmpty(ts))
                    {
                        dtVal = GetTime(ts);
                    }
                    result = result.Substring(0, ls);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            return GetTimeStamp(DateTime.Now);
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp(DateTime val)
        {
            return ((val.ToUniversalTime().Ticks - 621355968000000000) / 10000).ToString();
        }

        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime GetTime(string timeStamp)
        {
            try
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = long.Parse(timeStamp) * 10000;
                TimeSpan toNow = new TimeSpan(lTime);
                return dtStart.Add(toNow);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        public static DateTime? GetTime2(string timeStamp)
        {
            try
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = long.Parse(timeStamp) * 10000;
                TimeSpan toNow = new TimeSpan(lTime);
                return dtStart.Add(toNow);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取商品流水号
        /// </summary>
        /// <returns></returns>
        public static string GetNumer()
        {
            return "PD" + new Random().Next(100, 999) + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        /// <summary>
        /// 获取购物单流水号
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNumer()
        {
            return "OD" + new Random().Next(100, 999) + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        /// <summary>
        /// 获取第三方购物单流水号
        /// </summary>
        /// <returns></returns>
        public static string GetServiceNumer()
        {
            return "SN" + new Random().Next(100, 999) + DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        /// <summary>
        /// 获取红包流水号
        /// </summary>
        /// <returns></returns>
        public static string GetRedListNumer()
        {
            return new Random().Next(10, 99) + DateTime.Now.ToString("yyyyMMddHHmmssfff").Substring(2);
        }

        public static string GetUserShowName(RelexBarDLL.Users user)
        {
            if (user == null)
                return string.Empty;
            return user.RealCheck == (int)enRealCheckStatus.已验证 ? user.TrueName : HidePhone(user.Phone);
        }

        public static string HideSomeChar(string source, int begin, int length)
        {
            return HideSomeChar(source, begin, length, source.Length, '*');
        }
        public static string HideSomeChar(string source, int begin, int length, int maxLength, char replaceChar)
        {
            if (maxLength < source.Length)
            {
                source = source.Substring(0, maxLength);
            }

            string result = string.Empty;
            if (source.Length > begin && length > 0 && source.Length >= begin + length)
            {
                result = source.Substring(0, begin);//开头
                result = result.PadRight(begin + length, replaceChar);
                result += source.Substring(begin + length); //结尾
            }
            else
            {
                result = source;
            }

            return result;
        }
        public static string HidePhone(string phone)
        {
            return HideSomeChar(phone, 3, 4);
        }
        public static string HideBankAccount(string bankaccount)
        {
            return HideSomeChar(bankaccount, 0, bankaccount.Length - 4);
        }

        /// <summary>
        /// 获取随机验证码
        /// </summary>
        /// <param name="len">验证码字数</param>
        /// <param name="numAndchar">是否英文加字母（默认为纯数字）</param>
        /// <returns></returns>
        public static string GetRandomCode(int len, bool numAndchar = false)
        {
            string result = string.Empty;
            string code = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int maxInt = numAndchar ? (code.Length - 1) : 9;
            Random rd = new Random();
            for (int i = 0; i < len; i++)
            {
                result += code[rd.Next(0, maxInt)];
            }
            return result;
        }

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="recphone"></param>
        /// <param name="len"></param>
        /// <param name="numAndchar"></param>
        /// <returns></returns>
        public static bool SendSmsVerify(out string code, string before, string recphone, int len = 6, bool numAndchar = false)
        {
            code = GetRandomCode(len, numAndchar);
            return SendSmsVerify(code, before, recphone);
        }
        public static bool SendSmsVerify(string code, string before, string recphone)
        {
            return new ThirdServices().SendSms(recphone, before, code);
        }

        /// <summary>
        /// 发送图片验证码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="len"></param>
        /// <param name="numAndchar"></param>
        /// <returns></returns>
        public static System.Drawing.Bitmap SendImgVerify(out string code, int len = 6, bool numAndchar = false)
        {
            code = GetRandomCode(len, numAndchar);
            return SendImgVerify(code);
        }
        public static System.Drawing.Bitmap SendImgVerify(string code)
        {
            int width = Convert.ToInt32(code.Length * 12);    //计算图像宽度
            System.Drawing.Bitmap img = new System.Drawing.Bitmap(width, 23);
            System.Drawing.Graphics gfc = System.Drawing.Graphics.FromImage(img);//产生Graphics对象，进行画图
            gfc.Clear(System.Drawing.Color.White);
            drawLine(gfc, img);
            //写验证码，需要定义Font
            System.Drawing.Font font = new System.Drawing.Font("arial", 12, System.Drawing.FontStyle.Bold);
            System.Drawing.Drawing2D.LinearGradientBrush brush =
                new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Rectangle(0, 0, img.Width, img.Height),
                System.Drawing.Color.DarkOrchid, System.Drawing.Color.Blue, 1.5f, true);
            gfc.DrawString(code, font, brush, 3, 2);
            drawPoint(img);
            gfc.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.DarkBlue), 0, 0, img.Width - 1, img.Height - 1);

            gfc.Dispose();
            return img;
        }
        private static void drawLine(System.Drawing.Graphics gfc, System.Drawing.Bitmap img)
        {
            Random ran = new Random();
            //选择画10条线,也可以增加，也可以不要线，只要随机杂点即可
            for (int i = 0; i < 10; i++)
            {
                int x1 = ran.Next(img.Width);
                int y1 = ran.Next(img.Height);
                int x2 = ran.Next(img.Width);
                int y2 = ran.Next(img.Height);
                gfc.DrawLine(new System.Drawing.Pen(System.Drawing.Color.Silver), x1, y1, x2, y2);      //注意画笔一定要浅颜色，否则验证码看不清楚
            }
        }
        private static void drawPoint(System.Drawing.Bitmap img)
        {
            Random ran = new Random();
            /*
            //选择画100个点,可以根据实际情况改变
            for (int i = 0; i < 100; i++)
            {
                int x = ran.Next(img.Width);
                int y = ran.Next(img.Height);
                img.SetPixel(x,y,Color.FromArgb(ran.Next()));//杂点颜色随机
            }
             */
            int col = ran.Next();//在一次的图片中杂店颜色相同
            for (int i = 0; i < 100; i++)
            {
                int x = ran.Next(img.Width);
                int y = ran.Next(img.Height);
                img.SetPixel(x, y, System.Drawing.Color.FromArgb(col));
            }
        }

        public static System.Drawing.Bitmap GetQrCodeImg(string contents)
        {
            CommonClass.QRCode qr = new CommonClass.QRCode();
            return GetQrCodeImgAndLogo(contents, "");
        }
        public static System.Drawing.Bitmap GetQrCodeImgAndLogo(string contents, string logopath)
        {
            CommonClass.QRCode qr = new CommonClass.QRCode();
            return qr.EncodetoBitmap(contents, com.google.zxing.BarcodeFormat.QR_CODE, logopath);
        }
        public static void GetQrCodeImg(string contents, string path)
        {
            CommonClass.QRCode qr = new CommonClass.QRCode();
            qr.EncodeToFile(contents, path);
        }
        public static string GetQrCodeValue(string path)
        {
            CommonClass.QRCode qr = new CommonClass.QRCode();
            return qr.Decode(path);
        }
        /// <summary>
        /// 发送邮件验证码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="recEmail"></param>
        /// <param name="len"></param>
        /// <param name="numAndchar"></param>
        /// <returns></returns>
        public static bool SendEmailVerify(out string code, string recEmail, int len = 6, bool numAndchar = false)
        {
            code = GetRandomCode(len, numAndchar);
            return SendEmailVerify(code, recEmail);
        }
        public static bool SendEmailVerify(string code, string recEmail)
        {
            return new ThirdServices().SendEmail(recEmail, "福包多验证码", "【福包多】您本次操作的验证码为 " + code + " ,10分钟有效,请不要告诉任何人。");
        }
        #endregion

        #region Enum

        /// <summary>
        /// 可用状态
        /// </summary>
        public enum enStatus
        {
            /// <summary>
            /// 可用
            /// </summary>
            Enabled = 1,
            /// <summary>
            /// 不可用
            /// </summary>
            Unabled = 0,
        }

        /// <summary>
        /// 消息状态
        /// </summary>
        public enum enMessageState
        {
            /// <summary>
            /// 可用
            /// </summary>
            Enabled = 1,
            /// <summary>
            /// 不可用
            /// </summary>
            Unabled = 0,
            /// <summary>
            /// 已读
            /// </summary>
            HadRead = 2,
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public enum enMessageType
        {
            /// <summary>
            /// 系统消息
            /// </summary>
            System = 0,
            /// <summary>
            /// 用户消息
            /// </summary>
            Customer = 1,
            /// <summary>
            /// 其他消息
            /// </summary>
            Other = 2,
        }

        /// <summary>
        /// 男女性别
        /// </summary>
        public enum enSex
        {
            Man = 1,
            Women = 0,
        }

        /// <summary>
        /// 红包状态
        /// </summary>
        public enum enPacketStatus
        {
            /// <summary>
            /// 未激活
            /// </summary>
            NoActive = 0,
            /// <summary>
            /// 已激活
            /// </summary>
            Actived = 1,
            /// <summary>
            /// 资格卡激活完毕
            /// </summary>
            ActivedAll = 2,
            /// <summary>
            /// 已领取
            /// </summary>
            Used = 3,
        }

        /// <summary>
        /// 支付进出类型，记账用
        /// </summary>
        public enum enPayInOutType
        {
            /// <summary>
            /// 收入
            /// </summary>
            In = 1,
            /// <summary>
            /// 支出
            /// </summary>
            Out = 0,
        }

        /// <summary>
        /// 获取途径：0充值，1转账，2奖励，3红包，4提现，5返现，6线下支付，7线上支付 8，扫雷游戏
        /// </summary>
        public enum enPayFrom
        {
            /// <summary>
            /// 充值
            /// </summary>
            Recharge = 0,
            /// <summary>
            /// 转账
            /// </summary>
            Exchange = 1,
            /// <summary>
            /// 奖励
            /// </summary>
            Reward = 2,
            /// <summary>
            /// 红包
            /// </summary>
            RedPaged = 3,
            /// <summary>
            /// 提现
            /// </summary>
            Transfor = 4,
            /// <summary>
            /// 返现
            /// </summary>
            Cashback = 5,
            /// <summary>
            /// 线下支付
            /// </summary>
            OutLinePay = 6,
            /// <summary>
            /// 线上支付（商城消费等）
            /// </summary>
            OnLinePay = 7,
            /// <summary>
            /// 领取扫雷红包扫雷游戏
            /// </summary>
            SLGame = 8,
            /// <summary>
            /// 数字游戏
            /// </summary>
            NumberGame = 9,
            /// <summary>
            /// 骰子游戏
            /// </summary>
            TouZiGame = 10,
            /// <summary>
            /// 第三方充值
            /// </summary>
            OtherPay = 11,
            /// <summary>
            /// 游戏
            /// </summary>
            Game = 12,
            /// <summary>
            /// 佣金
            /// </summary>
            Commission = 13,
            /// <summary>
            /// 升级会员
            /// </summary>
            UpgradeLV = 14,
            /// <summary>
            /// 福音圈消费
            /// </summary>
            Infomations = 15,
            /// <summary>
            /// 后台管理员储值
            /// </summary>
            StoredValue = 16,
        }

        /// <summary>
        /// 币种/积分类型
        /// </summary>
        public enum enPayType
        {
            /// <summary>
            /// 都可以
            /// </summary>
            All = 0,
            /// <summary>
            /// 余额/金币
            /// </summary>
            Coin = 1,
            /// <summary>
            /// 积分
            /// </summary>
            Point = 2,
            /// <summary>
            /// 筹码
            /// </summary>
            Chip = 3,
            /// <summary>
            /// 福券
            /// </summary>
            FuQuan = 4,
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        public enum enOrderStatus
        {
            /// <summary>
            /// 取消
            /// </summary>
            Cancel = -1,
            /// <summary>
            /// 下单
            /// </summary>
            Order = 0,
            /// <summary>
            /// 已支付
            /// </summary>
            Payed = 1,
            /// <summary>
            /// 已发货
            /// </summary>
            Sended = 2,
            /// <summary>
            /// 已收货
            /// </summary>
            Recieved = 3,
            /// <summary>
            /// 已完成订单
            /// </summary>
            Completed = 4,
            /// <summary>
            /// 退货中
            /// </summary>
            Return = 5,
        }

        /// <summary>
        /// 订单类型
        /// </summary>
        public enum enOrderType
        {
            /// <summary>
            /// 线上买，线下收货
            /// </summary>
            OnLine = 1,
            /// <summary>
            /// 现场收货，线下购买
            /// </summary>
            Down = 2,
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        public enum enProductType
        {
            /// <summary>
            /// 实体商品
            /// </summary>
            Real = 0,
            /// <summary>
            /// 虚拟商品
            /// </summary>
            Virtual = 1,
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        public enum enPayment
        {
            /// <summary>
            /// 本系统余额？积分支付
            /// </summary>
            LOCAL = 0,
            /// <summary>
            /// 微信
            /// </summary>
            WX = 1,
            /// <summary>
            /// 阿里巴巴
            /// </summary>
            ALI = 2,
            /// <summary>
            /// 威富通
            /// </summary>
            WFT = 3,
        }

        /// <summary>
        /// 用户类型
        /// </summary>
        public enum enUserType
        {
            /// <summary>
            /// 会员
            /// </summary>
            User = 0,
            /// <summary>
            /// 商家
            /// </summary>
            Shop = 1,
            /// <summary>
            /// 代理
            /// </summary>
            Agent = 2,
        }

        /// <summary>
        /// 商家类型
        /// </summary>
        public enum enShopType
        {
            /// <summary>
            /// 自营
            /// </summary>
            Self = 0,
            /// <summary>
            /// 联盟
            /// </summary>
            Member = 1,
        }

        public enum enAdminPower
        {
            /// <summary>
            /// 超级管理员
            /// </summary>
            Super = 10,
            Normal = 1,
        }

        /// <summary>
        /// 日志类型
        /// </summary>
        public enum enLogType
        {
            /// <summary>
            /// 错误
            /// </summary>
            Error = -1,
            /// <summary>
            /// 普通
            /// </summary>
            None = 0,
            /// <summary>
            /// 登录
            /// </summary>
            Login = 1,
            /// <summary>
            /// 支付
            /// </summary>
            Pay = 2,
            /// <summary>
            /// 充值，转账
            /// </summary>
            Recharge = 3,
            /// <summary>
            /// 红包
            /// </summary>
            Redpackage = 4,
            /// <summary>
            /// 订单
            /// </summary>
            Order = 5,
            /// <summary>
            /// 资料
            /// </summary>
            Info = 6,
            /// <summary>
            /// 提现
            /// </summary>
            Transferout = 7,
            /// <summary>
            /// 短信
            /// </summary>
            SMS = 8,
            /// <summary>
            /// 邮件
            /// </summary>
            Email = 9,
            /// <summary>
            /// 用户操作
            /// </summary>
            User = 10,
            /// <summary>
            /// 接口错误
            /// </summary>
            Services = 11,
            /// <summary>
            /// api调用
            /// </summary>
            API = 12,
            /// <summary>
            /// 管理员操作
            /// </summary>
            Admin = 13,
        }

        /// <summary>
        /// 实名制验证状态
        /// </summary>
        public enum enRealCheckStatus
        {
            /// <summary>
            /// 未验证
            /// </summary>
            未验证 = 0,
            /// <summary>
            /// 审核中
            /// </summary>
            审核中 = 1,
            /// <summary>
            /// 已验证
            /// </summary>
            已验证 = 2,
            /// <summary>
            /// 不通过
            /// </summary>
            不通过 = 3,
        }

        /// <summary>
        /// 提现申请状态
        /// </summary>
        public enum enApplyStatus
        {
            Faild = -1,
            Normal = 0,
            Success = 1,
        }

        /// <summary>
        /// 验证码类型（手机、邮箱、图片、文字、线下扫码支付）
        /// </summary>
        public enum enCodeType
        {
            SMS = 1,
            Email = 2,
            Img = 3,
            Text = 4,
            Pay = 5,
        }

        /// <summary>
        /// 错误编码
        /// </summary>
        public enum ErrorCode
        {
            没有错误 = 0,

            数据库操作失败 = -89,

            重新登陆 = -97,
            账号在其他地方登陆 = -98,
            账号不存在 = -99,
            密码不正确 = -100,
            姓名不正确 = -101,
            手机不正确 = -102,
            账户余额不足 = -103,
            账号尚未实名制 = -104,
            银行卡不存在 = -105,
            账号不可用 = -106,
            账号类型不正确 = -107,
            账户积分不足 = -108,
            密码格式不正确 = -109,
            账号已被注册 = -110,
            微信已被注册 = -111,
            密码尚未设置 = -112,
            支付密码不正确 = -113,
            未设置支付密码 = -114,
            账户福券不足 = -115,
            推荐人不存在 = -116,

            验证码已过期 = -399,
            请先获取验证码 = -400,
            验证码不正确 = -401,
            验证码错误次数过多 = -402,
            验证码异常 = -403,

            商品不存在 = -999,
            商品不可购买 = -1000,
            商品数量不足 = -1001,
            订单异常 = -1002,
            订单已支付 = -1003,
            商品规格不正确 = -1004,
            收货地址不存在 = -1005,

            福包不存在 = -9999,
            福包未激活 = -9998,
            福包已被领取完 = -9997,
            您已领过福包 = -9996,
            您不符合领取福包条件 = -9995,
            您已经点赞过 = -9994,
            您已收藏过 = -9993,
            您今日领取福包已达到上限 = -9992,

            状态异常或已处理 = -99998,
            未知异常 = -999999,

            参数错误 = -2000,
            身份验证失败 = -90001,
            支付失败 = -90002,
            数据不存在 = -90003,
            数据状态异常 = -90004,
            AppID不正确 = -90005,
            签名验证失败 = -90006,
            Token无效 = -90007,
            订单创建失败 = -90008,
            订单不存在 = -90009,
            充值失败 = -90010,
            支付密码错误 = -90011,
            您不能操作他人订单 = -90012,

            群组不存在 = -30000,
            Email已存在 = -90012,
            手机号已存在 = -90013,
            转账记录不存在 = -90014,
            已领取或已退还 = -90015,
            金额读取失败 = -90016,

            相册不存在 = -90100,
            文件为空 = -90101,
            文件类型不正确 = -90102,

            权限不足 = -11,

            暂无此操作权限 = -12,
        }

        public enum enReportType
        {
            Register = 0,
            Login = 1,
            NewFriend = 2,

            ReCharge = 3,
            Exchange = 4,
            TransOut = 5,

            Order = 6,
            Payed = 7,

            BuyCard = 8,//购买轻客
            PayMoney = 9,//总消费金额
            NewUser = 10,//新用户
        }

        public enum enOSType
        {
            Android = 0,
            IOS = 1,
        }

        /// <summary>
        /// 发红包的类型
        /// </summary>
        public enum enRedType
        {
            /// <summary>
            /// 用户/商家主动发红包（用户主动发福包）（第三方支付完成的）
            /// </summary>
            Single_OtherPay = 0,
            /// <summary>
            /// 用户/商家主动发红包（用户主动发福包）
            /// </summary>
            Single = 1,
            /// <summary>
            /// 定时自动发红包（如商家交钱、代理商交钱）（升级福相/福将福包）
            /// </summary>
            Timeout = 2,
            /// <summary>
            /// 系统发红包（由系统达到某条件，或者后台管理员点击发出）（系统发福包）
            /// </summary>
            System = 3,
            /// <summary>
            /// 达到条件自动发红包 （共享福包）
            /// </summary>
            Auto_UserRecRed = 4,
            /// <summary>
            /// 升级一个代理自动发红包  （升级福相系统发福包）
            /// </summary>
            Auto_NewAgentSend = 5,
            /// <summary>
            /// 公司业绩自动发红包  （超级福包）
            /// </summary>
            Auto_SystemAchieveSend = 6,
            /// <summary>
            /// 所有粉丝、广告商、代理商的奖金系统自动扣除10%用来发红包 （直推/间推/团队福包分润）
            /// </summary>
            Auto_RewardSend = 7,
        }

        /// <summary>
        /// 收藏类型
        /// </summary>
        public enum enMycollectionType
        {
            /// <summary>
            /// 红包
            /// </summary>
            RedPacket = 0,
            /// <summary>
            /// 用户
            /// </summary>
            User = 1,
            /// <summary>
            /// 消息
            /// </summary>
            Message = 2,
            /// <summary>
            /// 广告圈
            /// </summary>
            Information = 3,
            /// <summary>
            /// 商品
            /// </summary>
            Product = 4,
        }

        /// <summary>
        /// 点赞或是访问
        /// </summary>
        public enum enViewGood
        {
            /// <summary>
            /// 访问
            /// </summary>
            View = 1,
            /// <summary>
            /// 点赞
            /// </summary>
            Good = 0,
        }


        #endregion

        public static decimal ToFixed(decimal d, int s)
        {
            decimal sp = Convert.ToDecimal(Math.Pow(10, s));

            if (d < 0)
                return Math.Truncate(d) + Math.Ceiling((d - Math.Truncate(d)) * sp) / sp;
            else
                return Math.Truncate(d) + Math.Floor((d - Math.Truncate(d)) * sp) / sp;
        }

        /// <summary>
        /// 将指定字符串按指定长度进行剪切，    
        /// </summary>
        /// <param name="oldStr">需要截断的字符串 </param>
        /// <param name="maxLength">字符串的最大长度 </param>
        /// <returns></returns>
        public static string StringTruncat(string oldStr, int maxLength)
        {
            if (string.IsNullOrEmpty(oldStr))
                //   throw   new   NullReferenceException( "原字符串不能为空 ");    
                return oldStr;
            if (maxLength < 1)
                throw new Exception("返回的字符串长度必须大于[0] ");
            if (oldStr.Length > maxLength)
            {
                string strTmp = oldStr.Substring(0, maxLength);
                return strTmp + "...";
            }
            return oldStr;
        }


        /// <summary>
        /// 权限与数据库对应
        /// </summary>
        public enum PermissionName
        {
            首页 = 5,
            用户管理 = 7,
            商品管理 = 8,
            订单管理 = 9,
            财务管理 = 10,
            营销管理 = 11,
            福音天地 = 12,
            关于系统 = 13,
            问题反馈 = 14,
            系统通知 = 15,
            联系我们 = 16,
            帮助中心 = 17,
            权限管理 = 18,
            用户新增 = 21,
            用户编辑 = 22,
            用户删除 = 24,
            用户查看 = 25,
            用户审核 = 71,
            商品新增 = 27,
            商品编辑 = 28,
            商品删除 = 29,
            商品查看 = 30,
            订单查看 = 31,
            订单发货 = 32,
            订单删除 = 33,
            财务查看 = 34,
            财务编辑 = 36,
            财务提现 = 37,
            红包新增 = 38,
            红包编辑 = 39,
            红包查看 = 41,
            红包删除 = 42,
            福音新增 = 43,
            福音编辑 = 44,
            福音查看 = 45,
            福音删除 = 46,
            系统新增 = 47,
            系统编辑 = 50,
            系统查看 = 52,
            管理员编辑 = 74,
            问题回复 = 55,
            问题删除 = 56,
            问题查看 = 58,
            通知查看 = 60,
            联系查看 = 61,
            帮助新增 = 62,
            帮助编辑 = 63,
            帮助查看 = 64,
            帮助删除 = 66,
            权限新增 = 67,
            权限编辑 = 68,
            权限查看 = 69,
            权限删除 = 70,


        }


    }
}
