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

$('.num-bg').click(function() {
    $(".num-input-box").slideUp(500);
});
$('.num-height').click(function() {
    $(".num-input-box").slideUp(500);
});

//----
var i = 0;
$(".num-key li .num-sgl").click(function() {

    if (i < 6) {

        $(".num-disp-box li").eq(i).addClass("num-cover");
        $(".num-disp-box li").eq(i).attr("data", $(this).text());
        i++
        if (i == 6) {

            setTimeout(function() {
                var data = "";
                $(".num-disp-box li").each(function() {
                    data += $(this).attr("data");
                });
                //alert("测试密码: " + data);
                $(".num-disp-box").data("pay", data);

            }, 100);
        } else {
            $(".num-disp-box").data("pay", '1');
        }
    }

});

$(".num-key li .num-del").click(function() {
    if (i > 0) {
        i--
        $(".num-disp-box li").eq(i).removeClass("num-cover");
        $(".num-disp-box li").eq(i).attr("data", "");
        $(".num-disp-box").data("pay", '');
    }
});

$(".num-key li .num-empty").click(function() {
    $(".num-disp-box li").removeClass("num-cover");
    $(".num-disp-box li").attr("data", "");
    $(".num-disp-box").data("pay", '');
    i = 0;
});

$('.empty-view').click(function() {

    appcan.window.close(-1);

});

$('#nav_left').click(function() {

    appcan.window.close(-1);

});

$('#nav_right').click(function() {

    judgPayNum();

});
