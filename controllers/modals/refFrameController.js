wwt.controllers.controller('refFrameController', ['$scope', function ($scope) {

  $scope.page = 'welcome';
  $scope.pages = ['welcome','options','position','trajectory'];
  $scope.offsetTypes = [{
    type:'FixedSherical',
    label:'Fixed Spherical'
  },{
    type: 'Orbital',
    label:'Orbital'
  },{
    type: 'Trajectory',
    label:'Trajectory'
  },{
    type: 'Synodic',
    label:'Synodic'
  }];
  $scope.offsetType='FixedSherical';
  $scope.buttonsEnabled = {
    next:false,
    back:false,
    finish:false
  };
  var calcButtonState = function(){
    var i = $scope.pages.indexOf($scope.page);
    $scope.buttonsEnabled.next = $scope.page=='options';
    $scope.buttonsEnabled.back = i >0;
    $scope.buttonsEnabled.finish = i===$scope.pages.length-1;
  };
  $scope.next = function(){
    var i = $scope.pages.indexOf($scope.page);
    $scope.page = $scope.pages[i+1];
    calcButtonState();
  };
  $scope.back = function(){
    var i = $scope.pages.indexOf($scope.page);
    $scope.page = $scope.pages[i-1];
    calcButtonState();
  };
  $scope.finish = function(){
    alert('tba');
  };
  $scope.refFrameName = '';
  calcButtonState();
}]);
