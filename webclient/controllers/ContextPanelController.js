wwt.controllers.controller('ContextPanelController',
	['$scope',
	'$rootScope',
	'$timeout', 
	'Util', 
	'SearchUtil',
	'ThumbList',
	function ($scope, $rootScope, $timeout, util, searchUtil, thumbList) {
	    
	    var lastUpdate = new Date();

	    var init = function () {
	        $scope.isContextPanel = true;
	        thumbList.init($scope, 'context');
            
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

	    $scope.clickThumb = function (item,instant) {
        $rootScope.instant = !!instant;
	      thumbList.clickThumb(item, $scope);
	      util.removeHoverCircle();
	    };

	    $scope.hoverThumb = function (item) {
	        $scope.drawCircleOverPlace(item);
	    };

	    var calcPageSize = function () {
	        thumbList.calcPageSize($scope, true);
	    };

	    $(window).on('resize', function () {
	        $scope.currentPage = 0;
	        calcPageSize();
	        
	    });

	    $scope.moveNboMenu = function (i) {
	        $('#nboMenuContainer' + i).append($('#researchMenu'));
	    };
	    $scope.showMenu = function (item, i) {
	        $('.popover-content .close-btn').click();
	        setTimeout(function () {
	            $('.popover-content .close-btn').click();
	            $('#menuContainer' + i).find('.dropdown').addClass('open');
	        }, 10);
	        $scope.setMenuContextItem(item, true);
	    };
	    $scope.preventClickBubble = function (event) {
	        event.stopImmediatePropagation();
	    };

	    $scope.goBack = function () {
	        thumbList.goBack($scope);
	    };
	    $scope.goFwd = function () {
	        thumbList.goFwd($scope);
	    };

	    function findNearbyObjects() {
	        if (($scope.lookAt === 'Sky' || $scope.lookAt === 'SolarSystem') && !wwt.tourPlaying) {
	            searchUtil.findNearbyObjects({
	                lookAt: $scope.lookAt,
	                singleton: $rootScope.singleton
	            }).then(function (result) {
	                $scope.currentPage = 0;
	                $('body').append($('#researchMenu'));
	                $scope.collection = result;
	                if (util.isMobile) {
	                    $scope.setNBO($scope.collection);
	                }
	                calcPageSize($scope.collection);
	            });
	        }
	    }

	    init();

	}]);

