wwt.controllers.controller(
  'MobileNavController',
  [
    '$rootScope',
    '$scope',
    'Util',
    '$modal',
    'Localization',

    function ($rootScope, $scope, util, $modal, loc) {
      var v = '?v=' + wwt.gitShortSha;

      $scope.showModal = function (modalButton) {
	$scope.hideMenu();

	if (typeof modalButton.modal == 'object') {
	  modalButton.modal.$promise.then(modalButton.modal.show);
	} else {
	  $(modalButton.modal).modal('show');
	}
      };

      $rootScope.languagePromise.then(function() {
	$scope.modalButtons = [
	  {
	    text: loc.getFromEn('Explore'),
	    icon: 'fa-binoculars',
	    modal: '#explorerModal'
	  }, {
	    text: loc.getFromEn('Tours'),
	    icon: 'fa-play',
	    modal: $modal({
	      contentTemplate: wwt.staticAssetsPrefix + 'views/modals/mobile-tours.html' + v,
	      show: false,
	      scope: $scope
	    })
	  }, {
	    text: loc.getFromEn('Search'),
	    icon: 'fa-search',
	    modal: $modal({
              contentTemplate: wwt.staticAssetsPrefix + 'views/modals/mobile-search.html' + v,
	      show: false,
	      scope: $scope,
	      prefixEvent: 'searchModal'
	    })
	  }, {
	    text: loc.getFromEn('View'),
	    icon: 'fa-eye',
	    modal: $modal({
              contentTemplate: wwt.staticAssetsPrefix + 'views/modals/mobile-view.html' + v,
	      show: false,
	      scope: $scope
	    })
	  }, {
	    text: loc.getFromEn('Settings'),
	    icon: 'fa-gears',
	    modal: $modal({
              contentTemplate: wwt.staticAssetsPrefix + 'views/modals/mobile-settings.html' + v,
	      show: false,
	      scope: $scope
	    })
	  }, {
	    text: loc.getFromEn('Layers'),
	    icon: 'fa-align-left',
	    modal: $modal({
              contentTemplate: wwt.staticAssetsPrefix + 'views/modals/mobile-layer-manager.html' + v,
	      show: false,
	      scope: $scope
	    })
	  }
	];
      });

      $scope.gotoPage = function(url) {
	$scope.hideMenu();
	open(url);
      }

      $scope.resetCamera = function () {
	$scope.trackingObj = null;
	$scope.hideMenu();
	util.resetCamera();
      }

      $scope.tabClick = function (tab) {
	$scope.hideMenu();
	tab.action();
      }

      $scope.hideMenu = function(selector) {
        if (typeof selector !== 'string') {
          selector = '.navbar-collapse';
        }

	$(selector + '.in').removeClass('in').addClass('collapse');
      }

      $scope.$on('searchModal.show.before', function () {
	$rootScope.searchModal = true;
      });

      $scope.$on('searchModal.hide', function () {
	$rootScope.searchModal = false;
      });
    }
  ]
);
