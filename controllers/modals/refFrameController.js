wwt.controllers.controller('refFrameController', ['$scope', function ($scope) {

  $scope.page = 'welcome';
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

  $scope.refFrameName = '';
}]);
