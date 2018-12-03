using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;

namespace RelexBarBLL
{
    public class Rewards : BaseBll
    {
        #region Fields
        /// <summary>
        /// 点击获取红包的时候上级可得百分比
        /// </summary>
        protected static decimal GetRedPacketPer1 = 0.05M;
        /// <summary>
        /// 点击获取红包的时候第二代上级可得百分比
        /// </summary>
        protected static decimal GetRedPacketPer2 = 0.05M;
        /// <summary>
        /// 点击获取红包的时候第一代代理可得百分比
        /// </summary>
        protected static decimal GetRedPacketPerDL1 = 0.05M;
        /// <summary>
        /// 点击获取红包的时候第二代代理可得百分比
        /// </summary>
        protected static decimal GetRedPacketPerDL2 = 0.05M;
        /// <summary>
        /// 点击获取红包的时候第一代商家可得百分比
        /// </summary>
        protected static decimal GetRedPacketPerSJ1 = 0.02M;
        /// <summary>
        /// 点击获取红包的时候第二代商家可得百分比
        /// </summary>
        protected static decimal GetRedPacketPerSJ2 = 0.02M;

        /// <summary>
        /// 发红包的时候上级可得百分比
        /// </summary>
        protected static decimal SendRedPacketPer1 = 0.05M;
        /// <summary>
        /// 发红包的时候第二代上级可得百分比
        /// </summary>
        protected static decimal SendRedPacketPer2 = 0.05M;
        /// <summary>
        /// 发红包的时候第一代代理可得百分比
        /// </summary>
        protected static decimal SendRedPacketPerDL1 = 0.05M;
        /// <summary>
        /// 发红包的时候第二代代理可得百分比
        /// </summary>
        protected static decimal SendRedPacketPerDL2 = 0.05M;


        /// <summary>
        /// 提现的时候第一代代理可得百分比
        /// </summary>
        protected static decimal TransforoutPerDL1 = 0.01M;
        /// <summary>
        /// 提现的时候第二代代理可得百分比
        /// </summary>
        protected static decimal TransforoutPerDL2 = 0.01M;

        #endregion

