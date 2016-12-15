/**
* WorldWide Telescope Web Client
* Copyright 2014-2015 WorldWide Telescope
* Licensed under MIT (https://github.com/WorldWideTelescope/wwt-web-client/blob/master/LICENSE.md)
**/
/**
 * Intro.js v0.8.0
 * https://github.com/usablica/intro.js
 * MIT licensed
 *
 * Copyright (C) 2013 usabli.ca - A weekend project by Afshin Mehrabani (@afshinmeh)
 */

(function (root, factory) {
  if (typeof exports === 'object') {
    // CommonJS
    factory(exports);
  } else if (typeof define === 'function' && define.amd) {
    // AMD. Register as an anonymous module.
    define(['exports'], factory);
  } else {
    // Browser globals
    factory(root);
  }
} (this, function (exports) {
  //Default config/variables
  var VERSION = '0.8.0';

  /**
   * IntroJs main class
   *
   * @class IntroJs
   */
  function IntroJs(obj) {
    this._targetElement = obj;

    this._options = {
      /* Next button label in tooltip box */
      nextLabel: 'Next &rarr;',
      /* Previous button label in tooltip box */
      prevLabel: '&larr; Back',
      /* Skip button label in tooltip box */
      skipLabel: 'Skip',
      /* Done button label in tooltip box */
      doneLabel: 'Done',
      /* Default tooltip box position */
      tooltipPosition: 'bottom',
      /* Next CSS class for tooltip boxes */
      tooltipClass: '',
      /* Close introduction when pressing Escape button? */
      exitOnEsc: true,
      /* Close introduction when clicking on overlay layer? */
      exitOnOverlayClick: true,
      /* Show step numbers in introduction? */
      showStepNumbers: true,
      /* Let user use keyboard to navigate the tour? */
      keyboardNavigation: true,
      /* Show tour control buttons? */
      showButtons: true,
      /* Show tour bullets? */
      showBullets: true,
      /* Scroll to highlighted element? */
      scrollToElement: true
    };
  }

  /**
   * Initiate a new introduction/guide from an element in the page
   *
   * @api private
   * @method _introForElement
   * @param {Object} targetElm
   * @returns {Boolean} Success or not?
   */
  function _introForElement(targetElm) {
    var introItems = [],
        self = this;

    if (this._options.steps) {
      //use steps passed programmatically
      var allIntroSteps = [];

      for (var i = 0, stepsLength = this._options.steps.length; i < stepsLength; i++) {
        var currentItem = _cloneObject(this._options.steps[i]);
        //set the step
        currentItem.step = introItems.length + 1;
        //use querySelector function only when developer used CSS selector
        if (typeof(currentItem.element) === 'string') {
          //grab the element with given selector from the page
          currentItem.element = document.querySelector(currentItem.element);
        }

        //intro without element
        if (typeof(currentItem.element) === 'undefined' || currentItem.element == null) {
          var floatingElementQuery = document.querySelector(".introjsFloatingElement");

          if (floatingElementQuery == null) {
            floatingElementQuery = document.createElement('div');
            floatingElementQuery.className = 'introjsFloatingElement';

            document.body.appendChild(floatingElementQuery);
          }

          currentItem.element  = floatingElementQuery;
          currentItem.position = 'floating';
        }

        if (currentItem.element != null) {
          introItems.push(currentItem);
        }
      }

    } else {
       //use steps from data-* annotations
      var allIntroSteps = targetElm.querySelectorAll('*[data-intro]');
      //if there's no element to intro
      if (allIntroSteps.length < 1) {
        return false;
      }

      //first add intro items with data-step
      for (var i = 0, elmsLength = allIntroSteps.length; i < elmsLength; i++) {
        var currentElement = allIntroSteps[i];
        var step = parseInt(currentElement.getAttribute('data-step'), 10);

        if (step > 0) {
          introItems[step - 1] = {
            element: currentElement,
            intro: currentElement.getAttribute('data-intro'),
            step: parseInt(currentElement.getAttribute('data-step'), 10),
            tooltipClass: currentElement.getAttribute('data-tooltipClass'),
            position: currentElement.getAttribute('data-position') || this._options.tooltipPosition
          };
        }
      }

      //next add intro items without data-step
      //todo: we need a cleanup here, two loops are redundant
      var nextStep = 0;
      for (var i = 0, elmsLength = allIntroSteps.length; i < elmsLength; i++) {
        var currentElement = allIntroSteps[i];

        if (currentElement.getAttribute('data-step') == null) {

          while (true) {
            if (typeof introItems[nextStep] == 'undefined') {
              break;
            } else {
              nextStep++;
            }
          }

          introItems[nextStep] = {
            element: currentElement,
            intro: currentElement.getAttribute('data-intro'),
            step: nextStep + 1,
            tooltipClass: currentElement.getAttribute('data-tooltipClass'),
            position: currentElement.getAttribute('data-position') || this._options.tooltipPosition
          };
        }
      }
    }

    //removing undefined/null elements
    var tempIntroItems = [];
    for (var z = 0; z < introItems.length; z++) {
      introItems[z] && tempIntroItems.push(introItems[z]);  // copy non-empty values to the end of the array
    }

    introItems = tempIntroItems;

    //Ok, sort all items with given steps
    introItems.sort(function (a, b) {
      return a.step - b.step;
    });

    //set it to the introJs object
    self._introItems = introItems;

    //add overlay layer to the page
    if(_addOverlayLayer.call(self, targetElm)) {
      //then, start the show
      _nextStep.call(self);

      var skipButton     = targetElm.querySelector('.introjs-skipbutton'),
          nextStepButton = targetElm.querySelector('.introjs-nextbutton');

      self._onKeyDown = function(e) {
        if (e.keyCode === 27 && self._options.exitOnEsc == true) {
          //escape key pressed, exit the intro
          _exitIntro.call(self, targetElm);
          //check if any callback is defined
          if (self._introExitCallback != undefined) {
            self._introExitCallback.call(self);
          }
        } else if(e.keyCode === 37) {
          //left arrow
          _previousStep.call(self);
        } else if (e.keyCode === 39 || e.keyCode === 13) {
          //right arrow or enter
          _nextStep.call(self);
          //prevent default behaviour on hitting Enter, to prevent steps being skipped in some browsers
          if(e.preventDefault) {
            e.preventDefault();
          } else {
            e.returnValue = false;
          }
        }
      };

      self._onResize = function(e) {
        _setHelperLayerPosition.call(self, document.querySelector('.introjs-helperLayer'));
      };

      if (window.addEventListener) {
        if (this._options.keyboardNavigation) {
          window.addEventListener('keydown', self._onKeyDown, true);
        }
        //for window resize
        window.addEventListener("resize", self._onResize, true);
      } else if (document.attachEvent) { //IE
        if (this._options.keyboardNavigation) {
          document.attachEvent('onkeydown', self._onKeyDown);
        }
        //for window resize
        document.attachEvent("onresize", self._onResize);
      }
    }
    return false;
  }

 /*
   * makes a copy of the object
   * @api private
   * @method _cloneObject
  */
  function _cloneObject(object) {
      if (object == null || typeof (object) != 'object' || typeof (object.nodeType) != 'undefined') {
          return object;
      }
      var temp = {};
      for (var key in object) {
          temp[key] = _cloneObject(object[key]);
      }
      return temp;
  }
  /**
   * Go to specific step of introduction
   *
   * @api private
   * @method _goToStep
   */
  function _goToStep(step) {
    //because steps starts with zero
    this._currentStep = step - 2;
    if (typeof (this._introItems) !== 'undefined') {
      _nextStep.call(this);
    }
  }

  /**
   * Go to next step on intro
   *
   * @api private
   * @method _nextStep
   */
  function _nextStep() {
      if ($('.introjs-nextbutton').prop('disabled'))return;
    this._direction = 'forward';

    if (typeof (this._currentStep) === 'undefined') {
      this._currentStep = 0;
    } else {
      ++this._currentStep;
    }

    if ((this._introItems.length) <= this._currentStep) {
      //end of the intro
      //check if any callback is defined
      if (typeof (this._introCompleteCallback) === 'function') {
        this._introCompleteCallback.call(this);
      }
      _exitIntro.call(this, this._targetElement);
      return;
    }

    var nextStep = this._introItems[this._currentStep];
    if (typeof (this._introBeforeChangeCallback) !== 'undefined') {
      this._introBeforeChangeCallback.call(this, nextStep.element);
    }

    _showElement.call(this, nextStep);
  }

  /**
   * Go to previous step on intro
   *
   * @api private
   * @method _nextStep
   */
  function _previousStep() {
      return;
    this._direction = 'backward';
    
    if (this._currentStep === 0) {
      return false;
    }

    var nextStep = this._introItems[--this._currentStep];
    if (typeof (this._introBeforeChangeCallback) !== 'undefined') {
      this._introBeforeChangeCallback.call(this, nextStep.element);
    }

    _showElement.call(this, nextStep);
  }

  /**
   * Exit from intro
   *
   * @api private
   * @method _exitIntro
   * @param {Object} targetElement
   */
  function _exitIntro(targetElement) {
    //remove overlay layer from the page
    var overlayLayer = targetElement.querySelector('.introjs-overlay');
    
    //return if intro already completed or skipped
    if (overlayLayer == null) {
      return;
    }

    //for fade-out animation
    overlayLayer.style.opacity = 0;
    setTimeout(function () {
      if (overlayLayer.parentNode) {
        overlayLayer.parentNode.removeChild(overlayLayer);
      }
    }, 500);

    //remove all helper layers
    var helperLayer = targetElement.querySelector('.introjs-helperLayer');
    if (helperLayer) {
      helperLayer.parentNode.removeChild(helperLayer);
    }

    //remove intro floating element
    var floatingElement = document.querySelector('.introjsFloatingElement');
    if (floatingElement) {
      floatingElement.parentNode.removeChild(floatingElement);
    }

    //remove `introjs-showElement` class from the element
    var showElement = document.querySelector('.introjs-showElement');
    if (showElement) {
      showElement.className = showElement.className.replace(/introjs-[a-zA-Z]+/g, '').replace(/^\s+|\s+$/g, ''); // This is a manual trim.
    }

    //remove `introjs-fixParent` class from the elements
    var fixParents = document.querySelectorAll('.introjs-fixParent');
    if (fixParents && fixParents.length > 0) {
      for (var i = fixParents.length - 1; i >= 0; i--) {
        fixParents[i].className = fixParents[i].className.replace(/introjs-fixParent/g, '').replace(/^\s+|\s+$/g, '');
      };
    }

    //clean listeners
    if (window.removeEventListener) {
      window.removeEventListener('keydown', this._onKeyDown, true);
    } else if (document.detachEvent) { //IE
      document.detachEvent('onkeydown', this._onKeyDown);
    }
    
    //set the step to zero
    this._currentStep = undefined;
  }

  /**
   * Render tooltip box in the page
   *
   * @api private
   * @method _placeTooltip
   * @param {Object} targetElement
   * @param {Object} tooltipLayer
   * @param {Object} arrowLayer
   */
  function _placeTooltip(targetElement, tooltipLayer, arrowLayer, helperNumberLayer) {
    //reset the old style
    tooltipLayer.style.top        = null;
    tooltipLayer.style.right      = null;
    tooltipLayer.style.bottom     = null;
    tooltipLayer.style.left       = null;
    tooltipLayer.style.marginLeft = null;
    tooltipLayer.style.marginTop  = null;

    arrowLayer.style.display = 'inherit';

    if (typeof(helperNumberLayer) != 'undefined' && helperNumberLayer != null) {
      helperNumberLayer.style.top  = null;
      helperNumberLayer.style.left = null;
    }

    //prevent error when `this._currentStep` is undefined
    if (!this._introItems[this._currentStep]) return;

    var tooltipCssClass = '';

    //if we have a custom css class for each step
    var currentStepObj = this._introItems[this._currentStep];
    if (typeof (currentStepObj.tooltipClass) === 'string') {
      tooltipCssClass = currentStepObj.tooltipClass;
    } else {
      tooltipCssClass = this._options.tooltipClass;
    }

    tooltipLayer.className = ('introjs-tooltip ' + tooltipCssClass).replace(/^\s+|\s+$/g, '');

    //custom css class for tooltip boxes
    var tooltipCssClass = this._options.tooltipClass;

    var currentTooltipPosition = this._introItems[this._currentStep].position;
    switch (currentTooltipPosition) {
      case 'top':
        tooltipLayer.style.left = '15px';
        tooltipLayer.style.top = '-' + (_getOffset(tooltipLayer).height + 10) + 'px';
        arrowLayer.className = 'introjs-arrow bottom';
        break;
      case 'right':
        tooltipLayer.style.left = (_getOffset(targetElement).width + 20) + 'px';
        arrowLayer.className = 'introjs-arrow left';
        break;
      case 'left':
        if (this._options.showStepNumbers == true) {  
          tooltipLayer.style.top = '15px';
        }
        tooltipLayer.style.right = (_getOffset(targetElement).width + 20) + 'px';
        arrowLayer.className = 'introjs-arrow right';
        break;
      case 'floating':
        arrowLayer.style.display = 'none';

        //we have to adjust the top and left of layer manually for intro items without element{
        var tooltipOffset = _getOffset(tooltipLayer);

        tooltipLayer.style.left   = '50%';
        tooltipLayer.style.top    = '50%';
        tooltipLayer.style.marginLeft = '-' + (tooltipOffset.width / 2)  + 'px';
        tooltipLayer.style.marginTop  = '-' + (tooltipOffset.height / 2) + 'px';

        if (typeof(helperNumberLayer) != 'undefined' && helperNumberLayer != null) {
          helperNumberLayer.style.left = '-' + ((tooltipOffset.width / 2) + 18) + 'px';
          helperNumberLayer.style.top  = '-' + ((tooltipOffset.height / 2) + 18) + 'px';
        }

        break;
      case 'bottom':
      // Bottom going to follow the default behavior
      default:
        tooltipLayer.style.bottom = '-' + (_getOffset(tooltipLayer).height + 10) + 'px';
        arrowLayer.className = 'introjs-arrow top';
        break;
    }
  }

  /**
   * Update the position of the helper layer on the screen
   *
   * @api private
   * @method _setHelperLayerPosition
   * @param {Object} helperLayer
   */
  function _setHelperLayerPosition(helperLayer) {
    if (helperLayer) {
      //prevent error when `this._currentStep` in undefined
      if (!this._introItems[this._currentStep]) return;

      var currentElement  = this._introItems[this._currentStep];
      var elementPosition = _getOffset(currentElement.element);

      var widthHeightPadding = 10;
      if (currentElement.position == 'floating') {
        widthHeightPadding = 0;
      }

      //set new position to helper layer
      helperLayer.setAttribute('style', 'width: ' + (elementPosition.width  + widthHeightPadding)  + 'px; ' +
                                        'height:' + (elementPosition.height + widthHeightPadding)  + 'px; ' +
                                        'top:'    + (elementPosition.top    - 5)   + 'px;' +
                                        'left: '  + (Math.max(elementPosition.left - 5, 0))  + 'px;');
    }
  }

  /**
   * Show an element on the page
   *
   * @api private
   * @method _showElement
   * @param {Object} targetElement
   */
  function _showElement(targetElement) {

    if (typeof (this._introChangeCallback) !== 'undefined') {
        this._introChangeCallback.call(this, targetElement.element);
    }

    var self = this,
        oldHelperLayer = document.querySelector('.introjs-helperLayer'),
        elementPosition = _getOffset(targetElement.element);

    if (oldHelperLayer != null) {
      var oldHelperNumberLayer = oldHelperLayer.querySelector('.introjs-helperNumberLayer'),
          oldtooltipLayer      = oldHelperLayer.querySelector('.introjs-tooltiptext'),
          oldArrowLayer        = oldHelperLayer.querySelector('.introjs-arrow'),
          oldtooltipContainer  = oldHelperLayer.querySelector('.introjs-tooltip'),
          skipTooltipButton    = oldHelperLayer.querySelector('.introjs-skipbutton'),
          prevTooltipButton    = oldHelperLayer.querySelector('.introjs-prevbutton'),
          nextTooltipButton    = oldHelperLayer.querySelector('.introjs-nextbutton');

      //hide the tooltip
      oldtooltipContainer.style.opacity = 0;

      if (oldHelperNumberLayer != null) {
        var lastIntroItem = this._introItems[(targetElement.step - 2 >= 0 ? targetElement.step - 2 : 0)];

        if (lastIntroItem != null && (this._direction == 'forward' && lastIntroItem.position == 'floating') || (this._direction == 'backward' && targetElement.position == 'floating')) {
          oldHelperNumberLayer.style.opacity = 0;
        }
      }

      //set new position to helper layer
      _setHelperLayerPosition.call(self, oldHelperLayer);

      //remove `introjs-fixParent` class from the elements
      var fixParents = document.querySelectorAll('.introjs-fixParent');
      if (fixParents && fixParents.length > 0) {
        for (var i = fixParents.length - 1; i >= 0; i--) {
          fixParents[i].className = fixParents[i].className.replace(/introjs-fixParent/g, '').replace(/^\s+|\s+$/g, '');
        };
      }

      //remove old classes
      var oldShowElement = document.querySelector('.introjs-showElement');
      oldShowElement.className = oldShowElement.className.replace(/introjs-[a-zA-Z]+/g, '').replace(/^\s+|\s+$/g, '');
      //we should wait until the CSS3 transition is competed (it's 0.3 sec) to prevent incorrect `height` and `width` calculation
      if (self._lastShowElementTimer) {
        clearTimeout(self._lastShowElementTimer);
      }
      self._lastShowElementTimer = setTimeout(function() {
        //set current step to the label
        if (oldHelperNumberLayer != null) {
          oldHelperNumberLayer.innerHTML = targetElement.step;
        }
        //set current tooltip text
        oldtooltipLayer.innerHTML = targetElement.intro;
        //set the tooltip position
        _placeTooltip.call(self, targetElement.element, oldtooltipContainer, oldArrowLayer, oldHelperNumberLayer);

        //change active bullet
        oldHelperLayer.querySelector('.introjs-bullets li > a.active').className = '';
        oldHelperLayer.querySelector('.introjs-bullets li > a[data-stepnumber="' + targetElement.step + '"]').className = 'active';

        //show the tooltip
        oldtooltipContainer.style.opacity = 1;
          if (oldHelperNumberLayer) {
              oldHelperNumberLayer.style.opacity = 1;
          }
      }, 350);

    } else {
      var helperLayer       = document.createElement('div'),
          arrowLayer        = document.createElement('div'),
          tooltipLayer      = document.createElement('div'),
          tooltipTextLayer  = document.createElement('div'),
          bulletsLayer      = document.createElement('div'),
          buttonsLayer      = document.createElement('div');

      helperLayer.className = 'introjs-helperLayer';

      //set new position to helper layer
      _setHelperLayerPosition.call(self, helperLayer);

      //add helper layer to target element
      this._targetElement.appendChild(helperLayer);

      arrowLayer.className = 'introjs-arrow';

      tooltipTextLayer.className = 'introjs-tooltiptext';
      tooltipTextLayer.innerHTML = targetElement.intro;

      bulletsLayer.className = 'introjs-bullets';

      if (this._options.showBullets === false) {
        bulletsLayer.style.display = 'none';
      }

      var ulContainer = document.createElement('ul');

      for (var i = 0, stepsLength = this._introItems.length; i < stepsLength; i++) {
        var innerLi    = document.createElement('li');
        var anchorLink = document.createElement('a');

        anchorLink.onclick = function() {
          self.goToStep(this.getAttribute('data-stepnumber'));
        };

        if (i === 0) anchorLink.className = "active";

        anchorLink.href = 'javascript:void(0);';
        anchorLink.innerHTML = "&nbsp;";
        anchorLink.setAttribute('data-stepnumber', this._introItems[i].step);

        innerLi.appendChild(anchorLink);
        ulContainer.appendChild(innerLi);
      }

      bulletsLayer.appendChild(ulContainer);

      buttonsLayer.className = 'introjs-tooltipbuttons';
      if (this._options.showButtons === false) {
        buttonsLayer.style.display = 'none';
      }

      tooltipLayer.className = 'introjs-tooltip';
      tooltipLayer.appendChild(tooltipTextLayer);
      tooltipLayer.appendChild(bulletsLayer);

      //add helper layer number
      if (this._options.showStepNumbers == true) {
        var helperNumberLayer = document.createElement('span');
        helperNumberLayer.className = 'introjs-helperNumberLayer';
        helperNumberLayer.innerHTML = targetElement.step;
        helperLayer.appendChild(helperNumberLayer);
      }
      tooltipLayer.appendChild(arrowLayer);
      helperLayer.appendChild(tooltipLayer);

      //next button
      var nextTooltipButton = document.createElement('a');

      nextTooltipButton.onclick = function() {
          if (self._introItems.length - 1 != self._currentStep) {

          _nextStep.call(self);
        }
      };

      nextTooltipButton.href = 'javascript:void(0);';
      nextTooltipButton.innerHTML = this._options.nextLabel;

      //previous button
      var prevTooltipButton = document.createElement('a');

      prevTooltipButton.onclick = function() {
        if (self._currentStep != 0) {
          _previousStep.call(self);
        }
      };

      prevTooltipButton.href = 'javascript:void(0);';
      prevTooltipButton.innerHTML = this._options.prevLabel;

      //skip button
      var skipTooltipButton = document.createElement('a');
      skipTooltipButton.className = 'introjs-button introjs-skipbutton';
      skipTooltipButton.href = 'javascript:void(0);';
      skipTooltipButton.innerHTML = this._options.skipLabel;

      skipTooltipButton.onclick = function() {
        if (self._introItems.length - 1 == self._currentStep && typeof (self._introCompleteCallback) === 'function') {
          self._introCompleteCallback.call(self);
        }

        if (self._introItems.length - 1 != self._currentStep && typeof (self._introExitCallback) === 'function') {
          self._introExitCallback.call(self);
        }

        _exitIntro.call(self, self._targetElement);
      };

      buttonsLayer.appendChild(skipTooltipButton);

      //in order to prevent displaying next/previous button always
      if (this._introItems.length > 1) {
        buttonsLayer.appendChild(prevTooltipButton);
        buttonsLayer.appendChild(nextTooltipButton);
      }

      tooltipLayer.appendChild(buttonsLayer);

      //set proper position
      _placeTooltip.call(self, targetElement.element, tooltipLayer, arrowLayer, helperNumberLayer);
    }

    if (this._currentStep == 0 && this._introItems.length > 1) {
      prevTooltipButton.className = 'introjs-button introjs-prevbutton introjs-disabled';
      nextTooltipButton.className = 'introjs-button introjs-nextbutton';
      skipTooltipButton.innerHTML = this._options.skipLabel;
    } else if (this._introItems.length - 1 == this._currentStep || this._introItems.length == 1) {
      skipTooltipButton.innerHTML = this._options.doneLabel;
      prevTooltipButton.className = 'introjs-button introjs-prevbutton';
      nextTooltipButton.className = 'introjs-button introjs-nextbutton introjs-disabled';
    } else {
      prevTooltipButton.className = 'introjs-button introjs-prevbutton';
      nextTooltipButton.className = 'introjs-button introjs-nextbutton';
      skipTooltipButton.innerHTML = this._options.skipLabel;
    }

    //Set focus on "next" button, so that hitting Enter always moves you onto the next step
    nextTooltipButton.focus();

    //add target element position style
    targetElement.element.className += ' introjs-showElement';

    var currentElementPosition = _getPropValue(targetElement.element, 'position');
    if (currentElementPosition !== 'absolute' &&
        currentElementPosition !== 'relative') {
      //change to new intro item
      targetElement.element.className += ' introjs-relativePosition';
    }

    var parentElm = targetElement.element.parentNode;
    while (parentElm != null) {
      if (parentElm.tagName.toLowerCase() === 'body') break;

      //fix The Stacking Contenxt problem. 
      //More detail: https://developer.mozilla.org/en-US/docs/Web/Guide/CSS/Understanding_z_index/The_stacking_context
      var zIndex = _getPropValue(parentElm, 'z-index');
      var opacity = parseFloat(_getPropValue(parentElm, 'opacity'));
      if (/[0-9]+/.test(zIndex) || opacity < 1) {
        parentElm.className += ' introjs-fixParent';
      }
    
      parentElm = parentElm.parentNode;
    }

    if (!_elementInViewport(targetElement.element) && this._options.scrollToElement === true) {
      var rect = targetElement.element.getBoundingClientRect(),
        winHeight=_getWinSize().height,
        top = rect.bottom - (rect.bottom - rect.top),
        bottom = rect.bottom - winHeight;

      //Scroll up
      if (top < 0 || targetElement.element.clientHeight > winHeight) {
        window.scrollBy(0, top - 30); // 30px padding from edge to look nice

      //Scroll down
      } else {
        window.scrollBy(0, bottom + 100); // 70px + 30px padding from edge to look nice
      }
    }
    
    if (typeof (this._introAfterChangeCallback) !== 'undefined') {
        this._introAfterChangeCallback.call(this, targetElement.element);
    }
  }

  /**
   * Get an element CSS property on the page
   * Thanks to JavaScript Kit: http://www.javascriptkit.com/dhtmltutors/dhtmlcascade4.shtml
   *
   * @api private
   * @method _getPropValue
   * @param {Object} element
   * @param {String} propName
   * @returns Element's property value
   */
  function _getPropValue (element, propName) {
    var propValue = '';
    if (element.currentStyle) { //IE
      propValue = element.currentStyle[propName];
    } else if (document.defaultView && document.defaultView.getComputedStyle) { //Others
      propValue = document.defaultView.getComputedStyle(element, null).getPropertyValue(propName);
    }

    //Prevent exception in IE
    if (propValue && propValue.toLowerCase) {
      return propValue.toLowerCase();
    } else {
      return propValue;
    }
  }

  /**
   * Provides a cross-browser way to get the screen dimensions
   * via: http://stackoverflow.com/questions/5864467/internet-explorer-innerheight
   *
   * @api private
   * @method _getWinSize
   * @returns {Object} width and height attributes
   */
  function _getWinSize() {
    if (window.innerWidth != undefined) {
      return { width: window.innerWidth, height: window.innerHeight };
    } else {
      var D = document.documentElement;
      return { width: D.clientWidth, height: D.clientHeight };
    }
  }

  /**
   * Add overlay layer to the page
   * http://stackoverflow.com/questions/123999/how-to-tell-if-a-dom-element-is-visible-in-the-current-viewport
   *
   * @api private
   * @method _elementInViewport
   * @param {Object} el
   */
  function _elementInViewport(el) {
    var rect = el.getBoundingClientRect();

    return (
      rect.top >= 0 &&
      rect.left >= 0 &&
      (rect.bottom+80) <= window.innerHeight && // add 80 to get the text right
      rect.right <= window.innerWidth
    );
  }

  /**
   * Add overlay layer to the page
   *
   * @api private
   * @method _addOverlayLayer
   * @param {Object} targetElm
   */
  function _addOverlayLayer(targetElm) {
    var overlayLayer = document.createElement('div'),
        styleText = '',
        self = this;

    //set css class name
    overlayLayer.className = 'introjs-overlay';

    //check if the target element is body, we should calculate the size of overlay layer in a better way
    if (targetElm.tagName.toLowerCase() === 'body') {
      styleText += 'top: 0;bottom: 0; left: 0;right: 0;position: fixed;';
      overlayLayer.setAttribute('style', styleText);
    } else {
      //set overlay layer position
      var elementPosition = _getOffset(targetElm);
      if (elementPosition) {
        styleText += 'width: ' + elementPosition.width + 'px; height:' + elementPosition.height + 'px; top:' + elementPosition.top + 'px;left: ' + Math.max(0,elementPosition.left) + 'px;';
        overlayLayer.setAttribute('style', styleText);
      }
    }

    targetElm.appendChild(overlayLayer);

    overlayLayer.onclick = function() {
      if (self._options.exitOnOverlayClick == true) {
        _exitIntro.call(self, targetElm);

        //check if any callback is defined
        if (self._introExitCallback != undefined) {
          self._introExitCallback.call(self);
        }
      }
    };

    setTimeout(function() {
      styleText += 'opacity: .8;';
      overlayLayer.setAttribute('style', styleText);
    }, 10);
    return true;
  }

  /**
   * Get an element position on the page
   * Thanks to `meouw`: http://stackoverflow.com/a/442474/375966
   *
   * @api private
   * @method _getOffset
   * @param {Object} element
   * @returns Element's position info
   */
  function _getOffset(element) {
    var elementPosition = {};

    //set width
    elementPosition.width = element.offsetWidth;

    //set height
    elementPosition.height = element.offsetHeight;

    //calculate element top and left
    var _x = 0;
    var _y = 0;
    while (element && !isNaN(element.offsetLeft) && !isNaN(element.offsetTop)) {
      _x += element.offsetLeft;
      _y += element.offsetTop;
      element = element.offsetParent;
    }
    //set top
    elementPosition.top = _y;
    //set left
    elementPosition.left = _x;

    return elementPosition;
  }

  /**
   * Overwrites obj1's values with obj2's and adds obj2's if non existent in obj1
   * via: http://stackoverflow.com/questions/171251/how-can-i-merge-properties-of-two-javascript-objects-dynamically
   *
   * @param obj1
   * @param obj2
   * @returns obj3 a new object based on obj1 and obj2
   */
  function _mergeOptions(obj1,obj2) {
    var obj3 = {};
    for (var attrname in obj1) { obj3[attrname] = obj1[attrname]; }
    for (var attrname in obj2) { obj3[attrname] = obj2[attrname]; }
    return obj3;
  }

  var introJs = function (targetElm) {
    if (typeof (targetElm) === 'object') {
      //Ok, create a new instance
      return new IntroJs(targetElm);

    } else if (typeof (targetElm) === 'string') {
      //select the target element with query selector
      var targetElement = document.querySelector(targetElm);

      if (targetElement) {
        return new IntroJs(targetElement);
      } else {
        throw new Error('There is no element with given selector.');
      }
    } else {
      return new IntroJs(document.body);
    }
  };

  /**
   * Current IntroJs version
   *
   * @property version
   * @type String
   */
  introJs.version = VERSION;

  //Prototype
  introJs.fn = IntroJs.prototype = {
    clone: function () {
      return new IntroJs(this);
    },
    setOption: function(option, value) {
      this._options[option] = value;
      return this;
    },
    setOptions: function(options) {
      this._options = _mergeOptions(this._options, options);
      return this;
    },
    start: function () {
      _introForElement.call(this, this._targetElement);
      return this;
    },
    goToStep: function(step) {
      _goToStep.call(this, step);
      return this;
    },
    nextStep: function() {
      _nextStep.call(this);
      return this;
    },
    previousStep: function() {
      _previousStep.call(this);
      return this;
    },
    exit: function() {
      _exitIntro.call(this, this._targetElement);
    },
    refresh: function() {
      _setHelperLayerPosition.call(this, document.querySelector('.introjs-helperLayer'));
      return this;
    },
    onbeforechange: function(providedCallback) {
      if (typeof (providedCallback) === 'function') {
        this._introBeforeChangeCallback = providedCallback;
      } else {
        throw new Error('Provided callback for onbeforechange was not a function');
      }
      return this;
    },
    onchange: function(providedCallback) {
      if (typeof (providedCallback) === 'function') {
        this._introChangeCallback = providedCallback;
      } else {
        throw new Error('Provided callback for onchange was not a function.');
      }
      return this;
    },
    onafterchange: function(providedCallback) {
      if (typeof (providedCallback) === 'function') {
        this._introAfterChangeCallback = providedCallback;
      } else {
        throw new Error('Provided callback for onafterchange was not a function');
      }
      return this;
    },
    oncomplete: function(providedCallback) {
      if (typeof (providedCallback) === 'function') {
        this._introCompleteCallback = providedCallback;
      } else {
        throw new Error('Provided callback for oncomplete was not a function.');
      }
      return this;
    },
    onexit: function(providedCallback) {
      if (typeof (providedCallback) === 'function') {
        this._introExitCallback = providedCallback;
      } else {
        throw new Error('Provided callback for onexit was not a function.');
      }
      return this;
    }
  };

  exports.introJs = introJs;
  return introJs;
}));

