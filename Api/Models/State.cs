using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Api.Models
{
    public class State
    {
        public int Id { get; set; }

        public string name { get; set; }

        public int CountryId { get; set; }

        public Country Country { get; set; }
       // public ICollection<City> States { get; set; }
    }
}
