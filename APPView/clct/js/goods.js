//接口.17.0.获取收藏列表
//Mtype    收藏类型    必填，0为红包收藏，1为用户收藏，2为消息收藏
function getCollectionList(uid, Index, Mtype, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "MyCollection/GetList",
        type : "POST",
        data : {
            Index : Index,
            Mtype : Mtype,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    //setCollectionList(uid, data, Index, fromPage);
                    var db = uexDataBaseMgr.open(uid + ".db");
                    if (db != null) {
                        collcSrchOne(db, uid, 0, data.list.length, data, [], Index, fromPage);
                    }

                } else {
                    openToast(data.msg, 3000, 5, 0);
                }

            } else {

                openToast('获取广告收藏失败', 3000, 5, 0);

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

//接口.17.0.收藏列表保存到本地
function collcSrchOne(db, uid, i, length, json, sql_tx, PageIndex, fromPage) {

    if (i < length) {

        var sql_sel = "select  * from tbl_collection_goods where uid = '" + uid + "' and gid = '" + json.list[i].ProID + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                var sql = "";
                if (data.length == 0) {
                    sql = "insert into tbl_collection_goods (uid, cid, gid) values ('" + uid + "', '" + json.list[i].ID + "', '" + json.list[i].ProID + "')";
                } else {
                    sql = "update tbl_collection_goods set cid='" + json.list[i].ID + "' where uid = '" + uid + "' and gid = '" + json.list[i].ProID + "'";
                }
                sql_tx.push(sql);

                collcSrchOne(db, uid, i + 1, length, json, sql_tx, PageIndex, fromPage);

            } else {

                uexDataBaseMgr.close(db);
                openToast('获取好友列表失败', 3000, 5, 0);

            }

        });

    } else {

        collcTxActn(db, uid, json, sql_tx, PageIndex, fromPage);

    }

}

//接口.17.1.事务处理
function collcTxActn(db, uid, json, sql_tx, PageIndex, fromPage) {

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            //friendDispList(db, uid, json);
            uexDataBaseMgr.close(db);
            //getPacketList(fromPage);
            setCollectionList(uid, json, PageIndex, fromPage);

        } else {

            uexDataBaseMgr.close(db);
            openToast('获取好友列表失败', 3000, 5, 0);

        }

    });

}

//接口.17.1.迭代收藏列表数据
function setCollectionList(uid, json, PageIndex, fromPage) {

    var fileUrl = localStorage.getItem("fileUrl");

    //debug(json);

    var content = '';
    var lastStr = '';

    $('#content').data('pageindex', PageIndex);
    $('#content').data('pagecount', json.pagecount);

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

        $(json.list).each(function(k, r) {

            //debug(imgArr);

            content += '<div data-guid="' + r.ProID + '" data-gname="' + r.Name + '" data-cid="' + r.ID + '" class="ub uc-a1 ub-ver collc-sty collc-clk">';

            content += '<div class="ub c-wh">';

            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 collc-img collc-wh umar-a" style="background-size: cover; background-image:url(' + fileUrl + r.Img + ');"></div>';

            content += '<div class="ub-f1 collc-cont">';

            content += '<div class="collc-txt">';
            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="goods-title title-overflow">';
            content += r.Title;
            content += '</div>';
            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="goods-name name-overflow">';
            content += r.Name;
            content += '</div>';
            content += '</div>';

            content += '<div class="ub collc-actn">';
            
            content += '<div class="ub ub-ac" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            content += '<div class="ub ub-img1 item-width read-gray"></div>';
            content += '<div class="item-txt ufm1">';
            content += r.ViewCount;
            content += '</div>';
            content += '</div>';

            content += '<div class="ub ub-ac collc-div price-sty" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            //content += '';
            if (r.PriceType == 0) {
                content += '￥' + r.Price;
            } else if (r.PriceType == 1) {
                content += '￥' + r.Price;
            } else if (r.PriceType == 2) {
                content += '福分' + r.Price;
            } else if (r.PriceType == 4) {
                content += '福券' + r.Price;
            }
            content += '</div>';

            content += '</div>';
            content += '</div>';
            content += '</div>';
            content += '</div>';

        });

        content += lastStr;

    }

    if (fromPage == 'refresh_goods') {
        $("#content").children().remove();
        appcan.frame.resetBounce(0);
        openToast('刷新数据成功', 3000, 5, 0);
    } else {
        appcan.frame.resetBounce(1);
        openToast('加载数据成功', 3000, 5, 0);
    }
    $('#content').append(content);

    $('.collc-clk').unbind();
    $('.collc-clk').click(function() {

        var name = new Date().format("yyyyMMddhhmmssSS");

        var guid = $(this).data('guid');
        var gname = $(this).data('gname');
        
        debug(guid);
        
        localStorage.setItem("guid", guid);
        localStorage.setItem("gname", gname);
        
        var localPack = localStorage.getItem('localPack');

        if (localPack == '1' || localPack == 1) {

            appcan.window.open({
                name : 'goods_detail',
                data : '../order/goods_detail.html',
                aniId : 10,
            });

        } else {

            uexWindow.open({
                name : "goods_detail",
                data : "../order/goods_detail.html",
                animID : 10,
                flag : 1024
            });

        }

    });

}

//接口.17.2.判断当前的页码为多少
//pagecount
function judgCurPage(uid, item, Mtype, fromPage) {

    var curpage = '';
    
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

                    getCollectionList(uid, pageindex + 1, Mtype, fromPage);

                }

            }

        }

    }

}
