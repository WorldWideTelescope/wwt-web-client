wwt.controllers.controller('DataVizController', ['$scope', '$rootScope', 'Util',function ($scope, $rootScope,util) {
  $scope.pages = ['welcome','position','scale','markers','colormap','date','hover'];
  $scope.page = 'welcome';
  $scope.buttonsEnabled = {
    next: false,
    back: false,
    finish: false
  };
  $scope.pasteExcel = function (e) {
    var ev = e.originalEvent;
    var pasteData = ev.clipboardData.getData('Text');
    var lines = pasteData.split(/[\n\r]/).filter(function(l){return l.length>1});
    $scope.layer = wwtlib.LayerManager.createSpreadsheetLayer($scope.layerMap, "clipboard", pasteData);
    console.log($scope.layer);
    $scope.columns = $scope.layer._table$1.header.map(function(c,i){
      return {label:c,index:i};
    });
    var none = {label:'None',index:-1};
    $scope.columns.splice(0,0,none);
    $scope.buttonsEnabled.next = 1;
    setTimeout(function(){$('table .paste-control').html('');},1);
  };

  var calcButtonState = function () {
    var i = $scope.pages.indexOf($scope.page);
    $scope.buttonsEnabled.next = $scope.page === 'welcome' ? !!$scope.layer : i < $scope.pages.length - 1;
    $scope.buttonsEnabled.back = i > 0;
    $scope.buttonsEnabled.finish = i === $scope.pages.length - 1;
  };
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
    //console.log($scope.dialog);
    $scope.dialog.OK($scope.layer);
  };
}]);
