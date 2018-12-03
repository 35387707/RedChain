Date.prototype.format = function(format) {
    var date = {
        "M+" : this.getMonth() + 1,
        "d+" : this.getDate(),
        "h+" : this.getHours(),
        "m+" : this.getMinutes(),
        "s+" : this.getSeconds(),
        "q+" : Math.floor((this.getMonth() + 3) / 3),
        "S+" : this.getMilliseconds()
    };
    if (/(y+)/i.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length));
    }
    for (var k in date) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? date[k] : ("00" + date[k]).substr(("" + date[k]).length));
        }
    }
    return format;
};

//var timestamp = new Date().format("yyyy-MM-dd hh:mm:ss");
//username: v.substring(0, v.lastIndexOf(':')),
//password: v.substring(v.lastIndexOf(':') + 1),
function timeStampSplit(timestamp) {

    //timestamp.substring(timestamp.indexOf('(')+1, timestamp.lastIndexOf(')'));
    //imgName.substring(imgName.lastIndexOf('.') + 1)

    var tempDate = timestamp.substring(0, timestamp.indexOf(' '));
    var tempTime = timestamp.substring(timestamp.lastIndexOf(' ') + 1);
    var year = tempDate.substring(0, tempDate.indexOf('-'));
    var month = tempDate.substring(tempDate.indexOf('-') + 1, tempDate.lastIndexOf('-'));
    var day = tempDate.substring(tempDate.lastIndexOf('-') + 1);

    var hour = tempTime.substring(0, tempTime.indexOf(':'));
    var minute = tempTime.substring(tempTime.indexOf(':') + 1, tempTime.lastIndexOf(':'));
    var second = tempTime.substring(tempTime.lastIndexOf(':') + 1);

    var quantum = '';
    hour = parseInt(hour);
    if (hour < 6) {
        quantum = '凌晨';
    } else if (hour < 9) {
        quantum = '早上';
    } else if (hour < 12) {
        quantum = '上午';
    } else if (hour < 14) {
        quantum = '中午';
    } else if (hour < 17) {
        quantum = '下午';
    } else if (hour < 19) {
        quantum = '傍晚';
    } else if (hour < 22) {
        quantum = '晚上';
    } else {
        quantum = '深夜';
    }

    minute = parseInt(minute);

    var half = '';
    if (minute <= 30) {
        half = '前半个小时';
    } else {
        half = '后半个小时';
    }

    var quarter = '';
    if (minute <= 15) {
        quarter = '一刻钟';
    } else if (minute <= 30) {
        quarter = '二刻钟';
    } else if (minute <= 45) {
        quarter = '三刻钟';
    } else {
        quarter = '四刻钟';
    }

    var level = '';
    if (minute < 10) {
        level = '0~9';
    } else if (minute < 20) {
        level = '10~19';
    } else if (minute < 30) {
        level = '20~29';
    } else if (minute < 40) {
        level = '30~39';
    } else if (minute < 50) {
        level = '40~49';
    } else {
        level = '50~59';
    }

    //alert(half);
    //alert(quarter);
    //alert(level);

    if (month.toString().length == 1) {
        month = '0' + month;
    }
    if (day.toString().length == 1) {
        day = '0' + day;
    }
    if (hour.toString().length == 1) {
        hour = '0' + hour;
    }
    if (minute.toString().length == 1) {
        minute = '0' + minute;
    }
    if (second.toString().length == 1) {
        second = '0' + second;
    }

    var json = {
        whole : timestamp,
        year : year,
        month : month,
        day : day,
        quantum : quantum,
        hour : hour,
        // half : half,
        // quarter : quarter,
        // level : level,
        minute : minute,
        second : second,
    }

    return json;

}

function msgTimeStampSplit(timestamp) {

    //timestamp.substring(timestamp.indexOf('(')+1, timestamp.lastIndexOf(')'));
    //imgName.substring(imgName.lastIndexOf('.') + 1)

    var tempDate = timestamp.substring(0, timestamp.indexOf(' '));
    var tempTime = timestamp.substring(timestamp.lastIndexOf(' ') + 1);
    var year = tempDate.substring(0, tempDate.indexOf('-'));
    var month = tempDate.substring(tempDate.indexOf('-') + 1, tempDate.lastIndexOf('-'));
    var day = tempDate.substring(tempDate.lastIndexOf('-') + 1);

    var hour = tempTime.substring(0, tempTime.indexOf(':'));
    var minute = tempTime.substring(tempTime.indexOf(':') + 1, tempTime.lastIndexOf(':'));
    var second = tempTime.substring(tempTime.lastIndexOf(':') + 1, tempTime.lastIndexOf('.'));

    var quantum = '';
    hour = parseInt(hour);
    if (hour < 6) {
        quantum = '凌晨';
    } else if (hour < 9) {
        quantum = '早上';
    } else if (hour < 12) {
        quantum = '上午';
    } else if (hour < 14) {
        quantum = '中午';
    } else if (hour < 17) {
        quantum = '下午';
    } else if (hour < 19) {
        quantum = '傍晚';
    } else if (hour < 22) {
        quantum = '晚上';
    } else {
        quantum = '深夜';
    }

    minute = parseInt(minute);

    var half = '';
    if (minute <= 30) {
        half = '前半个小时';
    } else {
        half = '后半个小时';
    }

    var quarter = '';
    if (minute <= 15) {
        quarter = '一刻钟';
    } else if (minute <= 30) {
        quarter = '二刻钟';
    } else if (minute <= 45) {
        quarter = '三刻钟';
    } else {
        quarter = '四刻钟';
    }

    var level = '';
    if (minute < 10) {
        level = '0~9';
    } else if (minute < 20) {
        level = '10~19';
    } else if (minute < 30) {
        level = '20~29';
    } else if (minute < 40) {
        level = '30~39';
    } else if (minute < 50) {
        level = '40~49';
    } else {
        level = '50~59';
    }

    //alert(half);
    //alert(quarter);
    //alert(level);

    if (month.toString().length == 1) {
        month = '0' + month;
    }
    if (day.toString().length == 1) {
        day = '0' + day;
    }
    if (hour.toString().length == 1) {
        hour = '0' + hour;
    }
    if (minute.toString().length == 1) {
        minute = '0' + minute;
    }
    if (second.toString().length == 1) {
        second = '0' + second;
    }

    var json = {
        whole : timestamp,
        year : year,
        month : month,
        day : day,
        quantum : quantum,
        hour : hour,
        // half : half,
        // quarter : quarter,
        // level : level,
        minute : minute,
        second : second,
    }

    return json;

}

