wwt.controllers.controller('ShareController',
[
	'$scope',
	'$rootScope',
	'Util',
	'$timeout',
	'HashManager',
	function ($scope, $rootScope, util, $timeout, hashManager) {

		$scope.includeViewport = true;

		$scope.init = function () {
			$scope.shareUrlReadOnly = $scope.shareUrl;
			if ($scope.lookAt != 'Sky'){
				$scope.shareUrlReadOnly = hashManager.setHashVal('lookAt', $scope.lookAt, true);
			}
			else if ($scope.lookAt === 'Earth') {
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.removeHashVal('place', true);
			}
			if ($scope.backgroundImagery && $scope.backgroundImagery.get_name() != 'Digitized Sky Survey (Color)') {
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.setHashVal('imagery', $scope.backgroundImagery.get_name(), true);
			}
			if ($('.cross-fader a.btn').css('left') != '100px') {
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.setHashVal('cf', $('.cross-fader a.btn').css('left').replace(/px/, ''), true);
			}

			$('#shareUrl').on('focus', function() {
				$(this).select();
			});
			$scope.includeViewportChange();
			selectUrl(999);
		};

		$scope.includeViewportChange = function() {
			if ($scope.includeViewport) {
				hashManager.setHashVal('ra', $rootScope.ctl.getRA().toFixed(5), true);
				hashManager.setHashVal('dec', $rootScope.ctl.getDec().toFixed(5), true);
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.setHashVal('fov', $rootScope.ctl.get_fov().toFixed(5), true);
			} else {
				hashManager.removeHashVal('ra', true);
				hashManager.removeHashVal('dec', true);
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.removeHashVal('fov', true);
			}
			selectUrl(222);
		};

		var selectUrl = function (delay) {
			setTimeout(function () {
				$('#shareUrl')[0].focus();
				$('#shareUrl')[0].select();
			}, delay);
		}

		$scope.hide = function() {
			$scope.$parent.$hide();
		};
	}
]);