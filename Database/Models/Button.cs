using System;
using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.DatabaseNS.Models
{
    [Table(Constants.ButtonTable)]
    public class Button : Base
    {
        public Guid? LinkTo { get; set; }
        public string Content { get; set; }
        public string Color { get; set; }
    }
}
