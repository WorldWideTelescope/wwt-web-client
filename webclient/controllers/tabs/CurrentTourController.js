wwt.controllers.controller('CurrentTourController', ['$scope', '$rootScope', function($scope,$rootScope) {
    var tourEdit = wwtlib.WWTControl.singleton.tourEdit;
    var tour;
    $scope.init = function (curTour) {
        $scope.tour = tour = curTour;
        tour.duration = 0;
        $scope.tourStops = tour.get_tourStops().map(function (s) {
            s.description = s.get_description();
            s.thumb = s.get_thumbnail();
            s.duration = s.get_duration();
            s.secDuration = Math.round(s.duration / 1000);
            if (s.secDuration < 10) {
                s.secDuration = '0' + s.secDuration;
            }
            s.secDuration = '0:' + s.secDuration;
            tour.duration += s.duration;
            return s;
        });
        tour.minuteDuration = Math.floor(tour.duration / 60000);
        tour.secDuration = Math.floor((tour.duration % 60000) / 1000);
    };

    $rootScope.$watch('currentTour', function() {
        console.log($rootScope.currentTour);
    });
    
    $scope.showContextMenu = function (index) {
        tour.set_currentTourstopIndex(index);
        tourEdit.tourStopList_MouseClick(this, e);
    };
    $scope.gotoStop = function (index, e) {
        tour.set_currentTourstopIndex(index);
    };

    $scope.showStartCameraPosition = function (index) {
        tour.set_currentTourstopIndex(index);
        tourEdit.tourStopList_ShowStartPosition();
    }
    $scope.showEndCameraPosition = function (index) {
        tour.set_currentTourstopIndex(index);
        tourEdit.tourStopList_ShowEndPosition();
    }

    $scope.pauseTour = function () {
        if (tourEdit.playing) {
            tourEdit.pauseTour();
        }
        else {
            tourEdit.playNow(true);
        }
        
        $rootScope.tourPaused = !wwtlib.WWTControl.singleton.tourEdit.playing;
    }

    wwtlib.WWTControl.singleton.tourEdit.tourStopList.refreshCallback = function () {
        $scope.$applyAsync(function () {

            $scope.tourStops = $scope.currentTour.get_tourStops().map(function (s) {
                s.description = s.get_description();
                s.thumb = s.get_thumbnail();
                s.duration = s.get_duration();
                s.secDuration = Math.round(s.duration / 1000);
                if (s.secDuration < 10) {
                    s.secDuration = '0' + s.secDuration;
                }
                s.secDuration = '0:' + s.secDuration;
                $scope.currentTour.duration += s.duration;
                return s;
            });

        });
    }
}]);

    