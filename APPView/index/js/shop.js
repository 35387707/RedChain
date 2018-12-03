/*
 * 接口.53.1.获取所有商品分类
 */
function getCategorist(uid, Headid, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "Product/Categorist",
        type : "POST",
        data : {
            Headid : Headid,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    //setGoodsList(uid, CID, Key, PageIndex, fromPage, data);
                    //localStorage.setItem("CategoristID", data.list[0].ID);
                    debug(data.list[0].ID);
                    getGoodsList(uid, data, data.list[0].ID, '', 1, fromPage);

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('获取商品列表失败', 3000, 5, 0);

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
 * 接口.53.获取商品列表
 * 参数说明:
 * 1.  CID   int  分类ID，可为空
 * 2.  Key   string   搜索关键字，可为空
 * 3.  PageIndex  int   页码，可为空
 */
function getGoodsList(uid, Categorist, CID, Key, PageIndex, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "Product/List",
        type : "POST",
        data : {
            CID : CID,
            Key : Key,
            PageIndex : PageIndex,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    setGoodsList(uid, Categorist, CID, Key, PageIndex, fromPage, data);

                } else {

                    openToast(data.msg, 3000, 5, 0);

                }

            } else {

                openToast('获取商品列表失败', 3000, 5, 0);

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

//接口.53.1.设置商品列表
function setGoodsList(uid, Categorist, CID, Key, PageIndex, fromPage, json) {

    if (fromPage == 'shop') {

        var tabStr = '';

        $(Categorist.list).each(function(i, v) {

            if (i == 0) {

                tabStr += '<div data-indx="' + i + '" data-cid="' + v.ID + '" data-name="' + v.Name + '" data-title="' + v.Title + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="swiper-slide nav-sub active">';
                tabStr += v.Name;
                tabStr += '</div>';

            } else {

                tabStr += '<div data-indx="' + i + '" data-cid="' + v.ID + '" data-name="' + v.Name + '" data-title="' + v.Title + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="swiper-slide nav-sub">';
                tabStr += v.Name;
                tabStr += '</div>';

            }

        });

        debug(tabStr);

        $('.nav-wrap').children().remove();
        $('.nav-wrap').append(tabStr);

    }

    var titHeight = $('#hdr_wrap').height();
    $('#content').css('padding-top', titHeight);

    var navSwiper = new Swiper('.nav-cont', {
        watchSlidesProgress : true,
        watchSlidesVisibility : true,
        slidesPerView : 'auto',
        spaceBetween : 90
    });

    $(".nav-cont .nav-wrap div").click(function() {
        if ($(this).hasClass('active')) {

        } else {

            $(".nav-cont .nav-wrap .active").removeClass('active');
            $(this).addClass('active');

            var CID = $(this).data('cid');
            debug(CID);

            getGoodsList(uid, '', CID, '', 1, 'tab_chg');

        }
        //appcan.window.selectMultiPopover("content", $(this).index());
    });

    var fileUrl = localStorage.getItem("fileUrl");

    //debug(json);

    var goodsStr = '';
    var lastStr = '';

    $('#goods_list').data("pageindex", PageIndex);
    $('#goods_list').data("pagecount", json.pagecount);

    if (PageIndex == json.pagecount) {

        lastStr += '<div class="end-nope-number">';
        lastStr += '没有更多了';
        lastStr += '</div>';

    }

    if (json.list.length == 0) {

        goodsStr += '<div class="end-nope-number">';
        goodsStr += '当前没有数据';
        goodsStr += '</div>';

        openToast('当前没有数据', 3000, 5, 0);

    } else {

        $(json.list).each(function(k, r) {

            //debug(imgArr);

            goodsStr += '<div class="ub ub-ver goods-style">';

            //name
            goodsStr += '<div class="ub ub-ver ub-fv" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            goodsStr += '<div class="ub goods-title-style ub-ac">';
            goodsStr += '<div class="goods-title-color ub-f1">';
            goodsStr += r.Name;
            goodsStr += '</div>';
            goodsStr += '</div>';
            goodsStr += '</div>';

            //cont
            goodsStr += '<div class="ub ub-ver">';
            goodsStr += '<div class="ub ub-ver ubb bc-border">';
            goodsStr += '<div class="ub">';
            goodsStr += '<div class="ub-f1 goods-cont">';

            //img
            goodsStr += '<div data-gname="' + r.Name + '" data-guid="' + r.ID + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img1 goods-img-style goods-info-img goods-sub" style="background-image:url(' + fileUrl + r.Img + ');"></div>';

            //right top
            //goodsStr += '<div class="goods-info-right">';
            //goodsStr += '<div class="ub-img1 right-size right-img" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            //goodsStr += '<div class="right-top">';
            //goodsStr += '<span class="right-txt">福包多赞助</span>';
            //goodsStr += '</div>';
            //goodsStr += '<div class="right-bot"></div>';
            //goodsStr += '</div>';
            //goodsStr += '</div>';

            //info-bot
            goodsStr += '<div class="goods-info-bot">';
            goodsStr += '<ul class="ub ub-ac info-bot-style" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';

            goodsStr += '<li class="ub-f1 info-bot-left">';
            goodsStr += '<div class="ub ub-ac" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            goodsStr += '<div class="ub ub-img info-icon-width fubao-icon"></div>';
            goodsStr += '<div class="info-txt-style">';
            if (r.PriceType == 0) {
                goodsStr += r.Price;
            } else if (r.PriceType == 1) {
                goodsStr += r.Price + '元';
            } else if (r.PriceType == 2) {
                goodsStr += r.Price + '福音积分';
            } else if (r.PriceType == 4) {
                goodsStr += r.Price + '福利积分';
            }
            //goodsStr += '3000福利分+20元包邮';
            goodsStr += '</div>';
            goodsStr += '</div>';
            goodsStr += '</li>';

            goodsStr += '<li class="info-bot-right">';
            goodsStr += '<div class="ub ub-ac" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            goodsStr += '<div class="info-right-txt info-right-title">';
            //goodsStr += '兑换人数: ';
            //goodsStr += '0';
            //goodsStr += '人';
            goodsStr += '</div>';
            goodsStr += '</div>';
            goodsStr += '</li>';

            goodsStr += '</ul>';
            goodsStr += '</div>';

            goodsStr += '</div>';
            goodsStr += '</div>';
            goodsStr += '</div>';
            goodsStr += '</div>';

            goodsStr += '<div class="goods-line"></div>';

            goodsStr += '</div>';

        });

        goodsStr += lastStr;

    }

    if (fromPage == 'refresh_shop') {

        $("#goods_list").children().remove();
        appcan.frame.resetBounce(0);
        openToast('刷新数据成功', 3000, 5, 0);

    } else if (fromPage == 'tab_chg') {

        $("#goods_list").children().remove();
        appcan.frame.resetBounce(1);
        openToast('加载数据成功', 3000, 5, 0);

    } else {

        appcan.frame.resetBounce(1);
        openToast('加载数据成功', 3000, 5, 0);

    }

    $('#goods_list').append(goodsStr);

    $('.goods-sub').unbind();
    $('.goods-sub').click(function() {

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
function judgCurPage(uid, item, CID, Key, fromPage) {

    var curpage = '';
    //$('.photo-album').children("div:last-child").index()
    var pagecount = parseInt($('#goods_list').data('pagecount'));
    var pageindex = parseInt($('#goods_list').data('pageindex'));
    curpage = $('#goods_list').children("div:last-child").index() + 1;

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

                    //getFollowList(uid, curpage + 1, Mtype, fromPage);
                    //getGoodsList(uid, CID, Key, curpage + 1, fromPage);
                    getGoodsList(uid, '', CID, Key, pageindex + 1, fromPage);

                }

            }

        }

    }

}

/*pane0*/
function dispSliderImg() {

    //广告图片轮播
    var slider = appcan.slider({
        selector : "#slider",
        aspectRatio : 9 / 16,
        hasLabel : true,
        index : 0,
        auto : 3000,
        canDown : true,
        hasCircle : true,
        circleSlide : true,
    });
    slider.set([{
        img : "../luck/img/choujiang_chanpinkeng@2x.png",
        label : "change the world",
    }, {
        img : "../luck/img/choujiang_chanpinkeng@2x.png",
        label : "the world is boring",
    }]);
    slider.on("clickItem", function(index, data) {
    });

}