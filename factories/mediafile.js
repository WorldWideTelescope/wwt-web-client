wwt.app.factory('MediaFile', ['$q', function ($q) {
    var api = {
        getTourProgress: getTourProgress,
        addTourMedia: addTourMedia,
        flushStore: flushStore,
        getBinaryData:getBinaryData
    };
    var tourMedia = [];

    function Media(params) {
        
        return{
            url:params.url,
            key:params.key,
            db:'touredit',
            size:params.size,
            filename:params.name
        }
        
    }

    var imageIndex = 0;
    

    function getTourProgress() {
        var savedMedia = localStorage.getItem('tourMedia');
        if (savedMedia) {
            tourMedia = JSON.parse(savedMedia);
        }
        return tourMedia
    }
    
    function concatMedia() {
        var mediaPromises = [];
        tourMedia.forEach(function (item) {
            mediaPromises.push(getBinaryData(item.url,true))
        });
        $q.all(mediaPromises).then(function (results) {
            var cab = document.implementation.createDocument(null, "FileCabinet");
            cab.documentElement.setAttribute('HeaderSize','0x00000000');
            var filesNode = cab.createElement('Files');
            cab.documentElement.appendChild(filesNode);
            var offset = 0;
            var cabData =  new Uint8Array();
            results.forEach(function (binary, i) {
                var file = cab.createElement('File');
                file.setAttribute('Name',tourMedia[i].filename);
                file.setAttribute('Url',tourMedia[i].url);
                file.setAttribute('Offset', offset);
                file.setAttribute('Size', binary.length);
                filesNode.appendChild(file);
                offset += binary.length;
                cabData = cabData.concat(binary);
            });
            var serializer = new XMLSerializer();
            var xmlString = "<?xml version='1.0' encoding='UTF-8'?>" + serializer.serializeToString(cab);
            var hex = xmlString.length.toString(16);
            var newHex = ('0x00000000').substr(0,10-hex.length) + hex;
            xmlString = xmlString.replace('0x00000000', newHex);
            
            var xbytes = [];

            for (var i = 0; i < xmlString.length; ++i) {
                xbytes.push(xmlString.charCodeAt(i));
            }
            
            var file = new Blob(xbytes.concat(cabdata), {
                name: 'IndexedDbTour.wtt',
                type: 'application/wtt'
            });
            addTourMedia('wtt', file).then(function (result) {
                console.log(result);
            });
        });
    }

    function addTourMedia(mediaKey, file, db) {
        
        var deferred = $q.defer();
        var keys = ['music', 'voiceOver', 'wtt', 'image'];
        var req = indexedDB.open('touredit');
        req.onupgradeneeded = function () {
            // Define the database schema if necessary.
            var db = req.result;
            var store = db.createObjectStore('files');
        };
        req.onsuccess = function () {
            var db = req.result;

            var key = keys.indexOf(mediaKey);
            if (key === 2) {
                key += imageIndex;
                imageIndex++;
            }
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
                    tourMedia[key] = media;
                    localStorage.setItem('tourMedia', JSON.stringify(tourMedia));
                    //if (tourMedia.length > 1) {
                    //    concatMedia();
                    //}
                };
            };
            addFile();
           
        }
        return deferred.promise;
    }

    function flushStore(db) {
        var deferred = $q.defer();
        var dbName = db || 'touredit';
        var req = indexedDB.deleteDatabase(dbName);
        req.onupgradeneeded = function () {
            deferred.reject('upgradeneeded');
        };
        req.onsuccess = deferred.resolve;
        req.onerror = deferred.reject;
        req.onblocked = deferred.reject;
        return deferred.promise;
    }



    function getBinaryData(url,asUIntArray) {
        var deferred = $q.defer();
        console.time('get binary string');
        var req = new XMLHttpRequest();
        req.open('GET', url, true);
        req.onload = function () {
            if (asUIntArray) {
                var uInt8Array = new Uint8Array(this.response); // Note:not xhr.responseText

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
    return api;
}]);