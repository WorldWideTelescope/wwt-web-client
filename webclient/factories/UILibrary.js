wwt.app.factory('UILibrary', ['$rootScope','AppState','Util', 'Localization','$modal', function ($rootScope, appState, util, loc,$modal) {

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
	$rootScope.bottomControlsWidth = function() {
		return (angular.element('div.context-panel').width() - angular.element('body.desktop .fov-panel').width()) + 1;
	}
	$rootScope.layerManagerHeight = function() {
		return $(window).height() - (168 + $('body.desktop .context-panel').height());
	};

	$rootScope.copyLink = function (event) {
	    var src = $(event.currentTarget);
	    var input = src.prev();
	    input[0].select();
	    document.execCommand('copy');
	    var flyout = $('<div class=clipboard-status>Copied successfully</div>');
	    input.parent().css('position', 'relative').append(flyout);
	    //flyout.fadeIn(200).show();
	    setTimeout(function () { flyout.fadeOut(1111); }, 3333);
	}

	$rootScope.loadVOTableModal = wwt.loadVOTableModal = function(votable){
	  var modalScope = $rootScope.$new();
	  modalScope.votable = votable;
	  $modal({
      scope: modalScope,
      templateUrl: 'views/modals/centered-modal-template.html',
      contentTemplate: 'views/modals/vo-table-viewer.html',
      show: true,
      placement: 'center'
    });
  }

	return true;
}]);

