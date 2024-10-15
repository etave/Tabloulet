using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.Scenes.Database.Models
{
    [Table(Constants.ModelTable)]
    public class Model : Base
    {
        public string Path { get; set; }
    }
}
