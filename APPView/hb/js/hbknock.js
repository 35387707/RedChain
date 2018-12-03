appcan.ready(function() {

    setSwipeCloseEnable(1);

    //var wishes = ["福运连连, 步步高升", "福星高照 , 事事顺心", "福禄双全, 天天开心", "五福监门 , 心想事成", "五福齐全 , 万事如意", "欣欣向荣 , 福国利民", "大展鸿图 , 才震四方", "宏图大展 , 融通四海", "国泰民安 , 四海升平", "吉星高照 , 金玉满堂"];
    var wishes = ["福运连连, 步步高升", "福星高照 , 事事顺心", "福禄双全, 天天开心", "五福监门 , 心想事成", "五福齐全 , 万事如意", "欣欣向荣 , 福国利民", "大展鸿图 , 才震四方", "宏图大展 , 融通四海", "国泰民安 , 四海升平", "吉星高照 , 金玉满堂", "心想事成, 万事如意", "财源滚滚, 喜气盈盈", "吉星高照, 财源广进", "前程似锦, 美梦成真", "飞黄腾达, 万事顺意", "生活如意, 事业高升", "荣华富贵, 一帆风顺", "金玉满堂, 五福临门", "笑口常开, 喜气盈门", "百业兴旺, 五谷丰登", "六六大顺, 万事大吉", "金玉满堂, 龙凤呈祥", "四时如意, 万事遂心", "金玉满堂, 喜气洋洋", "福照家门, 事事兴旺", "一帆风顺, 事事如意", "金榜题名, 心想事成", "财源广进, 富贵吉祥", "福气东来, 鸿运通天", "身体健康, 龙马精神", "青春常驻, 笑口常开", "大展宏图, 锦绣前程", "福寿安康, 寿与天齐", "四季平安, 五福临门", "家庭和睦, 幸福安康", "工作顺心, 事业有成", "天高地阔, 人寿年丰", "益寿延年, 富贵吉祥", "阖家欢乐, 龙凤呈祥", "金玉满堂, 喜气洋洋", "德勤益寿, 心阔延年", "鹏程万里, 万古长青", "大吉大利 , 笑口常开", "前程似锦, 事业有成", "福如东海, 寿比南山", "阖家幸福, 笑口常开", "美满家庭, 鸾凤和鸣", "吉祥如意, 意气风发", "花开富贵, 龙马精神"];

    $('#wishes').text(wishes[randomWishes()]);

    //setUserInfo(JSON.parse(localStorage.getItem("UserInfo")), 'hbknock');
    getPacketUserInfo();

    appcan.window.subscribe('hbknock', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            appcan.window.close(-1);

        }

    });

    appcan.window.subscribe('vipSucc', function(msg) {

        if (msg != "" && msg != null && msg != undefined) {

            appcan.window.closePopover('hbvip');
            localStorage.setItem("hbvip", 0);

            var bodyWidth = localStorage.getItem("bodyWidth");
            var bodyHeight = localStorage.getItem("bodyHeight");
            localStorage.setItem("succ", 1);
            appcan.window.openPopover({
                name : 'succ',
                dataType : 0,
                url : "../pop/succ.html",
                top : 0,
                left : 0,
                width : bodyWidth,
                height : bodyHeight,
            });

        }

    });

    // 拦截返回按钮
    uexWindow.setReportKey(0, 1);
    // 拦截后的事件绑定
    uexWindow.onKeyPressed = function(keyCode) {
        if (keyCode == 0) {
            //alert('点击了返回键')
            var hbvip = localStorage.getItem("hbvip");
            if (hbvip == 1 || hbvip == '1') {
                appcan.window.closePopover('hbvip');
                localStorage.setItem("hbvip", 0);
            } else {
                appcan.window.close(-1);
            }

        } else {
            alert('点击了其他键')
        }
    }
});

$(window).scroll(function() {
    var scrollTop = $(window).scrollTop();
    if (scrollTop > 30) {
        var opt = (scrollTop - 30) / 200;
        if (opt > 0.9) {
            opt = 0.9;
        }
        $(".headbg").css({
            'background-color' : "rgba(255,66,98," + opt + ")",
        });
    }
    if (scrollTop < 30) {
        $(".headbg").css({
            'background-color' : "rgba(255,66,98,0)",
        });
    }
});

$('.empty-view').click(function() {

    appcan.window.close(-1);

});

$('.close').click(function() {

    appcan.window.close(-1);

});

$('.knock').click(function() {

    //openToast("正在领取福包", 60000, 5, 1);
    clickRedPacket();

});

