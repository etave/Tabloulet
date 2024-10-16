using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.Database.Models
{
    [Table(Constants.ImageTable)]
    public class Image : Base
    {
        public string Path { get; set; }
    }
}
