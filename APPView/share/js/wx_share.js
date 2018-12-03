var appId = "wx4df89fa99fa5a101";
appcan.ready(function() {

    var params = {
        windowName : "share"
    };
    uexWeiXin.setCallbackWindowName(JSON.stringify(params));

});

//检查微信是否已安装
function judgWxInstall(scene) {

    uexWeiXin.registerApp("wx4df89fa99fa5a101");
    var info = uexWeiXin.isWXAppInstalled();
    if (info) {

        wxShareLink(scene);

    } else {

        openToast("当前设备没有安装微信", 5000, 5, 0);

    };

}

function wxShareLink(scene) {

    //'{"thumbImg":"res://logo_fangxing@2x.png","wedpageUrl":"http://www.fbddd.com/account/regist","scene":1,"title":"你好,我是AppCan","description":"你好,我是AppCan描述"}'
    var thumbImg = 'res://logo_fangxing@2x.png';
    var wedpageUrl = 'http://www.fbddd.com/account/regist';
    //var scene = 1;
    var title = '欢迎您加入福包多';
    var description = '福包多多，福报多多';

    var shareData = '{"thumbImg":"' + thumbImg + '","wedpageUrl":"' + wedpageUrl + '","scene":' + scene + ',"title":"' + title + '","description":"' + description + '"}';
    //uexWeiXin.shareLinkContent(shareData);
    uexWeiXin.shareLinkContent(shareData, function(data) {

        alert(JSON.stringify(data));

        if (!data) {

            alert("分享成功");

        } else {

            alert("分享失败");

        }

    });

}