//*搜尋
const searchBar = document.querySelector("#searchBar")
const restaurantList = document.querySelector("#restaurantList .col")
const noData = document.querySelector("#noData")
//*新增
const uploadBtn = document.querySelector("#uploadBtn")
//*資料驗證
const dataError = document.querySelectorAll(`[data-error$='！']`) //get element where data-error ends with '！'

function callSuccessMsg(msgTitle, msgHtml) {
    return Swal.fire({
        title: msgTitle,
        html: msgHtml,
        icon: "success",
        confirmButtonColor: "#3085d6",
        confirmButtonText: "OK"
    })
}

function callErrorMsg(msgTitle, msgHtml) {
    return Swal.fire({
        title: msgTitle,
        html: msgHtml,
        icon: "error",
        confirmButtonColor: "#3085d6",
        confirmButtonText: "OK"
    })
}

function callYesNoMsg(msgTitle) {
    return Swal.fire({
        title: msgTitle,
        icon: "question",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        confirmButtonText: "Yes"
    })
}


//#region - 搜尋餐廳查無資料
if ( noData && searchBar && !restaurantList) {
    if(noData.style.display === ""){
        let msgTitle = "查無資料"
        let msgHtml = "<b>您輸入的關鍵字查無資料，請重新輸入關鍵字再查詢</b>"
        callErrorMsg(msgTitle, msgHtml).then((result) => {
            if (result.isConfirmed) {
                const href = "/"
                window.location.href = href
            }
        })
    } 
}
//#endregion -

//#region - 是否確定上傳資料
function formSubmit(CRUDBtn,msgTitle){
    const uploadForm = CRUDBtn.parentElement.parentElement
    callYesNoMsg(msgTitle).then((result) => {
        if (result.isConfirmed) {
            uploadForm.submit()
        }
    })
}

if(uploadBtn){
    uploadBtn.addEventListener("click", (event) => {
        formSubmit(uploadBtn, "是否確定上傳?")
    })
}
//#endregion -

//#region - 資料驗證錯誤
function callDataError() {
    const msgTitle = "資料驗證錯誤"
    let msgHtml = '<div style="text-align:justify" class="col-10 offset-1"><b>'
    let msgCount = 0
    dataError.forEach((errorMsg) => {
        msgHtml += `<u>${++msgCount}.${errorMsg.dataset.error}</u><br>`
    })
    msgHtml += "</b></div>"

    callErrorMsg(msgTitle, msgHtml).then((result) => {
        if (result.isConfirmed) {
            dataError.forEach((inputData) => {
                if (inputData.id !== "uploadBtn") {
                    inputData.value = ""
                    inputData.innerHTML = ""
                    inputData.style.border = "1px solid red"
                }
                inputData.dataset.error = ""
            })
        }
    })
}
//#endregion -


