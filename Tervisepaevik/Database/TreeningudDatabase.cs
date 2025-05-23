using System.Collections.Generic;
using SQLite;
using Tervisepaevik.Models;

namespace Tervisepaevik.Database
{
    public class TreeningudDatabase
    {
        SQLiteConnection db;

        public TreeningudDatabase(string dbPath)
        {
            db = new SQLiteConnection(dbPath);
            db.CreateTable<TreeningudClass>();
        }

        public IEnumerable<TreeningudClass> GetTreeningud()
        {
            return db.Table<TreeningudClass>().ToList();
        }

        public TreeningudClass GetTreeningud(int id)
        {
            return db.Get<TreeningudClass>(id);
        }

        public int SaveTreeningud(TreeningudClass treeningud)
        {
            if (treeningud.Treeningud_id != 0)
            {
                db.Update(treeningud);
                return treeningud.Treeningud_id;
            }
            else
            {
                return db.Insert(treeningud);
            }
        }

        public int DeleteTreeningud(int id)
        {
            return db.Delete<TreeningudClass>(id);
        }
    }
}