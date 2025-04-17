using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using My_Restaurant.Models;
using My_Restaurant.Models.Repo;
using System.Web.UI;

namespace My_Restaurant.Controllers
{
    public class RestaurantController : Controller
    {
        SQLHelper sqlHelper = new SQLHelper();
        private  int intPageSize = 3;// 設定分頁每頁幾筆資料

        // GET: Restaurant
        #region Index
        public ActionResult Index()
        {
            //設定分頁顯示頁數
            ViewBag.visiblePages = 3;
            //計算資料總筆數
            string strSQL = "SELECT COUNT(RestaurantID) FROM Restaurant";
            double doubleCount = Convert.ToDouble(sqlHelper.GetInt(strSQL , new SqlParameter[] { }));
            //設定分頁總頁數
            ViewBag.totalPages = Math.Ceiling(doubleCount / intPageSize);

            return View();
        }


        public ActionResult GetPage(int page = 1)
        {
            //計算該分頁顯示前需跳過幾筆數據
            int skipCount = (page - 1) * intPageSize;
            //OFFSET @skip ROWS FETCH FIRST @pageSize ROWS ONLY語句需SQL Server2012以上版本才可使用
            string strSQL = @"
                            SELECT * FROM Restaurant 
                            ORDER BY uploadTime  desc
                            OFFSET @skip ROWS FETCH FIRST @pageSize ROWS ONLY";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@skip", skipCount),
                new SqlParameter("@pageSize", intPageSize)
            };
            List<Restaurant> result = GetRestaurants(strSQL , parameters);
            //將資料丟到RestaurantListPartial View進行局部畫面渲染並出現在Index View
            return PartialView("_RestaurantListPartial", result);
        }
        #endregion

        #region Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        //搜尋功能觸發時，需先直接返回第一頁搜尋結果
        public ActionResult Search(string keyword, int page = 1)
        {
            List<Restaurant> result = GetSearchReault(keyword, page);
            return View(result);
        }

        //由分頁AJAX觸發的第N頁查詢結果，只有RestaurantListPartial View需進行局部畫面更新
        public ActionResult GetSearchPage(string keyword, int page = 1)
        {
            List<Restaurant> result = GetSearchReault(keyword, page);
            return PartialView("_RestaurantListPartial", result);
        }

        private List<Restaurant> GetSearchReault(string keyword, int page)
        {
            ViewBag.visiblePages = 3;
            int skipCount = (page - 1) * intPageSize;

            string strSQL = @"
                    SELECT COUNT(RestaurantID) 
                    FROM Restaurant 
                    WHERE RestaurantName LIKE @keyword OR RestaurantName_EN LIKE @keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@keyword", $"%{keyword}%")
            };
            double doubleCount = Convert.ToDouble(sqlHelper.GetInt(strSQL  , parameters));
            ViewBag.totalPages = Math.Ceiling(doubleCount / intPageSize);

            strSQL = @"
                    SELECT * 
                    FROM Restaurant  
                    WHERE RestaurantName LIKE @keyword OR RestaurantName_EN LIKE @keyword 
                    ORDER BY uploadTime desc 
                    OFFSET @skip ROWS FETCH FIRST @pageSize ROWS ONLY";
            parameters = new SqlParameter[]
            {
                new SqlParameter("@keyword", $"%{keyword}%"),
                new SqlParameter("@skip", skipCount),
                new SqlParameter("@pageSize", intPageSize)
            };
            List<Restaurant> result = GetRestaurants(strSQL , parameters);
            return result;
        }
        #endregion

        #region Detail
        public ActionResult Detail(int id = 1)
        {
            string strSQL = "SELECT * FROM Restaurant WHERE RestaurantID = @id";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };
            Restaurant oneRestaurant = GetRestaurant(strSQL, parameters);

            return View(oneRestaurant);
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Restaurant oneRestaurant)
        {
            if (ModelState.IsValid)//後端資料驗證
            {
                string strSQL = "SELECT * FROM Restaurant WHERE Location = @location";
                SqlParameter[] parameters = new SqlParameter[]
                {
                new SqlParameter("@location", oneRestaurant.Location)
                };
                Restaurant oldRestaurant = GetRestaurant(strSQL, parameters);

                if (string.IsNullOrEmpty(oldRestaurant.RestaurantName))
                {
                    strSQL = @"
                            INSERT INTO Restaurant (RestaurantName,RestaurantName_EN,Category,Image,Location ,Phone,Google_map ,Rating ,Description,uploadTime)
                            VALUES(@RestaurantName , @RestaurantName_EN ,@Category , @Image , @Location , @Phone , @Google_map , @Rating , @Description , @uploadTime)";
                    parameters = new SqlParameter[]
                    {
                        new SqlParameter("@RestaurantName", oneRestaurant.RestaurantName),
                        new SqlParameter("@RestaurantName_EN", string.IsNullOrWhiteSpace(oneRestaurant.RestaurantName_EN) ? DBNull.Value : (object)oneRestaurant.RestaurantName_EN),
                        new SqlParameter("@Category", oneRestaurant.Category),
                        new SqlParameter("@Image",oneRestaurant.Image),
                        new SqlParameter("@Location",oneRestaurant.Location.Replace(" ", "")),
                        new SqlParameter("@Phone",oneRestaurant.Phone),
                        new SqlParameter("@Google_map",oneRestaurant.Google_map),
                        new SqlParameter("@Rating",oneRestaurant.Rating),
                        new SqlParameter("@Description",oneRestaurant.Description),
                        new SqlParameter("@uploadTime",DateTime.Now),
                    };
                    sqlHelper.CUD(strSQL, parameters);
                    //資料新增至資料庫後，返回Index，剛新增的資料會在第一筆顯示
                    return RedirectToAction("Index");
                }
                else
                {
                    //餐廳資料重複處理：設定ViewBag傳值至前端再搭配JS呼叫SweetAlert2 
                    ViewBag.callErrorModal = true;
                    ViewBag.errorTitle = "資料重複";
                    ViewBag.errorMessage = $"該地址已存在餐廳 :【 {oldRestaurant.RestaurantName}】，請重新確認您輸入的資料";
                    return View(oneRestaurant); 
                }
            }
            else
            {
                //後端資料驗證有誤，設定ViewBag傳值至前端再搭配JS呼叫SweetAlert2 
                ViewBag.callErrorModal = true;
                ViewBag.errorTitle = "驗證錯誤";
                return View(oneRestaurant);
            }
        }
        #endregion

        private List<Restaurant> GetRestaurants(string strSQL , SqlParameter[] parameters)
        {
            List<Restaurant> result = new List<Restaurant>();
            using (SqlConnection sqlConn = sqlHelper.GetSqlConnection())
            {
                using (SqlDataReader sqlDR = sqlHelper.GetDataReader(strSQL, parameters, sqlConn))
                {
                    while (sqlDR.Read())
                    {
                        result.Add(SetRestaurant(sqlDR));
                    }
                }
            }

            return result;
        }

        private Restaurant GetRestaurant(string strSQL, SqlParameter[] parameters)
        {
            Restaurant oneRestaurant = new Restaurant();
            using (SqlConnection sqlConn = sqlHelper.GetSqlConnection())
            {
                using (SqlDataReader sqlDR = sqlHelper.GetDataReader(strSQL, parameters, sqlConn))
                {
                    while (sqlDR.Read())
                    {
                        oneRestaurant = SetRestaurant(sqlDR);
                    }
                }
            }

            return oneRestaurant;
        }

        private Restaurant SetRestaurant(SqlDataReader sqlDR)
        {
            return new Restaurant
            {
                RestaurantID = Convert.ToInt32(sqlDR["RestaurantID"]),
                RestaurantName = sqlDR["RestaurantName"].ToString(),
                RestaurantName_EN = sqlDR["RestaurantName_EN"].ToString(),
                Category = sqlDR["Category"].ToString(),
                Image = sqlDR["Image"].ToString(),
                Location = sqlDR["Location"].ToString(),
                Phone = sqlDR["Phone"].ToString(),
                Google_map = sqlDR["Google_map"].ToString(),
                Rating = sqlDR["Rating"].ToString(),
                Description = sqlDR["Description"].ToString(),
                uploadTime = Convert.ToDateTime(sqlDR["uploadTime"])
            };
        }

    }
}