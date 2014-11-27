wwt.app.directive("scrollBuffer", function ($window) {
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
				scope.scrollDepth = totalItems;
				scope.$apply();
				
			}
			
		});
		
	};
});