function setAcctInfo() {

    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));

    var TrueName = '';
    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    if (UserInfo.Info.TrueName == '' || UserInfo.Info.TrueName == null || UserInfo.Info.TrueName == undefined) {
        TrueName = UserInfo.Info.Phone;
    } else {
        TrueName = UserInfo.Info.TrueName;
    }
    var Phone = UserInfo.Info.Phone;
    
    var HasPayPwd = '';
    if (UserInfo.Info.HasPayPwd == false || UserInfo.Info.HasPayPwd == 'false') {
        HasPayPwd = '未设置';
    } else {
        HasPayPwd = '已设置';
    }
    
    $('#HasPayPwd').text(HasPayPwd);
    
    $('#TrueName').text(TrueName);
    $('#Phone').text(Phone);

}


$('#nav_left').click(function() {
    appcan.window.close(-1);
});

$('.pwd').click(function() {

    var localPack = localStorage.getItem('localPack');

    if (localPack == '1' || localPack == 1) {

        appcan.window.open({
            name : 'pwd_enter',
            data : '../setup/pwd_enter.html',
            aniId : 10,
        });

    } else {

        uexWindow.open({
            name : "pwd_enter",
            data : "../setup/pwd_enter.html",
            animID : 10,
            flag : 1024
        });

    }

});

$('.pay').click(function() {
    
    var HasPayPwd = trim($('#HasPayPwd').text());
    
    localStorage.setItem("HasPayPwd", HasPayPwd);
    
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

});

$('.login').click(function() {

    openToast("功能正在开发中，敬请期待...", 3000, 5, 0);

});
