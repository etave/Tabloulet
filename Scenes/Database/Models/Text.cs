using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.Scenes.Database.Models
{
    [Table(Constants.TextTable)]
    public class Text : Base
    {
        public string Content { get; set; }
        public string Font { get; set; }
        public float FontSize { get; set; }
        public string Color { get; set; }
    }
}
