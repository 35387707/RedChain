var with_stat = true;
$('#withdraw').click(function() {

    if (with_stat) {

        with_stat = false;
        judgWithRedReq();

    }

});

function judgWithRedReq() {

    var req_arr = [];
    var bankId = trim($('#bankAccount').data('bankid'));
    if (bankId == '') {
        req_arr.push('银行卡');
    }

    var money = trim($('#money').val());
    if (money == 0 || money == '0' || money == '' || money == null || money == undefined) {
        req_arr.push('提现金额');
    }

    if (req_arr.length == 0 || req_arr == '') {

        with_stat = true;

        //judgImgUpld();
        judgWithPayPwd();

    } else {

        with_stat = true;
        openToast('请输入' + req_arr.join('、'), 5000, 5, 0);

    }

}

function judgWithPayPwd() {

    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    if (UserInfo.Info.HasPayPwd == false || UserInfo.Info.HasPayPwd == 'false') {

        openToast('您当前还没有设置支付密码', 5000, 5, 0);

        localStorage.setItem("HasPayPwd", '未设置');

        setTimeout(function() {

            var localPack = localStorage.getItem('localPack');

            if (localPack == '1' || localPack == 1) {

                appcan.window.open({
                    name : 'judg',
                    data : '../pay/judg.html',
                    aniId : 10,
                });

            } else {

                uexWindow.open({
                    name : "judg",
                    data : "../pay/judg.html",
                    animID : 10,
                    flag : 1024
                });

            }

        }, 1500);

    } else {

        var bodyWidth = localStorage.getItem("bodyWidth");
        var bodyHeight = localStorage.getItem("bodyHeight");

        localStorage.setItem("pay_pwd", 1);

        appcan.window.openPopover({
            name : 'pay_pwd',
            dataType : 0,
            url : "../pay/pwd.html",
            top : 0,
            left : 0,
            width : bodyWidth,
            height : bodyHeight,
        });

    }

}

/*
 * 接口.33.提现申请
 */
function doTransforout(paypwd) {

    var bankID = trim($('#bankAccount').data('bankid'));
    var money = trim($('#money').val());

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Account/DoTransforout",
        type : "POST",
        data : {
            bankID : bankID,
            money : money,
            paypwd : paypwd,
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    //uexLoadingView.close();

                    appcan.window.publish('refreshCash', 1);
                    payMsgToast('提现成功', 5000, 5, 0);

                    setTimeout(function() {

                        appcan.window.close(-1);

                    }, 1500);

                } else if (data.code == -113 || data.code == '-113') {

                    with_stat = true;
                    //uexLoadingView.close();

                    setTimeout(function() {

                        appcan.window.publish('wrongPwd', 1);

                    }, 300);

                } else {

                    with_stat = true;
                    //uexLoadingView.close();
                    payMsgToast(data.msg, 5000, 5, 0);

                }

            } else {

                with_stat = true;
                openToast('提现失败', 5000, 5, 0);

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
            with_stat = true;
            openToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}

/*
 * 判断当前账户是有有绑定过银行卡
 */
function judgDefBank() {

    var defBank = localStorage.getItem("defBank");
    if (defBank == '' || defBank == null || defBank == undefined || defBank == NaN) {

        getBankList('withdraw');

    } else {

        defBank = JSON.parse(defBank);
        
        debug(defBank);

        var bankImg = '';
        var bankName = defBank.BankName;
        var bankAccount = defBank.BankAccount;
        var bankId = defBank.ID;
        if (bankName.indexOf('农业') != -1) {
            bankImg = '<img src="../css/img/bank/nongye@2x.png" />';
        } else if (bankName.indexOf('工商') != -1) {
            bankImg = '<img src="../css/img/bank/gongshang@2x.png" />';
        } else if (bankName.indexOf('交通') != -1) {
            bankImg = '<img src="../css/img/bank/jiaotong@2x.png" />';
        } else if (bankName.indexOf('建设') != -1) {
            bankImg = '<img src="../css/img/bank/jianshe@2x.png" />';
        } else if (bankName.indexOf('招商') != -1) {
            bankImg = '<img src="../css/img/bank/zhaoshang@2x.png" />';
        } else {
            bankImg = '<img src="../css/img/bank/yinlian@2x.png" />';
        }

        $('#bankImg').children().remove();
        $('#bankImg').append(bankImg);
        $('#bankName').text(bankName);
        $('#bankAccount').text(bankAccount);
        $('#bankAccount').data('bankid', bankId);

    }

}

function setDefBank(json) {

    if (json.list.length == 0) {

        appcan.window.alert({
            title : "",
            content : "使用提现功能需添加一张支持提现的储蓄卡",
            buttons : ['添加', '取消'],
            callback : function(err, data, dataType, optId) {
                
                if (data == 0) {

                    var localPack = localStorage.getItem('localPack');
                    if (localPack == '1' || localPack == 1) {

                        localStorage.setItem('fromPage', 'withdraw');

                        appcan.window.open({
                            name : 'add_card',
                            data : '../bank/add_card.html',
                            aniId : 10,
                        });

                    } else {

                        uexWindow.open({
                            name : "add_card",
                            data : "../bank/add_card.html",
                            animID : 10,
                            flag : 1024
                        });

                    }

                } else if (data == 1) {

                    appcan.window.close(-1);

                }

            }
        });

    } else {

        localStorage.setItem("defBank", JSON.stringify(json.list[0]));

        var bankImg = '';
        var bankName = json.list[0].BankName;
        var bankAccount = json.list[0].BankAccount;
        var bankId = json.list[0].ID;
        if (bankName.indexOf('农业') != -1) {
            bankImg = '<img src="../css/img/bank/nongye@2x.png" />';
        } else if (bankName.indexOf('工商') != -1) {
            bankImg = '<img src="../css/img/bank/gongshang@2x.png" />';
        } else if (bankName.indexOf('交通') != -1) {
            bankImg = '<img src="../css/img/bank/jiaotong@2x.png" />';
        } else if (bankName.indexOf('建设') != -1) {
            bankImg = '<img src="../css/img/bank/jianshe@2x.png" />';
        } else if (bankName.indexOf('招商') != -1) {
            bankImg = '<img src="../css/img/bank/zhaoshang@2x.png" />';
        } else {
            bankImg = '<img src="../css/img/bank/yinlian@2x.png" />';
        }

        $('#bankImg').children().remove();
        $('#bankImg').append(bankImg);
        $('#bankName').text(bankName);
        $('#bankAccount').text(bankAccount);
        $('#bankAccount').data('bankid', bankId);

    }

}
