wwt.getRGBArray = function (colorString) {
		try {
			var rgb = [];
			var i;
			if (colorString.indexOf('#') == 0) { //hex
				if (colorString.length <= 5)
					for (i = 1; i < colorString.length; i++)
						rgb.push(parseInt('0x' + colorString.charAt(i), 16) * 16);
				else
					for (i = 1; i < colorString.length; i += 2)
						rgb.push(parseInt('0x' + colorString.substr(i, 2), 16));
			}
			else {
				// rgb(#,#,#) or rgba(#,#,#,#)
				if (colorString.indexOf(')') != -1)
					colorString = colorString.split('(')[1].split(')')[0];

				var split = colorString.split(',');

				for (i = 0; i < split.length; i++)
					rgb.push(parseFloat(split[i]));
			}
			return rgb;
		} catch (error) {
			return error;
		}
	},
	wwt.getHSV= function (c, rgb) {
		var r, g, b;
		if (!rgb) rgb = this.getRGBArray(c);
		r = rgb[0], g = rgb[1], b = rgb[2];
		r = (r / 255);
		g = (g / 255);
		b = (b / 255);
		var min = Math.min(Math.min(r, g), b),
				max = Math.max(Math.max(r, g), b);

		var value = max,
				saturation,
				hue;
			
		// Hue  
		if (max == min)
			hue = 0;
		else if (max == r)
			hue = (60 * ((g - b) / (max - min))) % 360;
		else if (max == g)
			hue = 60 * ((b - r) / (max - min)) + 120;
		else if (max == b)
			hue = 60 * ((r - g) / (max - min)) + 240;
		else hue = 0;
		if (hue < 0)
			hue += 360;

		// Saturation  
		if (max == 0)
			saturation = 0;
		else
			saturation = 1 - (min / max);

		return [Math.round(hue),
						Math.round(saturation * 100),
						Math.round(value * 100)];
	},
		wwt.getHex= function (rgb) { // rgb must be array
			rgb = this.convertToIntArray(rgb);
			var hex = '#';
			for (var i = 0; i < rgb.length; i++) {
				var s = rgb[i].toString(16);
				hex += s.length == 1 ? '0' + s : s;
			}
			return hex;
		},
		wwt.rgbFromHSV= function (hsv) { // hsv must be array
			var h = hsv[0],
			s = hsv[1] / 100,
			v = hsv[2] / 100;

			var hi = Math.floor((h / 60) % 6);
			var f = (h / 60) - hi;
			var p = v * (1 - s);
			var q = v * (1 - f * s);
			var t = v * (1 - (1 - f) * s);

			var rgb = [];

			switch (hi) {
				case 0: rgb = [v, t, p]; break;
				case 1: rgb = [q, v, p]; break;
				case 2: rgb = [p, v, t]; break;
				case 3: rgb = [p, q, v]; break;
				case 4: rgb = [t, p, v]; break;
				case 5: rgb = [v, p, q]; break;
			}

			var r = Math.min(255, Math.round(rgb[0] * 256)),
					g = Math.min(255, Math.round(rgb[1] * 256)),
					b = Math.min(255, Math.round(rgb[2] * 256));

			return [r, g, b];
		},
		// blend 2 colors together at a certain ratio
		wwt.blendColors= function (rgba1, rgba2, v1, v2, val) {
			var ratio = (val - v1) / (v2 - v1);
			var rgba = [];
			var hasAlpha = false;
			if (rgba1[3] && rgba1[3] <= 1 && rgba1[3] >= 0) {
				hasAlpha = true;
				rgba1[3] = Math.round(rgba1[3] * 1000);
				rgba2[3] = Math.round(rgba2[3] * 1000);
			}
			for (var i = 0; i < rgba1.length; i++) {
				var hc = rgba2[i];
				var lc = rgba1[i];
				// high color is lower than low color - reverse the subtracted vals
				var reverse = hc < lc;
				// difference between the high and low values
				var diff = reverse ? lc - hc : hc - lc;
				var diffRatio = Math.round(diff * ratio);
				// add or subtract from lc based on reverse mode
				rgba[i] = reverse ? lc - diffRatio : diffRatio + lc;
			}
			if (hasAlpha) {
				rgba[3] = rgba[3] / 1000;
				rgba1[3] = rgba1[3] / 1000;
				rgba2[3] = rgba2[3] / 1000;
			}
			return rgba;
		},
		wwt.convertToIntArray= function (a) {
			for (var i = 0; i < a.length; i++) {
				a[i] = parseInt(a[i]);
			}
			return a;
		},
		wwt.rnd=function (high) {
			var r = Math.random();
			var noise = 16180339; 
			r = r * noise % 1;
			r = Math.floor(r * high + 1);
			return r;
		},

		/// returns coords relative to the top/left of the element receiving the click
		wwt.getClickOffset = function (event) {
			var coords = { x: event.pageX, y: event.pageY };
			var off = $(event.target).offset();
			return {
				x: coords.x - off.left,
				y: coords.y - off.top
			};
		},

		wwt.getAngle = function (c1, c2, getDist) {
			if (c1.left || c1.left == 0) {
				c1.x = c1.left;
				c1.y = c1.top;
				c2.x = c2.left;
				c2.y = c2.top;
			}
			var dx = Math.abs(c1.x - c2.x),
					dy = Math.abs(c1.y - c2.y),
					dist = Math.sqrt(dx * dx + dy * dy),
					deg = (3600 + Math.round((Math.atan2(c2.y - c1.y, c2.x - c1.x) / 6.28) * 3600)) % 3600;
			deg = 3600 - deg;
			deg = deg / 10;
			return getDist ? {
				dist: dist,
				deg: deg % 360
			} : deg % 360;
		},
		wwt.getCoordsFromDegrees= function (deg, r, midX, midY) {
			return {
				x: midX + r * Math.cos(deg * (Math.PI / 180)),
				y: midY - r * Math.sin(deg * (Math.PI / 180))
			};
		},
		wwt.cssInt= function (el, attr) {
			return parseInt(el.css(attr));
		},
		wwt.round= function (flt, precision) {
			var multiplier = Math.pow(10, precision);
			return Math.round(flt * multiplier) / multiplier;
		},
		wwt.pct= function (v) {
			return Math.round(v * 1000) / 10 + '%';
		},
		
		
		// Robert Penner Easing Equations
		wwt.tweenStep= function (vFrom, vTo, curStep, steps, transition, easing) {
			var b = vFrom;
			var c = vTo - vFrom;
			var t = curStep;
			var d = steps;
			var s;
			switch (transition) {
				case 0: // transitions.linear
					return c * t / d + b;

				case 1: // transitions.back
					s = 1.70158;
					switch (easing) {
						case 1:
							return c * (t /= d) * t * ((s + 1) * t - s) + b;

						case 2:
							return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;

						default:
							if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525)) + 1) * t - s)) + b;
							return c / 2 * ((t -= 2) * t * (((s *= (1.525)) + 1) * t + s) + 2) + b;

					}

				case 2: // transitions.bounce
					var bounceOut = function (t, b, c, d) {
						if ((t /= d) < (1 / 2.75)) {
							return c * (7.5625 * t * t) + b;
						} else if (t < (2 / 2.75)) {
							return c * (7.5625 * (t -= (1.5 / 2.75)) * t + .75) + b;
						} else if (t < (2.5 / 2.75)) {
							return c * (7.5625 * (t -= (2.25 / 2.75)) * t + .9375) + b;
						} else {
							return c * (7.5625 * (t -= (2.625 / 2.75)) * t + .984375) + b;
						}
					};
					var bounceIn = function (t, b, c, d) {
						return c - (bounceOut(d - t, 0, c, d)) + b;
					};
					switch (easing) {
						case 1: return bounceIn(t, b, c, d);
						case 2: return bounceOut(t, b, c, d);
						default:
							if (t < d / 2)
								return (bounceIn(t * 2, 0, c, d)) * .5 + b;
							else
								return bounceOut(t * 2 - d, 0, c, d) * .5 + c * .5 + b;
					}

				case 3: // transitions.circular
					switch (easing) {
						case 1:
							return -c * (Math.sqrt(1 - (t /= d) * t) - 1) + b;
						case 2:
							return c * Math.sqrt(1 - (t = t / d - 1) * t) + b;
						default:
							if ((t /= d / 2) < 1) return -c / 2 * (Math.sqrt(1 - t * t) - 1) + b;
							return c / 2 * (Math.sqrt(1 - (t -= 2) * t) + 1) + b;
					}
				case 4: // transitions.cubic
					switch (easing) {
						case 1:
							return c * (t /= d) * t * t + b;
						case 2:
							return c * ((t = t / d - 1) * t * t + 1) + b;
						default:
							if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
							return c / 2 * ((t -= 2) * t * t + 2) + b;
					}
				case 5: // transitions.elastic
					// ReSharper disable AssignedValueIsNeverUsed 
					// this looks like a resharper glitch since s definitely IS used below
					s = (d * .3) / 4;
					// ReSharper restore AssignedValueIsNeverUsed
					var p = d * .3;
					var a = c;
					switch (easing) {
						case 1:
							if (t == 0) return b;
							if ((t /= d) == 1) return b + c;
							else
								s = p / (2 * Math.PI) * Math.asin(c / a);

							return -(a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b;

						case 2:
							if (t == 0) return b;
							if ((t /= d) == 1) return b + c;
							else {
								s = p / (2 * Math.PI) * Math.asin(c / a);
							}
							return (a * Math.pow(2, -10 * t) * Math.sin((t * d - s) * (2 * Math.PI) / p) + c + b);
						default:
							if (t == 0) return b; if ((t /= d / 2) == 2) return b + c;
							p = d * (.3 * 1.5);
							if (!a || a < Math.abs(c)) {
								a = c;
								s = p / 4;
							}
							else {
								s = p / (2 * Math.PI) * Math.asin(c / a);
							}
							if (t < 1) return -.5 * (a * Math.pow(2, 10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p)) + b;
							return a * Math.pow(2, -10 * (t -= 1)) * Math.sin((t * d - s) * (2 * Math.PI) / p) * .5 + c + b;
					}

				case 6: // transitions.exponential
					switch (easing) {
						case 1:
							return (t == 0) ? b : c * Math.pow(2, 10 * (t / d - 1)) + b;

						case 2:
							return (t == d) ? b + c : c * (-Math.pow(2, -10 * t / d) + 1) + b;

						default:
							if (t == 0) return b;
							if (t == d) return b + c;
							if ((t /= d / 2) < 1) return c / 2 * Math.pow(2, 10 * (t - 1)) + b;
							return c / 2 * (-Math.pow(2, -10 * --t) + 2) + b;

					}

				case 7: // transitions.quadratic
					switch (easing) {
						case 1:
							return c * (t /= d) * t + b;

						case 2:
							return -c * (t /= d) * (t - 2) + b;

						default:
							if ((t /= d / 2) < 1) return c / 2 * t * t + b;
							return -c / 2 * ((--t) * (t - 2) - 1) + b;

					}

				case 8: // transitions.quartic
					switch (easing) {
						case 1:
							return c * (t /= d) * t * t * t + b;

						case 2:
							return -c * ((t = t / d - 1) * t * t * t - 1) + b;

						default:
							if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
							return -c / 2 * ((t -= 2) * t * t * t - 2) + b;

					}

				case 9: // transitions.quintic
					switch (easing) {
						case 1:
							return c * (t /= d) * t * t * t * t + b;

						case 2:
							return c * ((t = t / d - 1) * t * t * t * t + 1) + b;

						default:
							if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
							return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;

					}

				case 10: // transitions.sine
					switch (easing) {
						case 1:
							return -c * Math.cos(t / d * (Math.PI / 2)) + c + b;

						case 2:
							return c * Math.sin(t / d * (Math.PI / 2)) + b;

						default:
							return -c / 2 * (Math.cos(Math.PI * t / d) - 1) + b;

					}

			}
			return 0;
		},
		wwt.clone= function (obj) {
			return $.parseJSON(JSON.stringify(obj));
		};
wwt.requestFullScreen = function(element) {
	if (element.requestFullscreen) {
		element.requestFullscreen();
	} else if (element.msRequestFullscreen) {
		element.msRequestFullscreen();
	} else if (element.mozRequestFullScreen) {
		element.mozRequestFullScreen();
	} else if (element.webkitRequestFullscreen) {
		element.webkitRequestFullscreen();
	} else {
		console.log("Fullscreen API is not supported");
	}
};
wwt.exitFullScreen = function(cb) {
	var previousFullScreen = document.fullScreenElement || document.mozFullScreenElement || document.webkitFullscreenElement;
	if (previousFullScreen) {

		if (previousFullScreen.cancelFullScreen) {
			previousFullScreen.cancelFullScreen();
		} else if (document.mozCancelFullScreen) {
			document.mozCancelFullScreen();
		} else if (document.webkitCancelFullScreen) {
			document.webkitCancelFullScreen();
		}
	} else if (document.msExitFullscreen) {
		document.msExitFullscreen();
	} /*else {
			fullScreenMode();
		}*/
	if (cb) {
		setTimeout(function() {
			cb({ fullscreen: false });
		}, 888);

	}
};