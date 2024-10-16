using System;
using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.Scenes.Database.Models
{
    [Table(Constants.PageTable)]
    public class Page
    {
        [PrimaryKey]
        public Guid Id { get; set; }
    }
}
