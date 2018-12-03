using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL
{
    public class SysPriceBLL:BaseBll
    {
        public List<SysPrice> GetSysPrice(int pageIndex, int pageSize, string key, DateTime? mindate, DateTime? maxdate, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.SysPrice.Where(m => 1 == 1);
                if (mindate != null && maxdate != null)
                {
                    q = q.Where(m => m.CreateTime >= mindate && m.CreateTime <= maxdate);
                }


                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Remark.Contains(key));
                }

                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }

        }

        public decimal[] GetTotalPrice(DateTime? mindate,DateTime? maxdate) {
            decimal[] d = new decimal[2];
            using (DBContext) {
                if (mindate == null&&maxdate==null)
                {
                    decimal? temp = DBContext.SysPrice.Where(m => m.InOut == 1).Sum(m => (decimal?)m.Money);
                    d[0] = temp == null ? 0 : temp.Value;
                    temp = DBContext.SysPrice.Where(m => m.InOut == 0).Sum(m => (decimal?)m.Money);
                    d[1] = temp == null ? 0 : temp.Value;
                }
                else {
                    if (maxdate == null)
                    {
                        maxdate = DateTime.Now;

                    }
                    else {
                        maxdate = maxdate.Value.AddDays(1);
                    }
                    decimal? temp = DBContext.SysPrice.Where(m => m.CreateTime >= mindate && m.CreateTime <= maxdate && m.InOut == 1).Sum(m => (decimal?)m.Money);
                    d[0] = temp == null ? 0 : temp.Value;
                    temp = DBContext.SysPrice.Where(m => m.InOut == 0).Sum(m => (decimal?)m.Money);
                    d[1] = temp == null ? 0 : temp.Value;
                }
                return d;
            }
        }
    }
}
