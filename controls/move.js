wwt.Move = function (createArgs) {
	
	//#region initialization
	var el,
		grid,
		bounds,
		actualBounds = { left: null, top: null },
		onmove,
		onstart,
		oncomplete,
		moveObj = {}, // var placeholder for instance of move function
		isMoving,
		target,
		pointerId;

	function init(args) {
		bounds = args.bounds || null; /*{ // boundaries from start of move
			x: [null, null], // min. max
			y: [null, null] // min, max
			};*/
		grid = args.grid || 1;
		el = args.el;
		target = args.target || el;
		onstart = args.onstart;
		onmove = args.onmove;
		oncomplete = args.oncomplete;
		setBounds();
		//  IE (sigh)
		if (window.PointerEvent || window.MSPointerEvent) { 
		    
			target.css('touch-action', 'none');
			var pointerDownName = window.PointerEvent ? 'pointerdown' : 'MSPointerDown';
			var pointerUpName = window.PointerEvent ? 'pointerup' : 'MSPointerUp';
			var pointerMoveName = window.PointerEvent ? 'pointermove' : 'MSPointerMove';
			document.body.addEventListener(pointerDownName, function (event) {
			    if (target.hasClass('disabled')) {
			        return;
			    }
				if ((event.target !== target[0] && !$(target).has(event.target).length) || isMoving) {
					return;
				}
        else if (event.target.className.indexOf('fa-close') > -1){
          return;
        }
				if (document.body.setPointerCapture) {
					document.body.setPointerCapture(event.pointerId);
				}
				else if (document.body.msSetPointerCapture) {
					document.body.msSetPointerCapture(event.pointerId);
				}


        console.log(event.target, event.target.className.indexOf('fa-close') > -1);
				event.preventDefault();
				event.stopPropagation();
				if (event.pointerId !== undefined) {
					pointerId = event.pointerId;
				}
				
				moveInit(event);

				document.body.addEventListener(pointerUpName, unbind, false);
				document.body.addEventListener(pointerMoveName, function (evt) {
					if (pointerId !== undefined && evt.pointerId === pointerId) {
						motionHandler(evt);
					} 
				}, false);
			}, false);
			
		} else {
		    
		    target.on('mousedown touchstart', function (event) {
		        if (target.hasClass('disabled')) {
		            return;
		        }
				event.preventDefault();
				event.stopPropagation();
				moveInit(event);
				$(document).on('mouseup touchend', unbind);
				$(document).on('mousemove touchmove', motionHandler);
				
			});
		}
		el.css({ position: 'absolute' });
	};

	function setBounds(newBounds) {
		bounds = newBounds || bounds;
		if (!bounds) {
			actualBounds.left = [0 - Infinity, Infinity];
			actualBounds.top = [0 - Infinity, Infinity];
			return;
		}
		var css = {
			left: parseFloat(el.css('left')) || 0,
			top: parseFloat(el.css('top')) || 0
		};
		if (bounds.x) {
			actualBounds.left = [css.left + bounds.x[0], css.left + bounds.x[1]];
		} else {
			actualBounds.left = [0 - Infinity, Infinity];
		}
		if (bounds.y) {
			actualBounds.top = [css.top + bounds.y[0], css.top + bounds.y[1]];
		} else {
			actualBounds.top = [0 - Infinity, Infinity];
		}
	}

	//#endregion

	//#region event handlers
	var moveInit = function (event) {
		moveObj.mouseCoord = getCoord(event);
		moveObj.startCoord = { x: parseFloat(el.css('left')), y: parseFloat(el.css('top')) };
		if (isNaN(moveObj.startCoord.x)) moveObj.startCoord.x = 0;
		if (isNaN(moveObj.startCoord.y)) moveObj.startCoord.y = 0;
		moveObj.moveDist = { x: 0, y: 0 };
		moveObj.totalDist = { x: 0, y: 0 };
		moveObj.clickOffset = wwt.getClickOffset(event);
		moveObj.css = { top: moveObj.startCoord.y, left: moveObj.startCoord.x };
		moveObj.maxX = actualBounds.left[0] + actualBounds.left[1];
		moveObj.maxY = actualBounds.top[0] + actualBounds.top[1];
		isMoving = true;
		if (onstart) {
			onstart.call(moveObj);
		}
		//el.trigger('dragstart');
	};

	var motionHandler = function (evt) {
		evt.stopPropagation();
		//evt.preventDefault();
		var newCoord = getCoord(evt);

		moveObj.moveDist = {
			x: newCoord.x - moveObj.mouseCoord.x,
			y: newCoord.y - moveObj.mouseCoord.y
		};
		moveObj.mouseCoord = newCoord;
		moveObj.css.top += moveObj.moveDist.y;
		moveObj.css.left += moveObj.moveDist.x;
		moveObj.totalDist = {
			x: moveObj.totalDist.x + moveObj.moveDist.x,
			y: moveObj.totalDist.y + moveObj.moveDist.y
		};
			
		moveObj.gridCss = {
			left: (Math.round(moveObj.totalDist.x / grid) * grid) + moveObj.startCoord.x,
			top: (Math.round(moveObj.totalDist.y / grid) * grid) + moveObj.startCoord.y
		};
		moveObj.css = moveObj.gridCss;
		

		moveObj.css.top = Math.min(Math.max(actualBounds.top[0], moveObj.css.top), actualBounds.top[1]);
		moveObj.css.left = Math.min(Math.max(actualBounds.left[0], moveObj.css.left), actualBounds.left[1]);
		moveObj.pctX = Math.max(actualBounds.left[0], moveObj.css.left) / moveObj.maxX;
		moveObj.pctY = Math.max(actualBounds.top[0], moveObj.css.top) / moveObj.maxY;
		el.css(moveObj.css);
			
		if (onmove) {
			//el.trigger('dragmove');
			onmove.call(moveObj);
		}
		
	};

	var unbind = function (evt) {
		pointerId = null;
		isMoving = false;
		$(document).off('mouseup touchend MSPointerUp', unbind);
		$(document).off('mousemove touchmove MSPointerMove', motionHandler);
		//el.trigger('dragmovecomplete');
		moveEnd(evt);
	};

	var moveEnd = function(event) {
		if (oncomplete)
			oncomplete.call(moveObj);
		isMoving = moveObj.isMoving = false;
		if (event) {
			event.preventDefault();
		}
		el.trigger('movecomplete',moveObj);
	};
	//#endregion

	var getCoord = function (evt) {
		var coord = {};
		if (evt.originalEvent && evt.originalEvent.targetTouches) {
			coord.x = evt.originalEvent.targetTouches[0].pageX;
			coord.y = evt.originalEvent.targetTouches[0].pageY;
		}
		else if (evt.originalEvent) {
			coord.x = evt.originalEvent.pageX;
			coord.y = evt.originalEvent.pageY;
		} else {
			coord.x = evt.pageX;
			coord.y = evt.pageY;
		}
		return coord;
	}

	init(createArgs);
	return {
		setBounds: setBounds
	};
};
