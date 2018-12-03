//设置红包主的信息
function setRedUser(json) {
    
    var fileUrl = localStorage.getItem("fileUrl");
    var HeadImg = '<div class="ub-img1 knock-img uc-a2" style="background-image:url(' + fileUrl + json.HeadImg + ');"></div>';
    $('#HeadImg').children().remove();
    $('#HeadImg').append(HeadImg);
    $('TrueName').text(json.name);
    $('.fu-money').text(json.price);

}

