//登录点击方法
$("#login").click(function() {

    var mobile = $("#username").val();
    validatePhone(mobile);

});

function validateMobile2(mobile) {

    var tmpStr = '';
    if (mobile.length == 0 || mobile == '' || mobile == undefined) {
        openToast('请输入手机号码', 2000, 5, 0);
        return false;
    } else {

        if (mobile.length != 11) {
            openToast('请输入有效的手机号码', 2000, 5, 0);
            return false;
        } else {

            //var regExps = /^(((13[0-9]{1})|(15[0-9]{1})|(18[0-9]{1}))+\d{8})$/;
            var regExps = /^1[3|4|5|7|8][0-9]\d{4,8}$/;
            if (!regExps.test(mobile)) {
                openToast('请输入有效的手机号码', 2000, 5, 0);
                return false;
            } else {
                //匹配成功后正式处理的注册逻辑
                //openToast(mobile, 2000, 5, 0);
                //1.发送验证码

                var username = $("#username").val();
                var password = $("#password").val();
                doLoginJudg(username, password);

            }

        }

    }

    //openToast(tmpStr, 2000, 5, 0);

}

function validatePhone(mobile) {

    if (mobile.length == 0 || mobile == '' || mobile == undefined) {
        openToast('请输入手机号码', 2000, 5, 0);
        return false;
    } else {

        var username = trim($("#username").val());
        var password = trim($("#password").val());
        var before = parseInt($('#areacode').data('areacode'));
        doLoginJudg(before, username, password);

    }

    //openToast(tmpStr, 2000, 5, 0);

}

//登录逻辑之前的判断
function doLoginJudg(before, username, password) {

    var judgArr = [];
    if (username == '' || username == null || username == undefined) {
        judgArr.push('账号');
    }
    if (password == '' || password == null || password == undefined) {
        judgArr.push('密码');
    }

    if (judgArr.length == 0 || judgArr == '') {

        showLoading('正在登录中...', 1);
        doLoginActn(before, username, password);

    } else {

        var str = '请输入您的' + judgArr.join('以及').toString();
        openToast(str, 3000, 5, 0);

    }

}

//登录的逻辑
function doLoginActn(before, username, password) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");

    appcan.ajax({
        url : ajaxUrl + "Account/DoLogin",
        type : "POST",
        data : {
            before : before,
            phone : username,
            pwd : password,
            remember : 1,
            ostype : '',
            device : '',
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            //debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '') {

                    localStorage.setItem("username", username);
                    localStorage.setItem("password", password);

                    var uid = data.user.ID;
                    localStorage.setItem("uid", uid);
                    localStorage.setItem("user_img", data.user.HeadImg1);

                    //doWsReqActn(id, token);

                    //getUserInfo(uid, 'login');
                    judgDb(uid);

                } else {

                    hideLoading(data.msg, 5000, 5, 0);

                }

            } else {

                hideLoading('登录失败', 5000, 5, 0);

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
            hideLoading('登录失败', 5000, 5, 0);

        },
    });

}

//websocket请求
function thingsToSimgple() {

    appcan.window.closePopover("loading");

    localStorage.setItem("guide", 1);

    var localPack = localStorage.getItem('localPack');

    if (getDevInfo('2') == 'Apple') {

        if (localPack == '0' || localPack == 0) {

            getPacketList('login');

        }

    } else {

        if (localPack == '1' || localPack == 1) {

            appcan.window.open({
                name : 'home2',
                data : '../index/home2.html',
                aniId : 10,
            });

        } else {

            appcan.window.open({
                name : 'home',
                data : '../index/home.html',
                aniId : 10,
            });

        }

    }

}