function pageTimeStampSplit(timestamp) {

    //timestamp.substring(timestamp.indexOf('(')+1, timestamp.lastIndexOf(')'));
    //imgName.substring(imgName.lastIndexOf('.') + 1)

    var tempDate = timestamp.substring(0, timestamp.indexOf(' '));
    var tempTime = timestamp.substring(timestamp.lastIndexOf(' ') + 1);
    var year = tempDate.substring(0, tempDate.indexOf('-'));
    var month = tempDate.substring(tempDate.indexOf('-') + 1, tempDate.lastIndexOf('-'));
    var day = tempDate.substring(tempDate.lastIndexOf('-') + 1);

    var hour = tempTime.substring(0, tempTime.indexOf(':'));
    var minute = tempTime.substring(tempTime.indexOf(':') + 1, tempTime.lastIndexOf(':'));
    var second = tempTime.substring(tempTime.lastIndexOf(':') + 1);

    var quantum = '';
    hour = parseInt(hour);
    if (hour < 6) {
        quantum = '凌晨';
    } else if (hour < 9) {
        quantum = '早上';
    } else if (hour < 12) {
        quantum = '上午';
    } else if (hour < 14) {
        quantum = '中午';
    } else if (hour < 17) {
        quantum = '下午';
    } else if (hour < 19) {
        quantum = '傍晚';
    } else if (hour < 22) {
        quantum = '晚上';
    } else {
        quantum = '深夜';
    }

    minute = parseInt(minute);

    var half = '';
    if (minute <= 30) {
        half = '前半个小时';
    } else {
        half = '后半个小时';
    }

    var quarter = '';
    if (minute <= 15) {
        quarter = '一刻钟';
    } else if (minute <= 30) {
        quarter = '二刻钟';
    } else if (minute <= 45) {
        quarter = '三刻钟';
    } else {
        quarter = '四刻钟';
    }

    var level = '';
    if (minute < 10) {
        level = '0~9';
    } else if (minute < 20) {
        level = '10~19';
    } else if (minute < 30) {
        level = '20~29';
    } else if (minute < 40) {
        level = '30~39';
    } else if (minute < 50) {
        level = '40~49';
    } else {
        level = '50~59';
    }

    //alert(half);
    //alert(quarter);
    //alert(level);

    if (day.toString().length == 1) {
        day = '0' + day;
    }
    if (hour.toString().length == 1) {
        hour = '0' + hour;
    }
    if (minute.toString().length == 1) {
        minute = '0' + minute;
    }
    if (second.toString().length == 1) {
        second = '0' + second;
    }

    var json = {
        whole : timestamp,
        year : year,
        month : month,
        day : day,
        quantum : quantum,
        hour : hour,
        // half : half,
        // quarter : quarter,
        // level : level,
        minute : minute,
        second : second,
    }

    return json;

}

function timeStampFormat(timestamp) {
    //shijianchuo是整数，否则要parseInt转换

    //切割字符串  /Date();
    //v.substring(0, v.lastIndexOf(':'));
    //v.substring(v.lastIndexOf(':') + 1);
    if (timestamp.indexOf('/Date(') != -1) {
        timestamp = timestamp.substring(timestamp.indexOf('(') + 1, timestamp.lastIndexOf(')'));
    }

    var time = new Date(parseInt(timestamp));
    return time.format("yyyy-MM-dd hh:mm:ss");

}

//alert(getDateDiff("2016-03-28 10:30:22","2016-03-29 10:38:22","minute"));
function getDateDiff(startTime, endTime, diffType) {
    //将xxxx-xx-xx的时间格式，转换为 xxxx/xx/xx的格式
    startTime = startTime.replace(/\-/g, "/");
    endTime = endTime.replace(/\-/g, "/");
    //将计算间隔类性字符转换为小写
    diffType = diffType.toLowerCase();
    var sTime = new Date(startTime);
    //开始时间
    var eTime = new Date(endTime);
    //结束时间
    //作为除数的数字
    var timeType = 1;
    switch (diffType) {
    case"second":
        timeType = 1000;
        break;
    case"minute":
        timeType = 1000 * 60;
        break;
    case"hour":
        timeType = 1000 * 3600;
        break;
    case"day":
        timeType = 1000 * 3600 * 24;
        break;
    default:
        break;
    }
    return parseInt((eTime.getTime() - sTime.getTime()) / parseInt(timeType));
}

function msgTimeSplLast(timestamp) {

    var newTimeStr = timestamp.substring(0, timestamp.indexOf('.'));

    return newTimeStr;

}
