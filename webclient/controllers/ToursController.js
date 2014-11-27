wwt.controllers.controller('ToursController',
	['$scope',
		'$rootScope',
	'AppState',
	'Tours',
	'$timeout','Util',
	function ($scope, $rootScope, appState, tours, $timeout, util) {
		var toursRoot;
		var depth = 1;
		var bc = [$scope.getFromEn('Tours')];
		var cache = [];
		$scope.pageCount = 1;
		$scope.pageSize = 1;
		$scope.currentPage = 0;

		tours.getRoot().then(function (result) {
			$scope.tourList = toursRoot = result;
			$scope.breadCrumb = bc;
			cache.push(result);
			calcPageSize();
		});

		$scope.clickThumb = function (item) {
			$scope.activeItem = item.get_thumbnailUrl() + item.get_name();
			if (item.get_name() === 'Up Level') {
				$scope.currentPage = 0;
				depth--;
				bc.pop();
				$scope.breadCrumb = bc;//.join(' > ') + ' >';
				cache.pop();
				$scope.tourList = cache[cache.length - 1];
				calcPageSize();
				return;
			}
			if (item.get_isFolder()) {
				$scope.currentPage = 0;
				depth++;
				bc.push(item.get_name());
				$scope.breadCrumb = bc;
				tours.getChildren(item).then(function (result) {
					$scope.tourList = result;
					cache.push(result);
					calcPageSize();
				});
				
			}
			
			
			if (Type.canCast(item, wwtlib.Tour)) {
				$scope.playTour(item.get_tourUrl());
				if (util.isMobile) {
					$rootScope.landscapeMessage = true;
					setTimeout(function() {
						$scope.$hide();
					},2222);

				}

			}

		};

		$scope.breadCrumbClick = function (index) {
			$scope.tourList = cache[index];
			while (bc.length - 1 > index) {
				bc.pop();
				cache.pop();
			}
		};


		var calcPageSize = function () {
			$timeout(function () {
				var tnWid = 116;
				var winWid = $(window).width();
				$scope.pageSize = Math.floor(winWid / tnWid);
				$scope.pageCount = Math.ceil($scope.tourList.length / $scope.pageSize);
			}, 1);
			
		};

		$scope.tourPreview = function (event, item) {
			if (item.get_isFolder() || item.get_name() === 'Up Level') return;
			$scope.relatedTours = tours.getToursById(item.relatedTours);
			$scope.tour = item;
			
			var a = $(event.currentTarget);
			a.parent().append($('#popTrigger'));
			$('#popTrigger').trigger('mouseenter');
		};

		$(window).on('resize', calcPageSize);

		
	}
]);