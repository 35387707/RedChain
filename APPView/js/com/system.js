//是否允许侧滑
function setSwipeCloseEnable(enable) {
    //Number,必选.传0表示禁止手势侧滑关闭,传1表示允许手势侧滑关闭
    //从上一个窗口打开必须传入flag: 1024
    var params = {
        enable : enable,
    };
    var paramStr = JSON.stringify(params);
    uexWindow.setSwipeCloseEnable(paramStr);
}

//2.0.打开底部导航栏
function openActnSht(fromPage) {

    var devCmy = uexDevice.getInfo('2');
    var devOs = uexDevice.getInfo('1');

    if (devCmy == 'Apple') {

        uexWindow.actionSheet({
            title : "图片上传",
            cancel : "取消",
            buttons : "打开相机,打开相册"
        }, function(index) {
            if (index == 0) {
                openCamera(fromPage);
            } else if (index == 1) {
                openPicker(fromPage);
            }
        });

    } else {

        uexWindow.actionSheet({
            title : "图片上传",
            cancel : "取消",
            buttons : "打开相机,打开相册"
        }, function(index) {
            if (index == 0) {
                openCamera(fromPage);
            } else if (index == 1) {
                openPicker(fromPage);
            }
        });

    }

};

//2.1.打开照相机
function openCamera(fromPage) {

    uexCamera.open(0, 100, function(picPath) {
        //debug(picPath);
        //$("img[data-pic_i='" + pic_i + "']").attr('src', picPath);
        //$('.dis-pic').attr('src', picPath);

        var imgIndx = new Date().format("yyyyMMddhhmmssSS");

        var imgArr = [];

        if (fromPage == 'personal_data') {

            imgArr.push(picPath);

        } else if (fromPage == 'chat') {

            imgArr.push(picPath);

        } else {
            var imgSgl = {
                imgIndx : imgIndx,
                imgPath : picPath,
            }

            imgArr.push(imgSgl);
        }

        if (fromPage == 'hbinfo' || fromPage == 'pub' || fromPage == 'luck_editor' || fromPage == 'vip_upgr') {

            setTimeout(function() {

                appcan.window.publish('chg_img', JSON.stringify(imgArr));

            }, 300);

        } else if (fromPage == 'chat') {

            doImgUpload(imgArr, 0, imgArr.length, 'chat');
            //doImgAppend(imgArr);

        } else if (fromPage == 'personal_data') {

            openToast('上传头像中...', 30000, 5, 1);
            doImgUpload(imgArr, 0, imgArr.length, 'personal_data');

        }

    });

};

//2.2.打开图片选择器
function openPicker(max, fromPage) {

    var data = {
        min : 0,
        max : max,
        quality : 0.8,
        detailedInfo : true,
        style : 1
    }

    uexImage.openPicker(data, function(error, info) {

        if (error == -1) {

            //alert("cancelled!");
            appcan.window.close(-1);

        } else if (error == 0) {

            //debug(info.data);
            //debug(info.data.length);
            //debug(info.detailedImageInfo);

            var imgArr = [];

            $(info.data).each(function(i, v) {

                var imgIndx = new Date().format("yyyyMMddhhmmssSS");

                if (fromPage == 'personal_data') {

                    imgArr.push(v);

                } else if (fromPage == 'chat') {

                    imgArr.push(v);

                } else {

                    var imgSgl = {
                        imgIndx : imgIndx + '_' + i,
                        imgPath : v,
                    }

                    imgArr.push(imgSgl);
                }

            });

            //debug(imgArr);

            if (fromPage == 'hbinfo' || fromPage == 'pub' || fromPage == 'luck_editor' || fromPage == 'vip_upgr') {

                if (imgArr.length != 0) {
                    appcan.window.publish('chg_img', JSON.stringify(imgArr));
                }

            } else if (fromPage == 'chat') {

                //doImgUpload(info.data);
                //doImgUpload(imgArr, 0, imgArr.length, 'chat');
                //doImgAppend(imgArr);
                if (imgArr.length != 0) {
                    appcan.window.publish('upldChatImg', JSON.stringify(imgArr));
                }

            } else if (fromPage == 'personal_data') {

                //openToast('上传头像中...', 30000, 5, 1);
                //doImgUpload(imgArr, 0, imgArr.length, 'personal_data');
                if (imgArr.length != 0) {
                    localStorage.setItem("imgArr", imgArr[0]);
                    appcan.window.publish('updtHeadImg', JSON.stringify(imgArr));
                }

            }

            appcan.window.close(-1);

        } else {

            openToast('打开相册出错', 3000, 5, 0);
            appcan.window.close(-1);

        }

    });

}

//2.3.打开照片浏览器
function openBrowser() {

    var localPack = localStorage.getItem('localPack');
    if (localPack == '1' || localPack == 1) {

        appcan.window.open({
            name : 'browser',
            data : '../pop/browser.html',
            aniId : 10,
        });

    } else {

        uexWindow.open({
            name : "browser",
            data : "../pop/browser.html",
            animID : 10,
            flag : 1024
        });

    }

}

//2.4.去到选择器页面
function toImgPicker(max, fromPage) {

    localStorage.setItem("max", max);
    localStorage.setItem('fromPage', fromPage);

    var localPack = localStorage.getItem('localPack');
    if (localPack == '1' || localPack == 1) {

        appcan.window.open({
            name : 'picker',
            data : '../pop/picker.html',
            aniId : 10,
        });

    } else {

        uexWindow.open({
            name : "picker",
            data : "../pop/picker.html",
            animID : 10,
            flag : 1024
        });

    }

}