        /// <summary>
        /// 发送红包后的奖励计算
        /// </summary>
        /// <param name="RID">总红包ID</param>
        public void SendRedRewards(Guid RID)
        {
            using (DBContext)
            {
                var redpack = DBContext.RedPacket.FirstOrDefault(m => m.RID == RID);
                if (redpack != null)
                {
                    var money = redpack.TotalPrice;//红包金额
                    Users FirFUser, SecFuser;
                    UsersBLL.GetFUsersByUID(DBContext, redpack.UID, out FirFUser, out SecFuser);
                    int count = 0, fjcount = 0;//福将，福相

                    if (FirFUser != null)
                    {
                        var pri = money * SendRedPacketPer1;
                        PayListBLL.Insert(DBContext, RID, FirFUser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, pri, "直推发福包分润");

                        if (FirFUser.UserType == (int)enUserType.Agent)//如果是代理则金额更多
                        {
                            var td = money * SendRedPacketPerDL1;
                            pri += td;

                            PayListBLL.Insert(DBContext, RID, FirFUser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, td, "福相团队一级发福包分润");
                            count++;
                            fjcount++;
                        }
                        else if (FirFUser.UserType == (int)enUserType.Shop)//如果是商家，则拿2%
                        {
                            var td = money * GetRedPacketPerSJ1;
                            pri += td;

                            PayListBLL.Insert(DBContext, RID, FirFUser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, td, "福将团队一级发福包分润");
                            fjcount++;
                        }

                        FirFUser.Balance += pri;
                    }
                    if (SecFuser != null)
                    {
                        var pri = money * SendRedPacketPer2;
                        PayListBLL.Insert(DBContext, RID, SecFuser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, pri, "间推发福包分润");

                        if (SecFuser.UserType == (int)enUserType.Agent)//如果是代理则金额更多
                        {
                            var td = money * SendRedPacketPerDL2;
                            pri += td;

                            PayListBLL.Insert(DBContext, RID, SecFuser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, td, count == 0 ? "福相团队一级发福包分润" : "福相团队次级发福包分润");
                            count++;
                            fjcount++;
                        }
                        else if (SecFuser.UserType == (int)enUserType.Shop)//如果是商家，则拿2%
                        {
                            var td = money * GetRedPacketPerSJ2;
                            pri += td;

                            PayListBLL.Insert(DBContext, RID, SecFuser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, td, fjcount == 0 ? "福将团队一级发福包分润" : "福将团队次级发福包分润");
                            fjcount++;
                        }
                        SecFuser.Balance += pri;
                    }
                    if (count < 2)//如果以上两层并不都是代理，则需要查找上一级代理
                    {
                        var ass = UsersBLL.GetShopAgentUsers(DBContext, redpack.UID, 4);//获取4代代理或商家
                        for (int i = count; i < ass.Count; i++)
                        {
                            if (ass[i].UserType == (int)enUserType.Agent)//如果是代理，则
                            {
                                var pri = money * SendRedPacketPer2;
                                ass[i].Balance += pri;

                                PayListBLL.Insert(DBContext, RID, ass[i].ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, pri, i == 0 ? "福相团队一级发福包分润" : "福相团队次级发福包分润");
                            }
                            else
                            {
                                if (fjcount < 2)
                                {
                                    var pri = money * GetRedPacketPerSJ2;
                                    ass[i].Balance += pri;

                                    PayListBLL.Insert(DBContext, RID, ass[i].ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, pri, i == 0 ? "福将团队一级发福包分润" : "福将团队次级发福包分润");
                                }
                            }
                            fjcount++;
                        }
                    }

                    DBContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 点击领取红包后的奖励计算
        /// </summary>
        /// <param name="RLID">红包ID</param>
        public void ClickRedRewards(Guid RLID)
        {
            using (DBContext)
            {
                var redpack = DBContext.RedPacketList.FirstOrDefault(m => m.RLID == RLID);
                if (redpack != null)
                {
                    var money = redpack.Money;//红包金额
                    Users FirFUser, SecFuser;
                    UsersBLL.GetFUsersByUID(DBContext, redpack.UID.Value, out FirFUser, out SecFuser);
                    int count = 0, fjcount = 0;//福将，福相

                    if (FirFUser != null)
                    {
                        var pri = money * GetRedPacketPer1;
                        PayListBLL.Insert(DBContext, RLID, FirFUser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, pri, "直推收福包分润");

                        if (FirFUser.UserType == (int)enUserType.Agent)//如果是代理则金额更多
                        {
                            var td = money * GetRedPacketPerDL1;
                            pri += td;

                            PayListBLL.Insert(DBContext, RLID, FirFUser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, td, "福相团队一级收福包分润");
                            count++;
                            fjcount++;
                        }
                        else if (FirFUser.UserType == (int)enUserType.Shop)//如果是商家，则拿2%
                        {
                            var td = money * GetRedPacketPerSJ1;
                            pri += td;

                            PayListBLL.Insert(DBContext, RLID, FirFUser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, td, "福将团队一级收福包分润");
                            fjcount++;
                        }
                        FirFUser.Balance += pri;
                    }
                    if (SecFuser != null)
                    {
                        var pri = money * GetRedPacketPer2;
                        PayListBLL.Insert(DBContext, RLID, SecFuser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, pri, "间推收福包分润");

                        if (SecFuser.UserType == (int)enUserType.Agent)//如果是代理则金额更多
                        {
                            var td = money * GetRedPacketPerDL2;
                            pri += td;

                            PayListBLL.Insert(DBContext, RLID, SecFuser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, td, count == 0 ? "福相团队一级收福包分润" : "福相团队次级收福包分润");
                            count++;
                            fjcount++;
                        }
                        else if (SecFuser.UserType == (int)enUserType.Shop)//如果是商家，则拿2%
                        {
                            var td = money * GetRedPacketPerSJ2;
                            pri += td;

                            PayListBLL.Insert(DBContext, RLID, SecFuser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, td, fjcount == 0 ? "福将团队一级收福包分润" : "福将团队次级收福包分润");
                            fjcount++;
                        }
                        SecFuser.Balance += pri;
                    }

                    if (count < 2)//如果以上两层并不都是代理，则需要查找上一级代理
                    {
                        var ass = UsersBLL.GetShopAgentUsers(DBContext, redpack.UID.Value, 4);//获取4代代理或商家
                        for (int i = count; i < ass.Count; i++)
                        {
                            if (ass[i].UserType == (int)enUserType.Agent)//如果是代理，则
                            {
                                var pri = money * GetRedPacketPerDL2;
                                ass[i].Balance += pri;

                                PayListBLL.Insert(DBContext, RLID, ass[i].ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, pri, i == 0 ? "福相团队一级收福包分润" : "福相团队次级收福包分润");
                            }
                            else
                            {
                                if (fjcount < 2)
                                {
                                    var pri = money * GetRedPacketPerSJ2;
                                    ass[i].Balance += pri;

                                    PayListBLL.Insert(DBContext, RLID, ass[i].ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, pri, i == 0 ? "福将团队一级收福包分润" : "福将团队次级收福包分润");
                                }
                            }
                            fjcount++;
                        }
                    }

                    DBContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 创建新会员的奖励计算
        /// </summary>
        /// <param name="NewUserID">新会员ID</param>
        public void CreateUserRewards(Guid NewUserID)
        { }

        /// <summary>
        /// 升级会员后的佣金计算
        /// </summary>
        /// <param name="UID">升级的用户</param>
        public void UpadateUserLVRewards(Guid UID)
        {
            using (DBContext)
            {
                var upUser = DBContext.Users.FirstOrDefault(m => m.ID == UID);//谁升级？
                if (upUser != null && upUser.UserType != (int)enUserType.User)
                {
                    Users FirFUser, SecFuser;
                    UsersBLL.GetFUsersByUID(DBContext, upUser.ID, out FirFUser, out SecFuser);
                    var money = 0;
                    var totalmoney = 0;
                    #region  如果被推荐人是代理
                    if (upUser.UserType == (int)enUserType.Agent)//如果被推荐人是代理，只有代理推荐代理才又奖励
                    {
                        totalmoney = 4000;
                        money = 2000;//最终加起来是4000就对了！
                        int dailyCount = 0;

                        if (FirFUser != null)
                        {
                            if (FirFUser.UserType != (int)enUserType.Agent)//如果不是代理，则只领1000元
                            {
                                money = 1000;
                            }
                            else//如果推荐人是代理，则奖励2000元
                            {
                                money = 2000;
                                dailyCount++;
                            }
                            totalmoney -= money;

                            FirFUser.Balance += money * (1 - SysConfigBLL.Poundage);
                            //FirFUser.RedBalance += money * SysConfigBLL.Poundage;
                            PayListBLL.Insert(DBContext, null, FirFUser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Commission, money, "直推福相奖励");

                            new RedPacksBLL().SendRedPacket(FirFUser.ID, enRedType.Auto_RewardSend, "直推福相奖励福包", "/homecss/image/redpack.jpg", "", 0, 0, 0, money * SysConfigBLL.Poundage, "", "", null);//粉丝、广告商、代理商的奖金系统自动扣除10%用来发红包
                            PayListBLL.Insert(DBContext, null, FirFUser.ID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, money * SysConfigBLL.Poundage, "直推福相奖励自动发福包");
                        }
                        if (SecFuser != null)
                        {
                            if (SecFuser.UserType != (int)enUserType.Agent)//如果不是代理，则只领1000元
                            {
                                money = 1000;
                            }
                            else//如果推荐人是代理，则奖励2000元
                            {
                                money = 2000;
                                dailyCount++;
                            }
                            totalmoney -= money;

                            SecFuser.Balance += money * (1 - SysConfigBLL.Poundage);
                            //SecFuser.RedBalance += money * SysConfigBLL.Poundage;
                            PayListBLL.Insert(DBContext, null, SecFuser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Commission, money, "间推福相奖励");

                            new RedPacksBLL().SendRedPacket(SecFuser.ID, enRedType.Auto_RewardSend, "间推福相奖励福包", "/homecss/image/redpack.jpg", "", 0, 0, 0, money * SysConfigBLL.Poundage, "", "", null);//粉丝、广告商、代理商的奖金系统自动扣除10%用来发红包
                            PayListBLL.Insert(DBContext, null, SecFuser.ID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, money * SysConfigBLL.Poundage, "间推福相奖励自动发福包");
                        }

                        if (totalmoney > 0 && totalmoney < 4000)//如果还有钱，则没有一分都不少，则说明两级内还没领完，则推送到最接近的两级代理分别领取
                        {
                            if (SecFuser != null && SecFuser.FID.HasValue)//第二级推荐人不为空
                            {
                                switch (dailyCount)
                                {
                                    case 0://一二级两个都不是代理
                                        var agent1 = UsersBLL.GetFUserByLV(DBContext, SecFuser.FID, enUserType.Agent);//最接近的那级代理人
                                        if (agent1 != null)//如果上级推荐人不是代理，则可以获取伞下全部广告商的10%
                                        {
                                            money = 1000;

                                            agent1.Balance += money * (1 - SysConfigBLL.Poundage);
                                            PayListBLL.Insert(DBContext, null, agent1.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Commission, money, "团队一级福相奖励");

                                            new RedPacksBLL().SendRedPacket(agent1.ID, enRedType.Auto_RewardSend, "团队一级福相福包", "/homecss/image/redpack.jpg", "", 0, 0, 0, money * SysConfigBLL.Poundage, "", "", null);//粉丝、广告商、代理商的奖金系统自动扣除10%用来发红包
                                            PayListBLL.Insert(DBContext, null, agent1.ID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, money * SysConfigBLL.Poundage, "团队一级福相奖励自动发福包");

                                            var agent2 = UsersBLL.GetFUserByLV(DBContext, agent1.FID, enUserType.Agent);//最接近的那级代理人
                                            if (agent2 != null)//如果上级推荐人不是代理，则可以获取伞下全部广告商的10%
                                            {
                                                money = 1000;

                                                agent2.Balance += money * (1 - SysConfigBLL.Poundage);
                                                PayListBLL.Insert(DBContext, null, agent2.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Commission, money, "团队次级福相奖励");

                                                new RedPacksBLL().SendRedPacket(agent2.ID, enRedType.Auto_RewardSend, "团队次级福相奖励福包", "/homecss/image/redpack.jpg", "", 0, 0, 0, money * SysConfigBLL.Poundage, "", "", null);//粉丝、广告商、代理商的奖金系统自动扣除10%用来发红包
                                                PayListBLL.Insert(DBContext, null, agent2.ID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, money * SysConfigBLL.Poundage, "团队次级福相奖励自动发福包");
                                            }
                                        }

                                        break;//
                                    case 1://有一个是代理
                                        var agent11 = UsersBLL.GetFUserByLV(DBContext, SecFuser.FID, enUserType.Agent);//最接近的那级代理人
                                        if (agent11 != null)//如果上级推荐人不是代理，则可以获取伞下全部广告商的10%
                                        {
                                            money = 2000;

                                            agent11.Balance += money * (1 - SysConfigBLL.Poundage);
                                            PayListBLL.Insert(DBContext, null, agent11.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Commission, money, "团队次级福相奖励");

                                            new RedPacksBLL().SendRedPacket(agent11.ID, enRedType.Auto_RewardSend, "团队次级福相奖励福包", "/homecss/image/redpack.jpg", "", 0, 0, 0, money * SysConfigBLL.Poundage, "", "", null);//粉丝、广告商、代理商的奖金系统自动扣除10%用来发红包
                                            PayListBLL.Insert(DBContext, null, agent11.ID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, money * SysConfigBLL.Poundage, "团队次级福相奖励自动发福包");
                                        }
                                        break;
                                    case 2: break;//如果两个都是代理，那肯定是结束的了!
                                }
                            }
                        }
                    }
                    #endregion
                    #region  如果被推荐人是商家
                    else if (upUser.UserType == (int)enUserType.Shop)//如果被推荐人是商家
                    {
                        money = 0;
                        totalmoney = 1200;

                        if (FirFUser != null)
                        {
                            if (FirFUser.UserType == (int)enUserType.User)//如果推荐人是普通用户
                            {
                                money = 300;
                            }
                            else//如果推荐人是其他用户
                            {
                                money = 600;
                            }
                            totalmoney -= money;

                            FirFUser.Balance += money * (1 - SysConfigBLL.Poundage);
                            //FirFUser.RedBalance += money * SysConfigBLL.Poundage;
                            PayListBLL.Insert(DBContext, null, FirFUser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Commission, money, "直推福将奖励");

                            new RedPacksBLL().SendRedPacket(FirFUser.ID, enRedType.Auto_RewardSend, "直推福将奖励福包", "/homecss/image/redpack.jpg", "", 0, 0, 0, money * SysConfigBLL.Poundage, "", "", null);//粉丝、广告商、代理商的奖金系统自动扣除10%用来发红包
                            PayListBLL.Insert(DBContext, null, FirFUser.ID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, money * SysConfigBLL.Poundage, "直推福将奖励自动发福包");
                        }

                        if (SecFuser != null)
                        {
                            if (SecFuser.UserType == (int)enUserType.User)//如果推荐人是普通用户
                            {
                                money = 300;
                            }
                            else//如果推荐人是其他用户
                            {
                                money = 600;
                            }
                            totalmoney -= money;

                            SecFuser.Balance += money * (1 - SysConfigBLL.Poundage);
                            //SecFuser.RedBalance += money * SysConfigBLL.Poundage;
                            PayListBLL.Insert(DBContext, null, SecFuser.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, money, "间推福将奖励");

                            new RedPacksBLL().SendRedPacket(SecFuser.ID, enRedType.Auto_RewardSend, "间推福将奖励福包", "/homecss/image/redpack.jpg", "", 0, 0, 0, money * SysConfigBLL.Poundage, "", "", null);//粉丝、广告商、代理商的奖金系统自动扣除10%用来发红包
                            PayListBLL.Insert(DBContext, null, SecFuser.ID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, money * SysConfigBLL.Poundage, "间推福将奖励自动发福包");
                        }

                        if (totalmoney > 0 && totalmoney <= 1200)//如果还有钱，则没有一分都不少，则说明两级内还没领完，则推送到最接近的两级代理分别领取
                        {
                            if (SecFuser != null && SecFuser.FID.HasValue)//第二级推荐人不为空
                            {
                                var agent1 = UsersBLL.GetFUserByLV(DBContext, SecFuser.FID, enUserType.Agent);//最接近的那级代理人
                                if (agent1 != null)//
                                {
                                    money = 300;
                                    totalmoney -= money;

                                    agent1.Balance += money * (1 - SysConfigBLL.Poundage);
                                    PayListBLL.Insert(DBContext, null, agent1.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Commission, money, "团队福将一级奖励");

                                    new RedPacksBLL().SendRedPacket(agent1.ID, enRedType.Auto_RewardSend, "团队福将一级奖励福包", "/homecss/image/redpack.jpg", "", 0, 0, 0, money * SysConfigBLL.Poundage, "", "", null);//粉丝、广告商、代理商的奖金系统自动扣除10%用来发红包
                                    PayListBLL.Insert(DBContext, null, agent1.ID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, money * SysConfigBLL.Poundage, "团队福将一级奖励自动发福包");

                                    if (totalmoney >= money)
                                    {
                                        var agent2 = UsersBLL.GetFUserByLV(DBContext, agent1.FID, enUserType.Agent);//最接近的那级代理人
                                        if (agent2 != null)//最接近的二级代理
                                        {
                                            agent2.Balance += money * (1 - SysConfigBLL.Poundage);
                                            PayListBLL.Insert(DBContext, null, agent2.ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Commission, money, "团队福将次级奖励");

                                            new RedPacksBLL().SendRedPacket(agent2.ID, enRedType.Auto_RewardSend, "团队福将次级奖励福包", "/homecss/image/redpack.jpg", "", 0, 0, 0, money * SysConfigBLL.Poundage, "", "", null);//粉丝、广告商、代理商的奖金系统自动扣除10%用来发红包
                                            PayListBLL.Insert(DBContext, null, agent2.ID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, money * SysConfigBLL.Poundage, "团队福将次级奖励自动发福包");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    DBContext.SaveChanges();

                    #region 每两级代理伞下的广告商享有200元的10%是20元
                    if (upUser.UserType == (int)enUserType.Shop)
                    {
                        var agents = UsersBLL.GetLVUsersByUID2(DBContext, upUser.ID, enUserType.Agent, 2);//获取两代代理
                        for (int i = 0; i < agents.Count; i++)
                        {
                            money = 200;

                            agents[i].Balance += money * (1 - SysConfigBLL.Poundage);
                            //FirFUser.RedBalance += money * SysConfigBLL.Poundage;
                            PayListBLL.Insert(DBContext, null, agents[i].ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Commission, money, i == 0 ? "团队福将广告一级奖励【200元】" : "团队福将广告次级奖励【200元】");

                            new RedPacksBLL().SendRedPacket(agents[i].ID, enRedType.Auto_RewardSend, "团队福将广告一级奖励福包", "/homecss/image/redpack.jpg", "", 0, 0, 0, money * SysConfigBLL.Poundage, "", "", null);//粉丝、广告商、代理商的奖金系统自动扣除10%用来发红包
                            PayListBLL.Insert(DBContext, null, agents[i].ID, enPayInOutType.Out, enPayType.Coin, enPayFrom.RedPaged, money * SysConfigBLL.Poundage, i == 0 ? "团队福将广告一级奖励自动发福包" : "团队福将广告次级奖励自动发福包");
                        }
                    }
                    #endregion

                    DBContext.SaveChanges();
                }
            }
        }

        /// <summary>
        /// 提现享有伞下提现收益两代 1%
        /// </summary>
        /// <param name="TransoutID">提现单的ID</param>
        public void TransforOutRewards(Guid TransoutID)
        {
            using (DBContext)
            {
                var trans = DBContext.TransferOut.FirstOrDefault(m => m.ID == TransoutID);
                if (trans != null && trans.Status == (int)enApplyStatus.Success)//提现成功才有奖励
                {
                    var money = trans.Price;//体现金额
                    var agents = UsersBLL.GetLVUsersByUID2(DBContext, trans.UID.Value, enUserType.Agent, 2);//获取两代代理
                    for (int i = 0; i < agents.Count; i++)
                    {
                        var pri = money * TransforoutPerDL1;
                        agents[i].Balance += pri;

                        PayListBLL.Insert(DBContext, TransoutID, agents[i].ID, enPayInOutType.In, enPayType.Coin, enPayFrom.Reward, pri, "团队提现奖励");
                    }
                }
                DBContext.SaveChanges();
            }
        }

    }
}
