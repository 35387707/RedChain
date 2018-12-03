using RelexBarBLL.Models;
using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL
{
    public class ProblemBLL : BaseBll
    {
        public List<ProblemModel> GetProblem(int pageIndex, int pageSize, string key, out int sum)
        {
            using (DBContext)
            {
                var q = DBContext.Problem.Join(DBContext.Users, p => p.UID, u => u.ID, (p, u) => new ProblemModel
                {
                    PID = p.PID,
                    UID = p.UID,
                    Content = p.Content,
                    CreateTime = p.CreateTime,
                    UpdateTime = p.UpdateTime,
                    UName = u.Name,
                    Phone = u.Phone
                }).Where(m => 1 == 1);
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Content.Contains(key));
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pageSize, pageIndex, out sum);
            }

        }
        public int Add(Problem pro)
        {
            using (DBContext)
            {
                DBContext.Problem.Add(pro);
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 记录联系我们
        /// </summary>
        /// <param name="company"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <param name="descr"></param>
        /// <returns></returns>
        public int ContactUs(string company, string name, string phone, string descr)
        {
            using (DBContext)
            {
                var model = new ContactUs();
                model.Name = name;
                model.Company = company;
                model.Phone = phone;
                model.Descr = descr;
                model.Status = 1;
                model.Note = "";
                model.UpdateTime = model.CreateTime = DateTime.Now;
                DBContext.ContactUs.Add(model);
                return DBContext.SaveChanges();
            }
        }

        /// <summary>
        /// 联系我们
        /// </summary>
        /// <param name="company"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <param name="descr"></param>
        /// <returns></returns>
        public List<ContactUs> GetContactList(string key, int pagesize, int pageinex, out int count)
        {
            using (DBContext)
            {
                var q = DBContext.ContactUs.AsQueryable();
                if (!string.IsNullOrEmpty(key))
                {
                    q = q.Where(m => m.Name.Contains(key));
                }
                return GetPagedList(q.OrderByDescending(m => m.CreateTime), pagesize, pageinex, out count);
            }
        }
    }
}