function randomWishes() {

    var num = '';
    var rslt = parseInt(10 * Math.random());
    if (rslt == 10) {
        num = 9;
    } else {
        num = rslt;
    }
    return num;

}

//1.接口8: 获取红包详情
function clickRedPacket() {

    var temp = localStorage.getItem("redPacketId");
    var number = temp.substring(0, temp.lastIndexOf('_&_'));
    var RID = temp.substring(temp.lastIndexOf('_&_') + 3);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "RedPacket/ClickRedPacket",
        type : "POST",
        data : {
            RID : RID,
            number : number,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            //debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    $('.fuwa-bg').removeClass('no-dis');
                    var volume = uexDevice.getVolume();
                    uexDevice.setVolume(0.25);
                    uexAudio.open("res://gongxifacai.mp3");
                    uexAudio.play(0);
                    uexDevice.setVolume(volume);

                    var name = new Date().format("yyyyMMddhhmmssSS");

                    appcan.window.publish('removeRedPacket', 0);
                    localStorage.setItem("fromPage", 'hbknock');
                    //openToast("正在打开福包...", 3000, 5, 1);
                    setTimeout(function() {

                        var localPack = localStorage.getItem('localPack');
                        if (localPack == '1' || localPack == 1) {

                            appcan.window.open({
                                name : 'hbadvert',
                                data : '../hb/hbadvert.html',
                                aniId : 10,
                            });

                        } else {

                            uexWindow.open({
                                name : 'hbadvert',
                                data : '../hb/hbadvert.html',
                                animID : 10,
                                flag : 1024
                            });

                        }

                    }, 2500);

                } else if (data.code == -9992) {

                    localStorage.setItem("fromPage", 'hbknock');

                    localStorage.setItem("hbvip", 1);

                    localStorage.setItem("tip", data.msg);

                    openToast(data.msg, 5000, 5, 0);

                    setTimeout(function() {

                        var bodyWidth = localStorage.getItem("bodyWidth");
                        var bodyHeight = localStorage.getItem("bodyHeight");
                        appcan.window.openPopover({
                            name : 'hbvip',
                            dataType : 0,
                            url : "../hb/hbvip.html",
                            top : 0,
                            left : 0,
                            width : bodyWidth,
                            height : bodyHeight,
                        });

                    }, 2000);

                } else if (data.code == -9996) {//本人已经领过

                    openToast(data.msg, 5000, 5, 0);
                    appcan.window.publish('removeRedPacket', 0);
                    setTimeout(function() {
                        appcan.window.close(-1);
                    }, 2000);

                } else if (data.code == -9997) {//红包已经领取完毕

                    openToast(data.msg, 5000, 5, 0);
                    appcan.window.publish('removeRedPacket', 0);
                    setTimeout(function() {
                        appcan.window.close(-1);
                    }, 2000);

                }

            } else {

                openToast('领取福包失败', 5000, 5, 0);

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

/*
 * 在当前页面展示用户头像、用户名
 */
function getPacketUserInfo() {

    var uid = localStorage.getItem("uid");
    var temp = localStorage.getItem("redPacketId");
    var number = temp.substring(0, temp.lastIndexOf('_&_'));
    var RID = temp.substring(temp.lastIndexOf('_&_') + 3);
    var debugJson = {
        whole : temp,
        rid : RID,
        number : number,
    }
    debug(debugJson);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "RedPacket/GetRecPacketDetail",
        type : "POST",
        data : {
            RID : RID,
            Number : number,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    setPacketUserInfo(data);

                } else {

                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                openToast('获取福包主信息失败', 3000, 5, 0);

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
            openToast(localStorage.getItem("errorMsg"), 3000, 5, 0);

        },
    });

}

//2.1.设置红包详情数据
function setPacketUserInfo(PacketDetail) {

    var fileUrl = localStorage.getItem("fileUrl");

    var TrueName = '';
    var HeadImg1 = '';

    if (PacketDetail.model.TrueName == null || PacketDetail.model.TrueName == '' || PacketDetail.model.TrueName == undefined) {
        TrueName = PacketDetail.model.Phone;
    } else {
        TrueName = PacketDetail.model.TrueName;
    }

    HeadImg1 = '<div data-userid="' + PacketDetail.model.UID + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 knock-img uc-a2" style="background-image:url(' + fileUrl + PacketDetail.model.HeadImg1 + ');"></div>';

    $('#HeadImg1').children().remove();
    $('#HeadImg1').append(HeadImg1);
    $('#TrueName').text(TrueName);

}