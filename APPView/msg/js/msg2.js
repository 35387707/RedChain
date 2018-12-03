//1.0.发送聊天内容入口
$('#send_msg').click(function() {

    ajaxMsgCont();

});

//0.1.打开sqlite的方法
function srchMsgBegin(uid, friend_uid, friend_status_id, msg_status_id) {
    var db = uexDataBaseMgr.open(uid + ".db");
    if (db != null) {
        debug('开始查找记录');
        srchMsgYear(db, uid, friend_uid, friend_status_id, msg_status_id);
    }
}

//1.1.发送聊天内容的逻辑
function ajaxMsgCont() {

    var timestamp = new Date().format("yyyy年MM月dd日hh时mm分ss秒SS毫秒");
    var time = timeStampSplit(timestamp);
    var uid = localStorage.getItem("uid");
    var msgCont = {
        uid : uid,
        friend_uid : 'friend_' + uid,
        msg_cont : '恭喜发财',
        msg_time_year : time.year,
        msg_time_month : time.month,
        msg_time_day : time.day,
        msg_time_quantum : time.quantum,
        msg_time_hour : time.hour,
        msg_time_minute : time.minute,
        msg_time_second : time.second,
        msg_time_msec : time.mesc,
        msg_time_normal : time.normal,
        msg_time_chinese : time.chinese,
        msg_time_remark : '',
    }

    alert(JSON.stringify(msgCont));

}

//2.1.获取聊天记录的年
function srchMsgYear(db, uid, friend_uid, friend_status_id, msg_status_id) {

    var json = [];

    var sql_sel = "select ";

    sql_sel += "total.total_num, self.uid_num, other.friend_uid_num, ";
    sql_sel += "total.msg_time_year  ";

    sql_sel += "from ";

    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as total_num, ";
    sql_sel += "msg.msg_time_year ";

    sql_sel += "from tbl_msg msg ";
    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year ";
    sql_sel += ") ";
    sql_sel += "total ";

    sql_sel += "left join ";

    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as uid_num, ";
    sql_sel += "msg.msg_time_year ";

    sql_sel += "from tbl_msg msg ";
    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year ";
    sql_sel += ") ";
    sql_sel += "self ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = self.msg_time_year ";

    sql_sel += "left join ";

    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as friend_uid_num,  ";
    sql_sel += "msg.msg_time_year ";

    sql_sel += "from tbl_msg msg  ";
    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + friend_uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year ";
    sql_sel += ") ";
    sql_sel += "other ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = other.msg_time_year ";

    debug(sql_sel);

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            //长度为0，代表没有该条记录 直接用这个data即可，不需要eval
            debug(jsonList);
            if (jsonList.length != 0) {

                $(jsonList).each(function(i, data) {

                    var isrt_json = {
                        total_num : data.total_num,
                        uid_num : data.uid_num,
                        friend_uid_num : data.friend_uid_num,
                        msg_time_year : data.msg_time_year,
                        msg_time_month : [],
                    }

                    json.push(isrt_json);

                });

            }

            srchMsgMonth(db, uid, friend_uid, friend_status_id, msg_status_id, json);

        } else {

            debug("查询year出错!");

        }

    });

}

