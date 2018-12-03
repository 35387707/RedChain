//接口.17.1.判断当前的红包是否已收藏
function isActnBegin(uid, rid, number, PacketDetail, PacketComment, PageIndex, fromPage) {

    var json = {
        collcetion : 0,
        cid : '',
        good : 0,
        share : 0,
    }

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        isCollcJudg(db, uid, rid, number, PacketDetail, PacketComment, PageIndex, json, fromPage);
    }

}

function isCollcJudg(db, uid, rid, number, PacketDetail, PacketComment, PageIndex, json, fromPage) {

    var sql_sel = "select * from tbl_collection_fubao where uid = '" + uid + "' and rid = '" + rid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            if (data.length != 0) {
                json.collcetion = 1;
                json.cid = data[0].cid;
            }

            isGoodJudg(db, uid, rid, number, PacketDetail, PacketComment, PageIndex, json, fromPage);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

function isGoodJudg(db, uid, rid, number, PacketDetail, PacketComment, PageIndex, json, fromPage) {

    var sql_sel = "select * from tbl_good_fubao where uid = '" + uid + "' and rid = '" + rid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            if (data.length != 0) {
                json.good = 1;
            }

            isShareJudg(db, uid, rid, number, PacketDetail, PacketComment, PageIndex, json, fromPage);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

function isShareJudg(db, uid, rid, number, PacketDetail, PacketComment, PageIndex, json, fromPage) {

    var sql_sel = "select * from tbl_share_fubao where uid = '" + uid + "' and rid = '" + rid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            if (data.length != 0) {
                json.share = 1;
            }

            debug(json);

            uexDataBaseMgr.close(db);

            localStorage.setItem("actnJson", JSON.stringify(json));
            setPacketDetail(uid, rid, number, PacketDetail, PacketComment, PageIndex, json, fromPage);
            
/*
            var name = new Date().format("yyyyMMddhhmmssSS"); 

            var localPack = localStorage.getItem('localPack');
            if (localPack == '1' || localPack == 1) {

                appcan.window.open({
                    name : 'hbdetail',
                    data : '../hb/hbdetail.html',
                    aniId : 10,
                });

            } else {

                uexWindow.open({
                    name : "hbdetail",
                    data : "../hb/hbdetail.html",
                    animID : 10,
                    flag : 1024
                });

            }*/


        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

/*
 * 收藏、点赞、分享的逻辑入口
 */

$('.actn-clk').click(function() {

    var indx = $(this).data('chg_s');

    if ($("div[data-chg_t='" + indx + "']").hasClass('little-red')) {

        if (indx == '收藏') {

            var uid = localStorage.getItem("uid");
            var cid = $("div[data-chg_s='" + indx + "']").data('cid');

            uexWindow.actionSheet({
                title : "",
                cancel : "取消",
                buttons : "取消收藏"
            }, function(index) {

                if (index == 0) {

                    deleteCollection(uid, cid, 'collection');

                }

            });

        } else {
            openToast("您已" + indx + '过当前福包信息', 5000, 5, 0);
        }

    } else {

        //$("a[data-chg_r='" + indx + "']").addClass('advert-actn')
        if (indx == '分享') {

            localStorage.setItem("share", 1);
            
            var rid = $('#content').data('rid');
            var fromPage = 'fubao';
            
            localStorage.setItem("sid", rid);
            localStorage.setItem("fromPage", fromPage);

            var bodyWidth = localStorage.getItem("bodyWidth");
            var bodyHeight = localStorage.getItem("bodyHeight");
            
            appcan.window.openPopover({
                name : 'share',
                dataType : 0,
                url : "../share/share.html",
                top : 0,
                left : 0,
                width : bodyWidth,
                height : bodyHeight,
            });

        } else if (indx == '点赞') {

            goodForRedPacket(indx);

        } else if (indx == '收藏') {

            addCollectionNew(indx);

        }

    }

});

//5.0.收藏红包
function addCollectionNew(indx) {

    var number = $('#content').data('number');
    var RID = $('#content').data('rid');

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "MyCollection/Add",
        type : "POST",
        data : {
            Mid : RID,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    isrtCollcSgl(indx, data.msg, RID);

                } else {

                    openToast('收藏失败', 3000, 5, 0);

                }

            } else {

                openToast('收藏失败', 3000, 5, 0);

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
 * 收藏成功之后把记录插入数据库
 */
function isrtCollcSgl(indx, cid, rid) {

    var uid = localStorage.getItem("uid");

    var sql_tx = [];

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {

        var sql = "insert into tbl_collection_fubao (uid, cid, rid) values ('" + uid + "', '" + cid + "', '" + rid + "')";
        sql_tx.push(sql);

        uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

            if (error == 0) {

                uexDataBaseMgr.close(db);
                $("div[data-chg_i='" + indx + "']").addClass('collc-icon-click');
                $("div[data-chg_t='" + indx + "']").addClass('little-red');
                $("div[data-chg_s='" + indx + "']").data('cid', cid);
                openToast('收藏成功', 3000, 5, 0);

            } else {

                uexDataBaseMgr.close(db);
                openToast('获取好友列表失败', 3000, 5, 0);

            }

        });
    }

}

//3.接口15 红包点赞
function goodForRedPacket(indx) {

    var number = $('#content').data('number');
    var RID = $('#content').data('rid');

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "RedPacket/GoodForRedPacket",
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

                    isrtGoodSgl(indx, RID);

                } else if (data.code == -9994 || data.code == '-9994') {

                    openToast(data.msg, 3000, 5, 0);
                    isrtGoodSgl(indx, RID);

                }

            } else {

                openToast('点赞失败', 3000, 5, 0);

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
 * 点赞成功之后把记录插入数据库
 */
function isrtGoodSgl(indx, rid) {

    var uid = localStorage.getItem("uid");

    var sql_tx = [];

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {

        var sql = "insert into tbl_good_fubao (uid, rid) values ('" + uid + "', '" + rid + "')";
        sql_tx.push(sql);

        uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

            if (error == 0) {

                uexDataBaseMgr.close(db);
                localGoodRefresh(indx);

            } else {

                uexDataBaseMgr.close(db);
                openToast('点赞失败', 3000, 5, 0);

            }

        });
    }

}

//3.1.点赞提交成功之后本地的数据加1
function localGoodRefresh(indx) {

    var GoodCount = parseInt($('#GoodCount').text());
    $('#GoodCount').html(GoodCount + 1);
    $("div[data-chg_i='" + indx + "']").addClass('good-icon-click');
    $("div[data-chg_t='" + indx + "']").addClass('little-red');
    closeToast();

}

/*
 * 点赞成功之后把记录插入数据库
 */
function isrtShareSgl(indx, rid) {

    var uid = localStorage.getItem("uid");

    var sql_tx = [];

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {

        var sql = "insert into tbl_share_fubao (uid, rid) values ('" + uid + "', '" + rid + "')";
        sql_tx.push(sql);

        uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

            if (error == 0) {

                uexDataBaseMgr.close(db);
                localShareRefresh(indx);

            } else {

                uexDataBaseMgr.close(db);
                openToast('点赞失败', 3000, 5, 0);

            }

        });
    }

}

//3.1.点赞提交成功之后本地的数据加1
function localShareRefresh(indx) {

    $("div[data-chg_i='" + indx + "']").addClass('share-icon-click');
    $("div[data-chg_t='" + indx + "']").addClass('little-red');
    closeToast();

}