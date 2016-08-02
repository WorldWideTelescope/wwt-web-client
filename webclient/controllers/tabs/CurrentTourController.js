wwt.controllers.controller('CurrentTourController', [
    '$scope', '$rootScope', 'Util', 'MediaFile',
    function ($scope, $rootScope, util, media) {
    var tourEdit = $scope.tourEdit = wwtlib.WWTControl.singleton.tourEdit;
    var tour;
    $scope.init = function (curTour) {
        $scope.musicFileUrl = false;
        $scope.voiceOverFileUrl = false;
        $scope.musicPlaying = false;
        $scope.voiceOverPlaying = false;
        $rootScope.currentTour = $scope.tour = tour = tourEdit.get_tour();
        tourEdit.tourStopList.refreshCallback = mapStops;
        mapStops(true);
        
        //$rootScope.$on('escKey', function () {
            //$scope.$applyAsync(showTourSlides);
        //});
        $rootScope.$watch('editingTour', function () { });
        if (true){//util.isDebug) {
            showTourSlides();
        }
        $('#contextmenu,#popoutmenu').on('click', mapStops);
        setTimeout(initVolumeSliders, 111);
    };

    var initVolumeSliders = function () {
        var volumeOpts = function (barEl, player) {
            player.volume = .5;
            return {
                el: barEl,
                bounds: {
                    x: [-50, 50],
                    y: [0, 0]
                },
                onstart: function () {
                    barEl.addClass('moving');
                },
                onmove: function () {
                    player.volume = this.css.left / 100;
                },
                oncomplete: function () {
                    barEl.removeClass('moving');
                }
            }
        };
        var musicVol = new wwt.Move(volumeOpts($('#musicVol'), $('#musicPlayer')[0]));
        var voiceVol = new wwt.Move(volumeOpts($('#voiceVol'), $('#voiceOverPlayer')[0]));

    };

    $scope.tourProp = function ($event, prop) {
        tour['set_' + prop]($event.target.value);
    };
    $scope.saveTour = function () {
        media.saveTour().then(function (tour) {
            console.log(tour);
        });
    }
    $scope.addShape = function (type) {
        tourEdit.tourEditorUI.addShape('', type);
    }
    
    $scope.mediaFileChange = function (e, mediaKey, isImage) {
        console.time('storeLocal: ' + mediaKey);
        var file = e.target.files[0];
        if (!file.name) {
            return;
        }
        $scope[mediaKey + 'FileName'] = file.name;
        media.addTourMedia(mediaKey, file).then(function (mediaResult) {
            //hook to the mediaResult.url here;
            
            if (!isImage) {
                $scope[mediaKey + 'FileUrl'] = true;
                $('#' + mediaKey + 'Player').attr('src', mediaResult.url);
                $scope[mediaKey + 'Playing'] = false;
            }
            console.timeEnd('storeLocal: ' + mediaKey);
            if (!isImage) {
                tourEdit.tourEditorUI.addAudio(mediaResult.url);
                //media.getBinaryData(mediaResult.url).then(function (binary) {
                //    console.log('binary string recieved - not logging because length = ' + binary.length);
                //});
            } else {
                tourEdit.tourEditorUI.addPicture(mediaResult.url);
                console.log('image created: '+ mediaResult.url)
            }
        });
        
    }

    $scope.toggleSound = function (mediaKey) {
        var audio = $('#' + mediaKey + 'Player')[0];
        if ($scope[mediaKey +'Playing']) {
            audio.pause();
        } else {
            audio.play();
        }
        $scope[mediaKey + 'Playing'] = !$scope[mediaKey + 'Playing'];
    }

    var showTourSlides = function () {
        $('#ribbon,.top-panel,.context-panel,.layer-manager').removeClass('hide').fadeIn(400);
        //tourEdit.pauseTour();
        $rootScope.tourPaused = true;
        $scope.escaped = true;
        //if (util.isDebug) {
            $rootScope.editingTour = true;
        //}
        setTimeout(function () {
            $rootScope.stopScroller = $('.scroller').jScrollPane({ scrollByY: 155, horizontalDragMinWidth: 155 }).data('jsp');
            $(window).on('resize', function () {
                
                $rootScope.stopScroller.reinitialise();
            });
        }, 200);
    };

    $scope.showContextMenu = function (index,e) {
        if (e) {
            
            $scope.selectStop(index);
            tourEdit.tourStopList_MouseClick(index, e);
            
        }
    };
    $scope.selectStop = function (index, e) {
        tourEdit.tourStopList.selectedItem = index;
        tourEdit.tourStopList.selectedItems = {};
        tourEdit.tourStopList.selectedItems[index] = $scope.tourStops[index];
        tour.set_currentTourstopIndex(index);
        $scope.activeIndex = index;
    };

    $scope.showStartCameraPosition = function (index) {
        tour.set_currentTourstopIndex(index);
        tourEdit.tourStopList_ShowStartPosition();
    };
    $scope.showEndCameraPosition = function (index) {
        tour.set_currentTourstopIndex(index);
        tourEdit.tourStopList_ShowEndPosition();
    };

    $scope.pauseTourEdit = function () {
        if (tourEdit.playing) {
            tourEdit.pauseTour();
        }
        else if ($scope.activeIndex) {
            tourEdit.playFromCurrentTourstop();
        } else {
            tourEdit.playNow(true);
        }
        $rootScope.tourPaused = !wwtlib.WWTControl.singleton.tourEdit.playing;
    };

    var mapStops = $scope.refreshStops = function (isInit) {
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

            //placeholder values until transition api is there
                s.atime = 2;
                s.btime = 2;
                s.holdtime = 4;

                s.transitionType = s.get__transition();

                return s;
            });
            tour.minuteDuration = Math.floor(tour.duration / 60000);
            tour.secDuration = Math.floor((tour.duration % 60000) / 1000);
            $scope.tour = tour;
            
            if (isInit) {
                $scope.selectStop(0);
            }
        });
    }

    
}]);

    