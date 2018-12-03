$("#send_code").click(function() {

    var mobile = $('#phone').val();
    var before = parseInt($('#areacode').data('areacode'));
    getJudgCode(before, mobile);

});

$('#reg').click(function() {
    //logAjax("login", "注册用户", "否")
    var mobile = $('#phone').val();
    var fromPage = 'reg';
    validateMobile(mobile, fromPage);
    //doJudgCode();
});

//正式注册的入口：设置用户名、密码
function doRegActn() {

    var password = trim($('#password').val());
    var phone = trim($("#phone").val());
    var code = trim($("#judg_code").val());
    var tjr = trim($("#tjr").val());

    if (password == null || password == '' || password == undefined) {

        openToast('用户名以及密码不能为空', 3000, 5, 0);

    } else {
        
        showLoading('正在注册...', 1);

        //正式开始注册7
        //password = md5(password);
        var ajaxUrl = localStorage.getItem("ajaxUrl");

        appcan.ajax({
            url : ajaxUrl + "Account/DoRegist",
            type : "POST",
            data : {
                code : code,
                pwd : password,
                tjr : tjr,
            },
            timeout : 30000,
            dataType : "json",
            success : function(data) {
                //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")
                debug(data);
                if (data.length == 0) {

                    hideLoading('注册失败', 3000, 5, 0);

                } else {

                    if (data.code == 1 || data.code == '1') {
                        
                        localStorage.setItem("succTxt", '欢迎 您已注册成功!');

                        localStorage.setItem("username", phone);
                        localStorage.setItem("password", password);
                        localStorage.setItem("succ", 1);
                        appcan.window.publish('regSucs', 1);
                        setTimeout(function() {

                            appcan.window.close(-1);

                        }, 500);

                    } else {

                        hideLoading(data.msg, 3000, 5, 0);

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
                hideLoading('注册失败', 3000, 5, 0);

            },
        });

    }

}

