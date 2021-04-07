wwt.controllers.controller('voTableViewer',
  ['$rootScope', '$scope', 'AppState', 'Util',
    function ($rootScope, $scope, appState, util) {
      var layer = $scope.voTableLayer;
      $scope.plotTypes = [{lbl: 'Gaussian', type: 0}, {lbl: 'Point', type: 1},
        {lbl: 'Circle', type: 2},
        {lbl: 'Square', type: 3},
        {lbl: 'Push Pin', type: 4}];

      $scope.plotType = layer.get_plotType();
      var init = function () {

        //$('.wwt-modal .modal-dialog, .wwt-modal .modal-content').width(1120);

        var colArray = [];
        if ($scope.votable && $scope.votable.columns) {
          Object.keys($scope.votable.columns).forEach(function (c, i) {
            var col = $scope.votable.columns[c];
            col.id = col.id || c;
            colArray.push(col);
            if (c.toUpperCase() === 'RA' || col.ucd === "pos.eq.ra;meta.main") {
              $scope.RASource = col.id;
              $scope.RAIndex = col.index;

            }
            if (c.toUpperCase() === 'DEC' || col.ucd === "pos.eq.dec;meta.main") {
              $scope.DecSource = col.id;
              $scope.DecIndex = col.index;
            }
          });

          $scope.SizeSource = $scope.votable.column[layer.get_sizeColumn()].id;
          $scope.SizeIndex = $scope.votable.column[layer.get_sizeColumn()].index;

          $('.modal-content div.results').on('scroll',function(e){
            var div = e.currentTarget;
            var rows = $scope.votable.rows;
            var paged = $scope.votable.pagedRows;
            if (paged.length < rows.length && div.scrollHeight - div.scrollTop < 800){
              $scope.$applyAsync(function(){
                paged = paged.concat(rows.slice(paged.length, Math.min(rows.length, paged.length + 100)));
                $scope.votable.pagedRows = paged;
                //console.log({div:e.currentTarget,displayed:$scope.votable.pagedRows,total:$scope.votable.rows});
              });
            }

          })
        }
        $scope.colArray = colArray;
      };

      $scope.mapColumn = function (layerPropKey, modelVal) {

        layer[layerPropKey] = $scope.votable.columns[modelVal].index;
        layer.cleanUp();
      }
      $scope.updatePlot = function () {
        layer.set_plotType($scope.plotType);
        console.log($scope.plotType);
        layer.cleanUp();
      }
      $scope.hilite = function (row, $index) {
        var ra = parseFloat(row.columnData[$scope.RAIndex]);
        var dec = parseFloat(row.columnData[$scope.DecIndex]);
        wwt.wc.gotoRaDecZoom(ra,dec,$rootScope.viewport.Fov, true);
        $scope.hiliteIndex = $index;
      };

      $scope.hiliteIndex = -1;

      setTimeout(init,555);
    }
  ]);

//ra: lngColumn
//dec: latColumn
//dist: altColumn
//typeL markerColumn
//sizeL sizeColum

//call layer cleanup
