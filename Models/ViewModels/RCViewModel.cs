using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace My_Restaurant.Models
{
    //一對多，一個餐廳類別對應多個餐廳
    //應該是以餐廳類別進行排序時使用，待測試
    public class RCViewModel
    {
        public Category CVM { get; set; }

        public IList<Restaurant_old> RVM { get; set; }
    }
} 