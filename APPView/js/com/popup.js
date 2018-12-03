//commonAlert.1.Android、ios统一的commonAlert提示
function commonAlert(msg) {
    appcan.window.alert({
        title : '提示',
        content : msg,
        buttons : ["确认"],
        callback : function(err, data, dataType, optld) {

        }
    });
}

function backToLast(db, msg) {
    appcan.window.closeToast();
    appcan.window.alert({
        title : '提示',
        content : msg,
        buttons : ["确认"],
        callback : function(err, data, dataType, optld) {

            uexDataBaseMgr.close(db);
            appcan.window.close(-1);

        }
    });
}

function errorAlert(db, msg) {

    appcan.window.closeToast();
    $('.cover').hide();
    uexDataBaseMgr.close(db);
    appcan.window.alert({
        title : '提示',
        content : msg,
        buttons : ["确认"],
        callback : function(err, data, dataType, optld) {

        }
    });

}

function simpleHide(db) {

    uexDataBaseMgr.close(db);
    appcan.window.closeToast();
    $('.cover').hide();

}

function openToast(msg, seconds, position, type) {

    appcan.window.openToast(msg, seconds, position, type);

}

function closeToast() {
    appcan.window.closeToast();
}

function showLoading(msg, type) {

    var bodyWidth = localStorage.getItem("bodyWidth");
    var bodyHeight = localStorage.getItem("bodyHeight");

    appcan.window.openPopover({
        name : 'loading',
        dataType : 0,
        url : "../pop/loading.html",
        top : 0,
        left : 0,
        width : bodyWidth,
        height : bodyHeight,
    });

    /*
     uexWindow.open({
     name : "loading",
     data : "../pop/loading.html",
     animID : 5,
     flag : 256
     });*/

    setTimeout(function() {

        appcan.window.openToast(msg, 1000000, 5, type);

    }, 150);

}

function initError() {

    var localPack = localStorage.getItem('localPack');
    if (localPack == '0' || localPack == 0) {
        appcan.window.publish('closLoadingView', 1);
    }
    //closLoadingView
    localStorage.setItem("initRedPacket", 0);

}

function cnctAgain() {

    appcan.window.closeToast();

    var localPack = localStorage.getItem('localPack');
    if (localPack == '0' || localPack == 0) {
        appcan.window.publish('closLoadingView', 1);
    }

    setTimeout(function() {

        appcan.window.publish('cnctAgain', 1);

    }, 300);

}

function hideLoading(msg, seconds, position, type) {
    
    if (msg == '登录失败') {
        localStorage.setItem("uid", '');
    }

    appcan.window.closePopover("loading");
    setTimeout(function() {

        appcan.window.openToast(msg, seconds, position, type);

    }, 150);

}

function payMsgToast(msg, seconds, position, type) {

    appcan.window.closePopover('pay');
    localStorage.setItem("pay", 0);

    appcan.window.closePopover('pay_pwd');
    localStorage.setItem("pay_pwd", 0);

    setTimeout(function() {

        if (msg == '' || msg == null || msg == undefined) {

        } else {
            appcan.window.openToast(msg, seconds, position, type);
        }

    }, 300);

}

