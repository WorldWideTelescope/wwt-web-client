wwt.app.directive('ngContextMenu', ['$dropdown', function ($dropdown) {

    return {
        restrict: 'A',
        scope: { method: '&ngContextMenu' },
        link: function (scope, element,attrs) {
            var handler = scope.method();
            //var item = attrs.item;
            var index = attrs.index;
           
            element.bind('contextmenu', function (event) {
                event.preventDefault();
                if (index) {
                    handler(parseInt(index));
                } else {
                    handler(event);
                }
                
            });
        }
    };
   
   
}]);