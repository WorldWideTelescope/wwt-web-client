wwt.controllers.controller('ShareController',
[
	'$scope',
	'$rootScope', 
	'Util',
	'$timeout',
	'HashManager',
	function($scope, $rootScope, util, $timeout, hashManager) {

		$scope.includeViewport = true;

		$scope.init = function() {
			$scope.shareUrlReadOnly = $scope.shareUrl;
			if ($scope.lookAt !== 'Sky') {
				$scope.shareUrlReadOnly = hashManager.setHashVal('lookAt', $scope.lookAt, true);
			} else if ($scope.lookAt === 'Earth') {
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

		$rootScope.$on('viewportchange', $scope.init);

		$scope.includeViewportChange = function() {
			if ($scope.includeViewport) {
				hashManager.setHashVal('ra', $rootScope.viewport.RA.toFixed(5), true);
				hashManager.setHashVal('dec', $rootScope.viewport.Dec.toFixed(5), true);
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.setHashVal('fov', $rootScope.viewport.Fov.toFixed(5), true);
			} else {
				hashManager.removeHashVal('ra', true);
				hashManager.removeHashVal('dec', true);
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.removeHashVal('fov', true);
			}
			$('meta[property="og:url"]').attr('content', $scope.shareUrlReadOnly);
			$('meta[property="og:title"]').attr('content', $scope.trackingObj ? $scope.trackingObj.get_name() + ' - WorldWide Telescope' : $scope.getFromEn('WorldWide Telescope Web Client'));
          $('meta[property="og:image"]').attr('content', $scope.trackingObj ? $scope.trackingObj.get_thumbnailUrl() : 'https://wwtweb.blob.core.windows.net/webclient/wwtlogo.png');
			selectUrl(222);
		};

		var selectUrl = function(delay) {
            setTimeout(function () {
                if ($('#shareUrl').length) {
                    $('#shareUrl')[0].focus();
                    $('#shareUrl')[0].select();
                }
			}, delay);
		}

		//$scope.shareFB = function() {
		//	FB.ui({
		//		method: 'share_open_graph',
		//		action_type: 'og.likes',
		//		action_properties: JSON.stringify({
		//			object: $scope.shareUrlReadOnly,
		//			scrape: true
		//		})
		//	},
		//		function(response){
		//			util.log(arguments);
		//		}
		//	);
		//};

		$scope.hide = function() {
			$scope.$parent.$hide();
		};
	}
]);
