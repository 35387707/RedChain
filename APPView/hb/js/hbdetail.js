//2.接口9 获取红包详情, 当接口8成功返回时, 就调用接口9
function getPacketDetail(fromPage) {

    openToast("正在获取福包详情", 60000, 5, 1);

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
            Pageindex : 1,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    localStorage.setItem("PacketDetail", JSON.stringify(data));

                    //获取红包评论的入口
                    //getPacketComment(uid, RID, number, data, 1, fromPage);

                    //savePacketBegin(uid, RID, number, data, 1, fromPage);
                    getPacketComment(uid, RID, number, data, 1, fromPage);

                } else {

                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                openToast('获取福包详情失败', 3000, 5, 0);

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

//接口.2.0.
function refreshPacketDetail(fromPage) {

    openToast("正在获取福包详情", 60000, 5, 1);

    var uid = localStorage.getItem("uid");
    var number = $('#content').data('number');
    var RID = $('#content').data('rid');
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
            Pageindex : 1,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    localStorage.setItem("PacketDetail", JSON.stringify(data));

                    //获取红包评论的入口
                    getPacketComment(uid, RID, number, data, 1, fromPage);
                    //savePacketBegin(uid, RID, number, data, 1, fromPage);

                } else {

                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                openToast('获取福包详情失败', 3000, 5, 0);

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
function setPacketDetail(uid, RID, number, PacketDetail, PacketComment, PageIndex, actnJson, fromPage) {

    appcan.window.publish('hbknock', 1);

    $('#content').data('rid', RID);
    $('#content').data('number', number);
    $('#content').data('from_page', fromPage);

    closeToast();

    var fileUrl = localStorage.getItem("fileUrl");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var imglist = '';
    var title = '';
    var TrueName = '';
    var HeadImg1 = '';
    var reccount = '';
    //真正的领取多少份的值
    var ViewCount = '';
    //阅读数量
    var GoodCount = '';
    var GetMoney = '';
    var reclist = '';
    var CreateTime = '';

    var redGetMine = '';
    var totalPrice = '';

    var indx = '';

    reccount = PacketDetail.reccount;

    if (PacketDetail.model.TrueName == null || PacketDetail.model.TrueName == '' || PacketDetail.model.TrueName == undefined) {
        TrueName = PacketDetail.model.Phone + '的福包';
    } else {
        TrueName = PacketDetail.model.TrueName + '的福包';
    }

    HeadImg1 = '<div data-userid="' + PacketDetail.model.UID + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 red-wd icon-cont uc-a2 red-host" style="background-image:url(' + fileUrl + PacketDetail.model.HeadImg1 + ');"></div>';
    //<div class="ub-img red-wd icon-cont"></div>

    var timeArea = getDateDiff(timeStampFormat(PacketDetail.model.CreateTime), curTime, 'day');
    var timeJson = timeStampSplit(timeStampFormat(PacketDetail.model.CreateTime));
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
    title += PacketDetail.model.title;
    title += '</a>';
    title += '<a class="content">';
    title += '';
    title += '</a>';
    title += '<a class="link">';
    title += '';
    title += '</a>';

    var imgArr = new Array();
    imgArr = PacketDetail.model.imglist.split(";");
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

    ViewCount = PacketDetail.model.ViewCount;
    GoodCount = PacketDetail.model.GoodCount;

    totalPrice = PacketDetail.model.TotalPrice;
    
    debug(PacketDetail.reclist);

    $(PacketDetail.reclist).each(function(l, z) {

        //reclist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img alrdy-wd alrdy-marg-right" style="background-image:url(' + fileUrl + z.HeadImg1 + ');"></div>';
        reclist += '<div class="li-img-style">';
        reclist += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1" style="background-image:url(' + fileUrl + z.HeadImg1 + ');"></div>';
        reclist += '</div>';

        /*
         if (localStorage.getItem("uid") == z.ID) {

         redGetMine = z.Money;

         }*/

    });

    if (number == '' || number == null || number == undefined || number == NaN) {
        redGetMine = '';
    } else {
        redGetMine = PacketDetail.model.GetMoney;
    }

    if (redGetMine == '') {
        GetMoney += '<a class="cash-total">总额:</a>';
        GetMoney += '<a class="cash-nope" id="GetMoney">';
        GetMoney += totalPrice;
        GetMoney += '</a>';
        GetMoney += '<a class="cash-unit">元</a>';
    } else {
        GetMoney += '<a class="cash-num" id="GetMoney">';
        GetMoney += redGetMine;
        GetMoney += '</a>';
        GetMoney += '<a class="cash-unit">元</a>';
    }

    $('#HeadImg1').children().remove();
    $('#HeadImg1').append(HeadImg1);

    $('#TrueName').text(TrueName);
    $('.money-sty').children().remove();
    $('.money-sty').append(GetMoney);

    $('#reccount').html(reccount);

    $('#ViewCount').html(ViewCount);
    $('#reclist').children().remove();
    $(reclist).appendTo('#reclist');

    $('#title').children().remove();
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

    $('.red-host').unbind();
    $('.red-host').click(function() {

        var uid = localStorage.getItem("uid");

        var userid = $(this).data('userid');

        debug(userid);

        if (uid == userid) {

            openToast('-_-|||，这个是你本人啊', 3000, 5, 0);

        } else {

            localStorage.setItem("userid", userid);

            localStorage.setItem("fromPage", 'hbdetail');

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

    setPacketComment(uid, RID, number, PacketComment, PageIndex, fromPage);

}


$('.reclist').click(function() {

    var rid = $('#content').data('rid');
    var redPacketId = '_&_' + rid;
    localStorage.setItem("redPacketId", redPacketId);

    var name = new Date().format("yyyyMMddhhmmssSS");

    var localPack = localStorage.getItem('localPack');
    if (localPack == '1' || localPack == 1) {

        appcan.window.open({
            name : name,
            data : '../hb/hbreceived.html',
            aniId : 10,
        });

    } else {

        uexWindow.open({
            name : name,
            data : '../hb/hbreceived.html',
            animID : 10,
            flag : 1024
        });

    }

});
