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
		var uploadUrl, // "https://upload.wikimedia.org/wikipedia/commons/8/8b/M81.jpg",
			statusCallback,
			sessionId = null,
			submissionId = null,
			jobId = null, 
			calibration = null,
			jobStatus = null,
			errorData = null,
			debug=false;

		/*var upload = '{"session": "####", "url": "//apod.nasa.gov/apod/image/1206/ldn673s_block1123.jpg", "scale_units": "degwidth", "scale_lower": 0.5, "scale_upper": 1.0, "center_ra": 290, "center_dec": 11, "radius": 2.0 }      ';*/


		function login() {
			showStatus(statusTypes.connecting);
			var loginData = {};
			loginData.apikey = "grgfoujnylhbwtjw"; // this may change we should put it in the web.config

			var loginJson = encodeURIComponent(JSON.stringify(loginData));

			$.ajax({
				url: "//nova.astrometry.net/api/login",
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
				url: "//nova.astrometry.net/api/url_upload",
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
				url: "//nova.astrometry.net/api/submissions/" + submissionId,
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
				url: "//nova.astrometry.net/api/jobs/" + jobId,
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
				url: "//nova.astrometry.net/api/jobs/" + jobId + "/calibration",
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
