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