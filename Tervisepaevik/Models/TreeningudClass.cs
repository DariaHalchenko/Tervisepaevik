using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Tervisepaevik.Models
{
    [Table("Treeningud")]
    public class TreeningudClass
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Treeningud_id { get; set; }
        public string Treeningu_nimi { get; set; }
        public TimeSpan Kallaaeg { get; set; }
        public string Treeningu_tuup { get; set; }
        public string Kirjeldus { get; set; }
        public string Link { get; set; }
        public int Kulutud_kalorid { get; set; }
        public byte[]? Treeningu_foto { get; set; }
    }
}
