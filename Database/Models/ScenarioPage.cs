using System;
using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.DatabaseNS.Models
{
    [Table(Constants.ScenarioPageTable)]
    public class ScenarioPage : IDatabaseModel
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid PageId { get; set; }
        public Guid ScenarioId { get; set; }
    }
}
