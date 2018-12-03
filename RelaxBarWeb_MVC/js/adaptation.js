$(document).ready(function(){
	var i = window.devicePixelRatio>1?1/window.devicePixelRatio:1;
	var meta =document.createElement("meta");
	meta.name="viewport";
	meta.content='width=device-width,user-scalable=no,initial-scale='+i+',minimum-scale='+i+',maximum-scale='+i;
	document.getElementsByTagName("head")[0].appendChild(meta);
	
	var html = document.getElementsByTagName("html")[0];
	var iW =document.body.offsetWidth;
	var scale=iW/750*20;
	html.style.fontSize=scale+"px";
});
