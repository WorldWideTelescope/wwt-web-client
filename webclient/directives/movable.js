wwt.app.directive('movable', ['AppState',function (appState) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var el = $(element);
            var target = el;
            var stickyCoords;
            var oncomplete = function () { };
            if (target.data('movable-target')) {
                el = $(target.data('movable-target'));
            }
            if (target.data('sticky')) {
                var stickyCss = appState.get(target.data('sticky'))
                if (stickyCss && stickyCss.top) {
                    el.css(stickyCss);
                    el.on('resize', function () {
                        stickyCss.width = el.width();
                        stickyCss.height = el.height();
                        appState.set(target.data('sticky'), stickyCss);
                    });
                }
                oncomplete = function () {
                    stickyCss.top = this.css.top;
                    stickyCss.left = this.css.left;
                    appState.set(target.data('sticky'), stickyCss);
                };
            }
            var move = Object.create(wwt.Move({
                el: el,
                target: target,
                oncomplete:oncomplete
            }));

        }
    };
}]);