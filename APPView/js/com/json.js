/*
 * json.1.判断字符串是否为json, 必须传入的为字符串才可以
 */
function isJSON(str) {

    if (jQuery.type(str) === 'object') {

        debug('一开始就是json了啊');
        return true;

    } else {

        if ( typeof str == 'string') {

            try {

                var obj = JSON.parse(str);
                if ( typeof obj == 'object' && obj) {
                    debug('是json');
                    return true;
                } else {
                    debug('不是json');
                    return false;
                }

            } catch (e) {
                //console.log('error：' + str + '!!!' + e);
                debug('不是json');
                return false;
            }

        } else {

            debug('不是json字符串');
            return 'string';

        }

    }
    //console.log('It is not a string!')
}

/*
 * json的切割方法
 */
function jsonSplit(data) {

    //var index = data.indexOf('"Content":"{');
    var index = data.indexOf('"Content":"');
    var cutStr = data.substring(index + 11, data.lastIndexOf('"}'));

    var delIndx = data.indexOf('"Content":');
    var findStr = data.substring(delIndx + 10, data.lastIndexOf('}'));
    var delStr = findStr.substring(1, findStr.lastIndexOf('"'));

    if (isJSON(cutStr) == true) {
        //var replaceStr = cutStr.replace(/"/g, '\\"');
        data = data.replace(findStr, delStr);
    }

    return data;

}

/*
 * 去掉前后空格的方法
 */
function trim(str) {

    var rslt = '';
    if (str == '' || str == null || str == undefined || str == NaN) {
        rslt = '';
    } else {
        rslt = str.replace(/(^\s*)|(\s*$)/g, "");
    }

    return rslt;
}

/*
 * 校验正负正数就返回true
 */
function isIntNumber(val) {
    var regPos = /^\d+(\.\d+)?$/;
    //非负浮点数
    var regNeg = /^(-(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*)))$/;
    //负浮点数
    if (regPos.test(val) || regNeg.test(val)) {
        return true;
    } else {
        return false;
    }
}

/*
 *  校验身份证
 */
function checkIdentity(identity) {
    var reg = /^[1-9]{1}[0-9]{14}$|^[1-9]{1}[0-9]{16}([0-9]|[xX])$/;
    if (reg.test(identity)) {
        return true;
    } else {
        return false;
    }
}

/*
 * 手机号中间隐藏为*号
 */
function hidePhoneMiddle(phone) {

    var reg = /^(\d{3})\d{4}(\d{4})$/;

    var rslt = phone.replace(reg, "$1****$2");

    return rslt;

}