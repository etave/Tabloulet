using System;
using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.DatabaseNS.Models
{
    [Table(Constants.ScenarioTable)]
    public class Scenario : IDatabaseModel
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid PageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