var ngIntroDirective = angular.module('angular-intro', []);

/**
* TODO: Use isolate scope, but requires angular 1.2: http://plnkr.co/edit/a2c14O?p=preview
* See: http://stackoverflow.com/q/18796023/237209
*/

ngIntroDirective.directive('ngIntroOptions', ['$timeout', '$parse', function ($timeout, $parse) {

    return {
        restrict: 'A',
        link: function (scope, element, attrs) {

            scope[attrs.ngIntroMethod] = function (step) {

                var intro;

                if (typeof (step) === 'string') {
                    intro = introJs(step);
                } else {
                    intro = introJs();
                }
                //setTimeout(function() {
                    intro.setOptions(scope.$eval(attrs.ngIntroOptions));
                    if (attrs.ngIntroOncomplete) {
                        intro.oncomplete($parse(attrs.ngIntroOncomplete)(scope));
                    }

                    if (attrs.ngIntroOnexit) {
                        intro.onexit($parse(attrs.ngIntroOnexit)(scope));
                    }

                    if (attrs.ngIntroOnchange) {
                        intro.onchange($parse(attrs.ngIntroOnchange)(scope));
                    }

                    if (attrs.ngIntroOnbeforechange) {
                        intro.onbeforechange($parse(attrs.ngIntroOnbeforechange)(scope));
                    }

                    if (attrs.ngIntroOnafterchange) {
                        intro.onafterchange($parse(attrs.ngIntroOnafterchange)(scope));
                    }

                    if (typeof (step) === 'number') {
                        intro.goToStep(step).start();
                    } else {
                        intro.start();
                    }
                //}, 1);
                
            };

            if (attrs.ngIntroAutostart == 'true') {
                $timeout(function () {
                    $parse(attrs.ngIntroMethod)(scope)();
                });
            }
        }
    };
}]);
/** 
* Copyright 2014, 2015  Microsoft Research
*
* Permission is hereby granted, free of charge, to any person
* obtaining a copy of this software and associated documentation
* files (the "Software"), to deal in the Software without
* restriction, including without limitation the rights to use, copy,
* modify, merge, publish, distribute, sublicense, and/or sell copies
* of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be
* included in all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
* NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
* BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
* ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
* CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
**/
var wwt = { 
	app: angular.module('wwtApp', [
		'mgcrea.ngStrap',
		'ngTouch',  
		'ngAnimate', 
		'ngRoute',
		'wwtControllers',
		'ngCookies',
		'angular-intro'
	]),
	controllers: angular.module('wwtControllers', []), 
	triggerResize: function () { },
	resize: function () {
		$('body.mobile #WWTCanvas')
			.height($('#WorldWideTelescopeControlHost').height())
			.width($('#WorldWideTelescopeControlHost').width());
		$('body.desktop #WWTCanvas')
			.height($(window).height())
			.width($(window).width());	
	}
};



$(window).on('load', function() {
	wwt.resize();
	//load search data after everything else
	var scr = document.createElement('script');
	scr.setAttribute("src", 'searchdata.min.js');
	document.getElementsByTagName("head")[0].appendChild(scr);
});

wwt.app.directive("scrollBuffer", ['$window',function ($window) {
	return function ($scope, element, attrs) {
		var buffer = parseInt(attrs.scrollBuffer);
		var scope = $scope;
		var parent = $(element).parent();
		while (!parent.hasClass('modal-dialog')) parent = parent.parent();
		parent.on('scroll', function () {
			var st = this.scrollTop;
			var e = $(this);
			var w = e.width();
			var h = e.height();
			var tn = e.find('.modal-content div.tn').first();
			var cols = Math.floor(w / tn.width());
			var rowsAboveFold = Math.ceil((st + h) / tn.height());
			var totalItems = (rowsAboveFold + buffer) * cols;
			if (scope.scrollDepth < totalItems) {
				scope.scrollDepth = totalItems;
				scope.$apply();
				
			}
			
		});
		
	};
}]);

wwt.app.directive("jqueryScrollbar", ['$rootScope','$window', function ($rootScope,$window) {
    return function ($scope, element, attrs) {
        
        var scope = $scope;
        var movable = $(element).find('.jspPane');
        $(element).on('mousewheel', function (event) {
            var e = event.originalEvent;
            movable = $(element).find('.jspPane');
            var delta = Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)));
            var curLeft = Math.abs(Math.floor(movable.position().left));
            var increment = 155;
            var newLeft;            
            if (delta < 0) {//scrolling down/right
                console.log('down')
                newLeft = Math.floor((curLeft + increment) / increment) * increment;
                newLeft = Math.max(curLeft, Math.abs(newLeft));
            } else {//scrolling up/left
                console.log('up')
                newLeft = Math.floor((curLeft - increment) / increment) * increment;
                newLeft = Math.min(curLeft, Math.abs(newLeft));
            }
            
            $(element).data('jsp').scrollToX(Math.abs(newLeft));
        })

    };
}
])
wwt.app.directive("localize", ['Localization', '$rootScope', 'AppState','Util', function (loc, $rootScope,appState,util) {
	return function ($scope, element, attrs) {
		if (appState.get('language') !== 'EN') {
			//if ($rootScope.languagePromise) {
				$rootScope.languagePromise.then(function() {
					replaceText(false);
				});
			//} 
		} else {
			replaceText(true);
		}

		function replaceText(useEn) {
			try {
				// possible binding expression needs to be eval-ed
				if (attrs.localize === '') {
					setTimeout(function () { replaceText(useEn) }, 200);
					return;
				}
				var el = $(element);
				var exp = new RegExp(attrs.localize, 'g');
				var localized = useEn ? attrs.localize : loc.getFromEn(attrs.localize);
				if (!attrs.locAttrOnly && !attrs.localizeOnly) {
					if (el.html().indexOf(attrs.localize) != -1 && !useEn) {
						el.html(el.html().replace(exp, localized));
					} else {
						el.html(localized);
					}
				}
				if (attrs.localizeAttr || attrs.localizeOnly) {
					var attrib = attrs.localizeAttr || attrs.localizeOnly;
					if (el.attr(attrib) && el.attr(attrib).indexOf(localized) != -1 && !useEn) {
						el.attr(attrib, el.attr(attrib).replace(exp, localized));
					} else {
						el.attr(attrib, localized);
					}
				}
			} catch (er) {
				util.log('localize', er);
			}
		}

	};
}]);
wwt.app.directive('ngContextMenu', ['$dropdown', function ($dropdown) {
    return {
        restrict: 'A',
        scope: { method: '&ngContextMenu' },
        link: function (scope, element){
            var handler = scope.method();
            element.bind('contextmenu', function (event) {
                event.preventDefault();
                var index = event.delegateTarget.getAttribute('index');
                if (index) {
                    handler(parseInt(index),event);
                } else if  (handler) {
                    handler(event);
                }
            });
        }
    };
}]);

wwt.app.directive('ngRightClick', function ($parse) {
    return function (scope, element, attrs) {
        var fn = $parse(attrs.ngRightClick);
        element.bind('contextmenu', function (event) {
            scope.$apply(function () {
                event.preventDefault();
                fn(scope, { $event: event });
            });
        });
    };
});


wwt.app.directive('contenteditable', [function() {
    return {
        restrict: 'A',
        link: function(scope, element, attrs) {
            var isDuration = (element.hasClass('duration'));
            var hasFocused = false;
            var isLabel = !isDuration;
            var stop = scope.stop;
            var s = stop;
            var lastGoodValue;
            var el = $(element)[0];
            function validate() {
                var val = element.html();
                
                var minSec = val.split(':');
                var sec, min,tenths = 0,secString;
                if (minSec.length === 2) {
                    min = parseInt(minSec[0].replace(/\D/g, ''));
                    secString = minSec[1].split('.');
                    
                } else if (minSec.length === 1) {
                    min = 0;
                    secString = minSec[0].split('.');
                }
                else {
                    s.duration = lastGoodValue;
                    return;
                }
                sec = parseInt(secString[0].replace(/\D/g, ''));
                if (secString.length === 2) {
                    tenths = parseInt(secString[1].replace(/\D/g, ''));
                }
                s.duration = (min * 60000) + (sec * 1000) + (tenths * 100);
                
            }

            
            if (isDuration) {
                renderDuration();
            } else {
                element.html(s.description);
            }
            
            function renderDuration() {
                if (s.duration > 100)
                    lastGoodValue = s.duration;
                else
                    s.duration = lastGoodValue;

                var min = (s.duration / 60 / 1000) << 0;
                var secs = ((s.duration / 1000) % 60);
                var tenths = Math.round((secs % 1) * 10);
                secs = Math.floor(secs);
                
                s.durationString = min + ':' + (secs < 10 ? '0' : '') + secs + '.' + tenths;
                stop.set_duration(lastGoodValue);
                if (hasFocused) {
                    angular.element('#currentTourPanel').scope().refreshStops();
                }
                element.html(s.durationString);
            }

            element.on('keyup', function (event) {
                if (isDuration) {
                    switch (event.keyCode) {
                        case 33:
                        case 38:
                            stop.duration += 1000;
                            renderDuration();
                            return;
                        case 34:
                        case 40:
                            stop.duration -= 1000;
                            renderDuration();
                            return;
                        case 27:
                        case 13:
                        case 9:
                            element.blur();
                            return;
                        default:
                            validate();
                            break;
                    }
                    
                }
            });
            var incrementing = false;
            
                 
            element.on('focus', function () {
                hasFocused = true;
                if (isDuration) {
                    if (incrementing) return;
                    scope.$apply(function () {
                        stop.editingDuration = true;
                    });
                    element.parent().find('.tinybutton').on('mousedown', function (e) {
                        incrementing = true;
                        setTimeout(function () { incrementing = false }, 500);
                        var btn = $(this);

                        if (btn.hasClass('duration-up')) {
                            stop.duration += 1000;
                            renderDuration();
                        } else {
                            stop.duration -= 1000;
                            renderDuration();
                        }
                        select();
                    });
                    element.parent().find('.tinybutton').on('mouseup', select);
                }
                else {
                    select();
                }
            });
            
            element.on('blur', function () {
                if (incrementing) return;
                scope.$applyAsync(function () {
                    if (isDuration) {
                        
                        validate();
                        renderDuration();
                        stop.set_duration(lastGoodValue);
                        stop.editingDuration = false;

                    } else {
                        s.set_description(element.html());
                    }
                    angular.element('#currentTourPanel').scope().refreshStops();
                });
            });
            function select() {
                setTimeout(function () {
                    
                    var txt = element.text();
                    var range = document.createRange();
                    var start = 0, end = txt.length;
                    if (isDuration) {
                        start = txt.indexOf(':') + 1;
                        end = txt.indexOf('.');
                    }
                    range.setStart(el.firstChild, start);
                    range.setEnd(el.firstChild, end);
                    var sel = window.getSelection();
                    sel.removeAllRanges();
                    sel.addRange(range);
                }, 10);

            }
            
        }
    };
}]);
wwt.app.directive('movable', ['AppState',function (appState) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var el = $(element);
            var target = el;
            var stickyCoords;
            var oncomplete = function () { };
            if (target.data('movable-target')) {
                el = $(target.data('movable-target'));
            }
            if (target.data('sticky')) {
                var stickyCss = appState.get(target.data('sticky'))
                if (stickyCss && stickyCss.top) {
                    el.css(stickyCss);
                    el.on('resize', function () {
                        stickyCss.width = el.width();
                        stickyCss.height = el.height();
                        appState.set(target.data('sticky'), stickyCss);
                    });
                }
                oncomplete = function () {
                    stickyCss.top = this.css.top;
                    stickyCss.left = this.css.left;
                    appState.set(target.data('sticky'), stickyCss);
                };
            }
            var move = Object.create(wwt.Move({
                el: el,
                target: target,
                oncomplete:oncomplete
            }));

        }
    };
}]);
wwt.app.directive('copyable', ['$timeout',function ($timeout) {
    return {
        restrict: 'E',
        templateUrl:'views/copy-to-clipboard.html',
        scope: { copy: '=' },
        link:function(scope,el,attrs){
            var input = el.find('input')[0];
            var copyButton = el.find('.input-group-addon');
            copyButton.bind('click', function (event) {
                input.select();
                document.execCommand('copy');
                $timeout(function () {
                    scope.copy.showStatus = true;
                    scope.copy.fadeout = false;
                    input.blur();
                }, 0);
                $timeout(function () { 
                    scope.copy.fadeout = true;
                }, 3333);
            })
        }
    };
}]);
wwt.app.factory('AppState', function() {
	var api = {
		set: setKey,
		get: getKey,
		getAll:getAll
	};

	var data;

	function setKey(key, val) {
	    try {
	        data[key] = val;
	        if (localStorage) {
	            localStorage.setItem('appState', JSON.stringify(data));
	        }
	    } catch (er) {
	        console.log('Error using localstorage. Is it turned off?');
	    }
	}

	function getKey(k) {
		return data[k];
	}

	function getAll() { return data; }

	var init = function () {
		var storedData = localStorage ? localStorage.getItem('appState') : {};
		data = storedData && localStorage ? JSON.parse(storedData) : {};
	};
	init();
	return api;
});  
wwt.app.factory('AutohidePanels', ['$rootScope', 'AppState', function ($rootScope,appState) {
    var api = {init:init};

    var  tourPlaying = false,
        editingTour = false,
        mouseInRegion = { tabs: false, context: false },
        panelHidden = { tabs: false, context: false },
        panels = { tabs: null, context: null },
        hideTimers = { tabs: null, context: null },
        showingPanels = { tabs: false, context: false },
        autoHide = { tabs: false, context: false },
        autoHideHover = { tabs: false, context: false },
        autoHideClick = { tabs: false, context: false },
    hideTimeout = 1200;

    function init() {
        panels = {
            tabs: $('#topPanel, .layer-manager'),
            context: $('.context-panel')
        };
        if (panels.tabs.length < 2) {
            setTimeout(init, 200);
            console.log('init', panels.tabs);
            return;
        }
        context = $('.context-panel');
        bindEvents();
        settingChange();
    };

    var bindEvents = function () {
        $rootScope.$on('autohideChange', settingChange);
        $rootScope.$watch('editingTour', tourStateChange);
        $rootScope.$watch('tourPlaying', function () {
            if ($rootScope.tourPlaying) {
                panels.tabs = $('#ribbon, #topPanel, .layer-manager');
            }
            tourStateChange();
        });
        $(window).on('mousedown', function () {
            $.each(['tabs', 'context'], function (i,groupKey) {
                if (tourPlaying && !editingTour && mouseInRegion[groupKey] && panelHidden[groupKey] && !showingPanels[groupKey]) {
                    regionClicked(groupKey);
                }
            });

        });

        $(window).on('mousemove touchstart touchmove touchend', function (event) {
            var y = event.pageY != undefined ? event.pageY : event.originalEvent.targetTouches[0].pageY;

            var inBottom = $(window).height() - 123 < y;
            if (inBottom !== mouseInRegion.context) {
                mouseInRegion.context = inBottom;
                cursorRegionChange();
            }
            var inTop = y < 142;
            if (inTop !== mouseInRegion.tabs) {
                mouseInRegion.tabs = inTop;
                cursorRegionChange();
            }
        });
    }

    var setBoth = function (o, ref) {
        if (typeof ref == 'object') {
            o.tabs = ref.tabs;
            o.context = ref.context;
        } else {
            o.tabs = ref;
            o.context = ref;
        }
    };
    

    var settingChange = function () {
        var settings = appState.get('settings'); 
        if (!settings || settings.autoHideTabs === undefined) {
            setBoth(autoHide, false);
        } else {
            if (settings.autoHideTabs !== autoHide.tabs && settings.autoHideTabs == false) {
                togglePanelGroup(true, 'tabs');
            }
            if (settings.autoHideContext !== autoHide.context && settings.autoHideContext == false) {
                togglePanelGroup(true, 'context');
            }
            autoHide.tabs = settings.autoHideTabs;
            autoHide.context = settings.autoHideContext;
        }
        if (tourPlaying) {
            panels.tabs = $('#ribbon, #topPanel, .layer-manager');
            
            setBoth(autoHideHover, true);
            setBoth(autoHideClick, true);
            hideTimeout = 100;
        } else /*if (editingTour)*/ {
            $('#ribbon').fadeIn();
            setBoth(autoHideClick, false);
            setBoth(autoHideHover, autoHide);
            panels.tabs = $('#topPanel, .layer-manager');
            hideTimeout = 1200;
        }
        cursorRegionChange();
    };
    

    var tourStateChange = function () {
        console.log('tourstatechange - playing:', $rootScope.tourPlaying);
        togglePanelGroup(!$rootScope.tourPlaying, 'tabs');
        togglePanelGroup(!$rootScope.tourPlaying, 'context');
        editingTour = $rootScope.editingTour;
        tourPlaying = $rootScope.tourPlaying;
        settingChange();
        cursorRegionChange();
    };

    var togglePanelGroup = function (show, groupKey) {
        //if (tabs.length < 2) tabs = $('#topPanel, .layer-manager');
        var panelGroup = panels[groupKey];
        if (show) {
            clearTimeout(hideTimers[groupKey]);
            if (panelHidden[groupKey] && !showingPanels[groupKey]) {
                panelHidden[groupKey] = false;
                showingPanels[groupKey] = true;
                panelGroup.fadeIn(800, function () { showingPanels[groupKey] = false; });
                if (tourPlaying && mouseInRegion.tabs) {
                    console.log('showingSlides'); 
                    $rootScope.$broadcast('showingSlides');
                }
            }
        } else {
            clearTimeout(hideTimers[groupKey]);
            hideTimers[groupKey] = setTimeout(function () {
                panelHidden[groupKey] = true;
                panelGroup.fadeOut(800);
            }, hideTimeout);
        }
    }
    
    
    var regionClicked = function (key) {
        console.log('regionClick', { autoHide: autoHide, autoHideClick: autoHideClick, tabs: panels.tabs });
        togglePanelGroup(true, key);
        if (key === 'tabs') {
            $rootScope.$broadcast('showingSlides');
        }
    }

    var cursorRegionChange = function () {
        if (tourPlaying && !editingTour) { return; }
        //console.trace('regionChange', { autoHide: autoHide, autoHideHover: autoHideHover });
        $.each(['tabs', 'context'], function (i,groupKey) {
            if (autoHideHover[groupKey]) {
                if (mouseInRegion[groupKey]) {
                    togglePanelGroup(true, groupKey);
                } else if (!panelHidden[groupKey]) {
                    togglePanelGroup(false, groupKey);
                }
            }
        });
    };

    
    
    return api;
}]);
wwt.app.factory('Localization', ['$http','$q','Util', function($http, $q, util) {
	var api = {
		setLanguage: setLanguage,
		getString: getString,
		getFromEn: getFromEn,
		getAvailableLanguages: getAvailableLanguages
	};
	var langCode,
		englishData,
		englishArray,
		locData,
		locArray,
		lpacks/*,
		unlocalized = {}*/;
	function getString(id, s) {
		return locArray && locArray[id] ? locArray[id] : s ? s : '';
	}
	/*
	setInterval(function () {
		var rawArray = [];
		$.each(unlocalized, function(k, v) {
			rawArray.push(k);
		});
		util.log('unlocalized', rawArray);
	}, 11111);
	to trap unlocalized
	uncomment unlocalized in this file
	the if ($.inArray in getFromEn below
	and the setInterval above

	in mainController, uncomment the first if in getFromEn
	open both desktop and mobile sites in debug
	open all dialogs, menus, tabs, etc.
	go to console after 10sec
	copy array to global
	run this
	var unloced = '';$.each(temp1, function(){unloced += this + '\n';}); console.log(unloced);
	copy/paste in mail to team
	revert this and maincontroller

	TODO:
	make all loc strings promise-based
	move maincontroller fn to uilibrary factory

	*/

	function getFromEn(s) {
		if (langCode === 'EN') {
			/*if ($.inArray(s, englishArray) == -1) {
				unlocalized[s] = 'missing';
			}*/
			return s;
		} else {
			return getString($.inArray(s,englishArray), s);
		}
	}

	function getAvailableLanguages() {
		var deferred = $q.defer();
		initPromise.then(function() {
			var langArray = [];
			lpacks.find('languagepack').each(function(i, pack) {
				langArray.push({ label: $(pack).attr('name'), code: $(pack).attr('code') });
			});
			
			deferred.resolve(langArray);
		});
		return deferred.promise;
	}

	function setLanguage(code) {
		var deferred = $q.defer();
		initPromise.then(function () {
			langCode = code;
			if (code === 'EN') {
				locData = englishData;
				locArray = locData.split('\n');

				deferred.resolve(true);
				return;
			}
			var url = lpacks.find('languagepack[code=' + code + ']').attr('url');
			util.log(url);
			$http.get(url).
				success(function(data) {
					data = transformLanguagePack(data);
					locData = data;
					locArray = [];
					$.each(data.split('\n'), function (i,item) {
						var spl = item.split('\t');
						var ind = parseInt(spl[0], 10);
						if (spl.length === 2 && !isNaN(ind)) {
							locArray[ind] = spl[1];
							//console.log(ind, item);
						}
					});
					deferred.resolve(true);
				});
		});      
		
		return deferred.promise;
	}
	var init = function () {
		var deferred = $q.defer();
		$q.all([
			$http.get('http://www.worldwidetelescope.org/wwtweb/catalog.aspx?X=Languages')
			.success(function(data) {
				lpacks = $(data);
			})
			.error(function(data, status, headers, config) {
				util.log(data, status, headers, config);
			}),
			$http.get('http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=lang_en')
			.success(function(data) {
				data = transformLanguagePack(data);
				englishData = data;
				var dsplit = data.split('\n');
				
				englishArray = [];
				$.each(dsplit, function () {
					var s1 = this.split('\t')[1];
					if (s1) {
						s1 = s1.split('\r')[0];
					}
					englishArray[parseInt(this.split('\t')[0])] = s1;
				});

			})
		]).then(function() {
			deferred.resolve(true);
		});
		return deferred.promise;
	};

	var transformLanguagePack = function (data) {
		if (data.charAt(0) == 1) {
			return data;
		}

		var re1 = new RegExp(data.charAt(0), "g");
		var re2 = new RegExp(data.charAt(3), "g");
		return data.replace(re1, '').replace(re2, '');
	};

	var initPromise = init();
	return api;
}]);


