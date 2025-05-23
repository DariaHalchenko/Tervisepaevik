using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tervisepaevik.Models;
using SQLite;

namespace Tervisepaevik.Database
{
    public class HommikusookDatabase
    {
        SQLiteConnection db;

        public HommikusookDatabase(string dbPath)
        {
            db = new SQLiteConnection(dbPath);
            db.CreateTable<HommikusookClass>();
        }

        public IEnumerable<HommikusookClass> GetHommikusook()
        {
            return db.Table<HommikusookClass>().ToList();
        }

        public HommikusookClass GetHommikusook(int id)
        {
            return db.Get<HommikusookClass>(id);
        }

        public int SaveHommikusook(HommikusookClass hommikusook)
        {
            if (hommikusook.Hommikusook_id != 0)
            {
                db.Update(hommikusook);
                return hommikusook.Hommikusook_id;
            }
            else
            {
                return db.Insert(hommikusook);
            }
        }

        public int DeleteHommikusook(int id)
        {
            return db.Delete<HommikusookClass>(id);
        }
    }
}
