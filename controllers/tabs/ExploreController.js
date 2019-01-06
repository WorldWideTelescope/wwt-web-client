wwt.controllers.controller('ExploreController',
	['$scope',
	'$rootScope',
	'AppState',
	'Places',
	'$timeout',
	'Util',
	'ThumbList',
	function ($scope, $rootScope, appState, places, $timeout, util,  thumbList) {
	    var exploreRoot;
	    var depth = 1;
	    var bc;
	    var cache = [];
	    
	    var openCollection,
			collectionPlace,
			collectionPlaceIndex,
			hashObj;

	    $rootScope.$on('hashChange', function (event, obj) {
	        if (obj['place'] && isNaN(parseInt(obj['place'].charAt(0)))) {
	            hashObj = obj;
	            collectionPlace = obj['place'];
	            collectionPlaceIndex = 1;
	            $scope.loadingUrlPlace = true;
	            $('#loadingModal').modal('show');
	            places.getRoot().then(function () {
	                $scope.breadCrumb = bc = [$scope.getFromEn('Collections')];
	                $('body').append($('#researchMenu'));
	                $scope.collection = exploreRoot;
	                findCollectionChild();
	            });
	        } 
	    });

	    $scope.initExploreView = function (hashChange) {
	        thumbList.init($scope, 'explore');
	        if (!hashChange) {
	            places.getRoot().then(function (result) {
	                $('body').append($('#researchMenu'));
	                $scope.collection = exploreRoot = result;

	                var collectionsString = $scope.getFromEn('Collections');
	                if (collectionsString.then) {
	                    collectionsString.then(function (s) {
	                        $scope.breadCrumb = bc = [s];
	                    });
	                } else {
	                    $scope.breadCrumb = bc = [collectionsString];
	                }

	                cache = [result];
	                calcPageSize();
	                $.each(result, function (i, item) {
	                    if (item.get_name() === 'Open Collections') {
	                        result.splice(0, 0, result.splice(i, 1)[0]);
	                        if (item.get_name() === 'Open Collections') {
	                            openCollection = true;
	                            $scope.clickThumb(item);
	                        } 
	                    }
	                });

	            });
	        }

	    };

	    var findCollectionChild = function () {
	        var guidParts = collectionPlace.split('.');
	        var relevantPart = guidParts.slice(0, collectionPlaceIndex).join('.');
	        var child = places.findChildById(relevantPart, $scope.collection);
	        if (collectionPlaceIndex < guidParts.length) {
	            collectionPlaceIndex++;
	            $scope.clickThumb(child, findCollectionChild);
	        } else {
	            $timeout(function () {
	                $scope.loadingUrlPlace = false;
	                $scope.clickThumb(child);
	                $('#loadingModal').modal('hide');
                      
	                if (hashObj['ra']) {
	                    var timer = hashObj['place'].toLowerCase().indexOf('hirise') !== -1 ? 6666 : 3333;
	                    setTimeout(function () {
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
        //called by openitemcontroller
	    $scope.$on('initExplorer', function (event, collectionUrl) {
	        if (!handledInitBroadcast) {
	            newCollectionUrl = collectionUrl;
	            $scope.initExploreView();
	            handledInitBroadcast = true;
	            setTimeout(function () { handledInitBroadcast = false; }, 2000);
	        }
	    });

	    var calcPageSize = function () {
	        thumbList.calcPageSize($scope, false);
	    };
	    $scope.clickThumb = function (item, folderCallback) {
	      if (typeof folderCallback === 'boolean'){
	        $rootScope.instant = folderCallback;
        }
	        var outParams = {
	            breadCrumb: bc,
              depth:depth,
	            cache: cache,
	            openCollection: openCollection,
	            newCollectionUrl: newCollectionUrl
	        };
	        var newParams = thumbList.clickThumb(item, $scope, outParams, folderCallback);
	        bc = newParams.breadCrumb;
	        cache = newParams.cache;
	        openCollection = newParams.openCollection;
	        newCollectionUrl = newParams.newCollectionUrl;
	        depth = newParams.depth;
	    };

	    $scope.expanded = false;
	    

	    $scope.breadCrumbClick = function (index) {
	        $scope.collection = cache[index];
	        while (bc.length - 1 > index) {
	            bc.pop();
	            cache.pop();
	        }
	        $scope.currentPage = 0;
	        calcPageSize();
	    };



	    $(window).on('resize', function () {
	        $scope.currentPage = 0;
	        calcPageSize();
	    });

	    
	    $scope.preventClickBubble = function (event) {
	        event.stopImmediatePropagation();
	    };

	    $scope.initExploreView();
	}]);

