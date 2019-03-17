wwt.controllers.controller('DataVizController', ['$scope', '$rootScope', 'Util', function ($scope, $rootScope, util) {
  $scope.pages = ['Welcome', 'Position', 'Scale', 'Markers', 'Color Map', 'Date Time', 'Hover Text'];


  if ($scope.propertyMode) {
    $scope.pages.splice(0, 1);
    $scope.page = 'Position';
    initColumns();
  } else {
    $scope.page = 'Welcome';
  }

  $scope.buttonsEnabled = {
    next: false,
    back: false,
    finish: true
  };
  var l;
  $scope.log = function (s) {
    console.log(s || $scope.layer);
  };
  $scope.pasteExcel = function (e) {
    var ev = e.originalEvent;
    var pasteData = ev.clipboardData.getData('Text')
      .replace(/(\r\n|\r|\n)/g,'\r\n');//normalize line endings
    //console.log('paste data ok');
    $('#pasteRow').remove();
    //setTimeout(function () {
      //gc
      //console.log('cleaned');
     $scope.layerName = 'clipboard';
      l = $scope.layer = wwtlib.LayerManager.createSpreadsheetLayer($scope.layerMap, $scope.layerName, pasteData);

      initColumns();
      $scope.buttonsEnabled.next = $scope.buttonsEnabled.finish = 1;
    //}, 1);
  };
  var oldName =  'clipboard';
  $scope.setName = function(n){
    setTimeout(function(){
      l.set_name(n);
      var maps = wwtlib.LayerManager.get_allMaps()[l.referenceFrame].childMaps;
      maps[n] = maps[oldName];
      delete maps[oldName];
      oldName = n;
      wwt.detectNewLayers();
    },123)
  };
  function initColumns() {
    l = $scope.layer;
    $scope.columns = $scope.layer._table$1.header.map(function (c, i) {
      return {label: c, type: i};
    });
    l.computeDateDomainRange(0,-1);
    console.log('compute');
    var none = {label: 'None', index: -1};
    $scope.columns.splice(0, 0, none);
    l.psType = l.get_pointScaleType();
    sliders = {};
  }

  var sliders = {};
  var initExpSlider = function (sel, prop, initVal) {
    $scope[prop] = initVal;
    if (sliders[prop]) {
      return;
    }
    setTimeout(function () {
      var bar = $(sel);
      var off = parseInt(bar.css('left').replace('px', ''));
      sliders[prop] = new wwt.Move({
        el: bar,
        grid: 4,
        bounds: {y: [0, 0], x: [0 - off, 100 - off]},
        onstart: function () {
          bar.addClass('moving');
        },
        onmove: function () {
          var f = (this.css.left - 50) / 4;
          var v = $scope[prop];
          f = Math.min(Math.max(f, -12), 12);
          v = Math.pow(2, f);
          $scope.$applyAsync(function () {
            $scope[prop] = v;
            l['set_' + prop](v);
          });
        },
        oncomplete: function () {
          bar.removeClass('moving');
        }
      });
    }, 10);
  };

  var calcButtonState = function () {
    //console.log($scope.propertyMode);
    if (!$scope.propertyMode) {
      var i = $scope.pages.indexOf($scope.page);
      $scope.buttonsEnabled.next = $scope.page === 'Welcome' ? !!$scope.layer : i < $scope.pages.length - 1;
      $scope.buttonsEnabled.back = i > 0;
      $scope.buttonsEnabled.finish = !!$scope.layer;
    }
    if ($scope.page === 'Scale') {
      initExpSlider('.scale-factor .btn', 'scaleFactor', 1);
    }
    if ($scope.page === 'Date Time') {
      initExpSlider('.time-decay .btn', 'decay', 16);
    }
  };
  $scope.setPage = function (p) {
    $scope.page = p;
    calcButtonState();
  }
  calcButtonState();
  $scope.next = function () {
    var i = $scope.pages.indexOf($scope.page);
    $scope.page = $scope.pages[i + 1];
    calcButtonState();
  };
  $scope.back = function () {
    var i = $scope.pages.indexOf($scope.page);
    $scope.page = $scope.pages[i - 1];
    calcButtonState();
  };
  $scope.finish = function () {
    $scope.layer.version++;
    $scope.dialog.OK($scope.layer);
  };
}]);