//2.2.获取聊天记录的月
function srchMsgMonth(db, uid, friend_uid, friend_status_id, msg_status_id, json) {

    debug(json);

    var sql_sel = "select ";

    sql_sel += "total.total_num, self.uid_num, other.friend_uid_num, ";
    sql_sel += "total.msg_time_year, total.msg_time_month ";

    sql_sel += "from ";

    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as total_num, ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month ";

    sql_sel += "from tbl_msg msg ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month ";
    sql_sel += ") ";
    sql_sel += "total ";

    sql_sel += "left join ";
    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as uid_num, ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month ";

    sql_sel += "from tbl_msg msg  ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month ";
    sql_sel += ") ";
    sql_sel += "self ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = self.msg_time_year ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_month = self.msg_time_month ";

    sql_sel += "left join ";
    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as friend_uid_num,  ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month ";

    sql_sel += "from tbl_msg msg  ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + friend_uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month ";
    sql_sel += ") ";
    sql_sel += "other ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = other.msg_time_year ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_month = other.msg_time_month ";

    debug(sql_sel);

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            debug(jsonList);

            if (jsonList.length != 0) {

                $(json).each(function(i, data) {

                    $(jsonList).each(function(j, data_j) {

                        if (data_j.msg_time_year == data.msg_time_year) {

                            var isrt_json = {
                                total_num : data_j.total_num,
                                uid_num : data_j.uid_num,
                                friend_uid_num : data_j.friend_uid_num,
                                msg_time_month : data_j.msg_time_month,
                                msg_time_day : [],
                            }

                            data.msg_time_month.push(isrt_json);

                        }

                    });

                });

            }

            srchMsgDay(db, uid, friend_uid, friend_status_id, msg_status_id, json);

        } else {

            debug("查询month出错!");

        }

    });

}

//2.3.获取聊天记录的天
function srchMsgDay(db, uid, friend_uid, friend_status_id, msg_status_id, json) {

    debug(json);

    var sql_sel = "select ";

    sql_sel += "total.total_num, self.uid_num, other.friend_uid_num, ";
    sql_sel += "total.msg_time_year, total.msg_time_month, total.msg_time_day ";

    sql_sel += "from ";

    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as total_num, ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day ";

    sql_sel += "from tbl_msg msg ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day ";
    sql_sel += ") ";
    sql_sel += "total ";

    sql_sel += "left join ";
    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as uid_num, ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day ";

    sql_sel += "from tbl_msg msg  ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day ";
    sql_sel += ") ";
    sql_sel += "self ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = self.msg_time_year ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_month = self.msg_time_month ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_day = self.msg_time_day ";

    sql_sel += "left join ";
    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as friend_uid_num,  ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day ";

    sql_sel += "from tbl_msg msg  ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + friend_uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day ";
    sql_sel += ") ";
    sql_sel += "other ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = other.msg_time_year ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_month = other.msg_time_month ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_day = other.msg_time_day ";

    debug(sql_sel);

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            //长度为0，代表没有该条记录 直接用这个data即可，不需要eval
            debug(jsonList);
            if (jsonList.length != 0) {

                $(json).each(function(i, data) {

                    $(data.msg_time_month).each(function(j, data_j) {

                        $(jsonList).each(function(k, data_k) {

                            if (data_k.msg_time_month == data_j.msg_time_month && data_k.msg_time_year == data.msg_time_year) {

                                var isrt_json = {
                                    total_num : data_k.total_num,
                                    uid_num : data_k.uid_num,
                                    friend_uid_num : data_k.friend_uid_num,
                                    msg_time_day : data_k.msg_time_day,
                                    msg_time_quantum : [],
                                }

                                data_j.msg_time_day.push(isrt_json);

                            }

                        });

                    });

                });

            }

            srchMsgQuantum(db, uid, friend_uid, friend_status_id, msg_status_id, json);

        } else {

            debug("查询day出错!");

        }

    });

}

