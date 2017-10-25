wwt.controllers.controller('voConeSearch',
  ['$rootScope',
    '$scope',
    'AppState',
    'Util',
    function ($rootScope, $scope, appState, util) {
      var init = function () {
        $('.wwt-modal .modal-dialog, .wwt-modal .modal-content').width(1120);

        if (util.getQSParam('debug') === 'disable') {
          wwtlib.WWTControl.singleton.render = function () {
            console.log('fixed render loop :)')
          }
          //testing only:
          $('#WorldWideTelescopeControlHost').html('');
        }
      }
      $scope.searchBaseURL = 'http://vizier.u-strasbg.fr/viz-bin/votable/-A?-source=J/AJ/138/598&';
      $scope.fromRegistry = true;
      $scope.fromView = true;
      $scope.catalogPref = true;
      $scope.siapImages = false;
      $scope.nope = function () {
        alert('not yet. soon...');
      };
      $scope.RA = $rootScope.viewport.RA;
      $scope.Dec = $rootScope.viewport.Dec;
      setTimeout(init, 444);
    }]);
