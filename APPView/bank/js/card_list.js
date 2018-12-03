/*
 * 接口.35.获取已绑定的银行卡列表
 */
function getBankList(fromPage) {

    openToast('正在加载数据...', 30000, 5, 1);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Bank/GetBankList",
        type : "POST",
        data : {
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            closeToast();

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    localStorage.setItem("bankList", JSON.stringify(data));

                    if (fromPage == 'withdraw') {

                        setDefBank(data);

                    } else if (fromPage == 'add_card') {

                    } else if (fromPage == 'card_list') {

                        setBankList(data);

                    }

                } else {

                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                openToast('获取数据失败', 5000, 5, 0);

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
            openToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}

//迭代银行卡列表
function setBankList(json) {

    //debug('PageIndex: ' + PageIndex);

    var fileUrl = localStorage.getItem("fileUrl");

    var listStr = '';
    var lastStr = '';

    if (json.list.length == 0) {

    } else {

        $(json.list).each(function(j, s) {

            listStr += '<div data-id="' + s.ID + '" data-bankname="' + s.BankName + '" data-bankzhihang="' + s.BankZhiHang + '" data-bankaccount="' + s.BankAccount + '" data-bankuser="' + s.BankUser + '" class="bank-padd bank-clk" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';

            listStr += '<div class="ub bank-style">';

            listStr += '<div class="bank-icon">';
            //listStr += '<img src="../css/img/bank/nongye@2x.png" />';
            if (s.BankName.indexOf('农业') != -1) {
                listStr += '<img src="../css/img/bank/nongye@2x.png" />';
            } else if (s.BankName.indexOf('工商') != -1) {
                listStr += '<img src="../css/img/bank/gongshang@2x.png" />';
            } else if (s.BankName.indexOf('交通') != -1) {
                listStr += '<img src="../css/img/bank/jiaotong@2x.png" />';
            } else if (s.BankName.indexOf('建设') != -1) {
                listStr += '<img src="../css/img/bank/jianshe@2x.png" />';
            } else if (s.BankName.indexOf('招商') != -1) {
                listStr += '<img src="../css/img/bank/zhaoshang@2x.png" />';
            } else {
                listStr += '<img src="../css/img/bank/yinlian@2x.png" />';
            }
            listStr += '</div>';

            listStr += '<div class="ub-f1 info-padd">';
            listStr += '<div class="nick-padd ub">';
            listStr += '<div class="user-nick overflow">';
            listStr += '<span>';
            listStr += s.BankName;
            listStr += '</span>';
            listStr += '</div>';
            listStr += '</div>';

            listStr += '<div class="user-note">';
            listStr += s.BankAccount;
            listStr += '</div>';
            listStr += '</div>';

            listStr += '<div class="bank-icon">';
            listStr += '<div class="ub-img li-right-arrow"></div>';
            listStr += '</div>';

            listStr += '</div>';
            listStr += '</div>';

        });

    }

    $("#content").children().remove();
    $('#content').append(listStr);

    $('.bank-clk').unbind();
    $('.bank-clk').click(function() {

        var ID = $(this).data('id');
        var BankName = $(this).data('bankname');
        var BankZhiHang = $(this).data('bankzhihang');
        var BankAccount = $(this).data('bankaccount');
        var BankUser = $(this).data('bankuser');

        var defBank = {
            ID : ID,
            BankName : BankName,
            BankZhiHang : BankZhiHang,
            BankAccount : BankAccount,
            BankUser : BankUser,
        }

        localStorage.setItem("defBank", JSON.stringify(defBank));
        appcan.window.publish('defBank', JSON.stringify(defBank));

        setTimeout(function() {
            appcan.window.publish('bankBack', 1);
        }, 300);

    });

}