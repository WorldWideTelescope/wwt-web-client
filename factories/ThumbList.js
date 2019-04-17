// Includes shared functions for ExploreController, SearchController, and
// ContextPanelController - handles thumbnail click, right-click, and paging
// behavior

// ToursController does not use this factory

wwt.app.factory('ThumbList', ['$rootScope', 'Util', 'Places', '$timeout', '$templateRequest', '$compile',
  function ($rootScope, util, places, $timeout, $templateRequest, $compile) {
    var api = {
      init: init,
      clickThumb: clickThumb,
      calcPageSize: calcPageSize,
      spliceOnePage: spliceOnePage,
      goFwd: goFwd,
      goBack: goBack
    };

    // Each controller calls init and passes in the controller
    // scope
    function init(scope, name) {
      scope.pageCount = 1;
      scope.pageSize = 1;
      scope.currentPage = 0;

      scope.preventClickBubble = function (event) {
        event.stopImmediatePropagation();
      };
      scope.goBack = function () {
        goBack(scope);
      };
      scope.goFwd = function () {
        goFwd(scope);
      };
      scope.showMenu = function (i) {
        if (!$('#researchMenu').length) {
          return $templateRequest('views/research-menu.html').then(function (tplContent) {
            var template = $compile(tplContent)(scope);
            $('body').append(template);
            scope.showMenu(i);
          });
        }

        var item = scope.collectionPage[i];
        item.contextMenuEvent = true;
        $('.popover-content .close-btn').click();
        if (!item.get_isFolder() && item.get_name() !== 'Up Level') {
          var menuContainer = $((name === 'context' ? '.nearby-objects ' : '.top-panel ') + '#menuContainer' + i);
          if (util.isMobile) {
            menuContainer = $('#' + name + 'Container #menuContainer' + i);
          }
          menuContainer.append($('#researchMenu'));
          setTimeout(function () {
            $('.popover-content .close-btn').click();
            menuContainer.find('#researchMenu')
              .addClass('open')
              .off('click')
              .on('click', function (event) {
                event.stopPropagation();
              });
            menuContainer.find('.drop-toggle').click();
            $timeout(function () {
              if (!util.isMobile) {
                $('.dropdown-backdrop').off('contextmenu');
                $('.dropdown-backdrop').on('contextmenu', function (event) {
                  $(this).click();
                  event.preventDefault();
                });
              }
              scope.setMenuContextItem(item, true);
              item.contextMenuEvent = false;
            }, 10);

          }, 10);
        }
      };

      // toggles the expanded thumbnail view to show 1 or 5 rows of thumbs
      scope.expandThumbnails = function (flag) {
        $('body').append($('#researchMenu'));
        scope.currentPage = 0;
        scope.expanded = flag != undefined ? flag : !scope.expanded;
        scope.expandTop(scope.expanded, name);
        calcPageSize(scope, name === 'context');
      };
      scope.dropdownClass = name === 'context' && !util.isMobile ? 'dropup menu-container' : 'dropdown menu-container';
      scope.popupPosition = name === 'context' && !util.isMobile ? 'top' : 'bottom';
    }

    function clickThumb(item, scope, outParams, callback) {

      setTimeout(function(){
        $rootScope.instant = false;
      },2222);
      if (!item){
        return console.warn(item);
      }
      if (item.contextMenuEvent) {
        return outParams;
      }
      if (!outParams) {
        outParams = {};
      }
      scope.activeItem = item.get_thumbnailUrl() + item.get_name();
      scope.setActiveItem(item);
      //wwt.wc.clearAnnotations();
      if (item.get_name() === 'Up Level') {
        $('body').append($('#researchMenu'));
        scope.currentPage = 0;
        outParams.depth--;
        outParams.breadCrumb.pop();
        scope.breadCrumb = outParams.breadCrumb;
        outParams.cache.pop();
        scope.collection = outParams.cache[outParams.cache.length - 1];
        calcPageSize(scope, false);
        return outParams;
      }
      if (item.get_url && item.get_url() && item.get_url().indexOf('?wwtfull') !== -1) {
        window.open(item.get_url());
        return outParams;
      }
      if (item.get_isFolder()) {
        $('#folderLoadingModal').modal('show');
        $('body').append($('#researchMenu'));
        scope.currentPage = 0;
        outParams.depth++;
        outParams.breadCrumb.push(item.get_name());
        scope.breadCrumb = outParams.breadCrumb;
        places.getChildren(item).then(function (result) {
          $('#folderLoadingModal').modal('hide');
          if ($.isArray(result[0])) {
            result = result[0];
          }
          var unique = [];
          $.each(result, function (index, el) {
            if ($.inArray(el, unique) === -1) unique.push(el);
          });
          scope.collection = unique;
          console.log({unique:unique});
          calcPageSize(scope, false);
          outParams.cache.push(result);
          if (outParams.openCollection) {
            if (outParams.newCollectionUrl) {
              var i = 0;
              while (result[i] && result[i].url && result[i].url.indexOf(outParams.newCollectionUrl) === -1) i++;

              scope.clickThumb(result[i] || result[i - 1]);
              outParams.newCollectionUrl = null;
            } else if (result.length) {
              scope.clickThumb(result[0]);
            }
          }

          if (callback) {
            callback();
          }
        });
        return outParams;
      } else if (outParams.openCollection) {
        outParams.openCollection = false;
      } else if (scope.$hide) {
        scope.$hide();
        $rootScope.searchModal = false;
      } else if (util.isMobile) {
        $('#explorerModal').modal('hide');
      }
      setTimeout(function(){
        $rootScope.$applyAsync(function(){
          $rootScope.setLookAt(null);
        })},111);
      if ((item.isFGImage && item.imageSet && scope.lookAt !== 'Sky') || item.isSurvey) {
        if (item.guid && item.guid.toLowerCase().indexOf('mars.') == -1) {
          scope.setLookAt('Sky', item.get_name(), true, item.isSurvey);
        }
        if (item.isSurvey) {
          scope.setSurveyBg(item.get_name(), item);
        } else {
          scope.setForegroundImage(item);
        }
        if (scope.$hide) {
          scope.$hide();
          $rootScope.searchModal = false;
        }
        return outParams;
      }
      else if (item.isPanorama) {
        scope.setLookAt('Panorama', item.get_name());
      } else if (item.isEarth) {
        scope.setLookAt('Earth', item.get_name());
      } else if (util.getIsPlanet(item)) {

        if (scope.lookAt === 'Sky') {
          //var c = item.get_camParams();
          //c.zoom = 0.1;

          wwtlib.WWTControl.singleton.gotoTarget3(item.get_camParams());
          return outParams;

        }
        if (scope.lookAt !== 'SolarSystem') {

          scope.setLookAt('Planet', item._name || '');
        }
      }
      if ((ss.canCast(item, wwtlib.Place) || item.isEarth) && !item.isSurvey) {
        scope.setForegroundImage(item);
      }
      if (ss.canCast(item, wwtlib.Tour)) {
        scope.playTour(item.get_tourUrl());
      }
      return outParams;

      /*var exUranus = {
        "_camParams": {
          "lat": -4.426167041642662,
          "lng": -351.77882519031607,
          "zoom": 0.1,
          "rotation": 0,
          "angle": 0,
          "raDec": true,
          "opacity": 100,
          "target": 65536,
          "viewTarget": {"x": 0, "y": 0, "z": 0},
          "targetReferenceFrame": ""
        },
        "_location3d": {"x": 0, "y": 0, "z": 0},
        "htmlDescription": "",
        "_constellation": "AQR",
        "_classification": 536870912,
        "_type": 2,
        "_magnitude": 0,
        "_distnace": 0,
        "angularSize": 0.1,
        "_thumbNail": null,
        "_studyImageset": null,
        "_backgroundImageSet": null,
        "_searchDistance": 0,
        "_elevation": 50,
        "_name": "Uranus",
        "guid": "Solar System (Sky).8",
        "thumb": "//cdn.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=uranus",
        "isPlanet": false,
        "isFGImage": true,
        "$$hashKey": "object:832",
        "imageSet": null
      }*/
    };

    function calcPageSize(scope, isContextPanel) {
      var list = scope.collection;
      var tnWid = 116;
      var winWid = $(window).width();

      if (isContextPanel && (scope.lookAt === 'Sky' || scope.lookAt === 'SolarSystem')) {
        winWid = winWid - 216; //angular.element('body.desktop .fov-panel').width();
      }
      scope.pageSize = util.isMobile ? 99999 : Math.floor(winWid / tnWid);

      if (scope.expanded) {
        scope.pageSize *= 5;
      }
      var listLength = list ? list.length : 2;
      $timeout(function () {
        scope.pageCount = Math.ceil(listLength / scope.pageSize);
        spliceOnePage(scope);
      }, 10);
    };

    function goBack(scope) {
      $('body').append($('#researchMenu'));
      scope.currentPage = scope.currentPage === 0 ? scope.currentPage : scope.currentPage - 1;
      return spliceOnePage(scope);
    };

    function goFwd(scope) {
      $('body').append($('#researchMenu'));
      scope.currentPage = scope.currentPage === scope.pageCount - 1 ? scope.currentPage : scope.currentPage + 1;
      return spliceOnePage(scope);
    };

    function spliceOnePage(scope) {
      if (scope.collection) {
        var start = scope.currentPage * scope.pageSize;
        scope.collectionPage = scope.collection.slice(start, start + scope.pageSize);
      }
    };

    return api;

  }]);

