wwt.controllers.controller('ThumbnailController',
	['$scope',
		'$rootScope',
	'AppState',
	'Places',
	'$timeout',
	'Util',
	'SearchUtil',
	'HashManager',
	function ($scope, $rootScope, appState, places, $timeout, util, searchUtil, hashManager) {
		var exploreRoot;
		var depth = 1;
		var bc;
		var cache = [];
		$scope.pageCount = 1;
		$scope.pageSize = 1;
		$scope.currentPage = 0;
		var explore,
			nearby,
			search,
			openCollection,
			collectionPlace,
			collectionPlaceIndex,
				hashObj;

		$scope.initExploreView = function (hashChange) {
			if (!explore) {
				$rootScope.$on('hashChange', function (event, obj) {
					if (obj['place'] && isNaN(parseInt(obj['place'].charAt(0)))) {
						hashObj = obj;
						collectionPlace = obj['place'];
						collectionPlaceIndex = 1;
						$scope.loadingUrlPlace = true;
						$('#loadingModal').modal('show');
						places.getRoot().then(function(result) {
							//$scope.initExploreView(true);
							$scope.breadCrumb = bc = [$scope.getFromEn('Collections')];
							$scope.exploreList = exploreRoot;
							findCollectionChild();
						}); 
					}
				});
				explore = true;
			}

			if (!hashChange) {
				places.getRoot().then(function(result) {
					$scope.exploreList = exploreRoot = result;

					var collectionsString = $scope.getFromEn('Collections');
					if (collectionsString.then) {
						collectionsString.then(function(s) {
							$scope.breadCrumb = bc = [s];
						});
					} else {
						$scope.breadCrumb = bc = [collectionsString];
					}

					cache = [result];
					calcPageSize($scope.exploreList);
					$.each(result, function(i, item) {
						if (item.get_name() === 'Open Collections') {
							result.splice(0, 0, item);
							result.pop();
							openCollection = true;
							$scope.clickThumb(item);
						}
					});
					
				});
			}
			
		};

		var findCollectionChild = function() {
			var guidParts = collectionPlace.split('.');
			var relevantPart = guidParts.slice(0, collectionPlaceIndex).join('.');
			var child = places.findChildById(relevantPart, $scope.exploreList);
			if (collectionPlaceIndex < guidParts.length) {
				collectionPlaceIndex++;
				$scope.clickThumb(child, findCollectionChild);
			} else {
				$timeout(function () {
					$scope.loadingUrlPlace = false;
					$scope.clickThumb(child);
					$('#loadingModal').modal('hide');
					
					if (hashObj['ra']) {
						var timer = hashObj['place'].toLowerCase().indexOf('hirise') != -1 ? 6666 : 3333;
						setTimeout(function() {
							$rootScope.ctl.gotoRaDecZoom(
								parseFloat(hashObj['ra']) * 15,
								parseFloat(hashObj['dec']),
								parseFloat(hashObj['fov']),
								false
							);
							
						}, timer);
					}
					location.hash = '/';
				}, 2222);
			}
		}
		

		var handledInitBroadcast = false,
			newCollectionUrl;
		$scope.$on('initExplorer', function (event,collectionUrl) {
			if (!handledInitBroadcast && explore) {
				newCollectionUrl = collectionUrl;
				$scope.initExploreView();
				handledInitBroadcast = true;
				setTimeout(function() { handledInitBroadcast = false; }, 2000);
			}
		});

		$scope.clickThumb = function (item, folderCallback) {
			
			wwt.wc.clearAnnotations();
			$scope.activeItem = item.get_thumbnailUrl() + item.get_name();
			if (item.get_name() === 'Up Level') {
				$scope.currentPage = 0;
				depth--;
				bc.pop();
				$scope.breadCrumb = bc;//.join(' > ') + ' >';
				cache.pop();
				$scope.exploreList = cache[cache.length - 1];
				calcPageSize($scope.exploreList);
				return;
			}
			if (item.get_isFolder()) {
				$scope.currentPage = 0;
				depth++;
				bc.push(item.get_name());
				$scope.breadCrumb = bc;//.join(' > ') + ' >';
				
				places.getChildren(item).then(function (result) {
					if ($.isArray(result[0]))
						result = result[0];
					var unique = [];
					$.each(result, function (index, el) {
						if ($.inArray(el, unique) === -1) unique.push(el);
					});
					$scope.exploreList = unique;
					
					cache.push(result);
					if (openCollection) {
						if (newCollectionUrl) {
							var i = 0;
							while (result[i].url && result[i].url.indexOf(newCollectionUrl) == -1) i++;

							$scope.clickThumb(result[i]);
							newCollectionUrl = null;
						} else {
							$scope.clickThumb(result[0]);
						}
					}
					calcPageSize($scope.exploreList);
					if (folderCallback) {
						folderCallback();
					}
				});
			}else if (openCollection) {
				openCollection = false;
			} else if ($scope.$hide) {
				$scope.$hide();
				$rootScope.searchModal = false;
				
			} else if (util.isMobile) {
				$('#explorerModal').modal('hide');
			}
			//$('.cross-fader').parent().hide();

			$scope.setActiveItem(item);
			

			if ((item.isFGImage && item.imageSet && $scope.lookAt !== 'Sky') || item.isSurvey){
				$scope.setLookAt('Sky', item.get_name(), true, item.isSurvey);
				if (item.isSurvey) {
					$scope.setSurveyBg(item.get_name());

				} else {
					$scope.setForegroundImage(item);
				}
				if ($scope.$hide) {
					$scope.$hide();
					$rootScope.searchModal = false;
				}
				return;
			}
			else if (item.isPanorama) {
				$scope.setLookAt('Panorama', item.get_name());
			} else if (item.isEarth) {
				$scope.setLookAt('Earth', item.get_name());
			} else if (util.getIsPlanet(item) && $scope.lookAt !== 'SolarSystem') {
				 $scope.setLookAt('Planet', item.get_name());
			} else if (item.isPlanet && $scope.lookAt !== 'SolarSystem') {
				$scope.setLookAt('Planet', '');
			} 
			if ((Type.canCast(item, wwtlib.Place)||item.isEarth) && !item.isSurvey) {
				$scope.setForegroundImage(item);
				
			}
			
		};

		$scope.expanded = false;
		$scope.expandThumbnails = function(flag) {
			$scope.expanded = flag != undefined ? flag : !$scope.expanded;
			$scope.expandTop($scope.expanded);
			calcPageSize($scope.exploreList); 
		}

		$scope.hoverThumb = function(item) {
			if (nearby) {
				$scope.drawCircleOverPlace(item);
			}
		};

		$scope.breadCrumbClick = function(index) {
			$scope.exploreList = cache[index];
			while (bc.length - 1 > index) {
				bc.pop(); 
				cache.pop();
			}
			$scope.currentPage = 0;
			calcPageSize();
		};


		var pagedList;
		var calcPageSize = function (list) {
			if (list) {
				pagedList = list;
			} else {
				list = pagedList;
			}
			$timeout(function() {
				var tnWid = 116;
				var winWid = $(window).width();
				
				if (nearby && ($scope.lookAt == 'Sky' || $scope.lookAt == 'SolarSystem')) {
					winWid = winWid - 216; //angular.element('body.desktop .fov-panel').width();
				}
				$scope.pageSize = util.isMobile?9999:Math.floor(winWid / tnWid);

				if ($scope.expanded) {
					$scope.pageSize *= 4;
				}
				var listLength = list ? list.length : 2;
				$scope.pageCount = Math.ceil(listLength / $scope.pageSize);
				spliceOnePage();
			},1);
			
		};

		$(window).on('resize', function () {
			$scope.currentPage = 0;
			var offset = nearby ? angular.element('body.desktop .fov-panel').width() : 0;
			calcPageSize(null, offset);
			//util.log('calc',offset);
		});

		$scope.moveMenu = function (i) {
			$('#menuContainer' + i).append($('#researchMenu'));
		};
		$scope.moveNboMenu = function (i) {
			$('#nboMenuContainer' + i).append($('#researchMenu'));
		};
		$scope.showMenu = function (item, i) {
			$('.popover-content .close-btn').click();
			setTimeout(function() {
				$('.popover-content .close-btn').click();
				$('#menuContainer' + i).find('.dropdown').addClass('open');
			}, 10);
			$scope.setMenuContextItem(item,true);
		};
		$scope.preventClickBubble = function(event) {
			event.stopImmediatePropagation();
		};

		$scope.goBack = function () {
			//$(document.body).append($('#researchMenu'));
			$scope.currentPage = $scope.currentPage == 0 ? $scope.currentPage : $scope.currentPage - 1;
			spliceOnePage();
		};
		$scope.goFwd = function () {
			//$(document.body).append($('#researchMenu'));
			$scope.currentPage = $scope.currentPage == $scope.pageCount - 1 ? $scope.currentPage : $scope.currentPage + 1;
			spliceOnePage();
		};

		var spliceOnePage = function () {
			if (nearby || search) {
				var start = $scope.currentPage * $scope.pageSize;
				var masterCollection = nearby ? $scope.placesInCone : $scope.searchResults;
				if (masterCollection) {
					if (nearby) {

						$scope.nearbyPlaces = masterCollection.slice(start, start + $scope.pageSize);
					} else {
						$scope.searchResultSet = masterCollection.slice(start, start + $scope.pageSize);
					}
				}
			}
		}

		var lastUpdate = new Date();
		$scope.initNearbyObjects = function() {
			nearby = true;
			$scope.placesInCone = [];
			$scope.scrollDepth = 40;
			$rootScope.$on('viewportchange', function (event, viewport) {
				if ((!viewport.isDirty && !viewport.init) || new Date().valueOf() - lastUpdate.valueOf() > 2000) {
					findNearbyObjects();
					lastUpdate = new Date();
				}
			});
			
			$scope.$watch('lookAt', findNearbyObjects);
			
		};
		
		function findNearbyObjects() {
			searchUtil.findNearbyObjects({
				lookAt: $scope.lookAt,
				singleton: $rootScope.singleton
			}).then(function(result) {
				$scope.currentPage = 0;
				$scope.placesInCone = result;
				if (util.isMobile) {
					$scope.setNBO($scope.placesInCone);
				}
				calcPageSize($scope.placesInCone);
			});
		}

		
		$scope.gotoCoord = function () {
			var tempPlace = wwtlib.Place.create('tmp', util.parseHms($scope.goto.Dec), util.parseHms($scope.goto.RA), null, null, wwtlib.ImageSetType[$scope.lookAt.toLowerCase()], 60);
			$rootScope.singleton.gotoTarget(tempPlace, false, false, true);
		};

		
		$scope.initSearch = function () {
			$scope.goto = {RA:'',Dec:''};
			if (util.isMobile) {
				$scope.scrollDepth = 40;
			}
			
			search = true;
			$timeout(function() {
				$scope.SearchType = 'J2000';
				$('#txtSearch').focus();
			},123);
		}

		$scope.searchKeyPress = function () {
			$timeout(function() {
				var q = $scope.q = $('#txtSearch').val();
				searchUtil.runSearch(q).then(function(result) {
					$scope.searchResults = result;
					calcPageSize($scope.searchResults);
				});
			}, 10);
		}
	}]);

