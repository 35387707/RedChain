/*
 * 接口.56.新增收货地址
 */
function getUserAddress(uid, fromPage) {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    //登录的逻辑
    appcan.ajax({
        url : ajaxUrl + "Account/GetUserAddress",
        type : "POST",
        data : {
        },
        timeout : 10000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    localStorage.setItem("GetUserAddress", 1);
                    var db = uexDataBaseMgr.open(uid + ".db");
                    if (db != null) {
                        addrSrchOne(db, uid, 0, data.list.length, data, [], fromPage);
                    }

                } else {

                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                openToast('获取收货地址失败', 5000, 5, 0);

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
 * friend.0.单个查询的方法
 */
function addrSrchOne(db, uid, i, length, json, sql_tx, fromPage) {

    if (i < length) {

        var sql_sel = "select  * from tbl_address where uid = '" + uid + "' and addrid = '" + json.list[i].ID + "'";
        uexDataBaseMgr.select(db, sql_sel, function(error, data) {
            if (!error) {

                var sql = "";
                if (data.length == 0) {
                    sql = "insert into tbl_address (uid, addrid, recname, phone, areid, areacode, areaname, indexArr, address) values ('" + uid + "', '" + json.list[i].ID + "', '" + json.list[i].TrueName + "','" + json.list[i].Phone + "', '" + json.list[i].AreaID + "', '" + json.list[i].AreaCode + "', '', '', '" + json.list[i].Address + "')";
                } else {
                    sql = "update tbl_address set recname='" + json.list[i].TrueName + "', phone='" + json.list[i].Phone + "', areid='" + json.list[i].AreaID + "', areacode='" + json.list[i].AreaCode + "', address='" + json.list[i].Address + "' where uid = '" + uid + "' and addrid = '" + json.list[i].ID + "'";
                }
                sql_tx.push(sql);

                addrSrchOne(db, uid, i + 1, length, json, sql_tx, fromPage);

            } else {

                uexDataBaseMgr.close(db);

            }

        });

    } else {

        addrTxActn(db, uid, json, sql_tx, fromPage);

    }

}

/*
 * friend.1.
 */
function addrTxActn(db, uid, json, sql_tx, fromPage) {

    debug(sql_tx);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sql_tx), function(error) {

        if (error == 0) {

            addrSrchLocal(db, uid, fromPage);

        } else {

            uexDataBaseMgr.close(db);

        }

    });

}

/*
 * friend.1.本地开始的查找朋友的逻辑
 */
function addrSrchBegin(uid, fromPage) {

    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        addrSrchLocal(db, uid, fromPage);
    }

}

/*
 * friend.3.查找sqlite的好友
 */
function addrSrchLocal(db, uid, fromPage) {

    //var sql_sel = "select  * from tbl_address where uid = '" + uid + "' order by id desc";
    var sql_sel = "select * from tbl_address where uid = '" + uid + "'";
    uexDataBaseMgr.select(db, sql_sel, function(error, data) {
        if (!error) {

            debug(data);
            addrDispList(db, uid, data, fromPage);

        } else {

            debug('获取消息出错');

        }

    });

}

//暂时封印起来的逻辑
function addrDispList(db, uid, json, fromPage) {

    uexDataBaseMgr.close(db);

    var defAddr = '';

    var addr_list = '';
    if (json.length != 0) {

        $(json).each(function(j, s) {

            addr_list += '<div data-aid="' + s.addrid + '" class="addr-padd" ontouchstart="appcan.touch(&#39;btn-act&#39;)">';
            addr_list += '<div class="ub addr-style">';

            addr_list += '<div data-addrid="' + s.addrid + '" data-recname="' + s.recname + '" data-phone="' + s.phone + '" data-areid="' + s.areid + '" data-areacode="' + s.areacode + '" data-address="' + s.address + '" data-defaultaddr="' + s.defaultAddr + '" class="ub-f1 info-padd addr-sub">';
            addr_list += '<div class="nick-padd ub">';
            addr_list += '<div class="user-nick">';
            addr_list += '<span id="UsreName">';
            if (s.recname == '' || s.recname == null || s.recname == undefined) {
                addr_list += s.phone;
            } else {
                addr_list += s.recname;
            }
            addr_list += '</span>';
            addr_list += '<span class="user-phone">';
            addr_list += s.phone;
            addr_list += '</span>';
            addr_list += '</div>';

            addr_list += '<div class="def-sty">';
            if (s.defaultAddr == 1) {
                addr_list += '<span class="def-bor">默认</span>';
                defAddr = {
                    addrid : s.addrid,
                    recname : s.recname,
                    phone : s.phone,
                    areid : s.areid,
                    areacode : s.areacode,
                    address : s.address,
                    defaultAddr : s.defaultAddr,
                }
            }
            addr_list += '</div>';
            addr_list += '</div>';

            addr_list += '<div class="user-note">';
            addr_list += s.address;
            addr_list += '</div>';

            addr_list += '</div>';

            addr_list += '<div data-addrid="' + s.addrid + '" data-recname="' + s.recname + '" data-phone="' + s.phone + '" data-areid="' + s.areid + '" data-areacode="' + s.areacode + '" data-address="' + s.address + '" data-defaultaddr="' + s.defaultAddr + '" class="addr-icon addr-edit">';
            addr_list += '<div class="ub-img li-right-edit"></div>';
            addr_list += '</div>';

            addr_list += '</div>';
            addr_list += '</div>';

        });

        if (defAddr == '' || defAddr == null || defAddr == undefined) {

            defAddr = {
                addrid : json[json.length - 1].addrid,
                recname : json[json.length - 1].recname,
                phone : json[json.length - 1].phone,
                areid : json[json.length - 1].areid,
                areacode : json[json.length - 1].areacode,
                address : json[json.length - 1].address,
                defaultAddr : json[json.length - 1].defaultAddr,
            }

        }

    }

    debug(defAddr);
    localStorage.setItem("defAddr", JSON.stringify(defAddr));

    $('#addr_list').children().remove();
    $('#addr_list').append(addr_list);

    $('.addr-sub').unbind();
    $('.addr-sub').click(function() {

        var addrid = $(this).data('addrid');
        var recname = $(this).data('recname');
        var phone = $(this).data('phone');
        var areid = $(this).data('areid');
        var areacode = $(this).data('areacode');
        var address = $(this).data('address');
        var defaultAddr = $(this).data('defaultaddr');

        var addrJson = {
            addrid : addrid,
            recname : recname,
            phone : phone,
            areid : areid,
            areacode : areacode,
            address : address,
            defaultAddr : defaultAddr,
        }

        appcan.window.publish('defArr', JSON.stringify(addrJson));

        var addrEnter = localStorage.getItem('addrEnter');
        if (addrEnter == 'order_pay') {
            setTimeout(function() {
                appcan.window.publish('defBack', 1);
            }, 300);
        }

    });

    $('.addr-edit').unbind();
    $('.addr-edit').click(function() {

        var addrid = $(this).data('addrid');
        var recname = $(this).data('recname');
        var phone = $(this).data('phone');
        var areid = $(this).data('areid');
        var areacode = $(this).data('areacode');
        var address = $(this).data('address');
        var defaultAddr = $(this).data('defaultaddr');

        var addrJson = {
            addrid : addrid,
            recname : recname,
            phone : phone,
            areid : areid,
            areacode : areacode,
            address : address,
            defaultAddr : defaultAddr,
        }

        localStorage.setItem("addrJson", JSON.stringify(addrJson));

        var localPack = localStorage.getItem('localPack');
        if (localPack == '1' || localPack == 1) {

            appcan.window.open({
                name : 'addr_edit',
                data : '../addr/addr_edit.html',
                aniId : 10,
            });

        } else {

            uexWindow.open({
                name : "addr_edit",
                data : "../addr/addr_edit.html",
                animID : 10,
                flag : 1024
            });

        }

    });

    if (fromPage == 'order_pay') {
        $('#recname').text(defAddr.recname);
        $('#phone').text(defAddr.phone);
        $('#address').text(defAddr.address);
        $('#address').data('addrid', defAddr.addrid);
    }

}

