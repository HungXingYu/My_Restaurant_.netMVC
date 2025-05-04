function setNoKeywordPagination(totalPages, visiblePages, url ) {
    $('#pagination').twbsPagination({
        totalPages: totalPages,
         visiblePages: visiblePages,
        hideOnlyOnePage: true,
        initiateStartPageClick:false,
        onPageClick: async function (event, pageNum) {
            let result = await ajaxGetPage(url, { page: pageNum});
            $('#restaurantList').html(result);
        }
    });
}

function setKeywordPagination(totalPages, visiblePages, url) {
    $('#pagination').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        onPageClick: async function (event, pageNum) {
            let data= { keyword: $('#keyword').val(), page: pageNum }
            let result = await ajaxGetPage(url, data);
            $('#restaurantList').html(result);
        }
    });
}

function setSortPagination_NoKeyword(totalPages, visiblePages, url) {
    $('#pagination').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        startPage:1,
        onPageClick: async function (event, pageNum) {
            let data = { sortValue: sort.dataset.value, page: pageNum }
            let result = await ajaxGetPage(url, data);
            $('#restaurantList').html(result);
        }
    });
}

function setSortPagination_Keyword(totalPages, visiblePages, url) {
    $('#pagination').twbsPagination({
        totalPages: totalPages,
        visiblePages: visiblePages,
        hideOnlyOnePage: true,
        initiateStartPageClick: false,
        startPage: 1,
        onPageClick: async function (event, pageNum) {
            let data = { keyword: $('#keyword').val(), sortValue: sort.dataset.value, page: pageNum }
            let result = await ajaxGetPage(url, data);
            $('#restaurantList').html(result);
        }
    });
}


