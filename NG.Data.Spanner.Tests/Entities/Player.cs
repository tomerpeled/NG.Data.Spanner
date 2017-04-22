using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace NG.Data.Spanner.Tests.Entities
{
    public enum Status
    {
        Active = 1,
        NotActive
    }

    public class Player
    {
        public string PlayerId { get; set; }

        public string CountryCode { get; set; }
        public Country Country { get; set; } // Navigation property example

        // Enum Example
	    public Status Status { get; set; }

		public int Age { get; set; }

		public bool IsGamer { get; set; }

        //Example for saving complex object as string (very inefficient but can be used for small tables)
        public string AddressJson
        {
            get
            {

                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                var serialized = JsonConvert.SerializeObject(Address, settings);
                return serialized;
            }
            set
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                var address = JsonConvert.DeserializeObject<Address>(value, settings);
                Address = address;
            }
        }
        [NotMapped]
        public Address Address { get; set; }

	    public List<GamePlayer> GamePlayers { get; set; }

	}

	public class Address
    {
        public string Street { get; set; }
    }

    public class ComplexAddress : Address
    {
        public int Test { get; set; }
    }
}
