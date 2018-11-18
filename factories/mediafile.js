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
            };
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
