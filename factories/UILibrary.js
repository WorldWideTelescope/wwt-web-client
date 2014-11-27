wwt.app.factory('UILibrary', ['$rootScope','AppState','Util', 'Localization', function ($rootScope, appState, util, loc) {

	$rootScope.layerManagerHidden = appState.get('layerManagerHidden') ? true : false;

	$rootScope.toggleLayerManager = function () {
		$rootScope.layerManagerHidden = !$rootScope.layerManagerHidden;
		appState.set('layerManagerHidden', $rootScope.layerManagerHidden);
	}

	$rootScope.getCreditsText = function (place) {
		return util.getCreditsText(place);
	}
	$rootScope.getCreditsUrl = function (place) {
		return util.getCreditsUrl(place);
	}

	$rootScope.getClassificationText = function (clsid) {
		var txt = util.getClassificationText(clsid);
		return txt || loc.getFromEn('Unknown');
	};

	$rootScope.secondsToTime = function (secs) {
		return util.secondsToTime(secs);
	}

	$rootScope.isMobile = util.isMobile;

	$rootScope.resLocation = $('body').data('res-location');
	
	return true;
}]);

