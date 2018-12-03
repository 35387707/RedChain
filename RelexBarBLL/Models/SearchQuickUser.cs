﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL.Models
{
    public class SearchQuickUser
    {
        public Guid ID { get; set; }
        public string CardNumber { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int? Sex { get; set; }
        public string HeadImg { get; set; }
        public string Email { get; set; }
        public string TrueName { get; set; }
        public string WxName { get; set; }
        public string Psw { get; set; }
        public string PayPsw { get; set; }
        public string CredID { get; set; }
        public string CredImg1 { get; set; }
        public string CredImg2 { get; set; }
      
        public int LV { get; set; }
        public decimal Score { get; set; }
        public decimal TotalScore { get; set; }
        public decimal Balance { get; set; }
        public int UserType { get; set; }
        public string AreaID { get; set; }
        public string Address { get; set; }
        public string Descrition { get; set; }
        public string HeadImg1 { get; set; }
        public string HeadImg2 { get; set; }
        public Nullable<System.Guid> FID { get; set; }
        public int RealCheck { get; set; }
      //  public int C_index { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string PhoneAreaCode { get; set; }
        public Nullable<int> OS { get; set; }
        public System.DateTime LastLoginTime { get; set; }
        public string AreaCode { get; set; }
        public string DEVICE { get; set; }
        public int AddFriendVerify { get; set; }
        public decimal RedBalance { get; set; }
    }
}
