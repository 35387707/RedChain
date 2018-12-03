//2.0.暂时保存sqlite的方法
function savePacketBegin(uid, RID, number, PacketDetail, PageIndex, fromPage) {

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        savePacketSrch(db, uid, RID, number, PacketDetail, PageIndex, fromPage, []);
    }

}

//2.0.0.查询的逻辑
function savePacketSrch(db, uid, RID, number, PacketDetail, PageIndex, fromPage, sql_tx) {

    var sql_sel = "select  * from tbl_packet_detail where rid = '" + RID + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            var sql = "";
            if (data.length == 0) {
                sql = "insert into tbl_packet_detail (rid, number, model, reccount, reclist) values ('" + RID + "', '" + number + "', '" + JSON.stringify(PacketDetail.model) + "', '" + PacketDetail.reccount + "', '" + JSON.stringify(PacketDetail.reclist) + "')";
            } else {
                sql = "update tbl_packet_detail set number='" + number + "', model='" + JSON.stringify(PacketDetail.model) + "', reccount='" + PacketDetail.reccount + "', reclist='" + JSON.stringify(PacketDetail.reclist) + "' where rid = '" + RID + "'";
            }
            sql_tx.push(sql);

            savePacketTx(db, uid, RID, number, PacketDetail, PageIndex, fromPage, sql_tx);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

//2.0.1.事务的逻辑
function savePacketTx(db, uid, RID, number, PacketDetail, PageIndex, fromPage, sql_tx) {

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            uexDataBaseMgr.close(db);
            getPacketComment(uid, RID, number, PacketDetail, PageIndex, fromPage);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

//2.1.0.查询数据
function srchPacketDetail() {

    var temp = localStorage.getItem("redPacketId");
    var number = temp.substring(0, temp.lastIndexOf('_&_'));
    var rid = temp.substring(temp.lastIndexOf('_&_') + 3);

    var db = uexDataBaseMgr.open(localStorage.getItem("uid") + ".db");
    if (db != null) {
        var sql_sel = "select  * from tbl_packet_detail where rid = '" + rid + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                uexDataBaseMgr.close(db);
                listPacketDetail(data);

            } else {

                uexDataBaseMgr.close(db);

            }

        });
    }

}

/*
 * 已领取福包的入口?
 */
function getRecDetailList(RID, number, Pageindex, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "RedPacket/GetRecPacketDetail",
        type : "POST",
        data : {
            RID : RID,
            Number : number,
            Pageindex : Pageindex,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    //获取红包评论的入口
                    //getPacketComment(uid, RID, number, data, 1, fromPage);

                    listPacketDetail(RID, number, Pageindex, data, fromPage);

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
function listPacketDetail(RID, number, Pageindex, PacketDetail, fromPage) {
    
    //RedPacket/GetRecPacketDetail
    //RID=95661b94-72bd-4dfe-a3cc-4ed85cf75f50&Number=&Pageindex=1

    $('#reclist').data('pageindex', Pageindex);
    $('#reclist').data('pagecount', PacketDetail.pagecount);
    //$('#content').data('pagecount', PacketDetail.pagecount);

    var lastStr = '';

    if (Pageindex == PacketDetail.pagecount) {

        lastStr += '<div class="end-nope-number">';
        lastStr += '没有更多了';
        lastStr += '</div>';

    }

    $('#content').data('rid', RID);
    $('#content').data('number', number);
    $('#content').data('from_page', fromPage);

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

    if (PacketDetail.reclist.length == 0) {
        
        reclist += '<div class="end-nope-number">';
        reclist += '当前没有数据';
        reclist += '</div>';

        openToast('当前没有数据', 3000, 5, 0);

    } else {

        $(PacketDetail.reclist).each(function(l, z) {

            var recsub = '';

            recsub += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="comt-padd c-wh rec-sub" data-truename="';
            if (z.TrueName == '' || z.TrueName == null || z.TrueName == undefined) {
                //recsub += localStorage.getItem("username");
                recsub += z.Phone;
            } else {
                recsub += z.TrueName;
            }
            recsub += '" ';
            recsub += 'data-userid="';
            recsub += z.ID;
            recsub += '">';
            recsub += '<div class="ub ubb bc-border border-padd-bottom">';
            recsub += '<div class="ub-img1 comt-icon-width mar-ar1" style="background-image:url(' + fileUrl + z.HeadImg1 + ');"></div>';
            recsub += '<div class="ub ub-ver ub-f1 mar-ar1">';
            recsub += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="user-hdr">';
            if (z.TrueName == '' || z.TrueName == null || z.TrueName == undefined) {
                //recsub += localStorage.getItem("username");
                recsub += z.Phone;
            } else {
                recsub += z.TrueName;
            }
            recsub += '</div>';
            recsub += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ut-s user-time">';
            recsub += timeStampFormat(z.UpdateTime);
            recsub += '</div>';
            recsub += '</div>';
            recsub += '<div class="ub ub-ver ub-ae ub-pe">';
            recsub += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="list-cash tx-r">';
            recsub += z.Money;
            recsub += '元</div>';
            recsub += '<div class="ut-s user-time">';
            recsub += '&nbsp;';
            recsub += '</div>';
            recsub += '</div>';
            recsub += '</div>';
            recsub += '</div>';

            if (localStorage.getItem("uid") == z.ID) {

                redGetMine = z.Money;

            }

            reclist += recsub;

        });
        
        reclist += lastStr;

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

    $('#title').children().remove();
    $('#title').append(title);

    if (fromPage == 'refresh_hbreceived') {
        $('#reclist').children().remove();
        appcan.frame.resetBounce(0);
    } else {
        appcan.frame.resetBounce(1);
    }
    //debug(reclist);
    $(reclist).appendTo('#reclist');

    $('#title').html(title);
    $('#imglist').children().remove();
    $(imglist).appendTo('#imglist');
    $('#GoodCount').html(GoodCount);

    $('#CreateTime').html(CreateTime);

    $('.rec-sub').unbind();
    $('.rec-sub').click(function() {

        var uid = localStorage.getItem("uid");

        var userid = $(this).data('userid');

        if (uid == userid) {

            openToast('-_-|||，这个是你本人啊', 3000, 5, 0);

        } else {

            var name = new Date().format("yyyyMMddhhmmssSS");

            localStorage.setItem("userid", userid);
            localStorage.setItem("fromPage", 'hbreceived');

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
                    flag : 1024,
                });

            }

        }

    });

    $('.red-host').unbind();
    $('.red-host').click(function() {

        var uid = localStorage.getItem("uid");
        var userid = $(this).data('userid');

        if (uid == userid) {

            openToast('-_-|||，这个是你本人啊', 3000, 5, 0);

        } else {

            var name = new Date().format("yyyyMMddhhmmssSS");

            localStorage.setItem("userid", userid);
            localStorage.setItem("fromPage", 'hbreceived');

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

    closeToast();

}

//接口.17.2.判断当前的页码为多少
//pagecount
function judgRecCurPage(RID, number, item, fromPage) {

    var curpage = '';
    //$('.photo-album').children("div:last-child").index()
    var pagecount = parseInt($('#reclist').data('pagecount'));
    var pageindex = parseInt($('#reclist').data('pageindex'));
    curpage = $('#reclist').children("div:last-child").index() + 1;

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

                    getRecDetailList(RID, number, pageindex + 1, fromPage);

                }

            }

        }

    }

}
