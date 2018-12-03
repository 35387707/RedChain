//接口.25.获取钱包报表
function getWalletInfo() {

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Bill/GetInfo",
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

                    setWalletInfo(data);

                } else {

                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                openToast('获取钱包数据失败', 5000, 5, 0);

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

//接口.25.1.设置钱包数据
function setWalletInfo(json) {

    //用户余额
    var Balance = ''
    //佣金金额
    var Commission = '';
    //奖金
    var Rewards = '';
    //发红包总金额
    var SendReds = '';
    //收红包总金额
    var RecReds = '';
    //福利积分
    var FootQuan = '';
    //福音积分
    var Score = '';
    //用户红包池
    var RedBalance = '';
    //超级福包共享池
    var FubaoShare = ''
    //剩余福包数
    var BigFubaoShare = ''

    Balance = json.Balance;
    Commission = json.Commission;
    Rewards = json.Rewards;
    SendReds = json.SendReds;
    RecReds = json.RecReds;
    FootQuan = json.FootQuan;
    Score = json.Score;
    RedBalance = json.RedBalance;
    FubaoShare = json.FubaoShare;
    BigFubaoShare = json.BigFubaoShare;

    $('#Balance').text(Balance);
    $('#Commission').text(Commission);
    $('#Rewards').text(Rewards);
    $('#SendReds').text(SendReds);
    $('#RecReds').text(RecReds);
    $('#FootQuan').text(FootQuan);
    $('#Score').text(Score);
    $('#RedBalance').text(RedBalance);
    $('#FubaoShare').text(FubaoShare);
    $('#BigFubaoShare').text(BigFubaoShare);

}

//接口.27.获取账单列表(分页返回, 每页20条);
function getBillList(PageIndex, type, fromPage) {

    openToast('正在加载数据...', 30000, 5, 1);
    
    var spec = {
        PageIndex : PageIndex,
        type : type,
        fromPage : fromPage,
    }

    debug(spec);

    var ajaxUrl = localStorage.getItem("ajaxUrl");
    appcan.ajax({
        url : ajaxUrl + "Bill/List",
        type : "POST",
        data : {
            PageIndex : PageIndex,
            type : type,
        },
        timeout : 30000,
        dataType : "json",
        success : function(data) {
            //var data = eval("(" + data.replace(/[\r\n]/g, "") + ")")

            debug(data);
            if (data.length != 0) {

                if (data.code == 1 || data.code == '1') {

                    setBillList(PageIndex, type, data, fromPage);

                } else {

                    openToast(data.msg, 5000, 5, 0);

                }

            } else {

                openToast('获取列表失败', 5000, 5, 0);

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

//接口.27.0.判断当前的页码为多少
//pagecount
function judgCurPage(type, fromPage, item) {

    var curpage = '';
    //$('.photo-album').children("div:last-child").index()
    var pagecount = parseInt($('#content').data('pagecount'));
    var pageindex = parseInt($('#content').data('pageindex'));
    curpage = $('#content').children("div:last-child").index() + 1;

    var spec = {
        type : type,
        curpage : curpage,
        pageindex : pageindex,
        pagecount : pagecount,
        item : item,
        fromPage : fromPage,
    }
    
    debug(spec);

    if (curpage == 1) {

        appcan.frame.resetBounce(1);
        openToast('当前没有数据', 3000, 5, 0);

    } else {

        if (curpage < item) {

            appcan.frame.resetBounce(1);
            openToast('所有的数据已经加载完成', 3000, 5, 0);

        } else {

            if (pageindex == pagecount) {

                appcan.frame.resetBounce(1);
                openToast('所有的数据已经加载完成', 3000, 5, 0);

            } else {

                if (pageindex < pagecount) {

                    getBillList(pageindex + 1, type, fromPage);

                }

            }

        }

    }

}

//接口.27.1.迭代账单列表数据
function setBillList(PageIndex, type, json, fromPage) {

    //debug('PageIndex: ' + PageIndex);

    var fileUrl = localStorage.getItem("fileUrl");

    var curTime = new Date().format("yyyy-MM-dd hh:mm:ss");
    var curJson = timeStampSplit(curTime);

    var listStr = '';
    var lastStr = '';

    $('#content').data('pageindex', PageIndex);
    $('#content').data('pagecount', json.pagecount);

    if (PageIndex == json.pagecount) {

        lastStr += '<div class="end-nope-number">';
        lastStr += '没有更多了';
        lastStr += '</div>';

    }

    if (json.list.length == 0) {

        listStr += '<div class="end-nope-number">';
        listStr += '当前没有数据';
        listStr += '</div>';

        openToast('当前没有数据', 3000, 5, 0);

    } else {

        $(json.list).each(function(j, s) {

            listStr += '<div class="ubb ub bc-border c-wh">';

            listStr += '<ul ontouchstart="appcan.touch(&#39;btn-act&#39;)" class=" ub uinn-a1 c-wh2 ub-f1">';

            listStr += '<li class="ub pos_re">';
            listStr += '<div class="uh-app3 uw-app3 uc-a-app2 ub-img mar-ar1" style="background-image:url(';
            listStr += '../hb/img/logo_yuanxing@2x.png';
            listStr += ');"></div>';
            listStr += '</li>';

            listStr += '<li class="ub-f1 ub">';
            listStr += '<div class="ub ub-ver ub-f1 umar-r-infor">';
            listStr += ' <div class="t-bla ulev-app1">';
            listStr += s.Remark;
            listStr += '</div>';
            listStr += '<div class="ulev-4 ub-f1 umar-t">';
            listStr += '<span class="t-gra-infor2">';

            var timeArea = getDateDiff(timeStampFormat(s.CreateTime), curTime, 'day');
            var timeJson = timeStampSplit(timeStampFormat(s.CreateTime));
            if (timeArea == 0 || timeArea == '0') {
                if (curJson.day - timeJson.day == 1) {
                    listStr += '昨天';
                    listStr += ' ';
                }
            } else if (timeArea == 1 || timeArea == '1') {
                if (curJson.day - timeJson.day == 2) {
                    listStr += timeJson.month;
                    listStr += '月';
                    listStr += timeJson.day;
                    listStr += '日';
                    listStr += ' ';
                } else if (curJson.day - timeJson.day == 1) {
                    listStr += '昨天';
                    listStr += ' ';
                }
            } else {
                listStr += timeJson.month;
                listStr += '月';
                listStr += timeJson.day;
                listStr += '日';
                listStr += ' ';
            }

            listStr += timeJson.quantum;
            listStr += timeJson.hour;
            listStr += ':';
            listStr += timeJson.minute;

            listStr += '</span>';
            listStr += '</div>';
            listStr += '</div>';

            listStr += '<div class="ub-pe ulev-2 spec-middle">';
            listStr += '<div class="spec-color">';
            if (s.InOut == 1 || s.InOut == '1') {
                listStr += '+';
            } else {
                listStr += '-';
            }
            listStr += s.Val;
            if (type == 7 || type == 8) {
                listStr += '分';
            } else {
                listStr += '元';
            }
            listStr += '</div>';
            listStr += '</li>';

            listStr += '</ul>';

            listStr += '</div>';

        });

        openToast('加载数据成功', 3000, 5, 0);

    }

    listStr += lastStr;

    $('#content').append(listStr);

    appcan.frame.resetBounce(1);

}

