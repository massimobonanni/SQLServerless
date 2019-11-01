using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SQLServerless.Functions.DTO
{
    public class ContactDTO
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("birthDate")]
        public DateTime? BirthDate { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }

        public override string ToString()
        {
            return $"{nameof(FirstName)}={FirstName}; {nameof(LastName)}={LastName}; {nameof(Email)}={Email}; {nameof(BirthDate)}={BirthDate}; {nameof(Height)}={Height};";
        }

    }
}
