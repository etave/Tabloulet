using System;
using SQLite;

namespace Tabloulet.Database.Models
{
    public class Base : IDatabaseModel
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid PageId { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float Rotation { get; set; }
        public bool IsMovable { get; set; }
    }
}
