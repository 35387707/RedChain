using RelexBarDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelexBarBLL
{
    public class AlbumListBLL:BaseBll
    {
        public int Add(Guid uid,string name,string desc,Guid? AlbumListID=null) {
            using (DBContext) {
                AlbumList a = new AlbumList();
                a.ID =AlbumListID==null?Guid.NewGuid():AlbumListID.Value;
                a.Name = name;
                a.Description = desc;
                a.UID = uid;
                a.CreateTime = a.UpdateTime = DateTime.Now;
                DBContext.AlbumList.Add(a);
                return DBContext.SaveChanges();
            }
        }
        /// <summary>
        /// 添加照片
        /// </summary>
        /// <param name="AlbumListID"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public int AddPhoto(Guid uid,Guid AlbumListID,string path) {
            using (DBContext) {
                int a = DBContext.AlbumList.Count(m=>m.ID==AlbumListID&&m.UID==uid);
                if (a==0) {
                    return (int)ErrorCode.相册不存在;
                }
                Guid photoID = Guid.NewGuid();
                //查询相册照片数量设置封面
                int i = DBContext.PhotoPath.Where(m => m.AlbumListID == AlbumListID).Count();
                if (i==0) {
                    AlbumList album=DBContext.AlbumList.FirstOrDefault(m=>m.ID==AlbumListID);
                    album.Cover = photoID;
                }
                PhotoPath p = new PhotoPath();
                p.ID = photoID;
                p.AlbumListID = AlbumListID;
                p.Path = path;
                p.CreateTime = p.UpdateTime = DateTime.Now;
                DBContext.PhotoPath.Add(p);
                return DBContext.SaveChanges();
            }
        }
    }
}
