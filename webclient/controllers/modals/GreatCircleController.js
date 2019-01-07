wwt.controllers.controller('greatCircleController', ['$scope', '$rootScope', 'Util',function ($scope, $rootScope,util) {
  $scope.getFromView = function (key) {
    var cam = $rootScope.singleton.renderContext.viewCamera;
    $scope.layer['_lat' + key + '$1'] = cam.lat;
    $scope.layer['_lng' + key + '$1'] = cam.lng;
  };

  $scope.hexColor = util.argb2Hex($scope.layer.color);
  $scope.colorChange = function () {
    util.hex2argb($scope.hexColor,$scope.layer.color);
  };
  $scope.ok = function(layer){
    layer.opened = true;
    console.log(layer);
    $scope.$hide();
  };
  console.log($scope.layer);
}]);
