﻿//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace RelexBarDLL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RelexBarEntities : DbContext
    {
        public RelexBarEntities()
            : base("name=RelexBarEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AdminPermission> AdminPermission { get; set; }
        public DbSet<AdminUser> AdminUser { get; set; }
        public DbSet<AdsList> AdsList { get; set; }
        public DbSet<AlbumList> AlbumList { get; set; }
        public DbSet<BankList> BankList { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<ChatGroup> ChatGroup { get; set; }
        public DbSet<ChatGroupUser> ChatGroupUser { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<FriendShip> FriendShip { get; set; }
        public DbSet<GCZone> GCZone { get; set; }
        public DbSet<GCZoneComment> GCZoneComment { get; set; }
        public DbSet<GCZoneLike> GCZoneLike { get; set; }
        public DbSet<GCZoneTimeLine> GCZoneTimeLine { get; set; }
        public DbSet<InfomationComment> InfomationComment { get; set; }
        public DbSet<Infomations> Infomations { get; set; }
        public DbSet<InfomationType> InfomationType { get; set; }
        public DbSet<InfomationViewList> InfomationViewList { get; set; }
        public DbSet<Logs> Logs { get; set; }
        public DbSet<MyCollection> MyCollection { get; set; }
        public DbSet<OrderList> OrderList { get; set; }
        public DbSet<OrderProductList> OrderProductList { get; set; }
        public DbSet<OtherLoginToken> OtherLoginToken { get; set; }
        public DbSet<OtherPayServiceLog> OtherPayServiceLog { get; set; }
        public DbSet<PayList> PayList { get; set; }
        public DbSet<PayOrder> PayOrder { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<PhotoPath> PhotoPath { get; set; }
        public DbSet<Problem> Problem { get; set; }
        public DbSet<ProductList> ProductList { get; set; }
        public DbSet<ProductSpecification> ProductSpecification { get; set; }
        public DbSet<RecAddress> RecAddress { get; set; }
        public DbSet<Recharge> Recharge { get; set; }
        public DbSet<RechargeCard> RechargeCard { get; set; }
        public DbSet<RedPackComment> RedPackComment { get; set; }
        public DbSet<RedPacket> RedPacket { get; set; }
        public DbSet<RedPacketGood> RedPacketGood { get; set; }
        public DbSet<RedPacketList> RedPacketList { get; set; }
        public DbSet<RedRecord> RedRecord { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<ServiceList> ServiceList { get; set; }
        public DbSet<ServiceOrder> ServiceOrder { get; set; }
        public DbSet<SysConfig> SysConfig { get; set; }
        public DbSet<SysPrice> SysPrice { get; set; }
        public DbSet<TempIndex> TempIndex { get; set; }
        public DbSet<TransferOut> TransferOut { get; set; }
        public DbSet<UserHelp> UserHelp { get; set; }
        public DbSet<UserMsg> UserMsg { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<VerifyCodes> VerifyCodes { get; set; }
        public DbSet<VersionList> VersionList { get; set; }
        public DbSet<Web_Area> Web_Area { get; set; }
        public DbSet<vw_Orders> vw_Orders { get; set; }
    }
}