using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL
{
    public class ServiceListBLL:BaseBll
    {
        public string GetAppSecretByAppId(string appid) {
            using (DBContext) {
                return DBContext.ServiceList.Where(m => m.AppID == appid).Select(m => m.AppSecret).FirstOrDefault();
            }
        }
        public string GetAppSecretByID(Guid id)
        {
            using (DBContext)
            {
                return DBContext.ServiceList.Where(m => m.ID ==id).Select(m => m.AppSecret).FirstOrDefault();
            }
        }
    }
}
