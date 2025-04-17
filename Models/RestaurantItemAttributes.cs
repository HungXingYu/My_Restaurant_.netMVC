using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace My_Restaurant.Models
{
    public class RestaurantItemAttributes
    {
        [Key]
        public int RestaurantID { get; set; }

        [StringLength(50)]
        [Display(Name = "餐廳中文名")]
        [Required(ErrorMessage = "餐廳中文名為必填欄位！")]
        public string RestaurantName { get; set; }

        [StringLength(50)]
        [Display(Name = "餐廳英文名")]
        [RegularExpression(@"^[A-Za-z\s]*$", ErrorMessage = "只能輸入大小寫英文！")]
        public string RestaurantName_EN { get; set; }

        [StringLength(20)]
        [Display(Name = "餐廳類別")]
        [Required(ErrorMessage = "餐廳類別為必填欄位！")]
        public string Category { get; set; }

        [Display(Name = "餐廳照片")]
        [Required(ErrorMessage = "餐廳照片網址為必填欄位！")]
        [RegularExpression(@"^(https?|ftp)://[^\s/$.?#].[^\s]*$", ErrorMessage = "請輸入正確的網址格式！")]
        public string Image { get; set; }

        [Display(Name = "餐廳地址")]
        [Required(ErrorMessage = "餐廳地址為必填欄位！")]
        public string Location { get; set; }

        [StringLength(40)]
        [Display(Name = "餐廳電話")]
        [Required(ErrorMessage = "餐廳電話為必填欄位！")]
        [RegularExpression(@"^(\(0\d{1,3}\)\d{5,8}|09\d{8})$", ErrorMessage = "請輸入正確的電話格式！")]
        public string Phone { get; set; }

        [Display(Name = "餐廳Google Map")]
        [Required(ErrorMessage = "餐廳Google Map網址為必填欄位！")]
        [RegularExpression(@"^(https?|ftp)://[^\s/$.?#].[^\s]*$", ErrorMessage = "請輸入正確的網址格式！")]
        public string Google_map { get; set; }

        [StringLength(10)]
        [Display(Name = "餐廳評分")]
        [Required(ErrorMessage = "餐廳評分為必填欄位！")]
        [Range(1.0, 5.0, ErrorMessage = "請輸入 1 至 5 的數字，可包含兩位小數！")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "最多只能輸入兩位小數！")]
        public string Rating { get; set; }

        [Display(Name = "餐廳簡介")]
        [Required(ErrorMessage = "餐廳簡介為必填欄位！")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "上傳時間")]
        public DateTime? uploadTime { get; set; }
    }

}