function getAreaCode(areacode) {

    //1963,1964,1968

    var rslt = [];

    var province = 0;
    var city = 0;
    var county = 0;

    if (areacode == null || areacode == '' || areacode == undefined || areacode == NaN) {

        province = 1963;
        city = 1964;
        county = 1968;

    } else {

        var arr = new Array();
        arr = areacode.split(",");

        province = parseInt(arr[0]);
        city = parseInt(arr[1]);
        county = parseInt(arr[2]);

    }

    rslt.push(province);
    rslt.push(city);
    rslt.push(county);

    return rslt;

}

function getIndexArr(indexArr) {

    //5,3,9

    var rslt = [];

    var zero = 0;
    var one = 0;
    var two = 0;

    if (indexArr == null || indexArr == '' || indexArr == undefined || indexArr == NaN) {

        zero = 5;
        one = 3;
        two = 9;

    } else {

        var arr = new Array();
        arr = indexArr.split(",");

        zero = parseInt(arr[0]);
        one = parseInt(arr[1]);
        two = parseInt(arr[2]);

    }

    rslt.push(zero);
    rslt.push(one);
    rslt.push(two);

    return rslt;

}

/*
 * 设置地址
 */
function setAddrInfo(addrJson) {

    debug(addrJson);

    var areacode = getAreaCode(addrJson.areacode);

    debug(areacode);

    getProviceIndex(areacode, addrJson);

}

/*
 * 根据省份查询索引
 */
function getProviceIndex(areacode, addrJson) {

    var provinceIndex = 0;
    var provinceName = '';

    $(provinces).each(function(i, v) {

        if (v.id == areacode[0]) {
            provinceIndex = i;
            provinceName = v.value;
        }

    });

    getCityIndex(areacode, addrJson, provinceIndex, provinceName);

}

function getCityIndex(areacode, addrJson, provinceIndex, provinceName) {

    var cityIndex = 0;
    var cityName = '';
    $(places[provinceIndex].childs).each(function(i, v) {

        if (v.id == areacode[1]) {
            cityIndex = i;
            cityName = v.value;
        }

    });

    getCountyIndex(areacode, addrJson, provinceIndex, provinceName, cityIndex, cityName);

}

function getCountyIndex(areacode, addrJson, provinceIndex, provinceName, cityIndex, cityName) {

    var countyIndex = 0;
    var countyName = '';
    $(places[provinceIndex].childs[cityIndex].childs).each(function(i, v) {

        if (v.id == areacode[2]) {
            countyIndex = i;
            countyName = v.value;
        }

    });

    localStorage.setItem("indexArr", provinceIndex + ',' + cityIndex + ',' + countyIndex);

    $('#recname').val(addrJson.recname);
    $('#areacode').data('areid', addrJson.areid);
    //区域id, 也就是第三层的id
    $('#address').val(addrJson.address);
    $('#phone').val(addrJson.phone);
    $('#areacode').data('areacode', addrJson.areacode);
    $('#areacode').val(provinceName + ' ' + cityName + ' ' + countyName);
    $('#areacode').data('addrid', addrJson.addrid);

    var defaultAddr = addrJson.defaultAddr;
    if (defaultAddr == 1 || defaultAddr == '1') {
        appcan.updateSwitch($('.switch')).dataset.checked = "true";
    }

}

