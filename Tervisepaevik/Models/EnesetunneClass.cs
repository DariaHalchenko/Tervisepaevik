using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Tervisepaevik.Models
{
    [Table("Enesetunne")]
    public class EnesetunneClass
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Enesetunne_id { get; set; }
        public DateTime Kuupaev { get; set; }
        public int Tuju { get; set; }
        public int Energia { get; set; }
        public string TujuImageSource => $"tuju{Tuju}.PNG";
        public string EnergiaImageSource => "energia.jpg";  
    }
}



