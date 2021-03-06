wwt.controllers.controller(
  'LoginController',
  [
    '$scope',
    '$rootScope',
    'Util',
    '$cookies',

    function ($scope, $rootScope, util, $cookies) {
      $rootScope.loggedIn = false;

      function init() {
        if (util.getQSParam('code') != null) {
          var returnUrl = location.href.split('?')[0];
          location.href = $rootScope.communitiesUrlPrefix + '/LiveId/AuthenticateFromCode/' + util.getQSParam('code') +
            '/?returnUrl=' + encodeURIComponent(returnUrl);
        } else if ($cookies.get('access_token')) {
          $rootScope.loggedIn = true;
          $rootScope.token = $cookies.get('access_token');
        }
      }

      $scope.login = $rootScope.login = function () {
        localStorage.setItem('login', new Date().valueOf())

        var wlUrl = 'https://login.live.com/oauth20_authorize.srf?client_id=' +
            $rootScope.msLiveOAuthAppId + '&scope=wl.offline_access%20wl.emails&response_type=code&redirect_uri=' +
            encodeURIComponent($rootScope.msLiveOAuthRedirUrl) + '&display=popup';
        location.href = wlUrl;
        return;
      }

      $scope.logout = function () {
        localStorage.setItem('login', new Date().valueOf())
        var storedData = localStorage.getItem('userSettings');
        var data = storedData ? JSON.parse(storedData) : {};
        data['rememberMe'] = false;
        localStorage.setItem('userSettings', JSON.stringify(data));
        location.href = $rootScope.communitiesUrlPrefix + '/Logout';
      }

      init();
    }
  ]
);
