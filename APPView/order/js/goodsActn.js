/*
* 1.广告收藏的逻辑
*/
//接口.17.1.判断当前的红包是否已收藏
function isActnBegin(uid, gid, GoodsDetail, fromPage) {

    var json = {
        collcetion : 0,
        cid : '',
        good : 0,
        share : 0,
    }

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        isCollcJudg(db, uid, gid, GoodsDetail, json, fromPage);
    }

}

function isCollcJudg(db, uid, gid, GoodsDetail, json, fromPage) {

    var sql_sel = "select * from tbl_collection_goods where uid = '" + uid + "' and gid = '" + gid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            if (data.length != 0) {
                json.collcetion = 1;
                json.cid = data[0].cid;
            }

            isGoodJudg(db, uid, gid, GoodsDetail, json, fromPage);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

function isGoodJudg(db, uid, gid, GoodsDetail, json, fromPage) {

    var sql_sel = "select * from tbl_good_goods where uid = '" + uid + "' and gid = '" + gid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            if (data.length != 0) {
                json.good = 1;
            }

            isShareJudg(db, uid, gid, GoodsDetail, json, fromPage);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

function isShareJudg(db, uid, gid, GoodsDetail, json, fromPage) {

    var sql_sel = "select * from tbl_share_goods where uid = '" + uid + "' and gid = '" + gid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            if (data.length != 0) {
                json.share = 1;
            }

            debug(json);

            uexDataBaseMgr.close(db);

            //setPacketDetail(uid, iid, GoodsDetail, json, fromPage)
            setGoodsDetail(uid, gid, GoodsDetail, json, fromPage);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

//5.0.收藏商品
function addCollectionNew(indx) {

    var GID = localStorage.getItem("guid");

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "MyCollection/Add",
        type : "POST",
        data : {
            Mid : GID,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    isrtCollcSgl(indx, data.msg, GID);

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
function isrtCollcSgl(indx, cid, gid) {

    var uid = localStorage.getItem("uid");

    var sql_tx = [];

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {

        var sql = "insert into tbl_collection_goods (uid, cid, gid) values ('" + uid + "', '" + cid + "', '" + gid + "')";
        sql_tx.push(sql);

        uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

            if (error == 0) {

                uexDataBaseMgr.close(db);
                $('#goods_clct').data('collcetion', 1);
                $('#goods_clct').data('cid', cid);
                $('#goods_clct').text('已收藏');
                openToast('收藏成功', 3000, 5, 0);

            } else {

                uexDataBaseMgr.close(db);
                openToast('获取好友列表失败', 3000, 5, 0);

            }

        });
    }

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
    var sql = "delete from tbl_collection_goods where uid = '" + uid + "' and cid = '" + cid + "'";
    sql_tx.push(sql)
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            //friendDispList(db, uid, json);
            uexDataBaseMgr.close(db);
            
            $('#goods_clct').data('collcetion', 0);
            $('#goods_clct').data('cid', '');
            $('#goods_clct').text('收藏');

            appcan.window.publish('removeGoods', cid);

            //$("div[data-cid='" + cid + "']").remove();
            openToast('取消收藏成功', 3000, 5, 0);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}