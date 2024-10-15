using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.Scenes.Database.Models
{
    [Table(Constants.ButtonTable)]
    public class Button : Base
    {
        public string Content { get; set; }
        public string Color { get; set; }
    }
}
