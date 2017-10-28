wwt.controllers.controller('voTableViewer',
  ['$rootScope', '$scope', 'AppState', 'Util',
    function ($rootScope, $scope, appState, util) {
      var init = function () {

        $('.wwt-modal .modal-dialog, .wwt-modal .modal-content').width(1120);
        console.log($scope.votable);
        var colArray = [];
        if ($scope.votable && $scope.votable.columns) {
          Object.keys($scope.votable.columns).forEach(function (c, i) {
            var col = $scope.votable.columns[c]
            colArray.push(col);
            if (c === 'RA') {
              $scope.RASource = col.id;
              $scope.RAIndex = col.index;

            }
            if (c === 'DEC') {
              $scope.DecSource = col.id;
              $scope.DecIndex = col.index;
            }
          });
        }
        $scope.colArray = colArray;
      }


      $scope.hilite = function (row, $index) {
        var ra = parseFloat(row.columnData[$scope.RAIndex]);
        var dec = parseFloat(row.columnData[$scope.DecIndex]);
        wwt.wc.gotoRaDecZoom(ra,dec,60, true);
        $scope.hiliteIndex = $index;
      };

      $scope.hiliteIndex = -1;

      setTimeout(init,555);
    }
  ]);

