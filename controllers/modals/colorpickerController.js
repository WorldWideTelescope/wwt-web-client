wwt.controllers.controller('colorpickerController',
  ['$scope', function ($scope) {
  var cp = $scope.colorpicker;

  var setMouse = function(e){
    if (!$scope.mouse){
      $scope.mouse = e;
    }
    $(window).off('mousemove',setMouse)
  }
  $(window).on('mousemove',setMouse);
  $scope.previewColor = 'rgba(0,0,0,0)';
  $scope.rgb = [0,0,0];
  $scope.opacity = 100;
  $scope.init = function(){
    var e = $scope.mouse || wwt.lastmouseContext;
    $('body.wwt-webclient-wrapper .colorpicker-modal.wwt-modal .modal-dialog').css({marginTop:e.pageY,marginLeft:e.pageX});
    console.log(wwt.lastmouseContext,$scope.mouse);
    var bar = $('.cross-fader.color-picker a.btn').css('left', 100);
    var slider = new wwt.Move({
      el: bar,
      bounds: {
        x: [-200,  0],
        y: [0, 0]
      },
      onstart: function () {
        bar.addClass('moving');
      },
      onmove: function (args) {
        $scope.opacity = this.css.left / 100;
        //$('img#colorhex').css('opacity', bar.css.left/100);
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
      /*if (c.name === ''){
        c.r = 255;
        c.b = 255;
        c.g = 255;
      }*/
      if (!c.a){
        c.a = 255;//????
      }
      $scope.opacity = cp.color.a/255;
    }
    $scope.rgb = [c.r,c.g,c.b];
    $scope.setColor();
  };

  $scope.setColor = function(){
    $scope.$applyAsync(function(){
      $scope.previewColor = 'rgba('+ $scope.rgb.join(',') + ',' + $scope.opacity +')';
    });

  }

  $scope.commitColor = function(){
    cp.color.a = Math.min(255,Math.max(0,Math.round($scope.opacity*255)));
    var rgb = $scope.rgb;
    cp.color.r = rgb[0];
    cp.color.g = rgb[1];
    cp.color.b = rgb[2];
    cp.color.name='custom';
    cp.pickColor({});
    $scope.$hide();
  }

  setTimeout($scope.init,500);
  }]);
