function setBalance() {

    //localStorage.getItem("uid"), JSON.parse(localStorage.getItem("UserInfo")), 'mine';

    var Balance = ''
    //福利积分
    var FootQuan = '';
    //福音积分
    var Score = '';

    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));

    Balance = UserInfo.Info.Balance;
    FootQuan = UserInfo.Info.FootQuan;
    Score = UserInfo.Info.Score;

    $('#Balance').text(Balance);
    $('#FootQuan').text(FootQuan);
    $('#Score').text(Score);

    //var PriceType = parseInt(localStorage.getItem("PriceType"));
    var PriceType = parseInt(localStorage.getItem("PriceType"));
    
    debug(PriceType);

    var title = '';

    if (PriceType == 0) {

        title = '福BAO多';

    } else if (PriceType == 1) {

        title = '福BAO多';

    } else if (PriceType == 2) {

        title = '福音积分';

    } else if (PriceType == 4) {

        title = '福券';
        
    }

    $('.pay-icon').removeClass('pay-green');
    $("div[data-round='" + title + "']").addClass('pay-green');

}

//接口.19.3.用户实体类, 登陆成功之后获取所有的用户信息  footQuan
function chkUserInfo(id, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Account/GetUserInfo",
        type : "POST",
        data : {
            Id : id,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")
            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    if (fromPage == 'page_other') {
                        localStorage.setItem("OtherUser", JSON.stringify(data));
                        setUserInfo(id, data, fromPage);
                    } else {
                        localStorage.setItem("UserInfo", JSON.stringify(data));
                        if (fromPage == 'login') {
                            friendAjaxList(id, fromPage);
                        } else {
                            setUserInfo(id, data, fromPage);
                        }
                    }

                } else {

                    hideLoading('获取用户信息失败', 3000, 5, 0);

                }

            } else {

                hideLoading('获取用户信息失败', 3000, 5, 0);

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
            hideLoading(localStorage.getItem("errorMsg"), 3000, 5, 0);

        },
    });

}