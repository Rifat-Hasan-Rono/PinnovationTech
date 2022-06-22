using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinnovationTech.Models
{
    public class DataTable
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int length { get; set; }
        public List<User> users { get; set; }
    }
}
