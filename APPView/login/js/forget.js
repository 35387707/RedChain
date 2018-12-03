$("#send_code").click(function() {

    var mobile = $('#phone').val();
    var before = parseInt($('#areacode').data('areacode'));
    getJudgCode(before, mobile);

});

$('#cmpl').click(function() {

    judgAgnPwd();

});

/*
 * 判断密码是否一致
 */
function judgAgnPwd() {

    var newPassword = trim($('#new_pwd').val());
    var againPassword = trim($('#agn_pwd').val());

    if ((newPassword == null || newPassword == '' || newPassword == undefined) || (againPassword == null || againPassword == '' || againPassword == undefined)) {

        openToast('密码输入不能为空', 3000, 5, 0);

    } else {

        if (newPassword != againPassword) {

            penToast('两次密码不一致', 3000, 5, 0);

        } else {

            doFindPwd();

        }

    }

}

//接口.29.找回密码
function doFindPwd() {

    openToast('找回密码中...', 30000, 5, 1);

    var code = localStorage.getItem("findCode");
    var pwd = $('#new_pwd').val();

    var temp = {
        code : code,
        pwd : pwd,
    }
    debug(temp);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Account/FindPsw",
        type : "POST",
        data : {
            code : code,
            pwd : pwd,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {
                    
                    localStorage.setItem("succTxt", '恭喜 找回密码成功！');

                    appcan.window.publish('findSucs', 1);
                    appcan.window.publish('fgtBack', 1);
                    openToast('找回密码成功', 3000, 5, 0);
                    setTimeout(function() {

                        appcan.window.close(-1);

                    }, 500);

                } else {

                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                openToast('找回密码失败', 5000, 5, 0);

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

//接口.29.找回密码
function findPwdNext(judg_code) {

    localStorage.setItem("findCode", judg_code);

    //judgAgnPwd();

    var localPack = localStorage.getItem('localPack');

    if (localPack == '1' || localPack == 1) {

        appcan.window.open({
            name : 'pwd',
            data : '../login/pwd.html',
            aniId : 10,
        });

    } else {

        uexWindow.open({
            name : "pwd",
            data : "../login/pwd.html",
            animID : 10,
            flag : 1024
        });

    }

}


$('#next').click(function() {
    //logAjax("login", "注册用户", "否")
    var mobile = $('#phone').val();
    var fromPage = 'find';
    validateMobile(mobile, fromPage);
    //doJudgCode();
});
