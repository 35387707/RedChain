using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelexBarDLL;

namespace RelexBarBLL
{
    public partial class RecAddressBLL : BaseBll
    {
        public List<RecAddress> GetUserAddressList(Guid Uid)
        {
            using (DBContext)
            {
                return DBContext.RecAddress.Where(m => m.UID == Uid && m.Status == (int)enStatus.Enabled).ToList();
            }
        }
        public List<RecAddress> GetUserAddressList(string Uid)
        {
            return GetUserAddressList(Guid.Parse(Uid));
        }

        public RecAddress GetAddressDetail(Guid id)
        {
            using (DBContext)
            {
                return DBContext.RecAddress.FirstOrDefault(m => m.ID == id && m.Status == (int)enStatus.Enabled);
            }
        }
        public RecAddress GetAddressDetail(string id)
        {
            return GetAddressDetail(Guid.Parse(id));
        }

        public Guid InsertAddress(Guid uid, string recname, string areid, string address, string phone, string areacode,
            string email, int? sex, bool isDefault)
        {
            using (DBContext)
            {
                RecAddress model = new RecAddress();
                model.ID = Guid.NewGuid();
                model.UID = uid;
                model.Address = address;
                model.AreaCode = areacode;
                model.AreaID = areid;
                model.Email = email;
                model.Phone = phone;
                model.Sex = sex;
                model.Status = (int)enStatus.Enabled;
                model.TrueName = recname;
                model.isDefault = isDefault;
                model.UpdateTime = model.CreateTime = DateTime.Now;
                DBContext.RecAddress.Add(model);
                var i = DBContext.SaveChanges();
                if (i > 0)
                {
                    if (isDefault)//如果是默认，则把其他的设置为非默认
                    {
                        ExceSql("update RecAddress set isDefault=0 where uid={0} and id <>{1}", uid, model.ID);
                    }
                    return model.ID;
                }
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 修改默认地址
        /// </summary>
        /// <returns></returns>
        public int EditDefault(Guid id, Guid UID, bool isDefault)
        {
            using (DBContext)
            {
                var model = DBContext.RecAddress.FirstOrDefault(m => m.ID == id && m.UID == UID);
                if (model == null)
                    return 0;

                model.isDefault = isDefault;

                if (DBContext.SaveChanges() > 0)
                {
                    if (isDefault)//如果是默认，则把其他的设置为非默认
                    {
                        ExceSql("update RecAddress set isDefault=0 where uid={0} and id <>{1}", UID, model.ID);
                    }
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 获取默认收货地址
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public RecAddress GetDefault(Guid UID)
        {
            using (DBContext)
            {
                return DBContext.RecAddress.FirstOrDefault(m => m.UID == UID && m.isDefault);
            }
        }

        public RecAddress Detail(Guid id)
        {
            using (DBContext)
            {
                return DBContext.RecAddress.FirstOrDefault(m => m.ID == id);
            }
        }

        public int DeleteAddress(Guid id)
        {
            using (DBContext)
            {
                var q = DBContext.RecAddress.FirstOrDefault(m => m.ID == id);
                if (q == null)
                    return 0;
                q.Status = (int)enStatus.Unabled;
                return DBContext.SaveChanges();
            }
        }
        public int DeleteAddress(string id)
        {
            return DeleteAddress(Guid.Parse(id));
        }

        public int UpdateAddress(Guid id, string recname, string areid, string address, string phone, string areacode, string email,
            int? sex, bool isDefault)
        {
            using (DBContext)
            {
                RecAddress model = DBContext.RecAddress.FirstOrDefault(m => m.ID == id);
                if (model == null)
                    return 0;
                model.Address = address;
                model.AreaCode = areacode;
                model.AreaID = areid;
                model.Email = email;
                model.Phone = phone;
                model.Sex = sex;
                model.TrueName = recname;
                model.UpdateTime = DateTime.Now;
                model.isDefault = isDefault;

                var i = DBContext.SaveChanges();
                if (i > 0)
                {
                    if (isDefault)//如果是默认，则把其他的设置为非默认
                    {
                        ExceSql("update RecAddress set isDefault=0 where uid={0} and id <>{1}", model.UID, model.ID);
                    }
                }
                return i;
            }
        }

        private string getAddress(int areaId)
        {
            using (DBContext)
            {
                try
                {
                    Web_Area area = DBContext.Web_Area.FirstOrDefault(m => m.ID == areaId);
                    if (area == null)
                    {
                        return null;
                    }
                    int[] arr = Array.ConvertAll<string, int>(area.Family.Split(','), s => int.Parse(s));
                    StringBuilder address = new StringBuilder();

                    for (int i = 1; i < arr.Length - 1; i++)
                    {
                        int aid = arr[i];
                        Web_Area a = DBContext.Web_Area.FirstOrDefault(m => m.ID == aid);
                        if (a != null)
                        {
                            address.Append(a.Name);
                        }
                    }
                    address.Append(area.Name);
                    return address.ToString();
                }
                catch (Exception)
                {

                    throw;
                }
            }



        }
    }
}
