using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tervisepaevik.Models;
using SQLite;

namespace Tervisepaevik.Database
{
    public class OhtusookDatabase
    {
        SQLiteConnection db;

        public OhtusookDatabase(string dbPath)
        {
            db = new SQLiteConnection(dbPath);
            db.CreateTable<OhtusookClass>();
        }

        public IEnumerable<OhtusookClass> GetOhtusook()
        {
            return db.Table<OhtusookClass>().ToList();
        }

        public OhtusookClass GetOhtusook(int id)
        {
            return db.Get<OhtusookClass>(id);
        }

        public int SaveOhtusook(OhtusookClass ohtusook)
        {
            if (ohtusook.Ohtusook_id != 0)
            {
                db.Update(ohtusook);
                return ohtusook.Ohtusook_id;
            }
            else
            {
                return db.Insert(ohtusook);
            }
        }

        public int DeleteOhtusook(int id)
        {
            return db.Delete<OhtusookClass>(id);
        }
    }
}
