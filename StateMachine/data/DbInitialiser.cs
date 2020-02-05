using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace StateMachine.data
{
    public class DbInitialiser
    {
        private readonly StateMachineDataContext _context;
        private readonly ILogger<DbInitialiser> _logger;

        public DbInitialiser(StateMachineDataContext context, ILogger<DbInitialiser> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Initialize()
        {
            _context.Database.EnsureCreated();


        }
    }
}