//2.4.获取聊天记录的时间段
function srchMsgQuantum(db, uid, friend_uid, friend_status_id, msg_status_id, json) {

    debug(json);

    var sql_sel = "select ";

    sql_sel += "total.total_num, self.uid_num, other.friend_uid_num, ";
    sql_sel += "total.msg_time_year, total.msg_time_month, total.msg_time_day, ";
    sql_sel += "total.msg_time_quantum ";

    sql_sel += "from ";

    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as total_num, ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum ";

    sql_sel += "from tbl_msg msg ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum ";
    sql_sel += ") ";
    sql_sel += "total ";

    sql_sel += "left join ";
    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as uid_num, ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum ";

    sql_sel += "from tbl_msg msg  ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum ";
    sql_sel += ") ";
    sql_sel += "self ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = self.msg_time_year ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_month = self.msg_time_month ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_day = self.msg_time_day ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_quantum = self.msg_time_quantum ";

    sql_sel += "left join ";
    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as friend_uid_num,  ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum ";

    sql_sel += "from tbl_msg msg  ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + friend_uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum ";
    sql_sel += ") ";
    sql_sel += "other ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = other.msg_time_year ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_month = other.msg_time_month ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_day = other.msg_time_day ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_quantum = other.msg_time_quantum ";

    debug(sql_sel);

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            //长度为0，代表没有该条记录 直接用这个data即可，不需要eval
            debug(jsonList);
            if (jsonList.length != 0) {

                $(json).each(function(i, data) {

                    $(data.msg_time_month).each(function(j, data_j) {

                        $(data_j.msg_time_day).each(function(k, data_k) {

                            $(jsonList).each(function(l, data_l) {

                                if (data_l.msg_time_day == data_k.msg_time_day && data_l.msg_time_month == data_j.msg_time_month && data_l.msg_time_year == data.msg_time_year) {

                                    var isrt_json = {
                                        total_num : data_l.total_num,
                                        uid_num : data_l.uid_num,
                                        friend_uid_num : data_l.friend_uid_num,
                                        msg_time_quantum : data_l.msg_time_quantum,
                                        msg_time_hour : [],
                                    }

                                    data_k.msg_time_quantum.push(isrt_json);

                                }

                            });

                        });

                    });

                });

            }

            srchMsgHour(db, uid, friend_uid, friend_status_id, msg_status_id, json);

        } else {

            debug("查询quantum出错!");

        }

    });

}

//2.5.获取聊天记录的时刻
function srchMsgHour(db, uid, friend_uid, friend_status_id, msg_status_id, json) {

    debug(json);

    var sql_sel = "select ";

    sql_sel += "total.total_num, self.uid_num, other.friend_uid_num, ";
    sql_sel += "total.msg_time_year, total.msg_time_month, total.msg_time_day, ";
    sql_sel += "total.msg_time_quantum, ";
    sql_sel += "total.msg_time_hour ";

    sql_sel += "from ";

    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as total_num, ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour ";

    sql_sel += "from tbl_msg msg ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour ";
    sql_sel += ") ";
    sql_sel += "total ";

    sql_sel += "left join ";
    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as uid_num, ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour ";

    sql_sel += "from tbl_msg msg  ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour ";
    sql_sel += ") ";
    sql_sel += "self ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = self.msg_time_year ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_month = self.msg_time_month ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_day = self.msg_time_day ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_quantum = self.msg_time_quantum ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_hour = self.msg_time_hour ";

    sql_sel += "left join ";
    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as friend_uid_num,  ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour ";

    sql_sel += "from tbl_msg msg  ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + friend_uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour ";
    sql_sel += ") ";
    sql_sel += "other ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = other.msg_time_year ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_month = other.msg_time_month ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_day = other.msg_time_day ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_quantum = other.msg_time_quantum ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_hour = other.msg_time_hour ";

    debug(sql_sel);

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            //长度为0，代表没有该条记录 直接用这个data即可，不需要eval
            debug(jsonList);
            if (jsonList.length != 0) {

                $(json).each(function(i, data) {

                    $(data.msg_time_month).each(function(j, data_j) {

                        $(data_j.msg_time_day).each(function(k, data_k) {

                            $(data_k.msg_time_quantum).each(function(l, data_l) {

                                $(jsonList).each(function(m, data_m) {

                                    if (data_m.msg_time_quantum == data_l.msg_time_quantum && data_m.msg_time_day == data_k.msg_time_day && data_m.msg_time_month == data_j.msg_time_month && data_m.msg_time_year == data.msg_time_year) {

                                        var isrt_json = {
                                            total_num : data_m.total_num,
                                            uid_num : data_m.uid_num,
                                            friend_uid_num : data_m.friend_uid_num,
                                            msg_time_hour : data_m.msg_time_hour,
                                            msg_time_quarter : [],
                                        }

                                        data_l.msg_time_hour.push(isrt_json);

                                    }

                                });

                            });

                        });

                    });

                });

            }

            srchMsgQuarter(db, uid, friend_uid, friend_status_id, msg_status_id, json);

        } else {

            debug("查询hour出错!");

        }

    });

}

