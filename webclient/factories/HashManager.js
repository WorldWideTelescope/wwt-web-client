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
