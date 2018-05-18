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
		            item.thumb = item.get_thumbnailUrl().replace("//wwtstaging.azurewebsites.net/Content/Images/", "https://wwtweb.blob.core.windows.net/images/");
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
		
			root.loadFromUrl('//worldwidetelescope.org/Resource/Service/Payload', function () {
				deferred.resolve(root.get_children());
			});
		}

		tryInit();
		
		return deferred.promise;
	};

	
	var initPromise = init();

	return api;
}]);

