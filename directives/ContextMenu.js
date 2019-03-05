wwt.app.directive('ngContextMenu', ['$dropdown', function ($dropdown) {
    return {
        restrict: 'A',
        scope: { method: '&ngContextMenu' },
        link: function (scope, element){
            var handler = scope.method();
            element.bind('contextmenu', function (event) {
                event.preventDefault();
              wwt.lastmouseContext = event;
                var index = event.delegateTarget.getAttribute('index');
                if (index) {
                    handler(parseInt(index),event);
                } else if  (handler) {
                    handler(event);
                }
            });
        }
    };
}]);

wwt.app.directive('ngRightClick', ['$parse', function ($parse) {
    return function (scope, element, attrs) {
        var fn = $parse(attrs.ngRightClick);
        element.bind('contextmenu', function (event) {

            scope.$applyAsync(function () {
              wwt.lastmouseContext = event;
                event.preventDefault();
                fn(scope, { $event: event });
            });
        });
    };
}]);

