wwt.controllers.controller('MobileNavController',
['$rootScope',
	'$scope',
	'Util',
	'$modal',
	function ($rootScope, $scope, util, $modal) {

		

		$scope.showModal = function (modalButton) {
			$scope.hideMenu();
			if (typeof modalButton.modal == 'object') {
				modalButton.modal.$promise.then(modalButton.modal.show);
			} else {
				$(modalButton.modal).modal('show');
			}
		}

		$scope.modalButtons = [
			{
				text: 'Explore',
				icon: 'fa-binoculars',
				modal: '#explorerModal'
			},
			{
				text: 'Tours',
				icon: 'fa-play',
				modal: $modal({
					contentTemplate: 'views/modals/mobile-tours.html',
					show: false,
					scope: $scope
				})
			},
			{
				text: 'Search',
				icon: 'fa-search',
				modal: $modal({
					contentTemplate: 'views/modals/mobile-search.html',
					show: false,
					scope: $scope,
					prefixEvent:'searchModal'
				})
			},
			{
				text: 'View',
				icon: 'fa-eye',
				modal: $modal({
					contentTemplate: 'views/modals/mobile-view.html',
					show: false,
					scope: $scope
				})
			},
			{
				text: 'Settings',
				icon: 'fa-gears',
				modal: $modal({
					contentTemplate: 'views/modals/mobile-settings.html',
					show: false,
					scope: $scope
				})
			},
			{
				text: 'Layers',
				icon: 'fa-align-left',
				modal: $modal({
					contentTemplate: 'views/modals/mobile-layer-manager.html',
					show: false,
					scope: $scope
				})
			}

		];

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