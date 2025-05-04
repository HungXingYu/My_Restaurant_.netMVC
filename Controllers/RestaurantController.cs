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
using Dapper; //Dapper Document: https://www.learndapper.com/dapper-query

namespace My_Restaurant.Controllers
{
    public class RestaurantController : Controller
    {
        SQLHelper sqlHelper = new SQLHelper();
        private  int intPageSize = 3;// 設定分頁每頁幾筆資料

        #region 未將餐廳類別獨立成一個Table，使用Restaurant_old資料表
        #region Index
        public ActionResult Index_old()
        {
            //設定分頁顯示頁數
            ViewBag.visiblePages = 3;
            //計算資料總筆數
            string strSQL = "SELECT COUNT(RestaurantID) FROM Restaurant_old";
            double doubleCount = Convert.ToDouble(sqlHelper.GetInt(strSQL, new SqlParameter[] { }));
            //設定分頁總頁數
            ViewBag.totalPages = Math.Ceiling(doubleCount / intPageSize);

            return View();
        }

        public ActionResult GetPage_old(int page = 1)
        {
            //計算該分頁顯示前需跳過幾筆數據
            int skipCount = (page - 1) * intPageSize;

            //OFFSET @skip ROWS FETCH FIRST @pageSize ROWS ONLY語句需SQL Server2012以上版本才可使用
            string strSQL = @"
                            SELECT * FROM Restaurant_old
                            ORDER BY uploadTime  desc
                            OFFSET @skip ROWS FETCH FIRST @pageSize ROWS ONLY";

            #region 不使用Dapper，一般ADO.NET用法
            //SqlParameter[] parameters = new SqlParameter[]
            //    {
            //    new SqlParameter("@skip", skipCount),
            //    new SqlParameter("@pageSize", intPageSize)
            //    };
            //List<Restaurant_old> result = GetRestaurant_olds(strSQL, parameters); 
            #endregion

            #region 使用Dapper
            var sqlParameters = new { skip = skipCount, pageSize = intPageSize };
            List<Restaurant_old> result = GetRestaurant_olds_Dapper(strSQL, sqlParameters);
            #endregion

            //將資料丟到RestaurantListPartial View進行局部畫面渲染並出現在Index View
            return PartialView("_RestaurantListPartial_old", result);
        }
        #endregion

        #region Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        //搜尋功能觸發時，需先直接返回第一頁搜尋結果
        public ActionResult Search_old(string keyword, int page = 1)
        {
            List<Restaurant_old> result = GetSearchReault_old(keyword, page);
            return View(result);
        }

        //由分頁AJAX觸發的第N頁查詢結果，只有RestaurantListPartial View需進行局部畫面更新
        public ActionResult GetSearchPage_old(string keyword, int page = 1)
        {
            List<Restaurant_old> result = GetSearchReault_old(keyword, page);
            return PartialView("_RestaurantListPartial_old", result);
        }

        private List<Restaurant_old> GetSearchReault_old(string keyword, int page)
        {
            #region 分頁頁數設定
            ViewBag.visiblePages = 3;
            int skipCount = (page - 1) * intPageSize;

            string strSQL = @"
                    SELECT COUNT(RestaurantID) 
                    FROM Restaurant_old 
                    WHERE RestaurantName LIKE @keyword OR RestaurantName_EN LIKE @keyword";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@keyword", $"%{keyword}%")
            };
            double doubleCount = Convert.ToDouble(sqlHelper.GetInt(strSQL, parameters));
            ViewBag.totalPages = Math.Ceiling(doubleCount / intPageSize); 
            #endregion

            strSQL = @"
                    SELECT * 
                    FROM Restaurant_old  
                    WHERE RestaurantName LIKE @keyword OR RestaurantName_EN LIKE @keyword 
                    ORDER BY uploadTime desc 
                    OFFSET @skip ROWS FETCH FIRST @pageSize ROWS ONLY";

