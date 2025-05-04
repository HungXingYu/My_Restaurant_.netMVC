using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace My_Restaurant.Models
{
    //一對一，一個餐廳對應一個餐廳類別
    public class ResaturantCategoryViewModel
    {
        public Category CategoryVM { get; set; }

        public Restaurant RestaurantVM  { get; set; }
    }
}