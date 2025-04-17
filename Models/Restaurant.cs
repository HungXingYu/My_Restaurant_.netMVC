namespace My_Restaurant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [MetadataType(typeof(RestaurantItemAttributes))]
    [Table("Restaurant")]
    public partial class Restaurant
    {
        public int RestaurantID { get; set; }

        public string RestaurantName { get; set; }

        public string RestaurantName_EN { get; set; }

        public string Category { get; set; }

        public string Image { get; set; }

        public string Location { get; set; }

        public string Phone { get; set; }

        public string Google_map { get; set; }

        public string Rating { get; set; }

        public string Description { get; set; }
        
        public DateTime? uploadTime { get; set; }
    }
}
