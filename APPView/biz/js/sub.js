/*
 * 接口.43.获取广告列表(分页返回，每页20条)
 */
function getInfomationList(PageIndex, type, fromPage) {

    debug(type);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Infomation/GetList",
        type : "POST",
        data : {
            PageIndex : PageIndex,
            type : type,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")
            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    setInfomationList(data, PageIndex, fromPage);

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('获取商圈数据失败', 3000, 5, 0);

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

//接口.43.0.判断当前的页码为多少
//pagecount
function judgCurPage(item, type, fromPage) {

    var curpage = '';
    //$('.photo-album').children("div:last-child").index()
    var pagecount = parseInt($('#content').data('pagecount'));
    var pageindex = parseInt($('#content').data('pageindex'));

    curpage = $('#content').children("div:last-child").index() + 1;

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

                    getInfomationList(pageindex + 1, type, fromPage);

                }

            }

        }

    }

}

//接口.43.1.迭代商圈列表数据
function setInfomationList(json, PageIndex, fromPage) {

    var fileUrl = localStorage.getItem("fileUrl");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    //debug(json);

    var content = '';
    var lastStr = '';

    $('#content').data('pageindex', PageIndex);
    $('#content').data('pagecount', json.pagecount)

    if (PageIndex == json.pagecount) {

        lastStr += '<div class="end-nope-number">';
        lastStr += '没有更多了';
        lastStr += '</div>';

    }

    if (json.list.length == 0) {

        content += '<div class="end-nope-number">';
        content += '当前没有数据';
        content += '</div>';

        openToast('当前没有数据', 3000, 5, 0);

    } else {

        $(json.list).each(function(j, s) {

            var imgArr = new Array();
            imgArr = s.imglist.split(";");

            //debug(imgArr);

            content += '<div class="ub ub-ver advert-sty">';

            content += '<div data-userid="' + s.UID + '" class="comt-marg c-wh usr-sub">';

            content += '<div class="ub">';

            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 comt-icon-width" style="background-image:url(' + fileUrl + s.HeadImg1 + ');"></div>';

            content += '<div class="ub ub-ver ub-f1 biz-user-info">';
            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="biz-user-hdr">';
            content += '<span class="info-overflow">';
            if (s.TrueName == '' || s.TrueName == null || s.TrueName == undefined) {
                content += s.Phone;
            } else {
                content += s.TrueName;
            }
            content += '</span>';
            content += '</div>';

            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="biz-user-time">';
            content += '<span class="info-overflow">';
            if (s.Descrition == '' || s.Descrition == null || s.Descrition == undefined) {
                content += '这家伙很懒，暂时还没有签名...';
            } else {
                content += s.Descrition;
            }
            content += '</span>';
            content += '</div>';

            content += '</div>';
            content += '</div>';
            content += '</div>';

            //广告内容
            content += '<div data-iid="' + s.IID + '" class="ub ub-ver biz-sub">';

            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="bc-text advert-title overflow">';

            content += '<a class="title">';
            content += s.title;
            content += '</a>';

            content += '<a class="link">';
            content += s.LinkTo;
            content += '</a>';

            content += '</div>';

            content += '<div class="img-list">';

            $(imgArr).each(function(k, r) {

                if (k <= 2) {

                    if (imgArr.length == 1) {

                        //content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-100 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                        content += '<div class="img-width-100 disp-pic" data-pic_p="' + fileUrl + r + '">';
                        content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-100" style="background-image:url(' + fileUrl + r + ');"></div>';
                        content += '</div>';

                    } else if (imgArr.length == 2) {

                        if (imgArr[imgArr.length - 1] == '' || imgArr[imgArr.length - 1] == null || imgArr[imgArr.length - 1] == undefined) {

                            if (k + 1 != imgArr.length) {
                                //content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-100 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                                content += '<div class="img-width-100 disp-pic" data-pic_p="' + fileUrl + r + '">';
                                content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-100" style="background-image:url(' + fileUrl + r + ');"></div>';
                                content += '</div>';
                            }

                        } else {
                            //content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                            content += '<div class="img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '">';
                            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-50" style="background-image:url(' + fileUrl + r + ');"></div>';
                            content += '</div>';
                        }

                    } else if (imgArr.length == 3) {

                        if (imgArr[imgArr.length - 1] == '' || imgArr[imgArr.length - 1] == null || imgArr[imgArr.length - 1] == undefined) {
                            //content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                            content += '<div class="img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '">';
                            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-50" style="background-image:url(' + fileUrl + r + ');"></div>';
                            content += '</div>';
                        } else {
                            //content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                            content += '<div class="img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '">';
                            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-33" style="background-image:url(' + fileUrl + r + ');"></div>';
                            content += '</div>';
                        }

                    } else {

                        //content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                        content += '<div class="img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '">';
                        content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-33" style="background-image:url(' + fileUrl + r + ');"></div>';
                        content += '</div>';

                    }

                }

            });

            content += '</div>';

            content += '<div class="ub ub-ac advert-remark-padd">';
            content += '<div class="ub ub-ac ub-f1 advert-remark-marg-right">';

            content += '<div class="advert-txt-sty read-cnt" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            content += s.ViewCount;
            content += '</div>';

            content += '<div class="advert-txt-sty" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            content += '访问量&nbsp;-&nbsp;';
            content += '</div>';

            content += '<div class="advert-txt-sty zan-cnt" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            content += s.GoodCount;
            content += '</div>';

            content += '<div class="advert-txt-sty" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            content += '赞';
            content += '</div>';

            content += '</div>';

            content += '<div class="ub ub-ac">';
            content += '<div class="advert-txt-sty advert-time" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';

            //content += '';
            var timeArea = getDateDiff(timeStampFormat(s.CreateTime), curTime, 'day');
            var timeJson = timeStampSplit(timeStampFormat(s.CreateTime));
            if (timeArea == 0 || timeArea == '0') {
                if (curJson.day - timeJson.day == 1) {
                    content += '昨天';
                    content += ' ';
                }
            } else if (timeArea == 1 || timeArea == '1') {
                if (curJson.day - timeJson.day == 2) {
                    content += timeJson.month;
                    content += '月';
                    content += timeJson.day;
                    content += '日';
                    content += ' ';
                } else if (curJson.day - timeJson.day == 1) {
                    content += '昨天';
                    content += ' ';
                }
            } else {
                content += timeJson.month;
                content += '月';
                content += timeJson.day;
                content += '日';
                content += ' ';
            }

            content += timeJson.quantum;
            content += timeJson.hour;
            content += ':';
            content += timeJson.minute;

            content += '</div>';
            content += '</div>';
            content += '</div>';

            //content += '<div data-rid="' + s.RID + '" data-cid="' + s.ID + '" class="ub ub-ac collc-div collc-del" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            //content += '<div class="ub ub-img collc-more-width collc-more-icon"></div>';
            //content += '</div>';

            content += '<div class="ub advert-actn-padd ub-ae">';

            content += '<div class="ub ub-ver ub-f1 ub-pc b-gra-per tab-sub">';
            content += '<div class="ub ub-ac" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            content += '<div class="ub ub-img little-icon-width collc-icon-gray"></div>';
            content += '<div class="little-txt ulev-2 ufm1">';
            content += '收藏';
            content += '</div>';
            content += '</div>';
            content += '</div>';

            content += '<div class="sign-border"></div>';

            content += '<div class="ub ub-ver ub-f1 ub-pc b-gra-per tab-sub">';
            content += '<div class="ub ub-ac" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            content += '<div class="ub ub-img little-icon-width share-icon-gray"></div>';
            content += '<div class="little-txt ulev-2 ufm1">';
            content += '分享';
            content += '</div>';
            content += '</div>';
            content += '</div>';

            content += '<div class="sign-border"></div>';

            content += '<div class="ub ub-ver ub-f1 ub-pc b-gra-per tab-sub">';
            content += '<div class="ub ub-ac" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            content += '<div class="ub ub-img little-icon-width good-icon-gray"></div>';
            content += '<div class="little-txt ulev-2 ufm1">';
            content += '点赞';
            content += '</div>';
            content += '</div>';
            content += '</div>';

            content += '</div>';
            content += '</div>';
            content += '</div>';

        });

        content += lastStr;

    }

    if (fromPage == 'reload' || fromPage == 'srch_sub') {
        $("#content").children().remove();
        appcan.frame.resetBounce(0);
        openToast('刷新数据成功', 3000, 5, 0);
    } else {
        appcan.frame.resetBounce(1);
        openToast('加载数据成功', 3000, 5, 0);
    }
    $('#content').append(content);

    appcan.frame.resetBounce(1);

    $('.usr-sub').unbind();
    $('.usr-sub').click(function() {

        var uid = localStorage.getItem("uid");

        var userid = $(this).data('userid');

        debug(userid);

        if (uid == userid) {

            openToast('-_-|||，这个是你本人啊', 3000, 5, 0);

        } else {
            
            var name = new Date().format("yyyyMMddhhmmssSS");

            localStorage.setItem("fromPage", 'sub');

            var name = new Date().format("yyyyMMddhhmmssSS");

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

    $('.biz-sub').unbind();
    $('.biz-sub').click(function() {

        var name = new Date().format("yyyyMMddhhmmssSS");

        var iid = $(this).data('iid');
        debug(iid);

        judgFirstTime(iid);

    });

}