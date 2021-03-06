﻿wwt.app.factory(
  'AutohidePanels',
  [
    '$rootScope',
    'AppState',
    'Util',

    function ($rootScope, appState, util) {
      var api = {
        init:init
      };

      var tourPlaying = false,
          editingTour = false,
          mouseInRegion = { tabs: false, context: false },
          panelHidden = { tabs: false, context: false },
          panels = { tabs: null, context: null },
          hideTimers = { tabs: null, context: null },
          showingPanels = { tabs: false, context: false },
          autoHide = { tabs: false, context: false },
          autoHideHover = { tabs: false, context: false },
          autoHideClick = { tabs: false, context: false },
          hideTimeout = 1200;

      function init() {
        if ($rootScope.is_mobile) {
          return;
        }

        panels = {
          tabs: $('#topPanel, .layer-manager'),
          context: $('.context-panel')
        };

        if (panels.tabs.length < 2) {
          setTimeout(init, 200);
          console.log('init', panels.tabs);
          return;
        }

        bindEvents();
        settingChange();

        // This is a hack to make it so that the other settings defined by the
        // SettingsController (beyond autohide settings) are applied on
        // startup. The SettingsController can't do that itself since it's
        // only initialized when the user navigates to the Settings tab in the
        // UI. The real solution is to turn the settings into something like a
        // Factory rather than a Controller, since they should have their own
        // existence separate from any UI elements, but that's a bit bigger
        // undertaking and there are only a couple of items. Inspired by
        // GitHub issue #287.

        var settings = appState.get('settings');

        if (settings) {
          if (settings.crosshairs !== undefined) {
            wwt.wc.settings.set_showCrosshairs(settings.crosshairs);
          }

          if (settings.smoothPanning !== undefined) {
            wwt.wc.settings.set_smoothPan(settings.smoothPanning);
          }
        }
      };

      var bindEvents = function () {
        $rootScope.$on('autohideChange', settingChange);
        $rootScope.$watch('editingTour', tourStateChange);
        $rootScope.$watch('tourPlaying', function () {
          if ($rootScope.tourPlaying) {
            panels.tabs = $('#ribbon, #topPanel, .layer-manager');
          }
          tourStateChange();
        });

        $(window).on('mousedown', function () {
          $.each(['tabs', 'context'], function (i,groupKey) {
            if (tourPlaying && !editingTour && mouseInRegion[groupKey] && panelHidden[groupKey] && !showingPanels[groupKey]) {
              regionClicked(groupKey);
            }
          });
        });

        $(window).on('mousemove touchstart touchmove touchend', function (event) {
          var y = event.pageY != undefined ? event.pageY : event.originalEvent.targetTouches[0].pageY;

          var inBottom = $(window).height() - 123 < y;
          if (inBottom !== mouseInRegion.context) {
            mouseInRegion.context = inBottom;
            cursorRegionChange();
          }

          var inTop = y < 142;
          if (inTop !== mouseInRegion.tabs) {
            mouseInRegion.tabs = inTop;
            cursorRegionChange();
          }
        });
      }

      var setBoth = function (o, ref) {
        if (typeof ref == 'object') {
          o.tabs = ref.tabs;
          o.context = ref.context;
        } else {
          o.tabs = ref;
          o.context = ref;
        }
      };

      var settingChange = function () {
        var settings = appState.get('settings');

        if (!settings || settings.autoHideTabs === undefined) {
          setBoth(autoHide, false);
        } else {
          if (settings.autoHideTabs !== autoHide.tabs && settings.autoHideTabs == false) {
            togglePanelGroup(true, 'tabs');
          }
          if (settings.autoHideContext !== autoHide.context && settings.autoHideContext == false) {
            togglePanelGroup(true, 'context');
          }
          autoHide.tabs = settings.autoHideTabs;
          autoHide.context = settings.autoHideContext;
        }

        if (tourPlaying) {
          panels.tabs = $('#ribbon, #topPanel, .layer-manager');

          setBoth(autoHideHover, true);
          setBoth(autoHideClick, true);
          hideTimeout = 100;
        } else {
          $('#ribbon').fadeIn();
          setBoth(autoHideClick, false);
          setBoth(autoHideHover, autoHide);
          panels.tabs = $('#topPanel, .layer-manager');
          hideTimeout = 1200;
        }

        cursorRegionChange();
      };

      var tourStateChange = function () {
        togglePanelGroup(!$rootScope.tourPlaying, 'tabs');
        togglePanelGroup(!$rootScope.tourPlaying, 'context');
        editingTour = $rootScope.editingTour;
        tourPlaying = $rootScope.tourPlaying;
        settingChange();
        cursorRegionChange();
      };

      var togglePanelGroup = function (show, groupKey) {
        var panelGroup = panels[groupKey];

        if (show) {
          clearTimeout(hideTimers[groupKey]);

          if (panelHidden[groupKey] && !showingPanels[groupKey]) {
            panelHidden[groupKey] = false;
            showingPanels[groupKey] = true;
            panelGroup.fadeIn(800, function () { showingPanels[groupKey] = false; });

            if (tourPlaying && mouseInRegion.tabs) {
              console.log('showingSlides');
              $rootScope.$broadcast('showingSlides');
            }
          }
        } else {
          clearTimeout(hideTimers[groupKey]);
          hideTimers[groupKey] = setTimeout(function () {
            panelHidden[groupKey] = true;
            panelGroup.fadeOut(800);
          }, hideTimeout);
        }
      }

      var regionClicked = function (key) {
        console.log('regionClick', { autoHide: autoHide, autoHideClick: autoHideClick, tabs: panels.tabs });
        togglePanelGroup(true, key);
        if (key === 'tabs') {
          $rootScope.$broadcast('showingSlides');
        }
      }

      var cursorRegionChange = function () {
        if (tourPlaying && !editingTour) {
          return;
        }

        $.each(['tabs', 'context'], function (i,groupKey) {
          if (autoHideHover[groupKey]) {
            if (mouseInRegion[groupKey]) {
              togglePanelGroup(true, groupKey);
            } else if (!panelHidden[groupKey]) {
              togglePanelGroup(false, groupKey);
            }
          }
        });
      };

      return api;
    }
  ]
);
