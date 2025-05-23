using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Tervisepaevik.Models;

namespace Tervisepaevik.Database
{
    public class VeejalgimineDatabase
    {
        SQLiteConnection db;

        public VeejalgimineDatabase(string dbPath)
        {
            db = new SQLiteConnection(dbPath);
            db.CreateTable<VeejalgimineClass>();
        }
        public IEnumerable<VeejalgimineClass> GetVeejalgimine()
        {
            return db.Table<VeejalgimineClass>().ToList();
        }

        public VeejalgimineClass GetVeejalgimine(int id)
        {
            return db.Get<VeejalgimineClass>(id);
        }

        public int SaveVeejalgimine(VeejalgimineClass veejalgimine)
        {
            if (veejalgimine.Veejalgimine_id != 0)
            {
                db.Update(veejalgimine);
                return veejalgimine.Veejalgimine_id;
            }
            else
            {
                return db.Insert(veejalgimine);
            }
        }

        public int DeleteVeejalgimine(int id)
        {
            return db.Delete<VeejalgimineClass>(id);
        }
    }
}
