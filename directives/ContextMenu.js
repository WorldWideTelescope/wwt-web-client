wwt.app.directive('ngContextMenu', ['$dropdown', function ($dropdown) {
    return {
        restrict: 'A',
        scope: { method: '&ngContextMenu' },
        link: function (scope, element){
            var handler = scope.method();
            element.bind('contextmenu', function (event) {
                event.preventDefault();
                var index = event.delegateTarget.getAttribute('index');
                if (index) {
                    handler(parseInt(index));
                } else if  (handler) {
                    handler(event);
                }
            });
        }
    };
}]);
