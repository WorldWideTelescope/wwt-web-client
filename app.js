

var wwt = {
	app: angular.module('wwtApp', [
		'mgcrea.ngStrap',
		//'ngTouch',
		'ngAnimate',
		'ngRoute',
		'wwtControllers',
		'ngCookies',
		'angular-intro'
	]),
	controllers: angular.module('wwtControllers', []),
	triggerResize: function () { },
	resize: function () {
		$('body.mobile #WWTCanvas')
			.height($('#WorldWideTelescopeControlHost').height())
			.width($('#WorldWideTelescopeControlHost').width());
		$('body.desktop #WWTCanvas')
			.height($(window).height())
			.width($(window).width());
		//if ($('body.desktop.length')) {
			
		//	/*$('body.desktop div.context-panel .controls, body.desktop div.context-panel .thumbnails')
		//		.width($('div.context-panel').width() - ($('body.desktop .fov-panel').width() + 1));*/
		//	//$('body.desktop .layer-manager .tree').css('height', $(window).height() - (166 + $('body.desktop .context-panel').height()));
			
		//}
	}
};



$(window).on('load', function() {
	wwt.resize();
	//load search data after everything else
	var scr = document.createElement('script');
	scr.setAttribute("src", 'searchdata.min.js');
	document.getElementsByTagName("head")[0].appendChild(scr);
});