//2.6.获取聊天记录的小时
function srchMsgQuarter(db, uid, friend_uid, friend_status_id, msg_status_id, json) {

    debug(json);

    var sql_sel = "select ";

    sql_sel += "total.total_num, self.uid_num, other.friend_uid_num, ";
    sql_sel += "total.msg_time_year, total.msg_time_month, total.msg_time_day, ";
    sql_sel += "total.msg_time_quantum, ";
    sql_sel += "total.msg_time_hour, total.msg_time_quarter ";

    sql_sel += "from ";

    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as total_num, ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour, msg.msg_time_quarter ";

    sql_sel += "from tbl_msg msg ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour, msg.msg_time_quarter ";
    sql_sel += ") ";
    sql_sel += "total ";

    sql_sel += "left join ";
    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as uid_num, ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour, msg.msg_time_quarter ";

    sql_sel += "from tbl_msg msg  ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour, msg.msg_time_quarter ";
    sql_sel += ") ";
    sql_sel += "self ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = self.msg_time_year ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_month = self.msg_time_month ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_day = self.msg_time_day ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_quantum = self.msg_time_quantum ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_hour = self.msg_time_hour ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_quarter = self.msg_time_quarter ";

    sql_sel += "left join ";
    sql_sel += "( ";
    sql_sel += "select ";
    sql_sel += "count(*) as friend_uid_num,  ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour, msg.msg_time_quarter ";

    sql_sel += "from tbl_msg msg  ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";
    sql_sel += "and msg.said_uid = '" + friend_uid + "' ";

    sql_sel += "group by ";
    sql_sel += "msg.msg_time_year, msg.msg_time_month, msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_hour, msg.msg_time_quarter ";
    sql_sel += ") ";
    sql_sel += "other ";
    sql_sel += "on ";
    sql_sel += "total.msg_time_year = other.msg_time_year ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_month = other.msg_time_month ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_day = other.msg_time_day ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_quantum = other.msg_time_quantum ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_hour = other.msg_time_hour ";
    sql_sel += "and ";
    sql_sel += "total.msg_time_quarter = other.msg_time_quarter ";

    debug(sql_sel);

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            //长度为0，代表没有该条记录 直接用这个data即可，不需要eval
            debug(jsonList);
            if (jsonList.length != 0) {

                $(json).each(function(i, data) {

                    $(data.msg_time_month).each(function(j, data_j) {

                        $(data_j.msg_time_day).each(function(k, data_k) {

                            $(data_k.msg_time_quantum).each(function(l, data_l) {

                                $(data_l.msg_time_hour).each(function(m, data_m) {

                                    $(jsonList).each(function(n, data_n) {

                                        if (data_n.msg_time_hour == data_m.msg_time_hour && data_n.msg_time_quantum == data_l.msg_time_quantum && data_n.msg_time_day == data_k.msg_time_day && data_n.msg_time_month == data_j.msg_time_month && data_n.msg_time_year == data.msg_time_year) {

                                            var isrt_json = {
                                                total_num : data_n.total_num,
                                                uid_num : data_n.uid_num,
                                                friend_uid_num : data_n.friend_uid_num,
                                                msg_time_quarter : data_n.msg_time_quarter,
                                                msg_cont : [],
                                            }

                                            data_m.msg_time_quarter.push(isrt_json);

                                        }

                                    });

                                });

                            });

                        });

                    });

                });

            }

            srchMsgCont(db, uid, friend_uid, friend_status_id, msg_status_id, json);

        } else {

            debug("查询quarter出错!");

        }

    });

}

