wwt.app.directive('contenteditable', [function() {
    return {
        restrict: 'A',
        link: function(scope, element, attrs) {
          if (element.hasClass('paste-control')){
            return;
          }
          var isDuration = (element.hasClass('duration'));
            var hasFocused = false;
            var isLabel = !isDuration;
            var stop = scope.stop;
            var s = stop;
            var lastGoodValue;
            var el = $(element)[0];
            function validate() {
                var val = element.html();
                
                var minSec = val.split(':');
                var sec, min,tenths = 0,secString;
                if (minSec.length === 2) {
                    min = parseInt(minSec[0].replace(/\D/g, ''));
                    secString = minSec[1].split('.');
                    
                } else if (minSec.length === 1) {
                    min = 0;
                    secString = minSec[0].split('.');
                }
                else {
                    s.duration = lastGoodValue;
                    return;
                }
                sec = parseInt(secString[0].replace(/\D/g, ''));
                if (secString.length === 2) {
                    tenths = parseInt(secString[1].replace(/\D/g, ''));
                }
                s.duration = (min * 60000) + (sec * 1000) + (tenths * 100);
                
            }

            
            if (isDuration) {
                renderDuration();
            } else {
                element.html(s.description);
            }
            
            function renderDuration() {
                if (s.duration > 100)
                    lastGoodValue = s.duration;
                else
                    s.duration = lastGoodValue;

                var min = (s.duration / 60 / 1000) << 0;
                var secs = ((s.duration / 1000) % 60);
                var tenths = Math.round((secs % 1) * 10);
                secs = Math.floor(secs);
                
                s.durationString = min + ':' + (secs < 10 ? '0' : '') + secs + '.' + tenths;
                stop.set_duration(lastGoodValue);
                if (hasFocused) {
                    angular.element('#currentTourPanel').scope().refreshStops();
                }
                element.html(s.durationString);
            }

            element.on('keyup', function (event) {
                if (isDuration) {
                    switch (event.keyCode) {
                        case 33:
                        case 38:
                            stop.duration += 1000;
                            renderDuration();
                            return;
                        case 34:
                        case 40:
                            stop.duration -= 1000;
                            renderDuration();
                            return;
                        case 27:
                        case 13:
                        case 9:
                            element.blur();
                            return;
                        default:
                            validate();
                            break;
                    }
                    
                }
            });
            var incrementing = false;
            
                 
            element.on('focus', function () {
                hasFocused = true;
                if (isDuration) {
                    if (incrementing) return;
                    scope.$apply(function () {
                        stop.editingDuration = true;
                    });
                    element.parent().find('.tinybutton').on('mousedown', function (e) {
                        incrementing = true;
                        setTimeout(function () { incrementing = false }, 500);
                        var btn = $(this);

                        if (btn.hasClass('duration-up')) {
                            stop.duration += 1000;
                            renderDuration();
                        } else {
                            stop.duration -= 1000;
                            renderDuration();
                        }
                        select();
                    });
                    element.parent().find('.tinybutton').on('mouseup', select);
                }
                else {
                    select();
                }
            });
            
            element.on('blur', function () {
                if (incrementing) return;
                scope.$applyAsync(function () {
                    if (isDuration) {
                        
                        validate();
                        renderDuration();
                        stop.set_duration(lastGoodValue);
                        stop.editingDuration = false;

                    } else {
                        s.set_description(element.html());
                    }
                    angular.element('#currentTourPanel').scope().refreshStops();
                });
            });
            function select() {
                setTimeout(function () {
                    
                    var txt = element.text();
                    var range = document.createRange();
                    var start = 0, end = txt.length;
                    if (isDuration) {
                        start = txt.indexOf(':') + 1;
                        end = txt.indexOf('.');
                    }
                    range.setStart(el.firstChild, start);
                    range.setEnd(el.firstChild, end);
                    var sel = window.getSelection();
                    sel.removeAllRanges();
                    sel.addRange(range);
                }, 10);

            }
            
        }
    };
}]);
