wwt.controllers.controller('refFrameController', ['$scope', function ($scope) {

  $scope.page = $scope.propertyMode?'options':'welcome';
  $scope.pages = ['welcome', 'options', 'position'/*, 'trajectory'*/];
  $scope.offsetTypes = [{
    type: 0,
    label: 'Fixed Spherical'
  }, {
    type: 1,
    label: 'Orbital'
  }, {
    type: 2,
    label: 'Trajectory'
  }, {
    type: 3,
    label: 'Synodic'
  }];
  $scope.altUnits = [
    {type: 1, label: 'Meters'},
    {type: 2, label: 'Feet'},
    {type: 3, label: 'Inches'},
    {type: 4, label: 'Miles' },
    {type: 5, label: 'Kilometers'},
    {type: 6, label: 'Astronomical Units'},
    {type: 7, label: 'Light Years'},
    {type: 8, label: 'Parsecs' },
    {type: 9, label: 'MegaParsecs'},
    {type: 10, label: 'Custom'}
  ];
  $scope.offsetType = 1;
  $scope.buttonsEnabled = {
    next: false,
    back: false,
    finish: false
  };

  $scope.offsetTypeChange = function () {
    $scope.refFrame.referenceFrameType = $scope.offsetType;
  };
  $scope.hexColor = '#ffffff';
  $scope.colorChange = function () {
    var hex = $scope.hexColor;
    var rgb = hex.match(/[A-Za-z0-9]{2}/g).map(function (v) {
      return parseInt(v, 16)
    });
    $scope.refFrame.representativeColor.r = rgb[0];
    $scope.refFrame.representativeColor.g = rgb[1];
    $scope.refFrame.representativeColor.b = rgb[2];
  };
  var calcButtonState = function () {
    var i = $scope.pages.indexOf($scope.page);
    $scope.buttonsEnabled.next = $scope.page === 'welcome' ?
      $scope.refFrame.name.length :
      i < $scope.pages.length - 1;
    $scope.buttonsEnabled.back = i > 0;
    $scope.buttonsEnabled.finish = i === $scope.pages.length - 1;
  };
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
    console.log($scope.dialog);
    $scope.dialog.OK($scope.refFrame);
  };

  calcButtonState();
}]);
