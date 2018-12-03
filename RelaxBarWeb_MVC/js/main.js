$(document).ready(function(){
    /*生成表情*/
    (function(){
        /*创建并添加emoji*/
        for(var i=0;i<24;i++){
            var a=document.createElement("a");
            a.href="javascript:void(0);";
            a.className="emoji";
//          if(i<8){
//              if(i===0){
//                  a.style.backgroundPosition=-(i*2.5+0.15)+"rem -0.15rem";
//              }else{
//                  a.style.backgroundPosition=-(i*2.8+0.15)+"rem -0.15rem";
//              };
//              $(".cfb-emoji-item").eq(0).append(a);
//          };
            a.style.backgroundImage = "url(/img/chat/emoji_" + (i + 1) + ".png)";
            $(a).attr("data-v", i);
            $(a).click(function () {
                var v = parseInt($(this).attr("data-v"));
                $("#scontent").val($("#scontent").val() + emoji[v]);
            });
            $(".cfb-emoji-item").eq(0).append(a);
            
        };
    })();
    
    /*点击加号显示更多菜单*/
    (function(){
        $(".cross").on("click",function(){
	        $(".header-extra-wrap").show();
	        return false;
	    });
	    $(document).on("click",function(){
	        $(".header-extra-wrap").hide();
	    });
    })();
    
    /*登录页——点击记住密码，选中状态切换*/
    (function(){
        var on=true;
        $(".psw-item").on("click",function(){
            if(on){
                $(this).find(".psw-cb-icon").addClass("checked").parents(".psw-item-wrap").siblings(".psw-item-wrap").find(".psw-cb-icon").removeClass("checked");
            };
        });
    })();
    
    /*点击开关切换*/
    (function(){
        $(".switch").on("click",function(){
            if($(this).hasClass("switch-on")){
                $(this).removeClass("switch-on").addClass("switch-off").find(".move").removeClass("move-on").addClass("move-off");
            }else if($(this).hasClass("switch-off")){
                $(this).removeClass("switch-off").addClass("switch-on").find(".move").removeClass("move-off").addClass("move-on");
            };
        });
    })();
    
    /*点击+号按钮弹起聊天框输入框*/
    (function(){
        $(".cfb-emoji").on("click",chatblockUp);
        $(".cfb-more").on("click",chatblockUp);
        //$(".cfb-txt").on("click",chatblockUp);
        $(document).on("click",chatblockDown);
        $(".cfb-item,.cfb-bot,cfb-top").on("click",function(){
            return false;
        });
        function chatblockUp(ev){
            var ev=ev||event;
            $(".chatfoot-block").removeClass("cfb-down").addClass("cfb-up");
            $(".chat-window-wrap").removeClass("chatblock-wrap-ptDown").addClass("chatblock-wrap-ptUp");
            if($(this).hasClass("cfb-emoji")){
                $(".swipe").show().siblings().hide();
            };
            if($(this).hasClass("cfb-more")){
                $(".cfb-item-wrap").show().siblings().hide();
            };
            ev.stopPropagation();
        };
        function chatblockDown(ev){
        	var ev=ev||event;
            $(".chatfoot-block").removeClass("cfb-up").addClass("cfb-down");
            $(".chat-window-wrap").removeClass("chatblock-wrap-ptUp").addClass("chatblock-wrap-ptDown");
        };
    })();
    
    /*点击扫雷发包选择发包类型*/
    (function(){
        $(".slfb-number").on("click",function(){
            $(this).addClass("active").siblings(".slfb-number").removeClass("active");
        });
        $(".slfb-jl-item").on("click",function(){
            $(this).addClass("active").siblings(".slfb-jl-item").removeClass("active");
        });
    })();
    
    /*点击选择银行卡*/
    (function(){
        $("#banklist .bankCard").on("click",function(){
        	var o=document.createElement("i");
        	o.className="checked-icon";
            $(this).append(o).siblings(".bankCard").find(".checked-icon").remove();
        });
    })();
    
    /*点击管理银行卡*/
    (function(){
        $(".dele-btn").on("click",function(){
            $(this).parents(".bankCard").remove();
        });
    })();
    
    /*点击删除并退出键，显示mask和弹框*/
    (function(){
        $("#leavegroup").on("click",function(){
            $(".blackmask").show().find("#ab-leavegroup").show();
        });
        $(".alert-confirm a").on("click",function(){
            $(".blackmask").hide();
        });
    })();
    
    /*发红包输入，按钮变高亮*/
    (function(){
        $(".money-iptTxt").on("input",function(){
            var t=$(this).val();
            if(t!==""){
                $(".bigBtn").removeClass("transparent");  
            };
        });
    })();
    
    /*新的朋友点击添加变成 已添加状态*/
    (function(){
        $(".nf-addbtn").on("click",function(){
            $(this).val("已添加").removeClass("nf-addbtn").addClass("nf-disable-btn").attr("disabled");
        });
    })();
    
    /*账单——点击“筛选”选择交易类型*/
    (function(){
        $(".deal-item").on("click",function(){
            $(this).addClass("active").siblings(".deal-item").removeClass("active");
        });
        $("#filtrate").on("click",function(){
            $(".blackmask").fadeIn(200);
            $(".deal-block").removeClass("deal-block-down").addClass("deal-block-up");
        });
        $(".deal-cancelbtn").on("click",function(){
            $(".blackmask").fadeOut(200);
            $(".deal-block").removeClass("deal-block-up").addClass("deal-block-down");
        });
    })();
    
    /*账单——点击日历按钮弹出年月区间选择框*/
    (function(){
        $(".calendar-icon").on("click", function () {
            $(".blackmask").fadeIn(200).find(".date-plugIn-wrap").show();
        });
        $(".pI-cancel").on("click",function(){
            $(".date-plugIn-wrap").hide().parents(".blackmask").hide();
        });
    })();
    /*点击聊天图片放大*/
    (function () {
        $(".chat-photo").on("click", function () {
            $("#bigimg").attr("src", $(this)[0].src);
            $("#bigimg-wrap").fadeIn(100);
        });
        $("#bigimg-wrap").on("click", function () {
            $("#bigimg").attr("src", "");
            $("#bigimg-wrap").fadeOut(100);
        });
    })();
    /*聊天窗口输入回车让窗口保持在最新一条消息*/
//  (function(){
//      $(".cfb-txt").on("keydown",function(ev){
//          if(ev.keyCode===13){
//              
//          };
//      });
//  })();
    
    /*下拉加载数据让窗口保持在原来的位置*/
    
    /*初始化窗口让窗口保持在最新一条消息，也就是底部*/
    //(function(){
    //	$(".chat-window-wrap").scrollTop($(".chat-window-wrap")[0].scrollHeight);
    //})();

    /*选择转发人员*/
    //$("#zf-contact-list .zf-contant-item").on("click", function () {
    //    if ($(this).find(".zf-checkbox").hasClass("active")) {
    //        //取消选中；
    //        $(this).find(".zf-checkbox").removeClass("active");
    //        if (!isChecked()) {
    //            $("#zf-qx-i").removeClass("active");
    //        };
    //    } else {
    //        //选中操作；
    //        $(this).find(".zf-checkbox").addClass("active");
    //        if (isChecked()) {
    //            $("#zf-qx-i").addClass("active");
    //        };
    //    };
    //});

    /*点击全选*/
    $("#zf-qx-i").on("click", function () {
        if ($(this).hasClass("active")) {
            $(this).removeClass("active");
            $("#zf-contact-list .zf-contant-item").find(".zf-checkbox").removeClass("active");
        } else {
            $(this).addClass("active");
            $("#zf-contact-list .zf-contant-item").find(".zf-checkbox").addClass("active");
        };
    });

    function isChecked() {
        var bool = null;
        var arr = $("#zf-contact-list .zf-contant-item").find(".zf-checkbox");
        arr.each(function (index, ele) {
            if (!$(ele).hasClass("active")) {
                bool = false;
                return false;
            } else {
                bool = true;
            };
        });
        return bool;
    };

});/*end*/
