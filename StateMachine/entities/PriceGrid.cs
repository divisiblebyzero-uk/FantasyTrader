using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachine.entities
{
    public class PriceGrid
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User Owner { get; set; }
        public ICollection<PriceGridEntry> PriceGridEntries { get; set; }
    }

    public class PriceGridEntry
    {
        public int Id { get; set; }
        public PriceGrid PriceGrid { get; set; }
        public string Symbol { get; set; }
    }
}
