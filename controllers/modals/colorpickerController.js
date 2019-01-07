wwt.controllers.controller('colorpickerController', ['$scope', function ($scope) {
  var cp = $scope.colorpicker;
  var setMouse = function(e){
    if (!$scope.mouse){
      $scope.mouse = e;
    }
    $(window).off('mousemove', setMouse);
  }
  $(window).on('mousemove', setMouse);
  var opacity;
  $scope.init = function () {

    var e = $scope.mouse || wwt.lastmouseContext;
    $('body.wwt-webclient-wrapper div.colorpicker-modal.wwt-modal div.modal-dialog')
      .css({marginTop:e.pageY, marginLeft:e.pageX});
    opacity = cp.color.a / 255;
    var left = Math.round(opacity*100);
    var bar = $('.cross-fader.color-picker a.btn').css('left', left);
    var slider = new wwt.Move({
      el: bar,
      bounds: {
        x: [0-left, 100-left],
        y: [0, 0]
      },
      onstart: function () {
        bar.addClass('moving');
      },
      onmove: function (args) {
        opacity = this.css.left / 100;
        $scope.setColor();
      },
      oncomplete: function () {
        bar.removeClass('moving');
      }
    });
    $scope.pickColor();
  }

  $scope.pickColor = function(event){
    var c;
    if (event) {
      c = cp.getColorFromClick(event);
    }
    else{
      c = cp.color;
      opacity = cp.color.a / 255;
    }
    $scope.rgb = [c.r,c.g,c.b];
    $scope.setColor();
  };

  $scope.setColor = function(){
      $scope.$applyAsync(function () {
      $scope.previewColor = 'rgba(' + $scope.rgb.join(',') + ',' + opacity + ')';
    });
  };

  $scope.commitColor = function(){
    cp.color.a = Math.min(255,Math.max(0,Math.round(opacity*255)));
    var rgb = $scope.rgb;
    cp.color.r = rgb[0];
    cp.color.g = rgb[1];
    cp.color.b = rgb[2];
    cp.color.name='custom';
    cp.pickColor({});
    $scope.$hide();
  }

  setTimeout($scope.init,800);
  }]);
