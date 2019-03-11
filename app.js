/** 
* Copyright 2014, 2015  Microsoft Research
*
* Permission is hereby granted, free of charge, to any person
* obtaining a copy of this software and associated documentation
* files (the "Software"), to deal in the Software without
* restriction, including without limitation the rights to use, copy,
* modify, merge, publish, distribute, sublicense, and/or sell copies
* of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be
* included in all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
* NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
* BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
* ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
* CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
**/
var wwt = { 
	app: angular.module('wwtApp', [
		'mgcrea.ngStrap',
		'ngTouch',  
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
      wwt.smallVP = ($(window).height() < 600 || $(window).width() < 700);
      wwt.definitelyMobile = wwt.smallVP && ($(window).height() < 900 && $(window).width() < 600);
  },
  smallVP:false
};

$(window).on('load', function() {
	wwt.resize();
	//load search data after everything else
	var scr = document.createElement('script');
	scr.setAttribute("src", 'searchdata.min.js');
	document.getElementsByTagName("head")[0].appendChild(scr);
});
