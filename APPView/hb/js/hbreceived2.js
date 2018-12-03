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

//2.1.设置红包详情数据
function listPacketDetail(json) {

    debug(json);

    var fileUrl = localStorage.getItem("fileUrl");

    var imglist = '';
    var title = '';
    var TrueName = '';
    var HeadImg1 = '';
    var reccount = '';
    var ViewCount = '';
    var GoodCount = '';
    var GetMoney = '';
    var CreateTime = '';
    var reclist = '';
    var redGetMine = '';
    var totalPrice = '';
    //<div class="ub-img alrdy-wd alrdy-marg-right goods-infor1"></div>
    //<div class="ub-img red-wd icon-cont"></div>
    //<div class="ub-f1 ub-img1 disp-img-width disp-pic" data-pic_p="../hb/img/picture1.png" style="background-image:url('../hb/img/picture1.png');"></div>
    reccount = json[0].reccount;

    var model = JSON.parse(json[0].model);
    var reclistJson = JSON.parse(json[0].reclist);

    debug(model);
    debug(reclistJson);

    if (model.TrueName == null || model.TrueName == '' || model.TrueName == undefined) {
        TrueName = model.Phone + '的福包';
    } else {
        TrueName = model.TrueName + '的福包';
    }

    HeadImg1 = '<div data-userid="' + model.UID + '" class="ub-img1 red-wd icon-cont uc-a2 red-host" style="background-image:url(' + fileUrl + model.HeadImg1 + ');"></div>';

    CreateTime = timeStampFormat(model.CreateTime);
    // /Date();

    title = model.title;

    var imgArr = new Array();
    imgArr = model.imglist.split(";");
    $(imgArr).each(function(k, r) {

        imglist += '<div class="ub-f1 ub-img1 disp-img-width disp-pic" data-pic_p="' + fileUrl + r + '" style="background-image:url(' + fileUrl + r + ');"></div>';

    });

    ViewCount = model.ViewCount;
    GoodCount = model.GoodCount;

    totalPrice = model.TotalPrice;

    $(reclistJson).each(function(l, z) {

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

        if (reclistJson.length == l + 1) {

            recsub += '<div class="end-nope-number">';
            recsub += '没有更多了';
            recsub += '</div>';

        }

        reclist += recsub;

    });

    if (json[0].number == '' || json[0].number == null || json[0].number == undefined || json[0].number == NaN) {
        redGetMine = '';
    } else {
        redGetMine = model.GetMoney;
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
