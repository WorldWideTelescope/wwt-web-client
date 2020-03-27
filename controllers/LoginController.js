wwt.controllers.controller('LoginController',
    ['$scope',
    '$rootScope',
    '$http',
    'Util',
    '$cookies',
    '$timeout',
    function ($scope, $rootScope, $http, util, $cookies, $timeout) {

        $rootScope.loggedIn = false;

        function init() {

            $rootScope.liveAppId = $('body').data('liveid');
            if (util.getQSParam('code') != null) {

                var returnUrl = location.href.split('?')[0];
                location.href = '/LiveId/AuthenticateFromCode/' + util.getQSParam('code') +
                    '?returnUrl=' + encodeURIComponent(returnUrl);
            } else if ($cookies.get('access_token')) {

                $rootScope.loggedIn = true;
                $rootScope.token = $cookies.get('access_token');
            } 
        }

        function log(response) {
            if (response.refresh_token) {
                $cookies.put('refresh_token', response.refresh_token, { expires: new Date(2050, 1, 1) });
                $cookies.put('access_token', response.access_token, { expires: new Date(2050, 1, 1) });
                $timeout(function () {
                    $rootScope.loggedIn = true;
                });
            } 
            console.log(response, arguments);
        }

        $scope.login = $rootScope.login = function () {
            localStorage.setItem('login', new Date().valueOf())
            var redir = 'http://' + location.host + '/webclient';
            var wlUrl = 'https://login.live.com/oauth20_authorize.srf?client_id=' +
                $rootScope.liveAppId + '&scope=wl.offline_access%20wl.emails&response_type=code&redirect_uri=' +
                encodeURIComponent(redir) + '&display=popup';
            location.href = wlUrl;
            return;
        }

        $scope.logout = function () {
            localStorage.setItem('login', new Date().valueOf())
            var storedData = localStorage.getItem('userSettings');
            var data = storedData ? JSON.parse(storedData) : {};
            data['rememberMe'] = false;
            localStorage.setItem('userSettings', JSON.stringify(data));
            location.href = '/Logout';
        }

        init();

    }]);
