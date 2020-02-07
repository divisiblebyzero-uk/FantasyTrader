using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StateMachine.entities
{
    public class User
    {
        public User(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public String Name { get; set; }
    }
}
