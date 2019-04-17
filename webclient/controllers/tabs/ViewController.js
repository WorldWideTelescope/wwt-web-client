wwt.controllers.controller('ViewController',
	['$scope',
	'AppState',
	'$timeout','Util','$rootScope','UILibrary',
	function ($scope, appState, $timeout, util,$rootScope,UILibrary) {
		var stc = $scope.spaceTimeController = wwtlib.SpaceTimeController;
		$scope.galaxyModeChange = function () {
			if ($scope.galaxyMode && $scope.viewFromLocation) {
				$scope.viewFromLocation = false;
				$scope.setViewFromLocation();
			}
			wwt.wc.settings.set_galacticMode($scope.galaxyMode);
		};

		//$scope.locationName = $rootScope.observingLocation ? $rootScope. : $scope.getFromEn('My Location');
		$scope.now = new Date();
		$scope.loc = {
			view:'View',
			realTime: 'Real Time',
			reverseTime: 'Reverse Time',
			paused:'Paused'
		};
		$rootScope.languagePromise.then(function() {
			$scope.loc.view = $scope.getFromEn('View');
			$scope.loc.realTime = $scope.getFromEn('Real Time');
			$scope.loc.reverseTime = $scope.getFromEn('Reverse Time');
			$scope.loc.paused = $scope.getFromEn('Paused');
		});

		function timeDateTimerTick() {
			if ($scope.activePanel === $scope.loc.view || util.isMobile) {
				$timeout(function() {
					//var offset = stc.$1 === undefined ? stc._offset : stc.$1;
					//var now = $scope.now = new Date(new Date().valueOf() + offset);
					var now = $scope.now = stc.get_now();
					$scope.year = now.getFullYear();
					$scope.month = (now.getMonth() + 1) % 12;
					$scope.date = now.getDate();
					$scope.hours = now.getHours();
					$scope.minutes = now.getMinutes();
					$scope.seconds = now.getSeconds();
				});
			}
		};



		$scope.fastBack_Click = function() {
			var tr = stc.get_timeRate();
			if (tr < -2 && tr >= -1000000000) {
				stc.set_timeRate(tr * 10);
			} else {
				stc.set_timeRate(-10);
			}
			stc.set_syncToClock(true);
			updateSpeed();
		};

		$scope.back_Click = function() {
			var tr = stc.get_timeRate();
			if (tr <= -10) {
				stc.set_timeRate(tr / 10);
				stc.set_syncToClock(true);
			} else {
				stc.set_timeRate(-2);
				stc.set_syncToClock(true);
			}
			if (stc.get_timeRate() === -1) {
				stc.set_timeRate(-2);
			}
			updateSpeed();
		};

		$scope.pause_Click = function() {
			stc.set_syncToClock(!stc.set_syncToClock);
			updateSpeed();
		};

		$scope.play_Click = function () {
			var tr = stc.get_timeRate();
			if (tr >= 10) {
				tr /= 10;
			} else {
				tr = 1;
			}
			stc.set_timeRate(tr);
			stc.set_syncToClock(true);
			updateSpeed();

		};

		$scope.fastForward_Click = function() {
			var tr = stc.get_timeRate();
			if (tr > 0 && tr <= 1000000000) {
				stc.set_timeRate(tr * 10);
			} else {
				stc.set_timeRate(10);
			}
			stc.set_syncToClock(true);
			updateSpeed();
		};

		function updateSpeed() {
			var tr = stc.get_timeRate();
			if (tr == -2)tr = -1;
			if (tr == 1){
				$scope.TimeMode = $scope.loc.realTime;
			} else if (stc.TimeRate == -2.0){
				$scope.TimeMode = $scope.loc.reverseime;
			} else {
				$scope.TimeMode = "X " + tr;
			} if (!stc.get_syncToClock()) {
				$scope.TimeMode = $scope.TimeMode + " : " + $scope.loc.paused;
			}   
		}
    $rootScope.updateDateUI = function(){
		  updateSpeed();
		  timeDateTimerTick();
    }

		$scope.timeNow_Click = function() {
			stc.set_syncToClock(true);
			stc.syncTime();
			stc.set_timeRate(1);
			updateSpeed();
		};

		$scope.setViewFromLocation = function() {
			if ($scope.galaxyMode && $scope.viewFromLocation) {
				$scope.galaxyMode = false;
				$scope.galaxyModeChange();
			}
			$scope.$applyAsync(function(){
        $rootScope.observingLocation.localHorizonMode = $scope.viewFromLocation;
        console.log({observingLocation:$rootScope.observingLocation});
        appState.set('observingLocation',$rootScope.observingLocation);
        console.log(appState.get('observingLocation'));
        $rootScope.ctl.settings.set_localHorizonMode($scope.viewFromLocation);
      });
		};

		$scope.showObservingLocationOptions = function(){
		  UILibrary.showModal({
        scope:$scope,
        controller:'ObservingLocationController',
        template:'observing-loc-options',
        cssClass:'set-position observing-loc',
        fixedPosition:true,
        callback:console.log
      });
    };
		setInterval(timeDateTimerTick, 300);
		updateSpeed();
    $scope.viewFromLocation = $rootScope.ctl.settings._localHorizonMode;
    setTimeout(function(){
      if (!$rootScope.observingLocation || $rootScope.observingLocation.lat===undefined) {
        wwtlib.WWTControl.useUserLocation();
      }
    }, 100);

	}
]);
