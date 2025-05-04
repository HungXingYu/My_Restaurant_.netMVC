//#region ajax GetPage 方法 -- Promise
function ajaxGetPage(url, data) {
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "Get",
            url: url,
            data: data,
            success: function (response) {
                resolve(response)
            },
            error: function (xhr) {
                reject(xhr)
            }
        })
    })
}
//#endregion

//#region ajax POST方法 -- Promise
function ajaxPost(url, data) {
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: url,
            data: data,
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                resolve(response)
            },
            error: function (xhr) {
                reject(xhr)
            }
        })
    })
}
//#endregion

