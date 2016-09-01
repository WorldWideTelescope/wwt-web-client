wwt.controllers.controller('CurrentTourController', [
    '$scope', '$rootScope', 'Util', 'MediaFile','AppState','$timeout',
    function ($scope, $rootScope, util, media,appState,$timeout) {
    var tourEdit = $scope.tourEdit = wwtlib.WWTControl.singleton.tourEdit;
    var tour;
    var mainScope = angular.element('div.desktop').scope();
    $scope.slideNumbering = appState.get('slideNumbering');
    $scope.overlayList = appState.get('overlayList');

    $scope.init = function (curTour) {
        tourEdit = $scope.tourEdit = wwtlib.WWTControl.singleton.tourEdit;
        $rootScope.currentTour = $scope.tour = tour = tourEdit.get_tour();
        tourEdit.tourEditorUI.editTextCallback = function (textObject, onFinished) {
            $scope.editText = { textObject: textObject, onFinished: onFinished };
            $('#editTourText').click();
        };
        tourEdit.tourStopList.refreshCallback = mapStops;
        $scope.editText = null;
        mainScope.ribbon.tabs[1].menu['Show Slide Overlays'] = [$scope.showOverlayList];
        mainScope.ribbon.tabs[1].menu['Show Slide Numbers'] = [$scope.showSlideNumbers]; 
        $rootScope.$on('escKey', showSlides);
        $rootScope.$on('closeTour', closeTour);
        $rootScope.$watch('editingTour', initEditMode);
        if (mainScope.autoEdit) {
            showSlides();
            $rootScope.editingTour = true;
        }
        
    };


    var showSlides = function () {
        mapStops(true);
        $scope.$applyAsync(showTourSlides);

    };
   
    var initEditMode = function () {
        if ($rootScope.editingTour !== true) { return; }
        tour._editMode = true;
        tourEdit.pauseTour();
        $('#contextmenu,#popoutmenu').on('click', function () {
            mapStops.apply($scope, []);
        });
        setTimeout(initVolumeSliders, 111);
        $('canvas').on('dblclick click', function () {
            mapStops.apply($scope, []);
        });
    };
    var closeTour = function () {
        console.trace('closetour');
        $scope.tourEdit = tourEdit = null;
        $rootScope.currentTour = $scope.tour = tour = null;
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
        var volumeOpts = function (barEl) {
            
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
                    var audio = barEl.attr('id') === 'voiceVol' ? $scope.activeSlide.voice : $scope.activeSlide.music;
                    if (audio) {
                        audio.set_volume(this.css.left);
                    }
                },
                oncomplete: function () {
                    barEl.removeClass('moving');
                    var audio = barEl.attr('id') === 'voiceVol' ? $scope.activeSlide.voice : $scope.activeSlide.music;
                    $scope.$applyAsync(function(){
                        audio.vol = this.css.left;
                    });
                }
            }
        };
        var musicVol = new wwt.Move(volumeOpts($('#musicVol')));
        var voiceVol = new wwt.Move(volumeOpts($('#voiceVol')));

    };

    $scope.tourProp = function ($event, prop) {
        tour['set_' + prop]($event.target.value);
    };
    $scope.saveTour = function () {
        
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            var blob = tour.saveToBlob();
            window.navigator.msSaveOrOpenBlob(blob);
        }
        else {
            // window.open(saveUrl, '_blank');
            var saveUrl = tour.saveToDataUrl();
            window.location.assign(saveUrl);
        }
        //window.location.assign(saveUrl);
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

        if (isImage) {
            tourEdit.tourEditorUI.addPicture(file);
        }
        else {
            tourEdit.tourEditorUI.addAudio(file, mediaKey === 'music');
            $timeout(bindAudio, 500);
        }
    };
    
    var showTourSlides = function () {
        $('#ribbon,.top-panel,.context-panel,.layer-manager').removeClass('hide').fadeIn(400);
        console.log(tourEdit.playing);
        setTimeout(function () {
            $rootScope.stopScroller = $('.scroller').jScrollPane({ scrollByY: 155, horizontalDragMinWidth: 155 }).data('jsp');
            $(window).on('resize', function () {
                
                $rootScope.stopScroller.reinitialise();
            });
        }, 500);
    };

    $scope.showContextMenu = function (index,e) {
        if (e) {
            
            $scope.selectStop(index);
            tourEdit.tourStopList_MouseClick(index, e);
            
        }
    };
    $scope.selectStop = function (index, e) {
        $scope.$applyAsync(function () { 
            
            $scope.activeSlide = tourEdit.tourStopList.selectedItem = $scope.tourStops[index];
            
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
            tourEdit.tour_CurrentTourstopChanged();
            $scope.$broadcast('initSlides');
            $timeout(bindAudio, 500);
        });
    };

    var bindAudio = function () {
        var mapAudioProps = function (audio) {
            if (audio) {
                audio.muted = audio.get_mute();
                audio.name = audio._name === '' ? audio._filename$1 : audio._name;
                audio.vol = audio.get_volume();
                audio.mute = function (flag) {
                    $scope.$applyAsync(function () {
                        audio.muted = flag;
                        audio.set_mute(flag);
                    });
                }
            }

            return audio;
        }

        $scope.activeSlide.music = mapAudioProps($scope.activeSlide._musicTrack);
        $scope.activeSlide.voice = mapAudioProps($scope.activeSlide._voiceTrack);
    }

    $scope.showStartCameraPosition = function (index) {
        tour.set_currentTourstopIndex(index);
        tourEdit.tourStopList_ShowStartPosition();
    };
    $scope.showEndCameraPosition = function (index) {
        tour.set_currentTourstopIndex(index);
        tourEdit.tourStopList_ShowEndPosition();
    };
         
    $scope.playButtonClick = function () {
        if (tourEdit.playing) {
            tourEdit.pauseTour();
        }
        else {
            tourEdit.playFromCurrentTourstop();
        } 
        //else { 
        //    tourEdit.playNow(true); 
        //}
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
                s.btime = s.get__transitionOutTime();
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

    