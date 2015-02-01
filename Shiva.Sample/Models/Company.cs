using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiva.Sample.Models
{
    public class Company
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public List<Person> Employees { get; private set; }

        public Company()
        {
            Employees = new List<Person>();
        }
    }
}
