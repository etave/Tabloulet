using System;
using SQLite;

namespace Tabloulet.DatabaseNS.Models
{
    public class Base : IDatabaseModel, IDatabaseModelComponent
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid PageId { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float SizeX { get; set; }
        public float SizeY { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float Rotation { get; set; }
        public int ZIndex { get; set; }
        public bool IsMovable { get; set; }
    }
}
