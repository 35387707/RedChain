//出现浮动层
$(".pay-at-once").click(function() {
    $(".num-cont").show();
});
//关闭浮动
$(".close").click(function() {
    $(".num-cont").hide();
    $(".num-disp-box li").removeClass("num-cover");
    $(".num-disp-box li").attr("data", "");
    i = 0;
});
//数字显示隐藏
$(".num-display").click(function() {
    $(".num-input-box").slideUp(500);
});
$(".num-disp-box").click(function() {
    $(".num-input-box").slideDown(500);
});
//----
var i = 0;
$(".num-key li .num-sgl").click(function() {

    if (i < 6) {
        $(".num-disp-box li").eq(i).addClass("num-cover");
        $(".num-disp-box li").eq(i).attr("data", $(this).text());
        i++
        if (i == 6) {
            var data = "";
            //setTimeout(function() {
                
               
                $(".num-disp-box li").each(function() {
                    data += $(this).attr("data");
                });
                 
              
             
                //debug("支付成功" + data);
                //openToast('支付中，请稍后...', 30000, 5, 1);
                //var payRoot = localStorage.getItem("payRoot");
                //debug(payRoot);
                //if (payRoot == 'fubao') {
                //    appcan.window.publish('payFubao', data);
                //} else if (payRoot == 'order_pay') {
                //    //orderAndPay(data);
                //    appcan.window.publish('payOrder', data);
                //} else if (payRoot == 'order_detail') {
                //    appcan.window.publish('doPayOrder', data);
                //} else if (payRoot == 'carry') {
                //    appcan.window.publish('payCarry', data);
                //} else if (payRoot == 'vip_upgr') {
                //    appcan.window.publish('vipUpgr', data);
                //} else if (payRoot == 'withdraw') {
                //    appcan.window.publish('withdraw', data);
                //}
                
            //}, 100);
           // console.log(data);
           // doTransforOther(data);

            var money = $('#money').val().trim();
            var recnum = $('#recnum').val().trim();

           // console.log(money);

            var token = GetQueryString("token");
            if (token != null && token != "") {

                var html = "";
                $.post("http://www.fbddd.com/Account/DoTransforOther", "token=" + token + "&recnum=" + recnum + "&money=" + money + "&pwd=" + data,
                    function (data) {
                        if (data.length != 0) {

                            if (data.code == 1 || data.code == '1') {

                                localStorage.setItem("succTxt", '恭喜 转账成功！');

                                // appcan.window.publish('carrySucs', 1);

                                alert('转账成功');

                            } else if (data.code == -113 || data.code == '-113') {

                                carry_stat = true;
                                alert(data.msg);

                            } else {

                                carry_stat = true;

                                alert(data.msg);
                            }
                        }
                        else {
                            alert("转账失败");
                        }
                    }, "json");
            }


           
        };
    }
});

$(".num-key li .num-del").click(function() {
    if (i > 0) {
        i--
        $(".num-disp-box li").eq(i).removeClass("num-cover");
        $(".num-disp-box li").eq(i).attr("data", "");
    }
});

$(".num-key li .num-empty").click(function() {
    $(".num-disp-box li").removeClass("num-cover");
    $(".num-disp-box li").attr("data", "");
    i = 0;
});

$('.empty-view').click(function() {

    appcan.window.closePopover('pay_pwd');
    localStorage.setItem("pay_pwd", 0);

});

$('#nav_left').click(function() {

    appcan.window.closePopover('pay_pwd');
    localStorage.setItem("pay_pwd", 0);

});


//function doTransforOther(pwd) {

//    //需要传入的参数
 
//}

