using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SampleDeliveryService.Models
{
    public class Order
    {
        [Display(Name = "Order Number")]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Display(Name = "First Name")]
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [Display(Name = "Number of packages")]
        [Range(1, 9999)]
        [JsonProperty(PropertyName = "packages")]
        public int Packages { get; set; }

        [JsonProperty(PropertyName = "street")]
        public string Street { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [Display(Name = "Zip Code")]
        [JsonProperty(PropertyName = "zipCode")]
        public int ZipCode { get; set; }

        [JsonProperty(PropertyName = "latitude")]
        public string Latitude { get; set; }

        [JsonProperty(PropertyName = "longitude")]
        public string Longitude { get; set; }

        [JsonProperty(PropertyName = "isComplete")]
        public bool Completed { get; set; }
    }
}
