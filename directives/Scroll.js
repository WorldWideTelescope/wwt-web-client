wwt.app.directive("scrollBuffer", ['$window',function ($window) {
	return function ($scope, element, attrs) {
		var buffer = parseInt(attrs.scrollBuffer);
		var scope = $scope;
		var parent = $(element).parent();
		while (!parent.hasClass('modal-dialog')) parent = parent.parent();
		parent.on('scroll', function () {
			var st = this.scrollTop;
			var e = $(this);
			var w = e.width();
			var h = e.height();
			var tn = e.find('.modal-content div.tn').first();
			var cols = Math.floor(w / tn.width());
			var rowsAboveFold = Math.ceil((st + h) / tn.height());
			var totalItems = (rowsAboveFold + buffer) * cols;
			if (scope.scrollDepth < totalItems) {
				scope.$applyAsync(function(){
          scope.scrollDepth = totalItems;
        });


				
			}
			
		});
		
	};
}]);

wwt.app.directive("jqueryScrollbar", ['$rootScope','$window', function ($rootScope,$window) {
    return function ($scope, element, attrs) {
        
        var scope = $scope;
        var movable = $(element).find('.jspPane');
        $(element).on('mousewheel', function (event) {
            var e = event.originalEvent;
            movable = $(element).find('.jspPane');
            var delta = Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)));
            var curLeft = Math.abs(Math.floor(movable.position().left));
            var increment = 155;
            var newLeft;            
            if (delta < 0) {//scrolling down/right
                console.log('down')
                newLeft = Math.floor((curLeft + increment) / increment) * increment;
                newLeft = Math.max(curLeft, Math.abs(newLeft));
            } else {//scrolling up/left
                console.log('up')
                newLeft = Math.floor((curLeft - increment) / increment) * increment;
                newLeft = Math.min(curLeft, Math.abs(newLeft));
            }
            
            $(element).data('jsp').scrollToX(Math.abs(newLeft));
        })

    };
}
])
