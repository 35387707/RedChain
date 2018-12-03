/*
 * 接口.47.获取广告评论
 */
function getComment(uid, IID, ViewInfo, PageIndex, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Infomation/GetComment",
        type : "POST",
        data : {
            IID : IID,
            PageIndex : PageIndex,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")
            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    if (fromPage == 'adv') {

                        isActnBegin(uid, IID, ViewInfo, data, PageIndex, fromPage);

                    } else {

                        localStorage.setItem("IsFirstTime", 0);
                        setComment(uid, IID, data, PageIndex, fromPage);

                    }

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('获取广告评论失败', 3000, 5, 0);

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
function judgCommentCurPage(uid, iid, item, fromPage) {

    var curpage = '';
    //$('.photo-album').children("div:last-child").index()
    var pagecount = $('#Comment').data('pagecount');

    curpage = $('#Comment').children("div:last-child").index() + 1;

    /*
     debug('curpage: ' + curpage);
     debug('parseInt(curpage): ' + parseInt(curpage));
     debug('pagecount: ' + pagecount); */

    if (curpage == 1) {

        appcan.frame.resetBounce(1);
        openToast('当前没有数据', 3000, 5, 0);

    } else {

        if (curpage < item) {

            appcan.frame.resetBounce(1);
            openToast('所有的数据已经加载完成', 3000, 5, 0);

        } else {

            curpage = parseInt(curpage / item);

            if (curpage == pagecount) {

                appcan.frame.resetBounce(1);
                openToast('所有的数据已经加载完成', 3000, 5, 0);

            } else {

                if (curpage < pagecount) {

                    getComment(uid, iid, '', curpage + 1, fromPage);

                }

            }

        }

    }

}

//47.2.设置广告评论数据
function setComment(uid, iid, Comment, PageIndex, fromPage) {

    if (fromPage == 'refresh_comment') {
        appcan.window.publish('cleanBizComment', 1);
    }

    var fileUrl = localStorage.getItem("fileUrl");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var comtList = '';
    var lastStr = '';

    if (PageIndex == Comment.pagecount) {

        lastStr += '<div class="end-nope-number">';
        lastStr += '没有更多了';
        lastStr += '</div>';
        //CommentPageCount

    }

    $('#Comment').data('pagecount', Comment.pagecount);

    //localStorage.setItem("CommentPageCount", Comment.pagecount);

    if (Comment.pagecount == '' || Comment.pagecount == null || Comment.pagecount == undefined) {

        lastStr += '<div class="end-nope-number">';
        lastStr += '没有更多了';
        lastStr += '</div>';

    }

    if (Comment.list.length == 0) {

        comtList += '<div class="end-nope-number">';
        comtList += '当前没有数据';
        comtList += '</div>';

    } else {

        $(Comment.list).each(function(l, z) {

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
            comtList += '<div data-userid="' + z.UID + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="user-hdr judg-sub info-overflow">';
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

    if (fromPage == 'refresh_comment') {
        $('#Comment').children().remove();
    }
    $('#Comment').append(comtList);

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

            localStorage.setItem("fromPage", 'adv');

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
    var IsFirstTime = localStorage.getItem("IsFirstTime");
        if (IsFirstTime == 1 || IsFirstTime == '1') {
    
            var localPack = localStorage.getItem('localPack');
            if (localPack == '1' || localPack == 1) {
    
                appcan.window.open({
                    name : 'cvr',
                    data : '../biz/cvr.html',
                    aniId : 10,
                });
    
            } else {
    
                uexWindow.open({
                    name : "cvr",
                    data : "../biz/cvr.html",
                    animID : 10,
                    flag : 1024
                });
    
            }
    
        }*/
    

}


$('#AjaxComment').click(function() {

    var Content = trim($('#Content').val());
    if (Content == '') {

        openToast("请输入您的评论", 3000, 5, 0);

    } else {

        //ajaxPacketComment(Content);
        appcan.window.publish('ajaxComment', Content);

    }

});

//4.0.提交红包评论
function ajaxComment(Content) {

    openToast("正在提交评论", 60000, 5, 1);

    var uid = localStorage.getItem("uid");

    var IID = localStorage.getItem("iid");

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Infomation/Comment",
        type : "POST",
        data : {
            IID : IID,
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
                    //getComment(uid, IID, ViewInfo, PageIndex, fromPage)
                    getComment(uid, IID, '', 1, 'refresh_comment');

                } else {

                    openToast('评论失败', 3000, 5, 0);

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