wwt.controllers.controller('LayerManagerController',
	['$scope',
	'AppState',
	'$timeout',
	'Util',
	function($scope, appState, $timeout,util) {
		var version = 4;

		function treeNode(args) {
			this.name = args.name;
			this.checked = args.checked === undefined ? true : args.checked;
			this.children = args.children || [];
			this.action = args.action;
			this.collapsed = args.collapsed || false;
			if (args.v) this.v = args.v;
		}

		var constellations = [];
		$scope.initLayerManager = function() {
			if (!wwtlib.Constellations.abbreviations) {
				setTimeout($scope.initLayerManager, 333);
				return;
			}
			$.each(wwtlib.Constellations.abbreviations, function(name, abbrev) {
				constellations.push(new treeNode({
					name: name
				}));
			});
			$scope.tree = appState.get('layerManager');
			if (!$scope.tree || !$scope.tree.v || $scope.tree.v !== version) { //dump appState version when change is made
				$scope.tree = initTree();

				appState.set('layerManager', $scope.tree);
			}
			$timeout(function() { initTreeNode(0, $scope.tree.children[0]); });
			wwt.resize();
		}

		//var filterNode = new treeNode({
		//    name: $scope.getFromEn('Filter'),
		//    children: constellations,
		//    collapsed:true
		//});
		//var picturesFilter = wwt.clone(filterNode);
		//picturesFilter.action = 'constellationArtFilter';
		//var boundriesFilter = wwt.clone(filterNode);
		//boundriesFilter.action = 'constellationBoundariesFilter';
		//var figuresFilter = wwt.clone(filterNode);
		//figuresFilter.action = 'constellationFiguresFilter';

		//function setParentRef(parentNode) {
		//    $.each(parentNode.children, function(i,item) {
		//        item.parentRef = parentNode;
		//    });
		//}

		//setParentRef(picturesFilter);
		//setParentRef(boundriesFilter);
		//setParentRef(figuresFilter);

		//console.log(filterNode);


		var initTree = function() {
			return new treeNode({
				v: version,
				name: $scope.getFromEn('Sky'),
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
											new treeNode({ name: $scope.getFromEn('Focused Only'), action: 'showConstellationSelection' })
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
						children: [/*
							new treeNode({
								name: $scope.getFromEn('Cosmic Microwave Background'),
								checked: false,
								action: 'solarSystemCMB'
							}), new treeNode({
								name: $scope.getFromEn('Cosmos (SDSS Galaxies)'),
								checked: true,
								action: 'solarSystemCosmos'
							}),*/ new treeNode({
								name: $scope.getFromEn('Milky Way (Dr. R. Hurt)'),
								checked: true,
								action: 'solarSystemMilkyWay'
							})/*, new treeNode({
								name: $scope.getFromEn('Stars (Hipparcos, ESA)'),
								checked: true,
								action: 'solarSystemStars'
							})*/, new treeNode({
								name: $scope.getFromEn('Planets (NASA, ETAL)'),
								checked: true,
								action: 'solarSystemPlanets'
							}), new treeNode({
								name: $scope.getFromEn('Planetary Orbits'),
								checked: true,
								action: 'solarSystemOrbits'
							})/*, new treeNode({
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
							}) /*, new treeNode({
							name: $scope.getFromEn('Multi-Res Solar System Bodies'),
							checked: true,
							action: 'solarSystemMultiRes'
						})*/
						]
					})
				]
			});
		}


		$scope.nodeChange = function(node) {
			appState.set('layerManager', $scope.tree);
			invokeSetting(node);
		};

		var invokeSetting = function(node) {
			if (node.action) {
				try {
					//var bool = node.checked ? 'true' : 'false';
					//eval('wwt.wc.settings.set_' + node.action + '(' + bool + ')');
					wwt.wc.settings['set_' + node.action](node.checked);
				} catch (er) {
					util.log(er, node.action);
				}
			}
		}

		function initTreeNode(i, node) {
			$.each(node.children, initTreeNode);
			invokeSetting(node);
		}
	}]
);