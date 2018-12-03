$('.calc-plus').click(function() {

    var calcNum = parseInt($('.calc-num').text().replace(/\s*/g, ""));
    var tmpNum = calcNum + 1;

    $('.calc-num').text(tmpNum);

    var price = parseInt($('#Price').data('price'));

    var calcPrice = parseInt($('.price').text().replace(/\s*/g, ""));
    var tmpPrice = calcPrice + price;

    appcan.window.publish('chgPrice', tmpPrice);

    $('.price').text(tmpPrice);

});
$('.calc-minus').click(function() {

    var calcNum = parseInt($('.calc-num').text().replace(/\s*/g, ""));
    var tmpNum = 1;

    if (calcNum > 1) {
        tmpNum = calcNum - 1;
    } else {
        openToast('当前已经是最小的购买数量', 3000, 5, 0);
    }

    $('.calc-num').text(tmpNum);

    var price = parseInt($('#Price').data('price'));

    var calcPrice = parseInt($('.price').text().replace(/\s*/g, ""));

    var tmpPrice = price;

    if (calcPrice > price) {
        tmpPrice = calcPrice - price;
    }

    appcan.window.publish('chgPrice', tmpPrice);

    $('.price').text(tmpPrice);

});
$('.addr-clk').click(function() {

    localStorage.setItem('addrEnter', 'order_pay');

    var localPack = localStorage.getItem('localPack');
    if (localPack == '1' || localPack == 1) {

        appcan.window.open({
            name : 'addr_mgt',
            data : '../addr/addr_mgt.html',
            aniId : 10,
        });

    } else {

        uexWindow.open({
            name : "addr_mgt",
            data : "../addr/addr_mgt.html",
            animID : 10,
            flag : 1024
        });

    }

});

//接口.57.0.设置商品详情
function setPayFooter(json) {

    debug(json);

    var fileUrl = localStorage.getItem("fileUrl");
    var BillTime = new Date().format("yyyy-MM-dd hh:mm:ss");

    $('#BillTime').text(BillTime);

    var Img = '';
    var Title = '';
    var Name = '';
    var Price = '';
    var PriceType = '';
    var RealPrice = '';

    Img = '<img src="' + fileUrl + json.model.Img + '" />';
    Title = json.model.Title;
    Name = json.model.Name;
    Price = json.model.Price;

    if (json.model.PriceType == 0) {

        $('#Price').text('￥' + Price);

    } else if (json.model.PriceType == 1) {

        $('#Price').text('￥' + Price);

    } else if (json.model.PriceType == 2) {

        $('#Price').text(Price + '福音积分');
        $('.pay-unit').text('福音积分');

    } else if (json.model.PriceType == 4) {

        $('#Price').text(Price + '福利积分');
        $('.pay-unit').text('福利积分');

    }

    $('.price').text(Price);

}

//接口.57.0.设置商品详情
function setPayGoods(json) {

    debug(json);

    var fileUrl = localStorage.getItem("fileUrl");
    var BillTime = new Date().format("yyyy-MM-dd hh:mm:ss");

    $('#BillTime').text(BillTime);

    var Img = '';
    var Title = '';
    var Name = '';
    var Price = '';
    var PriceType = '';
    var RealPrice = '';

    Img = '<img src="' + fileUrl + json.model.Img + '" />';
    Title = json.model.Title;
    Name = json.model.Name;
    Price = json.model.Price;

    if (json.model.PriceType == 0) {

        $('#Price').text('￥' + Price);

    } else if (json.model.PriceType == 1) {

        $('#Price').text('￥' + Price);

    } else if (json.model.PriceType == 2) {

        $('#Price').text(Price + '福音积分');
        $('.pay-unit').text('福音积分');

    } else if (json.model.PriceType == 4) {

        $('#Price').text(Price + '福利积分');
        $('.pay-unit').text('福利积分');

    }

    $('.price').text(Price);

    $('#Img').children().remove();
    $('#Img').append(Img);

    $('#Name').text(Name);
    $('#Title').text(Title);

    $('#Price').data('price', Price);
    $('#Price').data('price_type', json.model.PriceType);

    if (localStorage.getItem("GetUserAddress") == 1 || localStorage.getItem("GetUserAddress") == '1') {
        var defAddr = JSON.parse(localStorage.getItem("defAddr"));
        $('#recname').text(defAddr.recname);
        $('#phone').text(defAddr.phone);
        $('#address').text(defAddr.address);
        $('#address').data('addrid', defAddr.addrid);
    } else {
        getUserAddress(localStorage.getItem("uid"), 'order_pay');
    }

}

