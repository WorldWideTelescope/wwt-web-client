wwt.app.factory('Skyball',['$rootScope', function ($rootScope) {
	var api = {
		init: init
	};
	var canvas, ctx;
	//var renderLog = [];
	//var avgs = [];
	//var getAvg = function () {
	//	var sum = 0;
	//	$.each(renderLog, function() {
	//		sum += this;
	//	});
	//	avgs.push(sum / renderLog.length);
	//	renderLog = [];
	//}

	function draw(event, viewport) {
		if (!viewport.isDirty && !viewport.init){ return;}
		//var d1 = new Date();
		/*if (canvas == undefined) {
			init();
		}*/
		ctx.clearRect(0, 0, 100, 100);
		var sphereSize = $('#skyball').height();
		var radius = sphereSize / 2;
		var centerf = new point(sphereSize * .71, sphereSize * .71);
		var center = new point(sphereSize * .71, sphereSize * .71);
		var points = [];
		var rc = Math.PI / 180;
		var z = 0;
		var h = $('body').height();
		var w = $('body').width();
		var coords = [
			wwtlib.WWTControl.singleton.getCoordinatesForScreenPoint(0, 0),
			wwtlib.WWTControl.singleton.getCoordinatesForScreenPoint(w, 0),
			wwtlib.WWTControl.singleton.getCoordinatesForScreenPoint(w, h),
			wwtlib.WWTControl.singleton.getCoordinatesForScreenPoint(0, h)
		];
		var corners = [];
		$.each(coords, function(index, coord) {
			corners[index] = wwtlib.Coordinates.fromRaDec(coord.x, coord.y);
		});
		
		for (var i = 0; i < 4; i++) {
			points[i] = new point();
			points[i].x = centerf.x - (Math.cos((corners[i].get_RA() + 6) / 12 * 180 * rc) * Math.cos(corners[i].get_lat() * rc) * radius);
			points[i].y = centerf.y - (Math.sin(corners[i].get_lat() * rc) * radius);
			z += (Math.sin((corners[i].get_RA() + 6) / 12 * 180 * rc) * Math.cos(corners[i].get_lat() * rc) * radius);
			ctx.beginPath();
			ctx.lineWidth = '1';
			ctx.moveTo(center.x, center.y);
			ctx.lineTo(points[i].x, points[i].y);
			ctx.closePath();
			ctx.stroke();
		}
		ctx.beginPath();
		ctx.lineWidth = '1';
		ctx.strokeStyle = 'yellow';
		$.each(points, function(index, pt) {
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
		//var renderTime = new Date().valueOf() - d1.valueOf();
		//renderLog.push(renderTime);
		//if (renderLog.length == 50) {
		//	getAvg();
		//	console.log('skyball avg: ', avgs);
		//}
		
	};

	function init() {
		if (!$('#skyball').length) {
			setTimeout(init, 300);
			return;
		}
		var mobile = $('#skyball').hasClass('mobile');
		canvas = $('<canvas></canvas>')
		.css({
			position: 'absolute',
			top: mobile ? -5 : -9,
			left: mobile ? -9 : -14,
			height: 137,
			width:307
		});
		$('#skyball').append(canvas);
		ctx = canvas.get(0).getContext('2d');
		$rootScope.$on('viewportchange', draw);

		draw(null,{isDirty:true});
	}

	function point(x, y) {
		this.x = x, this.y = y;
	}

	
	return api;
}]);

