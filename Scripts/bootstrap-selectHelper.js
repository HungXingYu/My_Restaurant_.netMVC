//*排序
const sort = document.querySelector("#sort")
//*下拉選單
const dropdown = document.querySelector(".selectpicker")


//設定預設選項
/*bootstrap-select元件存在時，進行此設定*/
if (dropdown) {
    const dropdownValue = dropdown.dataset.value
    $(".selectpicker").selectpicker("val", dropdownValue)
}

/*排序功能的bootstrap-select元件存在時，進行以下流程*/
if (sort) {
    //#region 選擇排序
    function setSortChange(totalPages, visiblePages, url) {
        $("select").on("change", async function (e) {
            const selectSortValue = this.options[this.selectedIndex].value //取得選擇的排序類型
            dropdown.dataset.value = selectSortValue
            const pathname = window.location.pathname;
            let data 
            let result 

            $('#pagination').twbsPagination('destroy');
            if (pathname === "/Restaurant/Index" || pathname === "/") {
                setSortPagination_NoKeyword(totalPages, visiblePages, url)
                data = { sortValue: selectSortValue, page: 1 }
            } else if (pathname === "/Restaurant/Search") {
                setSortPagination_Keyword(totalPages, visiblePages, url)
                data = { keyword: $('#keyword').val(), sortValue: selectSortValue, page: 1 }
            }
            result = await ajaxGetPage(url, data)
            $('#restaurantList').html(result);
        })
    }
    //#endregion
}



    