wwt.app.factory('FinderScope',
    ['SearchData',
    '$timeout','Util',
    function (searchDataService, $timeout, util) {
        var api = {
            init: init,
            scopeMove:scopeMove
        };
        var searchData;
        function init() {
            searchDataService.getData().then(function(d) {
                searchData = d;
                scopeMove();
                //console.log(scopeMove());
            });
        };

        function scopeMove() {
            if (!searchData) {
                return false;
            }
            var pos = $('.finder-scope').position();
            var offsetX = 301;
            var offsetY = 87;
            
            var scopeCoords = wwtlib.WWTControl.singleton.getCoordinatesForScreenPoint(pos.left + offsetX, pos.top + offsetY);
            scopeCoords.x = (scopeCoords.x + 720) % 360;
            var scope = wwtlib.Coordinates.raDecTo3d(scopeCoords.x, scopeCoords.y);
            
            var constellation = wwtlib.Constellations.containment.findConstellationForPoint(scopeCoords.x, scopeCoords.y);
            var closestDist, closestPlace;
            var constellationPlaces,ssPlaces;
            $.each(searchData.Constellations, function (i, item) {
                if (item.name === constellation) {
                    constellationPlaces = item.places;
                } else if (item.name === 'SolarSystem') {
                    ssPlaces = item.places;
                }
            });
            var searchPlaces = ssPlaces.concat(constellationPlaces);
            $.each(searchPlaces, function (i, place) {
                try {
                    var placeDist = wwtlib.Vector3d.subtractVectors(place.get_location3d(), scope);
                    if ((i === 0) || closestDist.length() > placeDist.length()) {
                        closestPlace = place;
                        closestDist = placeDist;
                    }
                    
                    
                } catch (er) {
                    if (place && place.get_name()!='Earth')
                    util.log(er);
                }
            });

            util.getAstroDetails(closestPlace);
            /*if (details.bValid)
            {
                riseText.Text = UiTools.FormatDecimalHours(details.Rise);
                transitText.Text = UiTools.FormatDecimalHours(details.Transit);
                setText.Text = UiTools.FormatDecimalHours(details.Set);
            }*/
            return closestPlace;
        }

        
        return api;
    }
    ]);
// Includes shared functions for ExploreController, SearchController, and 
// ContextPanelController - handles thumbnail click, right-click, and paging 
// behavior

// ToursController does not use this factory

wwt.app.factory('ThumbList', ['$rootScope', 'Util', 'Places', '$timeout', function ($rootScope, util, places, $timeout) {
    var api = {
        init:init,
        clickThumb: clickThumb,
        calcPageSize: calcPageSize,
        spliceOnePage: spliceOnePage,
        goFwd: goFwd,
        goBack: goBack
    };
    
    // Each controller calls init and passes in the controller
    // scope
    function init(scope, name) {
        scope.pageCount = 1; 
        scope.pageSize = 1;
        scope.currentPage = 0;
        
        scope.preventClickBubble = function (event) {
            event.stopImmediatePropagation();
        };
        scope.goBack = function () {
            goBack(scope);
        };
        scope.goFwd = function () {
            goFwd(scope);
        }; 
        scope.showMenu = function (i) {
            var item = scope.collectionPage[i];
            item.contextMenuEvent = true;
            $('.popover-content .close-btn').click();
            if (!item.get_isFolder() && item.get_name() !== 'Up Level') {
                var menuContainer = $((name === 'context' ? '.nearby-objects ' : '.top-panel ') + '#menuContainer' + i);
                if (util.isMobile) {
                    menuContainer = $('#' + name + 'Container #menuContainer' + i);
                }
                menuContainer.append($('#researchMenu')); 
                setTimeout(function () {
                    $('.popover-content .close-btn').click();
                    menuContainer.find('#researchMenu')
                        .addClass('open')
                        .off('click')
                        .on('click', function (event) {
                            event.stopPropagation();
                        });
                    menuContainer.find('.drop-toggle').click();
                    $timeout(function () {
                        if (!util.isMobile) {
                            $('.dropdown-backdrop').off('contextmenu');
                            $('.dropdown-backdrop').on('contextmenu', function(event) {
                                $(this).click();
                                event.preventDefault();
                            });
                        }
                        scope.setMenuContextItem(item, true);
                        item.contextMenuEvent = false;
                    }, 10);

                }, 10);
            }
        };

        // toggles the expanded thumbnail view to show 1 or 5 rows of thumbs
        scope.expandThumbnails = function (flag) {
            $('body').append($('#researchMenu'));
            scope.currentPage = 0;
            scope.expanded = flag != undefined ? flag : !scope.expanded;
            scope.expandTop(scope.expanded,name);
            calcPageSize(scope, name === 'context'); 
        };
        scope.dropdownClass = name === 'context' && !util.isMobile ? 'dropup menu-container' : 'dropdown menu-container';
        scope.popupPosition = name === 'context' && !util.isMobile ? 'top' : 'bottom';
    }

    function clickThumb(item, scope, outParams, callback) {
        if (item.contextMenuEvent) {
            return outParams;
        }
        if (!outParams) {
            outParams = {}; 
        }
        scope.activeItem = item.get_thumbnailUrl() + item.get_name();
        scope.setActiveItem(item);
        wwt.wc.clearAnnotations();
        if (item.get_name() === 'Up Level') {
            $('body').append($('#researchMenu'));
            scope.currentPage = 0;
            outParams.depth--;
            outParams.breadCrumb.pop();
            scope.breadCrumb = outParams.breadCrumb;
            outParams.cache.pop();
            scope.collection = outParams.cache[outParams.cache.length - 1];
            calcPageSize(scope, false);
            return outParams;
        }
        if (item.get_url && item.get_url() && item.get_url().indexOf('?wwtfull') !== -1) {
            window.open(item.get_url());
            return outParams;
        }
        if (item.get_isFolder()) {
            $('#folderLoadingModal').modal('show');
            $('body').append($('#researchMenu'));
            scope.currentPage = 0;
            outParams.depth++;
            outParams.breadCrumb.push(item.get_name());
            scope.breadCrumb = outParams.breadCrumb;
            places.getChildren(item).then(function (result) {
                $('#folderLoadingModal').modal('hide');
                if ($.isArray(result[0])) {
                    result = result[0];
                }
                var unique = [];
                $.each(result, function (index, el) {
                    if ($.inArray(el, unique) === -1) unique.push(el);
                });
                scope.collection = unique;
                calcPageSize(scope, false);
                outParams.cache.push(result);
                if (outParams.openCollection) {
                    if (outParams.newCollectionUrl) {
                        var i = 0;
                        while (result[i].url && result[i].url.indexOf(outParams.newCollectionUrl) === -1) i++;

                        scope.clickThumb(result[i]);
                        outParams.newCollectionUrl = null;
                    } else if (result.length) {
                        scope.clickThumb(result[0]);
                    }
                }

                if (callback) {
                    callback();
                }
            });
            return outParams;
        } else if (outParams.openCollection) {
            outParams.openCollection = false;
        } else if (scope.$hide) {
            scope.$hide();
            $rootScope.searchModal = false;
        } else if (util.isMobile) {
            $('#explorerModal').modal('hide');
        }

        if ((item.isFGImage && item.imageSet && scope.lookAt !== 'Sky') || item.isSurvey) {
            if (item.guid && item.guid.toLowerCase().indexOf('mars.') == -1) {
                scope.setLookAt('Sky', item.get_name(), true, item.isSurvey);
            }
            if (item.isSurvey) {
                scope.setSurveyBg(item.get_name(), item);
            } else {
                scope.setForegroundImage(item);
            } 
            if (scope.$hide) {
                scope.$hide(); 
                $rootScope.searchModal = false;
            }
            return outParams;
        }
        else if (item.isPanorama) {
            scope.setLookAt('Panorama', item.get_name());
        } else if (item.isEarth) {
            scope.setLookAt('Earth', item.get_name());
        } else if (util.getIsPlanet(item) && scope.lookAt !== 'SolarSystem') {
            scope.setLookAt('Planet', item.get_name());
        } else if (item.isPlanet && scope.lookAt !== 'SolarSystem') {
            scope.setLookAt('Planet', '');
        }
        if ((ss.canCast(item, wwtlib.Place) || item.isEarth) && !item.isSurvey) {
            scope.setForegroundImage(item);
        }
        if (ss.canCast(item, wwtlib.Tour)) {
            scope.playTour(item.get_tourUrl());
        }
        return outParams;  
    };

    function calcPageSize(scope, isContextPanel) {
        var list = scope.collection;
        var tnWid = 116;
        var winWid = $(window).width();

        if (isContextPanel && (scope.lookAt === 'Sky' || scope.lookAt === 'SolarSystem')) {
            winWid = winWid - 216; //angular.element('body.desktop .fov-panel').width();
        }
        scope.pageSize = util.isMobile ? 99999 : Math.floor(winWid / tnWid);

        if (scope.expanded) {
            scope.pageSize *= 5;
        }
        var listLength = list ? list.length : 2;
        $timeout(function () {
            scope.pageCount = Math.ceil(listLength / scope.pageSize);
            spliceOnePage(scope);
        },10);
    };

    function goBack(scope) {
        $('body').append($('#researchMenu'));
        scope.currentPage = scope.currentPage === 0 ? scope.currentPage : scope.currentPage - 1;
        return spliceOnePage(scope);
    };

    function goFwd(scope) {
        $('body').append($('#researchMenu'));
        scope.currentPage = scope.currentPage === scope.pageCount - 1 ? scope.currentPage : scope.currentPage + 1;
        return spliceOnePage(scope);
    };

    function spliceOnePage(scope) {
        if (scope.collection) {
            var start = scope.currentPage * scope.pageSize;
            scope.collectionPage = scope.collection.slice(start, start + scope.pageSize);
        }
    };

    return api;

}]);


wwt.app.factory('Util', ['$rootScope', function ($rootScope) {
	var api = {
		getClassificationText: getClassificationText,
		getAstroDetails: getAstroDetails,
		formatDecimalHours: formatDecimalHours,
		formatHms: formatHms,
		drawCircleOverPlace: drawCircleOverPlace,
		getIsPlanet: getIsPlanet,
		secondsToTime: secondsToTime,
		getQSParam: getQSParam,
		getImageset: getImageset,
		getCreditsText: getCreditsText,
		getCreditsUrl: getCreditsUrl,
		isAccelDevice: isAccelDevice,
		isMobile:  $('body').hasClass('mobile'),
		isStaging: function() {
			return location.href.indexOf('worldwidetelescope') === -1;
		},
        isDebug:getQSParam('debug')!=null,
		nav: nav,
		log: log,
		resetCamera: resetCamera,
		goFullscreen: goFullscreen,
		exitFullscreen: exitFullscreen,
		toggleFullScreen: toggleFullScreen,
		getImageSetType: getImageSetType,
		trackViewportChanges: trackViewportChanges,
		parseHms: parseHms
		
};
	var fullscreen = false;
	function getClassificationText(clsid) {
		if (clsid && !isNaN(parseInt(clsid))) {
			var str;
			$.each(wwtlib.Classification, function (k, v) {
				if (v === clsid) {
					str = k;
				}
			});
			var out = str.replace(/^\s*/, ""); // strip leading spaces
			out = out.replace(/^[a-z]|[^\s][A-Z]/g, function (str, offset) {
				if (offset == 0) {
					return (str.toUpperCase());
				} else {
					return (str.substr(0, 1) + " " + str.substr(1).toUpperCase());
				}
			});
			return (out);
		} else return null;
	};

	function formatDecimalHours(dayFraction, spaced) {
		var ts = new Date(new Date().toUTCString()).valueOf() - new Date().valueOf();
		var hr = ts / (1000 * 60 * 60);
		var day = (dayFraction - hr) + 0.0083333334;
		while (day > 24){
			day -= 24;
		}
		while (day < 0){
			day += 24;
		}
		var hours = day.toFixed(0);
		var minutes = ((day * 60) - (hours * 60)).toFixed(0);

		var join = spaced ? ' : ' : ':';
		//var seconds = ((day * 3600) - (((hours * 3600) + ((double)minutes * 60.0)));

		return ([int2(hours), int2(minutes)]).join(join);
	}
	function int2(dec) {
		return Math.abs(Math.floor(dec)) < 10 ? dec < 0 ?  '-0' + Math.abs(Math.floor(dec)) : '0' + Math.floor(dec) : Math.floor(dec);
	}

	function truncate(n) {
	    return (n >= 0) ? Math.floor(n) : Math.ceil(n);
	}

	function formatHms(angle, isHmsFormat, signed, spaced) {
	    var minutes = (angle - truncate(angle)) * 60;
	    var seconds = (minutes - truncate(minutes)) * 60;
	    minutes = Math.abs(minutes);
	    seconds = Math.abs(seconds);

		var join = spaced ? ' : ' : ':';
		if (isNaN(angle)) {
			angle = minutes = seconds = 0;
		}
		return isHmsFormat ? int2(angle) + 'h'
			+ int2(minutes) + 'm'
			+ int2(seconds) + 's' :
			([signed && angle > 0 ? '+' + int2(angle) : int2(angle), int2(minutes), int2(seconds)]).join(join);
	};

	function parseHms(input) {
		var parts;
		function convertHmstoDec(hours, minutes, seconds) {
			var dec = parseInt(hours) + parseInt(minutes) / 60 + parseInt(seconds) / (60 * 60);
			return dec;
		}
		if (input.indexOf(':') != -1) {
			parts = input.split(':');
		}
		else if (input.indexOf('h') != -1) {
			parts = input.replace(/h/, ',').replace(/m/, ',').replace(/s/, '').split(',');
		}
		if (parts) {
			return convertHmstoDec(parts[0], parts[1], parts[2]);
		} else {
			return parseFloat(input);
		}
	}

	function getAstroDetails(place) {
		var coords = wwtlib.Coordinates.fromRaDec(place.get_RA(), place.get_dec());
		var stc = wwtlib.SpaceTimeController;
		var altAz = wwtlib.Coordinates.equitorialToHorizon(coords, stc.get_location(), stc.get_now());
		place.altAz = altAz;
		var classificationText = getClassificationText(place.get_classification());
		var riseSet;
		if (classificationText == 'Solar System') {

			var jNow = stc.get_jNow() + .5;
			var p1 = wwtlib.Planets.getPlanetLocation(place.get_name(), jNow - 1);
			var p2 = wwtlib.Planets.getPlanetLocation(place.get_name(), jNow);
			var p3 = wwtlib.Planets.getPlanetLocation(place.get_name(), jNow + 1);

			var type = 0;
			switch (place.get_name()) {
				case "Sun":
					type = 1;
					break;
				case "Moon":
					type = 2;
					break;
				default:
					type = 0;
					break;
			}

			riseSet = wwtlib.AstroCalc.getRiseTrinsitSet(jNow, stc.get_location().get_lat(), -stc.get_location().get_lng(), p1.RA, p1.dec, p2.RA, p2.dec, p3.RA, p3.dec, type);
		}
		else {
			riseSet = wwtlib.AstroCalc.getRiseTrinsitSet(stc.get_jNow() + .5, stc.get_location().get_lat(), -stc.get_location().get_lng(), place.get_RA(), place.get_dec(), place.get_RA(), place.get_dec(), place.get_RA(), place.get_dec(), 0);
		}
		if (!riseSet.bValid && !riseSet.bNeverRises) {
			riseSet.bNeverSets = true;
		}
		place.riseSet = riseSet;
	}

	function drawCircleOverPlace(place) {
		wwt.wc.clearAnnotations();
		if ($('#lstLookAt').val() === '2') {
			var circle = wwt.wc.createCircle();
			circle.set_center(place.get_location3d());
			circle.set_skyRelative(false);
			
			wwt.wc.addAnnotation(circle);
		}
	}

	function getIsPlanet(place) {
		var cls,isPlanet;
		if (typeof place.get_classification === 'function') {
			cls = place.get_classification();
			isPlanet = getClassificationText(cls) === 'Solar System';
		}
		return isPlanet || typeof place.get_rotation ==='function';
	}

	function secondsToTime(secs) {
		var hours = Math.floor(secs / (60 * 60));

		var divisorForMinutes = secs % (60 * 60);
		var minutes = Math.floor(divisorForMinutes / 60);

		var divisorForSeconds = divisorForMinutes % 60;
		var seconds = Math.ceil(divisorForSeconds);

		var obj = {
			"h": hours < 10 ? '0' + hours : hours.toString(),
			"m": minutes < 10 ? '0' + minutes : minutes.toString(),
			"s": seconds < 10 ? '0' + seconds : seconds.toString()
		};
		return obj;
	}

	function getQSParam(name) {
		name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
		var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
			results = regex.exec(location.search);
		return results == null ? null : decodeURIComponent(results[1].replace(/\+/g, " "));
	}

	function getImageset(place) {
		if (!place) {
			return null;
		} else if (ss.canCast(place, wwtlib.Imageset)) {
			return place;
		} else if (place.get_backgroundImageset && ss.canCast(place.get_backgroundImageset(), wwtlib.Imageset)) {
			return place.get_backgroundImageset();
		} else if (place.get_studyImageset && ss.canCast(place.get_studyImageset(), wwtlib.Imageset)) {
			return place.get_studyImageset();
		} else {
			return null;
		}
	}

	function getCreditsText(pl) {
		var imageSet = getImageset(pl);
		if (imageSet) {
			return imageSet.get_creditsText();
		} else {
			return '';
		}
	}
	function getCreditsUrl(pl) {
		var imageSet = getImageset(pl);
		if (imageSet) {
			return imageSet.get_creditsUrl();
		} else {
			return '';
		}
	}
	
	var accelDevice = false; 
	
	function redirectClient(val) {
		return;
		var qs = location.search.substr(1);
		var newQs = '?';

		$.each(qs.split('&'), function (i, s) {
			if (i > 0) {
				newQs += '&';
			}
			if (s.indexOf('client') !== 0) {
				newQs += s;

			}
		});
		if (newQs.length > 1) {
			newQs += '&';
		}
		newQs += 'client=' + val;
		location.href = '/webclient' + newQs + location.hash;
		
	}

	function isAccelDevice() {
		return accelDevice;
	}

	function log() {
		if (getQSParam('debug') != null) {
			console.log(arguments);
		}
	}

    var isStandalone = function() {
        return $('body').data('standalone-mode') == true;
    }

    function nav(url) {
        if (isStandalone() && url.indexOf('http') !== 0) {
            url = 'http://worldwidetelescope.org' + url;
        }
		window.open(url);
	}
    function resetCamera(leaveHash) {
        if (!leaveHash) {
            location.hash = '/';
        }
        wwt.wc.gotoRaDecZoom(0, 0, 60, true);

    };
    function exitFullscreen() {
        if (fullscreen) {
            wwt.exitFullScreen();
            fullscreen = false;
        }
    }
    function goFullscreen() {
        if (!fullscreen) {
            wwt.requestFullScreen(document.body);
            fullscreen = true;
        }
    }

	function toggleFullScreen () {
		if (fullscreen) {
			wwt.exitFullScreen();
			fullscreen = false;
		} else {
			wwt.requestFullScreen(document.body);
			fullscreen = true;
		}
	};

	var imageSetTypes = [];
	function getImageSetType(sType) {
		if (!imageSetTypes.length) {
			$.each(wwtlib.ImageSetType, function(k, v) {
				if (!isNaN(v)) {
					imageSetTypes[v] = k.toLowerCase();
				}
			});
		}
		return imageSetTypes.indexOf(sType.toLowerCase()) == -1 ? 2 : imageSetTypes.indexOf(sType.toLowerCase());
		
	}

	var keyHandler = function (e) {
	    switch (e.keyCode) {
	        case 27:
	            $rootScope.$broadcast('escKey');
	            fullscreen = false;
	            break;
	    }
	};

	$(document).on('keyup', keyHandler);

	var dirtyInterval,
        viewport = {
		isDirty: false,
		RA: 0,
		Dec: 0,
		Fov: 60
	};
	function trackViewportChanges() {
		viewport = {
			isDirty: false,
			init: true,
			RA: wwt.wc.getRA(),
			Dec: wwt.wc.getDec(),
			Fov: wwt.wc.get_fov()
		};

		$rootScope.$broadcast('viewportchange', viewport);

		$rootScope.languagePromise.then(function() {
			viewport = {
				isDirty: false,
				init: true,
				RA: wwt.wc.getRA(),
				Dec: wwt.wc.getDec(),
				Fov: wwt.wc.get_fov()
			};

			$rootScope.$broadcast('viewportchange', viewport);
			viewport.init = false;


			dirtyInterval = setInterval(dirtyViewport, 250);
		});
	}

	
	
	var dirtyViewport = function () {
		var wasDirty = viewport.isDirty;
		viewport.isDirty = wwt.wc.getRA() !== viewport.RA || wwt.wc.getDec() !== viewport.Dec || wwt.wc.get_fov() !== viewport.Fov;
		viewport.RA = wwt.wc.getRA();
		viewport.Dec = wwt.wc.getDec();
		viewport.Fov = wwt.wc.get_fov();
		if (viewport.isDirty || wasDirty) {
			$rootScope.viewport = viewport;
			$rootScope.$broadcast('viewportchange', viewport);
		}
	}
	var browsers = {};
	var has = function (src, search) {
	    return src.indexOf(search) >= 0;
	}
	var ua = navigator.userAgent.toLowerCase();

	browsers.isEdge = has(ua, 'edge/') > 0;
	browsers.isFF = has(ua, 'firefox') > 0;
	browsers.isIE = has(ua, 'msie') || has(ua, 'trident');
	browsers.isChrome = has(ua, 'chrome');
	browsers.isSafari = has(ua, 'safari') && !browsers.isChrome && !browsers.isIE && !browsers.isEdge && !browsers.isFF;;
	browsers.isChrome = has(ua, 'chrome') > 0 && !browsers.isIE && !browsers.isEdge && !browsers.isFF;
	browsers.isWindows = has(ua, 'windows');

	//console.log(browsers); 
	
	return $.extend(api, browsers);

}]);


wwt.app.factory('UILibrary', ['$rootScope','AppState','Util', 'Localization', function ($rootScope, appState, util, loc) {

	$rootScope.layerManagerHidden = appState.get('layerManagerHidden') ? true : false;

	$rootScope.toggleLayerManager = function () {
		$rootScope.layerManagerHidden = !$rootScope.layerManagerHidden;
		appState.set('layerManagerHidden', $rootScope.layerManagerHidden);
	} 

	$rootScope.getCreditsText = function (place) {
		return util.getCreditsText(place); 
	}
	$rootScope.getCreditsUrl = function (place) {
		return util.getCreditsUrl(place);
	}

	$rootScope.getClassificationText = function (clsid) {
		var txt = util.getClassificationText(clsid);
		return txt || loc.getFromEn('Unknown');
	};

	$rootScope.secondsToTime = function (secs) {
		return util.secondsToTime(secs);
	}

	$rootScope.isMobile = util.isMobile;

	$rootScope.resLocation = $('body').data('res-location');
	$rootScope.bottomControlsWidth = function() {
		return (angular.element('div.context-panel').width() - angular.element('body.desktop .fov-panel').width()) + 1;
	}
	$rootScope.layerManagerHeight = function() {
		return $(window).height() - (168 + $('body.desktop .context-panel').height());
	};
	
	$rootScope.copyLink = function (event) {
	    var src = $(event.currentTarget);
	    var input = src.prev();
	    input[0].select();
	    document.execCommand('copy');
	    var flyout = $('<div class=clipboard-status>Copied successfully</div>');
	    input.parent().css('position', 'relative').append(flyout);
	    //flyout.fadeIn(200).show();
	    setTimeout(function () { flyout.fadeOut(1111); }, 3333);
	}
	 
	return true;
}]);


wwt.app.factory('SearchUtil', [
	'SearchData',
    '$q',
    'Util',
    '$rootScope',
	function (searchDataService, $q, util, $rootScope) {
	
	var api = {
		runSearch: runSearch,
		findNearbyObjects: findNearbyObjects,
		getPlaceById:getPlaceById
	}
	function runSearch(q) {
		var deferred = $q.defer();

		searchDataService.getIndex().then(function (d) {
			var searchData = wwt.searchDataIndexed;
			var foundPlaces = [];
			if (q.length < 2) {
				foundPlaces = searchData[q];
			} else {
				var subset = searchData[q.charAt(0).toLowerCase()];
				$.each(subset, function (i, place) {
					var names = place.get_names();
					var placeChosen = false;
					$.each(names, function (j, name) {

						if (q.indexOf(' ') === -1 && name.split(' ').length > 1) {
							var words = name.split(' ');
							$.each(words, function (k, word) {
								if (word.toLowerCase().indexOf(q.toLowerCase()) === 0 && !placeChosen) {
									foundPlaces.push(place);
									placeChosen = true;
								}
							});
						} else if (name.toLowerCase().indexOf(q.toLowerCase()) === 0 && !placeChosen) {
							foundPlaces.push(place);
							placeChosen = true;
						}
					});

				});
			}
			deferred.resolve(foundPlaces.sort(sortByImagery));
		});

		return deferred.promise;
	}

	var sortByImagery = function(p1, p2) {
		return p2.get_constellation() === 'SolarSystem' && p1.get_constellation() !== 'SolarSystem' ? 1 :
			p1.get_constellation() === 'SolarSystem' && p2.get_constellation() !== 'SolarSystem' ? -1 :
			p2.get_studyImageset() && !p1.get_studyImageset() ? 1 :
			p1.get_studyImageset() && !p2.get_studyImageset() ? -1 :
			p1.get_name() - p2.get_name();
	}

	function getPlaceById(id) {
		var deferred = $q.defer();

		searchDataService.getData().then(function (d) {
			var constellationIndex = parseInt(id.split('.')[0]);
			var placeIndex = parseInt(id.split('.')[1]);
			deferred.resolve(d.Constellations[constellationIndex].places[placeIndex]);
		});

		return deferred.promise;
	}
		
	function findNearbyObjects(args) {
		var deferred = $q.defer();

		searchDataService.getData().then(function(d) {
			var searchData = wwt.searchData;
			if ($rootScope.viewport && (args.lookAt === 'Sky' || args.lookAt === 'SolarSystem')) {
				
				var ulCoords = args.singleton.getCoordinatesForScreenPoint(0, 0);
				var corner = wwtlib.Coordinates.raDecTo3d(ulCoords.x, ulCoords.y);
				var center = wwtlib.Coordinates.raDecTo3d($rootScope.viewport.RA, $rootScope.viewport.Dec);
				var dist = wwtlib.Vector3d.subtractVectors(corner, center).length();

				var constellation = args.singleton.constellation;
				var constellationPlaces, ssPlaces;
				$.each(searchData.Constellations, function(i, item) {
					if (item.name === constellation) {
						constellationPlaces = item.places;
					} else if (item.name === 'SolarSystem') {
						ssPlaces = item.places;
					}
				});

				if (args.lookAt === 'SolarSystem') {
					deferred.resolve(ssPlaces);
				}
				var searchPlaces = ssPlaces.concat(constellationPlaces);


				var results = [];
				//console.log(dist);
				$.each(searchPlaces, function(i, place) {
					if (place && place.get_name() !== 'Earth') {
						try {
							var placeDist = wwtlib.Vector3d.subtractVectors(place.get_location3d(), center);
							if (dist > placeDist.length()) {
							    results.push(place);
							    //if (place.get_constellation() === 'SolarSystem') {
							    //    console.log(place.get_name(), placeDist.length());
							    //}
							}
						} catch (er) {
							util.log(er, place);
						}
					}
				});
				deferred.resolve(results.sort(sortByImagery));

			} else {
				deferred.resolve([]);
			}
		});
		return deferred.promise;
	}

	return api;
	}]);


