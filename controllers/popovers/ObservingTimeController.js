wwt.controllers.controller('ObservingTimeController',
	['$scope',
	'AppState',
	function ($scope, appState) {
		var now = $scope.now;
		$scope.dt = {};
		$scope.dt.year = now.getFullYear();
		$scope.dt.month = (now.getMonth() + 1) % 12;
		$scope.dt.date = now.getDate();
		$scope.dt.hours = now.getHours();
		$scope.dt.minutes = now.getMinutes();
		$scope.dt.seconds = now.getSeconds();

		$scope.setNow = function () {
			var date = new Date(parseInt($scope.dt.year),
				parseInt($scope.dt.month) - 1,
				parseInt($scope.dt.date),
				parseInt($scope.dt.hours),
				parseInt($scope.dt.minutes),
				parseInt($scope.dt.seconds));
			wwtlib.SpaceTimeController.set_now(date);
		};
	}
]);