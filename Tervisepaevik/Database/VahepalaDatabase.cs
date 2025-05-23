using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tervisepaevik.Models;
using SQLite;

namespace Tervisepaevik.Database
{
    public class VahepalaDatabase
    {
        SQLiteConnection db;

        public VahepalaDatabase(string dbPath)
        {
            db = new SQLiteConnection(dbPath);
            db.CreateTable<VahepalaClass>();
        }

        public IEnumerable<VahepalaClass> GetVahepala()
        {
            return db.Table<VahepalaClass>().ToList();
        }

        public VahepalaClass GetVahepala(int id)
        {
            return db.Get<VahepalaClass>(id);
        }

        public int SaveVahepala(VahepalaClass vahepala)
        {
            if (vahepala.Vahepala_id != 0)
            {
                db.Update(vahepala);
                return vahepala.Vahepala_id;
            }
            else
            {
                return db.Insert(vahepala);
            }
        }

        public int DeleteVahepala(int id)
        {
            return db.Delete<VahepalaClass>(id);
        }
    }
}
