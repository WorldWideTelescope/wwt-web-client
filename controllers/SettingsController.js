wwt.controllers.controller('SettingsController',
	['$scope',
	'$rootScope',	
	'AppState',
	'$timeout',
	'$cookies',
	'Util',
	function ($scope, rs, appState, $timeout, $cookies, util) {
		//var settings = $scope.settings = wwt.wc.settings;
		$scope.defaults = {
			autoHideTabs: false,
			autoHideContext: false,
			smoothPanning: !util.isAccelDevice(),
			version: '5.1.02',
			crosshairs:true
		};

		//TODO: make event driven
		$timeout(function() {
			$scope.availableClients = [
				{
					code: 'HTML5',
					label: $scope.getFromEn('HTML5')
				}, {
					code: 'SL',
					label: $scope.getFromEn('Silverlight')
				} /*,{
				code: 'WWT',
				label: $scope.getFromEn('WorldWide Telescope Windows Client')
			}*/
			];
		}, 5);
		

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
			$timeout(function() {
				$.each($scope.savedSettings, function (setting, flag) {
					if ($scope[setting] !== flag || init) {
						if (init) {
							$scope[setting] = flag;
						} else {
							$scope.savedSettings[setting] = !flag;
						}
						if (setting === 'autoHideContext') {
							if (!$scope.savedSettings.autoHideContext) {
								context.fadeIn(800, function () { contextHidden = false; });
							}else if (init && $scope.autoHideContext) {
								setTimeout(function () {
									contextHidden = false;
									//context.fadeOut(800, function() { contextHidden = true; });
								},1200);
							}
						}
						if (setting === 'autoHideTabs') {
							if (!$scope.savedSettings.autoHideTabs) {
								tabs.fadeIn(800, function () { tabsHidden = false; });
							}else if (init && $scope.autoHideTabs) {
								setTimeout(function() {
									tabsHidden = false;
									//tabs.fadeOut(800, function() { tabsHidden = true; });
								},1200);
							}
						}
						
					}
				});
				wwt.wc.settings.set_showCrosshairs($scope.crosshairs);
				wwt.wc.settings.set_smoothPan($scope.smoothPanning);
				appState.set('settings', $scope.savedSettings);
			}, 10);
			
				
		};
		var mouseInTopRegion = false;
		var mouseInBottomRegion = false;
		var tabsHidden = false;
		var contextHidden = false;
		var tabs = $('#topPanel, .layer-manager'), context = $('.context-panel');
		var hideContextTimer,
			hideTabsTimer,
			showingTabs,showingContext;


		$(window).on('mousemove touchstart touchmove touchend', function (event) {
			if (wwt.tourPlaying) {
				showingTabs = false;
				showingContext = false;
				return;
			}
			if ($scope.savedSettings.autoHideContext || $scope.savedSettings.autoHideTabs) {
				if (!context.length) context = $('.context-panel');
				if (tabs.length < 2) tabs = $('#topPanel, .layer-manager');
				var y = event.pageY != undefined? event.pageY : event.originalEvent.targetTouches[0].pageY;
				
				mouseInBottomRegion = $(window).height() - 123 < y;
				mouseInTopRegion = y < 142;
				if ($scope.savedSettings.autoHideTabs) {
					if (mouseInTopRegion) {
						clearTimeout(hideTabsTimer);
						if (tabsHidden && showingTabs != true) {
							tabsHidden = false;
							showingTabs = true;
							tabs.fadeIn(800, function() { showingTabs = false; });
						}
					} else if (!tabsHidden) {
						clearTimeout(hideTabsTimer);
						hideTabsTimer = setTimeout(function () {
							tabsHidden = true;
							tabs.fadeOut(800);
						}, 1500);
					}
				}
				if ($scope.savedSettings.autoHideContext) {
					if (mouseInBottomRegion) {
						clearTimeout(hideContextTimer);
						if (contextHidden && showingContext != true) {
							contextHidden = false;
							showingContext = true;
							context.fadeIn(800,function() { showingContext = false; });
						}
					} else if (!contextHidden) {
						clearTimeout(hideContextTimer);
						hideContextTimer = setTimeout(function () {
							contextHidden = true;
							context.fadeOut(800);
						}, 1500);
					}
				}

			}

		});

		$scope.retrieveSettings();
	}
	]);