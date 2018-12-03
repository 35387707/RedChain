var carry_stat = true;
$('#carry').click(function() {

    if (carry_stat) {

        carry_stat = false;
        judgCarryReq();

    }

});

//判断所有必填条件
function judgCarryReq() {

    var req_arr = [];
    var money = trim($('#money').val());
    if (money == 0 || money == '0' || money == '' || money == null || money == undefined) {
        req_arr.push('金额');
    }

    var recnum = trim($('#recnum').val());
    if (recnum == '' || recnum == null || recnum == undefined) {
        req_arr.push('推荐码');
    }

    if (req_arr.length == 0 || req_arr == '') {

        carry_stat = true;

        //judgImgUpld();
        judgHasPayPwd();

    } else {

        carry_stat = true;
        openToast('请输入' + req_arr.join('、'), 3000, 5, 0);

    }

}


function doTransforOther(pwd) {

    //需要传入的参数
    var money = trim($('#money').val());
    var recnum = trim($('#recnum').val());

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "Account/DoTransforOther",
        type : "POST",
        data : {
            recnum : recnum,
            money : money,
            pwd : pwd,
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {
                    
                    localStorage.setItem("succTxt", '恭喜 转账成功！');

                    appcan.window.publish('carrySucs', 1);

                    payMsgToast('转账成功', 5000, 5, 0);

                    setTimeout(function() {

                        appcan.window.close(-1);

                    }, 1500);

                } else if (data.code == -113 || data.code == '-113') {

                    carry_stat = true;

                    setTimeout(function() {

                        appcan.window.publish('wrongPwd', 1);

                    }, 300);

                } else {

                    carry_stat = true;

                    payMsgToast(data.msg, 5000, 5, 0);

                }

            } else {

                carry_stat = true;
                payMsgToast('转账失败', 5000, 5, 0);

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
            carry_stat = true;
            payMsgToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}
