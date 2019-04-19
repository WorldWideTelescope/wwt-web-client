/*
This controller is the hub of the web client - with all the shared functionality
that needs to live at the top of the scope chain residing here.

UIManager was created to add some functions to the rootScope that could be removed
from the main controller to reduce its weight.

This file is too large and needs to be componentized a bit more. This is an ongoing
cleanup process.
*/
wwt.controllers.controller('MainController',
  ['$scope',
    '$rootScope',
    'UILibrary',
    '$q',
    'AppState',
    'Localization',
    '$timeout',
    'FinderScope',
    'SearchData',
    'Places',
    'Util',
    'HashManager',
    'Skyball',
    'SearchUtil',
    '$modal',
    '$element',
    '$cookies',
    'AutohidePanels',
    function ($scope, $rootScope, uiLibrary, $q, appState, loc, $timeout, finderScope, searchDataService, places, util, hashManager, skyball, searchUtil, $modal, $element, $cookies, AutohidePanels) {
      //TODO - figure out how to clean up lame long list of dependencies injected
      var ctl;

      //#region LookAt/Imagery
      var initialPass = true;
      $scope.lookTypes = ['Earth', 'Planet', 'Sky', 'Panorama', 'SolarSystem'];
      $scope.lookAt = 'Sky';
      $scope.imagery = [[], [], [], [], []];


      $scope.lookAtDropdownChanged = function (lookAtType) {
        if (lookAtType) {
          $scope.lookAt = lookAtType;
        }
        setTimeout(function () {
          $scope.lookAtChanged(null, true);
          $scope.setTrackingObj(false);

        }, 1);
      };

      $scope.lookAtChanged = function (imageryName, dropdownInvoked, noUpdate, keepCamera) {
        setTimeout(wwt.resize, 120);
        if (!keepCamera) {
          util.resetCamera(true);
        }
        $timeout(function () {
          if ($('#lstLookAt').length) {
            $scope.lookAt = $('#lstLookAt option:selected').text();
          }
          if ($scope.lookAt === '') {
            $scope.lookAt = 'Sky';
          }
          var collection = $scope.imagery[$.inArray($scope.lookAt, $scope.lookTypes)];
          if (collection[0] !== '-')
            collection.splice(0, 0, '-');
          if (imageryName == '') imageryName = '-';
          $scope.surveys = collection;
          var foundName = false;


          // HACK ALERT (Mars was hardcoded from Visible Imagery)
          if (imageryName === 'Mars') {
            imageryName = 'Visible Imagery';
          }
          if (imageryName) {
            $.each(collection, function (i, item) {
              if (item !== '' && item.get_name && (item.get_name().indexOf(imageryName) === 0 || imageryName.indexOf(item.get_name()) === 0)) {
                $scope.backgroundImagery = item;
                foundName = true;
              }
            });
          }
          if (!foundName) {
            if (initialPass || dropdownInvoked) {
              setTimeout(function () {
                initialPass = false;
              }, 500);
              $timeout(function () {
                $scope.backgroundImagery = collection[1];
                $scope.setSurveyBg();

              }, 123);
              return;
            } else if (!noUpdate) {
              $scope.backgroundImagery = collection[0];
              return;
            } else {
              return;
            }
          }
          $scope.setSurveyBg();
          $rootScope.lookAt = $scope.lookAt;
        }, 100);
      };
      $scope.setLookAt = function (lookAt, imageryName, noUpdate, keepCamera) {
        if (!lookAt || !isNaN(parseInt(lookAt))){
          lookAt = wwtlib.WWTControl.singleton.renderContext.get_backgroundImageset()._dataSetType;
          lookAt = $scope.lookTypes[lookAt];
          if ($scope.lookAt=== lookAt){
            return;
          }

        }
        $scope.lookAt = lookAt;
        //if (lookAt === 'Planet' && !imageryName) {
        //    imageryName = 'Mars';
        //}
        $scope.lookAtChanged(imageryName, false, noUpdate, keepCamera);
        setTimeout(wwt.resize, 1200);
      };
      $rootScope.setLookAt = $scope.setLookAt;
      //#endregion

      //#region initialization
      var initCanvas = function () {
        ctl = $rootScope.ctl = wwtlib.WWTControl.initControlParam("WWTCanvas", appState.get('WebGl'));

        // The .8 release of scriptsharp changed the location of the canCast function
        // This logic exists to ensure backwards compatibility when testing an older version
        // of the framework.
        if (window.Type && Type.canCast) {
          if (window.ss) {
            window.ss.canCast = Type.canCast;
          } else {
            window.ss = {canCast: Type.canCast};

          }
        }
        wwt.wc = ctl;
        wwt.resize();
        ctl.add_ready(function () {
          var imageSets = wwtlib.WWTControl.imageSets;
          $scope.surveys = [];
          $.each(imageSets, function () {
            var typeIndex = this.get_dataSetType();
            this.name = this.get_name() === 'Visible Imagery' ? 'Mars' : this.get_name();
            if (typeIndex === 2 && this.name.toLowerCase().indexOf('hipparcos') !== -1) {//hipparcos is broken :(
              $scope.surveys.push(this);
            }
            try {
              if (!(typeIndex === 2 && this.name.toLowerCase().indexOf('hipparcos') !== -1)) {//hipparcos is broken :(
                $scope.imagery[typeIndex].push(this);
              }
            } catch (er) {
              util.log(typeIndex, this);
            }
          });
          $scope.backgroundImagery = {
            name: 'Digitized Sky Survey (Color)',
            get_name: function () {
              return 'Digitized Sky Survey (Color)';
            }
          };
          $scope.lookAtChanged();
          AutohidePanels.init();

        });
        ctl.settings.set_showConstellationBoundries(false);

        util.resetCamera(true);
        $(window).on('resize', function () {
          wwt.resize();
          $scope.$applyAsync(function () {
            $scope.smallVP = wwt.smallVP;
          });
        });
        ctl.endInit();
        $rootScope.singleton = wwtlib.WWTControl.singleton;
        initContext();
        $rootScope.$on('hashChange', hashChange);

        $timeout(function () {
          var hash = hashManager.getHashObject();
          $rootScope.$broadcast('hashChange', hash);
          $scope.smallVP = wwt.smallVP;
          if (wwt.definitelyMobile && !util.isMobile) {
            location.href = util.mobileLink();
          }
        }, 100);

        //hashChange(null, hashManager.getHashObject());
      };

      var hashChange = function (e, obj) {
        if (!obj) {
          obj = hashManager.getHashObject();
        }
        var goto = function () {
          if (!obj) {
            obj = hashManager.getHashObject();
          }
          /*console.log('goto', parseFloat(obj['ra']) * 15,
            parseFloat(obj['dec']),
            parseFloat(obj['fov']),
            false);*/
          ctl.gotoRaDecZoom(
            parseFloat(obj['ra']) * 15,
            parseFloat(obj['dec']),
            parseFloat(obj['fov']),
            false
          );
          obj = null;
        }
        var setLookAtHash = function (cb) {
          $timeout(function () {
            $scope.setLookAt(obj['lookAt'], obj['imagery'] && obj['imagery'].split('_').join(' '));
            if (cb) {
              cb();
            }
            else if (obj['ra'] && (obj['lookAt'] === 'Earth' || obj.lookAt === 'Planet')) {
              setTimeout(goto, 2220);
            }

          }, 2000);
        }
        var loadPlace = function (openPlace) {

          $('#loadingModal').modal('show');
          var goPlace = function (place, delay) {
            if (obj['ra']) {
              wwt.wc.add_arrived(goto);
            }
            $scope.setForegroundImage(place);
            $('#loadingModal').modal('hide');
            if (delay === -1) return;
            if (obj['cf']) {
              setTimeout(function () {

                if (obj['cf']) {
                  $('.cross-fader a.btn').css('left', parseFloat(obj['cf']));

                  var ensureProperOpacity = function () {
                    ctl.setForegroundOpacity(parseFloat(obj['cf']));
                  };
                  for (var i = 1; i < 6; i++) {
                    setTimeout(ensureProperOpacity, i * 1000);
                  }
                }

              }, delay || 3333);
            }


          };
          if (obj['lookAt'] && obj['lookAt'] == 'SolarSystem') {
            //obj['imagery'] = undefined;
            setLookAtHash(function () {
              searchUtil.getPlaceById(openPlace).then(function (p) {
                console.log(p);
                setTimeout(function () {
                  goPlace(p);
                }, 2222);
              });
            });
          } else {
            searchUtil.getPlaceById(openPlace).then(goPlace);
          }
        };


        if (obj['place']) {
          var openPlace = obj['place'];
          if (!isNaN(parseInt(openPlace.charAt(0)))) {
            loadPlace(openPlace)
          }
        }
        else if (obj['ra'] !== undefined) {
          setTimeout(goto, 500);
        }
        try {
          if (!obj) {
            obj = hashManager.getHashObject();
          }
          if (obj['lookAt']) {
            setLookAtHash();

          }
          else if (obj['imagery']) {
            $timeout(function () {
              $scope.setLookAt('Sky', obj['imagery']);
            }, 2000);

          }
        } catch (ex) {
          setTimeout(hashChange, 2000);
          console.log(ex);
        }


      }

      $scope.initUI = function () {
        $scope.ribbon = {
          tabs: [
            {
              label: 'Explore',
              button: "rbnExplore",
              mobileLabel: 'Explore Collections',
              mobileAction: function () {
                $('#exploreModalLink').click();
              },
              menu: {
                Open: {
                  'Tour...': [$scope.openItem, 'tour'],
                  'Collection...': [$scope.openItem, 'collection'],
                  'Image...': [$scope.openItem, 'image'],
                  'FITS Image...': [$scope.openItem, 'FITS image']
                },
                sep1: null,
                'Tour WWT Features': [$scope.tourFeatures],
                'Show Welcome Tips': [showTips],
                'Show Finder (right click)': [$scope.showFinderScope],
                'WorldWide Telescope Home': [util.nav, '/home'],
                'Getting Started (Help)': [util.nav, '/Learn/'],
                'WorldWide Telescope Terms of Use': [util.nav, '/Terms'],
                'About WorldWide Telescope': [util.nav, '/About']/*,
          sep2: null,
          'Exit': [util.nav, 'Exit'],*/
              }
            }, {
              label: 'Guided Tours',
              button: 'rbnTours',
              menu: {

                'Tour Home Page': [util.nav, '/Learn/Exploring#guidedtours'],
                'Music and other Tour Resources': [util.nav, '/Download/TourAssets'],
                sep2: null,

                'Create a New Tour...': [$scope.createNewTour],

              }
            }, {
              label: 'Search',
              button: 'rbnSearch',
              menu: {
                'Search Now': [function () {
                  $timeout(function () {
                    changePanel('Search');
                  });
                }],
                'VO Cone Search': [function () {
                  var modalScope = $rootScope.$new();
                  modalScope.customClass = 'vo-cone-modal';
                  var coneSearchModal = $modal({
                    scope: modalScope,
                    templateUrl: 'views/modals/centered-modal-template.html',
                    contentTemplate: 'views/modals/vo-cone-search.html',
                    show: true,
                    placement: 'center',
                    backdrop: false
                  });
                }]
              }
            },
            {
              label: 'Communities',
              button: 'rbnCommunities',
              menu: {
                'Search Communities': [util.nav, '/Community']
              }
            }, {
              label: 'View',
              button: 'rbnView',
              menu: {
                'Reset Camera': [util.resetCamera],
                'Share this View': [copyShortcut],
                'Toggle Full Screen View (F11)': [util.toggleFullScreen],
                'Toggle Layer Manager': [$scope.toggleLayerManager]
              }
            }, {
              label: 'Settings',
              button: 'rbnSettings',
              menu: {
                'Restore Defaults': [$scope.restoreDefaultSettings],
                'Product Support': [util.nav, '/Support/IssuesAndBugs']
              }
            }]
        };
        if (util.getQSParam('ads')) {
          $scope.ribbon.tabs.push({
            label: 'ADS',
            button: 'rbnADS',
            menu: {
              'ADS Home Page': [function () {
                window.open('//www.adsass.org/wwt');
              }]
            }
          });
        }
        $scope.activePanel = util.getQSParam('ads') ? 'ADS' : 'Explore';

        $scope.UITools = wwtlib.UiTools;
        $scope.Planets = wwtlib.Planets;

        $rootScope.$on('viewportchange', viewportChange);
        util.trackViewportChanges();
        skyball.init();


        $(window).on('keydown', function (e) {
          if (e.which === 187) {
            ctl.zoom(.66666666666667);
          } else if (e.which === 189) {
            ctl.zoom(1.5);
          }
        });
      };

      var changePanel = function (panel) {
        $('body').append($('#researchMenu'));
        $scope.expandTop(false);
        $scope.activePanel = panel;
      }

      var initContext = function () {
        var isAds = util.getQSParam('ads') != null;


        var bar = $('.cross-fader a.btn').css('left', isAds ? 50 : 100);

        var xf = new wwt.Move({
          el: bar,
          bounds: {
            x: [isAds ? -50 : -100, isAds ? 50 : 0],
            y: [0, 0]
          },
          onstart: function () {
            bar.addClass('moving');
          },
          onmove: function () {
            ctl.setForegroundOpacity(this.css.left);
          },
          oncomplete: function () {
            bar.removeClass('moving');
          }
        });

        wwt.resize();
        if (util.getQSParam('tourUrl')) {
          $scope.playTour(decodeURIComponent(util.getQSParam('tourUrl')));
        }
        if (util.getQSParam('tour')) {
          $scope.playTour(decodeURIComponent(util.getQSParam('tour')));
        }
        uiLibrary.addDialogHooks();
        wwt.wc.add_refreshLayerManager(function () {
          $scope.$applyAsync(function () {
          });
        });

      };
      //#endregion

      //#region viewport/finderscope
      var viewportChange = function (event, viewport) {
        if (viewport.isDirty || viewport.init) {
          $rootScope.viewport = viewport;
          $scope.coords = wwtlib.Coordinates.fromRaDec(viewport.RA, viewport.Dec);
          wwt.coords = $scope.coords;
          wwt.viewport = viewport;
          var lng = $scope.coords.get_lng();
          if ($scope.lookAt==='Earth') {
            lng = (((180 - (($scope.coords.get_RA()) / 24.0 * 360) - 180) + 540) % 360) - 180;
          }
          $scope.formatted = {
            RA: util.formatHms(viewport.RA, true),
            Dec: util.formatHms(viewport.Dec, false, true),
            Lat: util.formatHms($scope.coords.get_lat(), false, false),
            Lng: util.formatHms(lng/*$scope.coords.get_lng()*/, false, false),
            Zoom: util.formatHms(viewport.Fov)
          };


          trackConstellation();
          if (viewport.init) {
            $timeout(trackConstellation, 1200);
          }
        }
        if ((viewport.isDirty || viewport.finderMove) && checkVisibleFinderScope()) {
          var found = finderScope.scopeMove();
          if (found) {
            $timeout(function () {
              $scope.scopePlace = found;
              $scope.drawCircleOverPlace($scope.scopePlace);
            });

          }
        }
      }

      var trackConstellation = function () {
        $scope.formatted.Constellation = $scope.constellations.fullNames ? $scope.getFromEn($scope.constellations.fullNames[$rootScope.singleton.constellation]) : '...';
      }

      var checkVisibleFinderScope = function () {
        if ($('.finder-scope:visible').length) {
          finderActive = true;
        } else if (finderActive) {
          finderActive = false;
          clearInterval(finderTimer);
        }
        return finderActive;
      }

      $scope.$on('showFinderScope', function () {
        $scope.showFinderScope();
      });

      $scope.$on('showContextMenu', function () {
        $scope.showContextMenu();
      });

      var finderTimer, finderActive = false, finderMoved = true;
      $scope.showFinderScope = function (event) {
        if ($scope.lookAt === 'Sky' && !$scope.editingTour) {
          var finder = $('.finder-scope');
          finder.toggle(!finder.prop('hidden')).css({
            top: event ? event.pageY - 88 : 180,
            left: event ? event.pageX - 301 : 250
          });
          if (finder.prop('hidden')) {
            finder.prop('hidden', false);
            finder.fadeIn(function () {
              if (!finder.prop('movebound')) {
                var finderScopeMove = new wwt.Move({
                  el: finder,
                  target: finder.find('.moveable'),
                  onmove: function () {
                    finderMoved = true;

                  }
                });
              }
              finder.prop('movebound', true);
            });
          }
          finderScope.init();
          if (event) {
            event.preventDefault();
          }
          finderTimer = setInterval(pollFinder, 400);
          viewportChange(null, {finderMove: true});
        }
      };

      var pollFinder = function () {
        if (checkVisibleFinderScope()) {
          if (finderMoved) {
            viewportChange(null, {finderMove: true});
            finderMoved = false;
          }
        }
      }

      $scope.initFinder = function () {
        searchDataService.getData().then(function () {

          var finder = $('.finder-scope').prop('hidden', true).fadeOut();
          finder.find('.close, .close-btn').on('click', function () {
            finder.fadeOut(function () {
              finder.prop('hidden', true);
            });
          });


          //$('#WWTCanvas').on('contextmenu', $scope.showFinderScope);
          $scope.showObject = function (place) {
            $rootScope.singleton.gotoTarget(place);
            $('.finder-scope').hide();
          }
        });
      };

      //#endregion

      //#region set fb/bg...
      var solarSystemInit = false;
      $scope.setSurveyBg = function (imageryName, imageSet) {

        if (imageryName) {
          if (imageryName === 'Mars') {
            imageryName = 'Visible Imagery';
          }
          var foundName = false;
          $.each($scope.surveys, function () {
            if (this.name && (this.name.indexOf(imageryName) === 0 || imageryName.indexOf(this.name) === 0)) {
              $scope.backgroundImagery = this;
              foundName = true;
            }
          });
          if (!foundName) {
            $scope.backgroundImagery = '';
            ctl.setBackgroundImageByName(imageryName);
            if (imageSet) {
              $rootScope.singleton.renderContext.set_backgroundImageset(imageSet);
            }
            return;
          }
        }

        if ($scope.backgroundImagery) {
          if ($scope.backgroundImagery !== '?')
            ctl.setBackgroundImageByName($scope.backgroundImagery.get_name());
          if (typeof $scope.backgroundImagery != 'string' && $scope.backgroundImagery.get_name() === '3D Solar System View' && !solarSystemInit) {
            setTimeout(function () {
              var bar = $('.planetary-scale .btn');
              var ps = new wwt.Move({
                el: bar,
                bounds: {
                  x: [0, 66],
                  y: [0, 0]
                },
                onstart: function () {
                  bar.addClass('moving');
                },
                onmove: function () {
                  ctl.settings.set_solarSystemScale(Math.max(this.css.left * 1.5, 1));
                },
                oncomplete: function () {
                  bar.removeClass('moving');
                }
              });
              solarSystemInit = true;
            }, 10);
          }
        }

      };

      $scope.setSurveyProperties = function () {
        $scope.propertyItem = $scope.backgroundImagery;
        $scope.propertyItem.isSurvey = true;
      };

      $scope.setActiveItem = function (item) {
        $scope.activeItem = item;
        if (item.guid) {
          $scope.shareUrl = hashManager.setHashVal('place', item.guid, true, true);

        }
        if (item.get_studyImageset) {
          $scope.activeItem.imageSet = item.get_studyImageset();
        }
      };

      $scope.setForegroundImage = function (item) {
        if (item.guid) {
          $scope.shareUrl = hashManager.setHashVal('place', item.guid, true, true);
        }
        if (util.isMobile) {
          $('#explorerModal').modal('hide');
          $('#nboModal').modal('hide');
        }

        var imageSet = util.getImageset(item);
        if (imageSet && !item.isEarth) {
          wwtlib.WWTControl.singleton.renderContext.set_foregroundImageset(imageSet);
        }
        $scope.setTrackingObj(item);

        if (!item.isSurvey && ss.canCast(item, wwtlib.Place)) {
          $('.finder-scope').hide();
          //$('.cross-fader').parent().toggle(imageSet!=null);
          $rootScope.singleton.gotoTarget(item, false, !!$rootScope.instant, true);
        } else if (!item.isEarth) {
          ctl.setForegroundImageByName(imageSet.get_name());
        } else {
          $rootScope.singleton.renderContext.set_backgroundImageset(imageSet);
        }

        //$('.cross-fader').parent().show();

      };
      $scope.setBackgroundImage = function (item) {
        var imageSet = util.getImageset(item);
        if (imageSet) {
          $rootScope.singleton.renderContext.set_backgroundImageset(imageSet);
        }
        if (!item.isSurvey) {
          $rootScope.singleton.gotoTarget(item, false, !!$rootScope.instant, true);
        }
      };
      //#endregion

      //#region menu actions
      $scope.menuClick = function (menu) {
        $scope.keepMenu = true;
        var m = $('#topMenu');
        m.html('');
        $.each(menu, function (menuItem, action) {
          var item;
          if (menuItem.indexOf('sep') === 0) {
            item = $('<li class="divider" role="presentation"></li>');
          } else {
            item = $('<li><a href="javascript:void(0)"></a></li>');
            item.find('a').text(loc.getFromEn(menuItem));
            if ($.isPlainObject(action)) {
              item.addClass('dropdown-submenu').find('a').attr('tab-index', -1);
              var sub = $('<ul class=dropdown-menu></ul>');
              item.append(sub);
              $.each(action, function (subItemLabel, subItemAction) {
                var subItem = $('<li><a href="javascript:void(0)"></a></li>');
                subItem.find('a').on('click', function () {
                  subItemAction[0](subItemAction[1]);
                }).data('action', subItemAction).text(loc.getFromEn(subItemLabel));
                sub.append(subItem);
              });
            } else {
              item.find('a').on('click', function () {
                action[0](action[1]);
              }).data('action', action);
            }
          }
          m.append(item);
        });
        var caret = $('#tabMenu' + this.$index);
        m.css({
          top: caret.offset().top + caret.height(),
          left: caret.offset().left
        }).show();
        setTimeout(function () {
          $(document).on('click', hideMenu);
          $scope.keepMenu = false;
        }, 123);

      };

      var hideMenu = function () {
        if ($scope.keepMenu) {
          return;
        }
        $('#topMenu').hide();
        $(document).off('click', hideMenu);
      };
      $scope.tabClick = function (tab) {
        if ($rootScope.editingTour) {
          //$scope.finishTour();
        }
        $('body').append($('#researchMenu'));
        $scope.expandTop(false);
        $scope.activePanel = tab.label;
        appState.set('activePanel', tab.label);
      };
      $scope.openItem = function (type) {
        $scope.$applyAsync(function () {
          $rootScope.openType = type;
          $rootScope.$broadcast('openItem');
          if (type === 'collection') {
            $scope.tabClick($scope.ribbon.tabs[0]);
          }
          $('#openModal').modal('show');
        });
      };

      $scope.playTour = function (url, edit) {
        if (!edit) {
          util.goFullscreen();
        }

        $('.finder-scope').hide();
        wwt.wc.add_tourError(function(e){
          util.toggleFullScreen();
          $scope.$applyAsync(function(){
            wwt.tourPlaying =$rootScope.tourPlaying = false;
          });
          uiLibrary.showErrorMessage('There was an error loading this tour. The tour file may be damaged or inaccessible.');
          console.warn('caught ya!',$scope,e);
        });
        wwt.wc.add_tourReady(function () {
          console.log({ready:wwtlib.WWTControl.singleton.tourEdit});
          $scope.$applyAsync(function () {
            $scope.activeItem = {label: 'currentTour'};
            $scope.activePanel = 'currentTour';
            $scope.ribbon.tabs[1].menu['Edit Tour'] = [$scope.editTour];

          });
          if (edit){
            $scope.editTour();
          }
        });

        wwtlib.WWTControl.singleton.playTour(url);

        $scope.$applyAsync(function () {
          wwt.tourPlaying = $rootScope.tourPlaying = true;
          $rootScope.tourPaused = edit;
        });
        wwt.wc.add_tourEnded(tourChangeHandler);

        //wwt.wc.add_tourPaused(tourChangeHandler);

      };

      $scope.editTour = function () {
        $rootScope.$applyAsync(function () {
          $rootScope.editingTour = true;
        });
      };
      $scope.initSlides = function () {
        $rootScope.$broadcast('showingSlides');
      };

      $rootScope.closeTour = function ($event) {
        util.exitFullscreen();
        $event.preventDefault();
        $event.stopPropagation();
        if (wwtlib.WWTControl.singleton.tourEdit.get_tour().get_tourDirty() && !confirm('You have unsaved changes. Close this tour and lose changes?')) {
          return;
        }
        $rootScope.editingTour = false;
        delete $scope.ribbon.tabs[1].menu['Edit Tour'];
        delete $scope.ribbon.tabs[1].menu['Show Slide Overlays'];
        delete $scope.ribbon.tabs[1].menu['Show Slide Numbers'];

        wwtlib.WWTControl.singleton.stopCurrentTour();
        $rootScope.$broadcast('closeTour');
        //wwtlib.WWTControl.singleton.tour.cleanUp();
        $scope.$applyAsync(function () {
          $scope.activePanel = 'Guided Tours';
          $rootScope.editingTour = false;
          $rootScope.tourPlaying = false;
          $rootScope.currentTour = null;
        });
      }

      $scope.createNewTour = function () {
        $scope.$applyAsync(function () {
          //todo show dialog for tour properties
          $rootScope.currentTour = wwtlib.WWTControl.singleton.createTour("New Tour");

          $scope.activeItem = {label: 'currentTour'};
          $scope.activePanel = 'currentTour';
          $rootScope.$applyAsync(function () {
            $rootScope.editingTour = true;
            $rootScope.currentTour._editMode = true;
          });
        });
      };

      function tourChangeHandler() {
        $rootScope.$broadcast('tourFinished');
        var settings = appState.get('settings') || {};
        $scope.$applyAsync(function () {
          wwt.tourPlaying = $rootScope.tourPlaying = false;
        });
        $rootScope.landscapeMessage = false;

        ctl.clearAnnotations();
      }

      var shareModal = $modal({
        contentTemplate: 'views/popovers/shareplace.html',
        show: false,
        scope: $scope
      });
      var copyShortcut = function () {
        shareModal.$promise.then(shareModal.show);
      };
      $scope.restoreDefaultSettings = function () {
        $rootScope.$broadcast('restoreDefaults');
      };
      var showTips = function () {
        $('#introModal').modal('show');
      };
      //#endregion

      //#region localization

      $scope.selectedLanguage = 'EN';
      $scope.setLanguageCode = function (code) {
        appState.set('language', code);
        $timeout(function () {
          if ($scope.selectedLanguage !== code) {
            $scope.selectedLanguage = code;
            $scope.languageCode = code;
          }
        }, 200);
        $rootScope.languagePromise = loc.setLanguage(code);
      };

      //appState.set('language', 'EN');
      $scope.setLanguageCode(appState.get('language') || 'EN');
      $scope.locString = function (id) {
        var deferred = $q.defer();
        $rootScope.languagePromise.then(function () {
          localized[id] = loc.getString(id);
          deferred.resolve(localized[id]);
        });
        return deferred.promise;
      };

      var localized = [];
      var locCalls = 0;
      $scope.getFromEn = function (englishString) {
        locCalls++;
        if (locCalls % 100 == 0) {
          //util.log('loc calls: ' + locCalls);
        }
        var key = englishString + $scope.selectedLanguage;
        if ($scope.selectedLanguage === 'EN') {
          localized[key] = englishString;
        }
        if (localized[key]) {
          return localized[key];
        }

        var deferred = $q.defer();
        $rootScope.languagePromise.then(function () {
          //var key = englishString + $scope.selectedLanguage;
          if ($scope.selectedLanguage == 'EN') {
            localized[key] = englishString;
          }
          else {
            localized[key] = loc.getFromEn(englishString);
          }
          deferred.resolve(localized[key]);
        });
        return deferred.promise;
        //return null;
      };
      loc.getAvailableLanguages().then(function (result) {
        $scope.availableLanguages = result;
      });

      //static localizable strings that should be calculated once to prevent endless looping
      $rootScope.loc = {
        na: '',
        neverRises: ''
      };


      $rootScope.languagePromise.then(function (result) {
        $rootScope.na = loc.getFromEn('n/a');
        $rootScope.neverRises = loc.getFromEn('Never Rises');
        $scope.hideIntroModal = appState.get('hideIntroModalv2');
        if (!$scope.hideIntroModal && !$scope.loadingUrlPlace) {
          if (localStorage.getItem('login')) {
            var now = new Date().valueOf();
            var loginTime = parseInt(localStorage.getItem('login'));
            if (now - loginTime < 33333) {
              return;//no autoshow popup when logged in within last 30sec
            }
          }
          setTimeout(showTips, 1200);
        }
      });
      //#endregion

      //#region view helpers

      $scope.formatHms = function (angle, isHmsFormat, signed, spaced) {
        return util.formatHms(angle, isHmsFormat, signed, spaced);
      };
      $scope.formatDecimalHours = function (dayFraction, spaced) {
        var split = wwtlib.UiTools.formatDecimalHours(dayFraction).split(':');
        if (parseInt(split[0]) < 10) split[0] = '0' + split[0];
        if (parseInt(split[1]) < 10) split[1] = '0' + split[1];
        return split.join(' : ');
        //return util.formatDecimalHours(dayFraction, spaced == undefined ? true : spaced);test
      }

      $rootScope.showTrackingString = function () {
        return ($scope.trackingObj && $(window).width() > 1159);
      }

      $rootScope.showCrossfader = function () {
        var show = false;
        if ($scope.activePanel === 'ADS') {
          return true;
        }
        try {
          if ($scope.lookAt === 'Sky' && $scope.trackingObj && (util.getImageset($scope.trackingObj) != null)) {
            if ($(window).width() > 800 || util.isMobile) {
              show = true;
            }
          }
        } catch (er) {
          show = false;
        }
        return show;
      };

      $scope.hideIntroModalChange = function (hideIntroModal) {
        appState.set('hideIntroModalv2', hideIntroModal);
      };
      $scope.iswebclientHome = $cookies.get('homepage') !== 'home';
      $scope.homePrefChange = function (isWebclient) {
        $cookies.remove('homepage');
        if (!isWebclient) {
          $cookies.put('homepage', 'home', {expires: new Date(2050, 1, 1), path: "/"});
        } else {
          $cookies.put('homepage', 'webclient', {expires: new Date(2050, 1, 1), path: "/"});
        }
      };

      $scope.setMenuContextItem = function (item, isExploreTab) {
        $scope.menuContext = item;
        $scope.propertyItem = item;
        $scope.propertyItem.isExploreTab = isExploreTab;
      };

      $scope.showProperties = function () {
        $('.popover-content .close-btn').click();
        $('.dropdown.open #researchMenu, .dropup.open #researchMenu').closest('.thumbwrap').find('.thumb-popover').click();
      };

      $scope.setTrackingObj = function (item) {
        $scope.trackingObj = item;
        if ($scope.trackingObj === null) {
          hashManager.removeHashVal('place', true);
        }
      };
      $scope.showMobileTracking = function () {
        return $scope.trackingObj &&
          $scope.trackingObj.get_name &&
          !$scope.tourPlaying &&
          $scope.lookAt !== 'Earth' &&
          $scope.lookAt !== 'Planet' &&
          $scope.lookAt !== 'Panorama';
      };

      $scope.displayXFader = function () {
        return (
          $scope.lookAt === 'Sky' &&
          $scope.trackingObj &&
          !$scope.tourPlaying &&
          ($scope.trackingObj.get_backgroundImageset() != null || $scope.trackingObj.get_studyImageset() != null));
      }

      $scope.gotoConstellation = function (c) {
        $rootScope.singleton.gotoTarget(wwtlib.Constellations.constellationCentroids[c], false, false, true);
      }

      $scope.drawCircleOverPlace = function (place) {
        util.drawCircleOverPlace(place);
      }
      $scope.clearAnnotations = function () {
        ctl.clearAnnotations();
      };

      $scope.topExpanded = false;
      $scope.expandTop = function (flag, panel) {
        $scope.topExpanded = flag;
        $scope.expandedPanel = panel;
      }

      $scope.tourFeatures = function () {
        $scope.loadingTour = true;
        setTimeout(function () {
          $('#introStartButton').click();
        }, 3);
        setTimeout(function () {
          $('#btnCloseIntro').click();
          $scope.loadingTour = false;
        }, 1000);
      };

      initCanvas();

      $scope.constellations = wwtlib.Constellations;

      $scope.nbo = [];
      $scope.setNBO = function (nbo) {
        $scope.nbo = nbo;
        $scope.nboCount = nbo.length;
        if ($scope.isLoading) {
          $scope.isLoading = false;
          //util.log(new Date().valueOf() - time.valueOf());
        }

      }
      $scope.hideMenu = function () {
        $('.navbar-collapse.in').removeClass('in').addClass('collapse');
      }
      $scope.showNbo = function () {
        $('#nboModalLink').click();
        $scope.hideMenu();
      }
      $scope.isLoading = true;
      //var time = new Date();
      $scope.fovClass = function () {
        return $scope.lookAt === 'Planet' || $scope.lookAt === 'Panorama' || $scope.lookAt === 'Earth' ? 'hide' :
          $scope.lookAt === 'SolarSystem' ? 'solar-system-mode fov-panel' :
            'fov-panel';
      }
      $scope.contextPanelClass = function () {
        var cls = $scope.lookAt === 'Planet' || $scope.lookAt === 'Panorama' || $scope.lookAt === 'Earth' ? 'context-panel compressed' : 'context-panel';
        if ($rootScope.tourPlaying) {
          cls += ' hide';
        }
        $rootScope.compressed = $scope.compressed = cls.indexOf('compressed')>0;
        return cls;
      }
      $scope.contextPagerRight = function () {
        return /*$scope.fovClass() != 'hide' && */ $scope.showTrackingString() ? 0 : 50;
      }
      if (util.getQSParam('editTour')) {
        $scope.playTour(decodeURIComponent(util.getQSParam('editTour')));
        $scope.autoEdit = true;
      }
      else if (appState.get('editTourOnLogin') && !util.getQSParam('code')) {
        $scope.playTour(appState.get('editTourOnLogin'));
        appState.set('editTourOnLogin', false);
        $scope.autoEdit = true;
      }
      if (util.getQSParam('playTour')) {
        $scope.playTour(decodeURIComponent(util.getQSParam('editTour')));

      }
      $scope.mobileLink = util.mobileLink();
    }
  ]);
