$('#Comment').click(function() {

    var Content = trim($('#Content').val());
    if (Content == '') {

        openToast("请输入您的评论", 3000, 5, 0);

    } else {

        //ajaxPacketComment(Content);
        appcan.window.publish('ajaxPacketComment', Content);

    }

});

//4.0.提交红包评论
function ajaxPacketComment(Content) {

    openToast("正在提交评论", 60000, 5, 1);

    var uid = localStorage.getItem("uid");
    
    var number = $('#content').data('number');
    var RID = $('#content').data('rid');
    
    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "RedPacket/Comment",
        type : "POST",
        data : {
            RID : RID,
            Number : number,
            Content : Content,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {
                    
                    //localStorage.setItem("fromPage", 'hbcomment');

                    openToast('评论成功', 3000, 5, 0);
                    //提交成功之后刷新列表
                    getPacketComment(uid, RID, number, '', 1, 'refresh_comment');

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('评论失败', 3000, 5, 0);

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

//接口.17.2.判断当前的页码为多少
//pagecount
function judgPacketCommentCurPage(uid, item, fromPage) {

    var number = $('#content').data('number');
    var RID = $('#content').data('rid');

    var curpage = '';
    //$('.photo-album').children("div:last-child").index()
    var pagecount = $('#PacketComment').data('pagecount');
    var pageindex = $('#PacketComment').data('pageindex');
    curpage = $('#PacketComment').children("div:last-child").index() + 1;

    /*
     debug('curpage: ' + curpage);
     debug('parseInt(curpage): ' + parseInt(curpage));
     debug('pagecount: ' + pagecount); */

    if (curpage == 1) {

        appcan.frame.resetBounce(1);
        openToast('当前没有评论', 3000, 5, 0);

    } else {

        if (curpage < item) {

            appcan.frame.resetBounce(1);
            openToast('所有的评论已经加载完成', 3000, 5, 0);

        } else {

            if (pageindex == pagecount) {

                appcan.frame.resetBounce(1);
                openToast('所有的评论已经加载完成', 3000, 5, 0);

            } else {

                if (pageindex < pagecount) {

                    getPacketComment(uid, RID, number, '', pageindex + 1, fromPage);

                }

            }

        }

    }

}

//4.1.获取全部红包评论 评论数字限制在500字以内
function getPacketComment(uid, RID, number, PacketDetail, PageIndex, fromPage) {

    //openToast("正在获取评论列表", 60000, 5, 1);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "RedPacket/GetComment",
        type : "POST",
        data : {
            RID : RID,
            Number : number,
            PageIndex : PageIndex,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {
                //执行各种放数据的方法

                if (data.code == 1 || data.code == '1') {

                    //setPacketDetail(uid, RID, number, PacketDetail, data, fromPage);
                    if (fromPage == 'hbdetail' || fromPage == 'refresh_hbdetail') {
                        isActnBegin(uid, RID, number, PacketDetail, data, PageIndex, fromPage);
                    } else {
                        setPacketComment(uid, RID, number, data, PageIndex, fromPage);
                    }

                } else {

                    openToast('获取福包评论失败', 3000, 5, 0);

                }

            } else {

                openToast('获取福包评论失败', 3000, 5, 0);

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

//2.2.设置红包评论数据
function setPacketComment(uid, RID, number, PacketComment, PageIndex, fromPage) {

    if (fromPage == 'refresh_comment') {
        appcan.window.publish('cleanComment', RID);
    }

    var fileUrl = localStorage.getItem("fileUrl");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var comtList = '';
    var lastStr = '';

    if (PageIndex == PacketComment.pagecount) {

        lastStr += '<div class="end-nope-number">';
        lastStr += '没有更多了';
        lastStr += '</div>';
        //PacketCommentPageCount

    }

    $('#PacketComment').data('pageindex', PageIndex);
    $('#PacketComment').data('pagecount', PacketComment.pagecount);

    //localStorage.setItem("PacketCommentPageCount", PacketComment.pagecount);

    if (PacketComment.pagecount == '' || PacketComment.pagecount == null || PacketComment.pagecount == undefined) {

        lastStr += '<div class="end-nope-number">';
        lastStr += '没有更多了';
        lastStr += '</div>';

    }

    if (PacketComment.list.length == 0) {

        comtList += '<div class="end-nope-number">';
        comtList += '当前没有评论';
        comtList += '</div>';

    } else {

        $(PacketComment.list).each(function(l, z) {

            comtList += '<div class="comt-padd c-wh" data-truename="';
            if (z.TrueName == '' || z.TrueName == null || z.TrueName == undefined) {
                comtList += z.Phone;
            } else {
                comtList += z.TrueName;
            }
            comtList += '" ';
            comtList += 'data-userid="';
            comtList += z.UID;
            comtList += '" ';
            comtList += 'data-username="';
            comtList += z.Name;
            comtList += '" ';
            comtList += 'data-userphone="';
            comtList += z.Phone;
            comtList += '">';
            comtList += '<div class="ub ubb bc-border border-padd-bottom">';
            comtList += '<div data-userid="' + z.UID + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 comt-icon-width judg-sub mar-ar1" style="background-image:url(' + fileUrl + z.HeadImg1 + ');"></div>';
            comtList += '<div class="ub ub-ver ub-f1 mar-ar1">';
            comtList += '<div data-userid="' + z.UID + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="user-hdr judg-sub">';
            if (z.TrueName == '' || z.TrueName == null || z.TrueName == undefined) {
                //comtList += localStorage.getItem("username");
                comtList += z.Phone;
            } else {
                comtList += z.TrueName;
            }
            comtList += '</div>';
            comtList += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ut-s user-comt user-comt-marg-top">';
            comtList += z.Content;
            comtList += '</div>';
            comtList += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ut-s user-time user-time-marg-top">';

            //comtList += timeStampFormat(z.UpdateTime);
            var timeArea = getDateDiff(timeStampFormat(z.UpdateTime), curTime, 'day');
            var timeJson = timeStampSplit(timeStampFormat(z.UpdateTime));
            if (timeArea == 0 || timeArea == '0') {
                if (curJson.day - timeJson.day == 1) {
                    comtList += '昨天';
                    comtList += ' ';
                }
            } else if (timeArea == 1 || timeArea == '1') {
                if (curJson.day - timeJson.day == 2) {
                    comtList += timeJson.month;
                    comtList += '月';
                    comtList += timeJson.day;
                    comtList += '日';
                    comtList += ' ';
                } else if (curJson.day - timeJson.day == 1) {
                    comtList += '昨天';
                    comtList += ' ';
                }
            } else {
                comtList += timeJson.month;
                comtList += '月';
                comtList += timeJson.day;
                comtList += '日';
                comtList += ' ';
            }

            comtList += timeJson.quantum;
            comtList += timeJson.hour;
            comtList += ':';
            comtList += timeJson.minute;

            comtList += '</div>';
            comtList += '</div>';

            //comtList += '<div class="ub ub-ver ub-ae ub-pe">';
            //comtList += '<div class="tx-r umar-t">';
            //comtList += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub ub-ac">';
            //comtList += '<div class="ub ub-img judg-icon-width msg-icon-gray judg-marg-right"></div>';
            //comtList += '<div class="ub ub-img judg-icon-width good-icon-gray"></div>';
            //comtList += '<div class="judg-txt ufm1">';
            //comtList += '赞';
            //comtList += '</div>';

            comtList += '</div>';
            comtList += '</div>';
            comtList += '</div>';

            comtList += '</div>';
            comtList += '</div>';

        });

        comtList += lastStr;

    }

    if (fromPage == 'refresh_comment' || fromPage == 'refresh_hbdetail') {
        appcan.frame.resetBounce(0);
        $('#PacketComment').children().remove();
    } else {
        appcan.frame.resetBounce(1);
    }
    $('#PacketComment').append(comtList);

    $('.fa-commenting-o').unbind();
    $('.fa-commenting-o').click(function() {

        openToast('开始聊天中', 3000, 5, 1);

    });

    $('.judg-sub').unbind();
    $('.judg-sub').click(function() {

        var uid = localStorage.getItem("uid");

        var userid = $(this).data('userid');
        
        debug(userid);

        if (uid == userid) {

            openToast('-_-|||，这个是你本人啊', 3000, 5, 0);

        } else {
            
            var name = new Date().format("yyyyMMddhhmmssSS");
            
            localStorage.setItem("userid", userid);

            localStorage.setItem("fromPage", 'hbdetail');

            var localPack = localStorage.getItem('localPack');
            if (localPack == '1' || localPack == 1) {

                appcan.window.open({
                    name : name,
                    data : '../other/page_other.html',
                    aniId : 10,
                });

            } else {

                uexWindow.open({
                    name : name,
                    data : '../other/page_other.html',
                    animID : 10,
                    flag : 1024
                });

            }

        }

    });

    /*
    var lastPage = localStorage.getItem("fromPage");
        if (lastPage == 'hbknock') {
    
            var localPack = localStorage.getItem('localPack');
            if (localPack == '1' || localPack == 1) {
    
                appcan.window.open({
                    name : 'hbadvert',
                    data : '../hb/hbadvert.html',
                    aniId : 10,
                });
    
            } else {
    
                uexWindow.open({
                    name : "hbadvert",
                    data : "../hb/hbadvert.html",
                    animID : 10,
                    flag : 1024
                });
    
            }
    
        }*/
    

}