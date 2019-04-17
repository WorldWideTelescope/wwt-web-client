wwt.controllers.controller('SearchController',
	['$scope',
	'$rootScope',
	'$timeout',
	'Util',
	'SearchUtil',
	'ThumbList', 
	function ($scope, $rootScope, $timeout, util, searchUtil, thumbList) {

	    $scope.goto = { RA: '', Dec: '' };

	    var init = function () {
	        thumbList.init($scope, 'search');
	        if (util.isMobile) {
	            $scope.scrollDepth = 40;
	        }
	        $timeout(function () {
	            $scope.SearchType = 'J2000';
	            $('#txtSearch').focus();
	        }, 123);
	    };

	    $scope.clickThumb = function (item,instant) {
	      $rootScope.instant = !!instant;
	        thumbList.clickThumb(item, $scope);
	    }; 
         
        var calcPageSize = function () {
	        thumbList.calcPageSize($scope, false);
	    };
	    
	    $scope.expanded = false;
	    $scope.expandThumbnails = function (flag) {
	        $scope.currentPage = 0;
	        $scope.expanded = flag != undefined ? flag : !$scope.expanded;
	        $scope.expandTop($scope.expanded);
	        calcPageSize();
	    };

	    $(window).on('resize', function () {
	        $scope.currentPage = 0;
	        calcPageSize();
	    });

	    
	    $scope.gotoCoord = function () {
	        var ra =  util.parseHms($scope.goto.RA);
	        var dec = util.parseHms($scope.goto.Dec);
	        if (isNaN(ra)||isNaN(dec)){
	          $scope.goto.error = true;
	          return;
          }
	        $scope.goto.error = false;
	        var tempPlace = wwtlib.Place.create('tmp', dec, ra, null, null, wwtlib.ImageSetType[$scope.lookAt.toLowerCase()], 60);
	        $rootScope.singleton.gotoTarget(tempPlace, false, false, true);
	    };

	    $scope.searchKeyPress = function() {
	        $timeout(function() {
	            var q = $scope.q = $('#txtSearch').val();
	            searchUtil.runSearch(q).then(function(result) {
	                $scope.collection = result;
	                calcPageSize();
	            });
	        }, 10);
	    };

	    init();
	}
]);

