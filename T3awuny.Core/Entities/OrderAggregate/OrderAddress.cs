using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.OrderAggregate
{
    public class OrderAddress
    {
        public string Name { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public OrderAddress()
        {
        }
        public OrderAddress(string name, string street, string city, string government, string country)
        {
            Name = name;
            Street = street;
            City = city;
            Country = country;
            Governorate = government;
        }
    }
}
