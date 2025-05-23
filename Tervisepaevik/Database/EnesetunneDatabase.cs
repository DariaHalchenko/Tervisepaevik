using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Tervisepaevik.Models;

namespace Tervisepaevik.Database
{
    public class EnesetunneDatabase
    {
        SQLiteConnection db;

        public EnesetunneDatabase(string dbPath)
        {
            db = new SQLiteConnection(dbPath);
            db.CreateTable<EnesetunneClass>();
        }

        public IEnumerable<EnesetunneClass> GetEnesetunne()
        {
            return db.Table<EnesetunneClass>().ToList();
        }

        public EnesetunneClass GetEnesetunne(int id)
        {
            return db.Get<EnesetunneClass>(id);
        }

        public int SaveEnesetunne(EnesetunneClass enesetunne)
        {
            if (enesetunne.Enesetunne_id != 0)
            {
                db.Update(enesetunne);
                return enesetunne.Enesetunne_id;
            }
            else
            {
                return db.Insert(enesetunne);
            }
        }
        public int DeleteEnesetunne(int id)
        {
            return db.Delete<EnesetunneClass>(id);
        }
    }
}
