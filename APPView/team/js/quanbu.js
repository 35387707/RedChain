/*
 * 接口.50.获取我的团队成员列表
 * 选填，如果为空则表示全部，0为普通会员，1为商家，2为代理
 */
function getMyRecommendList(pageindex, type, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Account/GetMyRecommendList",
        type : "POST",
        data : {
            pageindex : pageindex,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    setMyRecommendList(pageindex, data, fromPage);

                } else {

                    openToast('获取全部数据失败', 3000, 5, 0);

                }

            } else {

                openToast('获取全部数据失败', 3000, 5, 0);

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

//接口.50.0.判断当前的页码为多少
//pagecount
function judgCurPage(type, item, fromPage) {

    var curpage = '';
    //$('.photo-album').children("div:last-child").index()
    var pagecount = '';
    var pageindex = '';

    if (fromPage == 'quanbu') {

        curpage = $('#quanbu').children("div:last-child").index() + 1;
        pagecount = parseInt($('#quanbu').data("pagecount"));
        pageindex = parseInt($('#quanbu').data("pageindex"));

    } else if (fromPage == 'fuxing') {

        curpage = $('#fuxing').children("div:last-child").index() + 1;
        pagecount = parseInt($('#fuxing').data("pagecount"));
        pageindex = parseInt($('#fuxing').data("pageindex"));

    } else if (fromPage == 'fujiang') {

        curpage = $('#fujiang').children("div:last-child").index() + 1;
        pagecount = parseInt($('#fujiang').data("pagecount"));
        pageindex = parseInt($('#fujiang').data("pageindex"));

    } else if (fromPage == 'fuxiang') {

        curpage = $('#fuxiang').children("div:last-child").index() + 1;
        pagecount = parseInt($('#fuxiang').data("pagecount"));
        pageindex = parseInt($('#fuxiang').data("pageindex"));

    }

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

                    getMyRecommendList(pageindex + 1, type, fromPage);

                }

            }

        }

    }

}

//接口.50.1.迭代团队成员列表
function setMyRecommendList(pageindex, json, fromPage) {

    //debug('PageIndex: ' + PageIndex);

    var fileUrl = localStorage.getItem("fileUrl");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var listStr = '';
    var lastStr = '';

    if (fromPage == 'quanbu') {
        
        $('#quanbu').data("pageindex", pageindex);
        $('#quanbu').data("pagecount", json.pagecount);

    } else if (fromPage == 'fuxing') {

        $('#fuxing').data("pageindex", pageindex);
        $('#fuxing').data("pagecount", json.pagecount);

    } else if (fromPage == 'fujiang') {

        $('#fujiang').data("pageindex", pageindex);
        $('#fujiang').data("pagecount", json.pagecount);

    } else if (fromPage == 'fuxiang') {

        $('#fuxiang').data("pageindex", pageindex);
        $('#fuxiang').data("pagecount", json.pagecount);

    }

    if (pageindex == json.pagecount) {

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

        $(json.list).each(function(j, s) {

            listStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub ubb list-style">';

            listStr += '<div class="ub icon-style">';
            listStr += '<div class="ub-img1 icon-width" style="background-image:url(' + fileUrl + s.HeadImg1 + ');"></div>';
            listStr += '</div>';

            listStr += '<div class="icon-marg-right"></div>';

            listStr += '<div class="ub-f1 user-padd">';
            listStr += '<div class="nick-padd ub">';
            listStr += '<div class="user-nick overflow">';
            if (s.TrueName == '' || s.TrueName == null || s.TrueName == undefined) {
                listStr += s.Phone;
            } else {
                listStr += s.TrueName;
            }
            listStr += '</div>';

            if (s.UserType == 0) {
                listStr += '<div class="user-stat">';
                listStr += '<a class="stat-sty">福星 </a>';
                listStr += '</div>';
            } else if (s.UserType == 1) {
                listStr += '<div class="user-stat">';
                listStr += '<a class="stat-sty">福将 </a>';
                listStr += '</div>';
            } else if (s.UserType == 2) {
                listStr += '<div class="user-stat">';
                listStr += '<a class="stat-sty">福相 </a>';
                listStr += '</div>';
            }

            listStr += '</div>';

            listStr += '<div class="user-note overflow">';
            if (s.Descrition == '' || s.Descrition == null || s.Descrition == undefined) {
                listStr += '这家伙很懒, 暂时还没有签名...';
            } else {
                listStr += s.Descrition;
            }
            listStr += '</div>';

            listStr += '</div>';

            listStr += '<div class="ub ub-ver ub-ae ub-pe time-padd">';
            listStr += '<div class="right-time">'

            var timeArea = getDateDiff(timeStampFormat(s.UpdateTime), curTime, 'day');
            var timeJson = timeStampSplit(timeStampFormat(s.UpdateTime));
            if (timeArea == 0 || timeArea == '0') {
                if (curJson.day - timeJson.day == 1) {
                    listStr += '昨天';
                    listStr += ' ';
                }
            } else if (timeArea == 1 || timeArea == '1') {
                if (curJson.day - timeJson.day == 2) {
                    listStr += timeJson.month;
                    listStr += '月';
                    listStr += timeJson.day;
                    listStr += '日';
                    listStr += ' ';
                } else if (curJson.day - timeJson.day == 1) {
                    listStr += '昨天';
                    listStr += ' ';
                }
            } else {
                listStr += timeJson.month;
                listStr += '月';
                listStr += timeJson.day;
                listStr += '日';
                listStr += ' ';
            }

            listStr += timeJson.quantum;
            listStr += timeJson.hour;
            listStr += ':';
            listStr += timeJson.minute;

            listStr += '</div>';

            listStr += '</div>';

            listStr += '</div>';

        });

    }

    listStr += lastStr;

    if (fromPage == 'refresh_quanbu') {

        $('#quanbu').children().remove();
        appcan.frame.resetBounce(0);
        openToast('刷新数据成功', 3000, 5, 0);

    } else {

        appcan.frame.resetBounce(1);
        openToast('加载数据成功', 3000, 5, 0);

    }

    $('#quanbu').append(listStr);

}