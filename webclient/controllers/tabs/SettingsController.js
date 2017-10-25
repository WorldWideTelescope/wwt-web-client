wwt.controllers.controller('SettingsController',
	['$scope',
	'$rootScope',
	'AppState',
	'$timeout',
	'$cookies',
	'Util',
	'$q',
	function ($scope, $rootScope, appState, $timeout, $cookies, util, $q) {
		//var settings = $scope.settings = wwt.wc.settings;
		$scope.defaults = {
			autoHideTabs: false,
			autoHideContext: false,
			smoothPanning: !util.isAccelDevice(),
			version: '5.1.02',
			crosshairs: true
		};

		$q.all([$scope.getFromEn('HTML5'), $scope.getFromEn('Silverlight')]).then(function(arrayLabels) {
			$scope.availableClients = [
				{
					code: 'HTML5',
					label: arrayLabels[0]
				}, {
					code: 'SL',
					label: arrayLabels[1]
				} /*,{
				code: 'WWT',
				label: $scope.getFromEn('WorldWide Telescope Windows Client')
			}*/
			];
		});

		var redirTimer;
		$scope.setClientPref = function() {
			util.log($scope.preferredClient);
			$cookies.preferredClient = $scope.preferredClient;
			if ($scope.preferredClient == 'SL') {
				$scope.redirecting = true;
				$scope.redirectingSeconds = 5;
				setInterval(function() {
					$timeout(function() {
						$scope.redirectingSeconds--;
					});
				}, 1000);
				redirTimer = setTimeout(function() {
					location.reload();
				},5000);
			}
		};
		$scope.cancelRedir = function() {
			clearTimeout(redirTimer);
			$scope.redirecting = false;
		};

		$timeout(function () {
			$scope.preferredClient = $cookies.preferredClient || $scope.availableClients[0].code;
			//util.log($scope.preferredClient, $scope.availableClients);
		}, 10);

		$scope.$on('restoreDefaults', function () {
			appState.set('settings', null);
			$scope.retrieveSettings();
		});

		$scope.retrieveSettings = function () {

      $scope.savedSettings = appState.get('settings');
			if (!$scope.savedSettings || !$scope.savedSettings.version || $scope.savedSettings.version != $scope.defaults.version) {
				$scope.savedSettings = $scope.defaults;
			}
			if ($scope.savedSettings['crosshairs'] === undefined) {
				$scope.savedSettings = $scope.defaults;
			}
			$scope.saveSettings(true);
		};

    $scope.WebGl = appState.get('WebGl') ? true : false;
		$scope.setWebGl = function() {
			appState.set('WebGl', $scope.WebGl);
			location.reload();
		}

		$scope.saveSettings = function (init) {
		    $timeout(function () {
		        var broadcast = false;
				$.each($scope.savedSettings, function (setting, flag) {
					if ($scope[setting] !== flag || init) {
						if (init) {
							$scope[setting] = flag;
						} else {
						    $scope.savedSettings[setting] = $scope[setting];
						}
						if (setting.indexOf('autoHide') === 0) {
						    broadcast = true;
						}

					}
				});
				wwt.wc.settings.set_showCrosshairs($scope.crosshairs);
				wwt.wc.settings.set_smoothPan($scope.smoothPanning);
				appState.set('settings', $scope.savedSettings);
				if (broadcast) {
				    $rootScope.$broadcast('autohideChange');
				}
			}, 10);
		};

		$scope.retrieveSettings();
	}
]);
