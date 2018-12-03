$('#next').click(function() {

    var fromPage = 'judg';
    doJudgCode(fromPage);

});

//接口.29.找回密码
function payJudgNext(judg_code) {

    localStorage.setItem("payCode", judg_code);

    var localPack = localStorage.getItem('localPack');

    if (localPack == '1' || localPack == 1) {

        appcan.window.open({
            name : 'pwd_set',
            data : '../pay/pwd_set.html',
            aniId : 10,
        });

    } else {

        uexWindow.open({
            name : "pwd_set",
            data : "../pay/pwd_set.html",
            animID : 10,
            flag : 1024
        });

    }
}


$("#send_code").click(function() {

    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    var AreaCode = UserInfo.Info.AreaCode;
    var phone = UserInfo.Info.Phone;

    getJudgCode(AreaCode, phone);

});

function setUserPhone() {
    
    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    var temp = UserInfo.Info.Phone;
    var phone = hidePhoneMiddle(temp);
    $('#hidePhone').val(phone);
    
}