//2.3.点击浏览图片
function dispCurImg(imgSgl, imgArr) {

    //debug(JSON.stringify(imgArr));
    if (imgArr == null || imgArr.length == 0 || imgArr == '' || imgArr == undefined) {

    } else {

        var startIndex = 0;

        for (var i in imgArr) {
            if (imgArr[i].src == imgSgl) {
                startIndex = i;
            }
        };

        var data = {
            displayActionButton : true,
            displayNavArrows : true,
            enableGrid : true,
            //startOnGrid:true,
            startIndex : startIndex,
            data : imgArr,
            style : 1,
        }

        var json = JSON.stringify(data);

        uexImage.openBrowser(json, function() {

            appcan.window.close(-1);

        });

    }

};

//actn.0.打开自定义的actnSheet
function openDefnActn(actionSheetList) {

    var x = 0;
    var y = 0;
    var width = 0;
    var height = 0;

    var data = {
        'actionSheet_style' : {
            'frameBgColor' : '#ffffff',
            'frameBroundColor' : '#ffffff',
            'frameBgImg' : '',
            'btnSelectBgImg' : 'res://actn_sheet_cancel_act@2x.png',
            'btnUnSelectBgImg' : 'res://actn_sheet_cancel@2x.png',
            'cancelBtnSelectBgImg' : 'res://actn_sheet_cancel_act@2x.png',
            'cancelBtnUnSelectBgImg' : 'res://actn_sheet_cancel@2x.png',
            'textSize' : 17,
            'textNColor' : '#ffffff',
            'textHColor' : '#ffffff',
            'cancelTextNColor' : '#ffffff',
            'cancelTextHColor' : '#ffffff',
            'actionSheetList' : actionSheetList
        }
    }

    var jsonData = JSON.stringify(data);
    uexActionSheet.open(x, y, width, height, jsonData);
    var nextActn = localStorage.getItem("nextActn");
    if (nextActn == 'sign_out') {
        uexActionSheet.onClickItem = signOutActn;
    }

}

//创建actnSheetList
function creaActnLst(actnArr) {

    var menuArr = [];
    $(actnArr).each(function(i, v) {

        var tempJson = {
            'name' : v
        }

        menuArr.push(tempJson);

    });

    return menuArr;
}

//safe exit actn clk
function signOutActn(index) {

    if (index == 0) {

        openToast('正在关闭应用程序...', 30000, 5, 1);
        setTimeout(function() {

            uexWidgetOne.exit(0);

        }, 2000);

    } else if (index == 1) {

        openToast('正在退出当前账号...', 30000, 5, 1);
        localStorage.setItem("uid", '');
        setTimeout(function() {

            uexWidgetOne.restart();

        }, 2000);

    }

}

/*
 * 获取设备厂商
 */
function getDevInfo(type) {

    return uexDevice.getInfo(type);

}

/*
 * 设置设备信息
 */
function setPhoneInfo() {

    if (localStorage.getItem("cur_dev_cmy") == '' || localStorage.getItem("cur_dev_cmy") == null || localStorage.getItem("cur_dev_cmy") == undefined) {
        localStorage.setItem("cur_dev_cmy", uexDevice.getInfo('2'));
    }

    if (localStorage.getItem("cur_dev_mde") == '' || localStorage.getItem("cur_dev_mde") == null || localStorage.getItem("cur_dev_mde") == undefined) {
        localStorage.setItem("cur_dev_mde", uexDevice.getInfo('17'));
    }

    if (localStorage.getItem("cur_dev_typ") == '' || localStorage.getItem("cur_dev_typ") == null || localStorage.getItem("cur_dev_typ") == undefined) {
        localStorage.setItem("cur_dev_typ", uexDevice.getInfo('12'));
    }

    if (localStorage.getItem("cur_dev_os") == '' || localStorage.getItem("cur_dev_os") == null || localStorage.getItem("cur_dev_os") == undefined) {
        localStorage.setItem("cur_dev_os", uexDevice.getInfo('1'));
    }

    if (localStorage.getItem("cur_dev_imei") == '' || localStorage.getItem("cur_dev_imei") == null || localStorage.getItem("cur_dev_imei") == undefined) {
        localStorage.setItem("cur_dev_imei", uexDevice.getInfo('10'));
    }

    if (localStorage.getItem("cur_dev_res") == '' || localStorage.getItem("cur_dev_res") == null || localStorage.getItem("cur_dev_res") == undefined) {
        localStorage.setItem("cur_dev_res", uexDevice.getInfo('18'));
    }

    /*
     var cur_dev_cmy = localStorage.getItem("cur_dev_cmy");
     var cur_dev_mde = localStorage.getItem("cur_dev_mde");
     var cur_dev_typ = localStorage.getItem("cur_dev_typ");
     if (cur_dev_typ == 0) {
     cur_dev_typ = 'iPhone';
     } else if (cur_dev_typ == 1) {
     cur_dev_typ = 'iPad';
     } else {
     cur_dev_typ = 'android';
     }
     var cur_dev_os = localStorage.getItem("cur_dev_os");
     var cur_dev_imei = localStorage.getItem("cur_dev_imei");
     var cur_dev_net = uexDevice.getInfo('13');
     if (cur_dev_net == 0) {
     cur_dev_net = 'WIFI';
     } else if (cur_dev_net == -1) {
     cur_dev_net = '无网络连接';
     } else {
     cur_dev_net = '流量';
     }
     var temp_rest = uexDevice.getInfo('14');
     var cur_dev_rest = '';
     if (temp_rest != null) {
     cur_dev_rest = temp_rest / 1024 / 1024 / 1024;
     } */

}
