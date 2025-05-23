using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tervisepaevik.Models;
using SQLite;

namespace Tervisepaevik.Database
{
    public class LounasookDatabase
    {
        SQLiteConnection db;

        public LounasookDatabase(string dbPath)
        {
            db = new SQLiteConnection(dbPath);
            db.CreateTable<LounasookClass>();
        }

        public IEnumerable<LounasookClass> GetLounasook()
        {
            return db.Table<LounasookClass>().ToList();
        }

        public LounasookClass GetLounasook(int id)
        {
            return db.Get<LounasookClass>(id);
        }

        public int SaveLounasook(LounasookClass lounasook)
        {
            if (lounasook.Lounasook_id != 0)
            {
                db.Update(lounasook);
                return lounasook.Lounasook_id;
            }
            else
            {
                return db.Insert(lounasook);
            }
        }

        public int DeleteLounasook(int id)
        {
            return db.Delete<LounasookClass>(id);
        }
    }
}