            #region 不使用Dapper，一般ADO.NET用法
            //parameters = new SqlParameter[]
            //    {
            //    new SqlParameter("@keyword", $"%{keyword}%"),
            //    new SqlParameter("@skip", skipCount),
            //    new SqlParameter("@pageSize", intPageSize)
            //    };
            //List<Restaurant_old> result = GetRestaurant_olds(strSQL, parameters); 
            #endregion

            #region 使用Dapper
            var sqlParameters = new { keyword = $"%{keyword}%", skip = skipCount, pageSize = intPageSize };
            List<Restaurant_old> result = GetRestaurant_olds_Dapper(strSQL, sqlParameters);
            #endregion

            return result;
        }
        #endregion

        #region Detail
        public ActionResult Detail_old(int id = 1)
        {
            string strSQL = "SELECT * FROM Restaurant_old WHERE RestaurantID = @id";

            #region 不使用Dapper，一般ADO.NET用法
            //SqlParameter[] parameters = new SqlParameter[]
            //{
            //    new SqlParameter("@id", id)
            //};
            //Restaurant_old oneRestaurant = GetRestaurant_old(strSQL, parameters);
            #endregion

            #region 使用Dapper
            var sqlParameter = new { id = id };
            Restaurant_old oneRestaurant = GetRestaurant_old_Dapper(strSQL, sqlParameter);
            #endregion

            return View(oneRestaurant);
        }
        #endregion

        #region Create
        public ActionResult Create_old()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_old(Restaurant_old oneRestaurant)
        {
            if (ModelState.IsValid)//後端資料驗證
            {
                string strSQL = "SELECT * FROM Restaurant_old WHERE Location = @location";
                #region 不使用Dapper，一般ADO.NET用法
                //SqlParameter[] parameters = new SqlParameter[]
                //{
                //    new SqlParameter("@location", oneRestaurant.Location)
                //};
                //Restaurant_old oldRestaurant = GetRestaurant_old(strSQL, parameters);
                #endregion

                #region 使用Dapper
                object sqlParameter = new { location = oneRestaurant.Location };
                Restaurant_old oldRestaurant = GetRestaurant_old_Dapper(strSQL, sqlParameter);
                #endregion

                if (oldRestaurant == null || string.IsNullOrEmpty(oneRestaurant.RestaurantName))
                {
                    strSQL = @"
                            INSERT INTO Restaurant_old (RestaurantName,RestaurantName_EN,Category,Image,Location ,Phone,Google_map ,Rating ,Description,uploadTime)
                            VALUES(@RestaurantName , @RestaurantName_EN ,@Category , @Image , @Location , @Phone , @Google_map , @Rating , @Description , @uploadTime)";

                    #region 不使用Dapper，一般ADO.NET用法
                    //parameters = new SqlParameter[]
                    //{
                    //    new SqlParameter("@RestaurantName", oneRestaurant.RestaurantName),
                    //    new SqlParameter("@RestaurantName_EN", string.IsNullOrWhiteSpace(oneRestaurant.RestaurantName_EN) ? DBNull.Value : (object)oneRestaurant.RestaurantName_EN),
                    //    new SqlParameter("@Category", oneRestaurant.Category),
                    //    new SqlParameter("@Image",oneRestaurant.Image),
                    //    new SqlParameter("@Location",oneRestaurant.Location.Replace(" ", "")),
                    //    new SqlParameter("@Phone",oneRestaurant.Phone),
                    //    new SqlParameter("@Google_map",oneRestaurant.Google_map),
                    //    new SqlParameter("@Rating",oneRestaurant.Rating),
                    //    new SqlParameter("@Description",oneRestaurant.Description),
                    //    new SqlParameter("@uploadTime",DateTime.Now),
                    //};
                    //sqlHelper.CUD(strSQL, parameters);
                    #endregion

                    #region 使用Dapper
                    sqlParameter = new 
                    { 
                        RestaurantName = oneRestaurant.RestaurantName,
                        RestaurantName_EN = oneRestaurant.RestaurantName_EN,
                        Category = oneRestaurant.Category,
                        Image = oneRestaurant.Image,
                        Location = oneRestaurant.Location,
                        Phone = oneRestaurant.Phone,
                        Google_map = oneRestaurant.Google_map ,
                        Rating = oneRestaurant.Rating,
                        Description = oneRestaurant.Description,
                        uploadTime = DateTime.Now
                    };
                    sqlHelper.CUD_Dapper(strSQL, sqlParameter);
                    #endregion

                    //資料新增至資料庫後，返回Index，剛新增的資料會在第一筆顯示
                    return RedirectToAction("Index_old");
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

        private List<Restaurant_old> GetRestaurant_olds(string strSQL, SqlParameter[] parameters)
        {
            List<Restaurant_old> result = new List<Restaurant_old>();
            using (SqlConnection sqlConn = sqlHelper.GetSqlConnection())
            {
                using (SqlDataReader sqlDR = sqlHelper.GetDataReader(strSQL, parameters, sqlConn))
                {
                    while (sqlDR.Read())
                    {
                        result.Add(SetRestaurant_old(sqlDR));
                    }
                }
            }

            return result;
        }

        private Restaurant_old GetRestaurant_old(string strSQL, SqlParameter[] parameters)
        {
            Restaurant_old oneRestaurant_old = new Restaurant_old();
            using (SqlConnection sqlConn = sqlHelper.GetSqlConnection())
            {
                using (SqlDataReader sqlDR = sqlHelper.GetDataReader(strSQL, parameters, sqlConn))
                {
                    while (sqlDR.Read())
                    {
                        oneRestaurant_old = SetRestaurant_old(sqlDR);
                    }
                }
            }

            return oneRestaurant_old;
        }

        private Restaurant_old SetRestaurant_old(SqlDataReader sqlDR)
        {
            return new Restaurant_old
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

        private List<Restaurant_old> GetRestaurant_olds_Dapper(string strSQL , object sqlParameter)
        {
            using (SqlConnection sqlConn = sqlHelper.GetSqlConnection())
            {
                sqlConn.Open();
                List<Restaurant_old> result =   sqlConn.Query<Restaurant_old>(strSQL , sqlParameter).ToList();
                return result;
            }
        }

        private Restaurant_old GetRestaurant_old_Dapper(string strSQL, object sqlParameter)
        {
            using (SqlConnection sqlConn = sqlHelper.GetSqlConnection())
            {
                sqlConn.Open();
                //以下三種寫法都可以
                Restaurant_old result = sqlConn.Query<Restaurant_old>(strSQL, sqlParameter).FirstOrDefault();
                //Restaurant_old result = sqlConn.QuerySingle<Restaurant_old>(strSQL, sqlParameter);
                //Restaurant_old result = sqlConn.QueryFirstOrDefault<Restaurant_old>(strSQL, sqlParameter);
                return result;
            }
        }
        #endregion

        #region 將餐廳、餐廳類別各獨立成一個Table後Join，使用ResaturantCategoryViewModel(一對一)
        #region Index
        public ActionResult Index_viewModel()
        {
            //設定分頁顯示頁數
            ViewBag.visiblePages = 3;
            //計算資料總筆數
            string strSQL = "SELECT COUNT(RestaurantID) FROM Restaurant";
            double doubleCount = Convert.ToDouble(sqlHelper.GetInt(strSQL, new SqlParameter[] { }));
            //設定分頁總頁數
            ViewBag.totalPages = Math.Ceiling(doubleCount / intPageSize);

            return View();
        }

        public ActionResult GetPage_viewModel(int page = 1)
        {
            //計算該分頁顯示前需跳過幾筆數據
            int skipCount = (page - 1) * intPageSize;

            //OFFSET @skip ROWS FETCH FIRST @pageSize ROWS ONLY語句需SQL Server2012以上版本才可使用 
            string strSQL = @"
                        SELECT *
                        FROM Restaurant AS R
                        LEFT JOIN Category AS C 
                        ON R.CategoryID = C.CategoryID
                        ORDER BY R.uploadTime
                        OFFSET @skip ROWS FETCH FIRST @pageSize ROWS ONLY";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@skip", skipCount),
                new SqlParameter("@pageSize", intPageSize)
            };

            List<ResaturantCategoryViewModel> resultViewModel = new List<ResaturantCategoryViewModel>();
            using (SqlConnection sqlConn = sqlHelper.GetSqlConnection())
            {
                using (SqlDataReader sqlDR = sqlHelper.GetDataReader(strSQL, parameters, sqlConn))
                {
                    while (sqlDR.Read())
                    {
                        Restaurant oneRestaurant = new Restaurant
                        {
                            RestaurantID = Convert.ToInt32(sqlDR["RestaurantID"]),
                            RestaurantName = sqlDR["RestaurantName"].ToString(),
                            RestaurantName_EN = sqlDR["RestaurantName_EN"].ToString(),
                            Image = sqlDR["Image"].ToString(),
                            Location = sqlDR["Location"].ToString(),
                            Phone = sqlDR["Phone"].ToString(),
                            Google_map = sqlDR["Google_map"].ToString(),
                            Rating = sqlDR["Rating"].ToString(),
                            Description = sqlDR["Description"].ToString(),
                            uploadTime = Convert.ToDateTime(sqlDR["uploadTime"])
                        };

                        Category oneCategory = new Category
                        {
                            CategoryID = Convert.ToInt32(sqlDR["CategoryID"]),
                            CategoryName = sqlDR["CategoryName"].ToString()
                        };

                        resultViewModel.Add(new ResaturantCategoryViewModel { RestaurantVM = oneRestaurant, CategoryVM = oneCategory });
                    }
                }
            }

            return PartialView("_RestaurantListPartial_viewModel", resultViewModel);
        }
        #endregion
        #endregion

        #region 將餐廳、餐廳類別各獨立成一個Table後Join，使用Dapper
        #region Index
        public ActionResult Index()
        {
            //設定分頁顯示頁數
            ViewBag.visiblePages = 3;
            //計算資料總筆數
            string strSQL = "SELECT COUNT(RestaurantID) FROM Restaurant";
            double doubleCount = Convert.ToDouble(sqlHelper.GetInt(strSQL, new SqlParameter[] { }));
            //設定分頁總頁數
            ViewBag.totalPages = Math.Ceiling(doubleCount / intPageSize);

            return View();
        }

        public ActionResult GetPageBySort_noKeyword(string sortValue = "-uploadTime", int page = 1)
        {
            //計算該分頁顯示前需跳過幾筆數據
            int skipCount = (page - 1) * intPageSize;

            //設定order by 條件，預設為uploadTime desc
            if (sortValue.IndexOf("uploadTime") > -1)
            {
                sortValue = (sortValue.IndexOf('-') > -1) ? "R.uploadTime desc" : "R.uploadTime";
            }
            else
            {
                sortValue = (sortValue.IndexOf('-') > -1) ? "R.rating desc" : "R.rating";
            }
            string strSQL = $@"
                        SELECT *
                        FROM Restaurant AS R
                        LEFT JOIN Category AS C 
                        ON R.CategoryID = C.CategoryID
                        ORDER BY {sortValue}
                        OFFSET @skip ROWS FETCH FIRST @pageSize ROWS ONLY";
            object sqlParameters = new { skip = skipCount, pageSize = intPageSize };
            List<Restaurant> result = GetRestaurants(strSQL, sqlParameters);

            return PartialView("_RestaurantListPartial", result);
        }
        #endregion

        #region Search
        [HttpPost]
        [ValidateAntiForgeryToken]
        //搜尋功能觸發時，需先直接返回第一頁搜尋結果(預設排序為uploadTime desc)
        public ActionResult Search(string keyword, string sortValue = "-uploadTime", int page = 1)
        {
            List<Restaurant> result = GetSearchReault(keyword, sortValue , page);
            return View(result);
        }

        //由分頁AJAX觸發的第N頁查詢結果，只有RestaurantListPartial View需進行局部畫面更新
        public ActionResult GetSearchPageBySort_keyword(string keyword, string sortValue , int page)
        {
            List<Restaurant> result = GetSearchReault(keyword, sortValue, page);
            return PartialView("_RestaurantListPartial", result);
        }

        private List<Restaurant> GetSearchReault(string keyword, string sortValue, int page)
        {
            #region 分頁頁數設定
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
            double doubleCount = Convert.ToDouble(sqlHelper.GetInt(strSQL, parameters));
            ViewBag.totalPages = Math.Ceiling(doubleCount / intPageSize); 
            #endregion

            if (sortValue.IndexOf("uploadTime") > -1)
            {
                sortValue = (sortValue.IndexOf('-') > -1) ? "R.uploadTime desc" : "R.uploadTime";
            }
            else
            {
                sortValue = (sortValue.IndexOf('-') > -1) ? "R.rating desc" : "R.rating";
            }
            strSQL = $@"
                        SELECT *
                        FROM Restaurant AS R
                        LEFT JOIN Category AS C 
                        ON R.CategoryID = C.CategoryID
                        WHERE R.RestaurantName LIKE @keyword OR R.RestaurantName_EN LIKE @keyword
                        ORDER BY {sortValue}
                        OFFSET @skip ROWS FETCH FIRST @pageSize ROWS ONLY";
            object sqlParameters = new {keyword = $"%{keyword}%", skip = skipCount, pageSize = intPageSize };
            List<Restaurant> result = GetRestaurants(strSQL, sqlParameters);

            return result;
        }
        #endregion

        #region Detail
        public ActionResult Detail(int id = 1)
        {
            string strSQL = $@"
                        SELECT *
                        FROM Restaurant AS R
                        LEFT JOIN Category AS C 
                        ON R.CategoryID = C.CategoryID
                        WHERE R.RestaurantID = @id";
            object sqlParameters = new { id = id };
            Restaurant oneRestaurant = GetRestaurant(strSQL , sqlParameters);

            return View(oneRestaurant);
        }
        #endregion

        #region Create
        public ActionResult Create()
        {
            // 先寫 <option>子選項的 value，再寫 text
            ViewBag.categoryListItem = new SelectList(GetAllCategories(), "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Restaurant oneRestaurant)
        {
            ViewBag.categoryListItem = new SelectList(GetAllCategories(), "CategoryId", "CategoryName");
            if (ModelState.IsValid)
            {
                string strSQL = @"SELECT *
                        FROM Restaurant AS R
                        LEFT JOIN Category AS C 
                        ON R.CategoryID = C.CategoryID
                        WHERE Location = @location";
                object sqlParameter = new { location = oneRestaurant.Location };
                Restaurant oldRestaurant = GetRestaurant(strSQL, sqlParameter);

                if(oldRestaurant == null)
                {
                    strSQL = @"
                            INSERT INTO Restaurant (RestaurantName,RestaurantName_EN,CategoryID,Image,Location ,Phone,Google_map ,Rating ,Description,uploadTime)
                            VALUES(@RestaurantName , @RestaurantName_EN ,@CategoryID , @Image , @Location , @Phone , @Google_map , @Rating , @Description , @uploadTime)";
                    sqlParameter = new
                    {
                        RestaurantName = oneRestaurant.RestaurantName,
                        RestaurantName_EN = oneRestaurant.RestaurantName_EN,
                        CategoryID = oneRestaurant.CategoryID,
                        Image = oneRestaurant.Image,
                        Location = oneRestaurant.Location,
                        Phone = oneRestaurant.Phone,
                        Google_map = oneRestaurant.Google_map,
                        Rating = oneRestaurant.Rating,
                        Description = oneRestaurant.Description,
                        uploadTime = DateTime.Now
                    };
                    sqlHelper.CUD_Dapper(strSQL, sqlParameter);
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

        [HttpPost]
        public JsonResult CreateCategory(string categoryName)
        {
            string strSQL = @"SELECT * FROM Category WHERE CategoryName =@categoryName ";
            object sqlParameter = new { categoryName = categoryName };
            Category oldCategory = GetCategory(strSQL, sqlParameter);
            ResponseMessage response = new ResponseMessage();

            if (oldCategory != null)
            {
                response.msgTitle = "建立失敗";
                response.msgText = $"餐廳類別【{ categoryName}】已存在，請勿重複建立！";
            }
            else
            {
                strSQL = @"INSERT INTO Category (CategoryName) VALUES(@categoryName) ";
                sqlHelper.CUD_Dapper(strSQL, sqlParameter);
                response.msgTitle = "建立成功";
                response.msgText = $"餐廳類別【{ categoryName}】建立成功！";
            }

            return Json(response);
        }

        public JsonResult GetCategories()
        {
            List<Category> categories = GetAllCategories();
            return Json(categories, JsonRequestBehavior.AllowGet);
        }
        #endregion

        private List<Restaurant> GetRestaurants(string strSQL, object sqlParameters)
        {
            using (SqlConnection sqlConn = sqlHelper.GetSqlConnection())
            {
                //除Dapper官網(https://www.learndapper.com/relationships)以外另外參考：
                //object sqlParameters放置位置參考：https://www.cnblogs.com/harrychinese/p/dapper_multi_table_mapping.html
                //Dapper查詢參考：https://dotblogs.com.tw/supershowwei/2017/07/11/222837
                //此為一對一查詢，一筆餐廳資料對應一筆餐廳類別資料
                return sqlConn.Query<Restaurant, Category, Restaurant>(
                    strSQL,
                    (objRestaurant, objCategory) => {
                        objRestaurant.Category = objCategory; //直接將傳入的objCategory賦值給objRestaurant.Category，也就是藉由Restaurant Table類別檔及Category Table類別檔的導覽屬性將兩者進行1對1的綁定
                        return objRestaurant;
                    },
                    sqlParameters,
                    splitOn: "CategoryID").ToList();
            }
        }

        private Restaurant GetRestaurant(string strSQL, object sqlParameters)
        {
            using (SqlConnection sqlConn = sqlHelper.GetSqlConnection())
            {
                //除Dapper官網(https://www.learndapper.com/relationships)以外另外參考：
                //object sqlParameters放置位置參考：https://www.cnblogs.com/harrychinese/p/dapper_multi_table_mapping.html
                //Dapper查詢參考：https://dotblogs.com.tw/supershowwei/2017/07/11/222837
                //此為一對一查詢，一筆餐廳資料對應一筆餐廳類別資料
                return sqlConn.Query<Restaurant, Category, Restaurant>(
                    strSQL,
                    (objRestaurant, objCategory) => {
                        objRestaurant.Category = objCategory; //直接將傳入的objCategory賦值給objRestaurant.Category，也就是藉由Restaurant Table類別檔及Category Table類別檔的導覽屬性將兩者進行1對1的綁定
                        return objRestaurant;
                    },
                    sqlParameters,
                    splitOn: "CategoryID").FirstOrDefault();
            }
        }

        private List<Category> GetAllCategories()
        {
            string strSQL = @"SELECT * FROM Category";
            using (SqlConnection sqlConn = sqlHelper.GetSqlConnection())
            {
                sqlConn.Open();
                List<Category> result = sqlConn.Query<Category>(strSQL).ToList();

                return result;
            }
        }

        private Category GetCategory(string strSQL, object sqlParameters)
        {
            using (SqlConnection sqlConn = sqlHelper.GetSqlConnection())
            {
                sqlConn.Open();
                Category result = sqlConn.Query<Category>(strSQL , sqlParameters).FirstOrDefault();

                return result;
            }
        }
        #endregion





    }
}