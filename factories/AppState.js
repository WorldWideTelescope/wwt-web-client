wwt.app.factory('AppState', function() {
	var api = {
		set: setKey,
		get: getKey,
		getAll:getAll
	};

	var data;
 
	function setKey(key, val) {
        try {
            if (val === null && data[key]) {
                delete data[key]
            } else {
                data[key] = val;

            }
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
