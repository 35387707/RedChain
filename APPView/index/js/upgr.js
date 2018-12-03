//1.0.升级会员
function upgradeUserType(nt) {

    if (nt == 1 || nt == '1') {
        openToast("正在申请为商家", 60000, 5, 1);
    } else {
        openToast("正在申请为代理", 60000, 5, 1);
    }

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "pay/UpgradeUserType",
        type : "POST",
        data : {
            nt : nt,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            //debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    if (nt == 1 || nt == '1') {
                        openToast("申请为商家成功", 3000, 5, 0);
                    } else {
                        openToast("申请为代理成功", 3000, 5, 0);
                    }

                } else {

                    if (nt == 1 || nt == '1') {
                        openToast("申请为商家失败", 3000, 5, 0);
                    } else {
                        openToast("申请为代理失败", 3000, 5, 0);
                    }

                }

            } else {

                if (nt == 1 || nt == '1') {
                    openToast("申请为商家失败", 3000, 5, 0);
                } else {
                    openToast("申请为代理失败", 3000, 5, 0);
                }

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
            openToast('未知错误', 3000, 5, 0);

        },
    });

}
