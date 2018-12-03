(function($) {
    appcan.button("#nav-left", "btn-act", function() {
    });
    appcan.button("#nav-right", "btn-act", function() {
    });

    appcan.ready(function() {
        showGoods();
    });
    function showGoods() {
        var arrData = [{
            "goods" : "\'img/logo_yuanxing@2x.png\'",
            "synopsis" : "玩赚福BAO多: 收福包",
            "sort" : "我最不忍看你, 背向我转面, 要走一刻请不必诸多眷恋",
        }, {
            "goods" : "\'img/logo_yuanxing@2x.png\'",
            "synopsis" : "玩赚福BAO多: 收福包",
            "sort" : "浮沉浪似人潮, 哪会没有思念, 你我伤心到讲不出再见",
        }, {
            "goods" : "\'img/logo_yuanxing@2x.png\'",
            "synopsis" : "玩赚福BAO多: 收福包",
            "sort" : "我最不忍看你, 背向我转面, 要走一刻请不必诸多眷恋",
        }, {
            "goods" : "\'img/logo_yuanxing@2x.png\'",
            "synopsis" : "玩赚福BAO多: 收福包",
            "sort" : "浮沉浪似人潮, 哪会没有思念, 你我伤心到讲不出再见",
        }, {
            "goods" : "\'img/logo_yuanxing@2x.png\'",
            "synopsis" : "玩赚福BAO多: 收福包",
            "sort" : "我最不忍看你, 背向我转面, 要走一刻请不必诸多眷恋",
        }, {
            "goods" : "\'img/logo_yuanxing@2x.png\'",
            "synopsis" : "玩赚福BAO多: 收福包",
            "sort" : "浮沉浪似人潮, 哪会没有思念, 你我伤心到讲不出再见",
        }];
        var listData = [];
        for (var i = 0,
            len = arrData.length; i < len; i++) {
            var list = {
                title : arrData[i].synopsis,
                icon : arrData[i].goods,
                describe : arrData[i].sort
            }
            listData.push(list);

        }
        var lv1 = appcan.listview({
            selector : "#showGoods",
            type : "thickLine",
            hasIcon : true,
            hasAngle : false,
            hasSubTitle : false,
            multiLine : 1,
            hasCheckbox : false,
            align : 'left'
        });
        lv1.set(listData);
        lv1.on('click', function(ele, context, obj, subobj) {

        })
    }

})($);
