using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace My_Restaurant.Models
{
    //多對多(一對多的列表)，多個「一個餐廳類別對應多個餐廳」
    //這個要在前端View寫複雜程式，個人感覺不好用也想不到用法
    public class RCmultiViewModel
    {
        public List<Category> CVM { get; set; }

        public List<Restaurant_old> RVM { get; set; }
    }
}