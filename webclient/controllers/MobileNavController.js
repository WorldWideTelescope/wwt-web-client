wwt.controllers.controller('MobileNavController',
['$rootScope',
	'$scope',
	'Util',
	'$modal',
	'Localization',
  function ($rootScope, $scope, util, $modal, loc) {
    var v = '?v='+$(document.body).data('resourcesVersion');
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
				},
				{
					text: loc.getFromEn('Tours'),
					icon: 'fa-play',
					modal: $modal({
						contentTemplate: 'views/modals/mobile-tours.html' + v,
						show: false,
						scope: $scope
					})
				},
				{
					text: loc.getFromEn('Search'),
					icon: 'fa-search',
					modal: $modal({
                      contentTemplate: 'views/modals/mobile-search.html' + v,
						show: false,
						scope: $scope,
						prefixEvent: 'searchModal'
					})
				},
				{
					text: loc.getFromEn('View'),
					icon: 'fa-eye',
					modal: $modal({
                      contentTemplate: 'views/modals/mobile-view.html' + v,
						show: false,
						scope: $scope
					})
				},
				{
					text: loc.getFromEn('Settings'),
					icon: 'fa-gears',
					modal: $modal({
                      contentTemplate: 'views/modals/mobile-settings.html' + v,
						show: false,
						scope: $scope
					})
				},
				{
					text: loc.getFromEn('Layers'),
					icon: 'fa-align-left',
					modal: $modal({
                      contentTemplate: 'views/modals/mobile-layer-manager.html' + v,
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

		$scope.hideMenu = function() {
			$('.navbar-collapse.in').removeClass('in').addClass('collapse');
		}
		$scope.menuAction = function (action) {
			util.log(action);
		}
		
		$scope.$on('searchModal.show.before', function () {
			$rootScope.searchModal = true;
		});
		$scope.$on('searchModal.hide', function () {
			$rootScope.searchModal = false;
		});
	}
]);
