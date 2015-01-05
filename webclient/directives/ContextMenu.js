wwt.app.directive('ngContextMenu', ['$parse', function ($parse) {

    return {
        restrict: 'A',
        scope: { method: '&ngContextMenu' },
        link: function (scope, element,attrs) {
            var handler = scope.method();
            var item = attrs.item;
            var index = attrs.index;
           
            element.bind('contextmenu', function (event) {
                event.preventDefault();
                if (item) {
                    $('#menuContainer' + index).append($('#researchMenu'));
                    handler(JSON.parse(item), parseInt(index));
                } else {
                    handler(event);
                }
                
            });
        }
    };
   
   
}]);