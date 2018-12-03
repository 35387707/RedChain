using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL
{
    public class VersionListBLL:BaseBll
    {
        public VersionList GetNewVersion() {
            using (DBContext) {
                return DBContext.VersionList.OrderByDescending(m => m.CreateTime).FirstOrDefault();
            }
        }
        public List<VersionList> GetVersionList(int index,int pageSize,out int sum)
        {
            using (DBContext) {
                var q= DBContext.VersionList.OrderByDescending(m => m.CreateTime);
                return GetPagedList(q, pageSize, index, out sum);
            }
        }
        public int Add(VersionList model) {
            using (DBContext) {
                DBContext.VersionList.Add(model);
                return DBContext.SaveChanges();
            }
        }
        public int Update(Guid id, string internalVersion, string externalVersion, string updateLog,string DownLink) {
            using (DBContext) {
                VersionList v= DBContext.VersionList.FirstOrDefault(m=>m.ID==id);
                if (v == null)
                {
                    return -1;
                }
                else {
                    v.InternalVersion = internalVersion;
                    v.ExternalVersion = externalVersion;
                    v.UpdateLog = updateLog;
                    v.UpdateTime = DateTime.Now;
                    v.DownLink = DownLink;
                    return DBContext.SaveChanges();
                }
            }
        }
    }
}
