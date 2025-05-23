using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Tervisepaevik.Models
{
    [Table("Toidukorrad")]
    public class ToidukorradClass
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Toidukorrad_id { get; set; }
        public string Tuup { get; set; } // Hommikusook, Ohtusook, Vahepala, Louna
        public string Roa_nimi { get; set; }
        public DateTime Kuupaev { get; set; }
        public int Kalorid { get; set; }
        public byte[]? Toidu_foto { get; set; }
        public ImageSource FotoSource =>
        Toidu_foto != null ? ImageSource.FromStream(() => new MemoryStream(Toidu_foto)) : null;
    }
}
