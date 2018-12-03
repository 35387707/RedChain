//接口.28.修改密码
//type 排行榜类型 1今日排行榜, 2当月 3总排行榜
//sv 发/抢红包类型
function getFuqiRank(type, sv) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "RedPacket/GetRankList",
        type : "POST",
        data : {
            type : type,
            sv : sv,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    getTuhaoRank(type, 1, data);

                } else {

                    openToast('修改密码失败', 3000, 5, 0);

                }

            } else {

                openToast('修改密码失败', 3000, 5, 0);

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

//sv 发/抢红包类型
function getTuhaoRank(type, sv, json) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "RedPacket/GetRankList",
        type : "POST",
        data : {
            type : type,
            sv : sv,
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    setRankList(json, data);

                } else {

                    openToast('修改密码失败', 3000, 5, 0);

                }

            } else {

                openToast('修改密码失败', 3000, 5, 0);

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

//迭代排行榜数据
function setRankList(fuqiJson, tuhaoJson) {

    var fileUrl = localStorage.getItem("fileUrl");

    var fuqiStr = '';
    var fuqiBot = '';
    var tuhaoStr = '';
    var tuhaoBot = '';
    
    var botTemp = '<div class="rank-line"></div>';

    var uid = localStorage.getItem("uid");

    $(fuqiJson.list).each(function(i, v) {

        if (i == 0) {

            fuqiStr += '<div class="rank-line"></div>';

            fuqiStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
            fuqiStr += '<div class="ub border-padd-bottom">';
            fuqiStr += '<div class="ub ub-ver rank-icon-sty">';
            fuqiStr += '<div class="ub-img rank-width rank-no1 rank-pos"></div>';
            fuqiStr += '</div>';
            fuqiStr += '<div class="rank-icon-sty">';
            fuqiStr += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
            fuqiStr += '</div>';
            fuqiStr += '<div class="ub ub-ver ub-f1 mar-ar1">';
            fuqiStr += '<div class="user-hdr overflow">';
            if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                fuqiStr += v.Phone;
            } else {
                fuqiStr += v.TrueName;
            }
            fuqiStr += '</div>';
            fuqiStr += '<div class="ut-s user-time">';
            fuqiStr += '收到福包 ';
            fuqiStr += v.Price+'元';
            fuqiStr += '</div>';
            fuqiStr += '</div>';

            fuqiStr += '<div class="cash-sty">';
            fuqiStr += '<div class="list-cash">';
            fuqiStr += '+ '+v.Price+'元';
            fuqiStr += '</div>';
            fuqiStr += '</div>';

            fuqiStr += '</div>';
            fuqiStr += '</div>';

            if (v.UID == uid) {

                fuqiBot += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
                fuqiBot += '<div class="ub border-padd-bottom">';
                fuqiBot += '<div class="ub ub-ver rank-icon-sty">';
                fuqiBot += '<div class="ub-img rank-width rank-no1 rank-pos"></div>';
                fuqiBot += '</div>';
                fuqiBot += '<div class="rank-icon-sty">';
                fuqiBot += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
                fuqiBot += '</div>';
                fuqiBot += '<div class="ub ub-ver ub-f1 mar-ar1">';
                fuqiBot += '<div class="user-hdr overflow">';
                if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                    fuqiBot += v.Phone;
                } else {
                    fuqiBot += v.TrueName;
                }
                fuqiBot += '</div>';
                fuqiBot += '<div class="ut-s user-time">';
                fuqiBot += '收到福包 ';
                fuqiBot += v.Price+'元';
                fuqiBot += '</div>';
                fuqiBot += '</div>';

                fuqiBot += '<div class="cash-sty">';
                fuqiBot += '<div class="list-cash">';
                fuqiBot += '+ '+v.Price+'元';
                fuqiBot += '</div>';
                fuqiBot += '</div>';

                fuqiBot += '</div>';
                fuqiBot += '</div>';

            }

        } else if (i == 1) {

            fuqiStr += '<div class="rank-line"></div>';

            fuqiStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
            fuqiStr += '<div class="ub border-padd-bottom">';
            fuqiStr += '<div class="ub ub-ver rank-icon-sty">';
            fuqiStr += '<div class="ub-img rank-width rank-no2 rank-pos"></div>';
            fuqiStr += '</div>';
            fuqiStr += '<div class="rank-icon-sty">';
            fuqiStr += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
            fuqiStr += '</div>';
            fuqiStr += '<div class="ub ub-ver ub-f1 mar-ar1">';
            fuqiStr += '<div class="user-hdr overflow">';
            if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                fuqiStr += v.Phone;
            } else {
                fuqiStr += v.TrueName;
            }
            fuqiStr += '</div>';
            fuqiStr += '<div class="ut-s user-time">';
            fuqiStr += '收到福包 ';
            fuqiStr += v.Price+'元';
            fuqiStr += '</div>';
            fuqiStr += '</div>';

            fuqiStr += '<div class="cash-sty">';
            fuqiStr += '<div class="list-cash">';
            fuqiStr += '+ '+v.Price+'元';
            fuqiStr += '</div>';
            fuqiStr += '</div>';

            fuqiStr += '</div>';
            fuqiStr += '</div>';

            if (v.UID == uid) {

                fuqiBot += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
                fuqiBot += '<div class="ub border-padd-bottom">';
                fuqiBot += '<div class="ub ub-ver rank-icon-sty">';
                fuqiBot += '<div class="ub-img rank-width rank-no2 rank-pos"></div>';
                fuqiBot += '</div>';
                fuqiBot += '<div class="rank-icon-sty">';
                fuqiBot += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
                fuqiBot += '</div>';
                fuqiBot += '<div class="ub ub-ver ub-f1 mar-ar1">';
                fuqiBot += '<div class="user-hdr overflow">';
                if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                    fuqiBot += v.Phone;
                } else {
                    fuqiBot += v.TrueName;
                }
                fuqiBot += '</div>';
                fuqiBot += '<div class="ut-s user-time">';
                fuqiBot += '收到福包 ';
                fuqiBot += v.Price+'元';
                fuqiBot += '</div>';
                fuqiBot += '</div>';

                fuqiBot += '<div class="cash-sty">';
                fuqiBot += '<div class="list-cash">';
                fuqiBot += '+ '+v.Price+'元';
                fuqiBot += '</div>';
                fuqiBot += '</div>';

                fuqiBot += '</div>';
                fuqiBot += '</div>';

            }

        } else if (i == 2) {

            fuqiStr += '<div class="rank-line"></div>';

            fuqiStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
            fuqiStr += '<div class="ub border-padd-bottom">';
            fuqiStr += '<div class="ub ub-ver rank-icon-sty">';
            fuqiStr += '<div class="ub-img rank-width rank-no3 rank-pos"></div>';
            fuqiStr += '</div>';
            fuqiStr += '<div class="rank-icon-sty">';
            fuqiStr += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
            fuqiStr += '</div>';
            fuqiStr += '<div class="ub ub-ver ub-f1 mar-ar1">';
            fuqiStr += '<div class="user-hdr overflow">';
            if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                fuqiStr += v.Phone;
            } else {
                fuqiStr += v.TrueName;
            }
            fuqiStr += '</div>';
            fuqiStr += '<div class="ut-s user-time">';
            fuqiStr += '收到福包 ';
            fuqiStr += v.Price+'元';
            fuqiStr += '</div>';
            fuqiStr += '</div>';

            fuqiStr += '<div class="cash-sty">';
            fuqiStr += '<div class="list-cash">';
            fuqiStr += '+ '+v.Price+'元';
            fuqiStr += '</div>';
            fuqiStr += '</div>';

            fuqiStr += '</div>';
            fuqiStr += '</div>';

            if (v.UID == uid) {

                fuqiBot += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
                fuqiBot += '<div class="ub border-padd-bottom">';
                fuqiBot += '<div class="ub ub-ver rank-icon-sty">';
                fuqiBot += '<div class="ub-img rank-width rank-no3 rank-pos"></div>';
                fuqiBot += '</div>';
                fuqiBot += '<div class="rank-icon-sty">';
                fuqiBot += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
                fuqiBot += '</div>';
                fuqiBot += '<div class="ub ub-ver ub-f1 mar-ar1">';
                fuqiBot += '<div class="user-hdr overflow">';
                if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                    fuqiBot += v.Phone;
                } else {
                    fuqiBot += v.TrueName;
                }
                fuqiBot += '</div>';
                fuqiBot += '<div class="ut-s user-time">';
                fuqiBot += '收到福包 ';
                fuqiBot += v.Price+'元';
                fuqiBot += '</div>';
                fuqiBot += '</div>';

                fuqiBot += '<div class="cash-sty">';
                fuqiBot += '<div class="list-cash">';
                fuqiBot += '+ '+v.Price+'元';
                fuqiBot += '</div>';
                fuqiBot += '</div>';

                fuqiBot += '</div>';
                fuqiBot += '</div>';

            }

        } else {

            fuqiStr += '<div class="rank-line"></div>';

            fuqiStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
            fuqiStr += '<div class="ub border-padd-bottom">';
            fuqiStr += '<div class="ub ub-ver rank-icon-sty">';
            fuqiStr += '<div class="ub-img rank-width rank-pos">';
            fuqiStr += i + 1;
            fuqiStr += '</div>'
            fuqiStr += '</div>';
            fuqiStr += '<div class="rank-icon-sty">';
            fuqiStr += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
            fuqiStr += '</div>';
            fuqiStr += '<div class="ub ub-ver ub-f1 mar-ar1">';
            fuqiStr += '<div class="user-hdr overflow">';
            if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                fuqiStr += v.Phone;
            } else {
                fuqiStr += v.TrueName;
            }
            fuqiStr += '</div>';
            fuqiStr += '<div class="ut-s user-time">';
            fuqiStr += '收到福包 ';
            fuqiStr += v.Price+'元';
            fuqiStr += '</div>';
            fuqiStr += '</div>';

            fuqiStr += '<div class="cash-sty">';
            fuqiStr += '<div class="list-cash">';
            fuqiStr += '+ '+v.Price+'元';
            fuqiStr += '</div>';
            fuqiStr += '</div>';

            fuqiStr += '</div>';
            fuqiStr += '</div>';

            if (v.UID == uid) {

                fuqiBot += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
                fuqiBot += '<div class="ub border-padd-bottom">';
                fuqiBot += '<div class="ub ub-ver rank-icon-sty">';
                fuqiBot += '<div class="ub-img rank-width rank-pos">';
                fuqiBot += i + 1;
                fuqiBot += '</div>'
                fuqiBot += '</div>';
                fuqiBot += '<div class="rank-icon-sty">';
                fuqiBot += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
                fuqiBot += '</div>';
                fuqiBot += '<div class="ub ub-ver ub-f1 mar-ar1">';
                fuqiBot += '<div class="user-hdr overflow">';
                if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                    fuqiBot += v.Phone;
                } else {
                    fuqiBot += v.TrueName;
                }
                fuqiBot += '</div>';
                fuqiBot += '<div class="ut-s user-time">';
                fuqiBot += '收到福包 ';
                fuqiBot += v.Price+'元';
                fuqiBot += '</div>';
                fuqiBot += '</div>';

                fuqiBot += '<div class="cash-sty">';
                fuqiBot += '<div class="list-cash">';
                fuqiBot += '+ '+v.Price+'元';
                fuqiBot += '</div>';
                fuqiBot += '</div>';

                fuqiBot += '</div>';
                fuqiBot += '</div>';

            }

        }

    });
    
    fuqiStr += botTemp;

    $(tuhaoJson.list).each(function(i, v) {

        if (i == 0) {

            tuhaoStr += '<div class="rank-line"></div>';

            tuhaoStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
            tuhaoStr += '<div class="ub border-padd-bottom">';
            tuhaoStr += '<div class="ub ub-ver rank-icon-sty">';
            tuhaoStr += '<div class="ub-img rank-width rank-no1 rank-pos"></div>';
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="rank-icon-sty">';
            tuhaoStr += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="ub ub-ver ub-f1 mar-ar1">';
            tuhaoStr += '<div class="user-hdr overflow">';
            if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                tuhaoStr += v.Phone;
            } else {
                tuhaoStr += v.TrueName;
            }
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="ut-s user-time">';
            tuhaoStr += '送出福包';
            tuhaoStr += v.Price+'元';
            tuhaoStr += '</div>';
            tuhaoStr += '</div>';

            tuhaoStr += '<div class="cash-sty">';
            tuhaoStr += '<div class="list-cash">';
            tuhaoStr += '- '+v.Price+'元';
            tuhaoStr += '</div>';
            tuhaoStr += '</div>';

            tuhaoStr += '</div>';
            tuhaoStr += '</div>';

            if (v.UID == uid) {

                tuhaoBot += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
                tuhaoBot += '<div class="ub border-padd-bottom">';
                tuhaoBot += '<div class="ub ub-ver rank-icon-sty">';
                tuhaoBot += '<div class="ub-img rank-width rank-no1 rank-pos"></div>';
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="rank-icon-sty">';
                tuhaoBot += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="ub ub-ver ub-f1 mar-ar1">';
                tuhaoBot += '<div class="user-hdr overflow">';
                if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                    tuhaoBot += v.Phone;
                } else {
                    tuhaoBot += v.TrueName;
                }
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="ut-s user-time">';
                tuhaoBot += '送出福包';
                tuhaoBot += v.Price+'元';
                tuhaoBot += '</div>';
                tuhaoBot += '</div>';

                tuhaoBot += '<div class="cash-sty">';
                tuhaoBot += '<div class="list-cash">';
                tuhaoBot += '- '+v.Price+'元';
                tuhaoBot += '</div>';
                tuhaoBot += '</div>';

                tuhaoBot += '</div>';
                tuhaoBot += '</div>';

            }

        } else if (i == 1) {

            tuhaoStr += '<div class="rank-line"></div>';

            tuhaoStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
            tuhaoStr += '<div class="ub border-padd-bottom">';
            tuhaoStr += '<div class="ub ub-ver rank-icon-sty">';
            tuhaoStr += '<div class="ub-img rank-width rank-no2 rank-pos"></div>';
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="rank-icon-sty">';
            tuhaoStr += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="ub ub-ver ub-f1 mar-ar1">';
            tuhaoStr += '<div class="user-hdr overflow">';
            if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                tuhaoStr += v.Phone;
            } else {
                tuhaoStr += v.TrueName;
            }
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="ut-s user-time">';
            tuhaoStr += '送出福包';
            tuhaoStr += v.Price+'元';
            tuhaoStr += '</div>';
            tuhaoStr += '</div>';
            
            tuhaoStr += '<div class="cash-sty">';
            tuhaoStr += '<div class="list-cash">';
            tuhaoStr += '- '+v.Price+'元';
            tuhaoStr += '</div>';
            tuhaoStr += '</div>';
                        
            tuhaoStr += '</div>';
            tuhaoStr += '</div>';

            if (v.UID == uid) {

                tuhaoBot += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
                tuhaoBot += '<div class="ub border-padd-bottom">';
                tuhaoBot += '<div class="ub ub-ver rank-icon-sty">';
                tuhaoBot += '<div class="ub-img rank-width rank-no2 rank-pos"></div>';
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="rank-icon-sty">';
                tuhaoBot += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="ub ub-ver ub-f1 mar-ar1">';
                tuhaoBot += '<div class="user-hdr overflow">';
                if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                    tuhaoBot += v.Phone;
                } else {
                    tuhaoBot += v.TrueName;
                }
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="ut-s user-time">';
                tuhaoBot += '送出福包';
                tuhaoBot += v.Price+'元';
                tuhaoBot += '</div>';
                tuhaoBot += '</div>';
                
                tuhaoBot += '<div class="cash-sty">';
                tuhaoBot += '<div class="list-cash">';
                tuhaoBot += '- '+v.Price+'元';
                tuhaoBot += '</div>';
                tuhaoBot += '</div>';

                tuhaoBot += '</div>';
                tuhaoBot += '</div>';

            }

        } else if (i == 2) {

            tuhaoStr += '<div class="rank-line"></div>';

            tuhaoStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
            tuhaoStr += '<div class="ub border-padd-bottom">';
            tuhaoStr += '<div class="ub ub-ver rank-icon-sty">';
            tuhaoStr += '<div class="ub-img rank-width rank-no3 rank-pos"></div>';
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="rank-icon-sty">';
            tuhaoStr += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="ub ub-ver ub-f1 mar-ar1">';
            tuhaoStr += '<div class="user-hdr overflow">';
            if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                tuhaoStr += v.Phone;
            } else {
                tuhaoStr += v.TrueName;
            }
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="ut-s user-time">';
            tuhaoStr += '送出福包';
            tuhaoStr += v.Price+'元';
            tuhaoStr += '</div>';
            tuhaoStr += '</div>';
            
            tuhaoStr += '<div class="cash-sty">';
            tuhaoStr += '<div class="list-cash">';
            tuhaoStr += '- '+v.Price+'元';
            tuhaoStr += '</div>';
            tuhaoStr += '</div>';
                        
            tuhaoStr += '</div>';
            tuhaoStr += '</div>';

            if (v.UID == uid) {

                tuhaoBot += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
                tuhaoBot += '<div class="ub border-padd-bottom">';
                tuhaoBot += '<div class="ub ub-ver rank-icon-sty">';
                tuhaoBot += '<div class="ub-img rank-width rank-no3 rank-pos"></div>';
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="rank-icon-sty">';
                tuhaoBot += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="ub ub-ver ub-f1 mar-ar1">';
                tuhaoBot += '<div class="user-hdr overflow">';
                if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                    tuhaoBot += v.Phone;
                } else {
                    tuhaoBot += v.TrueName;
                }
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="ut-s user-time">';
                tuhaoBot += '送出福包';
                tuhaoBot += v.Price+'元';
                tuhaoBot += '</div>';
                tuhaoBot += '</div>';
                
                tuhaoBot += '<div class="cash-sty">';
                tuhaoBot += '<div class="list-cash">';
                tuhaoBot += '- '+v.Price+'元';
                tuhaoBot += '</div>';
                tuhaoBot += '</div>';

                tuhaoBot += '</div>';
                tuhaoBot += '</div>';

            }

        } else {

            tuhaoStr += '<div class="rank-line"></div>';

            tuhaoStr += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
            tuhaoStr += '<div class="ub border-padd-bottom">';
            tuhaoStr += '<div class="ub ub-ver rank-icon-sty">';
            tuhaoStr += '<div class="ub-img rank-width rank-pos">';
            tuhaoStr += i + 1;
            tuhaoStr += '</div>'
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="rank-icon-sty">';
            tuhaoStr += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="ub ub-ver ub-f1 mar-ar1">';
            tuhaoStr += '<div class="user-hdr overflow">';
            if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                tuhaoStr += v.Phone;
            } else {
                tuhaoStr += v.TrueName;
            }
            tuhaoStr += '</div>';
            tuhaoStr += '<div class="ut-s user-time">';
            tuhaoStr += '送出福包';
            tuhaoStr += v.Price+'元';
            tuhaoStr += '</div>';
            tuhaoStr += '</div>';
            
            tuhaoStr += '<div class="cash-sty">';
            tuhaoStr += '<div class="list-cash">';
            tuhaoStr += '- '+v.Price+'元';
            tuhaoStr += '</div>';
            tuhaoStr += '</div>';

            tuhaoStr += '</div>';
            tuhaoStr += '</div>';

            if (v.UID == uid) {

                tuhaoBot += '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="rank-padd c-wh">';
                tuhaoBot += '<div class="ub border-padd-bottom">';
                tuhaoBot += '<div class="ub ub-ver rank-icon-sty">';
                tuhaoBot += '<div class="ub-img rank-width rank-pos">';
                tuhaoBot += i + 1;
                tuhaoBot += '</div>'
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="rank-icon-sty">';
                tuhaoBot += '<div class="ub-img1 rank-icon-width" style="background-image:url(' + fileUrl + v.HeadImg + ');"></div>';
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="ub ub-ver ub-f1 mar-ar1">';
                tuhaoBot += '<div class="user-hdr overflow">';
                if (v.TrueName == '' || v.TrueName == null || v.TrueName == undefined) {
                    tuhaoBot += v.Phone;
                } else {
                    tuhaoBot += v.TrueName;
                }
                tuhaoBot += '</div>';
                tuhaoBot += '<div class="ut-s user-time">';
                tuhaoBot += '送出福包';
                tuhaoBot += v.Price+'元';
                tuhaoBot += '</div>';
                tuhaoBot += '</div>';
                
                tuhaoBot += '<div class="cash-sty">';
                tuhaoBot += '<div class="list-cash">';
                tuhaoBot += '- '+v.Price+'元';
                tuhaoBot += '</div>';
                tuhaoBot += '</div>';

                tuhaoBot += '</div>';
                tuhaoBot += '</div>';

            }

        }

    });
    
    tuhaoStr += botTemp;
    
    $('#fuqi_list').children().remove();
    $('#fuqi_list').append(fuqiStr);

    $('#tuhao_list').children().remove();
    $('#tuhao_list').append(tuhaoStr);
    
    var rankHeight = $('.rank-slide').height();
    
    if (fuqiBot == '') {
        $('.fuqi-bottom').remove();
        $('.fuqi-cont').height(rankHeight);
    } else {
        $('.fuqi-mine').children().remove();
        $('.fuqi-mine').append(fuqiBot);
    }

    if (tuhaoBot == '') {
        $('.tuhao-bottom').remove();
        $('.tuhao-cont').height(rankHeight);
    } else {
        $('.tuhao-mine').children().remove();
        $('.tuhao-mine').append(tuhaoBot);
    }

}

