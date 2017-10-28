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
        $scope.hiliteIndex = -1;
        var searchUrl = "http://nvo.stsci.edu/vor10/NVORegInt.asmx/VOTCapabilityPredicate?predicate=(title%20like%20'%25" +
          $scope.regTitle
          + "%25'%20or%20shortname%20like%20'%25" +
          $scope.regTitle
          + "%25')&capability=" + ($scope.coneSearch ? 'ConeSearch' : 'SIAP');
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
      };

      $scope.hiliteIndex = -1;

      $scope.regTitle = 'hubble';
      $scope.searchBaseURL = '';
      $scope.fromRegistry = true;
      $scope.fromView = true;
      $scope.coneSearch = true;
      $scope.siapImages = false;
      $scope.search = function () {
        var searchUrl;
        if ($scope.hiliteIndex < 0){
          //known good url for results
          searchUrl = "http://casjobs.sdss.org/vo/dr5cone/sdssConeSearch.asmx/ConeSearch?ra=202.507695905339&dec=47.2148314989668&sr=0.26563787460365";
        }
        else {
          var url = $scope.searchBaseURL.replace('&amp;', '&');
          var lastIndex = url.length - 1;
          if (url.lastIndexOf('?') !== lastIndex && url.lastIndexOf('&') !== lastIndex) {
            if (url.indexOf('?') > 0) {
              url += '&';
            } else {
              url += '?';
            }
          }

          var qObj = $scope.coneSearch ? {
            RA: $scope.RA,
            Dec: $scope.Dec,
            SR: $scope.Fov,
            VERB: $scope.verbosity
          } : {
            POS: $scope.RA + ',' + $scope.Dec,
            SIZE: $scope.Fov,
            VERB: $scope.verbosity
          };

          var params = [];
          Object.keys(qObj).forEach(function (k) {
            params.push(k + '=' + qObj[k]);
          });
          searchUrl = url + params.join('&');
        }
        wwtlib.VoTable.loadFromUrl(searchUrl, function () {
          var table = this;
          table.pagedRows = table.rows.slice(0,99);
          $rootScope.loadVOTableModal(this);
          //var layer = new wwtlib.VoTable();
          wwt.wc.addVoTableLayer(table);
          $scope.$parent.$hide();
        })

      };
      $scope.RA = $rootScope.viewport.RA;
      $scope.Dec = $rootScope.viewport.Dec;
      $scope.Fov = $rootScope.viewport.Fov;
      $scope.verbosity = 1;
      console.log($rootScope.viewport);
      setTimeout(init, 444);
    }]);
