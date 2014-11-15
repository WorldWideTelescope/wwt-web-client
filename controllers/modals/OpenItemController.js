wwt.controllers.controller('OpenItemController',
	['$rootScope',
	'$scope',
	'AppState',
	'Places',
	'Util',
	function ($rootScope,$scope, appState, places, util) {
		$scope.openItem = function (itemType) {
			if (itemType === 'collection') {
				places.openCollection($scope.openItemUrl).then(function (folder) {
					util.log('initExplorer broadcast', folder);
					$rootScope.newFolder = folder;
					$rootScope.$broadcast('initExplorer', $scope.openItemUrl);
				});
			}else if (itemType === 'tour') {
				$scope.playTour($scope.openItemUrl);
			} else {
				places.importImage($scope.openItemUrl).then(function(folder) {
					util.log('initExplorer broadcast', folder);
					$rootScope.newFolder = folder;
					$rootScope.$broadcast('initExplorer', $scope.openItemUrl);
				});
			}
			util.log(itemType,$scope.openItemUrl);
			$('#openModal').modal('hide');
		}
	}
]);

