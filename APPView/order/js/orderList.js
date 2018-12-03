/*
 * 接口.59.0.获取我的订单列表
 */
function getOrderList(uid, status, pageindex, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "Orders/List",
        type : "POST",
        data : {
            status : status,
            pageindex : pageindex,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    setOrderList(uid, status, pageindex, data, fromPage);

                } else {

                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                openToast('获取商品信息失败', 5000, 5, 0);

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
            openToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}

//接口.17.1.迭代收藏列表数据
function setOrderList(uid, status, pageindex, json, fromPage) {

    var fileUrl = localStorage.getItem("fileUrl");

    //debug(json);

    var content = '';
    var lastStr = '';

    $('#content').data('pageindex', pageindex);
    $('#content').data('pagecount', json.pagecount);

    if (pageindex == json.pagecount) {

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

            content += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="good-list good-sub" data-orderid="' + r.ID + '" data-number="' + r.Number + '" data-shopid="' + r.ShopID + '" data-recname="' + r.RecName + '" data-recareid="' + r.RecAreaID + '" data-recaddress="' + r.RecAddress + '" data-recphone="' + r.RecPhone + '" data-recareacode="' + r.RecAreaCode + '" data-recemail="' + r.RecEmail + '" data-recsex="' + r.RecSex + '" data-payment="' + r.Payment + '" data-paynumber="' + r.PayNumber + '" data-pricetype="' + r.PriceType + '" data-price="' + r.Price + '" data-fee="' + r.Fee + '" data-ordertype="' + r.OrderType + '" data-remark="' + r.Remark + '" data-paytime="' + r.PayTime + '" data-sendtime="' + r.SendTime + '" data-rectime="' + r.RecTime + '" data-finishtime="' + r.FinishTime + '" data-status="' + r.Status + '" data-createtime="' + r.CreateTime + '" data-updatetime="' + r.UpdateTime + '" data-notes="' + r.notes + '" data-orderproid="' + r.orderProID + '" data-proprice="' + r.proPrice + '" data-proname="' + r.proName + '" data-categoryid="' + r.CategoryID + '" data-count="' + r.Count + '" data-img="' + r.Img + '" data-pronumber="' + r.proNumber + '" data-propricetype="' + r.proPriceType + '" data-productid="' + r.ProductID + '" data-protype="' + r.proType + '" data-uphone="' + r.Uphone + '" data-utruename="' + r.UTrueName + '" data-ustatus="' + r.Ustatus + '" data-catename="' + r.CateName + '" data-family="' + r.Family + '" data-headid="' + r.HeadID + '" data-isshow="' + r.IsShow + '">';

            content += '<ul class="ub ub-ac bill-padd">';
            content += '<li class="ub-f1 bill-txt">';
            content += '下单时间: ';
            content += timeStampFormat(r.CreateTime);
            content += '</li>';
            content += '<li class="bill-type">';
            if (r.Status == -1) {
                content += '已取消';
            } else if (r.Status == 0) {
                content += '待付款';
            } else if (r.Status == 1 || r.Status == 2) {
                content += '待收货';
            } else if (r.Status == 3 || r.Status == 4) {
                content += '已完成';
            }
            content += '</li>';
            content += '</ul>';

            content += '<div class="ub list-style">';
            content += '<div class="icon-style">';
            content += '<img src="';
            content += fileUrl + r.Img;
            content += '" />';
            content += '</div>';

            content += '<div class="ub-f1 goods-padd">';
            content += '<div class="nick-padd ub">';
            content += '<div class="user-nick overflow">';
            content += r.proName;
            content += '</div>';
            content += '</div>';

            content += '<div class="user-note overflow">';
            content += '</div>';
            content += '</div>';

            content += '<div class="price-padd">';
            content += '<div class="nick-padd ub">';
            content += '<div class="price-num">';
            if (r.PriceType == 1) {
                content += '￥' + r.Price;
            } else if (r.PriceType == 2) {
                content += '￥' + r.Price;
            } else if (r.PriceType == 3) {
                content += '福音积分' + r.Price;
            } else if (r.PriceType == 4) {
                content += '福利积分' + r.Price;
            }
            content += '</div>';
            content += '</div>';
            content += '<div class="sgl-num">';
            content += 'x ';
            content += r.Count
            content += '</div>';
            content += '</div>';
            content += '</div>';

            content += '<ul class="ub ub-ac pay-padd" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            content += '<li class="ub-f1 pay-txt"></li>';
            content += '<li class="pay-detail">';
            content += '<span>实付: ';
            content += '<a class="pay-num">';
            if (r.PriceType == 1) {
                content += '￥' + (r.Price * r.Count);
            } else if (r.PriceType == 2) {
                content += '￥' + (r.Price * r.Count);
            } else if (r.PriceType == 3) {
                content += '福音积分' + (r.Price * r.Count);
            } else if (r.PriceType == 4) {
                content += '福利积分' + (r.Price * r.Count);
            }
            content += '</a>';
            content += '&nbsp;';
            content += '</span>';
            content += '<a class="pay-total">';
            content += '共';
            content += r.Count;
            content += '件商品';
            content += '</a>';
            content += '</li>';
            content += '</ul>';
            content += '</div>';

        });

        content += lastStr;

    }

    if (fromPage == 'refresh') {

        $("#content").children().remove();
        appcan.frame.resetBounce(0);
        openToast('刷新数据成功', 3000, 5, 0);

    } else {

        appcan.frame.resetBounce(1);
        openToast('加载数据成功', 3000, 5, 0);

    }

    debug(content);

    $('#content').append(content);

    $('.good-sub').unbind();
    $('.good-sub').click(function() {

        var ID = $(this).data('orderid');
        var Number = $(this).data('number');

        var ShopID = $(this).data('shopid');

        var RecName = $(this).data('recname');
        var RecAreaID = $(this).data('recareaid');
        var RecAddress = $(this).data('recaddress');
        var RecPhone = $(this).data('recphone');
        var RecAreaCode = $(this).data('recareacode');
        var RecEmail = $(this).data('recemail');
        var RecSex = $(this).data('recsex');

        var Payment = $(this).data('payment');
        var PayNumber = $(this).data('paynumber');

        var PriceType = $(this).data('pricetype');
        var Price = $(this).data('price');

        var Fee = $(this).data('fee');

        var OrderType = $(this).data('ordertype');
        var Remark = $(this).data('remark');

        var PayTime = $(this).data('paytime');
        var SendTime = $(this).data('sendtime');
        var RecTime = $(this).data('rectime');
        var FinishTime = $(this).data('finishtime');

        var Status = $(this).data('status');

        var CreateTime = $(this).data('createtime');
        var UpdateTime = $(this).data('updatetime');

        var notes = $(this).data('notes');

        var orderProID = $(this).data('orderproid');
        var proPrice = $(this).data('proprice');
        var proName = $(this).data('proname');
        var CategoryID = $(this).data('categoryid');
        var Count = $(this).data('count');

        var Img = $(this).data('img');
        var proNumber = $(this).data('pronumber');
        var proPriceType = $(this).data('propricetype');
        var ProductID = $(this).data('productid');
        var proType = $(this).data('protype');

        var Uphone = $(this).data('uphone');
        var UTrueName = $(this).data('uuruename');
        var Ustatus = $(this).data('ustatus');

        var CateName = $(this).data('catename');
        var Family = $(this).data('family');

        var HeadID = $(this).data('headid');

        var IsShow = $(this).data('isshow');

        var orderDetail = {
            ID : ID,
            Number : Number,

            ShopID : ShopID,

            RecName : RecName,
            RecAreaID : RecAreaID,
            RecAddress : RecAddress,
            RecPhone : RecPhone,
            RecAreaCode : RecAreaCode,
            RecEmail : RecEmail,
            RecSex : RecSex,

            Payment : Payment,
            PayNumber : PayNumber,

            PriceType : PriceType,
            Price : Price,

            Fee : Fee,

            OrderType : OrderType,

            Remark : Remark,

            PayTime : PayTime,
            SendTime : SendTime,
            RecTime : RecTime,
            FinishTime : FinishTime,

            Status : Status,

            CreateTime : CreateTime,
            UpdateTime : UpdateTime,

            notes : notes,

            orderProID : orderProID,
            proPrice : proPrice,
            proName : proName,
            CategoryID : CategoryID,
            Count : Count,

            Img : Img,
            proNumber : proNumber,
            proPriceType : proPriceType,
            ProductID : ProductID,
            proType : proType,

            Uphone : Uphone,
            UTrueName : UTrueName,

            Ustatus : Ustatus,

            CateName : CateName,
            Family : Family,

            HeadID : HeadID,

            IsShow : IsShow,
        };
        
        localStorage.setItem("orderStatus", Status);

        localStorage.setItem("oid", $(this).data('orderid'));

        var localPack = localStorage.getItem('localPack');
        if (localPack == '1' || localPack == 1) {

            appcan.window.open({
                name : 'order_detail',
                data : '../order/order_detail.html',
                aniId : 10,
            });

        } else {

            uexWindow.open({
                name : 'order_detail',
                data : '../order/order_detail.html',
                animID : 10,
                flag : 1024
            });

        }

    });

}

//接口.17.2.判断当前的页码为多少
//pagecount
function judgCurPage(uid, item, status, fromPage) {

    var curpage = '';
    //$('.photo-album').children("div:last-child").index()
    var pagecount = parseInt($('#content').data('pagecount'));
    var pageindex = parseInt($('#content').data('pageindex'));
    curpage = $('#content').children("div:last-child").index() + 1;

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
                    getOrderList(uid, status, pageindex + 1, fromPage);

                }

            }

        }

    }

}