//2.7.获取聊天记录的信息内容
function srchMsgCont(db, uid, friend_uid, friend_status_id, msg_status_id, json) {

    debug(json);

    var sql_sel = "select ";

    sql_sel += "msg.id, ";
    sql_sel += "msg.said_uid, ";
    sql_sel += "msg.uid, msg.friend_uid, ";

    sql_sel += "user.username, user.head_img as user_img, ";
    sql_sel += "friend.true_name as friend_name, friend.head_img as friend_img, friend.friend_status_id, friend.friend_status_name, ";

    sql_sel += "msg.msg_type_id, msg_typ.msg_type_name, ";
    sql_sel += "msg.msg_status_id, msg_stat.msg_status_name, ";
    sql_sel += "msg.msg_cont_type_id, msg_cont_typ.msg_cont_type_name, ";

    sql_sel += "msg.msg_cont, ";

    sql_sel += "msg.msg_time_chinese, ";
    sql_sel += "msg.msg_time_year, ";
    sql_sel += "msg.msg_time_month, ";
    sql_sel += "msg.msg_time_day, ";
    sql_sel += "msg.msg_time_quantum, ";
    sql_sel += "msg.msg_time_quarter, ";
    sql_sel += "msg.msg_time_minute, ";
    sql_sel += "msg.msg_time_second, ";
    sql_sel += "msg.msg_time_msec, ";
    sql_sel += "msg.msg_time_normal, ";
    sql_sel += "msg.msg_time_remark ";

    sql_sel += "from tbl_msg msg ";

    sql_sel += "left join tbl_user user on msg.uid = user.uid ";
    sql_sel += "left join (select friend_sub.id, friend_sub.uid, friend_sub.friend_uid, friend_sub.true_name, friend_sub.head_img, friend_sub.friend_status_id, friend_sub_stat.friend_status_name from tbl_friend friend_sub left join tbl_friend_status friend_sub_stat on friend_sub.friend_status_id = friend_sub_stat.id) as friend on msg.friend_uid = friend.friend_uid ";
    sql_sel += "left join tbl_msg_type msg_typ on msg.msg_type_id = msg_typ.id ";
    sql_sel += "left join tbl_msg_status msg_stat on msg.msg_status_id = msg_stat.id ";
    sql_sel += "left join tbl_msg_cont_type msg_cont_typ on msg.msg_cont_type_id = msg_cont_typ.id ";

    sql_sel += "where ";
    sql_sel += "msg.uid = '" + uid + "' ";
    sql_sel += "and msg.friend_uid = '" + friend_uid + "' ";
    sql_sel += "and friend.friend_status_id = " + friend_status_id + " ";
    sql_sel += "and msg.msg_status_id = " + msg_status_id + " ";

    sql_sel += "order by msg.id ";
    sql_sel += "asc ";

    debug(sql_sel);

    uexDataBaseMgr.select(db, sql_sel, function(error, jsonList) {
        if (!error) {

            //长度为0，代表没有该条记录 直接用这个data即可，不需要eval
            debug(jsonList);
            if (jsonList.length != 0) {

                $(json).each(function(i, data) {

                    $(data.msg_time_month).each(function(j, data_j) {

                        $(data_j.msg_time_day).each(function(k, data_k) {

                            $(data_k.msg_time_quantum).each(function(l, data_l) {

                                $(data_l.msg_time_hour).each(function(m, data_m) {

                                    $(data_m.msg_time_quarter).each(function(n, data_n) {

                                        $(jsonList).each(function(o, data_o) {

                                            // debug('data_o.msg_time_quarter: ' + data_o.msg_time_quarter);
                                            // debug('data_n.msg_time_quarter: ' + data_n.msg_time_quarter);
                                            // debug('data_m.msg_time_hour: ' + data_m.msg_time_hour);
                                            // debug('data_l.msg_time_quantum: ' + data_l.msg_time_quantum);
                                            // debug('data_k.msg_time_day: ' + data_k.msg_time_day);
                                            // debug('data_j.msg_time_month: ' + data_j.msg_time_month);
                                            // debug('data.msg_time_year: ' + data.msg_time_year);
                                            
                                            if (data_o.msg_time_quarter == data_n.msg_time_quarter) {
                                                debug('msg_time_quarter: 相等');
                                            } else if (data_o.msg_time_hour == data_m.msg_time_hour) {
                                                debug('msg_time_hour: 相等');
                                            } else if (data_o.msg_time_quantum == data_l.msg_time_quantum) {
                                                debug('msg_time_quantum: 相等');
                                            } else if (data_o.msg_time_day == data_k.msg_time_day) {
                                                debug('msg_time_day: 相等');
                                            } else if (data_o.msg_time_month == data_j.msg_time_month) {
                                                debug('msg_time_month: 相等');
                                            } else if (data_o.msg_time_year == data.msg_time_year) {
                                                debug('msg_time_year: 相等');
                                            }
                                            
                                            /*

                                            if (data_o.msg_time_quarter == data_n.msg_time_quarter && data_o.msg_time_hour == data_m.msg_time_hour && data_o.msg_time_quantum == data_l.msg_time_quantum && data_o.msg_time_day == data_k.msg_time_day && data_o.msg_time_month == data_j.msg_time_month && data_o.msg_time_year == data.msg_time_year) {

                                                debug('第9层');

                                                var isrt_json = {

                                                    id : data_o.id,

                                                    uid : data_o.uid,
                                                    friend_uid : data_o.friend_uid,
                                                    said_uid : data_o.said_uid,

                                                    username : data_o.username,
                                                    user_img : data_o.user_img,

                                                    friend_name : data_o.friend_name,
                                                    friend_img : data_o.friend_img,
                                                    friend_status_id : data_o.friend_status_id,
                                                    friend_status_name : data_o.friend_status_name,

                                                    msg_type_id : data_o.msg_type_id,
                                                    msg_type_name : data_o.msg_type_name,

                                                    msg_status_id : data_o.msg_status_id,
                                                    msg_status_name : data_o.msg_status_name,

                                                    msg_cont_type_id : data_o.msg_cont_type_id,
                                                    msg_cont_type_name : data_o.msg_cont_type_name,

                                                    msg_cont : data_o.msg_cont,

                                                    msg_time_chinese : data_o.msg_time_chinese,
                                                    msg_time_year : data_o.msg_time_year,
                                                    msg_time_month : data_o.msg_time_month,
                                                    msg_time_day : data_o.msg_time_day,
                                                    msg_time_quantum : data_o.msg_time_quantum,
                                                    msg_time_hour : data_o.msg_time_hour,
                                                    msg_time_quarter : data_o.msg_time_quarter,
                                                    msg_time_minute : data_o.msg_time_minute,
                                                    msg_time_second : data_o.msg_time_second,
                                                    msg_time_msec : data_o.msg_time_msec,
                                                    msg_time_normal : data_o.msg_time_normal,
                                                    msg_time_hour : data_o.msg_time_hour,
                                                    msg_time_remark : data_o.msg_time_remark,

                                                }

                                                data_n.msg_cont.push(isrt_json);

                                            } */

                                        });

                                    });

                                });

                            });

                        });

                    });

                });

            }

            dispMsgLog(db, json);

        } else {

            debug("查询cont出错!");

        }

    });

}

//3.1.迭代聊天记录
function dispMsgLog(db, json) {

    debug(json);

    if (json.length == 0 || json == '') {

        debug('当前没有查询到任何聊天记录');

    } else {

        var msgStr = '';
        $(json).each(function(i, v) {

            msgStr += '<div class="ub padd-right marg-bottom-normal">';
            msgStr += '<div class="ub ub-ac ub-f1 ub-pe">';
            msgStr += '<div class="text-width ub ub-ac border-right ulev-1-1 fontc-8 right-bg fw-nor">';
            msgStr += v.msg_cont;
            msgStr += '</div>';
            msgStr += '</div>';
            msgStr += '<div class="ub marg-left">';
            msgStr += '<img src="../msg/img/jjr.png" class="ub user-icon-width-normal">';
            msgStr += '</div>';
            msgStr += '</div>';

        });

    }

    //最后关闭数据库，释放内存
    uexDataBaseMgr.close(db);

}
