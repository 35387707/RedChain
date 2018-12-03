/*
 * 优化逻辑, 每个用户只用一个数据库
 */
function judgDb(uid) {

    var db = uexDataBaseMgr.open("menu.db");
    if (db != null) {

        var sql_sel = "select ";
        sql_sel += "* ";
        sql_sel += "from tbl_user_db ";
        sql_sel += "where ";
        sql_sel += "uid = '" + uid + "' ";

        uexDataBaseMgr.select(db, sql_sel, function(error, data) {

            if (!error) {

                debug(data);
                if (data.length == 0) {

                    existDb(db, uid);

                } else {

                    uexDataBaseMgr.close(db);
                    getUserInfo(uid, 'login');

                }

            } else {

                uexDataBaseMgr.close(db);
                debug("judgDb出错");
                hideLoading('登录失败', 5000, 5, 0);

            }

        });

    }

}

function existDb(db, uid) {

    var ret = uexFileMgr.isFileExistByPath("wgts://fbd.db");
    debug(ret);
    if (ret) {

        debug('文件已存在, 直接重命名即可');
        renameDb(db, uid);

    } else {

        debug('文件不存在, 还在去拷贝一个吧');
        copyDb(db, uid);

    }

}

function copyDb(db, uid) {

    uexFileMgr.copy({
        src : "res://fbd.db",
        target : "wgts://"
    }, function(error) {
        if (!error) {

            renameDb(db, uid);

        } else {

            uexDataBaseMgr.close(db);
            debug("copyDb出错");
            hideLoading('登录失败', 5000, 5, 0);

        }
    });

}

function renameDb(db, uid) {

    var data = {
        oldFilePath : "wgts://fbd.db",
        newFilePath : "wgts://" + uid + ".db"
    }
    
    debug(data);

    uexFileMgr.renameFile(JSON.stringify(data), function(err) {
        if (!err) {

            addDb(db, uid);

        } else {

            if (getDevInfo(2) == 'Apple') {

                //renameDb(db, uid);

            } else {
            }

            uexDataBaseMgr.close(db);
            debug("renameDb出错");
            hideLoading('登录失败', 5000, 5, 0);

        }

    });

}

function addDb(db, uid) {
    
    debug('正式添加db了哦');

    uexDataBaseMgr.copyDataBaseFile("wgts://" + uid + ".db", function(error) {

        if (!error) {

            saveMenu(db, uid);

        } else {

            uexDataBaseMgr.close(db);
            debug("addDb出错");
            hideLoading('登录失败', 5000, 5, 0);

        }

    });

}

function saveMenu(db, uid) {

    var sqls = [];
    sqls.push("insert into tbl_user_db (uid) values ('" + uid + "')");
    debug(sqls);
    uexDataBaseMgr.transactionEx(db, JSON.stringify(sqls), function(error) {

        if (error == 0) {

            uexDataBaseMgr.close(db);

            getUserInfo(uid, 'login');

        } else {

            uexDataBaseMgr.close(db);
            debug("saveMenu出错");
            hideLoading('登录失败', 5000, 5, 0);

        }

    });

}