var order_stat = true;
$('#orderAndPay').click(function() {

    if (order_stat) {

        order_stat = false;
        judgOrderReq();

    }

});

/*
 * 接口.57.0.判断必填
 */
function judgOrderReq() {

    var req_arr = [];

    var addressID = $('#address').data('addrid');
    if (addressID == '' || addressID == null || addressID == undefined) {
        req_arr.push('收货地址');
    }

    var Payment = 0;

    var PriceType = $('#Price').data('price_type');

    var Remark = '';

    var ProID = localStorage.getItem("guid");

    var count = parseInt($('.calc-num').text().replace(/\s*/g, ""));
    if (count == 0 || count == '0' || count == '' || count == null || count == undefined) {
        req_arr.push('购买数量');
    }

    var temp = {
        addressID : addressID,
        Payment : Payment,
        PriceType : PriceType,
        Remark : Remark,
        ProID : ProID,
        count : count,
    }

    debug(temp);

    if (req_arr.length == 0 || req_arr == '') {

        //openToast('下单中, 请稍后...', 50000, 5, 1);
        //orderAndPay();

        //localStorage.setItem("PayInfo", JSON.stringify(temp));
        
        localStorage.setItem("redmoney", $('.price').text().replace(/\s*/g, ""));

        order_stat = true;
        judgHasPayPwd();

    } else {

        order_stat = true;
        openToast('请输入' + req_arr.join('、'), 3000, 5, 0);

    }

}

/*
 * 接口.57.下单并付款
 */
function orderAndPay(Pwd) {

    //地址id
    var addressID = $('#address').data('addrid');
    //支付方式
    var Payment = 0;
    //支付币种类型
    var PriceType = parseInt($('#Price').data('price_type'));
    //备注、留言
    var Remark = '';
    //商品id
    var ProID = localStorage.getItem("guid");
    //商品数量
    var count = parseInt($('.calc-num').text().replace(/\s*/g, ""));

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "Orders/OrderAndPay",
        type : "POST",
        data : {
            addressID : addressID,
            Payment : Payment,
            PriceType : PriceType,
            Remark : Remark,
            ProID : ProID,
            count : count,
            Pwd : Pwd,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    payMsgToast('下单成功', 5000, 5, 0);

                    setTimeout(function() {

                        payOrderBack();

                    }, 1500);

                } else if (data.code == -113 || data.code == '-113') {

                    //openToast(data.msg, 5000, 5, 0);

                    setTimeout(function() {

                        appcan.window.publish('wrongPwd', 1);

                    }, 300);

                    /*
                     appcan.window.publish('payBack', 1);
                     setTimeout(function() {

                     appcan.window.publish('payOrder', 1);

                     setTimeout(function() {

                     appcan.window.closePopover('pay_pwd');
                     localStorage.setItem("pay_pwd", 0);

                     }, 250);

                     }, 750);*/

                } else {

                    payMsgToast(data.msg, 5000, 5, 0);

                    setTimeout(function() {

                        payOrderBack();

                    }, 1500);

                }

            } else {

                order_stat = true;

                payMsgToast('下单失败', 5000, 5, 0);

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
            order_stat = true;
            payMsgToast(localStorage.getItem("errorMsg"), 5000, 5, 0);

        },
    });

}
