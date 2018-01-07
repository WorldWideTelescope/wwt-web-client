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
	  modalScope.customClass = 'vo-tbl-modal';
    modalScope.voTableLayer = votable.get_table ? votable : wwtlib.VoTableLayer.create(votable);
    modalScope.votable = modalScope.voTableLayer.get_table();

	  $modal({
      scope: modalScope,
      templateUrl: 'views/modals/centered-modal-template.html',
      contentTemplate: 'views/modals/vo-table-viewer.html',
      show: true,
      placement: 'center',
      backdrop: false
    });
  };

	var showColorpicker = function(colorpicker,e){
    var modalScope = $rootScope.$new();
    modalScope.colorpicker = colorpicker;
    modalScope.mouse = e;
    modalScope.customClass = 'colorpicker-modal';
    $modal({
      scope: modalScope,
      templateUrl: 'views/modals/centered-modal-template.html',
      contentTemplate: 'views/modals/colorpicker.html',
      show: true,
      placement: 'center',
      backdrop: false,
      controller:'colorpickerController'
    });
  }

  var loadingModal;
	$rootScope.loading = function(flag,content){
	  if (loadingModal){
	    loadingModal.hide();
	    loadingModal = null;
    }if (flag){
      loadingModal = $modal({
        templateUrl: 'views/modals/loading-content.html',
        show: true,
        content:content || 'Content Loading. Please Wait...',
        placement: 'center'
      });
    }
  }
  if (util.getQSParam('debug') === 'disable') {
    setTimeout(function () {
      wwtlib.WWTControl.singleton.render = function () {
        console.log('fixed render loop :)');
      }
      //testing only:
      $('#WorldWideTelescopeControlHost').html('');
    }, 888)

  }

	return {
	  addDialogHooks:function(){
      wwt.wc.add_voTableDisplay(wwt.loadVOTableModal);
      wwt.wc.add_colorPickerDisplay(showColorpicker)
    }
  };
}]);

