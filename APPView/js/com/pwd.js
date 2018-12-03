//接口.28.修改密码 密码类型 1为登录密码 2为支付密码
function doChangePwd(id, oldPwd, newPwd, code, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Account/DoChangePwd",
        type : "POST",
        data : {
            id : id,
            oldPwd : oldPwd,
            newPwd : newPwd,
            code : code,
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {
                    
                    localStorage.setItem("succ", 1);

                    //openToast('修改密码成功', 3000, 5, 0);
                    if (fromPage == 'pay_pwd') {

                        var HasPayPwd = localStorage.getItem("HasPayPwd");

                        if (HasPayPwd == '已设置') {
                            localStorage.setItem("succTxt", '恭喜 修改支付密码成功！');
                            openToast('修改支付密码成功', 5000, 5, 0);
                            //appcan.window.publish('setSucs', 1);
                        } else if (HasPayPwd == '未设置') {
                            localStorage.setItem("succTxt", '恭喜 设置支付密码成功！');
                            openToast('设置支付密码成功', 5000, 5, 0);
                        }
                        appcan.window.publish('pwdSucs', 1);

                        setTimeout(function() {

                            appcan.window.close(-1);

                        }, 500);

                    } else if (fromPage == 'log_pwd') {
                        
                        localStorage.setItem("password", newPwd);

                        localStorage.setItem("succTxt", '恭喜 修改登录密码成功！');

                        appcan.window.publish('logSucs', 1);
                        
                        openToast('修改登录面成功', 5000, 5, 0);

                        setTimeout(function() {

                            appcan.window.close(-1);

                        }, 500);

                    }

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('修改密码失败', 5000, 5, 0);

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