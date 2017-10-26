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
      $scope.searchRegistry = function () {
        var searchUrl = "http://nvo.stsci.edu/vor10/NVORegInt.asmx/VOTCapabilityPredicate?predicate=(title%20like%20'%25" +
          $scope.regTitle
          + "%25'%20or%20shortname%20like%20'%25" +
          $scope.regTitle
          + "%25')&capability=" + ($scope.catalogPref ? 'ConeSearch' : 'SIAP');
        console.log(searchUrl);
        wwtlib.VoTable.loadFromUrl(searchUrl, function () {
          $scope.table = this;
          $scope.$applyAsync(function () {
          });
          Object.keys(this.columns).forEach(function (c, i) {
            console.log($scope.table.columns[c].index + ':' + c + '(' + $scope.table.rows[5].columnData[$scope.table.columns[c].index] + ')');

          })
          console.log(this);
        })
      };
      $scope.displayColumns = [
        'title'
        , 'type'
        , 'capabilityClass'
        , 'publisher'
        , 'waveband'
        , 'identifier'
        , 'maxSearchRadius'
        , 'maxRecords'
      ];
      $scope.getData = function (row, columnKey) {
        return row.columnData[$scope.table.columns[columnKey].index] || '-';
      };
      $scope.hilite = function (row, $index) {
        $scope.hiliteIndex = $index;
        $scope.searchBaseURL = $scope.getData(row, 'accessURL');
      }
      $scope.hiliteIndex = 0;

      $scope.regTitle = 'hubble';
      $scope.searchBaseURL = '';
      $scope.fromRegistry = true;
      $scope.fromView = true;
      $scope.catalogPref = true;
      $scope.siapImages = false;
      $scope.nope = function () {
        alert('not yet. soon...');
      };
      $scope.RA = $rootScope.viewport.RA;
      $scope.Dec = $rootScope.viewport.Dec;
      $scope.Fov = $rootScope.viewport.Fov;
      console.log($rootScope.viewport);
      setTimeout(init, 444);
    }]);
