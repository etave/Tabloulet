using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.DatabaseNS.Models
{
    [Table(Constants.TextTable)]
    public class Text : Base
    {
        public string Content { get; set; }
        public string Font { get; set; }
        public int FontSize { get; set; }
    }
}
