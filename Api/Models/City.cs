using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Api.Models
{
    public class City
    {
        public int Id { get; set; }

        public string name { get; set; }

        public int StateId { get; set; }
        
        public State State { get; set; }
    }
}
