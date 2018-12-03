//接口.26.获取用户所发出的所有红包列表
function getSendPacketList(PageIndex, ID, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "RedPacket/GetSendPacketList",
        type : "POST",
        data : {
            PageIndex : PageIndex,
            ID : ID,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    setSendPacketList(data, PageIndex, fromPage);

                } else {

                    openToast('获取可取红包失败', 3000, 5, 0);

                }

            } else {

                openToast('获取可取红包失败', 3000, 5, 0);

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

//接口.26.1.迭代抢到红包数据
function setSendPacketList(json, PageIndex, fromPage) {
    
    if (fromPage == 'page_other') {
        appcan.window.publish('otherJudgCond', 1);
    }

    //debug('PageIndex: ' + PageIndex);
    debug(json);

    var fileUrl = localStorage.getItem("fileUrl");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var listStr = '';
    var lastStr = '';

    var SendReds = 0;
    var RecReds = 0;

    if (json.SendReds == '' || json.SendReds == null || json.SendReds == undefined) {
        SendReds = 0;
    } else {
        SendReds = json.SendReds;
    }

    if (json.RecReds == '' || json.RecReds == null || json.RecReds == undefined) {
        RecReds = 0;
    } else {
        RecReds = json.RecReds;
    }

    //localStorage.setItem("pagecount", json.pagecount);
    $('#GetSendPacketList').data('pageindex', PageIndex);
    $('#GetSendPacketList').data('pagecount', json.pagecount);

    if (PageIndex == json.pagecount) {

        lastStr += '<div class="end-nope-number">';
        lastStr += '没有更多了';
        lastStr += '</div>';

    }

    if (json.list.length == 0) {

        listStr += '<div class="end-nope-number">';
        listStr += '当前没有数据';
        listStr += '</div>';

        openToast('当前没有数据', 3000, 5, 0);

    } else {

        debug(json.list);

        $(json.list).each(function(j, s) {

            var imgArr = new Array();
            imgArr = s.imglist.split(";");

            listStr += '<div data-rid="' + s.RID + '" class="ub ub-ver collc-sty red-sub">';

            listStr += '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub t-bla ub-ac collc-padd c-wh">';

            var timeJson = pageTimeStampSplit(timeStampFormat(s.CreateTime));

            listStr += '<li class="ub-f1 ut-s ulev-app1 collc-right-wd">';
            listStr += '<a class="other-day">';
            listStr += timeJson.day;
            listStr += '</a>';

            listStr += '<a class="other-month">';
            listStr += timeJson.month;
            listStr += '月';
            listStr += '</a>';
            listStr += '</li>'

            listStr += '<li>';
            listStr += '<a class="collc-whole">'
            listStr += timeJson.quantum;
            listStr += timeJson.hour;
            listStr += ':';
            listStr += timeJson.minute;
            listStr += '</a>';
            listStr += '</li>'

            listStr += '</ul>';

            listStr += '<div class="ub c-wh">';
            listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img collc-img collc-wh umar-a" style="background-size: cover; background-image:url(' + fileUrl + imgArr[0] + ');"></div>';
            listStr += '<div class="ub-f1 collc-cont">';
            listStr += '<div class="collc-txt">';
            listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="txt-sty txt-overflow">';
            listStr += s.title;
            listStr += '</div>';
            listStr += '</div>';

            listStr += '<div class="ub collc-actn">';

            listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub ub-ac">';
            listStr += '<div class="ub ub-img1 item-width read-red"></div>';
            listStr += '<div class="item-txt ufm1">';
            listStr += s.ViewCount;
            listStr += '</div>';
            listStr += '</div>';

            /*
            listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub ub-ac item-marg">';
            listStr += '<div class="ub ub-img1 item-width msg-red"></div>';
            listStr += '<div class="item-txt ufm1">';
            listStr += '0';
            listStr += '</div>';
            listStr += '</div>';*/


            listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub ub-ac item-marg">';
            listStr += '<div class="ub ub-img1 item-width good-red"></div>';
            listStr += '<div class="item-txt ufm1">';
            listStr += s.GoodCount;
            listStr += '</div>';
            listStr += '</div>';

            listStr += '</div>';

            listStr += '</div>';
            listStr += '</div>';
            listStr += '</div>';
            listStr += '</div>';

        });

        listStr += lastStr;

        openToast('加载数据成功', 3000, 5, 0);

    }

    $('#RecReds').text(RecReds);
    $('#SendReds').text(SendReds);

    $('#GetSendPacketList').append(listStr);

    appcan.frame.resetBounce(1);

    $('.red-sub').unbind();
    $('.red-sub').click(function() {

        var rid = $(this).data('rid');

        localStorage.setItem("fromPage", fromPage);

        var redPacketId = '_&_' + rid;
        localStorage.setItem("redPacketId", redPacketId);
        
        var name = new Date().format("yyyyMMddhhmmssSS");

        openToast('查看详情中', 3000, 5, 1);

        setTimeout(function() {

            var localPack = localStorage.getItem('localPack');
            if (localPack == '1' || localPack == 1) {

                appcan.window.open({
                    name : name,
                    data : '../hb/hbdetail.html',
                    aniId : 10,
                });

            } else {

                uexWindow.open({
                    name : name,
                    data : '../hb/hbdetail.html',
                    animID : 10,
                    flag : 1024
                });

            }

        }, 1000);

    });

}

//接口.26.2.判断当前的页码为多少
//pagecount
function judgCurPage(item, fromPage) {

    var curpage = '';
    //$('.photo-album').children("div:last-child").index()
    var pagecount = parseInt($('#GetSendPacketList').data('pagecount'));
    var pageindex = parseInt($('#GetSendPacketList').data('pageindex'));
    curpage = $('#GetSendPacketList').children("div:last-child").index() + 1;

    /*
     debug('curpage: ' + curpage);
     debug('parseInt(curpage): ' + parseInt(curpage));
     debug('pagecount: ' + pagecount);*/

    if (curpage == 1) {

        appcan.frame.resetBounce(1);
        openToast('当前没有数据', 3000, 5, 0);

    } else {

        if (curpage < item) {

            appcan.frame.resetBounce(1);
            openToast('所有的数据已经加载完成', 3000, 5, 0);

        } else {

            if (pageindex == pagecount) {

                appcan.frame.resetBounce(1);
                openToast('所有的数据已经加载完成', 3000, 5, 0);

            } else {

                if (pageindex < pagecount) {

                    var ID = '';
                    if (fromPage == 'page_mine') {
                        ID = localStorage.getItem("uid");
                    } else if (fromPage == 'page_other') {
                        ID = localStorage.getItem("userid");
                    } else if (fromPage == 'page_other_refresh') {
                        ID = localStorage.getItem("userid");
                    }

                    getSendPacketList(pageindex + 1, ID, fromPage);

                }

            }

        }

    }

}