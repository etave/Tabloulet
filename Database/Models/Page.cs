using System;
using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.DatabaseNS.Models
{
    [Table(Constants.PageTable)]
    public class Page : IDatabaseModel
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BackgroundColor { get; set; }
    }
}
