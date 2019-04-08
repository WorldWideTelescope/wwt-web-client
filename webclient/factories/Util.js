wwt.app.factory('Util', ['$rootScope', function ($rootScope) {
	var api = {
		getClassificationText: getClassificationText,
		getAstroDetails: getAstroDetails,
		formatDecimalHours: formatDecimalHours,
		formatHms: formatHms,
		drawCircleOverPlace: drawCircleOverPlace,
    removeHoverCircle: removeHoverCircle,
		getIsPlanet: getIsPlanet,
		secondsToTime: secondsToTime,
		getQSParam: getQSParam,
		getImageset: getImageset,
		getCreditsText: getCreditsText,
		getCreditsUrl: getCreditsUrl,
		isAccelDevice: isAccelDevice,
		isMobile:  $('body').hasClass('mobile'),
		isStaging: function() {
			return location.href.indexOf('worldwidetelescope') === -1;
		},
        isDebug:getQSParam('debug')!=null,
		nav: nav,
		log: log,
		resetCamera: resetCamera,
		goFullscreen: goFullscreen,
		exitFullscreen: exitFullscreen,
		toggleFullScreen: toggleFullScreen,
		getImageSetType: getImageSetType,
		trackViewportChanges: trackViewportChanges,
    parseHms: parseHms,
    mobileLink:mobileLink,
    resVersion:getQSParam('debug') != null ? $('body').data('resVersion') : Math.floor(Math.random()*99999),
    argb2Hex:argb2Hex,
    hex2argb:hex2argb,
    firstCharLower:firstCharLower,
    firstCharUpper:firstCharUpper
};
	var fullscreen = false;
	function getClassificationText(clsid) {
		if (clsid && !isNaN(parseInt(clsid))) {
			var str;
			$.each(wwtlib.Classification, function (k, v) {
				if (v === clsid) {
					str = k;
				}
			});
			var out = str.replace(/^\s*/, ""); // strip leading spaces
			out = out.replace(/^[a-z]|[^\s][A-Z]/g, function (str, offset) {
				if (offset == 0) {
					return (str.toUpperCase());
				} else {
					return (str.substr(0, 1) + " " + str.substr(1).toUpperCase());
				}
			});
			return (out);
		} else return null;
	};

	function formatDecimalHours(dayFraction, spaced) {
		var ts = new Date(new Date().toUTCString()).valueOf() - new Date().valueOf();
		var hr = ts / (1000 * 60 * 60);
		var day = (dayFraction - hr) + 0.0083333334;
		while (day > 24){
			day -= 24;
		}
		while (day < 0){
			day += 24;
		}
		var hours = day.toFixed(0);
		var minutes = ((day * 60) - (hours * 60)).toFixed(0);

		var join = spaced ? ' : ' : ':';
		//var seconds = ((day * 3600) - (((hours * 3600) + ((double)minutes * 60.0)));

		return ([int2(hours), int2(minutes)]).join(join);
	}
	function int2(dec) {
	  var sign = dec < 0 ? '-' : '';
	  var int = Math.floor(Math.abs(dec));
	  var pad = int < 10 ? '0' :'';
		return sign + pad + int;
	}

	function truncate(n) {
	    return (n >= 0) ? Math.floor(n) : Math.ceil(n);
	}

	function formatHms(angle, isHmsFormat, signed, spaced) {
	    var minutes = (angle - truncate(angle)) * 60;
	    var seconds = (minutes - truncate(minutes)) * 60;
	    minutes = Math.abs(minutes);
	    seconds = Math.abs(seconds);

		var join = spaced ? ' : ' : ':';
		if (isNaN(angle)) {
			angle = minutes = seconds = 0;
		}
		return isHmsFormat ? int2(angle) + 'h'
			+ int2(minutes) + 'm'
			+ int2(seconds) + 's' :
			([signed && angle > 0 ? '+' + int2(angle) : int2(angle), int2(minutes), int2(seconds)]).join(join);
	};

	function parseHms(input) {
		var parts;
		function convertHmstoDec(hours, minutes, seconds) {

			var c = Math.abs(parseInt(hours)) + Math.abs(parseInt(minutes)) / 60 + Math.abs(parseFloat(seconds)) / (60 * 60);
			if (hours.charAt(0) === '-'){
        c = 0 - c;
      }
			return c;
		}
		if (input.indexOf(':') != -1) {
			parts = input.split(':');
		}
		else if (input.indexOf('h') != -1) {
			parts = input.replace(/h/, ',').replace(/m/, ',').replace(/s/, '').split(',');
		}else if ($.trim(input).split(' ').length===3){
      parts = input.split(' ');
    }
		if (parts) {

		  return convertHmstoDec(parts[0], parts[1], parts[2]);
		} else {
		  return parseFloat(input);
		}
	}

	function getAstroDetails(place) {
		var coords = wwtlib.Coordinates.fromRaDec(place.get_RA(), place.get_dec());
		var stc = wwtlib.SpaceTimeController;
		var altAz = wwtlib.Coordinates.equitorialToHorizon(coords, stc.get_location(), stc.get_now());
		place.altAz = altAz;
		var classificationText = getClassificationText(place.get_classification());
		var riseSet;
		if (classificationText == 'Solar System') {

			var jNow = stc.get_jNow() + .5;
			var p1 = wwtlib.Planets.getPlanetLocation(place.get_name(), jNow - 1);
			var p2 = wwtlib.Planets.getPlanetLocation(place.get_name(), jNow);
			var p3 = wwtlib.Planets.getPlanetLocation(place.get_name(), jNow + 1);

			var type = 0;
			switch (place.get_name()) {
				case "Sun":
					type = 1;
					break;
				case "Moon":
					type = 2;
					break;
				default:
					type = 0;
					break;
			}

			riseSet = wwtlib.AstroCalc.getRiseTrinsitSet(jNow, stc.get_location().get_lat(), -stc.get_location().get_lng(), p1.RA, p1.dec, p2.RA, p2.dec, p3.RA, p3.dec, type);
		}
		else {
			riseSet = wwtlib.AstroCalc.getRiseTrinsitSet(stc.get_jNow() + .5, stc.get_location().get_lat(), -stc.get_location().get_lng(), place.get_RA(), place.get_dec(), place.get_RA(), place.get_dec(), place.get_RA(), place.get_dec(), 0);
		}
		if (!riseSet.bValid && !riseSet.bNeverRises) {
			riseSet.bNeverSets = true;
		}
		place.riseSet = riseSet;
	}
  var circ = null;
	function drawCircleOverPlace(place) {
    removeHoverCircle();
		if ($('#lstLookAt option:selected').prop('index') === 2) {
		  var circle = wwt.wc.createCircle();
			circle.set_id('focused');
			circle.setCenter(place.get_RA() * 15, place.get_dec());//setCenter(place.get_location3d());
			circle.set_skyRelative(false);
			circle.set_radius(.22);
			circle.set_lineWidth(3);
			wwt.wc.addAnnotation(circle);
			circ = circle;
			//console.log(circle);
		}
	}

	function removeHoverCircle(){
	  if (circ){
      wwt.wc.removeAnnotation(circ);
    }
  }

	function getIsPlanet(place) {
		var cls,isPlanet;
		if (typeof place.get_classification === 'function') {
			cls = place.get_classification();
			isPlanet = getClassificationText(cls) === 'Solar System';
		}
		return isPlanet || typeof place.get_rotation ==='function';
	}

	function secondsToTime(secs) {
		var hours = Math.floor(secs / (60 * 60));

		var divisorForMinutes = secs % (60 * 60);
		var minutes = Math.floor(divisorForMinutes / 60);

		var divisorForSeconds = divisorForMinutes % 60;
		var seconds = Math.ceil(divisorForSeconds);

		var obj = {
			"h": hours < 10 ? '0' + hours : hours.toString(),
			"m": minutes < 10 ? '0' + minutes : minutes.toString(),
			"s": seconds < 10 ? '0' + seconds : seconds.toString()
		};
		return obj;
	}

	function getQSParam(name) {
		name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
		var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
			results = regex.exec(location.search);
		return results == null ? null : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

  function mobileLink() {
    var delim='?'
    if (location.search.split('=').length > 1) {
      delim = '&';
    }
    var url = location.href;

    var bit = api.isMobile ? 0 : 1;
    if (getQSParam('mobile')) {
      return url.replace('mobile=' + getQSParam('mobile'), 'mobile=' + bit);
    }
    return location.href + delim + 'mobile=' + bit;
  }

	function getImageset(place) {
		if (!place) {
			return null;
		} else if (ss.canCast(place, wwtlib.Imageset)) {
			return place;
		} else if (place.get_backgroundImageset && ss.canCast(place.get_backgroundImageset(), wwtlib.Imageset)) {
			return place.get_backgroundImageset();
		} else if (place.get_studyImageset && ss.canCast(place.get_studyImageset(), wwtlib.Imageset)) {
			return place.get_studyImageset();
		} else {
			return null;
		}
	}

	function getCreditsText(pl) {
		var imageSet = getImageset(pl);
		if (imageSet) {
			return imageSet.get_creditsText();
		} else {
			return '';
		}
	}
	function getCreditsUrl(pl) {
		var imageSet = getImageset(pl);
		if (imageSet) {
			return imageSet.get_creditsUrl();
		} else {
			return '';
		}
	}
	
	var accelDevice = false; 
	
	function redirectClient(val) {
		return;
		var qs = location.search.substr(1);
		var newQs = '?';

		$.each(qs.split('&'), function (i, s) {
			if (i > 0) {
				newQs += '&';
			}
			if (s.indexOf('client') !== 0) {
				newQs += s;

			}
		});
		if (newQs.length > 1) {
			newQs += '&';
		}
		newQs += 'client=' + val;
		location.href = '/webclient' + newQs + location.hash;
		
	}

	function isAccelDevice() {
		return accelDevice;
	}

	function log() {
		if (getQSParam('debug') != null) {
			console.log(arguments);
		}
	}

    var isStandalone = function() {
        return $('body').data('standalone-mode') == true;
    }

    function nav(url) {
        if (isStandalone() && url.indexOf('http') !== 0) {
            url = '//worldwidetelescope.org' + url;
        }
		window.open(url);
	}
    function resetCamera(leaveHash) {
        if (!leaveHash) {
            location.hash = '/';
        }
        wwt.wc.gotoRaDecZoom(0, 0, 60, true);

    };
    function exitFullscreen() {
        if (fullscreen) {
            wwt.exitFullScreen();
            fullscreen = false;
        }
    }
    function goFullscreen() {
        if (!fullscreen) {
            wwt.requestFullScreen(document.body);
            fullscreen = true;
        }
    }

	function toggleFullScreen () {
		if (fullscreen) {
			wwt.exitFullScreen();
			fullscreen = false;
		} else {
			wwt.requestFullScreen(document.body);
			fullscreen = true;
		}
	};

	var imageSetTypes = [];
	function getImageSetType(sType) {
		if (!imageSetTypes.length) {
			$.each(wwtlib.ImageSetType, function(k, v) {
				if (!isNaN(v)) {
					imageSetTypes[v] = k.toLowerCase();
				}
			});
		}
		return imageSetTypes.indexOf(sType.toLowerCase()) == -1 ? 2 : imageSetTypes.indexOf(sType.toLowerCase());
		
	}

	var keyHandler = function (e) {
	    switch (e.keyCode) {
	        case 27:
	            $rootScope.$broadcast('escKey');
	            fullscreen = false;
	            break;
	    }
	};

	$(document).on('keyup', keyHandler);

	var dirtyInterval,
        viewport = {
		isDirty: false,
		RA: 0,
		Dec: 0,
		Fov: 60
	};
	function trackViewportChanges() {
		viewport = {
			isDirty: false,
			init: true,
			RA: wwt.wc.getRA(),
			Dec: wwt.wc.getDec(),
			Fov: wwt.wc.get_fov()
		};

		$rootScope.$broadcast('viewportchange', viewport);

		$rootScope.languagePromise.then(function() {
			viewport = {
				isDirty: false,
				init: true,
				RA: wwt.wc.getRA(),
				Dec: wwt.wc.getDec(),
				Fov: wwt.wc.get_fov()
			};

			$rootScope.$broadcast('viewportchange', viewport);
			viewport.init = false;


			dirtyInterval = setInterval(dirtyViewport, 250);
		});
	}

  function argb2Hex(argb){
	  var convChannel = function(cbyte){
	    var h = cbyte.toString(16);
	    return h.length == 2 ? h : '0'+h;
    };
    return '#'+
      convChannel(argb.r) +
      convChannel(argb.g) +
      convChannel(argb.b)

  }
  function hex2argb(hex,argb){
    var rgb = hex.match(/[A-Za-z0-9]{2}/g).map(function (v) {
      return parseInt(v, 16)
    });
    argb.r = rgb[0];
    argb.g = rgb[1];
    argb.b = rgb[2];
    return argb;
  }
  function firstCharLower(s){
    return s.charAt(0).toLowerCase()+s.substr(1)
  }
  function firstCharUpper(s){
    return s.charAt(0).toUpperCase()+s.substr(1)
  }
	var dirtyViewport = function () {
		var wasDirty = viewport.isDirty;
		viewport.isDirty = wwt.wc.getRA() !== viewport.RA || wwt.wc.getDec() !== viewport.Dec || wwt.wc.get_fov() !== viewport.Fov;
		viewport.RA = wwt.wc.getRA();
		viewport.Dec = wwt.wc.getDec();
		viewport.Fov = wwt.wc.get_fov();
		if (viewport.isDirty || wasDirty) {
			$rootScope.viewport = viewport;
			$rootScope.$broadcast('viewportchange', viewport);
		}
	}
	var browsers = {};
	var has = function (src, search) {
	    return src.indexOf(search) >= 0;
	}
	var ua = navigator.userAgent.toLowerCase();

	browsers.isEdge = has(ua, 'edge/') > 0;
	browsers.isFF = has(ua, 'firefox') > 0;
	browsers.isIE = has(ua, 'msie') || has(ua, 'trident');
	browsers.isChrome = has(ua, 'chrome');
	browsers.isSafari = has(ua, 'safari') && !browsers.isChrome && !browsers.isIE && !browsers.isEdge && !browsers.isFF;;
	browsers.isChrome = has(ua, 'chrome') > 0 && !browsers.isIE && !browsers.isEdge && !browsers.isFF;
	browsers.isWindows = has(ua, 'windows');

	//console.log(browsers); 
	
	return $.extend(api, browsers);

}]);

