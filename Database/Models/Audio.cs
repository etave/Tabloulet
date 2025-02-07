﻿using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.DatabaseNS.Models
{
    [Table(Constants.AudioTable)]
    public class Audio : Base
    {
        public string Path { get; set; }
    }
}
