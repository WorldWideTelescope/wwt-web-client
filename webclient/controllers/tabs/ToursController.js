wwt.controllers.controller('ToursController',
	['$scope',
		'$rootScope',
	'AppState',
	'Tours',
	'$timeout','Util','$popover',
	function ($scope, $rootScope, appState, tours, $timeout, util, $popover) {
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
			
			
			if (ss.canCast(item, wwtlib.Tour)) {
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
				var tnWid = 118;
				var winWid = $(window).width();
				$scope.pageSize = Math.floor(winWid / tnWid);
				$scope.pageCount = Math.ceil($scope.tourList.length / $scope.pageSize);
			}, 1);
			
		};

	    var popover = null;
	    var mask = null;
	    $scope.tourPreview = function (event, item) {

	        if (item.get_isFolder() || item.get_name() === 'Up Level') return;
	        $scope.relatedTours = tours.getToursById(item.relatedTours);
			$rootScope.tour = item;
	        if (!mask) {
	            mask = $('<div></div>').css({
	                height: 70,
	                width: 120,
	                top: -999,
	                left: -999,
	                opacity: 0,
                    background:'#fff',
                    position: 'fixed',
                    zIndex:2
                    
	            }).on('mouseleave',hideMask);
	            $('body').append(mask);
	        }
	        if (popover) {
		        popover.hide();
		    }
		    var options = {
		        title: item.get_name(),
		        target: $(event.currentTarget),
		        id: 'tourpop',
		        templateUrl:'views/popovers/tour-template.html',
		        contentTemplate: 'views/popovers/tour-info.html',
		        placement: 'bottom-left',
		        scope: $scope,
		        trigger:'manual'
		    };
		    
		    popover = $popover($(event.currentTarget),options);
		    popover.$promise.then(function () {
		        popover.show();
		        var thumb = $(event.currentTarget);
		        var pos = thumb.offset();
		        mask.css({
		            top: pos.top - 2, 
		            left: pos.left - 2,
		            opacity: .01
		        });
		        var fixImages = function () {//wth?
		            $('.tour-info .author-image img').each(function () {
		                if (this.naturalHeight > 0 && $(this).height() === 0) {
		                    //console.log('fixing',this.naturalHeight, $(this).height());
		                    $(this).css({
		                        height: this.naturalHeight,
		                        width: this.naturalWidth
		                    });
		                    //console.log('fixed?', this.naturalHeight, $(this).height());
		                }
		                
		                
		            });
		        }
		        $('.tour-info img').off('load');
		        $('.tour-info img').on('load', fixImages);
		        setTimeout(fixImages, 500);
		        setTimeout(fixImages, 1500);
		    });
	    };
        var hideMask = function() {
            mask.css({
                top: -999,
                left: -999
            });
        }

	    //var fixPop = function() {
        //    $('.popover').removeClass('modal').css('top', '8px').removeAttr('tabindex')
            
        //    /*$('.popover').find('.modal-dialog,.modal-content,.modal-body')
        //        .css({ margin: 0, padding: 0,width:470 })
        //        .removeClass('modal-dialog modal-content modal-body');*/
        //    var thumb = $('.popover').parent().find('a').first();
        //    var pos = thumb.offset();
        //    $('.modal-backdrop').css({
        //        height: thumb.height() + 8,
        //        width: thumb.width() + 12,
        //        top: pos.top - 2,
        //        left: pos.left - 2,
        //        right: '',
        //        bottom: '',
        //        opacity: 1
        //    });
        //    $('.popover').find('.label').click();
        //    $('.popover').find('.fa-play-circle').trigger('mouseenter').focus();
        //    $('.popover').css('height', $('.modal-dialog').height());
        //    util.log(pos, $('.popover').find('.label').text());
        //}

	    $(window).on('resize', calcPageSize);

		
	}
]);