/*
 * 接口.44.获取广告详情
 */
function getViewInfo(fromPage) {

    var uid = localStorage.getItem("uid");
    var IID = localStorage.getItem("iid");

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Infomation/ViewInfo",
        type : "POST",
        data : {
            IID : IID,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")
            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {
                    
                    debug(data.IsFirstTime);

                    //setInfomationList(data, PageIndex, fromPage);
                    localStorage.setItem("IsFirstTime", data.IsFirstTime);
                    localStorage.setItem("ViewInfo", JSON.stringify(data));
                    getComment(uid, IID, data, 1, fromPage);

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

/*
 * 接口.44.1.设置广告详情数据
 */
function setViewInfo(uid, iid, ViewInfo, Comment, PageIndex, actnJson, fromPage) {
    
    $('#content').data('iid', iid);

    //IsFirstTime

    closeToast();

    var fileUrl = localStorage.getItem("fileUrl");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var imglist = '';
    var title = '';
    var TrueName = '';
    var Descrition = '';
    var HeadImg1 = '';
    //真正的领取多少份的值
    var ViewCount = '';
    //阅读数量
    var GoodCount = '';
    var GetMoney = '';
    var CreateTime = '';

    var indx = '';

    if (ViewInfo.model.TrueName == null || ViewInfo.model.TrueName == '' || ViewInfo.model.TrueName == undefined) {
        TrueName = ViewInfo.model.Phone;
    } else {
        TrueName = ViewInfo.model.TrueName;
    }

    if (ViewInfo.model.Descrition == '' || ViewInfo.model.Descrition == null || ViewInfo.model.Descrition == undefined) {
        Descrition = '这家伙很懒, 暂时还没有签名...';
    } else {
        Descrition = ViewInfo.model.Descrition;
    }

    HeadImg1 = '<div data-userid="' + ViewInfo.model.UID + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 comt-icon-width biz-host" style="background-image:url(' + fileUrl + ViewInfo.model.HeadImg1 + ');"></div>';
    //<div class="ub-img red-wd icon-cont"></div>

    var timeArea = getDateDiff(timeStampFormat(ViewInfo.model.CreateTime), curTime, 'day');
    var timeJson = timeStampSplit(timeStampFormat(ViewInfo.model.CreateTime));
    if (timeArea == 0 || timeArea == '0') {
        if (curJson.day - timeJson.day == 1) {
            CreateTime += '昨天';
            CreateTime += ' ';
        }
    } else if (timeArea == 1 || timeArea == '1') {
        if (curJson.day - timeJson.day == 2) {
            CreateTime += timeJson.month;
            CreateTime += '月';
            CreateTime += timeJson.day;
            CreateTime += '日';
            CreateTime += ' ';
        } else if (curJson.day - timeJson.day == 1) {
            CreateTime += '昨天';
            CreateTime += ' ';
        }
    } else {
        CreateTime += timeJson.month;
        CreateTime += '月';
        CreateTime += timeJson.day;
        CreateTime += '日';
        CreateTime += ' ';
    }

    CreateTime += timeJson.quantum;
    CreateTime += timeJson.hour;
    CreateTime += ':';
    CreateTime += timeJson.minute;

    // /Date();

    title += '<a class="title">';
    title += ViewInfo.model.title;
    title += '</a>';
    title += '<a class="content">';
    title += '';
    title += '</a>';
    title += '<a class="link">';
    title += '';
    title += '</a>';

    var imgArr = new Array();
    imgArr = ViewInfo.model.imglist.split(";");
    debug(imgArr);
    debug(imgArr.length);
    $(imgArr).each(function(k, r) {

        if (imgArr.length == 1) {

            if (imgArr[imgArr.length - 1] == '' || imgArr[imgArr.length - 1] == null || imgArr[imgArr.length - 1] == undefined) {

            } else {
                //imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-100 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '<div class="img-width-100 disp-pic" data-pic_p="' + fileUrl + r + '">';
                imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-100" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '</div>';
            }

        } else if (imgArr.length == 2) {

            if (imgArr[imgArr.length - 1] == '' || imgArr[imgArr.length - 1] == null || imgArr[imgArr.length - 1] == undefined) {

                if (k + 1 != imgArr.length) {
                    //imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-100 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                    imglist += '<div class="img-width-100 disp-pic" data-pic_p="' + fileUrl + r + '">';
                    imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-100" style="background-image:url(' + fileUrl + r + ');"></div>';
                    imglist += '</div>';
                }

            } else {
                //imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '<div class="img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '">';
                imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-50" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '</div>';
            }

        } else if (imgArr.length == 3) {

            if (imgArr[imgArr.length - 1] == '' || imgArr[imgArr.length - 1] == null || imgArr[imgArr.length - 1] == undefined) {
                //imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '<div class="img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '">';
                imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-50" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '</div>';
            } else {
                //imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '<div class="img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '">';
                imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-33" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '</div>';
            }

        } else if (imgArr.length == 4) {

            if (imgArr[imgArr.length - 1] == '' || imgArr[imgArr.length - 1] == null || imgArr[imgArr.length - 1] == undefined) {
                //imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '<div class="img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '">';
                imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-33" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '</div>';
            } else {
                //imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '<div class="img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '">';
                imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-50" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '</div>';
            }

        } else if (imgArr.length == 5) {

            if (imgArr[imgArr.length - 1] == '' || imgArr[imgArr.length - 1] == null || imgArr[imgArr.length - 1] == undefined) {
                //imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '<div class="img-width-50 disp-pic" data-pic_p="' + fileUrl + r + '">';
                imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-50" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '</div>';
            } else {
                //imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '<div class="img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '">';
                imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-33" style="background-image:url(' + fileUrl + r + ');"></div>';
                imglist += '</div>';
            }

        } else {

            //imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1  img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';
            imglist += '<div class="img-width-33 disp-pic" data-pic_p="' + fileUrl + r + '">';
            imglist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 img-sty-33" style="background-image:url(' + fileUrl + r + ');"></div>';
            imglist += '</div>';

        }

    });

    ViewCount = ViewInfo.model.ViewCount;
    GoodCount = ViewInfo.model.GoodCount;

    $('#HeadImg1').children().remove();
    $('#HeadImg1').append(HeadImg1);

    $('#TrueName').text(TrueName);
    
    $('#Descrition').text(Descrition);

    $('#ViewCount').html(ViewCount);

    $('#title').append(title);
    
    $('#imglist').children().remove();
    $(imglist).appendTo('#imglist');

    $('#GoodCount').html(GoodCount);

    $('#CreateTime').html(CreateTime);

    if (actnJson.collcetion == 1 || actnJson.collcetion == '1') {

        indx = '收藏';
        $("div[data-chg_i='" + indx + "']").addClass('collc-icon-click');
        $("div[data-chg_t='" + indx + "']").addClass('little-red');
        $("div[data-chg_s='" + indx + "']").data('cid', actnJson.cid);

    }
    if (actnJson.good == 1 || actnJson.good == '1') {

        indx = '点赞';
        $("div[data-chg_i='" + indx + "']").addClass('good-icon-click');
        $("div[data-chg_t='" + indx + "']").addClass('little-red');

    }
    if (actnJson.share == 1 || actnJson.share == '1') {

        indx = '分享';
        $("div[data-chg_i='" + indx + "']").addClass('share-icon-click');
        $("div[data-chg_t='" + indx + "']").addClass('little-red');

    }

    $('.biz-host').unbind();
    $('.biz-host').click(function() {

        var uid = localStorage.getItem("uid");

        var userid = $(this).data('userid');

        debug(userid);

        if (uid == userid) {

            openToast('-_-|||，这个是你本人啊', 3000, 5, 0);

        } else {

            localStorage.setItem("userid", userid);

            localStorage.setItem("fromPage", 'adv');

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

    //debug(comtList);

    $('.disp-pic').unbind();
    $('.disp-pic').click(function() {

        var picIndx = $(this).data('pic_p');

        //查看所有相关的图片
        var picArr = [];
        $('.disp-pic').each(function(j, s) {

            //debug($(this).data('pic_p'));
            //debug($(this).data('img_s'));

            if ($(this).data('pic_p')) {
                var picSgl = {
                    src : $(this).data('pic_p'),
                    desc : '',
                };
                picArr.push(picSgl);
            }

        });

        localStorage.setItem("imgSgl", picIndx);
        localStorage.setItem("imgArr", JSON.stringify(picArr));
        setTimeout(function() {

            openBrowser();

        }, 300);

    });

    setComment(uid, iid, Comment, PageIndex, fromPage);

}
