//var re = /(\w)*(\w)\2{2}(\w)*/g;
//判断只能输入6位, 并且不能重复
function judgPayNum() {

    var value = $(".num-disp-box").data("pay");
    //alert(value);

    if (value.length == 0 || value == null || value == '' || value == undefined) {

        openToast('请输入6位数的支付密码', 5000, 5, 0);
        return false;

    } else {

        if (value.length != 6) {

            openToast('请输入6位数的支付密码', 5000, 5, 0);
            return false;

        } else {

            var regExps = /(\d)\1{2,}/ig;

            if (regExps.test(value)) {

                openToast('不能含有连续三位重复的数字', 5000, 5, 0);
                return false;

            } else {

                //alert('输入正确');
                var rslt = denySimplePwd(value);
                if (rslt == 1) {

                    openToast('该支付密码过于简单, 请重新输入', 5000, 5, 0);
                    return false;

                } else {

                    var HasPayPwd = localStorage.getItem("HasPayPwd");
                    if (HasPayPwd == '未设置') {
                        openToast("正在设置支付密码", 300000, 5, 1);
                    } else if (HasPayPwd == '已设置') {
                        openToast("正在修改支付密码", 300000, 5, 1);
                    }

                    //openToast('输入正确', 5000, 5, 0);
                    var id = 2;
                    var oldPwd = '';
                    var newPwd = value;
                    var code = localStorage.getItem("payCode");
                    var fromPage = 'pay_pwd';
                    doChangePwd(id, oldPwd, newPwd, code, fromPage);

                }

            }

        }

    }

}

/*
 * 密码不能过于简单
 */
function denySimplePwd(value) {

    var arr = ['012345', '123456', '234567', '345678', '456789', '567890', '678910', '109876', '098765', '987654', '876543', '765432', '654321', '543210'];
    var rslt = 0;
    $(arr).each(function(i, v) {

        if (v == value) {
            rslt = 1;
        }

    });

    return rslt;

}

function payOrderBack() {

    var localPack = localStorage.getItem('localPack');

    if (localPack == '1' || localPack == 1) {

        appcan.window.open({
            name : 'order',
            data : '../order/order.html',
            aniId : 10,
        });

    } else {

        uexWindow.open({
            name : "order",
            data : "../order/order.html",
            animID : 10,
            flag : 1024
        });

    }

    setTimeout(function() {

        appcan.window.publish('orderHome', 1);
        appcan.window.publish('orderBack', 1);
        appcan.window.closePopover('pay_pwd');
        localStorage.setItem("pay_pwd", 0);

    }, 100);

}

/*
 * 判断是否有设置支付密码
 */
function judgHasPayPwd() {

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
        localStorage.setItem("pay", 1);

        appcan.window.openPopover({
            name : 'pay',
            dataType : 0,
            url : "../pay/pay.html",
            top : 0,
            left : 0,
            width : bodyWidth,
            height : bodyHeight,
        });

    }

}
