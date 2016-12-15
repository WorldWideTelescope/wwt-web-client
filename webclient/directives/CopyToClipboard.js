wwt.app.directive('copyable', ['$timeout',function ($timeout) {
    return {
        restrict: 'E',
        templateUrl:'views/copy-to-clipboard.html',
        scope: { copy: '=' },
        link:function(scope,el,attrs){
            var input = el.find('input')[0];
            var copyButton = el.find('.input-group-addon');
            copyButton.bind('click', function (event) {
                input.select();
                document.execCommand('copy');
                $timeout(function () {
                    scope.copy.showStatus = true;
                    scope.copy.fadeout = false;
                    input.blur();
                }, 0);
                $timeout(function () { 
                    scope.copy.fadeout = true;
                }, 3333);
            })
        }
    };
}]);