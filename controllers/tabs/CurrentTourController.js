wwt.controllers.controller('CurrentTourController', ['$scope', '$rootScope', function($scope,$rootScope) {
    var tourEdit = wwtlib.WWTControl.singleton.tourEdit;
    var tour;
    $scope.init = function (curTour) {
        $rootScope.currentTour = $scope.tour = tour = tourEdit.get_tour();
        tourEdit.tourStopList.refreshCallback = mapStops;
        mapStops();  
    };

    $scope.showContextMenu = function (index,e) {
        if (e) {
            tour.set_currentTourstopIndex(index);
            tourEdit.tourStopList_MouseClick(index, e);
        }
    };
    $scope.gotoStop = function (index, e) {
        tour.set_currentTourstopIndex(index);
    };

    $scope.showStartCameraPosition = function (index) {
        tour.set_currentTourstopIndex(index);
        tourEdit.tourStopList_ShowStartPosition();
    };
    $scope.showEndCameraPosition = function (index) {
        tour.set_currentTourstopIndex(index);
        tourEdit.tourStopList_ShowEndPosition();
    };

    $scope.pauseTour = function () {
        if (tourEdit.playing) {
            tourEdit.pauseTour();
        }
        else {
            tourEdit.playNow(true);
        }
        $rootScope.tourPaused = !wwtlib.WWTControl.singleton.tourEdit.playing;
    }

    var mapStops = function () {
        $scope.$applyAsync(function () { 
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
            $scope.tour = tour;
        });
    }

    
}]);

    