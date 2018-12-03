/*
 * hdr.1.设置地图的头部用户信息
 */
function setMapHeader() {
    
    var fileUrl = localStorage.getItem("fileUrl");
    
    var HeadImg = '';
    var Name = '';
    var TrueName = '';
    
    var district = '';
    
    var addr = JSON.parse(localStorage.getItem('addrJson'));
    var UserInfo = JSON.parse(localStorage.getItem("UserInfo"));
    
    if (UserInfo.Info.TrueName == '' || UserInfo.Info.TrueName == null || UserInfo.Info.TrueName == undefined) {
        TrueName = UserInfo.Info.Phone;
    } else {
        TrueName = UserInfo.Info.TrueName;
    }
    
    district = addr.district;
    
    HeadImg = '<div ontouchstart="appcan.touch(&#39;btn-act&#39;)" class="ub-img icon-style float-right" style="background-image:url(' + fileUrl + UserInfo.Info.HeadImg + ');"></div>';
    
    $('#HeadImg').children().remove();
    $('#HeadImg').append(HeadImg);
    
    $('#TrueName').text(TrueName);
    
    $('#district').text(district);

}
