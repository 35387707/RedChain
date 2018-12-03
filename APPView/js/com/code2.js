//点击发送验证码
$("#send_code").click(function() {

    var mobile = $('#phone').val();
    getJudgCode(mobile);

});

function validateMobile(mobile, fromPage) {

    var tmpStr = '';
    if (mobile.length == 0 || mobile == '' || mobile == undefined) {

        openToast('请输入手机号码', 3000, 5, 0);
        return false;

    } else {

        if (mobile.length != 11) {

            openToast('请输入有效的手机号码', 3000, 5, 0);
            return false;

        } else {

            //var regExps = /^(((13[0-9]{1})|(15[0-9]{1})|(18[0-9]{1}))+\d{8})$/;
            var regExps = /^1[3|4|5|7|8][0-9]\d{4,8}$/;
            if (!regExps.test(mobile)) {

                openToast('请输入有效的手机号码', 3000, 5, 0);
                return false;

            } else {

                //匹配成功后正式处理的注册逻辑
                //openToast(mobile, 2000, 5, 0);
                //1.发送验证码
                doJudgCode(fromPage);
            }

        }

    }

    //openToast(tmpStr, 2000, 5, 0);

}

//可点击的状态
var btn_status = true;
function getJudgCode(mobile) {

    if (mobile.length == 0 || mobile == '' || mobile == undefined) {

        openToast('请输入手机号码', 3000, 5, 0);
        return false;

    } else {

        if (mobile.length != 11) {

            openToast('请输入有效的手机号码', 3000, 5, 0);
            return false;

        } else {

            //var regExps = /^(((13[0-9]{1})|(15[0-9]{1})|(18[0-9]{1}))+\d{8})$/;
            var regExps = /^1[3|4|5|7|8][0-9]\d{4,8}$/;
            if (!regExps.test(mobile)) {

                openToast('请输入有效的手机号码', 3000, 5, 0);
                return false;

            } else {

                //匹配成功后正式处理的注册逻辑
                //openToast(mobile, 2000, 5, 0);
                //1.发送验证码

                var phone = mobile;

                var ajaxUrl = localStorage.getItem("ajaxUrl");

                if (btn_status) {

                    appcan.ajax({
                        url : ajaxUrl + "Verifycode/getVerifyCode",
                        type : "POST",
                        data : {
                            t : 2,
                            r : phone,
                            b : '',
                        },
                        timeout : 10000,
                        dataType : "json",
                        success : function(data) {

                            debug(data);
                            if (data == '1') {

                                btn_status = false;
                                time_code_value = 0;
                                time_code();
                                ajax_time = new Date();
                                localStorage.setItem("phone", phone);

                            } else {

                                openToast(data.msg, 5000, 5, 0);

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
                            openToast('验证码获取失败', 3000, 5, 0);
                        },
                    });

                } else {

                }

            }

        }

    }

}

//计时开始的时间
var time_code_value = 0;
//计时器对象
var t;

//计时方法　
function time_code() {
    time_code_value++;
    var value = -(time_code_value - 60);
    $("#send_code").text("剩余 " + value + " 秒");
    if (time_code_value < 60) {
        //继续执行
        t = setTimeout("time_code()", 1000);
    } else {
        //停止计时
        $("#send_code").text("发送验证码");
        btn_status = true;
        clearTimeout(t);
    }

}


//验证时间
var ajax_time = new Date();
//点击注册时候的方法
function doJudgCode(fromPage) {

    var judg_code = $("#judg_code").val();
    var phone = '';
    if (fromPage == 'judg') {
        var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
        phone = UserInfo.Info.Phone;
    } else {
        phone = $("#phone").val();
    }
    
    if (phone == localStorage.getItem("phone")) {

        if (judg_code.length == 0 || judg_code == '' || judg_code == null || judg_code == undefined) {

            openToast('请输入验证码', 3000, 5, 0);

        } else {

            if (judg_code.length == 6) {

                if (/^[0-9]+$/.test(judg_code)) {

                    if (fromPage == 'reg') {
                        
                         doRegActn();
                        
                    } else if (fromPage == 'find') {
                        
                        findPwdNext(judg_code);
                        
                    } else if (fromPage == 'judg') {
                        
                        payJudgNext(judg_code);
                        
                    }

                } else {
                    //alert('搞什么啊');
                    openToast('请输入正确的验证码格式', 3000, 5, 0);
                }

            } else {
                openToast('请输入正确的验证码格式', 3000, 5, 0);
            }

        }

    } else {

        openToast('请先获取验证码', 3000, 5, 0);

    }

}