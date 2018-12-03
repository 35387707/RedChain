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
    $('#Phone').val(Phone);

}

$('#sure').click(function() {
    
    judgUsrPwd();
    
});

//判断密码
function judgUsrPwd() {

    var id = 1;
    var fromPage = 'log_pwd';
    var code = '';

    var judgOldPwd = localStorage.getItem("password");

    var oldPassword = trim($('#old_pwd').val());
    var newPassword = trim($('#new_pwd').val());
    var againPassword = trim($('#agn_pwd').val());

    if ((oldPassword == null || oldPassword == '' || oldPassword == undefined) || (newPassword == null || newPassword == '' || newPassword == undefined) || (againPassword == null || againPassword == '' || againPassword == undefined)) {

        openToast('密码输入不能为空', 3000, 5, 0);

    } else {

        if (oldPassword != judgOldPwd) {

            openToast('旧密码输入错误', 3000, 5, 0);

        } else {

            if (newPassword == judgOldPwd) {

                openToast('新旧密码不能相同', 3000, 5, 0);

            } else {

                if (againPassword == newPassword) {
                    
                    openToast('正在修改登录密码...', 30000, 5, 1);

                    doChangePwd(id, oldPassword, newPassword, code, fromPage);

                } else {

                    openToast('两次密码不一致', 3000, 5, 0);

                }

            }

        }

    }

}