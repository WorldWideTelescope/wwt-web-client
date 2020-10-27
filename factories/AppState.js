wwt.app.factory(
  'AppState',

  function() {
    var api = {
      set: setKey,
      get: getKey,
      getAll: getAll
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

    function getAll() {
      return data;
    }

    var initialState = {
      layerManagerHidden: true,
    };

    var init = function () {
      try {
        var storedString = localStorage.getItem('appState');
        var storedObj = JSON.parse(storedString);

        // This seems like the most robust way to make sure that we got a
        // dictlike (N.B., JSON.parse(null) => null).
        if (Object.prototype.toString.call(storedObj) != '[object Object]') {
          throw new Error('stored object not dict');
        }

        data = storedObj;
      } catch {
        data = initialState;
      }
    };

    init();
    return api;
  }
);
