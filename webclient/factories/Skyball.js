wwt.app.factory('Skyball', ['$rootScope', function ($rootScope) {
  var api = {
    init: init
  };
  var canvas, ctx; 

  function draw() {
    var viewport = arguments[1];
    //console.log({ viewport: viewport, event: event });
    if (!viewport.isDirty) {
      return;
    }

    ctx.clearRect(0, 0, 100, 100);
    var sphereSize = $('#skyball').height();
    var radius = sphereSize / 2;
    var centerf = new Point(sphereSize * .71, sphereSize * .71);
    var center = new Point(sphereSize * .71, sphereSize * .71);
    var points = [];
    var rc = Math.PI / 180;
    var z = 0;
    var body = $('body');
    var h = body.height();
    var w = body.width();
    var coords = [
      wwtlib.WWTControl.singleton.getCoordinatesForScreenPoint(0, 0),
      wwtlib.WWTControl.singleton.getCoordinatesForScreenPoint(w, 0),
      wwtlib.WWTControl.singleton.getCoordinatesForScreenPoint(w, h),
      wwtlib.WWTControl.singleton.getCoordinatesForScreenPoint(0, h)
    ];
    coords.forEach(function (coord) {
      var corner = wwtlib.Coordinates.fromRaDec(coord.x, coord.y);
      var point = new Point();
      points.push(point);
      point.x = centerf.x - (Math.cos((corner.get_RA() + 6) / 12 * 180 * rc) * Math.cos(corner.get_lat() * rc) * radius);
      point.y = centerf.y - (Math.sin(corner.get_lat() * rc) * radius);
      z += (Math.sin((corner.get_RA() + 6) / 12 * 180 * rc) * Math.cos(corner.get_lat() * rc) * radius);
      ctx.beginPath();
      ctx.lineWidth = '1';
      ctx.moveTo(center.x, center.y);
      ctx.lineTo(point.x, point.y);
      ctx.closePath();
      ctx.stroke();
    });
    //console.log({coordx: coords[0].x, coordy: coords[0].y});

    ctx.beginPath();
    ctx.lineWidth = '1';
    ctx.strokeStyle = 'yellow';
    $.each(points, function (i, pt) {
      if (i === 0) {
        ctx.moveTo(pt.x, pt.y);
      } else {
        ctx.lineTo(pt.x, pt.y);
      }
    });
    ctx.closePath();
    ctx.fillStyle = (z / 4) > 0 ? 'rgba(255,255,0,.9)' : 'rgba(255,255,0,.5)';
    ctx.fill();
    ctx.stroke();
  }

  function init() {
    var skyball = $('#skyball');
    if (!skyball.length) {
      setTimeout(init, 500);
      return;
    }

    var mobile = skyball.hasClass('mobile');
    canvas = $('<canvas></canvas>')
      .css({
        position: 'absolute',
        top: mobile ? -5 : -9,
        left: mobile ? -9 : -14,
        height: 137,
        width: 307
      });
    skyball.append(canvas);
    ctx = canvas.get(0).getContext('2d');
    $rootScope.$on('viewportchange', draw);

    draw(null, {isDirty: true});

  }

  function Point(x, y) {
    this.x = x;
    this.y = y;
  }

  //console.log('skyball');

  return api;
}]);

