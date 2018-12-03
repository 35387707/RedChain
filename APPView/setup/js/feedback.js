//接口.30.3.问题反馈判断
function judgProblem() {

    var content = trim($('#question').val());

    if (content == '' || content == null || content == undefined) {
        
        openToast('请输入反馈内容', 3000, 5, 0);

    } else {

        addProblem(content);

    }

}

//接口.30.4.问题反馈ajax
function addProblem(content) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Account/AddProblem",
        type : "POST",
        data : {
            content : content,
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    //setCollectionList(uid, data, Index, fromPage);
                    openToast('反馈成功', 3000, 5, 0);
                    setTimeout(function() {

                        appcan.window.close(-1);

                    }, 500);

                } else {
                    
                    openToast(data.msg, 3000, 5, 0);
                    
                }

            } else {

                openToast('提交反馈失败', 3000, 5, 0);

            }
        },
        error : function(xhr, errorType, error, msg) {

            var debugMsg = {
                xhr : xhr,
                errorType : errorType,
                error : error,
                msg : msg,
            }
            debug(debugMsg);
            openToast(localStorage.getItem("errorMsg"), 3000, 5, 0);

        },
    });

}