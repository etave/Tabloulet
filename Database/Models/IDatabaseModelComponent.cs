using System;

namespace Tabloulet.DatabaseNS.Models
{
    public interface IDatabaseModelComponent
    {
        Guid PageId { get; set; }

        Guid Id { get; set; }
    }
}
