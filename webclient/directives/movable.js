wwt.app.directive('movable', ['AppState',function (appState) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var el = $(element);
            var target = el;
            var stickyCoords;
          var tSel = target.data('movable-target')
            var oncomplete = function () { };
          if (tSel) {
            el = target.parentsUntil(tSel).parent();
            //find(target.data('movable-target'));
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
          if (tSel && tSel.indexOf('modal') > -1) {

            setTimeout(function () {
              if (target.width() < 177) {
                return;
              }
              el.css({
                position: 'absolute',
                left: ($(document.body).width() - el.width()) / 2
              });

              //console.log({w: el.width()});
              Object.create(wwt.Move({
                el: el,
                target: target,
                oncomplete: oncomplete
              }));
            }, 1234);
          } else {
            var move = Object.create(wwt.Move({
                el: el,
                target: target,
              oncomplete: oncomplete
            }));
          }
        }
    };
}]);
