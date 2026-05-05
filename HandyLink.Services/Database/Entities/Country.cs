using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandyLink.Services.Database.Entities
{
    public class Country : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<City> Cities { get; set; } = new List<City>();
    }
}
