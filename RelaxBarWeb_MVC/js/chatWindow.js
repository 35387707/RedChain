$(document).ready(function(){
    /*初始化窗口让窗口保持在最新一条消息，也就是底部*/
    (function(){
    	$(".chat-window-wrap").scrollTop($(".chat-window-wrap")[0].scrollHeight);
    })();
    (function () {
        /*聊天窗口——输入框输入的时候，输入框的高度随着文字的高度撑开，由于textarea不能自动撑开，会出现滚动条，因此用Js控制*/
        $("#scontent").on("input", function () {
            if ($(this).val().length === 0) {
                cfbReset();
                return;
            };
            var h = $(this)[0].scrollHeight / 20;
            if (h > 8) {
                return;
            };
            $(this).css("height", h + "rem");
            if (h - 2 < 2) {
                return;
            };
            //$("#chatfoot-block").css("padding-bottom", (h - 2) + "rem");
        });

        function cfbReset() {
            //重置 输入框的高度 和 聊天版块的padding-bottom；
            $("#scontent").css("height", "2.5rem");
           // $("#chatfoot-block").css("padding-bottom", "2rem");
        };
    })();
    /*聊天窗口---输入有文字的话，弹出发送按钮，否则收起发送按钮*/
    //(function(){
    //    //$(".cfb-txt").on("input",function(ev){
    //    //	var t1=null;
    //    //    if(ev.target.value.length!==0){
    //    //        t1=setTimeout(function(){
    //    //           // $(".cfb-sendbtn").stop().fadeIn(50).siblings(".cfb-more").hide();
    //    //            clearTimeout(t1);
    //    //            t1=null;
    //    //        },200);
    //    //    }else{
    //    //       // $(".cfb-more").stop().fadeIn(50).siblings(".cfb-sendbtn").hide();
    //    //    };
    //    //});
    //    //$(".cfb-sendbtn").on("click",function(){
    //    //    $(this).siblings(".cfb-txt").val("");
    //    //});
    //})();
});
$(function () {
    $(".cfb-txt").on("input", function () {
        if ($(".cfb-sendbtn").css("display") == "none") {
            $(".cfb-sendbtn").show();
            $(".cfb-more").hide();
        }
        if ($(this).val().length == 0) {
            $(".cfb-sendbtn").hide();
            $(".cfb-more").show();
        }
        
    });
    $(".cfb-txt").focus(function () {
        $(".cfb-sendbtn").show();
        $(".cfb-more").hide();
    });
    $(".cfb-txt").blur(function () {
        if ($(this).val().length == 0) {
            $(".cfb-sendbtn").hide();
            $(".cfb-more").show();
        }
        
    });
    $(".cfb-sendbtn").on("click", function () {
        $(".cfb-sendbtn").hide();
        $(".cfb-more").show();
        sendText();
    });
});
