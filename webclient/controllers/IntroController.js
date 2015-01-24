wwt.app.controller('IntroController',['$rootScope','$scope','$timeout','Localization', function ($rootScope,$scope, $timeout,localization) {
	$scope.completed = function () {
		cleanUp();
	};
	$scope.exit = function () {
		cleanUp();
	};

	var cleanUp = function() {
		$('#WorldWideTelescopeControlHost').css('zIndex', 0);
		$('.layer-manager').css('zIndex', 1);
	};

	$scope.beforeChange = function () {
		var step = $scope.options.steps[this._currentStep];
		var stepNum = this._currentStep + 1;
		try {
			step.before();
		} catch (er) {
			setTimeout(function() {
				step.before();
			}, 10);
		}
		
		setTimeout(function() {
			$('.introjs-nextbutton').addClass('disabled').prop('disabled', true);
		}, 100);
		setTimeout(function() {
			$('.introjs-nextbutton').removeClass('disabled').prop('disabled', false);
			$('.introjs-tooltipbuttons small').text('(Step ' + stepNum + ' of ' + ($scope.options.steps.length - 1) + ')');
		}, step.enableMs ? step.enableMs : 1000);
	};
	$scope.afterChange = function () {
		$('.introjs-tooltipbuttons small').remove();
		var descEl = $('<small class=pull-left></small>')
			.text('(Preparing step ' + (this._currentStep + 1) + ' of ' + ($scope.options.steps.length - 1) + ')')
			.css({
				position: 'relative',
				top: 5,
				opacity:.8
			});
		$('.introjs-tooltipbuttons').append(descEl);
	};
	var loc = function(s) {
		return localization.getFromEn(s);
	}
	var start = function () {
		setTimeout(function() { $('#rbnExplore').click(); }, 500);
		$('#WorldWideTelescopeControlHost').css('zIndex', -2);
		$('.layer-manager').css('zIndex', -1);
		$('.introjs-nextbutton').hide();
	};
	$timeout(function () {
		$scope.options = {
			steps: [{
				element: $('#rbnExplore').parent().parent()[0],
				intro: loc('Each button has two parts.  Clicking on the top part with the label will reveal the associated panel.'),
				position: 'bottom',
				before: start
			},{
				element: $('#rbnExplore').parent().parent()[0],
				intro: loc('Clicking the arrow at the bottom of the button reveals a drop-down menu.'),
				position: 'bottom',
				before: function () {
					$('#tabMenu0')
						.addClass('hover')
						.parent().addClass('hover')
						.parent().addClass('hover')
						.parent().addClass('hover');
				}
			}, {
				element: $('#topPanel')[0],
				intro: loc('Within the Explore panel, you can browse imagery organized into various collections.'),
				position: 'bottom',
				before: function () {
					$('#tabMenu0')
						.removeClass('hover')
						.parent().removeClass('hover')
						.parent().removeClass('hover')
						.parent().removeClass('hover');
					
				}
			}, {
				element: $('#topPanel')[0],
				intro: loc('Choose from different image collections, such as this one on “Hubble Studies.”'),
				position: 'bottom',
				before: function () {
					$($('#topPanel .thumbnail')[5]).addClass('hover');
					setTimeout(function () {
						$($('#topPanel .thumbnail')[5]).click();
					}, 1);
				},
				enableMs:2000
			}, {
				element: $('.tn-expander')[0],
				intro: loc('The arrow at the bottom of panel of thumbnails expands or collapses the panel.'),
				position: 'bottom',
				before: function () {
					$('.tn-expander').click();
				}
			},{
				element: $('#topPanel')[0],
				intro: loc('Choose an item, such as the “Sombrero Galaxy.”  This moves the view to that location and overlays a foreground image from the Hubble Space Telescope on top of the all-sky background.'),
				position: 'bottom',
				before: function () {
					$($('#topPanel .thumbnail')[12]).addClass('hover');
					setTimeout(function() {
						$($('#topPanel .thumbnail')[12]).click();
					}, 800);
				},
				enableMs: 8000
			}, {
				element: $('#rbnTours').parent().parent()[0],
				intro: loc('Guided Tours have been created to present particular topics by using WWT to show specific views of the sky and astronomical objects.  You can browse guided tours by clicking the “Guided Tours” button.'),
				position: 'bottom',
				before: function () {
					$('#rbnTours').click();
				}
			}, {
				element: $('#topPanel')[0],
				intro: 'Guided tours are grouped by category',
				position: 'bottom',
				before: function () {
					$($('#topPanel .thumbnail')[2]).addClass('hover');
				}
			}, {
				element: $('#topPanel')[0],
				intro: loc('You can play a tour by clicking on the thumbnail.   Note, that most tours have music and narration so turn on your speakers! Please wait until this feature tour is complete before clicking a guided tour.'),
				position: 'bottom',
				before: function () {
					$($('#topPanel .thumbnail')[2]).removeClass('hover').click();
				}
			}, {
				element: $('#rbnSearch').parent().parent()[0],
				intro: ('The search panel enables you to search for objects in the sky by name or position.'),
				position: 'bottom',
				before: function () {
					$('#rbnSearch').click();
				}
			},{
				element: $('#topPanel')[0],
				intro: loc('Let’s search for “M51.” This shows a list of thumbnails in the search panel.  You can click on any to orient your view on the object.'),
				position: 'bottom',
				before: function () {
					var e = jQuery.Event("keypress");
					e.which = 50; // # Some rando key code value
					$("input").trigger(e);
					setTimeout(function () {
						$('#txtSearch').val('M');
						$('#txtSearch').trigger(e);
					}, 300);
					setTimeout(function () {
						$('#txtSearch').val('M5');
						$('#txtSearch').trigger(e);
					}, 1500);
					setTimeout(function () {
						$('#txtSearch').val('M51');
						$('#txtSearch').trigger(e);
					}, 2500);
					setTimeout(function () {
						$($('#topPanel .thumbnail')[2]).addClass('hover').click();
					}, 3500);
				},
				enableMs: 8000
			}, {
				element: $('.cross-fader').parent()[0],
				intro: loc('If you have a foreground and background image showing, you can adjust the opacity of the foreground image with the “Image Crossfade” slider.'),
				position: 'top',
				before: function () {
					$('.finder-scope').prop('hidden', true).fadeOut();

				}
			},{
				element: $('#topPanel')[0],
				intro: loc('Clicking on the View tab brings up a panel that allows you to setup your viewing location and the time.'),
				position: 'bottom',
				before: function () {
					$('#rbnView').click();
				}
			}, {
				element: $('#topPanel')[0],
				intro: loc('The Settings panel allows you to customize various things, such as the language.'),
				position: 'bottom',
				before: function () {
					$('#rbnSettings').click();
				}
			}, {
				element: null,
				intro: loc('You can use your mouse wheel (or touchpad scrolling function) to zoom in and out. You can also type + or -'),
				position: 'bottom',
				before: function () {
					setTimeout(function () {
						wwt.wc.zoom(.6667);
					}, 300);
					setTimeout(function () {
						wwt.wc.zoom(.6667);
					}, 1300);
					setTimeout(function () {
						wwt.wc.zoom(1.5);
					}, 2300);
					setTimeout(function () {
						wwt.wc.zoom(1.5);
					}, 3300);
				},
				enableMs: 3500
			}, {
				element: null,
				intro: loc('You can move the View by left-clicking and dragging your mouse.'),
				position: 'bottom',
				before: function () {
					wwt.wc.gotoRaDecZoom(0, 0, 60, true);
					setTimeout(function () {
						wwt.wc.gotoRaDecZoom(22, 0, 60);
					}, 1200);
					setTimeout(function () {
						wwt.wc.gotoRaDecZoom(0, 0, 60);
					},2200);

				},
				enableMs: 4000
			}, {
				element: null,
				intro: loc('You can rotate the view by holding the Control Key while you move your mouse.'),
				position: 'bottom',
				before: function () {
					wwtlib.WWTControl.singleton.renderContext.targetCamera.angle = -.3;
					wwtlib.WWTControl.singleton.renderContext.targetCamera.rotation = -.3;
					wwtlib.WWTControl.singleton.render();
					setTimeout(function () {
						wwtlib.WWTControl.singleton.renderContext.targetCamera.angle = .3;
						wwtlib.WWTControl.singleton.renderContext.targetCamera.rotation = .3;
						wwtlib.WWTControl.singleton.render();
					}, 1100);
					setTimeout(function () {
						wwtlib.WWTControl.singleton.renderContext.targetCamera.angle = 0;
						wwtlib.WWTControl.singleton.renderContext.targetCamera.rotation = 0;
						wwtlib.WWTControl.singleton.render();
					}, 2200);

				},
				enableMs: 2500
			}, {
				element: $('.finder-scope')[0],
				intro: loc('Right-clicking anywhere in the main view brings up a Finder Scope that allows you to investigate a specific object in more detail. The white circle shows the nearest object from the center of the crosshairs.'),
				position: 'right',
				before: function () {
					wwt.wc.gotoRaDecZoom(22, -11, 60, true);
					$rootScope.$broadcast('showFinderScope');

				}
			}, {
				element: $('#lstLookAt')[0],
				intro: loc('Near the bottom is a menu that controls what you are looking at.  You can look at the Sky, which is what you are looking at now.'),
				position: 'top',
				before: function () {
					$('.finder-scope').prop('hidden', true).fadeOut();

				}
			}, {
				element: $('#lstLookAt')[0],
				intro: loc('You can also look at Earth, which brings up a 3D view of our planet as seen from space.'),
				position: 'top',
				before: function () {
					setTimeout(function() {
						$('#lstLookAt').val(0).trigger('change');
					}, 10);
					
					setTimeout(function() {
						wwt.wc.zoom(.3);
					}, 1200);

				},
				enableMs: 1500
			}, {
				element: $('#lstLookAt')[0],
				intro: loc('You can look at Planets which shows you 3D views of other solar system worlds, such as the planet Mars.'),
				position: 'top',
				before: function () {
					$('#lstLookAt').val(1).trigger('change');
					setTimeout(function () {
						wwt.wc.zoom(.3);
					}, 1200);
				},
				enableMs: 2000
			}, {
				element: $('#lstLookAt')[0],
				intro: loc('When looking at Panoramas you can explore various panoramic images, which are wrap-around images taken from the surface of Earth, Mars, and Earth’s Moon.'),
				position: 'top',
				before: function () {
					$('#lstLookAt').val(3).trigger('change');
					
				},
				enableMs: 2000
			}, {
				element: $('#lstLookAt')[0],
				intro: loc('You can also look at the SolarSystem, which shows a 3D view of our Solar System, the 3D views of planets and orbits.'),
				position: 'top',
				before: function () {
					$('#lstLookAt').val(4).trigger('change');
					wwt.wc.gotoRaDecZoom(22, 90, 45);
				},
				enableMs: 2000
			}, {
				element: $('#topPanel')[0],
				intro: loc('When looking at SolarSystem, you can open the View tab and advance time forward many times faster than real-time to show the motion of the planets.'),
				position: 'bottom',
				before: function () {
					wwt.wc.gotoRaDecZoom(22, 90, 15);
					$('#rbnView').click();
					var decimalPlaces = 0;
					var speedUp = function() {
						decimalPlaces++;
						setTimeout(function() {
							$('#btnFastFwd').click();
							if (decimalPlaces < 7)speedUp();
						}, 500);
					};
					setTimeout(speedUp,1500);
				},
				enableMs: 5000
			}, {
				element: $('.context-panel')[0],
				intro: loc('The context panel at the bottom shows a list of objects with associated images that are in the main view.'),
				position: 'top',
				before: function () {
					$('#btnTimeNow').click();
					$('#lstLookAt').val(2).trigger('change');
					wwt.wc.gotoRaDecZoom(0, 0, 60);
				},
				enableMs: 2000
			}, {
				element: $('.context-panel')[0],
				intro: loc('When you move the view, the context panel updates the list of images in the view.'),
				position: 'top',
				before: function () {
					wwt.wc.gotoRaDecZoom(33, -11, 30);
				}
			},{
				element: $('.context-panel')[0],
				intro: loc('You can click on any thumbnail to move the main view to that location.'),
				position: 'top',
				before: function () {
					$($('.context-panel .thumbnail')[4]).addClass('hover').click();
				}
			}, {
				element: $('.layer-manager')[0],
				intro: loc('Additional information, such as grids and constellations, are available in the Layer Manager shown on the left.'),
				position: 'right',
				before: function() {}
			}, {
				element: $('#btnToggleLayerMgr')[0],
				intro: loc('You can hide and show the layer manager by clicking the Layer Manager button.'),
				position: 'right',
				before: function() {
					$('.introjs-nextbutton').hide();
				}
			}, {
				element: null,
				intro:loc('The tour is finished. You can play this tour anytime from the Welcome dialog. The Welcome dialog is available under the Explore menu. We hope you enjoy using WorldWide Telescope!')
				}
			],
			showStepNumbers: false,
			exitOnOverlayClick: false,
			exitOnEsc: true,
			nextLabel: '<strong>Next <i class="fa fa-arrow-right"></i></strong>',
			prevLabel: '<span><i class="fa fa-arrow-left"></i> Back</span>',
			skipLabel: 'Exit Tour',
			doneLabel: 'Close'
		};
	}, 3333);
	
	
}]);