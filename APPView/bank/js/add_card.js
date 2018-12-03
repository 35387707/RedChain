/*
 * 接口.34.银行卡绑定
 */
var add_stat = true;
$('#add_card').click(function() {

    if (add_stat) {

        add_stat = false;
        judgCardReqCont();

    }

});

//判断所有必填条件
function judgCardReqCont() {

    var req_arr = [];
    var bankName = trim($('#bankName').val());
    if (bankName == '') {
        req_arr.push('银行名称');
    }

    var bankZhiHang = trim($('#bankZhiHang').val());
    if (bankZhiHang == '') {
        req_arr.push('支行名称');
    }

    var bankAccount = trim($('#bankAccount').val());
    if (bankAccount == '') {
        req_arr.push('银行账户');
    }

    var bankUser = trim($('#bankUser').val());
    if (bankUser == '') {
        req_arr.push('卡户姓名');
    }

    if (req_arr.length == 0 || req_arr == '') {

        addCardNew();

    } else {

        add_stat = true;
        openToast('请输入' + req_arr.join('、'), 3000, 5, 0);

    }

}

/*
 *
 */
function addCardNew() {

    var bankName = trim($('#bankName').val());
    var bankZhiHang = trim($('#bankZhiHang').val());
    var bankAccount = trim($('#bankAccount').val());
    var bankUser = trim($('#bankUser').val());

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Bank/Add",
        type : "POST",
        data : {
            bankName : bankName,
            bankZhiHang : bankZhiHang,
            bankAccount : bankAccount,
            bankUser : bankUser,
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {
                    
                    var fromPage = localStorage.getItem('fromPage');
                    if (fromPage == 'withdraw') {
                        
                        var defBank = {
                            ID : data.msg,
                            BankName : bankName,
                            BankZhiHang : bankZhiHang,
                            BankAccount : bankAccount,
                            BankUser : bankUser,
                        }
                        localStorage.setItem("defBank", JSON.stringify(defBank));
                        appcan.window.publish('defBank', JSON.stringify(defBank));
                        
                    } else if (fromPage == 'card_list') {
                        
                        appcan.window.publish('refreshBank', 1);
                        
                    }

                    openToast('新增银行卡成功', 5000, 5, 0);
                    setTimeout(function() {
                        appcan.window.close(-1);
                    }, 1500);

                } else {

                    add_stat = true;
                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                add_stat = true;
                openToast('新增银行卡失败', 5000, 5, 0);

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
            add_stat = true;
            openToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}
