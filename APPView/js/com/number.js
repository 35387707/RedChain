$.fn.onlyNumAlpha = function() {
    $(this).keypress(function(event) {
        var eventObj = event || e;
        var keyCode = eventObj.keyCode || eventObj.which;
        if ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122))
            return true;
        else
            return false;
    }).focus(function() {
        this.style.imeMode = 'disabled';
    }).bind("paste", function() {
        var clipboard = window.clipboardData.getData("Text");
        if (/^(\d|[a-zA-Z])+$/.test(clipboard))
            return true;
        else
            return false;
    });
};

$(".onlyNumAlpha").onlyNumAlpha();

/*
 $(".judg-i").bind("keypress", function(event) {
 var event = event || window.event;
 var getValue = $(this).val();
 //控制只能输入的值
 if (event.which && (event.which < 48 || event.which > 57) && event.which != 8) {
 event.preventDefault();
 return;
 }
 });*/

/*
 $(".deny-quote").keyup(function() {//keyup浜嬩欢澶勭悊
 $(this).val($(this).val().replace(/["'<>%;#$@,|:=/*)(&+]/, ''));
 }).bind("paste", function() {//CTR+V浜嬩欢澶勭悊
 $(this).val($(this).val().replace(/["'<>%;#$@,|:=/*)(&+]/, ''));
 }).css("ime-mode", "disabled");*/
$(".deny-quote").keyup(function() {//keyup事件处理
    $(this).val($(this).val().replace(/["']/, ''));
}).bind("paste", function() {//CTR+V事件处理
    $(this).val($(this).val().replace(/["']/, ''));
}).css("ime-mode", "disabled");
//CSS设置输入法不可用

$(".judg-i").bind("keypress", function(event) {
    var event = event || window.event;
    var getValue = $(this).val();
    //控制第一个不能输入小数点"."
    if (getValue.length == 0 && event.which == 46) {
        event.preventDefault();
        return;
    }
    if (getValue.length == 0 && event.which == 45) {
        $(this).val(parseInt('-'));
        return;
    }
    //控制只能输入一个小数点"."
    if (getValue.indexOf('.') != -1 && event.which == 46) {
        event.preventDefault();
        return;
    }
    if (getValue.length != 0 && event.which == 45) {
        event.preventDefault();
        return;
    }
    //控制只能输入一个"-"
    if (getValue.indexOf('-') != -1 && event.which == 45) {
        event.preventDefault();
        return;
    }

    //控制只能输入的值
    if (event.which && (event.which < 48 || event.which > 57) && event.which != 8 && event.which != 46 && event.which != 45) {
        event.preventDefault();
        return;
    }
});

$(".judg-num").bind("keypress", function(event) {
    var event = event || window.event;
    var getValue = $(this).val();
    //控制只能输入的值
    if (event.which && (event.which < 48 || event.which > 57) && event.which != 8) {
        event.preventDefault();
        return;
    }
});

//只能输入数字
$(".only-num").keyup(function() {
    var tmpVal = $(this).val();
    $(this).val(tmpVal.replace(/\D|^0/g, ''));
}).bind("paste", function() {
    var tmpVal = $(this).val();
    $(this).val(tmpVal.replace(/\D|^0/g, ''));
}).css("ime-mode", "disabled");

//只能输入数字以及小数点
$(".only-num-dec").keyup(function() {
    $(this).val($(this).val().replace(/[^0-9.]/g, ''));
}).bind("paste", function() {
    $(this).val($(this).val().replace(/[^0-9.]/g, ''));
    //粘贴的不是数字，则替换为''
    //或 return false; 禁用粘贴功能
}).css("ime-mode", "disabled");
//CSS设置输入法不可用

//自定义获取范围随机数的逻辑
function randomFrom(lowerValue, upperValue) {
    return Math.floor(Math.random() * (upperValue - lowerValue + 1) + lowerValue);
}