(function($) {

    var lv1 = appcan.listview({
        selector : "#listview1",
        type : "thinLine",
        hasIcon : false,
        hasAngle : false,
        hasControl : true,
    });
    lv1.set([{
        title : '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub t-bla ub-ac ">' + '<li class="ub-f1 ut-s ulev-app1">系统消息 (系统通知聊天消息)</li>' + '</ul>',
        "switchBtn" : {
            value : false,
            mini : true
        }
    }, {
        title : '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub t-bla ub-ac">' + '<li class="ub-f1 ut-s ulev-app1">声音</li>' + '</ul>',
        "switchBtn" : {
            value : true,
            mini : true
        }
    }, {
        title : '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub t-bla ub-ac">' + '<li class="ub-f1 ut-s ulev-app1">震动</li>' + '</ul>',
        "switchBtn" : {
            value : false,
            mini : true
        }
    }]);

    lv1.on("switch:change", function(ele, obj) {
        //  lv1.updateItem(ele,"title","Switch:"+obj.switchBtn.value);
    })
    var lv2 = appcan.listview({
        selector : "#listview2",
        type : "thinLine",
        hasIcon : false,
        hasAngle : false,
        hasControl : true,
    });
    lv2.set([{
        title : '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub t-bla ub-ac ">' + '<li class="ub-f1 ut-s ulev-app1">拨打后弹出反馈提示</li>' + '</ul>',
        "switchBtn" : {
            value : true,
            mini : true
        }
    }]);

    lv2.on("switch:change", function(ele, obj) {
        //  lv1.updateItem(ele,"title","Switch:"+obj.switchBtn.value);
    })
})($);
