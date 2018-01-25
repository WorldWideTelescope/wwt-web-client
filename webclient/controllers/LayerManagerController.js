wwt.controllers.controller('LayerManagerController',
  ['$scope',
    'AppState',
    '$timeout',
    'Util',
    function ($scope, appState, $timeout, util) {
    var version = 12;

      function treeNode(args) {
        this.name = args.name;
        this.checked = args.checked === undefined ? true : args.checked;
        this.children = args.children || [];
        this.action = args.action;
        this.collapsed = args.collapsed || false;
        this.disabled = false;
        if (args.mergeWith){
          //console.log({mergedFound:args})
          this.mergeWith = args.mergeWith;
        }
        if (args.v) this.v = args.v;
      }
      var allMaps = {};
      var constellations = [];

      $scope.initLayerManager = function () {
        if (!wwtlib.Constellations.abbreviations) {
          setTimeout($scope.initLayerManager, 333);
          return;
        }
        $.each(wwtlib.Constellations.abbreviations, function (name, abbrev) {
          constellations.push(new treeNode({
            name: name
          }));
        });
        $scope.tree = initTree();//appState.get('layerManager');

    $scope.scrubber = {left:' ',right:' ',table:[0,1,2,3,4,5]};

        $timeout(function () {

          wwt.wc.add_timeScrubberHook(function(prop,val){
            switch (prop){
              case 'left':
              case 'right':
              case 'title':
                $scope.$applyAsync(function() {
                  $scope.scrubber[prop] = val;
                });
                break;
              default:
                var el =  $('.scrubber-slider');
                el.find('.btn').css('left', val * el.width());
                break;
            }
          });

          initTreeNode(0, $scope.tree);
          $timeout(function () {
            var w = $('.scrubber-slider').width();
            var bar = $('.scrubber-slider a.btn');
            var scrubberMover = new wwt.Move({
              el:bar,
              bounds: {
                x: [0, w],
                y: [0, 0]
              },
              onmove:function(){
                wwt.wc.setTimeScrubberPosition(this.css.left/w);
              }
            });


            allMaps = wwtlib.LayerManager.get_allMaps();
            var sunTree = {Sun: (allMaps.Sun)};

            sunTree.Sun.collapsed = false;

            $.each(sunTree.Sun.childMaps, function (name, node) {
              node.collapsed = false;
              //node.name = name;
              $.each(node.childMaps, function (childName, child) {
                child.collapsed = true;
                //child.name = childName;
              });
            });
            $scope.sunTree = sunTree;
          }, 123);
        });
        wwt.resize();
      };
      $scope.getChildren = function(node){
        var children  = node.children || {};
        if (children.length){

          if (node.mergeWith){
            //console.log('mergeWith',node.mergeWith);
            allMaps = wwtlib.LayerManager.get_allMaps();
            if (allMaps && allMaps[node.mergeWith]) {
              //console.log(allMaps[node.mergeWith]);
              var addedChildren = $scope.getChildren(allMaps[node.mergeWith]);
              $.each(children, function (i, childNode) {
                addedChildren[childNode.name] = childNode;
              });
              return addedChildren;
            }
          }
          return children;
        }
        if (node.childMaps && Object.keys(node.childMaps).length){
          children = node.childMaps;
        }
        if (node.layers && node.layers.length){
          node.layers.forEach(function(l){
            children[l._name] = l;
          });
        }
          return children;
      };
      var initTree = function () {
        return new treeNode({
          v: version,
          name: $scope.getFromEn('Sky'),
          mergeWith: 'Sky',// key of node in wwtlib.LayerManager.get_allMaps() to merge into settings. Set this on any settings node
          action: 'showSkyNode',
          children: [
            new treeNode({
              name: $scope.getFromEn('Overlays'),
              action: 'showSkyOverlays',
              children: [
                new treeNode({
                  name: $scope.getFromEn('Constellations'),
                  action: 'constellationsEnabled',
                  children: [
                    new treeNode({
                      name: $scope.getFromEn('Constellation Pictures'),
                      //children: picturesFilter,
                      action: 'showConstellationPictures',
                      checked: false
                    }), new treeNode({
                      name: $scope.getFromEn('Constellation Figures'),
                      //children: figuresFilter,
                      action: 'showConstellationFigures'
                    }), new treeNode({
                      name: $scope.getFromEn('Constellation Boundaries'),
                      collapsed: true,
                      children: [
                        new treeNode({name: $scope.getFromEn('Focused Only'), action: 'showConstellationSelection'})
                      ],
                      action: 'showConstellationBoundries'
                    }), new treeNode({
                      name: $scope.getFromEn('Constellation Names'),
                      checked: false,
                      action: 'showConstellationLabels'
                    })
                  ]
                }),
                new treeNode({
                  name: $scope.getFromEn('Grids'),
                  action: 'showSkyGrids',
                  children: [
                    new treeNode({
                      name: $scope.getFromEn('Equatorial Grid'),
                      checked: false,
                      collapsed: true,
                      action: 'showGrid',
                      children: [
                        new treeNode({
                          name: $scope.getFromEn('Axis Labels'),
                          checked: true,
                          action: 'showEquatorialGridText'
                        })
                      ]
                    }), new treeNode({
                      name: $scope.getFromEn('Galactic Grid'),
                      checked: false,
                      collapsed: true,
                      action: 'showGalacticGrid',
                      children: [
                        new treeNode({
                          name: $scope.getFromEn('Axis Labels'),
                          checked: true,
                          action: 'showGalacticGridText'
                        })
                      ]
                    }), new treeNode({
                      name: $scope.getFromEn('AltAz Grid'),
                      checked: false,
                      collapsed: true,
                      action: 'showAltAzGrid',
                      children: [
                        new treeNode({
                          name: $scope.getFromEn('Axis Labels'),
                          checked: true,
                          action: 'showAltAzGridText'
                        })
                      ]
                    }), new treeNode({
                      name: $scope.getFromEn('Ecliptic Grid'),
                      checked: false,
                      collapsed: true,
                      children: [
                        new treeNode({
                          name: $scope.getFromEn('Axis Labels'),
                          checked: true,
                          action: 'showEclipticGridText'
                        })
                      ],
                      action: 'showEclipticGrid'
                    }), new treeNode({
                      name: $scope.getFromEn('Ecliptic Overview'),
                      checked: true,
                      collapsed: true,
                      action: 'showEcliptic',
                      children: [
                        new treeNode({
                          name: $scope.getFromEn('Month Labels'),
                          checked: false,
                          action: 'showEclipticOverviewText'
                        })
                      ]
                    }), new treeNode({
                      name: $scope.getFromEn('Precession Chart'),
                      checked: false,
                      action: 'showPrecessionChart'
                    })
                  ]
                })
              ]
            }),
            new treeNode({
              name: $scope.getFromEn('2d Sky'),
              checked: true,
              action: 'showSkyNode',
              children: [
                new treeNode({
                  name: $scope.getFromEn('Show Solar System'),
                  checked: true,
                  action: 'showSolarSystem'
                }) /*,
						new treeNode({
							name: $scope.getFromEn('Field of View Indicators'),
							checked: true,
							action: 'showFieldOfView'
						})*/
              ]
            }),
            new treeNode({
              name: $scope.getFromEn('3d Solar System'),
              checked: true,
              action: 'showSolarSystem',
              children: [
                new treeNode({
                  name: $scope.getFromEn('Milky Way (Dr. R. Hurt)'),
                  checked: true,
                  action: 'solarSystemMilkyWay'
                }), new treeNode({
                  name: $scope.getFromEn('Stars (Hipparcos, ESA)'),
                  checked: true,
                  action: 'solarSystemStars'
                }), new treeNode({
                  name: $scope.getFromEn('Planets (NASA, ETAL)'),
                  checked: true,
                  action: 'solarSystemPlanets'
                }), new treeNode({
                  name: $scope.getFromEn('Planetary Orbits'),
                  checked: true,
                  action: 'solarSystemOrbits'
                }), new treeNode({
                  name: $scope.getFromEn('Moon & Satellite Orbits'),
                  checked: false,
                  action: 'solarSystemMinorOrbits'
                }), new treeNode({
                  name: $scope.getFromEn('Asteroids (IAU MPC)'),
                  checked: false,
                  action: 'solarSystemMinorPlanets'
                }), new treeNode({
                  name: $scope.getFromEn('Lighting and Shadows'),
                  checked: true,
                  action: 'solarSystemLighting'
                }), new treeNode({
                  name: $scope.getFromEn('Multi-Res Solar System Bodies'),
                  checked: true,
                  action: 'solarSystemMultiRes'
                })
              ]
            })
          ]
        });
      };

      $scope.showMenu = function (layerMap, event) {
        if ($scope.activeLayer) {
          $scope.activeLayer.active = false;
        }
        $scope.$applyAsync(function () {
          layerMap.active = true;
          $scope.activeLayer = layerMap;
        });

        //ensure selection is changed first.
        wwtlib.LayerManager.layerSelectionChanged(layerMap);


        //console.log('invoke context menu on node', event, layerMap);
        wwtlib.LayerManager.showLayerMenu(layerMap, event.pageX, event.pageY);
      }

      $scope.selectionChanged = function (layerMap, event) {
        if (event.currentTarget.tagName==='LABEL' && event.offsetX > 17){
          //console.log(event);
          event.preventDefault();
          event.stopImmediatePropagation();

          if ($scope.activeLayer) {
            $scope.activeLayer.active = false;
          }
          $scope.$applyAsync(function () {
            if (layerMap && layerMap.action) {
              wwtlib.LayerManager.layerSelectionChanged(layerMap);
              layerMap.active = true;
            }
            $scope.activeLayer = layerMap;
          });
        }
      };

      $scope.hasChildren = function (node) {
        var children = $scope.getChildren(node);
        return children.length || Object.keys(children).length > 0;
      };

      $scope.isObjectNode = function(node){
        return node.action == undefined;
      }

      $scope.nodeChange = function (node) {
        //appState.set('layerManager', $scope.tree);
        invokeSetting(node);
      };

      $scope.collapsed = function(node){
        return $scope.isObjectNode(node) ? !node.open : node.collapsed;
      };
      $scope.collapse = function(node){
        var key = 'collapsed';
        var collapse = !$scope.collapsed(node);
        if ($scope.isObjectNode(node)){
          key = 'open';
          collapse = !collapse;
        }
        node[key] = collapse;
      };


      var invokeSetting = function (node) {
        if (!node.disabled && node.action &&
          wwt.wc.settings['set_' + node.action]) {
          var settingFlag = node.checked && !node.disabled;
          wwt.wc.settings['set_' + node.action](settingFlag);
        }
        setChildState(node);
      };

      // enable/disable all child settings based on parent
      var setChildState = function (node) {
        if (node.children) {
          $.each(node.children, function (i, child) {
            child.disabled = !node.checked || node.disabled;
            if (child.action && wwt.wc.settings['set_' + child.action]) {
              var settingFlag = child.checked && !child.disabled;
              wwt.wc.settings['set_' + child.action](settingFlag);
            }
            setChildState(child);
          });
        }
      };

      function initTreeNode(i, node) {
        $.each(node.children, initTreeNode);
        invokeSetting(node);
      }
      wwt.detectNewLayers = function(){
        $scope.$applyAsync(function(){
          //console.log('detecting new layers',wwtlib.LayerManager.get_allMaps(),{newMaps:wwtlib.LayerManager.get_allMaps() !== allMaps});
          allMaps = wwtlib.LayerManager.get_allMaps();
        })
      }
    }]
);
