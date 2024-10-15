using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.Scenes.Database.Models
{
    [Table(Constants.ImageTable)]
    public class Image : Base
    {
        public string Path { get; set; }
    }
}
