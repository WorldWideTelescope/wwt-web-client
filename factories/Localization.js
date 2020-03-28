wwt.app.factory(
  'Localization',
  [
    '$http',
    '$q',
    'Util',

    function($http, $q, util) {
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
	  lpacks;

      function getString(id, s) {
	return locArray && locArray[id] ? locArray[id] : s ? s : '';
      }

      function getFromEn(s) {
	if (langCode === 'EN') {
	  return s;
	} else {
	  return getString($.inArray(s, englishArray), s);
	}
      }

      function getAvailableLanguages() {
	var deferred = $q.defer();

	initPromise.then(function() {
	  var langArray = [];

	  lpacks.find('languagepack').each(function(i, pack) {
	    langArray.push({
              label: $(pack).attr('name'),
              code: $(pack).attr('code')
            });
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

	  var url = wwtlib.URLHelpers.singleton.rewrite(lpacks.find('languagepack[code=' + code + ']').attr('url'));
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
	  $http.get(wwtlib.URLHelpers.singleton.coreStaticUrl('/wwtweb/catalog.aspx?X=Languages'))
	    .success(function(data) {
	      lpacks = $(data);
	    })
	    .error(function(data, status, headers, config) {
	      util.log(data, status, headers, config);
	    }),

	  $http.get(wwtlib.URLHelpers.singleton.coreStaticUrl('/wwtweb/catalog.aspx?Q=lang_en'))
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
    }
  ]
);
