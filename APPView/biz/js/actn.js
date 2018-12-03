/*
* 1.广告收藏的逻辑
*/
//接口.17.1.判断当前的红包是否已收藏
function isActnBegin(uid, iid, ViewInfo, Comment, PageIndex, fromPage) {

    var json = {
        collcetion : 0,
        cid : '',
        good : 0,
        share : 0,
    }

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        isCollcJudg(db, uid, iid, ViewInfo, Comment, PageIndex, json, fromPage);
    }

}

function isCollcJudg(db, uid, iid, ViewInfo, Comment, PageIndex, json, fromPage) {

    var sql_sel = "select * from tbl_collection_advert where uid = '" + uid + "' and iid = '" + iid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            if (data.length != 0) {
                json.collcetion = 1;
                json.cid = data[0].cid;
            }

            isGoodJudg(db, uid, iid, ViewInfo, Comment, PageIndex, json, fromPage);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

function isGoodJudg(db, uid, iid, ViewInfo, Comment, PageIndex, json, fromPage) {

    var sql_sel = "select * from tbl_good_advert where uid = '" + uid + "' and iid = '" + iid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            if (data.length != 0) {
                json.good = 1;
            }

            isShareJudg(db, uid, iid, ViewInfo, Comment, PageIndex, json, fromPage);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

function isShareJudg(db, uid, iid, ViewInfo, Comment, PageIndex, json, fromPage) {

    var sql_sel = "select * from tbl_share_advert where uid = '" + uid + "' and iid = '" + iid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            if (data.length != 0) {
                json.share = 1;
            }

            debug(json);

            uexDataBaseMgr.close(db);

            localStorage.setItem("actnJson", JSON.stringify(json));

            //setPacketDetail(uid, iid, ViewInfo, Comment, PageIndex, json, fromPage)
            setViewInfo(uid, iid, ViewInfo, Comment, PageIndex, json, fromPage);

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

                    deleteCollection(uid, cid, 'adv');

                }

            });

        } else {
            openToast("您已" + indx + '过当前广告', 5000, 5, 0);
        }

    } else {

        //$("a[data-chg_r='" + indx + "']").addClass('advert-actn')
        if (indx == '分享') {

            localStorage.setItem("share", 1);

            var iid = $('#content').data('iid');
            var fromPage = 'advert';

            localStorage.setItem("sid", iid);
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

    var IID = localStorage.getItem("iid");

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "MyCollection/Add",
        type : "POST",
        data : {
            Mid : IID,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    isrtCollcSgl(indx, data.msg, IID);

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
function isrtCollcSgl(indx, cid, iid) {

    var uid = localStorage.getItem("uid");

    var sql_tx = [];

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {

        var sql = "insert into tbl_collection_advert (uid, cid, iid) values ('" + uid + "', '" + cid + "', '" + iid + "')";
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

    var IID = localStorage.getItem("iid");

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Infomation/Good",
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

                    isrtGoodSgl(indx, IID);

                } else if (data.code == -9994 || data.code == '-9994') {

                    openToast(data.msg, 3000, 5, 0);
                    isrtGoodSgl(indx, IID);

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
function isrtGoodSgl(indx, iid) {

    var uid = localStorage.getItem("uid");

    var sql_tx = [];

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {

        var sql = "insert into tbl_good_advert (uid, iid) values ('" + uid + "', '" + iid + "')";
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

//接口.18.1.删除收藏
function deleteCollection(uid, cid, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "MyCollection/Delete",
        type : "POST",
        data : {
            Id : cid,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            //debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    var db = uexDataBaseMgr.open(uid + ".db");
                    if (db != null) {
                        deleteCollcSql(db, uid, cid, fromPage);
                    }

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('取消收藏成功', 3000, 5, 0);

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
 * 接口.18.2.本地删除收藏的逻辑
 */
function deleteCollcSql(db, uid, cid, fromPage) {

    var sql_tx = [];
    var sql = "delete from tbl_collection_advert where uid = '" + uid + "' and cid = '" + cid + "'";
    sql_tx.push(sql)
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            //friendDispList(db, uid, json);
            uexDataBaseMgr.close(db);
            var indx = '收藏';
            $("div[data-chg_i='" + indx + "']").removeClass('collc-icon-click');
            $("div[data-chg_t='" + indx + "']").removeClass('little-red');
            $("div[data-chg_s='" + indx + "']").data('cid', '');

            if (fromPage == 'adv') {
                appcan.window.publish('removeAdvert', cid);
            }

            //$("div[data-cid='" + cid + "']").remove();
            openToast('取消收藏成功', 3000, 5, 0);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

/*
 * 点赞成功之后把记录插入数据库
 */
function isrtShareSgl(indx, iid) {

    var uid = localStorage.getItem("uid");

    var sql_tx = [];

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {

        var sql = "insert into tbl_share_advert (uid, iid) values ('" + uid + "', '" + iid + "')";
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