wwt.app.factory('Skyball',['$rootScope', function ($rootScope) {
	var api = {
		init: init
	};
	var canvas, ctx;
	 
	function draw(event, viewport) {
		if (!viewport.isDirty && !viewport.init){ return;}
		
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


wwt.app.factory('HashManager', [
	'$rootScope', function($rootScope) {
		var api = {
			setHashVal: setHashVal,
			getHashVal: getHashVal,
			removeHashVal: removeHashVal,
			getHashObject: getHashObj
		};
		
		var privateHash = '#/';

		function setHashVal(key, v, privateOnly, reset) {
			if (isNaN(v)) {
				v = v.replace(/\s/g, '_');
			}
			var newHash = '';
			var hash = privateOnly ? privateHash.substr(2) : location.hash.substr(2);
			if (reset) {
				newHash = key + '=' + v;
			} else if (getHashVal(key, privateOnly)) {

				var pairs = hash.split('&');
				if (pairs.length > 1) {
					$.each(pairs, function (i, pair) {
						var kvPair = pair.split('=');
						if (i > 0) {
							newHash += '&';
						}
						if (kvPair[0] == key) {
							newHash += kvPair[0] + '=' + v;
						} else {
							newHash += pair;
						}
					});
				} else if (hash.split('=')[0] == key) {

					newHash += key + '=' + v;
				}


			} else {
				if (hash.length > 2) {
					newHash = hash + '&' + key + '=' + v;
				} else {
					newHash = key + '=' + v;
				}
			}
			newHash = newHash.replace(/&&/g, '&');
			if (!privateOnly) {
				location.href = '#/' + newHash;
			} else {
				privateHash = '#/' + newHash;
			}

			return location.href.split('#')[0] + '#/' + newHash;
		}

		function removeHashVal(key, privateOnly) {
			var newHash = '';
			var hash = privateOnly ? privateHash.substr(2) : location.hash.substr(2);
			if (getHashVal(key, privateOnly)) {
				var pairs = hash.split('&');
				if (pairs.length > 1) {
					$.each(pairs, function (i, pair) {
						var kvPair = pair.split('=');
						if (i > 0) {
							newHash += '&';
						}
						if (kvPair[0] != key) {
							newHash += pair;
						}
					});
				} else if (hash.split('=')[0] == key) {

					newHash += '';
				}
			} else {
				newHash = hash;
			}
			newHash = newHash.replace(/&&/g, '&');
			if (!privateOnly) {
				location.href = '#/' + newHash;
			} else {
				privateHash = '#/' + newHash;
			}

			return location.href.split('#')[0] + '#/' + newHash;
		}

		function getHashVal(key, privateOnly) {
			/*var value = null;
			var hash = privateOnly ? privateHash.substr(2) : location.hash.substr(2);
			if (hash.length > 2) {
				var pairs = hash.split('&');
				if (pairs.length > 1) {
					$.each(pairs, function (i, pair) {
						var kvPair = pair.split('=');
						if (kvPair[0] == key) {
							value = kvPair[1];
						}
					});
				}
				else if (hash.split('=')[0] == key) {
					value = hash.split('=')[1];
				}
			}
			return value;*/ 
			return getHashObj(privateOnly)[key];
		}

		function getHashObj(privateOnly, hashString) {
			var obj = {};
			var hash = hashString ? hashString : privateOnly ? privateHash.substr(2) : location.hash.substr(2);
			if (hash.length > 2 && hash.indexOf('=') != -1) {
				var pairs = hash.split('&');
				if (pairs.length > 0) {
					$.each(pairs, function (i, pair) {
						if (pair.indexOf('=') != -1 && pair.length > 2) {
							var kvPair = pair.split('=');
							obj[kvPair[0].replace(/_/g, ' ')] = kvPair[1].replace(/_/g, ' ');
						}
					});
				}
				
			}
			return obj;
		}


		var hashChange = function (e) {

			setTimeout(function() {
				$rootScope.$broadcast('hashChange', getHashObj());
			}, 10);
			
		}

		window.onhashchange = hashChange;
		return api;
	}
]);

wwt.app.factory('MediaFile', ['$q', function ($q) {
    var api = {
        addLocalMedia: addLocalMedia,
        flushStore: flushStore,
        getBinaryData: getBinaryData,
        
    };
    var mediaCache = [];

    function Media(params) {
        
        return{
            url:params.url,
            key:params.key,
            db:'tempblob',
            size:params.size,
            filename:params.name
        }
    }

    function addLocalMedia(mediaKey, file, db) {
        
        var deferred = $q.defer();
        var keys = ['collection', 'tour', 'image'];
        var req = indexedDB.open('tempblob');
        req.onupgradeneeded = function () {
            // Define the database schema if necessary.
            var db = req.result;
            var store = db.createObjectStore('files');
        };
        req.onsuccess = function () {
            var db = req.result;

            var key = keys.indexOf(mediaKey);
            
            var tx = db.transaction('files', 'readwrite');
            var store = tx.objectStore('files');
            var addFile = function () {
                var addTx = store.put(file, key);
                addTx.onsuccess = readFile;
            }
            var readFile = function () {
                var mediaReq = store.get(key);
                mediaReq.onsuccess = function (e) {
                    var file = mediaReq.result;
                    var localUrl = URL.createObjectURL(file);
                    var media = Media({
                        url: localUrl,
                        key: key,
                        size: file.size,
                        name: file.name
                    });
                    deferred.resolve(media);
                    mediaCache[key] = media;                    
                };
            };
            addFile();
           
        }
        return deferred.promise;
    }

    

    function flushStore(db) {
        var deferred = $q.defer();
        var dbName = db || 'tempblob';
        var req = indexedDB.deleteDatabase(dbName);
        req.onupgradeneeded = function () {
            deferred.reject('upgradeneeded');
        };
        req.onsuccess = deferred.resolve;
        req.onerror = deferred.reject;
        req.onblocked = deferred.reject;
        return deferred.promise;
    }



    function getBinaryData(url,asUIntArray,asArrayBuffer) {
        var deferred = $q.defer();
        console.time('get binary string');
        var req = new XMLHttpRequest();
        req.open('GET', url, true);
        req.onload = function () {
            if (asArrayBuffer) {
                deferred.resolve(this.response);
            }
            else if (asUIntArray) {
                var uInt8Array = new Uint8Array(this.response); 
                for (var i = 0, len = uInt8Array.length; i < len; ++i) {
                    uInt8Array[i] = this.response[i];
                }
                deferred.resolve(uInt8Array);
            }
            else {
                deferred.resolve(req.responseText);
            }
            console.timeEnd('get binary data');
        }
        if (asUIntArray) {
            req.responseType = 'arraybuffer';
        } else {
            req.overrideMimeType('text\/plain; charset=x-user-defined');
        }
        req.send(null);
        return deferred.promise;
    
    }

    var appendBuffer = function (buffer1, buffer2) {
        var tmp = new Uint8Array(buffer1.byteLength + buffer2.byteLength);
        tmp.set(new Uint8Array(buffer1), 0);
        tmp.set(new Uint8Array(buffer2), buffer1.byteLength);
        return tmp.buffer;
    };

    function stringToUint(string) {
        string = btoa(unescape(encodeURIComponent(string))),
            charList = string.split(''),
            uintArray = [];
        for (var i = 0; i < charList.length; i++) {
            uintArray.push(charList[i].charCodeAt(0));
        }
        return new Uint8Array(uintArray);
    }

    function str2ab(str) {
        var buf = new ArrayBuffer(str.length * 2); // 2 bytes for each char
        var bufView = new Uint16Array(buf);
        for (var i = 0, strLen = str.length; i < strLen; i++) {
            bufView[i] = str.charCodeAt(i);
        }
        return buf;
    }

    return api;
}]);
wwt.app.factory('Places', ['$http', '$q', '$timeout', 'Util',
	function ($http, $q, $timeout, util) {
		
	var api = {
		getRoot: getRoot,
		getSolarSystemPlaces:getSolarSystemPlaces,
		getChildren: getChildren,
		openCollection: openCollection,
		importImage: importImage,
		findChildById: findChildById
	};
	var root,
		rootFolders,
		openCollectionsFolder;

	function getRoot() {
		var deferred = $q.defer();
		initPromise.then(function (folders) {
			rootFolders = folders;
			$.each(folders, function(i, item) {
			    item.guid = item.get_name();
			    if (item.get_thumbnailUrl) {
			        fixThumb(item);
			    }
			});
			transformData(folders);
			deferred.resolve(root.get_children());
		});
		return deferred.promise;
	}

        var fixThumb = function(item) {
            item.thumb = item.get_thumbnailUrl().replace("wwtstaging.azurewebsites.net/Content/Images/", "wwtweb.blob.core.windows.net/images/")
			        .replace("www.worldwidetelescope.org/Content/Images/", "wwtweb.blob.core.windows.net/images/")
                    .replace("worldwidetelescope.org/Content/Images/", "wwtweb.blob.core.windows.net/images/");
        }

	    function getChildren(obj) {
		var deferred = $q.defer();
		
		obj.childLoadCallback(function () {
			var children = obj.get_children();
			$.each(children, function (i, item) {
			    item.guid = obj.guid + '.' + (item.get_isFolder() ? item.get_name() : i);
			    if (item.get_thumbnailUrl) {
			        fixThumb(item);
			    }
			});
			deferred.resolve(transformData(children));
		});
		
		return deferred.promise;
	}

	function getSolarSystemPlaces() {
		var deferred = $q.defer();
		getRoot().then(function (rf) {
			getChildren(rf[1]).then(function(d) {
				deferred.resolve(d.slice(1));
			});
		});
		return deferred.promise;
	};

	var transformData = function(items) {
		$.each(items, function (i, item) {
			try {
				if (typeof item.get_type == 'function') {
					item.isPlanet = item.get_type() === 1;
					//item.isFolder = item.get_type() === 0;
					item.isFGImage = item.get_type() === 2 && typeof item.get_camParams == 'function';
				}
				if (typeof item.get_dataSetType == 'function') {
					item.isEarth = item.get_dataSetType() === 0;
					item.isPanorama = item.get_dataSetType() === 3;
					item.isSurvey = item.get_dataSetType() === 2;
					item.isPlanet = item.get_dataSetType() === 1;
				}
			} catch (er) {
				util.log(item, er);
			}
		});
		return items; 
	};

	var init = function () {
		var deferred = $q.defer();

		function tryInit() {
			if (!wwt.wc) {
				setTimeout(tryInit, 333);
				return;
			}
			root = wwt.wc.createFolder();
		
			root.loadFromUrl('http://www.worldwidetelescope.org/wwtweb/catalog.aspx?W=ExploreRoot', function () {
				var collection;
				if (util.getQSParam('wtml') != null) {
					openCollectionsFolder = wwt.wc.createFolder();
					openCollectionsFolder.set_name('Open Collections');
					collection = wwt.wc.createFolder();
					collection.loadFromUrl(util.getQSParam('wtml'), function() {
						collection.get_children();
						openCollectionsFolder.addChildFolder(collection);
						root.addChildFolder(openCollectionsFolder);
					    addVampFeeds();
						deferred.resolve(root.get_children());
					});
				} else if (location.href.indexOf('?image=') !== -1) {
				    addVampFeeds();
					importImage(location.href.split('?image=')[1]).then(function(data) {
						deferred.resolve(root.get_children());
					});

				} else {
				    addVampFeeds();
					deferred.resolve(root.get_children());
				}
			});
		}

		tryInit();
		
		return deferred.promise;
	};

	function openCollection(url) {
	    var deferred = $q.defer();
	    if (!openCollectionsFolder) {
	        openCollectionsFolder = wwt.wc.createFolder();
	        openCollectionsFolder.set_name('Open Collections');
	        openCollectionsFolder.guid = 'f0';
	        root.addChildFolder(openCollectionsFolder);
	    }
	    var collection = wwt.wc.createFolder();
	    collection.loadFromUrl(url, function () {
	        //collection.get_children();
	        collection.url = url;
	        openCollectionsFolder.addChildFolder(collection);
	        if (collection.get_name() == '') {
	            deferred.resolve(collection.get_children());
	        } else {
	            deferred.resolve(collection);
	        }
	    });
	    return deferred.promise;
	}

	function addVampFeeds() {
	    var vampFolder = wwt.wc.createFolder();
	    vampFolder.set_name('New VAMP Feeds');
	    vampFolder.guid = '0v0';
	    vampFolder.set_url('http://www.worldwidetelescope.org/wwtweb/catalog.aspx?W=vampfeeds');
	    root.addChildFolder(vampFolder);
	    
	}

	function importImage(url, manualData) {
		var deferred = $q.defer();
		if (!openCollectionsFolder) {
			openCollectionsFolder = wwt.wc.createFolder();
			openCollectionsFolder.set_name('Open Collections');
			root.addChildFolder(openCollectionsFolder);
		}
		var collection = wwt.wc.createFolder();
		
		collection.set_name("Imported image");
		//collection.url = url;
		
		var encodedUrl = url.indexOf('%2F%2F') != -1 ? url : encodeURIComponent(url);
		if (manualData) {
			encodedUrl += manualData;
		}
		collection.loadFromUrl('http://www.worldwidetelescope.org/WWTWeb/TileImage.aspx?imageurl=' + encodedUrl, function () {
			//collection.get_children();
			//collection.url = url;
			if (collection.get_children()[0].get_RA() != 0 || collection.get_children()[0].get_dec() != 0) {
				openCollectionsFolder.addChildFolder(collection);
				getChildren(collection).then(function(children) {
					if (collection.get_name() == '') {
						deferred.resolve(collection.get_children());
					} else {
						deferred.resolve(collection);
					}
				});
			} else {
				deferred.resolve(false);
			}


		});
		/*wwt.wc.add_collectionLoaded(function(data) {
			console.log(data);
			$.each(data, function(k, fld) {
				if (ss.canCast(fld, wwtlib.Folder)) {
					$.each(fld, function(ky, wf) {
						if (ss.canCast(wf, wwtlib.WebFile)) {
							console.log(wf);
						}
					});
				}
			});
			deferred.resolve(collection);
		});
		wwt.wc.loadImageCollection('http://www.worldwidetelescope.org/WWTWeb/TileImage.aspx?imageurl=' + encodeURIComponent(url));*/
		//});
		return deferred.promise;
	}

	function findChildById(guid, collection) {
		var place = null;
		guid = guid.replace(/_/g, ' ');
		$.each(collection, function (i, item) {
			if (item.guid === guid) {
				place = item;
			}
		});
		return place;
	}

	var initPromise = init();

	return api;
}]);


/*

folder
get_type = 0
get_subType = null
get_dataSetType = undefined

panorama
get_type = undefined
get_subType = undefined
get_dataSetType = 3
get_camParams = undefined

image (e.g. hubble galaxy)
get_type = 2
get_subType = undefined
get_dataSetType = undefined
get_camParams = function

survey
get_type = undefined
get_subType = undefined
get_dataSetType = 2
get_camParams = undefined

earth
get_type = undefined
get_subType = undefined
get_dataSetType = 0
get_camParams = undefined

planet
get_type = 1
get_subType = undefined
get_dataSetType = undefined

--or--

get_type = undefined
get_subType = undefined
get_dataSetType = 1

*/
wwt.app.factory('Tours', ['$rootScope', '$http', '$q', '$timeout', 'Util', function ($rootScope, $http, $q, $timeout, util) {
    var api = {
        getRoot: getRoot,
        getChildren: getChildren,
        getTourById: getTourById,
        getToursById: getToursById
    };
    var root;
    var rootFolders;
    var tourHash = {};

    function getTourById(id) {
        return tourHash[id.toLowerCase()];
    }
    function getToursById(guids) {
        if (!guids) return null;
        var tours = [];
        $.each(guids.split(';'), function (i, item) {
            var tour = getTourById(item);
            if (tour)
                tours.push(tour);
        });
        return guids.length > 1 ? tours : null;
    }

    function getRoot() {
        var deferred = $q.defer();
        initPromise.then(function () {
            rootFolders = root.get_children();
            deferred.resolve(transformData(rootFolders));
            $.each(rootFolders, function (i, folder) {
                var kids = folder.get_children();
                $.each(kids, function (j, tour) {
                    if (tour.get_isTour()) {
                        tourHash[tour.id.toLowerCase()] = tour;
                    }
                });
            });
        });
        return deferred.promise;
    }

    function getChildren(obj) {
        var deferred = $q.defer();

        obj.childLoadCallback(function () {
            deferred.resolve(transformData(obj.get_children()));


        });

        return deferred.promise;
    }

    var transformData = function (items) {
        $.each(items, function (i, item) {
            try {
                item.name = item.get_name();
                item.thumb = item.get_thumbnailUrl();
                //console.log(item.thumb);
                item.isFolder = item.get_isFolder();
                /*item.isImage = item.get_isImage();
                if (typeof item.get_type == 'function') {
                    item.isPlanet = item.get_type() === 1;
                    item.isFolder = item.get_type() === 0;
                    item.isFGImage = item.get_type() === 2 && typeof item.get_camParams == 'function';
                }
                if (typeof item.get_dataSetType == 'function') {
                    item.isEarth = item.get_dataSetType() === 0;
                    item.isPanorama = item.get_dataSetType() === 3;
                    item.isSurvey = item.get_dataSetType() === 2;
                }

                */

            } catch (er) {
                util.log(item, er);
            }
        });
        return items;
    };

    var init = function () {
        var deferred = $q.defer();

        root = wwt.wc.createFolder();
        var toursUrl = 'gettours_webclient.xml';
        root.loadFromUrl(toursUrl, function () {
            //root.refresh();
            deferred.resolve(root.get_children());
        });

        return deferred.promise;
    };



    var initPromise = init();
    return api;
}]);



wwt.app.factory('SearchData', [
	'$http', '$q', '$timeout', 'Places','Util', function($http, $q, $timeout, places, util) {
		var api = {
			getData: getData,
			getIndex: getIndex,
			importWtml:importWtml
		};
		var data,
			searchIndex = {},
			initPromise,
		    constellations = [];

		function getData() {
			var deferred = $q.defer();
			initPromise.then(function () {
				deferred.resolve(data);
			});
			return deferred.promise;
		};
		function getIndex() {
			var deferred = $q.defer();
			initPromise.then(function () {
				deferred.resolve(searchIndex);
			});
			return deferred.promise;
		};

		var deferredInit = $q.defer();
		var isId = 100;
	var init = function () {
		if (wwt.searchData) {
			wwt.searchDataIndexed = [];
			data = wwt.searchData;
			var start = new Date();
			
			$.each(data.Constellations, function (i, item) {
				/*if (item.name === 'SolarSystem') {
					item.places = ssData;
					return;
				}*/
				constellations[i] = item.name;
				$.each(item.places, function(j, place) {
					var fgi = place.fgi,
						imgSet;
					if (fgi) {
						isId++;
						
						imgSet = wwtlib.Imageset.create(
							fgi.n,//name
							fgi.u,//url
							fgi.dt || 2,//datasettype -default to sky
							fgi.bp,//bandPass
							fgi.pr,//projection
							isId,//imagesetid
							fgi.bl,//baseLevel
							fgi.lv,//levels
							null,//tilesize
							fgi.bd,//baseTileDegrees
							'',//extension
							fgi.bu,//bottomsUp
							fgi.q,//quadTreeTileMap,
							fgi.cX,//centerX
							fgi.cY,//centerY
							fgi.r,//rotation
							true,//sparse
							fgi.tu,//thumbnailUrl,
							fgi.ds,//defaultSet,
							false,//elevationModel
							fgi.wf,//widthFactor,
							fgi.oX,//offsetX
							fgi.oY,//offsetY
							fgi.ct,//creditsText
							fgi.cu,//creditsUrl
							'', '',
							0,//meanRadius
							null);
					}
					var pl = wwtlib.Place.create(
						place.n,//name
						place.d,//dec
						place.r,//ra
						place.c,//classification
						item.name,//constellation
						fgi ? fgi.dt : 2,//type
						place.z//zoomfactor
						);
					if (imgSet) {
						pl.set_studyImageset(imgSet);
					}

					if (item.name === 'SolarSystem') {
						$.each(pl, function(k, member) {
							if (ss.canCast(member, wwtlib.CameraParameters)) {
								member.target = wwtlib.SolarSystemObjects.undefined;
							}
						});
					}
					
					pl.guid = i + "." + j;
					//re-place js data with place obj
					item.places[j] = pl;

					indexPlaceNames(pl);

				});
			});
			var end = new Date();
			util.log('parsed places in ' + (end.valueOf() - start.valueOf()) + 'ms', data);
			importWtml('Wise.wtml').then(function () {
			    console.log('wise loaded');
			    importWtml('Hubble.wtml').then(function () {
			        console.log('hubble loaded');
			        importWtml('ESO.wtml').then(function () {
			            console.log('eso loaded');
			            importWtml('Chandra.wtml').then(function () {
			                console.log('chandra loaded'); 
			            });
			        });
			    });
		        deferredInit.resolve(data);
		    });
			
			
		} else {
			setTimeout(init, 333);
		}
		return deferredInit.promise;
	};

	var indexPlaceNames = function(pl) {
		var addPlace = function(s, place) {
			var firstChar = s.charAt(0).toLowerCase();
			if (firstChar === "'") firstChar = s.charAt(1).toLowerCase();
			if (searchIndex[firstChar]) {
				if (searchIndex[firstChar][searchIndex[firstChar].length - 1] !== place) {
					searchIndex[firstChar].push(place);
					wwt.searchDataIndexed = searchIndex;
				}
			} else {
			    try {
			        wwt.searchDataIndexed[firstChar] = searchIndex[firstChar] = [place];
			    } catch (er) {
			        console.error(er);
			    }
			}
		}

		$.each(pl.get_names(), function (n, name) {
			if (name.indexOf(' ') !== -1) {
				var words = name.split(' ');
				$.each(words, function(w, word) {
					addPlace(word, pl);
				});
			}
			else if (name.charAt(0)) {
				addPlace(name, pl);
			}
		});
	}

	function importWtml(wtmlPath) {
	    var deferred = $q.defer();
		
		$.ajax({
			url: wtmlPath
		}).done(function() {
			var wtml = $($.parseXML(arguments[0]));
			wtml.find('Place').each(function(i, place) {
				place = $(place);
				var constellation, ra = parseFloat(place.attr('RA')), dec = parseFloat(place.attr('Dec'));
				if (ra !== 0 || dec !== 0) {
					constellation = wwtlib.Constellations.containment.findConstellationForPoint(ra, dec); 
						
					var fgi = place.find('ImageSet').length ? place.find('ImageSet') : null;
					var wwtPlace = wwtlib.Place.create(
						place.attr('Name'),
						dec,
						ra,
						place.attr('DataSetType'),
						constellation,
						fgi ? util.getImageSetType(fgi.attr('DataSetType')) : 2, //type
						parseFloat(place.find('ZoomLevel')) //zoomfactor
					);
					if (fgi != null) {
						isId++;
						wwtPlace.set_studyImageset(wwtlib.Imageset.create(
								fgi.attr('Name'),
								fgi.attr('Url'),
								util.getImageSetType(fgi.attr('DataSetType')),
								fgi.attr('BandPass'),
								wwtlib.ProjectionType[fgi.attr('Projection').toLowerCase()],
								isId, //imagesetid
								parseInt(fgi.attr('BaseTileLevel')),
								parseInt(fgi.attr('TileLevels')),
								null, //tilesize
								parseFloat(fgi.attr('BaseDegreesPerTile')),
								fgi.attr('FileType'),
								fgi.attr('BottomsUp') === 'True',
								'', //quadTreeTileMap (I need to find a wtml file that has this and check spelling of the attr)
								parseFloat(fgi.attr('CenterX')),
								parseFloat(fgi.attr('CenterY')),
								parseFloat(fgi.attr('Rotation')),
								true, //sparse
								fgi.find('ThumbnailUrl').text(), //thumbnailUrl,
								false, //defaultSet,
								false, //elevationModel
								parseFloat(fgi.attr('WidthFactor')), //widthFactor,
								parseFloat(fgi.attr('OffsetX')),
								parseFloat(fgi.attr('OffsetY')),
								fgi.find('Credits').text(),
								fgi.find('CreditsUrl').text(),
								'', '',
								0, //meanRadius
								null)
						);
					}

					indexPlaceNames(wwtPlace);
					var cIndex = constellations.indexOf(constellation);
					var constellationPlaces = wwt.searchData.Constellations[cIndex].places;
					wwtPlace.guid = cIndex + '.' +
						constellationPlaces.length;
					constellationPlaces.push(wwtPlace);
				}


			});
			deferred.resolve(true);
		});
		
	    return deferred.promise;
	}

	

	initPromise = init();
	
	return api;
}]);
wwt.app.factory('Astrometry', [
	'$http', '$q', '$timeout', 'Util', function ($http, $q, $timeout, util) {
		var api = {
			submitImage: function (imageUrl, callback, debugFlag) {
				if (callback) {
					statusCallback = callback;
				}
				uploadUrl = imageUrl,
				sessionId = null,
				submissionId = null,
				jobId = null,
                debug = debugFlag,
				login();
			}
		};

    var statusTypes = {
        connecting: 'Connecting', 
        connected: 'Connect Success',
        connectFail: 'Connection Failed',
        uploading: 'Uploading Image',
        uploadSuccess: 'Upload Success',
        uploadFail: 'Upload Failed',
        statusCheck: 'Checking Status',
        statusCheckFail: 'Status Check Failed',
        jobFound: 'Job Found',
        jobStatusCheck: 'Checking Job Status',
        jobFail: 'Could Not Resolve Image',
        jobStatus: 'Solving Image',
        jobSuccess: 'Job Succeeded',
        calibrationFail: 'Calibration Results Failed'
    };
		var uploadUrl, // "http://www.noao.edu/outreach/aop/observers/m51rolfe.jpg",
			statusCallback,
			sessionId = null,
			submissionId = null,
			jobId = null, 
			calibration = null,
			jobStatus = null,
			errorData = null,
			debug=false;

		/*var upload = '{"session": "####", "url": "http://apod.nasa.gov/apod/image/1206/ldn673s_block1123.jpg", "scale_units": "degwidth", "scale_lower": 0.5, "scale_upper": 1.0, "center_ra": 290, "center_dec": 11, "radius": 2.0 }      ';*/


		function login() {
			showStatus(statusTypes.connecting);
			var loginData = {};
			loginData.apikey = "mxzoqrhqsvkwtybb"; // this may change we should put it in the web.config

			var loginJson = encodeURIComponent(JSON.stringify(loginData));

			$.ajax({
				url: "http://nova.astrometry.net/api/login",
				type: "POST",
				data: "request-json=" + loginJson,
				dataType: "json",
				crossDomain: true
			}).done(function (result) {
				sessionId = result.session;
				errorData = null;
				showStatus(statusTypes.connected);
				uploadImage();
			}).fail(function (xhr, ajaxOptions, error) {
				errorData = error;
				showStatus(statusTypes.connectFail);
			});

		}

		function uploadImage() {
			showStatus(statusTypes.uploading);
			var uploadData = {
				url: uploadUrl,
				session: sessionId
			};

			var uploadJson = encodeURIComponent(JSON.stringify(uploadData));

			$.ajax({
				url: "http://supernova.astrometry.net/api/url_upload",
				type: "POST",
				data: "request-json=" + uploadJson,
				dataType: "json",
				crossDomain: true
			}).done(function (result) {
				errorData = null;
				submissionId = result.subid;
				showStatus(statusTypes.uploadSuccess);
				checkStatus();
			}).fail(function (xhr, ajaxOptions, error) {
				errorData = error;
				showStatus(statusTypes.uploadFail);
			}); 
		}

		function checkStatus() {
			showStatus(statusTypes.statusCheck);
			$.ajax({
				url: "http://supernova.astrometry.net/api/submissions/" + submissionId,
				type: "GET",
				dataType: "json",
				crossDomain: true
			}).done(function (result) {
				$.each(result.jobs, function (i, job) {
					if (job != null) {
						jobId = job;
					}
				});
				if (jobId) {
					showStatus(statusTypes.jobFound);
					checkJobStatus();
				} else {
					setTimeout(checkStatus, 2000);
				}

			}).fail(function (xhr, ajaxOptions, error) {
				errorData = error;
				showStatus(statusTypes.statusCheckFail);
			});
		}


		function checkJobStatus() {
			//showStatus("Checking Job Status: " + jobId);
			$.ajax({
				url: "http://supernova.astrometry.net/api/jobs/" + jobId,
				type: "GET",
				dataType: "json",
				crossDomain: true
			}).done(function (result) {
				jobStatus = result.status;
				if (jobStatus.indexOf("fail") != -1) {
					showStatus(statusTypes.jobFail);
					return;
				}
				else if (result.status != "success") {
					if (debug) {
						calibration = {};
						calibration.ra = 202.45355674088898;
						calibration.dec = 47.20018130592933; 
						calibration.rotation = 122.97953942448784;
						calibration.scale = 0.3413275776344843;
						calibration.parity = 1;

						showStatus(statusTypes.jobSuccess);
					}
					showStatus(statusTypes.jobStatus);
					window.setTimeout(checkJobStatus, 5000);
				} else {

					getCalibration();
				}

			}).fail(function (xhr, ajaxOptions, error) {
				errorData = error;
				showStatus(statusTypes.jobFail);
			});
		}

		function getCalibration() {
			$.ajax({
				url: "http://supernova.astrometry.net/api/jobs/" + jobId + "/calibration",
				type: "GET",
				dataType: "json",
				crossDomain: true
			}).done(function (result) {
				calibration = {};
				calibration.ra = result.ra; // in degrees devide 15 for hours
				calibration.dec = result.dec; // in degrees
				calibration.rotation = result.orientation;
				calibration.scale = result.pixscale;
				calibration.parity = result.parity;
				calibration.radius = result.radius;

				showStatus(statusTypes.jobSuccess);
			}).fail(function (xhr, ajaxOptions, error) {
				errorData = error;
				showStatus(statusTypes.calibrationFail);

			});
		}

		function showStatus(status) {
			var statusData = {};
			statusData.sessionId = sessionId,
			statusData.submissionId = submissionId,
			statusData.jobId = jobId,
			statusData.calibration = calibration,
			statusData.jobStatus = jobStatus,
			statusData.errorData = errorData;
			$.each(statusTypes, function (type, message) {
				if (message === status) {
					statusData.status = type;
					statusData.message = message;
				}
			});
			statusCallback(statusData);

		}

		return api;
	}]);
wwt.app.factory('Community', ['$http', '$q', '$timeout', 'Util',
	function ($http, $q, $timeout, util) {
		
	var api = {
		getRoot: getRoot,
		getChildren: getChildren
		
	};
	var root,
		rootFolders,
		openCollectionsFolder;

	function getRoot() {
		var deferred = $q.defer();
		initPromise.then(function (folders) {
			rootFolders = folders;
		    $.each(folders, function(i,item) {
		        if (item.get_thumbnailUrl) {
		            item.thumb = item.get_thumbnailUrl().replace("http://wwtstaging.azurewebsites.net/Content/Images/", "https://wwtweb.blob.core.windows.net/images/");
		        }
		    });
			deferred.resolve(folders);
		});
		return deferred.promise;
	}

	function getChildren(obj) {
		var deferred = $q.defer();
		
		obj.childLoadCallback(function () {
			var children = obj.get_children();
			$.each(children, function (i, item) {
				item.guid = obj.guid + '.' + (item.get_isFolder() ? item.get_name() : i);
			});
			deferred.resolve(transformData(children));
		});
		
		return deferred.promise;
	}

	

	var init = function () {
		var deferred = $q.defer();

		function tryInit() {
			if (!wwt.wc) {
				setTimeout(tryInit, 333);
				return;
			}
			root = wwt.wc.createFolder();
		
			root.loadFromUrl('http://worldwidetelescope.org/Resource/Service/Payload', function () {
				deferred.resolve(root.get_children());
			});
		}

		tryInit();
		
		return deferred.promise;
	};

	
	var initPromise = init();

	return api;
}]);


wwt.controllers.controller('ContextPanelController',
	['$scope',
	'$rootScope',
	'$timeout', 
	'Util', 
	'SearchUtil',
	'ThumbList',
	function ($scope, $rootScope, $timeout, util, searchUtil, thumbList) {
	    
	    var lastUpdate = new Date();

	    var init = function () {
	        $scope.isContextPanel = true;
	        thumbList.init($scope, 'context');
            
	        $scope.placesInCone = [];
	        $scope.scrollDepth = 40;
	        $rootScope.$on('viewportchange', function (event, viewport) {
	            if ((!viewport.isDirty && !viewport.init) || new Date().valueOf() - lastUpdate.valueOf() > 2000) {
	                findNearbyObjects();
	                lastUpdate = new Date();
	            }
	        });
	        $scope.$watch('lookAt', findNearbyObjects);
	    };

	    $scope.clickThumb = function (item) {
	        thumbList.clickThumb(item, $scope);
	    };

	    $scope.hoverThumb = function (item) {
	        $scope.drawCircleOverPlace(item);
	    };

	    var calcPageSize = function () {
	        thumbList.calcPageSize($scope, true);
	    };

	    $(window).on('resize', function () {
	        $scope.currentPage = 0;
	        calcPageSize();
	        
	    });

	    $scope.moveNboMenu = function (i) {
	        $('#nboMenuContainer' + i).append($('#researchMenu'));
	    };
	    $scope.showMenu = function (item, i) {
	        $('.popover-content .close-btn').click();
	        setTimeout(function () {
	            $('.popover-content .close-btn').click();
	            $('#menuContainer' + i).find('.dropdown').addClass('open');
	        }, 10);
	        $scope.setMenuContextItem(item, true);
	    };
	    $scope.preventClickBubble = function (event) {
	        event.stopImmediatePropagation();
	    };

	    $scope.goBack = function () {
	        thumbList.goBack($scope);
	    };
	    $scope.goFwd = function () {
	        thumbList.goFwd($scope);
	    };

	    function findNearbyObjects() {
	        if (($scope.lookAt === 'Sky' || $scope.lookAt === 'SolarSystem') && !wwt.tourPlaying) {
	            searchUtil.findNearbyObjects({
	                lookAt: $scope.lookAt,
	                singleton: $rootScope.singleton
	            }).then(function (result) {
	                $scope.currentPage = 0;
	                $('body').append($('#researchMenu'));
	                $scope.collection = result;
	                if (util.isMobile) {
	                    $scope.setNBO($scope.collection);
	                }
	                calcPageSize($scope.collection);
	            });
	        }
	    }

	    init();

	}]);


/*
This controller is the hub of the web client - with all the shared functionality
that needs to live at the top of the scope chain residing here.

UIManager was created to add some functions to the rootScope that could be removed
from the main controller to reduce its weight. 

This file is too large and needs to be componentized a bit more. This is an ongoing
cleanup process.
*/
wwt.controllers.controller('MainController',
	['$scope',
	'$rootScope',
	'UILibrary',
	'$q',
	'AppState', 
	'Localization',
	'$timeout',
	'FinderScope',
	'SearchData',
	'Places',
	'Util',
	'HashManager',
	'Skyball',
	'SearchUtil',
	'$modal',
    '$element',
    '$cookies',
    'AutohidePanels',
	function ($scope, $rootScope, uiLibrary, $q, appState, loc, $timeout, finderScope, searchDataService, places, util, hashManager, skyball, searchUtil, $modal, $element, $cookies, AutohidePanels) {
        //TODO - figure out how to clean up lame long list of dependencies injected
		var ctl;
		 
		//#region LookAt/Imagery 
		var initialPass = true;
		$scope.lookTypes = ['Earth', 'Planet', 'Sky', 'Panorama', 'SolarSystem'];
		$scope.lookAt = 'Sky';
		$scope.imagery = [[], [], [], [], []];

        
		$scope.lookAtDropdownChanged = function (lookAtType) {
			if (lookAtType) {
				$scope.lookAt = lookAtType;
			}
			setTimeout(function() {
				$scope.lookAtChanged(null, true);
				$scope.setTrackingObj(false);
				
			}, 1);
		};

		$scope.lookAtChanged = function (imageryName, dropdownInvoked, noUpdate, keepCamera) {
			setTimeout(wwt.resize, 120);
			if (!keepCamera) {
				util.resetCamera(true);
			}
			$timeout(function () {
				if ($('#lstLookAt').length) {
					$scope.lookAt = $('#lstLookAt option:selected').text();
				}
				if ($scope.lookAt === '') { 
					$scope.lookAt = 'Sky';
				}
				var collection = $scope.imagery[$.inArray($scope.lookAt, $scope.lookTypes)];
				if (collection[0] !== '-')
				    collection.splice(0, 0, '-');
				if (imageryName == '') imageryName = '-';
				$scope.surveys = collection;
				var foundName = false;
				
				
				// HACK ALERT (Mars was hardcoded from Visible Imagery)
				if (imageryName === 'Mars') {
					imageryName = 'Visible Imagery';
				}
				if (imageryName) {
					$.each(collection, function (i,item) {
					    if (item !== '' && item.get_name() && (item.get_name().indexOf(imageryName) === 0 || imageryName.indexOf(item.get_name()) === 0)) {
					        $scope.backgroundImagery = item;
							foundName = true;
						}
					});
				} if (!foundName) {
					if (initialPass || dropdownInvoked) {
						setTimeout(function() { initialPass = false; }, 500);
						$timeout(function() {
							$scope.backgroundImagery = collection[1];
							$scope.setSurveyBg();

						}, 123);
						return;
					} else if (!noUpdate) {
						$scope.backgroundImagery = collection[0];
						return;
					} else {
						return;
					}
				}
				$scope.setSurveyBg();
			},100);
		};
		$scope.setLookAt = function (lookAt, imageryName, noUpdate, keepCamera) {
		    $scope.lookAt = lookAt;
		    //if (lookAt === 'Planet' && !imageryName) {
		    //    imageryName = 'Mars';
		    //}
			$scope.lookAtChanged(imageryName, false, noUpdate, keepCamera);
			setTimeout(wwt.resize, 1200);
		};
		//#endregion

		//#region initialization
		var initCanvas = function() {
		    ctl = $rootScope.ctl = wwtlib.WWTControl.initControlParam("WWTCanvas", appState.get('WebGl'));

		    // The .8 release of scriptsharp changed the location of the canCast function
		    // This logic exists to ensure backwards compatibility when testing an older version
            // of the framework.
			if (window.Type && Type.canCast) {
			    if (window.ss) {
			        window.ss.canCast = Type.canCast;
			    } else {
			        window.ss = { canCast: Type.canCast };
			    
			    }
		    }
		    wwt.wc = ctl;
			wwt.resize();
			ctl.add_ready(function() {
				var imageSets = wwtlib.WWTControl.imageSets;
				$scope.surveys = [];
				$.each(imageSets, function () {
					var typeIndex = this.get_dataSetType();
					this.name = this.get_name() === 'Visible Imagery' ? 'Mars' : this.get_name();
					if (typeIndex === 2 && this.name.toLowerCase().indexOf('hipparcos') !== -1) {//hipparcos is broken :(
						$scope.surveys.push(this);
					}
					try {
						if (!(typeIndex === 2 && this.name.toLowerCase().indexOf('hipparcos') !== -1)) {//hipparcos is broken :(
							$scope.imagery[typeIndex].push(this);
						}
					} catch (er) {
						util.log(typeIndex,this);
					}
				});
				$scope.backgroundImagery = {
					name: 'Digitized Sky Survey (Color)',
					get_name: function() {
						return 'Digitized Sky Survey (Color)';
					}
				};
				$scope.lookAtChanged();
				AutohidePanels.init();

			});
			ctl.settings.set_showConstellationBoundries(false);
			
			util.resetCamera(true);
			$(window).on('resize', wwt.resize);
			ctl.endInit();
			$rootScope.singleton = wwtlib.WWTControl.singleton;
			initContext();
			$rootScope.$on('hashChange', hashChange);
			
			$timeout(function () {
				var hash = hashManager.getHashObject();
				$rootScope.$broadcast('hashChange', hash);
			}, 100);
			
			//hashChange(null, hashManager.getHashObject());
		};

		var hashChange = function (e, obj) {
		    var goto = function () {
		        ctl.gotoRaDecZoom(
					parseFloat(obj['ra']) * 15,
					parseFloat(obj['dec']),
					parseFloat(obj['fov']),
					false
				);
		    }

		    if (obj['place']) {
		        var openPlace = obj['place'];
		        if (!isNaN(parseInt(openPlace.charAt(0)))) {
		            $('#loadingModal').modal('show');
		            searchUtil.getPlaceById(openPlace).then(function (place) {
		                $scope.setForegroundImage(place);
		                if (obj['ra']) {
		                    setTimeout(function () {
		                        goto();
		                        if (obj['cf']) {
		                            $('.cross-fader a.btn').css('left', parseFloat(obj['cf']));

		                            var ensureProperOpacity = function () {
		                                ctl.setForegroundOpacity(parseFloat(obj['cf']));
		                            };
		                            for (var i = 1; i < 6; i++) {
		                                setTimeout(ensureProperOpacity, i * 1000);
		                            }
		                        }

		                    }, 3333);
		                }
		                $('#loadingModal').modal('hide');
		                
		            });
		        }
		    }
		    else if (obj['ra'] !== undefined) {
		        goto();
		    }
		    if (obj['lookAt']) {
		        $timeout(function () {
		            $scope.setLookAt(obj['lookAt'], obj['imagery']);
		            if (obj['ra'] && (obj['lookAt'] === 'Earth' || obj.lookAt === 'Planet')) {

		                setTimeout(goto, 2220);

		            }
		            
		        }, 2000);

		    }
		    else if (obj['imagery']) {
		        $timeout(function () { $scope.setLookAt('Sky', obj['imagery']); }, 2000);
		        
		    }


		}

		$scope.initUI = function() {
			$scope.ribbon = {
				tabs: [
				{
					label: 'Explore', 
					button: "rbnExplore",
					mobileLabel: 'Explore Collections',
						mobileAction: function() {
							$('#exploreModalLink').click();
						},
					menu: {
						Open: {
							'Tour...': [$scope.openItem, 'tour'],
							'Collection...': [$scope.openItem, 'collection'],
							'Image...': [$scope.openItem, 'image']
						},
						sep1: null,
						'Tour WWT Features': [$scope.tourFeatures],
						'Show Welcome Tips':[showTips],
						'Show Finder (right click)': [$scope.showFinderScope],
						'WorldWide Telescope Home': [util.nav, '/home'],
						'Getting Started (Help)': [util.nav, '/Learn/'],
						'WorldWide Telescope Terms of Use': [util.nav, '/Terms'],
						'About WorldWide Telescope': [util.nav, '/About']/*,
						sep2: null,
						'Exit': [util.nav, 'Exit'],*/
					}
				},{
					label:'Guided Tours',
					button:'rbnTours',
					menu: {
                        
					    'Tour Home Page': [util.nav, '/Learn/Exploring#guidedtours'],
					    'Music and other Tour Resources': [util.nav, '/Download/TourAssets'],
					    sep2: null,
					    
					    'Create a New Tour...': [$scope.createNewTour],
					    
					}
				}, {
				    label: 'Search',
				    button: 'rbnSearch',
				    menu: {
				        'Search Now': [function () { $timeout(function () { changePanel('Search'); }); }]
				    }
				},
                {
                    label: 'Communities',
                    button: 'rbnCommunities',
                    menu: {
                        'Search Communities': [util.nav, '/Community']
                    }
                }, {
					label:'View',
					button:'rbnView',
					menu: {
						'Reset Camera': [util.resetCamera],
						'Share this View': [copyShortcut],
						'Toggle Full Screen View (F11)': [util.toggleFullScreen],
						'Toggle Layer Manager': [$scope.toggleLayerManager]
					}
				},{
					label:'Settings',
					button: 'rbnSettings',
					menu: {
						'Restore Defaults': [$scope.restoreDefaultSettings],
						'Product Support': [util.nav, '/Support/IssuesAndBugs']
					}
				}]
			};
			if (util.getQSParam('ads')) {
				$scope.ribbon.tabs.push({
					label: 'ADS',
					button: 'rbnADS',
					menu: {
						'ADS Home Page': [function() {
							window.open('http://www.adsass.org/wwt');
						}]
					}
				});
			}
			$scope.activePanel = util.getQSParam('ads') ? 'ADS' : 'Explore';

			$scope.UITools = wwtlib.UiTools;
			$scope.Planets = wwtlib.Planets;

			$rootScope.$on('viewportchange', viewportChange);
			util.trackViewportChanges();
			skyball.init();
			
			

			$(window).on('keydown', function (e) {
				if (e.which === 187) {
					ctl.zoom(.66666666666667);
				}else if (e.which === 189){
					ctl.zoom(1.5);
				}
			});
			
		};

        var changePanel = function(panel) {
            $('body').append($('#researchMenu'));
            $scope.expandTop(false);
            $scope.activePanel = panel;
        }

	    var initContext = function () {
	        var isAds = util.getQSParam('ads') != null;
	        
	        
	        var bar = $('.cross-fader a.btn').css('left', isAds ? 50 : 100);

			var xf = new wwt.Move({
				el: bar,
				bounds: {
					x: [isAds ? -50 : -100, isAds ? 50 : 0],
					y: [0, 0]
				},
				onstart: function () {
					bar.addClass('moving');
				},
				onmove: function () {
					ctl.setForegroundOpacity(this.css.left);
				},
				oncomplete: function () {
					bar.removeClass('moving');
				}
			});

			wwt.resize();
            if (util.getQSParam('tourUrl')) {
	            $scope.playTour(decodeURIComponent(util.getQSParam('tourUrl')));
	        }
		};
		//#endregion 


		//#region viewport/finderscope
		var viewportChange = function(event,viewport) {
			if (viewport.isDirty || viewport.init) {
				$rootScope.viewport = viewport;
				$scope.coords = wwtlib.Coordinates.fromRaDec(viewport.RA, viewport.Dec);
				$scope.formatted = {
					RA: util.formatHms(viewport.RA, true),
					Dec: util.formatHms(viewport.Dec, false, true),
					Lat: util.formatHms($scope.coords.get_lat(), false, false),
					Lng: util.formatHms($scope.coords.get_lng(), false, false),
					Zoom: util.formatHms(viewport.Fov)
				}
				trackConstellation();
				if (viewport.init) {
					$timeout(trackConstellation, 1200);
				}
			}
			if ((viewport.isDirty || viewport.finderMove) && checkVisibleFinderScope()) {
				var found = finderScope.scopeMove();
				if (found) {
					$timeout(function() {
						$scope.scopePlace = found; 
						$scope.drawCircleOverPlace($scope.scopePlace);
					});
					
				}
			}
		}

		var trackConstellation = function() {
			$scope.formatted.Constellation = $scope.constellations.fullNames ? $scope.getFromEn($scope.constellations.fullNames[$rootScope.singleton.constellation]) : '...';
		}

		var checkVisibleFinderScope = function() {
			if ($('.finder-scope:visible').length) {
				finderActive = true;
			} else if (finderActive) {
				finderActive = false;
				clearInterval(finderTimer);
			}
			return finderActive;
		}

		$scope.$on('showFinderScope', function () {
			$scope.showFinderScope();
		});

		$scope.$on('showContextMenu', function () {
		    $scope.showContextMenu();
		});

		var finderTimer, finderActive = false,finderMoved = true;
		$scope.showFinderScope = function (event) {
			if ($scope.lookAt === 'Sky' && !$scope.editingTour) {
				var finder = $('.finder-scope');
				finder.toggle(!finder.prop('hidden')).css({
					top: event ? event.pageY - 88 : 180,
					left: event ? event.pageX - 301 : 250
				});
				if (finder.prop('hidden')) {
					finder.prop('hidden', false);
					finder.fadeIn(function() {
						if (!finder.prop('movebound')) {
							var finderScopeMove = new wwt.Move({
								el: finder,
								target: finder.find('.moveable'),
								onmove: function () {
									finderMoved = true;

								}
							});
						}
						finder.prop('movebound', true);
					});
				}
				finderScope.init();
				if (event) {
					event.preventDefault();
				}
				finderTimer = setInterval(pollFinder, 400);
				viewportChange(null, { finderMove: true });
			}
		};

		var pollFinder = function() {
			if (checkVisibleFinderScope()) {
				if (finderMoved) {
					viewportChange(null, { finderMove: true });
					finderMoved = false;
				}
			}
		}

		$scope.initFinder = function () {
			searchDataService.getData().then(function () {

				var finder = $('.finder-scope').prop('hidden', true).fadeOut();
				finder.find('.close, .close-btn').on('click', function () {
					finder.fadeOut(function () { finder.prop('hidden', true); });
				});
				

				//$('#WWTCanvas').on('contextmenu', $scope.showFinderScope);
				$scope.showObject = function (place) {
					$rootScope.singleton.gotoTarget(place);
					$('.finder-scope').hide();
				}
			});
		};

		//#endregion

		//#region set fb/bg...
		var solarSystemInit = false;
		$scope.setSurveyBg = function (imageryName, imageSet) {

			if (imageryName) {
				if (imageryName === 'Mars') {
					imageryName = 'Visible Imagery';
				}
				var foundName = false;
				$.each($scope.surveys, function () {
					if (this.name &&(this.name.indexOf(imageryName) === 0 || imageryName.indexOf(this.name) === 0)) {
						$scope.backgroundImagery = this;
						foundName = true;
					}
				});
				if (!foundName) {
					$scope.backgroundImagery = '';
					ctl.setBackgroundImageByName(imageryName);
					if (imageSet) {
					    $rootScope.singleton.renderContext.set_backgroundImageset(imageSet);
					}
					return;
				} 
			}

			if ($scope.backgroundImagery) {
			    if ($scope.backgroundImagery !== '?')
			        ctl.setBackgroundImageByName($scope.backgroundImagery.get_name());
			    if (typeof $scope.backgroundImagery != 'string' && $scope.backgroundImagery.get_name() === '3D Solar System View' && !solarSystemInit) {
			        setTimeout(function () {
			            var bar = $('.planetary-scale .btn');
			            var ps = new wwt.Move({
			                el: bar,
			                bounds: {
			                    x: [0, 66],
			                    y: [0, 0]
			                },
			                onstart: function () {
			                    bar.addClass('moving');
			                },
			                onmove: function () {
			                    ctl.settings.set_solarSystemScale(Math.max(this.css.left * 1.5, 1));
			                },
			                oncomplete: function () {
			                    bar.removeClass('moving');
			                }
			            });
			            solarSystemInit = true;
			        }, 10);
			    }
			}

		};

		$scope.setSurveyProperties = function() {
			$scope.propertyItem = $scope.backgroundImagery;
			$scope.propertyItem.isSurvey = true;
		};

		$scope.setActiveItem = function (item) {
			$scope.activeItem = item;
			if (item.guid) {
				$scope.shareUrl = hashManager.setHashVal('place', item.guid, true, true);

			}
			if (item.get_studyImageset) {
				$scope.activeItem.imageSet = item.get_studyImageset();
			}
		};

		$scope.setForegroundImage = function (item) {
			if (item.guid) {
				$scope.shareUrl = hashManager.setHashVal('place', item.guid, true, true);
			}
			if (util.isMobile) {
				$('#explorerModal').modal('hide');
				$('#nboModal').modal('hide');
			}

			var imageSet = util.getImageset(item);
			if (imageSet && !item.isEarth) {

			    wwtlib.WWTControl.singleton.renderContext.set_foregroundImageset(imageSet);
			}
			$scope.setTrackingObj(item);

			if (!item.isSurvey && ss.canCast(item, wwtlib.Place)) {
				$('.finder-scope').hide();
				//$('.cross-fader').parent().toggle(imageSet!=null);
				$rootScope.singleton.gotoTarget(item, util.getIsPlanet(item), false, true);

				return;
			} else if (!item.isEarth) {
				ctl.setForegroundImageByName(imageSet.get_name());
			} else {
				$rootScope.singleton.renderContext.set_backgroundImageset(imageSet);
			}

			//$('.cross-fader').parent().show();

		};
		$scope.setBackgroundImage = function (item) {
			var imageSet = util.getImageset(item);
			if (imageSet) {
				$rootScope.singleton.renderContext.set_backgroundImageset(imageSet);
			}
			if (!item.isSurvey) {
				$rootScope.singleton.gotoTarget(item, false, false, true);
			}
		};
		//#endregion

		
		//#region menu actions
		$scope.menuClick = function (menu) {
			$scope.keepMenu = true;
			var m = $('#topMenu');
			m.html('');
			$.each(menu, function (menuItem, action) {
				var item;
				if (menuItem.indexOf('sep') === 0) {
					item = $('<li class="divider" role="presentation"></li>');
				} else {
					item = $('<li><a href="javascript:void(0)"></a></li>');
					item.find('a').text(loc.getFromEn(menuItem));
					if ($.isPlainObject(action)) {
						item.addClass('dropdown-submenu').find('a').attr('tab-index', -1);
						var sub = $('<ul class=dropdown-menu></ul>');
						item.append(sub);
						$.each(action, function (subItemLabel, subItemAction) {
							var subItem = $('<li><a href="javascript:void(0)"></a></li>');
							subItem.find('a').on('click', function () {
								subItemAction[0](subItemAction[1]);
							}).data('action', subItemAction).text(loc.getFromEn(subItemLabel));
							sub.append(subItem);
						});
					} else {
						item.find('a').on('click', function () {
							action[0](action[1]);
						}).data('action', action);
					}
				}
				m.append(item);
			});
			var caret = $('#tabMenu' + this.$index);
			m.css({
				top: caret.offset().top + caret.height(),
				left: caret.offset().left
			}).show();
			setTimeout(function () {
				$(document).on('click', hideMenu);
				$scope.keepMenu = false;
			}, 123);

		};

		var hideMenu = function () {
			if ($scope.keepMenu) {
				return;
			}
			$('#topMenu').hide();
			$(document).off('click', hideMenu);
		};
		$scope.tabClick = function (tab) {
		    if ($rootScope.editingTour) {
		        //$scope.finishTour();
		    }
		    $('body').append($('#researchMenu'));
			$scope.expandTop(false); 
			$scope.activePanel = tab.label;
			appState.set('activePanel', tab.label);
		};
		$scope.openItem = function (type) {
		    $scope.$applyAsync(function () {
		        $rootScope.openType = type;
		        $rootScope.$broadcast('openItem');
		        if (type === 'collection') {
		            $scope.tabClick($scope.ribbon.tabs[0]);
		        }
		        $('#openModal').modal('show');
		    });
		};

		$scope.playTour = function (url) {
		    
	        util.goFullscreen();
		    console.log(encodeURIComponent(url));
	        $('.finder-scope').hide();
	        wwtlib.WWTControl.singleton.playTour(url);
	        $scope.$applyAsync(function () {
                wwt.tourPlaying = $rootScope.tourPlaying = true;
	            $rootScope.tourPaused = false;
            });
	        wwt.wc.add_tourEnded(tourChangeHandler);
	        wwt.wc.add_tourReady(function() {	            
	            
	            $scope.$applyAsync(function () {
	                $scope.activeItem = { label: 'currentTour' };
	                $scope.activePanel = 'currentTour';
	                $scope.ribbon.tabs[1].menu['Edit Tour'] = [$scope.editTour];
	                    
	            });
	            
	        });
	        //wwt.wc.add_tourPaused(tourChangeHandler);

		};

		$scope.editTour = function () {
		    $rootScope.$applyAsync(function () {
		        $rootScope.editingTour = true;
		    });
		};
       $scope.initSlides = function(){
            $rootScope.$broadcast('showingSlides');
        };

		$rootScope.closeTour = function ($event) {
		    util.exitFullscreen();
		    $event.preventDefault();
		    $event.stopPropagation();
		    if (wwtlib.WWTControl.singleton.tourEdit.get_tour().get_tourDirty() && !confirm('You have unsaved changes. Close this tour and lose changes?')) {
		        return;
		    }
		    $rootScope.editingTour = false;
		    delete $scope.ribbon.tabs[1].menu['Edit Tour'];
		    delete $scope.ribbon.tabs[1].menu['Show Slide Overlays'];
		    delete $scope.ribbon.tabs[1].menu['Show Slide Numbers'];
		    
		    wwtlib.WWTControl.singleton.stopCurrentTour();
		    $rootScope.$broadcast('closeTour');
		    //wwtlib.WWTControl.singleton.tour.cleanUp();
		    $scope.$applyAsync(function () {
		        $scope.activePanel = 'Guided Tours';
	            $rootScope.editingTour = false;
		        $rootScope.tourPlaying = false;
		        $rootScope.currentTour = null;
	        });
	    }

		$scope.createNewTour = function () {
		    $scope.$applyAsync(function () {
		        //todo show dialog for tour properties
		        $rootScope.currentTour = wwtlib.WWTControl.singleton.createTour("New Tour");

		        $scope.activeItem = { label: 'currentTour' };
		        $scope.activePanel = 'currentTour';
		        $rootScope.$applyAsync(function () {
		            $rootScope.editingTour = true;
		            $rootScope.currentTour._editMode = true;
		        });
		    });
	    };

		function tourChangeHandler() {
		    $rootScope.$broadcast('tourFinished');
		    var settings = appState.get('settings') || {};
		    $scope.$applyAsync(function () {
			    wwt.tourPlaying = $rootScope.tourPlaying = false;
            });
			$rootScope.landscapeMessage = false;
			
			ctl.clearAnnotations();
		}

		var shareModal = $modal({
			contentTemplate: 'views/popovers/shareplace.html',
			show: false,
			scope: $scope
		});
		var copyShortcut = function() {
			shareModal.$promise.then(shareModal.show);
		};
		$scope.restoreDefaultSettings = function() {
			$rootScope.$broadcast('restoreDefaults');
		};
		var showTips = function() {
			$('#introModal').modal('show');
		};
		//#endregion

		//#region localization
		
		$scope.selectedLanguage = 'EN';
		$scope.setLanguageCode = function(code) {
			appState.set('language', code);
			$timeout(function() {
				if ($scope.selectedLanguage !== code) {
					$scope.selectedLanguage = code;
					$scope.languageCode = code;
				}
			}, 200);
			$rootScope.languagePromise = loc.setLanguage(code);
		};
		
		//appState.set('language', 'EN');
		$scope.setLanguageCode(appState.get('language') || 'EN');
		$scope.locString = function(id) {
			var deferred = $q.defer();
			$rootScope.languagePromise.then(function () {
				localized[id] = loc.getString(id);
				deferred.resolve(localized[id]);
			});
			return deferred.promise;
		};
		
		var localized = [];
		var locCalls = 0;
		$scope.getFromEn = function (englishString) {
			locCalls++;
			if (locCalls % 100 == 0) {
				//util.log('loc calls: ' + locCalls);
			}
			var key = englishString + $scope.selectedLanguage;
			if ($scope.selectedLanguage === 'EN') {
				localized[key] = englishString;
			}
			if (localized[key]) {
				return localized[key];
			}

			var deferred = $q.defer();
			$rootScope.languagePromise.then(function () {
				//var key = englishString + $scope.selectedLanguage;
				if ($scope.selectedLanguage == 'EN') {
					localized[key] = englishString;
				}
				else  {
					localized[key] = loc.getFromEn(englishString);
				}
				deferred.resolve(localized[key]);
			});
			return deferred.promise;
			//return null;
		};
		loc.getAvailableLanguages().then(function (result) {
			$scope.availableLanguages = result;
		});

		//static localizable strings that should be calculated once to prevent endless looping
		$rootScope.loc = {
			na: '',
			neverRises: ''
		};

		
		$rootScope.languagePromise.then(function (result) {
			$rootScope.na = loc.getFromEn('n/a');
			$rootScope.neverRises = loc.getFromEn('Never Rises');
			$scope.hideIntroModal = appState.get('hideIntroModalv2');
			if (!$scope.hideIntroModal && !$scope.loadingUrlPlace) {
			    if (localStorage.getItem('login')) {
			        var now = new Date().valueOf();
			        var loginTime = parseInt(localStorage.getItem('login'));
			        if (now - loginTime < 33333) {
			            return;//no autoshow popup when logged in within last 30sec
			        }
			    }
				setTimeout(showTips,1200);
			}
		});
		//#endregion

		//#region view helpers

		$scope.formatHms = function (angle, isHmsFormat, signed, spaced) {
			return util.formatHms(angle, isHmsFormat, signed, spaced);
		};
		$scope.formatDecimalHours = function (dayFraction, spaced) {
			var split = wwtlib.UiTools.formatDecimalHours(dayFraction).split(':');
			if (parseInt(split[0]) < 10) split[0] = '0' + split[0];
			if (parseInt(split[1]) < 10) split[1] = '0' + split[1];
			return split.join(' : ');
			//return util.formatDecimalHours(dayFraction, spaced == undefined ? true : spaced);test
		}

		$rootScope.showTrackingString = function() {
			return ($scope.trackingObj && $(window).width() > 1159);
		}

	    $rootScope.showCrossfader = function() {
	        var show = false;
	        if ($scope.activePanel === 'ADS') {
	            return true;
	        }
	        try {
	            if ($scope.lookAt === 'Sky' && $scope.trackingObj && (util.getImageset($scope.trackingObj) != null)) {
	                if ($(window).width() > 800 || util.isMobile) {
	                    show = true;
	                }
	            }
	        } catch (er) {
	            show = false;
	        }
	        return show;
	    };
		
	    $scope.hideIntroModalChange = function (hideIntroModal) {
	        appState.set('hideIntroModalv2', hideIntroModal);
	    };
	    $scope.iswebclientHome = $cookies.get('homepage') !== 'home';
	    $scope.homePrefChange = function (isWebclient) {
	        $cookies.remove('homepage');
	        if (!isWebclient) {
	            $cookies.put('homepage', 'home', { expires: new Date(2050, 1, 1), path: "/" });
	        } else {
	            $cookies.put('homepage', 'webclient', { expires: new Date(2050, 1, 1), path: "/" });
	        }
	    };
		
		$scope.setMenuContextItem = function(item,isExploreTab) {
			$scope.menuContext = item;
			$scope.propertyItem = item;
			$scope.propertyItem.isExploreTab = isExploreTab;
		};
        
		$scope.showProperties = function () {
			$('.popover-content .close-btn').click();
			$('.dropdown.open #researchMenu, .dropup.open #researchMenu').closest('.thumbwrap').find('.thumb-popover').click();
		};

		$scope.setTrackingObj = function(item) {
			$scope.trackingObj = item;
			if ($scope.trackingObj === null) {
				hashManager.removeHashVal('place', true);
			}
		};
	    $scope.showMobileTracking = function() {
	        return $scope.trackingObj &&
	            $scope.trackingObj.get_name &&
	            !$scope.tourPlaying &&
	            $scope.lookAt !== 'Earth' &&
	            $scope.lookAt !== 'Planet' &&
	            $scope.lookAt !== 'Panorama';
	    };

	    $scope.displayXFader = function () { 
	        return (
                $scope.lookAt === 'Sky' &&
	            $scope.trackingObj &&
                !$scope.tourPlaying &&
                ($scope.trackingObj.get_backgroundImageset() != null || $scope.trackingObj.get_studyImageset() != null));
	    }

	    $scope.gotoConstellation = function(c) {
			$rootScope.singleton.gotoTarget(wwtlib.Constellations.constellationCentroids[c], false, false, true);
		}

		$scope.drawCircleOverPlace = function(place) {
			util.drawCircleOverPlace(place);
		}
		$scope.clearAnnotations = function() {
			ctl.clearAnnotations();
		};

		$scope.topExpanded = false;
		$scope.expandTop = function(flag, panel) {
		    $scope.topExpanded = flag;
		    $scope.expandedPanel = panel;
		}

		$scope.tourFeatures = function () {
			$scope.loadingTour = true;
			setTimeout(function() {$('#introStartButton').click();}, 3);
			setTimeout(function () {
				$('#btnCloseIntro').click();
				 $scope.loadingTour = false;
			}, 1000);
		};

		initCanvas();

		$scope.constellations = wwtlib.Constellations;
		
		$scope.nbo = [];
		$scope.setNBO = function (nbo) {
			$scope.nbo = nbo;
			$scope.nboCount = nbo.length;
			if ($scope.isLoading) {
				$scope.isLoading = false;
				//util.log(new Date().valueOf() - time.valueOf());
			}
			
		}
		$scope.hideMenu = function () {
			$('.navbar-collapse.in').removeClass('in').addClass('collapse');
		}
		$scope.showNbo = function() {
			$('#nboModalLink').click();
			$scope.hideMenu();
		}
		$scope.isLoading = true;
		//var time = new Date();
		$scope.fovClass = function () {
			return $scope.lookAt === 'Planet' || $scope.lookAt === 'Panorama' || $scope.lookAt === 'Earth' ? 'hide' :
				$scope.lookAt === 'SolarSystem' ? 'solar-system-mode fov-panel' :
				'fov-panel';
		}
		$scope.contextPanelClass = function () {
		    var cls = $scope.lookAt === 'Planet' || $scope.lookAt === 'Panorama' || $scope.lookAt === 'Earth' ? 'context-panel compressed' : 'context-panel';
		    if ($rootScope.tourPlaying) {
		        cls += ' hide';
		    }
		    return cls;
		}
		$scope.contextPagerRight = function() {
			return /*$scope.fovClass() != 'hide' && */ $scope.showTrackingString() ? 0 : 50;
		}
		if (util.getQSParam('editTour')) {
		    $scope.playTour(decodeURIComponent(util.getQSParam('editTour')));
		    $scope.autoEdit = true;
		}
		if (util.getQSParam('playTour')) {
		    $scope.playTour(decodeURIComponent(util.getQSParam('editTour')));
		    
		}
	}
]);

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
wwt.controllers.controller('MobileNavController',
['$rootScope',
	'$scope',
	'Util',
	'$modal',
	'Localization',
	function ($rootScope, $scope, util, $modal,loc) {

		

		$scope.showModal = function (modalButton) {
			$scope.hideMenu();
			if (typeof modalButton.modal == 'object') {
				modalButton.modal.$promise.then(modalButton.modal.show);
			} else {
				$(modalButton.modal).modal('show');
			}
		}

		$rootScope.languagePromise.then(function() {

			$scope.modalButtons = [
				{
					text: loc.getFromEn('Explore'),
					icon: 'fa-binoculars',
					modal: '#explorerModal'
				},
				{
					text: loc.getFromEn('Tours'),
					icon: 'fa-play',
					modal: $modal({
						contentTemplate: 'views/modals/mobile-tours.html',
						show: false,
						scope: $scope
					})
				},
				{
					text: loc.getFromEn('Search'),
					icon: 'fa-search',
					modal: $modal({
						contentTemplate: 'views/modals/mobile-search.html',
						show: false,
						scope: $scope,
						prefixEvent: 'searchModal'
					})
				},
				{
					text: loc.getFromEn('View'),
					icon: 'fa-eye',
					modal: $modal({
						contentTemplate: 'views/modals/mobile-view.html',
						show: false,
						scope: $scope
					})
				},
				{
					text: loc.getFromEn('Settings'),
					icon: 'fa-gears',
					modal: $modal({
						contentTemplate: 'views/modals/mobile-settings.html',
						show: false,
						scope: $scope
					})
				},
				{
					text: loc.getFromEn('Layers'),
					icon: 'fa-align-left',
					modal: $modal({
						contentTemplate: 'views/modals/mobile-layer-manager.html',
						show: false,
						scope: $scope
					})
				}
			];
		});

		$scope.gotoPage = function(url) {
			$scope.hideMenu();
			open(url);
		}

		$scope.resetCamera = function () {
			$scope.trackingObj = null;
			$scope.hideMenu();
			util.resetCamera();
		}
		$scope.tabClick = function (tab) {

			$scope.hideMenu();
			tab.action();
		}

		$scope.hideMenu = function() {
			$('.navbar-collapse.in').removeClass('in').addClass('collapse');
		}
		$scope.menuAction = function (action) {
			util.log(action);
		}
		
		$scope.$on('searchModal.show.before', function () {
			$rootScope.searchModal = true;
		});
		$scope.$on('searchModal.hide', function () {
			$rootScope.searchModal = false;
		});
	}
]);
wwt.controllers.controller('LayerManagerController',
	['$scope',
	'AppState',
	'$timeout',
	'Util',
	function($scope, appState, $timeout,util) {
		var version = 5;
		function treeNode(args) {
			this.name = args.name;
			this.checked = args.checked === undefined ? true : args.checked;
			this.children = args.children || [];
			this.action = args.action;
			this.collapsed = args.collapsed || false;
		    this.disabled = false;
			if (args.v) this.v = args.v;
		}
         
		var constellations = [];
		$scope.initLayerManager = function() {
			if (!wwtlib.Constellations.abbreviations) {
				setTimeout($scope.initLayerManager, 333);
				return;
			}
			$.each(wwtlib.Constellations.abbreviations, function(name, abbrev) {
				constellations.push(new treeNode({
					name: name
				}));
			});
			$scope.tree = appState.get('layerManager');
			if (!$scope.tree || !$scope.tree.v || $scope.tree.v !== version) { //dump appState version when change is made
				$scope.tree = initTree();

				appState.set('layerManager', $scope.tree);
			}
			$timeout(function() { initTreeNode(0, $scope.tree); });
			wwt.resize();
		}

		var initTree = function() {
			return new treeNode({
				v: version,
				name: $scope.getFromEn('Sky'),
				action: 'showSkyNode',
				children: [
					new treeNode({
						name: $scope.getFromEn('Overlays'),
						action: 'showSkyOverlays',
						children: [
							new treeNode({
								name: $scope.getFromEn('Constellations'),
								action: 'constellationsEnabled',
								children: [
									new treeNode({
										name: $scope.getFromEn('Constellation Pictures'),
										//children: picturesFilter,
										action: 'showConstellationPictures',
										checked: false
									}), new treeNode({
										name: $scope.getFromEn('Constellation Figures'),
										//children: figuresFilter,
										action: 'showConstellationFigures'
									}), new treeNode({
										name: $scope.getFromEn('Constellation Boundaries'),
										collapsed: true,
										children: [
											new treeNode({ name: $scope.getFromEn('Focused Only'), action: 'showConstellationSelection' })
										],
										action: 'showConstellationBoundries'
									}), new treeNode({
										name: $scope.getFromEn('Constellation Names'),
										checked: false,
										action: 'showConstellationLabels'
									})
								]
							}),
							new treeNode({
								name: $scope.getFromEn('Grids'),
								action: 'showSkyGrids',
								children: [
									new treeNode({
										name: $scope.getFromEn('Equatorial Grid'),
										checked: false,
										collapsed: true,
										action: 'showGrid',
										children: [
											new treeNode({
												name: $scope.getFromEn('Axis Labels'),
												checked: true,
												action: 'showEquatorialGridText'
											})
										]
									}), new treeNode({
										name: $scope.getFromEn('Galactic Grid'),
										checked: false,
										collapsed: true,
										action: 'showGalacticGrid',
										children: [
											new treeNode({
												name: $scope.getFromEn('Axis Labels'),
												checked: true,
												action: 'showGalacticGridText'
											})
										]
									}), new treeNode({
										name: $scope.getFromEn('AltAz Grid'),
										checked: false,
										collapsed: true,
										action: 'showAltAzGrid',
										children: [
											new treeNode({
												name: $scope.getFromEn('Axis Labels'),
												checked: true,
												action: 'showAltAzGridText'
											})
										]
									}), new treeNode({
										name: $scope.getFromEn('Ecliptic Grid'),
										checked: false,
										collapsed: true,
										children: [
											new treeNode({
												name: $scope.getFromEn('Axis Labels'),
												checked: true,
												action: 'showEclipticGridText'
											})
										],
										action: 'showEclipticGrid'
									}), new treeNode({
										name: $scope.getFromEn('Ecliptic Overview'),
										checked: true,
										collapsed: true,
										action: 'showEcliptic',
										children: [
											new treeNode({
												name: $scope.getFromEn('Month Labels'),
												checked: false,
												action: 'showEclipticOverviewText'
											})
										]
									}), new treeNode({
										name: $scope.getFromEn('Precession Chart'),
										checked: false,
										action: 'showPrecessionChart'
									})
								]
							})
						]
					}),
					new treeNode({
						name: $scope.getFromEn('2d Sky'),
						checked: true,
						action: 'showSkyNode',
						children: [
							new treeNode({
								name: $scope.getFromEn('Show Solar System'),
								checked: true,
								action: 'showSolarSystem'
							}) /*,
						new treeNode({
							name: $scope.getFromEn('Field of View Indicators'),
							checked: true,
							action: 'showFieldOfView'
						})*/
						]
					}),
					new treeNode({
						name: $scope.getFromEn('3d Solar System'),
						checked: true,
						action: 'showSolarSystem',
						children: [
                            new treeNode({
								name: $scope.getFromEn('Milky Way (Dr. R. Hurt)'),
								checked: true,
								action: 'solarSystemMilkyWay'
							}), new treeNode({
								name: $scope.getFromEn('Planets (NASA, ETAL)'),
								checked: true,
								action: 'solarSystemPlanets'
							}), new treeNode({
								name: $scope.getFromEn('Planetary Orbits'),
								checked: true,
								action: 'solarSystemOrbits'
							}), new treeNode({
								name: $scope.getFromEn('Lighting and Shadows'),
								checked: true,
								action: 'solarSystemLighting'
							})
						]
					})
				]
			});
		}


		$scope.nodeChange = function(node) {
			appState.set('layerManager', $scope.tree);
			invokeSetting(node);
		};

		//var invokeSetting = function(node) {
		//	if (node.action) {
		//		try {
		//			wwt.wc.settings['set_' + node.action](node.checked);
		//		} catch (er) {
		//			util.log(er, node.action);
		//		}
		//	}
		//}

		var invokeSetting = function (node) {
		    if (!node.disabled && node.action &&
              wwt.wc.settings['set_' + node.action]) {
		        var settingFlag = node.checked && !node.disabled;
		        wwt.wc.settings['set_' + node.action](settingFlag);
		    }
		    setChildState(node);
		};


	    // enable/disable all child settings based on parent
		var setChildState = function (node) {
		    if (node.children) {
		        $.each(node.children, function (i, child) {
		            child.disabled = !node.checked || node.disabled;
		            if (child.action && wwt.wc.settings['set_' + child.action]) {
		                var settingFlag = child.checked && !child.disabled;
		                wwt.wc.settings['set_' + child.action](settingFlag);
		            }
		            setChildState(child);
		        });
		    }
		}


		function initTreeNode(i, node) {
			$.each(node.children, initTreeNode);
			invokeSetting(node);
		}
	}]
);
wwt.controllers.controller('ADSController',
    ['$scope',
    'Util',
    '$timeout',
    function ($scope, util, $timeout) {
       
        $scope.adsFilter = 'All';
        
        var years = ["date-pre1800_512", "date-1800_1850_512", "date-1850_1900_512", "date-1900_1910_512",
    "date-1910_1920_512", "date-1920_1930_512", "date-1930_1940_512", "date-1940_1945_512", "date-1945_1950_512",
    "date-1950_1955_512", "date-1955_1960_512", "date-1960_1965_512", "date-1965_1970_512", "date-1970_1975_512",
    "date-1975_1980_512", "date-1980_1985_512", "date-1985_1990_512", "date-1990_512", "date-1991_512",
    "date-1992_512", "date-1993_512", "date-1994_512", "date-1995_512", "date-1996_512", "date-1997_512",
    "date-1998_512", "date-1999_512", "date-2000_512", "date-2001_512", "date-2002_512", "date-2003_512",
    "date-2004_512", "date-2005_512", "date-2006_512", "date-2007_512", "date-2008_512", "date-2009_512",
    "date-2010_512", "date-2011_512", "date-2012_512", "date-2013_512"
        ];
        
        var collections = [{
                label: 'GLIMPSE',
                name: 'GLIMPSE/MIPSGAL'
            }, {
                label:'All',
                name:'allSources_512'
            }, {
                label: 'WISE',
                name: 'WISE All Sky (Infrared)'
            }, {
                label: 'X-ray',
                name: 'X_512'
            }, {
                label: 'Ultraviolet',
                name: 'UV_512'
            }, {
                label: 'Star',
                name: 'Star_512'
            }, {
                label: 'Radio',
                name: 'Radio_512'
            }, {
                label: 'Other',
                name: 'Other_512'
            }, {
                label: 'Nebula',
                name: 'Nebula_512'
            }, {
                label: '',
                name: 'lut_y'
            }, {
                label: '',
                name: 'lut_x'
            }, {
                label: 'Infrared',
                name: 'Infrared_512'
            }, {
                label: 'HII regions',
                name: 'HII-region_512'
            }, {
                label: 'Galaxy',
                name: 'Galaxy_512'
            }
        ];
        $scope.initAds = function () {
            wwt.wc.add_collectionLoaded(defaultLayers);
            wwt.wc.loadImageCollection('adsass.wtml');
            var bar = $('.year-slider a.btn');
            var ys = new wwt.Move({
                el: bar,
                bounds: {
                    x: [-55, 45],
                    y: [0, 0]
                },
                onstart: function () {
                    bar.addClass('moving');
                    setYear();
                },
                onmove: function () {
                    setYear();
                },
                oncomplete: function () {
                    bar.removeClass('moving');
                }
            });
        }
       
        var setYear = function () {
            $timeout(function () {
                $scope.fgImagery = 'year';
                var left = $('.year-slider a.btn').position().left / 100;
                var y = years[Math.max(0, Math.round(left * years.length) - 1)];

                $scope.year = y.split('ate-')[1].split('_512')[0].replace(/_/, '-');
                wwt.wc.setForegroundImageByName(y);
                wwt.wc.setForegroundOpacity($('.cross-fader a.btn').position().left);
            });
        }

        $scope.bgChange = function() {
            $scope.setSurveyBg($scope.bgImagery);
        };
        $scope.adsChange = function () {
            if ($scope.fgImagery === 'year') {
                setYear();
                return;

            }
            $.each(collections, function (i, c) {

                if (c.label === $scope.fgImagery) {
                    wwt.wc.setForegroundImageByName(c.name);
                    wwt.wc.setForegroundOpacity($('.cross-fader a.btn').position().left);
                }
            });
        }

        function defaultLayers() {
            var ra = parseFloat(util.getQSParam('ra') || 0);
            var dec = parseFloat(util.getQSParam('dec') || 0);
            var fov = parseFloat(util.getQSParam('fov') || 60);
            var layer = (util.getQSParam('layer') || 'allSources');
            if (layer.slice(layer.length - 4) !== '_512') {
                layer = layer + '_512';
            }

            wwt.wc.setForegroundImageByName(layer);

            $('#facet-list a').each(function (i, o) {
                if ($(o).attr('href') === layer) {
                    o.click();
                    $('#foreground-lbl').text($(o).text());
                    return;
                }
            });


            wwt.wc.setForegroundOpacity(50);

            wwt.wc.gotoRaDecZoom(ra, dec, fov, true);
            $timeout(function() {
                $scope.fgImagery = 'All';
                $scope.bgImagery = 'WISE All Sky (Infrared)';
                $scope.bgChange();
                
                $scope.setSurveyBg('WISE All Sky (Infrared)');
                
                
            }, 1300);
        }
        
    }
]);
wwt.controllers.controller('ExploreController',
	['$scope',
	'$rootScope',
	'AppState',
	'Places',
	'$timeout',
	'Util',
	
	'ThumbList',
	function ($scope, $rootScope, appState, places, $timeout, util,  thumbList) {
	    var exploreRoot;
	    var depth = 1;
	    var bc;
	    var cache = [];
	    
	    var openCollection,
			collectionPlace,
			collectionPlaceIndex,
			hashObj;

	    $rootScope.$on('hashChange', function (event, obj) {
	        if (obj['place'] && isNaN(parseInt(obj['place'].charAt(0)))) {
	            hashObj = obj;
	            collectionPlace = obj['place'];
	            collectionPlaceIndex = 1;
	            $scope.loadingUrlPlace = true;
	            $('#loadingModal').modal('show');
	            places.getRoot().then(function () {
	                $scope.breadCrumb = bc = [$scope.getFromEn('Collections')];
	                $('body').append($('#researchMenu'));
	                $scope.collection = exploreRoot;
	                findCollectionChild();
	            });
	        } 
	    });

	    $scope.initExploreView = function (hashChange) {
	        thumbList.init($scope, 'explore');
	        if (!hashChange) {
	            places.getRoot().then(function (result) {
	                $('body').append($('#researchMenu'));
	                $scope.collection = exploreRoot = result;

	                var collectionsString = $scope.getFromEn('Collections');
	                if (collectionsString.then) {
	                    collectionsString.then(function (s) {
	                        $scope.breadCrumb = bc = [s];
	                    });
	                } else {
	                    $scope.breadCrumb = bc = [collectionsString];
	                }

	                cache = [result];
	                calcPageSize();
	                $.each(result, function (i, item) {
	                    if (item.get_name() === 'Open Collections'||item.get_name() === 'New VAMP Feeds') {
	                        result.splice(0, 0, result.splice(i, 1)[0]);
	                        if (item.get_name() === 'Open Collections') {
	                            openCollection = true;
	                            $scope.clickThumb(item);
	                        } 
	                    }
	                });

	            });
	        }

	    };

	    var findCollectionChild = function () {
	        var guidParts = collectionPlace.split('.');
	        var relevantPart = guidParts.slice(0, collectionPlaceIndex).join('.');
	        var child = places.findChildById(relevantPart, $scope.collection);
	        if (collectionPlaceIndex < guidParts.length) {
	            collectionPlaceIndex++;
	            $scope.clickThumb(child, findCollectionChild);
	        } else {
	            $timeout(function () {
	                $scope.loadingUrlPlace = false;
	                $scope.clickThumb(child);
	                $('#loadingModal').modal('hide');
                      
	                if (hashObj['ra']) {
	                    var timer = hashObj['place'].toLowerCase().indexOf('hirise') !== -1 ? 6666 : 3333;
	                    setTimeout(function () {
	                        $rootScope.ctl.gotoRaDecZoom(
								parseFloat(hashObj['ra']) * 15,
								parseFloat(hashObj['dec']),
								parseFloat(hashObj['fov']),
								false
							);
	                    }, timer);
	                }
	                location.hash = '/';
	            }, 2222);
	        }
	    }


	    var handledInitBroadcast = false,
			newCollectionUrl;
        //called by openitemcontroller
	    $scope.$on('initExplorer', function (event, collectionUrl) {
	        if (!handledInitBroadcast) {
	            newCollectionUrl = collectionUrl;
	            $scope.initExploreView();
	            handledInitBroadcast = true;
	            setTimeout(function () { handledInitBroadcast = false; }, 2000);
	        }
	    });

	    var calcPageSize = function () {
	        thumbList.calcPageSize($scope, false);
	    };
	    $scope.clickThumb = function (item, folderCallback) {
	        var outParams = {
	            breadCrumb: bc,
                depth:depth,
	            cache: cache,
	            openCollection: openCollection,
	            newCollectionUrl: newCollectionUrl
	        };
	        var newParams = thumbList.clickThumb(item, $scope, outParams, folderCallback);
	        bc = newParams.breadCrumb;
	        cache = newParams.cache;
	        openCollection = newParams.openCollection;
	        newCollectionUrl = newParams.newCollectionUrl;
	        depth = newParams.depth;
	    };

	    $scope.expanded = false;
	    

	    $scope.breadCrumbClick = function (index) {
	        $scope.collection = cache[index];
	        while (bc.length - 1 > index) {
	            bc.pop();
	            cache.pop();
	        }
	        $scope.currentPage = 0;
	        calcPageSize();
	    };



	    $(window).on('resize', function () {
	        $scope.currentPage = 0;
	        calcPageSize();
	    });

	    
	    $scope.preventClickBubble = function (event) {
	        event.stopImmediatePropagation();
	    };

	    $scope.initExploreView();
	}]);


wwt.controllers.controller('SearchController',
	['$scope',
	'$rootScope',
	'$timeout',
	'Util',
	'SearchUtil',
	'ThumbList', 
	function ($scope, $rootScope, $timeout, util, searchUtil, thumbList) {

	    $scope.goto = { RA: '', Dec: '' };
	    

	    var init = function () {
	        thumbList.init($scope, 'search');
	        if (util.isMobile) {
	            $scope.scrollDepth = 40;
	        }
	        $timeout(function () {
	            $scope.SearchType = 'J2000';
	            $('#txtSearch').focus();
	        }, 123);
	    }

	    $scope.clickThumb = function (item) {
	        thumbList.clickThumb(item, $scope);
	    }; 
         
        var calcPageSize = function () {
	        thumbList.calcPageSize($scope, false);
	    };
	    
	    $scope.expanded = false;
	    $scope.expandThumbnails = function (flag) {
	        $scope.currentPage = 0;
	        $scope.expanded = flag != undefined ? flag : !$scope.expanded;
	        $scope.expandTop($scope.expanded);
	        calcPageSize();
	    };

	    $(window).on('resize', function () {
	        $scope.currentPage = 0;
	        calcPageSize();
	    });

	    
	    $scope.gotoCoord = function () {
	        var tempPlace = wwtlib.Place.create('tmp', util.parseHms($scope.goto.Dec), util.parseHms($scope.goto.RA), null, null, wwtlib.ImageSetType[$scope.lookAt.toLowerCase()], 60);
	        $rootScope.singleton.gotoTarget(tempPlace, false, false, true);
	    };

	    $scope.searchKeyPress = function() {
	        $timeout(function() {
	            var q = $scope.q = $('#txtSearch').val();
	            searchUtil.runSearch(q).then(function(result) {
	                $scope.collection = result;
	                calcPageSize();
	            });
	        }, 10);
	    };

	    init();
	}
]);


wwt.controllers.controller('SettingsController',
	['$scope',
	'$rootScope',	
	'AppState',
	'$timeout',
	'$cookies',
	'Util',
	'$q',
	function ($scope, $rootScope, appState, $timeout, $cookies, util, $q) {
		//var settings = $scope.settings = wwt.wc.settings;
		$scope.defaults = {
			autoHideTabs: false,
			autoHideContext: false,
			smoothPanning: !util.isAccelDevice(),
			version: '5.1.02',
			crosshairs: true
		};

		$q.all([$scope.getFromEn('HTML5'), $scope.getFromEn('Silverlight')]).then(function(arrayLabels) {
			$scope.availableClients = [
				{
					code: 'HTML5',
					label: arrayLabels[0]
				}, {
					code: 'SL',
					label: arrayLabels[1]
				} /*,{
				code: 'WWT',
				label: $scope.getFromEn('WorldWide Telescope Windows Client')
			}*/
			];
		});

		
		

		var redirTimer;
		$scope.setClientPref = function() {
			util.log($scope.preferredClient);
			$cookies.preferredClient = $scope.preferredClient;
			if ($scope.preferredClient == 'SL') {
				$scope.redirecting = true;
				$scope.redirectingSeconds = 5;
				setInterval(function() {
					$timeout(function() {
						$scope.redirectingSeconds--;
					});
				}, 1000);
				redirTimer = setTimeout(function() {
					location.reload();
				},5000);
			}
		};
		$scope.cancelRedir = function() {
			clearTimeout(redirTimer);
			$scope.redirecting = false;
		};

		$timeout(function () {
			$scope.preferredClient = $cookies.preferredClient || $scope.availableClients[0].code;
			//util.log($scope.preferredClient, $scope.availableClients);
		}, 10);

		$scope.$on('restoreDefaults', function () {
			appState.set('settings', null);
			$scope.retrieveSettings();
		});

		$scope.retrieveSettings = function () {
		   
			$scope.savedSettings = appState.get('settings');
			if (!$scope.savedSettings || !$scope.savedSettings.version || $scope.savedSettings.version != $scope.defaults.version) {
				$scope.savedSettings = $scope.defaults;
			}
			if ($scope.savedSettings['crosshairs'] === undefined) {
				$scope.savedSettings = $scope.defaults;
			}
			$scope.saveSettings(true);
		};

		$scope.WebGl = appState.get('WebGl') ? true : false;
		$scope.setWebGl = function() {
			appState.set('WebGl', $scope.WebGl);
			location.reload();
		}

		$scope.saveSettings = function (init) {
		    $timeout(function () {
		        var broadcast = false;
				$.each($scope.savedSettings, function (setting, flag) {
					if ($scope[setting] !== flag || init) {
						if (init) {
							$scope[setting] = flag;
						} else {
						    $scope.savedSettings[setting] = $scope[setting];
						}
						if (setting.indexOf('autoHide') === 0) {
						    broadcast = true;
						}
						
					}
				});
				wwt.wc.settings.set_showCrosshairs($scope.crosshairs);
				wwt.wc.settings.set_smoothPan($scope.smoothPanning);
				appState.set('settings', $scope.savedSettings);
				if (broadcast) {
				    $rootScope.$broadcast('autohideChange');
				}
			}, 10);
			
				
		};
		

		$scope.retrieveSettings();
	}
]);
wwt.controllers.controller('ViewController',
	['$scope',
	'AppState',
	'$timeout','Util','$rootScope',
	function ($scope, appState,$timeout, util,$rootScope) {
		var stc = $scope.spaceTimeController = wwtlib.SpaceTimeController;
		$scope.galaxyModeChange = function () {
			if ($scope.galaxyMode && $scope.viewFromLocation) {
				$scope.viewFromLocation = false;
				$scope.setViewFromLocation();
			}
			wwt.wc.settings.set_galacticMode($scope.galaxyMode);
		};
		wwtlib.WWTControl.useUserLocation();
		$scope.locationName = $scope.getFromEn('My Location');
		$scope.now = new Date();
		$scope.loc = {
			view:'View',
			realTime: 'Real Time',
			reverseTime: 'Reverse Time',
			paused:'Paused'
		};
		$rootScope.languagePromise.then(function() {
			$scope.loc.view = $scope.getFromEn('View');
			$scope.loc.realTime = $scope.getFromEn('Real Time');
			$scope.loc.reverseTime = $scope.getFromEn('Reverse Time');
			$scope.loc.paused = $scope.getFromEn('Paused');
		});

		function timeDateTimerTick() {
			if ($scope.activePanel === $scope.loc.view || util.isMobile) {
				$timeout(function() {
					//var offset = stc.$1 === undefined ? stc._offset : stc.$1;
					//var now = $scope.now = new Date(new Date().valueOf() + offset);
					var now = $scope.now = stc.get_now();
					$scope.year = now.getFullYear();
					$scope.month = (now.getMonth() + 1) % 12;
					$scope.date = now.getDate();
					$scope.hours = now.getHours();
					$scope.minutes = now.getMinutes();
					$scope.seconds = now.getSeconds();
				});
			}
		};
		

		$scope.fastBack_Click = function() {
			var tr = stc.get_timeRate();
			if (tr < -2 && tr >= -1000000000) {
				stc.set_timeRate(tr * 10);
			} else {
				stc.set_timeRate(-10);
			}
			stc.set_syncToClock(true);
			updateSpeed();
		};

		$scope.back_Click = function() {
			var tr = stc.get_timeRate();
			if (tr <= -10) {
				stc.set_timeRate(tr / 10);
				stc.set_syncToClock(true);
			} else {
				stc.set_timeRate(-2);
				stc.set_syncToClock(true);
			}
			if (stc.get_timeRate() == -1) {
				stc.set_timeRate(-2);
			}
			updateSpeed();
		};

		$scope.pause_Click = function() {
			stc.set_syncToClock(!stc.set_syncToClock);
			updateSpeed();
		};

		$scope.play_Click = function () {
			var tr = stc.get_timeRate();
			if (tr >= 10) {
				tr /= 10;
			} else {
				tr = 1;
			}
			stc.set_timeRate(tr);
			stc.set_syncToClock(true);
			updateSpeed();

		};

		$scope.fastForward_Click = function() {
			var tr = stc.get_timeRate();
			if (tr > 0 && tr <= 1000000000) {
				stc.set_timeRate(tr * 10);
			} else {
				stc.set_timeRate(10);
			}
			stc.set_syncToClock(true);
			updateSpeed();
		};

		function updateSpeed() {
			var tr = stc.get_timeRate();
			if (tr == -2)tr = -1;
			if (tr == 1){
				$scope.TimeMode = $scope.loc.realTime;
			} else if (stc.TimeRate == -2.0){
				$scope.TimeMode = $scope.loc.reverseime;
			} else {
				$scope.TimeMode = "X " + tr;
			} if (!stc.get_syncToClock()) {
				$scope.TimeMode = $scope.TimeMode + " : " + $scope.loc.paused;
			}   
		}

		$scope.timeNow_Click = function() {
			stc.set_syncToClock(true);
			stc.syncTime();
			stc.set_timeRate(1);
			updateSpeed();
		};

		$scope.setViewFromLocation = function() {
			if ($scope.galaxyMode && $scope.viewFromLocation) {
				$scope.galaxyMode = false;
				$scope.galaxyModeChange();
			}
			$rootScope.ctl.settings.set_localHorizonMode($scope.viewFromLocation);
		};
		setInterval(timeDateTimerTick, 300);
		updateSpeed();


	}
]);
wwt.controllers.controller('ToursController',
	['$scope',
		'$rootScope',
	'AppState',
	'Tours',
	'$timeout','Util','$popover',
	function ($scope, $rootScope, appState, tours, $timeout, util, $popover) {
		var toursRoot;
		var depth = 1;
		var bc = [$scope.getFromEn('Tours')];
		var cache = [];
		$scope.pageCount = 1;
		$scope.pageSize = 1;
		$scope.currentPage = 0;

		tours.getRoot().then(function (result) {
			$scope.tourList = toursRoot = result;
			$scope.breadCrumb = bc;
			cache.push(result);
			calcPageSize();
		});

		$scope.clickThumb = function (item) {
            
			$scope.activeItem = item.get_thumbnailUrl() + item.get_name();
			if (item.get_name() === 'Up Level') {
				$scope.currentPage = 0;
				depth--;
				bc.pop();
				$scope.breadCrumb = bc;//.join(' > ') + ' >';
				cache.pop();
				$scope.tourList = cache[cache.length - 1];
				calcPageSize();
				return;
			}
			if (item.get_isFolder()) {
				$scope.currentPage = 0;
				depth++;
				bc.push(item.get_name());
				$scope.breadCrumb = bc;
				tours.getChildren(item).then(function (result) {
					$scope.tourList = result;
					cache.push(result);
					calcPageSize();
				});
				
			}
			
			
			if (ss.canCast(item, wwtlib.Tour)) {
				$scope.playTour(item.get_tourUrl());
				if (util.isMobile) {
					$rootScope.landscapeMessage = true;
					setTimeout(function() {
						$scope.$hide();
					},2222);

				}

			}

		};

		$scope.breadCrumbClick = function (index) {
			$scope.tourList = cache[index];
			while (bc.length - 1 > index) {
				bc.pop();
				cache.pop();
			}
		};


		var calcPageSize = function () {
			$timeout(function () {
				var tnWid = 118;
				var winWid = $(window).width();
				$scope.pageSize = Math.floor(winWid / tnWid);
				$scope.pageCount = Math.ceil($scope.tourList.length / $scope.pageSize);
			}, 1);
			
		};

	    var popover = null;
	    var mask = null;
	    $scope.tourPreview = function (event, item) {

	        if (item.get_isFolder() || item.get_name() === 'Up Level') return;
	        $scope.relatedTours = tours.getToursById(item.relatedTours);
			$rootScope.tour = item;
	        if (!mask) {
	            mask = $('<div></div>').css({
	                height: 70,
	                width: 120,
	                top: -999,
	                left: -999,
	                opacity: 0,
                    background:'#fff',
                    position: 'fixed',
                    zIndex:2
                    
	            }).on('mouseleave',hideMask);
	            $('body').append(mask);
	        }
	        if (popover) {
		        popover.hide();
		    }
		    var options = {
		        title: item.get_name(),
		        target: $(event.currentTarget),
		        id: 'tourpop',
		        templateUrl:'views/popovers/tour-template.html',
		        contentTemplate: 'views/popovers/tour-info.html',
		        placement: 'bottom-left',
		        scope: $scope,
		        trigger:'manual'
		    };
		    
		    popover = $popover($(event.currentTarget),options);
		    popover.$promise.then(function () {
		        popover.show();
		        var thumb = $(event.currentTarget);
		        var pos = thumb.offset();
		        mask.css({
		            top: pos.top - 2, 
		            left: pos.left - 2,
		            opacity: .01
		        });
		        var fixImages = function () {//wth?
		            $('.tour-info .author-image img').each(function () {
		                if (this.naturalHeight > 0 && $(this).height() === 0) {
		                    //console.log('fixing',this.naturalHeight, $(this).height());
		                    $(this).css({
		                        height: this.naturalHeight,
		                        width: this.naturalWidth
		                    });
		                    //console.log('fixed?', this.naturalHeight, $(this).height());
		                }
		                
		                
		            });
		        }
		        $('.tour-info img').off('load');
		        $('.tour-info img').on('load', fixImages);
		        setTimeout(fixImages, 500);
		        setTimeout(fixImages, 1500);
		    });
	    };
        var hideMask = function() {
            mask.css({
                top: -999,
                left: -999
            });
        }

	    //var fixPop = function() {
        //    $('.popover').removeClass('modal').css('top', '8px').removeAttr('tabindex')
            
        //    /*$('.popover').find('.modal-dialog,.modal-content,.modal-body')
        //        .css({ margin: 0, padding: 0,width:470 })
        //        .removeClass('modal-dialog modal-content modal-body');*/
        //    var thumb = $('.popover').parent().find('a').first();
        //    var pos = thumb.offset();
        //    $('.modal-backdrop').css({
        //        height: thumb.height() + 8,
        //        width: thumb.width() + 12,
        //        top: pos.top - 2,
        //        left: pos.left - 2,
        //        right: '',
        //        bottom: '',
        //        opacity: 1
        //    });
        //    $('.popover').find('.label').click();
        //    $('.popover').find('.fa-play-circle').trigger('mouseenter').focus();
        //    $('.popover').css('height', $('.modal-dialog').height());
        //    util.log(pos, $('.popover').find('.label').text());
        //}

	    $(window).on('resize', calcPageSize);

		
	}
]);
wwt.controllers.controller('CommunityController',
    ['$scope',
    'Util',
    '$timeout', 'ThumbList','Community',
    function ($scope, util, $timeout, thumbList, community) {
        var depth = 1;
        var bc;
        var cache = [];
        $scope.initCommunityView = function () {
            thumbList.init($scope, 'communities');
            
                community.getRoot().then(function (result) {
                    $('body').append($('#researchMenu'));
                    $scope.collection = result;

                    var collectionsString = $scope.getFromEn('Collections');
                    if (collectionsString.then) {
                        collectionsString.then(function (s) {
                            $scope.breadCrumb = bc = [s];
                        });
                    } else {
                        $scope.breadCrumb = bc = [collectionsString];
                    } 

                    cache = [result];
                    calcPageSize();

                });
            

        };
        var calcPageSize = function () {
            thumbList.calcPageSize($scope, false);
        };
        $scope.clickThumb = function (item, folderCallback) {
            var outParams = {
                breadCrumb: bc,
                depth: depth,
                cache: cache
            };
            var newParams = thumbList.clickThumb(item, $scope, outParams, folderCallback);
            bc = newParams.breadCrumb;
            cache = newParams.cache;
            depth = newParams.depth;
        };

        $scope.expanded = false;


        $scope.breadCrumbClick = function (index) {
            $scope.collection = cache[index];
            while (bc.length - 1 > index) {
                bc.pop();
                cache.pop();
            }
            $scope.currentPage = 0;
            calcPageSize();
        };



        $(window).on('resize', function () {
            $scope.currentPage = 0;
            calcPageSize();
        });


        $scope.preventClickBubble = function (event) {
            event.stopImmediatePropagation();
        };

        $scope.initCommunityView();
        
    }
]);
wwt.controllers.controller('CurrentTourController', [
    '$scope', '$rootScope', 'Util', 'MediaFile','AppState','$timeout','$modal',
    function ($scope, $rootScope, util, media,appState,$timeout,$modal) {
    var tourEdit = $scope.tourEdit = wwtlib.WWTControl.singleton.tourEdit;
    var tour;
    var isNewTour = false;
    var mainScope = angular.element('div.desktop').scope();
    
    $scope.slideNumbering = appState.get('slideNumbering');
    $scope.overlayList = appState.get('overlayList');
    var scrollerInit = false;

    $scope.init = function (curTour) {
        tourEdit = $scope.tourEdit = wwtlib.WWTControl.singleton.tourEdit;
        $rootScope.currentTour = $scope.tour = tour = tourEdit.get_tour();
        tourEdit.tourEditorUI.editTextCallback = function (textObject, onFinished) {
            $scope.editText = { textObject: textObject, onFinished: onFinished };
            $('#editTourText').click();
        };
        mapStops(true);
        wwt.wc.add_slideChanged(function () {
            //console.log(arguments, tour, tourEdit);
            $scope.$applyAsync(function () {//tween <.5 

                $scope.activeIndex = tour.get_currentTourstopIndex();
                $scope.activeSlide = tour.get_currentTourStop();
                tourEdit.tourStopList.selectedItems = {};
                tourEdit.tourStopList.selectedItems[$scope.activeIndex] = $scope.activeSlide;
                
            }); 
        });
        


        tourEdit.tourStopList.refreshCallback = mapStops;
        $scope.editText = null;
        mainScope.ribbon.tabs[1].menu['Show Slide Overlays'] = [$scope.showOverlayList];
        mainScope.ribbon.tabs[1].menu['Show Slide Numbers'] = [$scope.showSlideNumbers]; 
        $rootScope.$on('escKey', showSlides);
        $rootScope.$on('closeTour', closeTour);
        $rootScope.$watch('editingTour', initEditMode);
        $rootScope.$on('tourFinished', finishedPlaying);
        $rootScope.$on('showingSlides', function () {
            if (!scrollerInit) {
                scrollerInit = true;
                showTourSlides();
            }
        });
        if (mainScope.autoEdit) {
            showSlides();
            tour._editMode = true;
            tourEdit.pauseTour();
            $rootScope.editingTour = true;
        }
        self.addEventListener("beforeunload", function (e) {
            if (tourEdit.get_tour().get_tourDirty()) {
                e.returnValue= "You have unsaved changes that will be lost if you proceed. Click cancel to save changes."
            }
        });
    };


    var showSlides = function () {
        if (tourEdit.playing) {
            tourEdit.pauseTour();
        }
        
        $scope.$applyAsync(showTourSlides);
        
    };
   
    var initEditMode = function () {
        
        if (!util.isWindows && !util.isChrome) {
            alert('Editing Tours requires advanced browser features. For the best experience, please use Chrome. There are known incompatibilities in other browsers.');
        }
        if ($rootScope.editingTour !== true) { return; }
        tour._editMode = true;
        tourEdit.pauseTour();
        $('#contextmenu,#popoutmenu').on('click', function () {
            mapStops.apply($scope, []);
        });
        setTimeout(initVolumeSliders, 111);
        $('canvas').on('dblclick click', function () {
            mapStops.apply($scope, []);
        });
    };
    var closeTour = function () {
        console.trace('closetour');
        $scope.tourEdit = tourEdit = null;
        $rootScope.currentTour = $scope.tour = tour = null;
    };
    var finishedPlaying = function () {
        if (tour._currentTourstopIndex == tour._tourStops.length -1) {
            showSlides(true);
            $scope.selectStop(0, null);
            if (tourEdit.playing) {
                $scope.playButtonClick();
            } else {
                $rootScope.tourPlaying = false;
                $rootScope.tourPaused = true;
            }
        }
    };
    $scope.showSlideNumbers = function () {
        $scope.$applyAsync(function () {
            $scope.slideNumbering = !$scope.slideNumbering;
            appState.set('slideNumbering', $scope.slideNumbering);
        });
    }
    $scope.showOverlayList = function () {
        $scope.$applyAsync(function () {
            $scope.overlayList = !$scope.overlayList;
            appState.set('overlayList', $scope.overlayList);
        });
    }

    var initVolumeSliders = function () {
        var volumeOpts = function (barEl) {
            
            return {
                el: barEl,
                bounds: {
                    x: [-50, 50],
                    y: [0, 0]
                },
                onstart: function () {
                    barEl.addClass('moving');
                },
                onmove: function () {
                    var audio = barEl.attr('id') === 'voiceVol' ? $scope.activeSlide.voice : $scope.activeSlide.music;
                    if (audio) {
                        audio.set_volume(this.css.left);
                    }
                },
                oncomplete: function () {
                    barEl.removeClass('moving');
                    var audio = barEl.attr('id') === 'voiceVol' ? $scope.activeSlide.voice : $scope.activeSlide.music;
                    $scope.$applyAsync(function(){
                        audio.vol = this.css.left;
                    });
                }
            }
        };
        var musicVol = new wwt.Move(volumeOpts($('#musicVol')));
        var voiceVol = new wwt.Move(volumeOpts($('#voiceVol')));

    };

    $scope.tourProp = function ($event, prop) {
        tour['set_' + prop]($event.target.value);
    };
    
    

    $scope.saveTour = function () {
        var saveRawFile = function () {
            if (navigator && navigator.msSaveBlob) {
                navigator.msSaveBlob(blob, filename);
            }
            else {
                $('#downloadTour').remove();
                var saveUrl = URL.createObjectURL(blob);
                var a = document.createElement('a');
                a.download = filename;
                a.href = saveUrl;
                a.innerHTML = '&nbsp;';
                a.id = 'downloadTour';
                a.style.position = 'absolute';
                a.style.top = '-999px';
                a.dataset.downloadurl = ['application/wtt', a.download, a.href].join(':');
                document.body.appendChild(a);
                $('#downloadTour')[0].click();
            }
        };
        var blob = tour.saveToBlob();
        var filename = tour._title + '.wtt';
        if ($rootScope.loggedIn) {
            $scope.modalData = {
                step: 'save',
                tourTitle:tour._title
            };
            $scope.modalData.saveChoice = function (upload) {
                $scope.$applyAsync(function () {
                if (upload) {
                    $scope.modalData.step='progress'
                    var fd = new FormData();
                    fd.append('fname', filename);
                    fd.append('data', blob);
                    $.ajax('/Resource/Service/Content/PublishTour/' + tour._title, {
                        type: 'POST',
                        //url: '/Resource/Service/Content/Publish/' + encodeURIComponent(filename),
                        data: blob,
                        processData: false,
                        beforeSend: function (request) {
                            request.setRequestHeader("LiveUserToken", $rootScope.token);
                        },
                        contentType: 'text/plain'
                    }).done(function (data) {
                        
                        $scope.$applyAsync(function () {
                            if (data && data.length > 1) {
                                $scope.modalData.step = 'success';
                                var url = 'http://' + location.host + 'file/Download/' + data + '/' + tour._title + '/wtt'
                                $scope.modalData.download = {
                                    url: url,
                                    showStatus: false,
                                    label: 'Share Download Link'
                                };
                                $scope.modalData.share = {
                                    url: 'http://' + location.host + '/webclient?tourUrl=' + encodeURIComponent(url),
                                    showStatus: false,
                                    label: 'Share Playable Link'
                                }


                            } else {
                                $scope.modalData.step = 'error';
                                $scope.modalData.error = +data;

                            }
                        });
                        
                        
                    });
                }
                else {
                    saveRawFile();
                    
                }
                });
            }
            var saveTourAsModal = $modal({
                scope: $scope,
                templateUrl: 'views/modals/tour-uploader.html',
                show: true,
                content: '',
                placement:'center'
            });
           
        }

        var hideModal = function (modal) {
            modal.$promise.then(modal.hide);
            $scope.showModalButtons = false;
        };
    };
    $scope.addShape = function (type) {
        tourEdit.tourEditorUI.addShape('', type);
    }
    

    
    $scope.mediaFileChange = function (e, mediaKey, isImage) {
        console.time('storeLocal: ' + mediaKey);
        var file = e.target.files[0];
        if (!file.name) {
            return;
        }

        if (isImage) {
            tourEdit.tourEditorUI.addPicture(file);
        }
        else {
            console.log('addAudio');
            tourEdit.tourEditorUI.addAudio(file, mediaKey === 'music');
            $timeout(bindAudio, 500);
        }
    };
    
    var showTourSlides = function () {
        //$('#ribbon,.top-panel,.context-panel,.layer-manager').removeClass('hide').fadeIn(400);
        console.log(tourEdit.playing);
        setTimeout(function () {
            $rootScope.stopScroller = $('.scroller').jScrollPane({ scrollByY: 155, horizontalDragMinWidth: 155 }).data('jsp');
            $(window).on('resize', function () {
                
                $rootScope.stopScroller.reinitialise();
            });
        }, 500);
    };

    $scope.showContextMenu = function (index,e) {
        if (e) {
            $scope.selectStop(index);
            tourEdit.tourStopList_MouseClick(index, e);
        }
    };


    $scope.selectStop = function (index, e) {
        $scope.$applyAsync(function () { 
             
            $scope.activeSlide = tourEdit.tourStopList.selectedItem = $scope.tourStops[index];
            
            $scope.activeIndex = index;
            if (tour._editMode) {
                if (e && e.shiftKey) {
                    tourEdit.tourStopList.selectedItems = {};
                    for (var i = Math.min(index, $scope.lastFocused) ; i <= Math.max(index, $scope.lastFocused) ; i++) {
                        tourEdit.tourStopList.selectedItems[i] = $scope.tourStops[i];
                    }
                }
                else if (e && e.ctrlKey) {
                    var keys = Object.keys(tourEdit.tourStopList.selectedItems);
                    if (tourEdit.tourStopList.selectedItems[index] && keys.length > 1) {
                        delete tourEdit.tourStopList.selectedItems[index];
                        $scope.activeIndex = keys[0];//set to first key
                    } else {
                        tourEdit.tourStopList.selectedItems[index] = $scope.tourStops[index];
                    }
                } else {
                    tourEdit.tourStopList.selectedItems = {};
                    tourEdit.tourStopList.selectedItems[index] = $scope.tourStops[index];
                }
            }
            else {
                tourEdit.tourStopList.selectedItems = {};
                tourEdit.tourStopList.selectedItems[index] = $scope.tourStops[index];
            }
            
            tour.set_currentTourstopIndex($scope.activeIndex);
            $scope.lastFocused = index;
            $scope.selectedSlide = $scope.tourStops[$scope.activeIndex];
            tourEdit.tour_CurrentTourstopChanged();
            $scope.$broadcast('initSlides');
            $timeout(bindAudio, 500);
        });
    };

    var bindAudio = function () {
        if (!$scope.activeSlide) { return;}
        var mapAudioProps = function (audio) {
            if (audio) {
                audio.muted = audio.get_mute();
                audio.name = audio._name === '' ? audio._filename$1 : audio._name;
                audio.vol = audio.get_volume();
                audio.mute = function (flag) {
                    $scope.$applyAsync(function () {
                        audio.muted = flag;
                        audio.set_mute(flag);
                    });
                }
            }

            return audio;
        }

        $scope.activeSlide.music = mapAudioProps($scope.activeSlide._musicTrack);
        $scope.activeSlide.voice = mapAudioProps($scope.activeSlide._voiceTrack);
    }

    $scope.showStartCameraPosition = function (index) {
        tour.set_currentTourstopIndex(index);
        tourEdit.tourStopList_ShowStartPosition();
    };
    $scope.showEndCameraPosition = function (index) {
        tour.set_currentTourstopIndex(index);
        tourEdit.tourStopList_ShowEndPosition();
    };
         
    $scope.playButtonClick = function () {
        if (tourEdit.playing) {
            tourEdit.pauseTour();
        }
        else {
            if (tour._editMode) {
                $scope.selectStop(0);
                tourEdit.playNow(true);
            }
            else {
                tourEdit.playFromCurrentTourstop();
            }
        } 
        $rootScope.tourPlaying = tourEdit.playing;
        $rootScope.tourPaused = !tourEdit.playing;
    };
    var refreshLoop;
    var mapStops = $scope.refreshStops = function (isInit) {
        if (refreshLoop) {
            return;
        }
        refreshLoop = true;
        setTimeout(function () { refreshLoop = false }, 55);
        $scope.$applyAsync(function () {
            tour.duration = 0;
            $scope.tourStops = tour.get_tourStops().map(function (s) {
                s.description = s.get_description();
                s.thumb = s.get_thumbnail(); 
                s.duration = s.get_duration();
                tour.duration += s.duration;

                //placeholder values until transition api is there
                s.atime = s.get__transitionOutTime();
                s.btime = s.get__transitionTime();
                s.holdtime = s.get__transitionHoldTime();
                s.transitionType = s.get__transition();
                s.isMaster = s.get_masterSlide();
                return s;
            });
            tour.minuteDuration = (tour.duration / 60 / 1000) << 0;
            tour.secDuration = ((tour.duration / 1000) % 60) << 0;
            $scope.tour = tour;
            if (!isNewTour && !$scope.tourStops.length && tour._title === 'New Tour') {
                isNewTour = true;
                setTimeout(function () {
                    $('#newTourProps').click();
                }, 500);
                return;
            }
            if ((isNewTour && $scope.tourStops.length) || (isInit && isInit === true)) {
                isNewTour = false;
                $scope.selectStop(0);
                
                $scope.$watch('activeSlide._tweenPosition', function (e) {//todo:investigate perf implications
                    console.log('tweenPos', e);
                    if (e === 1) {
                        finishedPlaying();//hack - need tourfinished event
                    }
                });
            }
            
            $scope.$broadcast('initSlides');
        });
    };

    $scope.launchFileBrowser = function (inputId) {
        $('#' + inputId).click();
    };

    $scope.setStopTransition = function (index, transitionType, transTime) {
        if (transitionType || transitionType === 0) {
            var stop = $scope.tourStops[index];
            stop.set__transition(transitionType);
            stop.transitionType = transitionType;
            return;
        } else if (transTime && typeof transTime === 'string') {
            var stop = $scope.tourStops[index];
            switch (transTime) {
                case 'atime':
                    stop.set__transitionOutTime(stop.atime);
                    break;
                case 'btime':
                    stop.set__transitionTime(stop.btime);
                    break;
                case 'holdtime':
                    stop.set__transitionHoldTime(stop.holdtime);
                    break;
            }
        }
    };
}]);


    
wwt.controllers.controller('TourSlideText', [
	'$scope',
	'$rootScope',
	'Util',
	'$timeout',
    function ($scope, $rootScope, util, $timeout) {
        var editorUI = wwtlib.WWTControl.singleton.tourEdit.tourEditorUI;
        var iframeBody;
        var editScope = null;
        var pristineText = null;
        function init() {
            editScope = angular.element('#currentTourPanel').scope();
            if (editScope.editText) {
                pristineText = editScope.editText.textObject;
                //console.log(pristineText);
                $.each(editableKeys, function (i, key) {
                    if (pristineText[key] !== textObject[key]) {
                        if (key.indexOf('Color') > 0) {
                            textObject[key] = pristineText[key].a === 0 ? 'transparent' : pristineText[key].toFormat();
                        }
                        else{
                            textObject[key] = pristineText[key];
                        }
                    }
                });

            }
        }
         
        var textObject = {
            text: '',
            foregroundColor: '#ffffff',
            backgroundColor: 'transparent',
            bold: false,
            italic: false,
            underline: false,
            fontSize: 24,
            fontName: 'Arial',
            borderStyle:0
        };
        var editableKeys = Object.keys(textObject);
 
    var saving = false;
    function initEditorObserver() {

        iframeBody = $('.modal.tour-text iframe').contents().find("body");
        if (editScope.editText) {
            iframeBody.html('');
            textObject.text.split('\n').forEach(function (s, i) {
                iframeBody.append($('<p></p>').html(s));
            });
            iframeBody.find('p').css({
                color: textObject.foregroundColor,
                backgroundColor: textObject.backgroundColor,
                fontWeight: textObject.fontWeight ? 'bold' : 'normal',
                fontSize: textObject.fontSize + 'pt',
                textDecoration: textObject.underline ? 'underline' : 'none',
                fontStyle: textObject.italic ? 'italic' : 'none',
                fontFamily: textObject.fontName,
                margin:'3px 0'
            });
        }
        var getObserver = function (cb) {

            return new MutationObserver(function (mutations) {
                if (!saving) {
                    mutations.forEach(function () { setTimeout(cb, 100) });
                } else {
                    console.log('not observing after save');
                }
            });
        };
        var fontConfig = {
            attributes: true,
            childList: true,
            characterData: true
        };
        var fontFamily = $('.mce-widget[aria-label="Font Family"] span.mce-txt');
        fontFamily.text(textObject.fontName);
        var fontFamilyObserver = function (mutation) {
            var ffText = fontFamily.text();
            if (ffText.indexOf('Font') === 0) {
                fontFamily.text(textObject.fontName);
                return;
            }
            iframeBody.find('*').css('font-family', fontFamily.text());
            textObject.fontName = fontFamily.text();
        };
        var fontSizes = $('.mce-widget[aria-label="Font Sizes"] span.mce-txt');
        fontSizes.text(textObject.fontSize);
        var fontSizesObserver = function (mutation) {
            var fsText = fontSizes.text();
            if (fsText.indexOf('Font') === 0) {
                fontSizes.text(textObject.fontSize);
                return;
            }
            iframeBody.find('*').css('font-size', fontSizes.text());
            textObject.fontSize = parseInt(fontSizes.text().replace('pt', ''));
        };
        var boldBtn = $('.mce-i-bold').parent().parent();
        var boldObserver = function (mutation) {
            textObject.bold = boldBtn.hasClass('mce-active');
            iframeBody.find('*').css('font-weight', textObject.bold ? 'bold' : 'normal')
        };
        var italicBtn = $('.mce-i-italic').parent().parent();
        var italicObserver = function (mutation) {
            textObject.italic = italicBtn.hasClass('mce-active');
            iframeBody.find('*').css('font-style', textObject.italic ? 'italic' : 'normal')
        };
        var underBtn = $('.mce-i-underline').parent().parent();
        var underObserver = function (mutation) {
            textObject.underline = underBtn.hasClass('mce-active');
            iframeBody.find('*').css('text-decoration', textObject.underline ? 'underline' : 'none');
        };

        getObserver(fontFamilyObserver).observe(fontFamily.parent().parent()[0], fontConfig);
        getObserver(fontSizesObserver).observe(fontSizes.parent().parent()[0], fontConfig);
        getObserver(boldObserver).observe(boldBtn[0], fontConfig);
        getObserver(italicObserver).observe(italicBtn[0], fontConfig);
        getObserver(underObserver).observe(underBtn[0], fontConfig);

        var bgColor = $('.mce-ico.mce-i-backcolor').parent().parent().find('.mce-preview')[0];
        var bgColorObserver = function (mutation) {
            var newBg = $(bgColor).css('background-color');
            textObject.backgroundColor = newBg;
            console.log(newBg, $('.popover.tour-text iframe').contents());
            iframeBody.css('background-color', newBg);
        };
        var fgColor = $('.mce-ico.mce-i-forecolor').parent().parent().find('.mce-preview')[0];
        var fgColorObserver = function (mutation) {
            var newFg = $(fgColor).css('background-color');
            iframeBody.find('*').css('color', newFg);
            textObject.foregroundColor = newFg;
        };

        var colorConfig = {
            attributes: true,
            childList: false,
            characterData: false
        };
        getObserver(fgColorObserver).observe(fgColor, colorConfig);
        getObserver(bgColorObserver).observe(bgColor, colorConfig);
    };
    var att = 0;
    var readyTimer = function () {
        att++;
        if ($('.modal .mce-ico.mce-i-forecolor').length) {
            console.log('editor ready');
            initEditorObserver();
        }
        else {
            console.log(att + ' init attempts');
            setTimeout(readyTimer, 100);
        }
    };

    var hideEditor = $scope.hideEditor = function () {
        if (editScope.editText) {
            editScope.editText.onFinished(pristineText);
            editScope.editText = null;
        }
        $scope.$parent.$applyAsync(function () {
            $scope.$parent.$hide();
        });
    }
    $timeout(function () {
        tinymce.init({
            selector: 'textarea.tour-text',
            fontsize_formats: "8pt 9pt 10pt 11pt 12pt 14pt 16pt 18pt 20pt 24pt 28pt 32pt 36pt 40pt 44pt 48pt 54pt 60pt 66pt 72pt 80pt 88pt 96pt 112pt 128pt 150pt 200pt",
            height: 500,
            theme: 'modern',
            plugins: ['save contextmenu textcolor colorpicker'],
            toolbar1: 'save | undo redo | fontselect fontsizeselect | bold italic underline | forecolor backcolor ',
            toolbar2: '',
            save_onsavecallback: function () {
                saving = true;
                textObject.text = '';
                while (iframeBody.find('p').last().text().trim() === '') {
                    iframeBody.find('p').last().remove()
                }
                iframeBody.find('p').each(function (i, p) {
                    if (i > 0) {
                        textObject.text += '\n';
                    }
                    textObject.text += $(p).text();
                });
                //console.log(textObject);
                try {
                    function rgb2hex(rgb) {
                        if (rgb.indexOf('rgb') !== 0) return rgb;
                        
                        rgb = rgb.split('(')[1].replace(')','').split(',');
                        function hex(x) {
                            return ("0" + parseInt(x).toString(16)).slice(-2);
                        }
                        var hexColor = "#" + hex(rgb[0]) + hex(rgb[1]) + hex(rgb[2]);
                        return hexColor;
                    }
                    
                    if (editScope.editText) {
                        $.each(editableKeys, function (i, key) {
                            if (pristineText[key] !== textObject[key])
                                if (key.indexOf('Color') > 0) {
                                    pristineText[key] = wwtlib.Color.load(rgb2hex(textObject[key]));
                                } else {
                                    pristineText[key] = textObject[key];
                                }
                        });
                        //console.log(pristineText);

                        editScope.editText.onFinished(pristineText);
                        editScope.editText = null;
                    } else {
                        var txtObj = wwtlib.TextObject.create(
                            textObject.text,
                            textObject.bold,
                            textObject.italic,
                            textObject.underline,
                            textObject.fontSize,
                            textObject.fontName,
                            wwtlib.Color.load(rgb2hex(textObject.foregroundColor)),
                            wwtlib.Color.load(rgb2hex(textObject.backgroundColor)),
                            textObject.borderStyle);
                        //console.log(txtObj);
                        editorUI.addText({}, txtObj);
                    }
                } catch (ex) {
                    console.trace(ex);
                }
                hideEditor();
            },
            content_css: "css/mcecontent.css"
        });
        readyTimer();
    }, 10);
    init();
    }]
);
    
wwt.controllers.controller('ShareController',
[
	'$scope',
	'$rootScope',
	'Util',
	'$timeout',
	'HashManager',
	function($scope, $rootScope, util, $timeout, hashManager) {

		$scope.includeViewport = true;

		$scope.init = function() {
			$scope.shareUrlReadOnly = $scope.shareUrl;
			if ($scope.lookAt !== 'Sky') {
				$scope.shareUrlReadOnly = hashManager.setHashVal('lookAt', $scope.lookAt, true);
			} else if ($scope.lookAt === 'Earth') {
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.removeHashVal('place', true);
			}
			if ($scope.backgroundImagery && $scope.backgroundImagery.get_name() != 'Digitized Sky Survey (Color)') {
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.setHashVal('imagery', $scope.backgroundImagery.get_name(), true);
			}
			if ($('.cross-fader a.btn').css('left') != '100px') {
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.setHashVal('cf', $('.cross-fader a.btn').css('left').replace(/px/, ''), true);
			}

			$('#shareUrl').on('focus', function() {
				$(this).select();
			});
			$scope.includeViewportChange();
			selectUrl(999);
		};

		$rootScope.$on('viewportchange', $scope.init);

		$scope.includeViewportChange = function() {
			if ($scope.includeViewport) {
				hashManager.setHashVal('ra', $rootScope.viewport.RA.toFixed(5), true);
				hashManager.setHashVal('dec', $rootScope.viewport.Dec.toFixed(5), true);
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.setHashVal('fov', $rootScope.viewport.Fov.toFixed(5), true);
			} else {
				hashManager.removeHashVal('ra', true);
				hashManager.removeHashVal('dec', true);
				$scope.shareUrlReadOnly = $scope.shareUrl = hashManager.removeHashVal('fov', true);
			}
			$('meta[property="og:url"]').attr('content', $scope.shareUrlReadOnly);
			$('meta[property="og:title"]').attr('content', $scope.trackingObj ? $scope.trackingObj.get_name() + ' - WorldWide Telescope' : $scope.getFromEn('WorldWide Telescope Web Client'));
			$('meta[property="og:image"]').attr('content', $scope.trackingObj ? $scope.trackingObj.get_thumbnailUrl() : location.host + '/webclient/Images/wwtlogo.png');
			selectUrl(222);
		};

		var selectUrl = function(delay) {
			setTimeout(function() {
				$('#shareUrl')[0].focus();
				$('#shareUrl')[0].select();
			}, delay);
		}

		//$scope.shareFB = function() {
		//	FB.ui({
		//		method: 'share_open_graph',
		//		action_type: 'og.likes',
		//		action_properties: JSON.stringify({
		//			object: $scope.shareUrlReadOnly,
		//			scrape: true
		//		})
		//	},
		//		function(response){
		//			util.log(arguments);
		//		}
		//	);
		//};

		$scope.hide = function() {
			$scope.$parent.$hide();
		};
	}
]);
wwt.controllers.controller('OpenItemController',
	['$rootScope',
	'$scope',
	'AppState',
	'Places',
	'Util',
	'Astrometry',
    'MediaFile',
	function ($rootScope, $scope, appState, places, util, astrometry, media) {
	    
	    $rootScope.$on('openItem', function () {
	        $scope.openItemUrl = '';
	        setTimeout(function () {
	            $('#txtOpenItem').focus();
	        },100);
	    });

	    $scope.openItem = function () {
	        var itemType = $rootScope.openType;
	        if (itemType === 'collection') {
	            places.openCollection($scope.openItemUrl).then(function (folder) {
	                util.log('initExplorer broadcast', folder);
	                $rootScope.newFolder = folder;
	                $rootScope.$broadcast('initExplorer', $scope.openItemUrl);
	                $('#openModal').modal('hide');
	            });
	        } else if (itemType === 'tour') {
	            $scope.playTour($scope.openItemUrl);
	            $('#openModal').modal('hide');
	        } else {
	            //var qs = '&ra=202.45355674088898&dec=47.20018130592933&scale=' + (0.3413275776344843 / 3600) + '&rotation=122.97953942448784';
	            //$scope.openItemUrl = 'http://www.noao.edu/outreach/aop/observers/m51rolfe.jpg';
	            places.importImage($scope.openItemUrl).then(function (folder) {
	                //var imported = folder.get_children()[0];
	                if (folder) {
	                    util.log('initExplorer broadcast', folder);
	                    $rootScope.newFolder = folder;
	                    $rootScope.$broadcast('initExplorer', $scope.openItemUrl);
	                    $('#openModal').modal('hide');
	                } else {
	                    $scope.importState = 'notAVMTagged';
	                    $scope.imageFail = true;
	                }
	            });
	        } 
	    };

	    $scope.mediaFileChange = function (e) {
	        var type = $rootScope.openType;
	        console.time('openLocal: ' + type);
	        var file = e.target.files[0];
	        if (!file.name) {
	            return;
	        }

	        $scope[type + 'FileName'] = file.name;

	        media.addLocalMedia(type, file).then(function (mediaResult) {
	            console.timeEnd('openLocal: ' + type);
	            $scope.openItemUrl = mediaResult.url;
	            $scope.openItem();
	        });

	    }

		$scope.astrometryStatusText = '';
		$scope.astroCallback = function(data) {
			if ($scope.astrometryStatusText.indexOf(data.message) == 0) {
				$scope.astrometryStatusText += ' .';
			} else {
				$scope.astrometryStatusText = data.message;
			}
			if (data.calibration) {
				/*calibration.ra = result.ra; // in degrees devide 15 for hours
				calibration.dec = result.dec; // in degrees
				calibration.rotation = result.orientation;
				calibration.scale = result.pixscale;
				calibration.parity = result.parity;
				calibration.radius = result.radius;*/
				$scope.importState = 'astrometrySuccess';
				var qs = '&ra=' + data.calibration.ra +
					'&dec=' + data.calibration.dec +
					'&scale=' + (data.calibration.scale / 3600) +
					'&rotation=' + data.calibration.rotation;
				if (data.calibration.parity !== 1) {
					qs += '&reverseparity=true';
				}

				places.importImage($scope.openItemUrl, qs).then(function (folder) {
							
					$rootScope.newFolder = folder;
					$rootScope.$broadcast('initExplorer', $scope.openItemUrl);
					$scope.imageFail = false;
					$scope.importState = '';
					$('#openModal').modal('hide');
					return;
				});
			}
			if (data.status.toLowerCase().indexOf('fail') != -1) {
				$scope.importState = 'astrometryFail';
					
			}
		}

		$scope.solveAstrometry = function () {
		    $scope.importState = 'astrometryProgress';
		    astrometry.submitImage($scope.openItemUrl, $scope.astroCallback, false);
		};
			
	}
]);


wwt.controllers.controller('ObservingTimeController',
	['$scope',
	'AppState',
	function ($scope, appState) {
		var now = $scope.now;
		$scope.dt = {};
		$scope.dt.year = now.getFullYear();
		$scope.dt.month = (now.getMonth() + 1) % 12;
		$scope.dt.date = now.getDate();
		$scope.dt.hours = now.getHours();
		$scope.dt.minutes = now.getMinutes();
		$scope.dt.seconds = now.getSeconds();

		$scope.setNow = function () {
			var date = new Date(parseInt($scope.dt.year),
				parseInt($scope.dt.month) - 1,
				parseInt($scope.dt.date),
				parseInt($scope.dt.hours),
				parseInt($scope.dt.minutes),
				parseInt($scope.dt.seconds));
			wwtlib.SpaceTimeController.set_now(date);
		};
	}
]);
wwt.controllers.controller('SlideSelectionController', ['$scope', '$timeout', function ($scope,$timeout) {
    var tourScope = angular.element('#currentTourPanel').scope();
    var overlays, selectionSet, selection;
    var init = $scope.init = function () {
        selection = tourScope.tourEdit.tourEditorUI.selection;
        selectionSet = $scope.selectionSet = selection.selectionSet;
        
        overlays = $scope.overlays = tourScope.selectedSlide._overlays;

        $scope.$applyAsync(function () {
            overlays.forEach(function (overlay, j) {
                overlay.selected = selection.isOverlaySelected(overlay);
            });
        });
    }

    $scope.selectionChange = function (overlay) {
        selection.clearSelection();
        var range = [];
        overlays.forEach(function (overlay, i) {
            if (overlay.selected) {
                range.push(overlay);
            }
        });
        selection.addSelectionRange(range);

    }

    var rebind = function () {
        init.apply($scope, []);
    }

    $('canvas').on('dblclick click keyup', rebind);
    tourScope.$on('initSlides', rebind);
    $timeout(rebind, 100);
}]);
wwt.controllers.controller('LoginController',
    ['$scope',
    '$rootScope',
    '$http',
    'Util',
    '$cookies',
    '$timeout',
    function ($scope, $rootScope, $http, util, $cookies, $timeout) {

        $rootScope.loggedIn = false;

        function init() {

            $rootScope.liveAppId = $('body').data('liveid');
            if (util.getQSParam('code') != null) {

                var returnUrl = location.href.split('?')[0];
                location.href = '/LiveId/AuthenticateFromCode/' + util.getQSParam('code') +
                    '?returnUrl=' + encodeURIComponent(returnUrl);
            } else if ($cookies.get('access_token')) {

                $rootScope.loggedIn = true;
                $rootScope.token = $cookies.get('access_token');
            }
        }

        function log(response) {
            if (response.refresh_token) {
                $cookies.put('refresh_token', response.refresh_token, { expires: new Date(2050, 1, 1) });
                $cookies.put('access_token', response.access_token, { expires: new Date(2050, 1, 1) });
                $timeout(function () {
                    $rootScope.loggedIn = true;
                });
            }
            console.log(response, arguments);
        }

        $scope.login = function () {
            localStorage.setItem('login', new Date().valueOf())
            var redir = 'http://' + location.host + '/webclient';
            var wlUrl = 'https://login.live.com/oauth20_authorize.srf?client_id=' +
                $rootScope.liveAppId + '&scope=wl.offline_access%20wl.emails&response_type=code&redirect_uri=' +
                encodeURIComponent(redir) + '&display=popup';
            location.href = wlUrl;
            return;
        }

        $scope.logout = function () {
            localStorage.setItem('login', new Date().valueOf())
            var storedData = localStorage.getItem('userSettings');
            var data = storedData ? JSON.parse(storedData) : {};
            data['rememberMe'] = false;
            localStorage.setItem('userSettings', JSON.stringify(data));
            location.href = '/Logout';
        }

        init();

    }]);
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

				if (document.body.setPointerCapture) {
					document.body.setPointerCapture(event.pointerId);
				}
				else if (document.body.msSetPointerCapture) {
					document.body.msSetPointerCapture(event.pointerId);
				}
				event.preventDefault();
				event.stopPropagation();
				if (event.pointerId) {
					pointerId = event.pointerId;
				}
				
				moveInit(event);

				document.body.addEventListener(pointerUpName, unbind, false);
				document.body.addEventListener(pointerMoveName, function (evt) {
					if (pointerId && evt.pointerId === pointerId) {
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