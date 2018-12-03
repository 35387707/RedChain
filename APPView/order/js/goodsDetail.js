/*
 * 接口.52.获取商品列表
 * 参数说明:
 * 1.  ID   guid  商品ID, 必填
 */
function getGoodsDetail(uid, ID, fromPage) {
    
    debug(ID);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "Product/Detail",
        type : "POST",
        data : {
            ID : ID,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    localStorage.setItem("GoodsDetail", JSON.stringify(data));
                    //setGoodsDetail(uid, ID, fromPage, data);
                    isActnBegin(uid, ID, data, fromPage)

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

//接口.52.1.设置商品详情
function setGoodsDetail(uid, ID, json, actnJson, fromPage) {
    
    appcan.window.publish('goodsActn', JSON.stringify(actnJson));

    var fileUrl = localStorage.getItem("fileUrl");
    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    //debug(json);

    var ImgList = '';
    var Title = '';
    var Price = '';
    var PriceType = '';
    var RealPrice = '';
    var CompleteCount = '';

    ImgList += '<div class="swiper-slide goods-slide">';
    ImgList += '<div data-pic_p="' + fileUrl + json.model.Img + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="goods-img disp-pic" style="background-image:url(' + fileUrl + json.model.Img + ');"></div>';
    ImgList += '</div>';

    var imgArr = new Array();
    imgArr = json.model.ImgList.split(";");
    $(imgArr).each(function(k, r) {

        if (imgArr.length == 1) {

            if (imgArr[imgArr.length - 1] == '' || imgArr[imgArr.length - 1] == null || imgArr[imgArr.length - 1] == undefined) {

            } else {
                ImgList += '<div class="swiper-slide goods-slide">';
                ImgList += '<div data-pic_p="' + fileUrl + r + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="goods-img disp-pic" style="background-image:url(' + fileUrl + r + ');"></div>';
                ImgList += '</div>';

            }

        } else if (imgArr.length == 2) {

            if (imgArr[imgArr.length - 1] == '' || imgArr[imgArr.length - 1] == null || imgArr[imgArr.length - 1] == undefined) {

                if (k + 1 != imgArr.length) {
                    ImgList += '<div class="swiper-slide goods-slide">';
                    ImgList += '<div data-pic_p="' + fileUrl + r + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="goods-img disp-pic" style="background-image:url(' + fileUrl + r + ');"></div>';
                    ImgList += '</div>';
                }

            } else {
                ImgList += '<div class="swiper-slide goods-slide">';
                ImgList += '<div data-pic_p="' + fileUrl + r + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="goods-img disp-pic" style="background-image:url(' + fileUrl + r + ');"></div>';
                ImgList += '</div>';
            }

        } else {
            ImgList += '<div class="swiper-slide goods-slide">';
            ImgList += '<div data-pic_p="' + fileUrl + r + '" ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="goods-img disp-pic" style="background-image:url(' + fileUrl + r + ');"></div>';
            ImgList += '</div>';
        }

    });

    Title = json.model.Name;
    Price = json.model.Price;
    localStorage.setItem("PriceType", json.model.PriceType);
    if (json.model.PriceType == 0) {
        PriceType = '元';
    } else if (json.model.PriceType == 1) {
        PriceType = '元';
    } else if (json.model.PriceType == 2) {
        PriceType = '福音积分';
    } else if (json.model.PriceType == 4) {
        PriceType = '福利积分';
    }
    RealPrice = json.model.RealPrice;
    CompleteCount = json.model.CompleteCount;
    if (CompleteCount == '' || CompleteCount == null || CompleteCount == undefined) {
        CompleteCount = 0;
    }

    $('#ImgList').children().remove();
    $('#ImgList').append(ImgList);

    $('#Title').text(Title);
    $('#Price').text(Price);
    $('#PriceType').text(PriceType);
    $('#RealPrice').text(RealPrice);
    $('#CompleteCount').text(CompleteCount);

    $('.disp-pic').unbind();
    $('.disp-pic').click(function() {

        var picIndx = $(this).data('pic_p');

        //查看所有相关的图片
        var picArr = [];
        $('.disp-pic').each(function(j, s) {

            //debug($(this).data('pic_p'));
            //debug($(this).data('img_s'));

            if ($(this).data('pic_p')) {
                var picSgl = {
                    src : $(this).data('pic_p'),
                    desc : '',
                };
                picArr.push(picSgl);
            }

        });

        localStorage.setItem("imgSgl", picIndx);
        localStorage.setItem("imgArr", JSON.stringify(picArr));
        setTimeout(function() {

            openBrowser();

        }, 300);

    });

    var buylist = '';
    debug(json.buylist);
    $(json.buylist).each(function(i, v) {

        buylist += '<ul class="ub ub-ac tab-padd" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
        buylist += '<li class="tab-left-wd ub ub-ac ub-pc">';
        buylist += '<div class="ub-img1 tab-width" style="background-image:url(' + fileUrl + v.HeadImg1 + ');"></div>';
        buylist += '</li>';
        buylist += '<li class="ub-f1 ut-s tab-right-wd">';
        buylist += '<a class="tab-user">';
        if (v.UTrueName == '' || v.UTrueName == null || v.UTrueName == undefined) {
            buylist += v.Uphone;
        } else {
            buylist += v.UTrueName;
        }
        buylist += '</a>';
        buylist += '</li>';
        buylist += '<li class="tab-exc">';
        
        var timeArea = getDateDiff(timeStampFormat(v.UpdateTime), curTime, 'day');
        var timeJson = timeStampSplit(timeStampFormat(v.UpdateTime));
        if (timeArea == 0 || timeArea == '0') {
            if (buylist.day - timeJson.day == 1) {
                buylist += '昨天';
                buylist += ' ';
            }
        } else if (timeArea == 1 || timeArea == '1') {
            if (buylist.day - timeJson.day == 2) {
                buylist += timeJson.month;
                buylist += '月';
                buylist += timeJson.day;
                buylist += '日';
                buylist += ' ';
            } else if (curJson.day - timeJson.day == 1) {
                buylist += '昨天';
                buylist += ' ';
            }
        } else {
            buylist += timeJson.month;
            buylist += '月';
            buylist += timeJson.day;
            buylist += '日';
            buylist += ' ';
        }
        buylist += timeJson.quantum;
        buylist += timeJson.hour;
        buylist += ':';
        buylist += timeJson.minute;
        
        buylist += '</li>';
        buylist += '</ul>';

    });
    
    $('#buylist').children().remove();
    $('#buylist').append(buylist);

}