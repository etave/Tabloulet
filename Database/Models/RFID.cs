using System;
using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.DatabaseNS.Models
{
    [Table(Constants.RFIDTable)]
    public class RFID : IDatabaseModel
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid PageId { get; set; }
        public Guid RFIDTag { get; set; }
        public Guid LinkTo { get; set; }
        public string Name { get; set; }
    }
}
