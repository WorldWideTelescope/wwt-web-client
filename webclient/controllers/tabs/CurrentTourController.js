wwt.controllers.controller('CurrentTourController', [
    '$scope', '$rootScope', 'Util', 'MediaFile','AppState',
    function ($scope, $rootScope, util, media,appState) {
    var tourEdit = $scope.tourEdit = wwtlib.WWTControl.singleton.tourEdit;
    var tour;
    var mainScope = angular.element('div.desktop').scope();
    $scope.slideNumbering = appState.get('slideNumbering');
    $scope.overlayList = appState.get('overlayList');
    $scope.init = function (curTour) {
        $scope.musicFileUrl = false;
        $scope.voiceOverFileUrl = false;
        $scope.musicPlaying = false;
        $scope.voiceOverPlaying = false;
        $rootScope.currentTour = $scope.tour = tour = tourEdit.get_tour();
        tourEdit.tourStopList.refreshCallback = mapStops;
        $scope.editText = null;
        tourEdit.tourEditorUI.editTextCallback = function (textObject, onFinished) {
            $scope.editText = { textObject: textObject, onFinished: onFinished };
            $('#editTourText').click();
        } 
        mapStops(true);
        
        //$rootScope.$on('escKey', function () {
            //$scope.$applyAsync(showTourSlides);
        //});
        $rootScope.$watch('editingTour', function () { });
        if (true){//util.isDebug) {
            showTourSlides();
            mainScope.ribbon.tabs[1].menu['Show Slide Overlays'] = [$scope.showOverlayList];
            mainScope.ribbon.tabs[1].menu['Show Slide Numbers'] = [$scope.showSlideNumbers];
        }
        $('#contextmenu,#popoutmenu').on('click', mapStops);
        setTimeout(initVolumeSliders, 111);
        $('canvas').on('dblclick click', $scope.$applyAsync);
    };

    $scope.showSlideNumbers = function () {
        $scope.$applyAsync(function () {
            $scope.slideNumbering = !$scope.slideNumbering;
            appState.set('slideNumbering', $scope.slideNumbering);
        });
    }
    $scope.showOverlayList = function () {
        $scope.$applyAsync(function () {
            $scope.overlayList = !$scope.overlayList;
            appState.set('overlayList', $scope.overlayList);
        });
    }

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
        var xml = tour.getTourXML();
        console.log(xml);
        // media.saveTour().then(function (tour) {
        //     console.log(tour);
        //});
    };
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
                console.log('image created: ' + mediaResult.url);
                tourEdit.tourEditorUI.addPicture(mediaResult.url);
                media.getBinaryData(mediaResult.url).then(function (binary) {
                    window.bdata = binary;
                    console.log(binary.length, binary);
                });

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
        $scope.$applyAsync(function () {
            tourEdit.tourStopList.selectedItem = $scope.tourStops[index];
            $scope.activeIndex = index;
            if (e && e.shiftKey) {
                tourEdit.tourStopList.selectedItems = {};
                for (var i = Math.min(index, $scope.lastFocused) ; i <= Math.max(index, $scope.lastFocused) ; i++) {
                    tourEdit.tourStopList.selectedItems[i] = $scope.tourStops[i];
                }
            }
            else if (e && e.ctrlKey) {
                var keys = Object.keys(tourEdit.tourStopList.selectedItems);
                if (tourEdit.tourStopList.selectedItems[index] && keys.length > 1) {
                    delete tourEdit.tourStopList.selectedItems[index];
                    $scope.activeIndex = keys[0];//set to first key
                } else {
                    tourEdit.tourStopList.selectedItems[index] = $scope.tourStops[index];
                }
            }
            else {
                tourEdit.tourStopList.selectedItems = {};
                tourEdit.tourStopList.selectedItems[index] = $scope.tourStops[index];
            }
            tour.set_currentTourstopIndex($scope.activeIndex);
            $scope.lastFocused = index;
            $scope.selectedSlide = $scope.tourStops[$scope.activeIndex];
            $scope.$broadcast('initSlides');
        });
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
                tour.duration += s.duration;

                //placeholder values until transition api is there
                s.atime = s.get__transitionTime();
                s.btime = s.get__transitionOutTime();;
                s.holdtime = s.get__transitionHoldTime();
                s.transitionType = s.get__transition();
                s.isMaster = s.get_masterSlide();
                return s;
            });
            tour.minuteDuration = Math.floor(tour.duration / 60000);
            tour.secDuration = Math.floor((tour.duration % 60000) / 1000);
            $scope.tour = tour;

            if (isInit && isInit===true) {
                $scope.selectStop(0);
                if ($scope.tourStops.length < 2 && tour._title ==='New Tour') {
                    setTimeout(function () {
                        $('#newTourProps').click();
                    }, 500);
                }
                
            }
            $scope.$broadcast('initSlides');
        });
    };

    

    $scope.setStopTransition = function (index, transitionType, transTime) {
        if (transitionType || transitionType === 0) {
            var stop = $scope.tourStops[index];
            stop.set__transition(transitionType);
            stop.transitionType = transitionType;
            return;
        } else if (transTime && typeof transTime === 'string') {
            switch (transTime) {
                case 'atime':
                    stop.set__transitionTime(stop.atime);
                    break;
                case 'btime':
                    stop.set__transitionOutTime(stop.btime);
                    break;
                case 'holdtime':
                    stop.set__transitionHoldTime(stop.holdtime);
                    break;
            }
        }
    };
}]);

    