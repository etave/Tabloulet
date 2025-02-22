﻿using SQLite;
using Tabloulet.Helpers;

namespace Tabloulet.DatabaseNS.Models
{
    [Table(Constants.VideoTable)]
    public class Video : Base
    {
        public string Path { get; set; }
        public bool Autoplay { get; set; }
        public bool Loop { get; set; }
    }
}
