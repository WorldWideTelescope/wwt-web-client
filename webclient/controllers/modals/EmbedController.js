wwt.controllers.controller('EmbedController', ['$scope', 'AppState', '$rootScope', function ($scope, appState, $rootScope) {
  $rootScope.$on('escKey', $scope.$hide);
  var embedSize = appState.get('embedSize');
  $scope.embedSize = embedSize = embedSize || {height: 500, width: 800};
  console.log('stored embed', embedSize);
  $('.embed-modal').css('visibility','hidden');
  var modal;
  var save = function () {
    setTimeout(function () {
      if (!$('.embed-vid').length) {
        return;
      }
      embedSize = $scope.embedSize = {
        height: modal.height(),
        width: modal.width(),
        top: modal.parent().css('top'),
        left: modal.parent().css('left')
      };

      appState.set('embedSize', embedSize);
      console.log('saved', embedSize)
    }, 333);

  };
  var repos = function(){
    modal.parent().css({
      top: embedSize.top,
      left: embedSize.left
    });
  }
  $.getScript('https://code.jquery.com/ui/1.11.3/jquery-ui.min.js', function () {

    $('.modal-content .embed').parent().addClass('embed-vid');

    modal = $('.embed-vid');
    var modalBody = modal.find('.embed');
    modalBody.height(embedSize.height - 24).width(embedSize.width);
    modal.parent().on('movecomplete', save);
    if (embedSize.top) {

      setTimeout(function () {
        console.log('positions', embedSize);
        $('.embed-modal').css('visibility','visible');
      }, 555);
      setTimeout(repos, 333);
      setTimeout(repos, 111);
      setTimeout(repos, 777);
      setTimeout(repos, 999);
      setTimeout(repos, 1234);
    }else{
      $('.embed-modal').css('visibility','visible');
    }
    modal.height(embedSize.height).width(embedSize.width)
      .resizable({
        minHeight: 150,
        minWidth: 250
      }).on('resize', function () {
      save();
      modalBody.height(embedSize.height - 24).width(embedSize.width);

    });
  });
}]);
