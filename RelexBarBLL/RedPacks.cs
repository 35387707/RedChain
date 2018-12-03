using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;
using System.Data.SqlClient;
using System.Web;

namespace RelexBarBLL
{
    /// <summary>
    /// 红包基类
    /// </summary>
    public partial class RedPacksBLL : BaseBll
    {
        public static decimal UserRedBalance = 1054;//个人红包池
        object lockRedBalance = new object();//锁定个人红包吃

        #region 发红包

        /// <summary>
        /// 发红包
        /// </summary>
        /// <param name="UID">用户ID</param>
        /// <param name="title"></param>
        /// <param name="imglist"></param>
        /// <param name="LinkTo"></param>
        /// <param name="DelayTime">间隔时间</param>
        /// <param name="OncePacketCount">单次红包个数</param>
        /// <param name="SinglePrice">单个红包价格</param>
        /// <param name="TotalPrice">总价</param>
        /// <returns></returns>
        public bool SendRedPacket(Guid UID, enRedType RedType, string title, string imglist, string LinkTo, int? DelayTime, int? OncePacketCount, decimal SinglePrice, decimal TotalPrice, string areacode, string arealimit, int? sex, int ctype = 1)
        {
            using (DBContext)
            {
                var RID = Guid.NewGuid();
                switch (RedType)
                {
                    case enRedType.Single:
                    case enRedType.Single_OtherPay:
                        #region Single
                        var user = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                        if (user == null)
                            return false;
                        if (RedType == enRedType.Single)//如果是用余额发的红包
                        {
                            if (user.Balance < TotalPrice)
                                return false;
                            user.Balance -= TotalPrice;
                        }
                        if (ctype == 1)
                        {
                            user.FootQuan += TotalPrice;//发出去多少钱，他可以得到多少福券
                        }
                        else
                        {
                            user.Score += TotalPrice;//发出去多少钱，他可以得到多少积分
                            user.TotalScore += TotalPrice;//发出去多少钱，他可以得到多少积分
                        }

                        //用户主动发红包，则立即发送
                        RedPacket redpack = new RedPacket();
                        redpack.RID = RID;
                        redpack.UID = UID;
                        redpack.RedType = (int)enRedType.Single;
                        redpack.BelongID = null;
                        redpack.BeginTime = DateTime.Now;
                        redpack.EndTime = DateTime.Now.AddYears(1);//一年后无人领则过期
                        redpack.title = title;
                        redpack.imglist = imglist;
                        redpack.LinkTo = LinkTo;
                        redpack.DelayTime = DelayTime;
                        redpack.SinglePrice = SinglePrice;
                        redpack.TotalPrice = TotalPrice / 2;//扣除50%
                        redpack.OncePacketCount = (int)(redpack.TotalPrice / redpack.SinglePrice);
                        redpack.Status = (int)enPacketStatus.Actived;
                        redpack.CreateTime = redpack.UpdateTime = DateTime.Now;
                        if (!string.IsNullOrEmpty(areacode))
                        {
                            var q = areacode.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (q.Length > 1)
                            {
                                redpack.AreaX = decimal.Parse(q[0]);
                                redpack.AreaY = decimal.Parse(q[1]);
                            }
                        }
                        if (!string.IsNullOrEmpty(arealimit))
                        {
                            redpack.AreaLimit = int.Parse(arealimit);
                        }
                        redpack.SexLimit = sex;
                        DBContext.RedPacket.Add(redpack);

                        //插入到list表
                        string sql = @"insert into RedPacketList select top " + redpack.OncePacketCount + " newid(),'" + RID + "',NULL," + SinglePrice + ",1,'"
                            + redpack.BeginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + redpack.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',getdate(),getdate(),'" + GetRedListNumer() + "' from TempIndex";
                        DBContext.Database.ExecuteSqlCommand(sql);

                        PayListBLL.Insert(DBContext, RID, UID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, TotalPrice, "发出福包");
                        if (ctype == 1)
                        {
                            PayListBLL.Insert(DBContext, RID, UID, enPayInOutType.In, enPayType.FuQuan, enPayFrom.RedPaged, TotalPrice, "获得福利积分");
                        }
                        else
                        {
                            PayListBLL.Insert(DBContext, RID, UID, enPayInOutType.In, enPayType.Point, enPayFrom.RedPaged, TotalPrice, "获得福音积分");
                        }

                        SysConfigBLL.ReflashSystemMoney(redpack.TotalPrice);

                        #endregion
                        break;
                    case enRedType.System:
                        //系统发送，系统达到特定条件后再发送
                        #region System
                        //用户主动发红包，则立即发送
                        RedPacket redpack1 = new RedPacket();
                        redpack1.RID = RID;
                        redpack1.UID = UID;
                        redpack1.RedType = (int)RedType;
                        redpack1.BelongID = null;
                        redpack1.BeginTime = DateTime.Now;
                        redpack1.EndTime = DateTime.Now.AddYears(1);//一年后无人领则过期
                        redpack1.title = title;
                        redpack1.imglist = imglist;
                        redpack1.LinkTo = LinkTo;
                        redpack1.DelayTime = DelayTime;
                        redpack1.SinglePrice = SinglePrice;
                        redpack1.TotalPrice = TotalPrice;//系统红包不扣钱
                        redpack1.OncePacketCount = (int)(redpack1.TotalPrice / redpack1.SinglePrice);
                        redpack1.Status = (int)enPacketStatus.Actived;
                        redpack1.CreateTime = redpack1.UpdateTime = DateTime.Now;
                        DBContext.RedPacket.Add(redpack1);

                        logBll.InsertLog(Guid.Empty, string.Format("管理员：{0},操作系统发福包成功,标题:{1},总价：{2}", (HttpContext.Current.Session["admin"] as AdminUser).Name, title, TotalPrice), enLogType.Admin);


                        //插入到list表
                        string sql1 = @"insert into RedPacketList select top " + redpack1.OncePacketCount + " newid(),'" + RID + "',NULL," + SinglePrice + ",1,'"
                            + redpack1.BeginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + redpack1.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',getdate(),getdate(),'" + GetRedListNumer() + "' from TempIndex";
                        DBContext.Database.ExecuteSqlCommand(sql1);

                        #endregion
                        break;
                    case enRedType.Timeout:
                        //定时自动发红包（如商家交钱、代理商交钱）
                        #region Timeout

                        RedPacket redpack12 = new RedPacket();
                        redpack12.RID = RID;
                        redpack12.UID = UID;
                        redpack12.RedType = (int)RedType;
                        redpack12.BelongID = null;
                        redpack12.BeginTime = DateTime.Now;
                        redpack12.EndTime = DateTime.Now.AddYears(2);//两年后无人领则过期
                        redpack12.title = title;
                        redpack12.imglist = imglist;
                        redpack12.LinkTo = LinkTo;
                        redpack12.DelayTime = DelayTime;
                        redpack12.SinglePrice = SinglePrice;
                        redpack12.TotalPrice = TotalPrice;
                        redpack12.OncePacketCount = (int)(redpack12.TotalPrice / redpack12.SinglePrice);
                        redpack12.Status = (int)enPacketStatus.Actived;
                        redpack12.CreateTime = redpack12.UpdateTime = DateTime.Now;
                        DBContext.RedPacket.Add(redpack12);

                        #endregion
                        break;
                    case enRedType.Auto_UserRecRed:
                        #region Auto_UserRecRed

                        //红包池达到条件，自动发送新的红包
                        RedPacket redpack_2 = new RedPacket();
                        redpack_2.RID = RID;
                        redpack_2.UID = UID;
                        redpack_2.RedType = (int)RedType;
                        redpack_2.BelongID = null;
                        redpack_2.BeginTime = DateTime.Now.AddMinutes(30);//30分钟后可领取
                        redpack_2.EndTime = DateTime.Now.AddYears(1);//一年后无人领则过期
                        redpack_2.title = title;
                        redpack_2.imglist = imglist;
                        redpack_2.LinkTo = LinkTo;
                        redpack_2.DelayTime = DelayTime;
                        redpack_2.OncePacketCount = 3010;
                        redpack_2.SinglePrice = 0;//不一定
                        redpack_2.TotalPrice = TotalPrice;
                        redpack_2.Status = (int)enPacketStatus.Actived;
                        redpack_2.CreateTime = redpack_2.UpdateTime = DateTime.Now;
                        DBContext.RedPacket.Add(redpack_2);

                        var num = GetRedListNumer();
                        for (int i = 10; i < 101; i += 10)//10个大红包，{10、20、30、40、50、60、70、80、90、100}
                        {
                            RedPacketList rpl = new RedPacketList();
                            rpl.BeginTime = redpack_2.BeginTime;
                            rpl.EndTime = redpack_2.EndTime;
                            rpl.Money = i;
                            rpl.Number = num;
                            rpl.RID = RID;
                            rpl.RLID = Guid.NewGuid();
                            rpl.CreateTime = rpl.UpdateTime = DateTime.Now;
                            rpl.Status = (int)enPacketStatus.Actived;
                            DBContext.RedPacketList.Add(rpl);
                        }
                        //插入到list表，每个0.1元，总共3000个红包
                        string sql_2 = @"insert into RedPacketList select top 3000 newid(),'" + RID + "',NULL,0.1,1,'"
                            + redpack_2.BeginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + redpack_2.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',getdate(),getdate(),'" + num + "' from TempIndex";
                        DBContext.Database.ExecuteSqlCommand(sql_2);

                        #endregion
                        break;
                    case enRedType.Auto_NewAgentSend:
                        //新增代理奖励全员福包
                        #region Auto_NewAgentSend

                        //每增加一个代理9900，公司拿出216元发红包，30分钟自动发。
                        RedPacket redpack_3 = new RedPacket();
                        redpack_3.RID = RID;
                        redpack_3.UID = UID;
                        redpack_3.RedType = (int)RedType;
                        redpack_3.BelongID = null;
                        redpack_3.BeginTime = DateTime.Now.AddMinutes(30);//30分钟后可领取
                        redpack_3.EndTime = DateTime.Now.AddYears(1);//一年后无人领则过期
                        redpack_3.title = title;
                        redpack_3.imglist = imglist;
                        redpack_3.LinkTo = LinkTo;
                        redpack_3.DelayTime = DelayTime;
                        redpack_3.OncePacketCount = 810;
                        redpack_3.SinglePrice = 0;//不一定
                        redpack_3.TotalPrice = 216;
                        redpack_3.Status = (int)enPacketStatus.Actived;
                        redpack_3.CreateTime = redpack_3.UpdateTime = DateTime.Now;
                        DBContext.RedPacket.Add(redpack_3);

                        var num3 = GetRedListNumer();
                        //10个大红包，每个10元
                        string sql_3 = @"insert into RedPacketList select top 10 newid(),'" + RID + "',NULL,10,1,'"
                            + redpack_3.BeginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + redpack_3.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',getdate(),getdate(),'" + num3 + "' from TempIndex";
                        DBContext.Database.ExecuteSqlCommand(sql_3);
                        //80元发出800个红包，每个红包0.1元
                        sql_3 = @"insert into RedPacketList select top 800 newid(),'" + RID + "',NULL,0.1,1,'"
                            + redpack_3.BeginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + redpack_3.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',getdate(),getdate(),'" + num3 + "' from TempIndex";
                        DBContext.Database.ExecuteSqlCommand(sql_3);

                        #endregion
                        break;
                    case enRedType.Auto_SystemAchieveSend:
                        //系统是否达到业绩
                        #region Auto_SystemAchieveSend

                        //系统是否够达到业绩
                        SysConfigBLL.ReflashSystemMoney(null);

                        int totalSend = (int)(SysConfigBLL.SystemMoney / SysConfigBLL.SystemAchieveSend);
                        var hadSend = DBContext.RedPacket.Count(m => m.RedType == (int)enRedType.Auto_SystemAchieveSend);//发了几次？
                        if (totalSend > hadSend)//总发数量超过已发数量，则发一波
                        {
                            //公司总业绩每增加300万业绩，拿出2%累计60000元
                            RedPacket redpack_42 = new RedPacket();
                            redpack_42.RID = RID;
                            redpack_42.UID = UID;
                            redpack_42.RedType = (int)RedType;
                            redpack_42.BelongID = null;
                            redpack_42.BeginTime = DateTime.Now.AddMinutes(30);//30分钟后可领取
                            redpack_42.EndTime = DateTime.Now.AddYears(1);//一年后无人领则过期
                            redpack_42.title = "系统福包:大家发财";
                            redpack_42.imglist = "";
                            redpack_42.LinkTo = LinkTo;
                            redpack_42.DelayTime = DelayTime;
                            redpack_42.OncePacketCount = 6017;
                            redpack_42.SinglePrice = 0;//不一定
                            redpack_42.TotalPrice = 62000;  //TotalPrice;
                            redpack_42.Status = (int)enPacketStatus.Actived;
                            redpack_42.CreateTime = redpack_42.UpdateTime = DateTime.Now;
                            DBContext.RedPacket.Add(redpack_42);

                            var num4 = GetRedListNumer();
                            //系统自动发出17个红包，17个人抢,30分钟自动发。
                            //金额分别{ 200、300、400、500、600、700、800、900、1000}【2000、3000、4000、5000广告商抢】【6000、7000、8000、9000代理商抢】
                            for (int i = 200; i < 1001; i += 100)
                            {
                                RedPacketList rpl = new RedPacketList();
                                rpl.BeginTime = redpack_42.BeginTime;
                                rpl.EndTime = redpack_42.EndTime;
                                rpl.Money = i;
                                rpl.Number = num4;
                                rpl.RID = RID;
                                rpl.RLID = Guid.NewGuid();
                                rpl.CreateTime = rpl.UpdateTime = DateTime.Now;
                                rpl.Status = (int)enPacketStatus.Actived;
                                DBContext.RedPacketList.Add(rpl);
                            }
                            for (int i = 2000; i < 9001; i += 1000)
                            {
                                RedPacketList rpl = new RedPacketList();
                                rpl.BeginTime = redpack_42.BeginTime;
                                rpl.EndTime = redpack_42.EndTime;
                                rpl.Money = i;
                                rpl.Number = num4;
                                rpl.RID = RID;
                                rpl.RLID = Guid.NewGuid();
                                rpl.CreateTime = rpl.UpdateTime = DateTime.Now;
                                rpl.Status = (int)enPacketStatus.Actived;
                                DBContext.RedPacketList.Add(rpl);
                            }

                            //600元发出6000个红包，每个红包0.1元
                            string sql_4 = @"insert into RedPacketList select top 6000 newid(),'" + RID + "',NULL,0.1,1,'"
                                + redpack_42.BeginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + redpack_42.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',getdate(),getdate(),'" + num4 + "' from TempIndex";
                            DBContext.Database.ExecuteSqlCommand(sql_4);
                        }
                        #endregion
                        break;
                    case enRedType.Auto_RewardSend:
                        //奖金的10%发广告
                        #region Auto_RewardSend

                        var num5 = GetRedListNumer();

                        RedPacket redpack_4 = new RedPacket();
                        redpack_4.RID = RID;
                        redpack_4.UID = UID;
                        redpack_4.RedType = (int)RedType;
                        redpack_4.BelongID = null;
                        redpack_4.BeginTime = DateTime.Now.AddMinutes(30);//30分钟后可领取
                        redpack_4.EndTime = DateTime.Now.AddYears(1);//一年后无人领则过期
                        redpack_4.title = title;
                        redpack_4.imglist = imglist;
                        redpack_4.LinkTo = LinkTo;
                        redpack_4.DelayTime = DelayTime;
                        redpack_4.Status = (int)enPacketStatus.Actived;
                        redpack_4.CreateTime = redpack_4.UpdateTime = DateTime.Now;
                        DBContext.RedPacket.Add(redpack_4);

                        if (TotalPrice == 30)//30元的红包
                        {
                            redpack_4.OncePacketCount = 10;
                            redpack_4.SinglePrice = 0;//不一定
                            redpack_4.TotalPrice = 25;

                            for (int i = 0; i < 5; i++)
                            {
                                RedPacketList rpl = new RedPacketList();
                                rpl.BeginTime = redpack_4.BeginTime;
                                rpl.EndTime = redpack_4.EndTime;
                                rpl.Money = 2;//每个2元
                                rpl.Number = num5;
                                rpl.RID = RID;
                                rpl.RLID = Guid.NewGuid();
                                rpl.CreateTime = rpl.UpdateTime = DateTime.Now;
                                rpl.Status = (int)enPacketStatus.Actived;
                                DBContext.RedPacketList.Add(rpl);
                                RedPacketList rpl2 = new RedPacketList();
                                rpl2.BeginTime = redpack_4.BeginTime;
                                rpl2.EndTime = redpack_4.EndTime;
                                rpl2.Money = 3;//每个3元
                                rpl2.Number = num5;
                                rpl2.RID = RID;
                                rpl2.RLID = Guid.NewGuid();
                                rpl2.CreateTime = rpl2.UpdateTime = DateTime.Now;
                                rpl2.Status = (int)enPacketStatus.Actived;
                                DBContext.RedPacketList.Add(rpl2);
                            }
                        }
                        else if (TotalPrice == 60)//60元级别
                        {
                            redpack_4.OncePacketCount = 10;
                            redpack_4.SinglePrice = 0;//不一定
                            redpack_4.TotalPrice = 45;

                            for (int i = 0; i < 5; i++)
                            {
                                RedPacketList rpl = new RedPacketList();
                                rpl.BeginTime = redpack_4.BeginTime;
                                rpl.EndTime = redpack_4.EndTime;
                                rpl.Money = 4;//每个2元
                                rpl.Number = num5;
                                rpl.RID = RID;
                                rpl.RLID = Guid.NewGuid();
                                rpl.CreateTime = rpl.UpdateTime = DateTime.Now;
                                rpl.Status = (int)enPacketStatus.Actived;
                                DBContext.RedPacketList.Add(rpl);
                                RedPacketList rpl2 = new RedPacketList();
                                rpl2.BeginTime = redpack_4.BeginTime;
                                rpl2.EndTime = redpack_4.EndTime;
                                rpl2.Money = 5;//每个3元
                                rpl2.Number = num5;
                                rpl2.RID = RID;
                                rpl2.RLID = Guid.NewGuid();
                                rpl2.CreateTime = rpl2.UpdateTime = DateTime.Now;
                                rpl2.Status = (int)enPacketStatus.Actived;
                                DBContext.RedPacketList.Add(rpl2);
                            }
                        }
                        else if (TotalPrice == 200)//200元级别
                        {
                            redpack_4.OncePacketCount = 20;
                            redpack_4.SinglePrice = 0;//不一定
                            redpack_4.TotalPrice = 160;

                            for (int i = 0; i < 4; i++)
                            {
                                RedPacketList rpl = new RedPacketList();
                                rpl.BeginTime = redpack_4.BeginTime;
                                rpl.EndTime = redpack_4.EndTime;
                                rpl.Money = 6;//每个2元
                                rpl.Number = num5;
                                rpl.RID = RID;
                                rpl.RLID = Guid.NewGuid();
                                rpl.CreateTime = rpl.UpdateTime = DateTime.Now;
                                rpl.Status = (int)enPacketStatus.Actived;
                                DBContext.RedPacketList.Add(rpl);
                                RedPacketList rpl2 = new RedPacketList();
                                rpl2.BeginTime = redpack_4.BeginTime;
                                rpl2.EndTime = redpack_4.EndTime;
                                rpl2.Money = 7;//每个3元
                                rpl2.Number = num5;
                                rpl2.RID = RID;
                                rpl2.RLID = Guid.NewGuid();
                                rpl2.CreateTime = rpl2.UpdateTime = DateTime.Now;
                                rpl2.Status = (int)enPacketStatus.Actived;
                                DBContext.RedPacketList.Add(rpl2);
                                RedPacketList rp3 = new RedPacketList();
                                rp3.BeginTime = redpack_4.BeginTime;
                                rp3.EndTime = redpack_4.EndTime;
                                rp3.Money = 8;//每个2元
                                rp3.Number = num5;
                                rp3.RID = RID;
                                rp3.RLID = Guid.NewGuid();
                                rp3.CreateTime = rp3.UpdateTime = DateTime.Now;
                                rp3.Status = (int)enPacketStatus.Actived;
                                DBContext.RedPacketList.Add(rp3);
                                RedPacketList rpl4 = new RedPacketList();
                                rpl4.BeginTime = redpack_4.BeginTime;
                                rpl4.EndTime = redpack_4.EndTime;
                                rpl4.Money = 9;//每个3元
                                rpl4.Number = num5;
                                rpl4.RID = RID;
                                rpl4.RLID = Guid.NewGuid();
                                rpl4.CreateTime = rpl4.UpdateTime = DateTime.Now;
                                rpl4.Status = (int)enPacketStatus.Actived;
                                DBContext.RedPacketList.Add(rpl4);
                                RedPacketList rp5 = new RedPacketList();
                                rp5.BeginTime = redpack_4.BeginTime;
                                rp5.EndTime = redpack_4.EndTime;
                                rp5.Money = 10;//每个2元
                                rp5.Number = num5;
                                rp5.RID = RID;
                                rp5.RLID = Guid.NewGuid();
                                rp5.CreateTime = rp5.UpdateTime = DateTime.Now;
                                rp5.Status = (int)enPacketStatus.Actived;
                                DBContext.RedPacketList.Add(rp5);
                            }
                        }
                        else if (TotalPrice == 20)
                        {
                            redpack_4.OncePacketCount = 160;
                            redpack_4.SinglePrice = 0;//不一定
                            redpack_4.TotalPrice = 16;

                            //160个红包，每个红包0.1元
                            string sql_4 = @"insert into RedPacketList select top 160 newid(),'" + RID + "',NULL,0.1,1,'"
                                + redpack_4.BeginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + redpack_4.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',getdate(),getdate(),'" + num5 + "' from TempIndex";
                            DBContext.Database.ExecuteSqlCommand(sql_4);
                        }
                        else if (TotalPrice == 100)
                        {
                            redpack_4.OncePacketCount = 800;
                            redpack_4.SinglePrice = 0;//不一定
                            redpack_4.TotalPrice = 80;

                            //800个红包，每个红包0.1元
                            string sql_4 = @"insert into RedPacketList select top 800 newid(),'" + RID + "',NULL,0.1,1,'"
                                + redpack_4.BeginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "','" + redpack_4.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "',getdate(),getdate(),'" + num5 + "' from TempIndex";
                            DBContext.Database.ExecuteSqlCommand(sql_4);
                        }

                        #endregion
                        break;
                }
                var result = DBContext.SaveChanges();

                if (result > 0)
                {
                    if (RedType == enRedType.Single)//发当个红包的时候，需要给上级奖励
                    {
                        //计算奖励
                        new Rewards().SendRedRewards(RID);
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 检测并发送达到条件后的自动发送红包到详细红包组中
        /// </summary>
        /// <param name="RID"></param>
        public void TimeoutSendRedPacketDetail()
        {
            using (DBContext)
            {
                var redList = DBContext.RedPacket.Where(m => m.Status == (int)enPacketStatus.Actived && m.RedType == (int)enRedType.Timeout).ToList();//处理到时自动发红包的红包
                if (redList.Count > 0)
                {
                    //SendRedPacket();//发红包
                }
            }
        }

        /// <summary>
        /// 单对单发送普通红包，扣去金额
        /// </summary>
        /// <returns></returns>
        public int SendRed(Guid UID, Guid RUID, decimal Price)
        {
            using (DBContext)
            {
                var q = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (q == null || q.Status == (int)enStatus.Unabled)
                {
                    return (int)ErrorCode.账号不可用;
                }
                if (q.Balance < Price)
                {
                    return (int)ErrorCode.账户余额不足;
                }
                q.Balance -= Price;

                PayListBLL.Insert(DBContext, RUID, UID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, Price, "发出福包");

                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 领取聊天红包
        /// </summary>
        /// <returns></returns>
        public int RecChatRed(Guid UID, Guid MID)
        {
            using (DBContext)
            {
                var q = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                if (q == null || q.Status == (int)enStatus.Unabled)
                {
                    return (int)ErrorCode.账号不可用;
                }
                var msg = DBContext.ChatMessage.FirstOrDefault(m => m.MID == MID && m.TUID == UID);
                if (msg == null)
                {
                    return (int)ErrorCode.福包不存在;
                }
                decimal Price;
                var c = CommonClass.ChangeData.ExchangeDataType(msg.Content);
                if (!decimal.TryParse(c["price"].ToString(), out Price))
                {
                    return (int)ErrorCode.福包不存在;
                }
                q.Balance += Price;

                PayListBLL.Insert(DBContext, MID, UID, enPayInOutType.In, enPayType.Coin, enPayFrom.RedPaged, Price, "领取福包");

                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 发送商家升级后的定时红包
        /// </summary>
        public void SendTimeOutRedpack()
        {
            using (DBContext)
            {
                DateTime dt = DateTime.Now;
                DateTime dtEnd = dt.AddYears(1);
                var dtyear = dt.AddYears(-1);//一年前的单子都可以发出
                var list = DBContext.RedPacket.Where(m => m.RedType == (int)enRedType.Timeout && m.BeginTime >= dtyear).ToList();
                foreach (var temp in list)
                {
                    //符合条件则发送新的包
                    //插入到list表
                    string sql = @"insert into RedPacketList select top " + 3 + " newid(),'" + temp.RID + "',NULL," + temp.SinglePrice + ",1,'"
                        + dt.ToString("yyyy-MM-dd HH:mm:ss") + "','" + dtEnd.ToString("yyyy-MM-dd HH:mm:ss") + "',getdate(),getdate(),'" + GetRedListNumer() + "' from TempIndex";
                    DBContext.Database.ExecuteSqlCommand(sql);
                }
            }
        }

        public List<RedPacket> GetRedPacketByNumber(Guid uid, string number, enPacketStatus? status)
        {
            using (DBContext)
            {
                var q = DBContext.RedPacket.Where(m => m.UID == uid && m.RID == Guid.Parse(number));
                if (status != null)
                {
                    q = q.Where(m => m.Status == (int)status.Value);
                }
                return q.ToList();
            }
        }

        public int UpdateRedPacketStatus(Guid? RID)
        {
            using (DBContext)
            {
                DBContext.RedPacket.FirstOrDefault(m => m.RID == RID).Status = (int)enPacketStatus.Actived;
                return DBContext.SaveChanges();

            }
        }

        public int UpdateRedPacketListStatus(Guid? RID)
        {
            using (DBContext)
            {
                DBContext.RedPacketList.FirstOrDefault(m => m.RID == RID).Status = (int)enPacketStatus.Actived;
                return DBContext.SaveChanges();

            }
        }


        public decimal GetRedPacketTotalPrice(Guid RID)
        {
            using (DBContext)
            {
                List<RedPacket> list = DBContext.RedPacket.Where(m => m.RID == RID).ToList();
                decimal sum = list.Sum(m => m.TotalPrice) * 2;
                return sum;
            }
        }

        /// <summary>
        /// 获取最新一条升级自动发红包的记录
        /// </summary>
        public RedPacket GetRedPacketNewByUID(Guid UID)
        {
            using (DBContext)
            {
                //  return DBContext.RedPacket.FirstOrDefault(m => m.UID == UID && m.RedType == (int)enRedType.Timeout);
                return DBContext.RedPacket.Where(m => m.UID == UID && m.RedType == (int)enRedType.Timeout).Take(1).FirstOrDefault();
            }
        }

        public int EditRedPacketNewByID(Guid? RID, string title, string imglist)
        {
            using (DBContext)
            {
                RedPacket v = DBContext.RedPacket.FirstOrDefault(m => m.RID == RID);
                if (v == null)
                    return (int)ErrorCode.数据不存在;
                v.title = title;
                v.imglist = imglist;
                v.BeginTime = DateTime.Now;
                v.EndTime = DateTime.Now.AddYears(7);//两年后无人领则过期
                v.UpdateTime = DateTime.Now;
                return DBContext.SaveChanges();
            }
        }


        public BankList GetNewBankList(Guid uid)
        {
            using (DBContext)
            {
                return DBContext.BankList.Where(m => m.UID == uid && m.Status == (int)enStatus.Enabled).Take(1).FirstOrDefault();
            }
        }

        #endregion

        #region 获取/领取红包

        public decimal GetRecMaxRedPrice(Guid UID, int utype)
        {
            decimal maxredprice = 0.9M;
            switch ((enUserType)utype)
            {
                case enUserType.Agent:
                    maxredprice = 9900;
                    break;
                case enUserType.Shop:
                    maxredprice = 5000;
                    break;
                case enUserType.User:
                    var maxfriend = new FriendBLL().GetMyFriendCount(UID);
                    if (maxfriend > 100)
                    {
                        maxredprice = 1000;
                    }
                    else if (maxfriend > 50 && maxfriend <= 100)
                    {
                        maxredprice = 100;
                    }
                    else if (maxfriend > 20 && maxfriend <= 50)
                    {
                        maxredprice = 10;
                    }
                    else if (maxfriend > 10 && maxfriend <= 20)
                    {
                        maxredprice = 1;
                    }
                    else
                    {
                        maxredprice = 0.9M;
                    }
                    break;
            }
            return maxredprice;
        }

        public class PacketList
        {
            public string Number { get; set; }
            public Guid RID { get; set; }
        }

        /// <summary>
        /// 获取用户可领取的附近所有红包
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="top">默认显示50个红包</param>
        /// <returns></returns>
        public dynamic GetPacketList(Guid UID, int top = 50)
        {
            using (DBContext)
            {
                var dt = DateTime.Now;
                var uinfo = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                var sex = uinfo.Sex;

                var maxredprice = GetRecMaxRedPrice(UID, uinfo.UserType);
                //var q = from a in DBContext.RedPacketList
                //        join c in DBContext.RedPacket on a.RID equals c.RID into temp2
                //        from tt2 in temp2.DefaultIfEmpty()
                //        join b in DBContext.RedPacketList.Where(m => m.UID == UID).Select(m => m.Number) on a.Number equals b into temp
                //        from tt in temp.DefaultIfEmpty()
                //        where a.BeginTime <= dt && a.EndTime >= dt && a.Status == (int)enPacketStatus.Actived && a.UID == null && (!tt2.SexLimit.HasValue || tt2.SexLimit == sex)
                //              && tt == null  //没有相同的number，说明该批次没有被领取过
                //        select a;
                //return q.GroupBy(m => new { m.Number, m.RID }).OrderBy(m => m.Key.Number).Select(m => m.Key).Take(top).ToList();

                //string sql = @"select top 50 b.Number,b.RID from RedPacket a
                //                inner join RedPacketList b on a.RID=b.RID
                //                where b.BeginTime<=GETDATE() and b.EndTime>=GETDATE() and b.Status=1 and b.UID is null and b.Money <=" + maxredprice + @" 
                //                and b.Number not in(select Number from RedPacketList where UID={0})";
                //if (sex.HasValue)
                //{
                //    sql += " and (a.SexLimit is null or a.SexLimit=" + sex.Value + ") ";
                //}
                //sql += " group by b.Number,b.RID ";

                string sql = @"select top 50 t.Number,t.RID from
                                (select b.Number,b.RID from RedPacket a
                                inner join RedPacketList b on a.RID=b.RID
                                where b.BeginTime<=GETDATE() and b.EndTime>=GETDATE() and b.Status=1 and b.UID is null and b.Money <=" + maxredprice + @" 
                                ";
                if (sex.HasValue)
                {
                    sql += " and (a.SexLimit is null or a.SexLimit=" + sex.Value + ") ";
                }
                sql += @" group by b.Number,b.RID)t
                            left join (select Number from RedPacketList where UID={0})t2 on t.Number=t2.Number
                            where t2.Number is null";

                return DBContext.Database.SqlQuery<PacketList>(sql, UID).ToList();
                //var dt = DateTime.Now;
                //var q = from a in DBContext.RedPacketList
                //        from c in DBContext.RedPacket
                //        from d in DBContext.Users
                //        join b in DBContext.RedPacketList.Where(m => m.UID == UID).Select(m => m.Number) on a.Number equals b into temp
                //        from tt in temp.DefaultIfEmpty()
                //        where a.RID == c.RID && c.UID == d.ID && a.BeginTime <= dt && a.EndTime >= dt && a.Status == (int)enPacketStatus.Actived && a.UID == null
                //            && tt == null  //没有相同的number，说明该批次没有被领取过
                //        select new { a.Number, a.RID, c.UID, d.TrueName, d.Phone, d.HeadImg1 };
                //return q.GroupBy(m => new { m.Number, m.RID, m.UID, m.TrueName, m.Phone, m.HeadImg1 }).OrderBy(m => m.Key.Number).Select(m => m.Key).Take(top).ToList();
            }
        }

        /// <summary>
        /// 获取红包详细信息（发包人，广告等）
        /// </summary>
        /// <param name="RID">红包ID</param>
        /// <returns></returns>
        public dynamic GetPacketDetail(Guid RID)
        {
            using (DBContext)
            {
                return DBContext.RedPacket.FirstOrDefault(m => m.RID == RID);
            }
        }

        /// <summary>
        /// 获取红包详细信息（发包人名字头像，广告等）
        /// </summary>
        /// <param name="RID">红包ID</param>
        /// <returns></returns>
        public dynamic GetPacketDetail(Guid UID, Guid RID, string Number)
        {
            using (DBContext)
            {
                var model = DBContext.RedPacket.FirstOrDefault(m => m.RID == RID);
                if (model == null)
                {
                    return null;
                }
                model.ViewCount++;
                DBContext.SaveChanges();
                var q = from a in DBContext.RedPacket
                        join b in DBContext.Users on a.UID equals b.ID into ru
                        from rus in ru.DefaultIfEmpty()
                        join c in DBContext.RedPacketList.Where(m => m.UID == UID && m.RID == RID && m.Number == Number) on a.RID equals c.RID into t
                        from tt in t.DefaultIfEmpty()
                        where a.RID == RID
                        select new
                        {
                            a.RID,
                            a.UID,
                            a.title,
                            //title = rus == null ? "亿万福包,疯狂来袭" : a.title,
                            a.imglist,
                            //imglist = rus == null ? "/homecss/image/redpack.jpg" : a.imglist,
                            a.LinkTo,
                            a.TotalPrice,
                            a.CreateTime,
                            a.EndTime,
                            a.ViewCount,
                            a.GoodCount,
                            UserType = rus == null ? -1 : rus.UserType,
                            TrueName = rus == null ? "" : rus.TrueName,
                            HeadImg1 = rus == null ? "/homecss/image/logo.png" : rus.HeadImg1,
                            Phone = rus == null ? "福包多多【系统】" : rus.Phone,
                            GetMoney = tt == null ? 0 : tt.Money,
                            IsColletion = DBContext.MyCollection.Count(m => m.UID == UID && m.MID == a.RID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.RedPacket) == 0 ? "0" : "1", //收藏状态（0未收藏，1已收藏） 
                                                                                                                                                                                                                   // IsFollower = DBContext.MyCollection.Count(m => m.UID == UID && m.MID == a.RID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.RedPacket) == 0 ? "0" : "1", //关注状态（0未关注，1已关注）
                            IsUserFollower = DBContext.MyCollection.Count(m => m.UID == UID && m.MID == a.UID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.User) == 0 ? "0" : "1", //用户关注状态（0未关注，1已关注）
                            IsGoodFor = DBContext.RedPacketGood.Count(m => m.RLID == a.RID && m.UID == UID) == 0 ? "0" : "1", //点赞状态（0未点赞，1已收藏）
                            CollectionID = DBContext.MyCollection.FirstOrDefault(m => m.UID == UID && m.MID == a.RID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.RedPacket) == null ? "0" : DBContext.MyCollection.FirstOrDefault(m => m.UID == UID && m.MID == a.RID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.RedPacket).ID.ToString(),
                            UserFollowerID = DBContext.MyCollection.FirstOrDefault(m => m.UID == UID && m.MID == a.UID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.User) == null ? "0" : DBContext.MyCollection.FirstOrDefault(m => m.UID == UID && m.MID == a.UID && m.Status == (int)enStatus.Enabled && m.MType == (int)enMycollectionType.User).ID.ToString(),
                            GoodID = DBContext.RedPacketGood.FirstOrDefault(m => m.RLID == a.RID && m.UID == UID) == null ? "0" : DBContext.RedPacketGood.FirstOrDefault(m => m.RLID == a.RID && m.UID == UID).ID.ToString(),
                            IsFriend = DBContext.FriendShip.Count(m => m.UID == UID && m.FriendID == a.UID && m.IsDel == 0) == 0 ? "0" : "1", //是否已加好友
                            IsSayHello = DBContext.ChatMessage.Count(m => m.FUID == UID && m.TUID == a.UID && m.TType == (int)RelexBarBLL.EnumCommon.MessageTType.User && m.MType == (int)RelexBarBLL.EnumCommon.MessageType.SayHello) == 0 ? "0" : "1", //是否敲过门
                        };

                return q.FirstOrDefault();
            }
        }

        /// <summary>
        /// 点赞红包
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="RID"></param>
        /// <param name="Number"></param>
        /// <returns></returns>
        public int GoodForRedPacket(Guid UID, Guid RID)
        {
            using (DBContext)
            {
                var model = DBContext.RedPacket.FirstOrDefault(m => m.RID == RID);
                if (model == null)
                    return (int)ErrorCode.福包不存在;
                if (DBContext.RedPacketGood.Count(m => m.RLID == RID && m.UID == UID) > 0)
                {
                    return (int)ErrorCode.您已经点赞过;
                }

                RedPacketGood rpg = new RedPacketGood();
                rpg.UID = UID;
                rpg.RLID = RID;
                rpg.Status = (int)enStatus.Enabled;
                rpg.CreateTime = rpg.UpdateTime = DateTime.Now;
                model.GoodCount++;
                DBContext.RedPacketGood.Add(rpg);
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 点击领取红包
        /// </summary>
        /// <param name="UID">用户ID</param>
        /// <param name="RLID">具体的红包ID</param>
        /// <returns></returns>
        public int ClickRedPacket(Guid UID, Guid RID, string Number, int UserType)
        {
            using (DBContext)
            {
                int canresult = new RedPacksBLL().CanGetRedPacket(UID);
                if (canresult < 1)
                {
                    return canresult;
                }
                if (DBContext.RedPacketList.Count(m => m.UID == UID && m.RID == RID && m.Number == Number) > 0)
                {
                    return (int)ErrorCode.您已领过福包;
                }
                var maxredprice = GetRecMaxRedPrice(UID, UserType);

                var red = DBContext.RedPacketList.FirstOrDefault(m => m.RID == RID && m.Number == Number && m.Status == (int)enPacketStatus.Actived && m.Money <= maxredprice);
                if (red == null)
                {
                    return (int)ErrorCode.福包已被领取完;
                }
                red.Status = (int)enPacketStatus.Used;
                red.UID = UID;
                red.UpdateTime = DateTime.Now;
                var user = DBContext.Users.FirstOrDefault(m => m.ID == UID);
                user.Balance += red.Money * (1 - SysConfigBLL.Poundage);//余额
                user.RedBalance += red.Money * SysConfigBLL.Poundage;//红包池
                PayListBLL.Insert(DBContext, red.RLID, UID, enPayInOutType.In, enPayType.Coin, enPayFrom.RedPaged, red.Money, "收到福包");
                PayListBLL.Insert(DBContext, red.RLID, UID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, red.Money * SysConfigBLL.Poundage, "自动扣除到共享福包池");
                DBContext.SaveChanges();

                lock (lockRedBalance)
                {
                    var totalRedBalance = DBContext.Users.Where(m => m.Status == (int)enStatus.Enabled).Sum(m => m.RedBalance);
                    var hadSendRedBalanceTimes = DBContext.RedPacket.Count(m => m.RedType == (int)enRedType.Auto_UserRecRed);//已经发了几次？
                    if ((int)(totalRedBalance / UserRedBalance) > hadSendRedBalanceTimes)//红包池是否达到条件，则自动发红包
                    {
                        //new RedPacksBLL().SendRedPacket(user.ID, enRedType.Auto_UserRecRed, "大家好，我是" + user.TrueName + "(" + user.Phone + ")", user.HeadImg1, "", null, null, 0, UserRedBalance, "", "", null);
                        new RedPacksBLL().SendRedPacket(Guid.Empty, enRedType.Auto_UserRecRed, "亿万福包,疯狂来袭", "/Content/fbinfo/gxfb.jpg", "", null, null, 0, UserRedBalance, "", "", null);
                    }
                }
                //计算奖励
                new Rewards().ClickRedRewards(red.RLID);
                return 1;
            }
        }

        /// <summary>
        /// 判断当前用户领取福包是否已达上限
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public int CanGetRedPacket(Guid UID)
        {
            using (DBContext)
            {
                var dt = DateTime.Now.Date;//当天时间
                //默认每天最多领取30个红包
                var daycount = DBContext.RedPacketList.Count(m => m.UID == UID && m.UpdateTime > dt && m.Status == (int)enPacketStatus.Used);
                if (daycount >= CanGetMaxRedsCount(UID))//目前测试先放10个
                {
                    return (int)ErrorCode.您今日领取福包已达到上限;
                }
                //分享一个好友，多领10个
                return 1;
            }
        }

        /// <summary>
        /// 可获取最多的红包数是多少
        /// </summary>
        /// <returns></returns>
        private int CanGetMaxRedsCount(Guid UID)
        {
            var user = DBContext.Users.FirstOrDefault(m => m.ID == UID);
            if (user == null)
                return 0;
            var tjcount = DBContext.Users.Count(m => m.FID == UID);//推荐多少人
            var multcount = 0;

            switch ((enUserType)user.UserType)
            {
                case enUserType.Agent: multcount += 20; break;
                case enUserType.Shop: multcount += 20; break;
                default:
                    if (tjcount >= 10 && tjcount < 20)
                    {
                        multcount += 5;
                    }
                    else if (tjcount >= 20)
                    {
                        multcount += 10;
                    }
                    multcount += 10;
                    break;
            }
            return multcount;
        }

        /// <summary>
        /// 获取用户今日领取数量
        /// </summary>
        /// <returns></returns>
        public int GetUserReciveCount(Guid UID, DateTime time, out int maxcount)
        {
            using (DBContext)
            {
                var dt = time.Date;//当天时间
                maxcount = CanGetMaxRedsCount(UID);
                return DBContext.RedPacketList.Count(m => m.UID == UID && m.UpdateTime > dt && m.Status == (int)enPacketStatus.Used);
            }
        }

        /// <summary>
        /// 获取已领取红包详细信息
        /// </summary>
        /// <param name="RID">红包ID</param>
        /// <returns></returns>
        public List<RedPacketList> GetRecPacketList(Guid UID, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.RedPacketList.Where(m => m.UID == UID).OrderByDescending(m => m.UpdateTime);
                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }

        /// <summary>
        /// 获取已领取红包详细信息
        /// </summary>
        /// <param name="RID">红包ID</param>
        /// <returns></returns>
        public List<RedPacket> GetSendPacketList(Guid UID, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.RedPacket.Where(m => m.UID == UID).OrderByDescending(m => m.UpdateTime);
                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }

        /// <summary>
        /// 获取所有已领取该红包的人员等信息
        /// </summary>
        /// <param name="RID">红包ID</param>
        /// <returns></returns>
        public dynamic GetRecPacketList(Guid RID, string Number, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from m in DBContext.RedPacketList
                        from a in DBContext.Users
                        where m.RID == RID && (string.IsNullOrEmpty(Number) || m.Number == Number) && m.Status == (int)enPacketStatus.Used && m.UID == a.ID
                        orderby m.UpdateTime descending
                        select new
                        {
                            a.HeadImg1,
                            a.TrueName,
                            a.ID,
                            m.Money,
                            a.Phone,
                            m.UpdateTime
                        };

                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }

        /// <summary>
        /// 获取所有领取该红包的信息
        /// </summary>
        /// <param name="RID"></param>
        /// <returns></returns>
        public List<RedPacketList> GetRedPacketList(Guid RID)
        {
            using (DBContext)
            {
                var q = from m in DBContext.RedPacketList
                        from a in DBContext.Users
                        where m.RID == RID && m.Status == (int)enPacketStatus.Used && m.UID == a.ID
                        orderby m.UpdateTime descending
                        select new
                        {
                            a.HeadImg1,
                            a.TrueName,
                            a.Name,
                            a.ID,
                            m.Money,
                            m.UpdateTime,
                            m.BeginTime,
                            m.EndTime,
                            m.Number
                        };
                //  count = q.Count();

                // return q.Take(20).ToList();

                return DBContext.RedPacketList.Where(m => m.RID == RID).ToList();

                //  return GetDataTable(q);

            }
        }

        /// <summary>
        /// 获取红包排行榜
        /// </summary>
        /// <returns></returns>
        public dynamic GetRankList(int type, int sv, int top = 50)
        {
            using (DBContext)
            {
                DateTime? dtBegin = null;
                switch (type)
                {
                    case 1: dtBegin = DateTime.Now.Date; break;
                    case 2: dtBegin = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).Date; break;
                }
                if (sv == 1)//发红包排行
                {
                    var q = from a in DBContext.RedPacket
                            from b in DBContext.Users
                            where a.UID == b.ID && a.Status == (int)enStatus.Enabled && a.RedType == (int)enRedType.Single && (!dtBegin.HasValue || a.CreateTime >= dtBegin)
                            group a by new { b.ID, b.HeadImg1, b.TrueName, b.Phone } into g
                            orderby g.Sum(m => m.TotalPrice) descending
                            select new
                            {
                                HeadImg = g.Key.HeadImg1,
                                UID = g.Key.ID,
                                TrueName = g.Key.TrueName,
                                Phone = g.Key.Phone,
                                Price = g.Sum(m => m.TotalPrice),
                            };
                    return q.OrderByDescending(m => m.Price).ThenBy(m => m.Phone).Take(top).ToList();
                }
                else//抢红包排行
                {
                    var q = from a in DBContext.RedPacketList
                            from b in DBContext.Users
                            where a.UID == b.ID && a.Status == (int)enPacketStatus.Used && (!dtBegin.HasValue || a.UpdateTime >= dtBegin)
                            group a by new { b.ID, b.HeadImg1, b.TrueName, b.Phone } into g
                            orderby g.Sum(m => m.Money) descending
                            select new
                            {
                                HeadImg = g.Key.HeadImg1,
                                UID = g.Key.ID,
                                TrueName = g.Key.TrueName,
                                Phone = g.Key.Phone,
                                Price = g.Sum(m => m.Money),
                            };
                    return q.OrderByDescending(m => m.Price).ThenBy(m => m.Phone).Take(top).ToList();

                }
            }
        }

        /// <summary>
        /// 获取最新抢到红包的人
        /// </summary>
        /// <returns></returns>
        public dynamic GetNewsRecRedPack()
        {
            using (DBContext)
            {
                var q = from m in DBContext.RedPacketList
                        from a in DBContext.Users
                        where m.Status == (int)enPacketStatus.Used && m.UID == a.ID
                        orderby m.UpdateTime descending
                        select new
                        {
                            a.HeadImg1,
                            a.TrueName,
                            a.ID,
                            m.Money,
                            m.UpdateTime
                        };
                return q.Take(5).ToList();
            }
        }

        /// <summary>
        /// 剩余福包释放数
        /// </summary>
        /// <param name="UID"></param>
        /// <returns></returns>
        public int TimeOutRedCount(Guid UID)
        {
            using (DBContext)
            {
                var q = from m in DBContext.RedPacket
                        where m.UID == UID && m.Status == (int)enPacketStatus.Actived && m.RedType == (int)enRedType.Timeout
                        select new
                        {
                            count = DBContext.RedPacketList.Count(a => a.RID == m.RID),
                            total = m.OncePacketCount
                        };
                var models = q.ToList();
                if (models.Count > 0)
                {
                    return models.Sum(m => m.total.Value - m.count);
                }
                else
                    return 0;
            }
        }

        #endregion

        #region 评论红包

        /// <summary>
        /// 评论
        /// </summary>
        /// <returns></returns>
        public int Comment(Guid RID, string Number, Guid UID, string content)
        {
            using (DBContext)
            {
                RedPackComment rpc = new RedPackComment();
                rpc.CID = Guid.NewGuid();
                rpc.Content = content;
                rpc.RID = RID;
                rpc.RRID = Guid.Empty;
                rpc.Number = Number;
                rpc.UID = UID;
                rpc.Status = (int)enStatus.Enabled;
                rpc.CreateTime = rpc.UpdateTime = DateTime.Now;
                DBContext.RedPackComment.Add(rpc);
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 评论
        /// </summary>
        /// <returns></returns>
        public dynamic GetComments(Guid RID, string Number, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = from a in DBContext.RedPackComment
                        from b in DBContext.Users
                        where a.UID == b.ID && a.RID == RID && a.Number == Number && a.Status == (int)enStatus.Enabled
                        orderby a.UpdateTime descending
                        select new
                        {
                            a.UID,
                            b.Name,
                            b.Phone,
                            b.TrueName,
                            b.HeadImg1,
                            a.Content,
                            a.UpdateTime
                        };
                return GetPagedList(q, pagesize, pageinex, out count);
            }
        }

        #endregion

        //public List<RedPacket> GetRedPacketSearch(string key, int pageSize, int pageIndex, out int sum)
        //{
        //    using (DBContext)
        //    {
        //        var q = DBContext.RedPacket.Where(m => m.RID != Guid.Empty);
        //        if (!string.IsNullOrEmpty(key))
        //        {
        //            q = q.Where(m => m.title.Contains(key));
        //        }
        //        return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
        //    }
        //}


        //public List<Models.RedPacksModel> GetRedPacketSearch(string key, int pagesize, int pageinex, out int count, Guid? UID)
        //{
        //    using (DBContext)
        //    {     
        //        var q = from b in DBContext.RedPacket
        //              //  from u1 in DBContext.Users.DefaultIfEmpty()
        //                join u1 in DBContext.Users.DefaultIfEmpty() on b.UID equals u1.ID
        //                into ww
        //                from xx in ww.DefaultIfEmpty()
        //              //  where u1.ID == b.UID
        //                orderby b.CreateTime, b.Status
        //                select new Models.RedPacksModel
        //                {
        //                    UID = b.UID,
        //                    Name = xx.Name,
        //                    TrueName = xx.TrueName,
        //                    Phone = xx.Phone,
        //                    UserType = xx.UserType,
        //                    RealCheck = xx.RealCheck,
        //                    Email = xx.Email,

        //                    RID = b.RID,
        //                    RedType = b.RedType,
        //                    Status = b.Status,
        //                    BeginTime = b.BeginTime,
        //                    EndTime = b.EndTime,
        //                    CreateTime = b.CreateTime,
        //                    UpdateTime = b.UpdateTime,
        //                    title = b.title,
        //                    imglist = b.imglist,
        //                    LinkTo = b.LinkTo,
        //                    DelayTime = b.DelayTime,
        //                    OncePacketCount = b.OncePacketCount,
        //                    SinglePrice = b.SinglePrice,
        //                    TotalPrice = b.TotalPrice
        //                };
        //        if (UID != null)
        //        {
        //            q = q.Where(m => m.UID == UID.Value);
        //        }
        //        if (!string.IsNullOrWhiteSpace(key))
        //        {
        //            q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key) || m.title.Contains(key));
        //        }

        //        return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
        //    }
        //}

        public List<Models.RedPacksModel> GetRedPacketSearch(string key, string userType, string type, DateTime? beginTime, DateTime? endTime, int pagesize, int pageindex, out int sum)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from (select ROW_NUMBER() over(order by b.CreateTime desc) row_number,u.Name,u.TrueName,u.Phone,u.UserType,u.RealCheck,u.Email, b.UID, b.RID, b.RedType,b.Status,b.BeginTime,b.CreateTime,b.EndTime,b.UpdateTime,b.title,b.imglist,b.LinkTo,b.DelayTime,b.OncePacketCount,b.TotalPrice,b.SinglePrice ");
            sql.Append("from  RedPacket b  left join Users u  on b.UID=u.ID ");

            StringBuilder tj = new StringBuilder();
            sql.Append("where 1=1 ");
            if (!string.IsNullOrEmpty(key))
            {
                tj.Append(" and (u.phone like @key or b.title like @key or u.TrueName like @key) ");
            }
            if (!string.IsNullOrEmpty(type))
            {
                tj.Append(" and b.RedType = " + type);
            }
            if (!string.IsNullOrEmpty(userType))
            {
                tj.Append(" and u.UserType=" + userType);
            }
            if (beginTime != null)
            {
                tj.Append(" and b.CreateTime>convert(datetime,'" + beginTime.Value.ToString("yyyy-MM-dd") + "')");
            }
            if (endTime != null)
            {
                tj.Append(" and b.CreateTime<convert(datetime,'" + endTime.Value.AddDays(1).ToString("yyyy-MM-dd") + "')");
            }

            string sqlend = " ) as t where t.row_number > @min and t.row_number <= @max";
            using (DBContext)
            {
                sum = DBContext.Database.SqlQuery<int>("select count(*) from RedPacket b left join Users u on b.UID=u.ID where 1=1 " + tj.ToString(), new SqlParameter[] {
                    new SqlParameter("@key","%"+key+"%")
                }).FirstOrDefault();
                return DBContext.Database.SqlQuery<Models.RedPacksModel>(sql.Append(tj).ToString() + sqlend, new SqlParameter[] {
                    new SqlParameter("@key","%"+key+"%"),
                    new SqlParameter("@min", (pageindex - 1) * pagesize),
                    new SqlParameter("@max", pageindex * pagesize)
                }).ToList();

            }
        }

        public List<Models.RedPacksModel> GetRedPacketExcel(string key, string userType, string type, DateTime? beginTime, DateTime? endTime)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from (select ROW_NUMBER() over(order by b.CreateTime desc) row_number,u.Name,u.TrueName,u.Phone,u.UserType,u.RealCheck,u.Email, b.UID, b.RID, b.RedType,b.Status,b.BeginTime,b.CreateTime,b.EndTime,b.UpdateTime,b.title,b.imglist,b.LinkTo,b.DelayTime,b.OncePacketCount,b.TotalPrice,b.SinglePrice ");
            sql.Append("from  RedPacket b  left join Users u  on b.UID=u.ID ");

            StringBuilder tj = new StringBuilder();
            sql.Append("where 1=1 ");
            if (!string.IsNullOrEmpty(key))
            {
                tj.Append(" and (u.phone like @key or b.title like @key or u.TrueName like @key) ");
            }
            if (!string.IsNullOrEmpty(type))
            {
                tj.Append(" and b.RedType = " + type);
            }
            if (!string.IsNullOrEmpty(userType))
            {
                tj.Append(" and u.UserType=" + userType);
            }
            if (beginTime != null)
            {
                tj.Append(" and b.CreateTime>convert(datetime,'" + beginTime.Value.ToString("yyyy-MM-dd") + "')");
            }
            if (endTime != null)
            {
                tj.Append(" and b.CreateTime<convert(datetime,'" + endTime.Value.AddDays(1).ToString("yyyy-MM-dd") + "')");
            }

            string sqlend = " ) as t";
            using (DBContext)
            {

                return DBContext.Database.SqlQuery<Models.RedPacksModel>(sql.Append(tj).ToString() + sqlend, new SqlParameter[] {
                    new SqlParameter("@key","%"+key+"%")
                }).ToList();

            }
        }

        public List<Models.RedPacksListModels> GetRedPacketListSearch(string key, string userType, string redtype, DateTime? beginTime, DateTime? endTime, int pagesize, int pageinex, out int count, Guid? RID)
        {
            using (DBContext)
            {
                var q = from u1 in DBContext.Users
                        from b in DBContext.RedPacket
                        from c in DBContext.RedPacketList
                        where c.RID == b.RID && c.Status == (int)enPacketStatus.Used && u1.ID == c.UID
                        orderby c.CreateTime, c.Status
                        select new Models.RedPacksListModels
                        {
                            UID = c.UID,
                            Name = u1.Name,
                            HeadImg = u1.HeadImg1,
                            CardNumber = u1.CardNumber,
                            // GName = u1.Name,
                            TrueName = u1.TrueName,
                            Phone = u1.Phone,
                            UserType = u1.UserType,
                            RealCheck = u1.RealCheck,
                            Email = u1.Email,
                            RID = b.RID,
                            title = b.title,
                            RedType = b.RedType,
                            Status = c.Status,
                            Money = c.Money,
                            Number = c.Number,
                            BeginTime = c.BeginTime,
                            EndTime = c.EndTime,
                            CreateTime = c.CreateTime,
                            UpdateTime = c.UpdateTime,

                        };
                if (RID != null)
                {
                    q = q.Where(m => m.RID == RID);
                }
                if (!string.IsNullOrWhiteSpace(key))
                {
                    q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key) || m.title.Contains(key));
                }
                if (!string.IsNullOrEmpty(userType))
                {
                    int Type = int.Parse(userType);
                    q = q.Where(m => m.UserType == Type);
                }
                if (!string.IsNullOrEmpty(redtype))
                {
                    int red = int.Parse(redtype);
                    q = q.Where(m => m.RedType == red);
                }
                if (beginTime.HasValue)
                {
                    q = q.Where(m => m.UpdateTime > beginTime);
                }
                if (endTime.HasValue)
                {
                    DateTime end = endTime.Value.AddDays(1);
                    q = q.Where(m => m.UpdateTime < end);
                }

                return GetPagedList(q.OrderByDescending(m => m.UpdateTime), pagesize, pageinex, out count);
            }
        }


        public List<Models.RedPacksListModels> GetRedPacketListExcel(string key, string userType, string redtype, DateTime? beginTime, DateTime? endTime, Guid? RID)
        {
            using (DBContext)
            {
                var q = from u1 in DBContext.Users
                        from b in DBContext.RedPacket
                        from c in DBContext.RedPacketList
                        where c.RID == b.RID && c.Status == (int)enPacketStatus.Used && u1.ID == c.UID
                        orderby c.CreateTime, c.Status
                        select new Models.RedPacksListModels
                        {
                            UID = c.UID,
                            Name = u1.Name,
                            // GName = u1.Name,
                            TrueName = u1.TrueName,
                            Phone = u1.Phone,
                            UserType = u1.UserType,
                            RealCheck = u1.RealCheck,
                            Email = u1.Email,
                            RID = b.RID,
                            title = b.title,
                            RedType = b.RedType,
                            Status = c.Status,
                            Money = c.Money,
                            Number = c.Number,
                            BeginTime = c.BeginTime,
                            EndTime = c.EndTime,
                            CreateTime = c.CreateTime,
                            UpdateTime = c.UpdateTime,

                        };
                if (RID != null)
                {
                    q = q.Where(m => m.RID == RID);
                }
                if (!string.IsNullOrWhiteSpace(key))
                {
                    q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key) || m.title.Contains(key));
                }
                if (!string.IsNullOrEmpty(userType))
                {
                    int Type = int.Parse(userType);
                    q = q.Where(m => m.UserType == Type);
                }
                if (!string.IsNullOrEmpty(redtype))
                {
                    int red = int.Parse(redtype);
                    q = q.Where(m => m.RedType == red);
                }
                if (beginTime.HasValue)
                {
                    q = q.Where(m => m.UpdateTime > beginTime);
                }
                if (endTime.HasValue)
                {
                    DateTime end = endTime.Value.AddDays(1);
                    q = q.Where(m => m.UpdateTime < end);
                }

                return q.OrderByDescending(m => m.CreateTime).ToList();
            }
        }


        public RedPacket GetRedPacketById(Guid id)
        {
            using (DBContext)
            {
                return DBContext.RedPacket.FirstOrDefault(m => m.RID == id);
            }
        }

        public int InsertRedPacket(string title, string img, decimal SinglePrice, decimal TotalPrice)
        {
            using (DBContext)
            {
                RedPacket model = new RedPacket();
                model.RID = Guid.NewGuid();
                model.title = title;
                model.imglist = img;
                model.SinglePrice = SinglePrice;
                model.TotalPrice = TotalPrice;
                model.Status = (int)enStatus.Enabled;
                model.CreateTime = model.UpdateTime = DateTime.Now;

                DBContext.RedPacket.Add(model);
                return DBContext.SaveChanges();
            }
        }

        public int UpdateRedPacket(RedPacket model)
        {
            using (DBContext)
            {
                DBContext.RedPacket.Attach(model);
                DBContext.Entry<RedPacket>(model).State = System.Data.Entity.EntityState.Modified;
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 更改红包状态
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int ChangeRedPacketStatus(Guid uid, enStatus status)
        {
            using (DBContext)
            {
                RedPacket Red = DBContext.RedPacket.Where(m => m.RID == uid).FirstOrDefault();
                if (Red != null)
                {
                    Red.Status = (int)status;
                    Red.UpdateTime = DateTime.Now;
                }
                return DBContext.SaveChanges();
            }
        }


        public List<Models.RedPacksModel> GetRedPacketListByRedType(string key, string userType, enRedType? redtype, DateTime? beginTime, DateTime? endTime, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                //from u1 in DBContext.Users
                //from b in DBContext.RedPacket
                //where  u1.ID == b.UID
                var q = from b in DBContext.RedPacket
                        join u1 in DBContext.Users.DefaultIfEmpty() on b.UID equals u1.ID
                        into ww
                        from xx in ww.DefaultIfEmpty()
                        orderby b.CreateTime descending
                        select new Models.RedPacksModel
                        {
                            // UID = u1.UID,
                            Name = xx.Name == null ? "系统" : (xx.Phone + "【" + xx.CardNumber + "】"),
                            // GName = u1.Name,
                            UID = b.UID,
                            HeadImg = xx.Name == null ? "/homecss/image/logo.png" : xx.HeadImg1,
                            TrueName = xx.TrueName,
                            Phone = xx.Phone,
                            UserType = xx.UserType,
                            RealCheck = xx.RealCheck,
                            Email = xx.Email,

                            RID = b.RID,
                            RedType = b.RedType,
                            Status = b.Status,
                            BeginTime = b.BeginTime,
                            EndTime = b.EndTime,
                            CreateTime = b.CreateTime,
                            UpdateTime = b.UpdateTime,
                            title = b.title,
                            imglist = b.imglist,
                            LinkTo = b.LinkTo,
                            DelayTime = b.DelayTime,
                            OncePacketCount = b.OncePacketCount,
                            SinglePrice = b.SinglePrice,
                            TotalPrice = b.TotalPrice

                        };
                if (!string.IsNullOrWhiteSpace(key))
                {
                    q = q.Where(m => m.Name == key || m.TrueName.Contains(key) || m.Phone.Contains(key) || m.title.Contains(key));
                }
                if (!string.IsNullOrEmpty(userType))
                {
                    int Type = int.Parse(userType);
                    q = q.Where(m => m.UserType == Type);
                }
                //if (!string.IsNullOrEmpty(redtype))
                //{
                //   //if(redtype==1)
                //    int red = int.Parse(redtype);
                //    q = q.Where(m => m.RedType == red);
                //}
                if (redtype != null)
                {
                    if (redtype == enRedType.Single)
                    {
                        q = q.Where(m => m.RedType == (int)enRedType.Single);
                    }
                    else if (redtype == 0)
                    {
                        q = q.Where(m => m.RedType != (int)enRedType.Single);
                    }
                    else if (redtype == enRedType.Auto_UserRecRed || redtype == enRedType.Auto_SystemAchieveSend || redtype == enRedType.Auto_NewAgentSend)
                    {
                        q = q.Where(m => m.RedType == (int)enRedType.Auto_UserRecRed || m.RedType == (int)enRedType.Auto_SystemAchieveSend || m.RedType == (int)enRedType.Auto_NewAgentSend);
                    }
                    else
                    {
                        q = q.Where(m => m.RedType == (int)redtype);
                    }
                }
                if (beginTime.HasValue)
                {
                    q = q.Where(m => m.UpdateTime > beginTime);
                }
                if (endTime.HasValue)
                {
                    DateTime end = endTime.Value.AddDays(1);
                    q = q.Where(m => m.UpdateTime < end);
                }

                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }



    }
}
