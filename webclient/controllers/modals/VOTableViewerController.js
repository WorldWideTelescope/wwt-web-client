wwt.controllers.controller('voTableViewer',
  ['$rootScope', '$scope', 'AppState', 'Util',
    function ($rootScope, $scope, appState, util) {
      var init = function () {

        $('.wwt-modal .modal-dialog, .wwt-modal .modal-content').width(1120);
        console.log($scope.votable);
        if ($scope.votable && $scope.votable.columns) {
          Object.keys($scope.votable.columns).forEach(function (c, i) {
            if (c === 'RA') {
              $scope.RAIndex = i;
            }
            if (c === 'DEC') {
              $scope.DECIndex = i;
            }
          });
        }

      }


      $scope.hilite = function (row, $index) {
        console.log('hilite',row,$index);
        $scope.hiliteIndex = $index;
      };

      $scope.hiliteIndex = -1;

      setTimeout(init,555);
    }
  ]);

