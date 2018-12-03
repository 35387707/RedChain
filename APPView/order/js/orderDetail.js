/*
 * 接口.60.获取订单详情
 */
function getOrderDetail(uid, OID, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "Orders/Detail",
        type : "POST",
        data : {
            OID : OID,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    setOrderDetail(data);

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

/*
 * 接口.60.1.设置订单详情
 */
function setOrderDetail(json) {

    debug(json);

    var fileUrl = localStorage.getItem("fileUrl");

    var ID = json.model.ID;
    var Number = json.model.Number;

    var ShopID = json.model.ShopID;

    var RecName = json.model.RecName;
    var RecAreaID = json.model.RecAreaID;
    var RecAddress = json.model.RecAddress;
    var RecPhone = json.model.RecPhone;
    var RecAreaCode = json.model.RecAreaCode;
    var RecEmail = json.model.RecEmail;
    var RecSex = json.model.RecSex;

    var Payment = json.model.Payment;
    var PayNumber = json.model.PayNumber;

    var PriceType = json.model.PriceType;
    var Price = json.model.Price;

    var Fee = json.model.Fee;

    var OrderType = json.model.OrderType;
    var Remark = json.model.Remark;

    var PayTime = json.model.PayTime;
    var SendTime = json.model.SendTime;
    var RecTime = json.model.RecTime;
    var FinishTime = json.model.FinishTime;

    var Status = json.model.Status;

    var CreateTime = json.model.CreateTime;
    var UpdateTime = json.model.UpdateTime;

    var notes = json.model.notes;

    var orderProID = json.model.orderProID;
    var proPrice = json.model.proPrice;
    var proName = json.model.proName;
    var CategoryID = json.model.CategoryID;
    var Count = json.model.Count;

    var Img = '';
    Img += '<img ontouchstart="appcan.touch(&#39;btn-act&#39;)" src="' + fileUrl + json.model.Img + '" />';
    var proNumber = json.model.proNumber;
    var proPriceType = json.model.proPriceType;
    var ProductID = json.model.ProductID;
    var proType = json.model.proType;

    var Uphone = json.model.Uphone;
    var UTrueName = json.model.UTrueName;
    var Ustatus = json.model.Ustatus;

    var CateName = json.model.CateName;
    var Family = json.model.Family;

    var HeadID = json.model.HeadID;

    var IsShow = json.model.IsShow;
    
    localStorage.setItem("PriceType", json.model.PriceType);
    
    localStorage.setItem("redmoney", Price);

    var whole = '';

    if (json.model.PriceType == 0) {

        $('#Price').text('￥' + Price);
        whole = '￥';

    } else if (json.model.PriceType == 1) {

        $('#Price').text('￥' + Price);
        whole = '￥';

    } else if (json.model.PriceType == 2) {

        $('#Price').text(Price + '福音积分');
        $('.pay-unit').text('福音积分');
        whole = '福音积分';

    } else if (json.model.PriceType == 4) {

        $('#Price').text(Price + '福利积分');
        $('.pay-unit').text('福利积分');
        whole = '福利积分';

    }

    $('.price').text(Price);

    $('#Img').children().remove();
    $('#Img').append(Img);

    $('#Count').text('x ' + Count);

    var dispPrice = whole + (Count * Price);
    $('.whole').text(dispPrice);

    $('#Number').text(Number);

    $('.CreateTime').text(timeStampFormat(CreateTime));

    $('#proName').text(proName);

    $('#RecName').text(RecName);
    $('#RecAddress').text(RecAddress);
    if (Status == -1) {
        var str = '<a class="wait-pay">订单已取消</a>';
        $('.hdr-stat').append(str);
        $('.status').text('已取消');
    } else if (Status == 0) {
        var str = '<a class="wait-pay">等待卖家付款</a><a class="wait-type"> (若您在30分内未支付, 订单将被取消)</a>';
        $('.hdr-stat').append(str);
        $('.status').text('待付款');
    } else if (Status == 1 || Status == 2) {
        var str = '<a class="wait-pay">待收货</a>';
        $('.hdr-stat').append(str);
        $('.status').text('待收货');
    } else if (Status == 3 || Status == 4) {
        var str = '<a class="wait-pay">已签收</a>';
        $('.hdr-stat').append(str);
        $('.status').text('已签收');
    }
    appcan.window.publish('dispPrice', dispPrice);

}

var pay_stat = true;
$('#payOrder').click(function() {

    var orderStatus = localStorage.getItem("orderStatus");

    debug(orderStatus);

    if (orderStatus == -1 || orderStatus == '-1') {

        openToast('无效订单', 5000, 5, 0);

    } else if (orderStatus == 0 || orderStatus == '0') {

        /*
         if (pay_stat) {
         pay_stat = false;
         doPayOrder();
         }*/
        judgHasPayPwd();

    }

});

/*
 * 接口.58.支付已下但未付款的订单
 */
function doPayOrder(Pwd) {

    openToast('付款中, 请稍后...', 50000, 5, 1);

    var ajaxUrl = localStorage.getItem("ajaxUrl");

    var OID = localStorage.getItem("oid");

    debug(OID);

    appcan.ajax({
        url : ajaxUrl + "Orders/PayOrder",
        type : "POST",
        data : {
            OID : OID,
            Pwd : Pwd,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {
                
                closeToast();

                if (data.code == 1 || data.code == '1') {

                    openToast('付款成功', 5000, 5, 0);
                    appcan.window.publish('orderChg', 1);
                    setTimeout(function() {

                        appcan.window.close(-1);

                    }, 1500);

                } else if (data.code == -113 || data.code == '-113') {

                    setTimeout(function() {

                        appcan.window.publish('wrongPwd', 1);

                    }, 300);

                } else {

                    payMsgToast(data.msg, 5000, 5, 0);

                }

            } else {

                pay_stat = true;
                payMsgToast('付款失败', 5000, 5, 0);

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
            pay_stat = true;
            payMsgToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}

var cancel_stat = true;
$('#cancel').click(function() {

    var orderStatus = localStorage.getItem("orderStatus");

    debug(orderStatus);

    if (orderStatus == -1 || orderStatus == '-1') {

        openToast('无效订单', 5000, 5, 0);

    } else if (orderStatus == 0 || orderStatus == '0') {

        appcan.window.alert({
            title : '温馨提示',
            content : '您确定要取消订单吗?',
            buttons : ["确认", "取消"],
            callback : function(err, data, dataType, optld) {

                if (data == "0") {

                    if (cancel_stat) {
                        cancel_stat = false;
                        var Status = -1;
                        updtOrderStat(Status);
                    }

                }

            }
        });

    }

});

/*
 * 接口.64.更新订单状态
 */
function updtOrderStat(Status) {

    openToast('取消中, 请稍后...', 50000, 5, 1);

    var ajaxUrl = localStorage.getItem("ajaxUrl");

    var OID = localStorage.getItem("oid");

    debug(OID);

    appcan.ajax({
        url : ajaxUrl + "Orders/UpdateStatus",
        type : "POST",
        data : {
            OID : OID,
            Status : Status,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    openToast('取消订单成功', 5000, 5, 0);
                    setTimeout(function() {

                        appcan.window.publish('removeOrder', OID);
                        appcan.window.close(-1);

                    }, 2000);

                } else {

                    openToast(data.msg, 5000, 5, 0);
                    setTimeout(function() {

                        appcan.window.close(-1);

                    }, 2000);

                }

            } else {

                cancel_stat = true;
                openToast('取消订单失败', 5000, 5, 0);

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
            cancel_stat = true;
            openToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}