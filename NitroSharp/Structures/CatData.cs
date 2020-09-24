using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

using Newtonsoft.Json;

namespace NitroSharp.Structures
{
    public struct CatData
    {
        [JsonProperty("breeds")]
        public CatBreed[] Breeds { get; private set; }

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("url")]
        public string Url { get; private set; }

        [JsonProperty("width")]
        public int Width { get; private set; }

        [JsonProperty("height")]
        public int Height { get; private set; }
    }
    
    public struct CatBreed
    {
        [JsonProperty("weight")]
        public CatWeight Weight { get; private set; }

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("temperament")]
        public string Temperament { get; private set; }

        [JsonProperty("origin")]
        public string Origin { get; private set; }

        [JsonProperty("country_codes")]
        public string CountryCodes { get; private set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; private set; }

        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("life_span")]
        public string LifeSpan { get; private set; }

        [JsonProperty("indoor")]
        public bool Indoor { get; private set; }

        [JsonProperty("lap")]
        public bool lap { get; private set; }

        [JsonProperty("alt_names")]
        public string AltNames { get; private set; }

        [JsonProperty("adaptability")]
        public int Adaptability { get; private set; }

        [JsonProperty("affection_level")]
        public int AffectionLevel { get; private set; }

        [JsonProperty("child_friendly")]
        public int ChildFriendly { get; private set; }

        [JsonProperty("dog_friendly")]
        public int DogFriendly { get; private set; }

        [JsonProperty("energy_level")]
        public int EnergyLevel { get; private set; }

        [JsonProperty("grooming")]
        public int Grooming { get; private set; }

        [JsonProperty("health_issues")]
        public int HealthIssues { get; private set; }

        [JsonProperty("intelligence")]
        public int Intelligence { get; private set; }

        [JsonProperty("shedding_level")]
        public int SheddingLevel { get; private set; }

        [JsonProperty("social_needs")]
        public int SocialNeeds { get; private set; }

        [JsonProperty("stranger_friendly")]
        public int StrangerFriendly { get; private set; }

        [JsonProperty("vocalisation")]
        public int Vocalisation { get; private set; }

        [JsonProperty("experimental")]
        public bool Experimental { get; private set; }

        [JsonProperty("hairless")]
        public bool Hairless { get; private set; }

        [JsonProperty("natural")]
        public bool Natural { get; private set; }

        [JsonProperty("rare")]
        public bool Rare { get; private set; }

        [JsonProperty("rex")]
        public bool Rex { get; private set; }

        [JsonProperty("suppressed_tail")]
        public bool SuppressedTail { get; private set; }

        [JsonProperty("short_legs")]
        public bool ShortLegs { get; private set; }

        [JsonProperty("wikipedia_url")]
        public string WikipediaUrl { get; private set; }

        [JsonProperty("hypoallergenic")]
        public bool Hypoallergenic { get; private set; }
    }

    public struct CatWeight
    {
        [JsonProperty("imperial")]
        public string Imperial { get; private set; }
        [JsonProperty("metric")]
        public string Metric { get; private set; }
    }
}
