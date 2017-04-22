using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NG.Data.Spanner.Tests.Entities
{
    public class Country
    {
        [Key]
        public string CountryCode { get; set; }

		public string Name { get; set; }

		[NotMapped]
        public long? PropertyNoMapped { get; set; }
    }
}
