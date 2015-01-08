/*! Script# Runtime
 * Designed and licensed for use and distribution with Script#-generated scripts.
 * Copyright (c) 2012, Nikhil Kothari, and the Script# Project.
 * More information at http://scriptsharp.com
 */

"use strict";

(function(global) {
  function _ss() {

// Various Helpers/Utilities

function _nop() {
}

function isValue(o) {
  return (o !== null) && (o !== undefined);
}

function _value(args) {
  for (var i = 2, l = args.length; i < l; i++) {
    if (isValue(args[i])) {
      return args[i];
    }
  }
  return null;
}
function value(a, b) {
  return isValue(a) ? a : isValue(b) ? b : _value(arguments);
}

function extend(o, items) {
  for (var n in items) {
    o[n] = items[n];
  }
  return o;
}

function parseBoolean(s) {
  return (s.toLowerCase() == 'true');
}

function parseRegExp(s) {
  if (s[0] == '/') {
    var endSlashIndex = s.lastIndexOf('/');
    if (endSlashIndex > 1) {
      var expression = s.substring(1, endSlashIndex);
      var flags = s.substr(endSlashIndex + 1);
      return new RegExp(expression, flags);
    }
  }

  return null;
}

function parseNumber(s) {
  if (!s || !s.length) {
    return 0;
  }
  if ((s.indexOf('.') >= 0) || (s.indexOf('e') >= 0) ||
      endsWith(s, 'f') || endsWith(s, 'F')) {
    return parseFloat(s);
  }
  return parseInt(s, 10);
}

function parseDate(s) {
  var t = Date.parse(s);
  return isNaN(t) ? undefined : new Date(t);
}

function truncate(n) {
  return (n >= 0) ? Math.floor(n) : Math.ceil(n);
}

function now() {
  return new Date();
}

function today() {
  var d = new Date();
  return new Date(d.getFullYear(), d.getMonth(), d.getDate());
}

function compareDates(d1, d2) {
  return (d1 === d2) ? true : ((isValue(d1) && isValue(d2)) ? (d1.getTime() == d2.getTime()) : false);
}

function _popStackFrame(e) {
  if (!isValue(e.stack) ||
      !isValue(e.fileName) ||
      !isValue(e.lineNumber)) {
    return;
  }

  var stackFrames = e.stack.split('\n');
  var currentFrame = stackFrames[0];
  var pattern = e.fileName + ':' + e.lineNumber;
  while (isValue(currentFrame) && currentFrame.indexOf(pattern) === -1) {
    stackFrames.shift();
    currentFrame = stackFrames[0];
  }

  var nextFrame = stackFrames[1];
  if (!isValue(nextFrame)) {
    return;
  }

  var nextFrameParts = nextFrame.match(/@(.*):(\d+)$/);
  if (!isValue(nextFrameParts)) {
    return;
  }

  stackFrames.shift();
  e.stack = stackFrames.join('\n');
  e.fileName = nextFrameParts[1];
  e.lineNumber = parseInt(nextFrameParts[2], 10);
}

function error(message, errorInfo, innerException) {
  var e = new Error(message);
  if (errorInfo) {
    for (var v in errorInfo) {
      e[v] = errorInfo[v];
    }
  }
  if (innerException) {
    e.innerException = innerException;
  }

  _popStackFrame(e);
  return e;
}

function fail(message) {
  console.assert(false, message);
  if (global.navigator) {
    eval('debugger;');
  }
}

// Collections

function toArray(obj) {
  return obj ? (typeof obj == 'string' ? JSON.parse('(' + obj + ')') : Array.prototype.slice.call(obj)) : null;
}
function removeItem(a, item) {
  var index = a.indexOf(item);
  return index >= 0 ? (a.splice(index, 1), true) : false;
}

function clearKeys(obj) {
  for (var key in obj) {
    delete obj[key];
  }
}
function keyExists(obj, key) {
  return obj[key] !== undefined;
}
function keys(obj) {
  if (Object.keys) {
    return Object.keys(obj);
  }
  var keys = [];
  for (var key in obj) {
    keys.push(key);
  }
  return keys;
}
function keyCount(obj) {
  return keys(obj).length;
}

function Enumerator(obj, keys) {
  var index = -1;
  var length = keys ? keys.length : obj.length;
  var lookup = keys ? function() { return { key: keys[index], value: obj[keys[index]] }; } :
                      function() { return obj[index]; };

  this.current = null;
  this.moveNext = function() {
    index++;
    this.current = lookup();
    return index < length;
  };
  this.reset = function() {
    index = -1;
    this.current = null;
  };
}
var _nopEnumerator = {
  current: null,
  moveNext: function() { return false; },
  reset: _nop
};

function enumerate(o) {
  if (!isValue(o)) {
    return _nopEnumerator;
  }
  if (o.getEnumerator) {
    return o.getEnumerator();
  }
  if (o.length !== undefined) {
    return new Enumerator(o);
  }
  return new Enumerator(o, keys(o));
}

function Stack() {
  this.count = 0;
  this._items = [];
}
var Stack$ = {

  clear: function() {
    this._items.length = 0;
    this.count = 0;
  },
  contains: function(item) {
    for (var i = this.count - 1; i >= 0; i--) {
      if (this._items[i] === item) {
        return true;
      }
    }
    return false;
  },
  getEnumerator: function() {
    return new Enumerator(this._items.reverse());
  },
  peek: function() {
    return this._items[this.count - 1];
  },
  push: function(item) {
    this._items.push(item);
    this.count++;
  },
  pop: function() {
    if (this.count) {
      this.count--;
      return this._items.pop();
    }
    return undefined;
  }
}

function Queue() {
  this.count = 0;
  this._items = [];
  this._offset = 0;
}
function _cleanQueue(q) {
  q._items = q._items.slice(q._offset);
  q._offset = 0;
}
var Queue$ = {

  clear: function() {
    this._items.length = 0;
    this._offset = 0;
    this.count = 0;
  },
  contains: function(item) {
    for (var i = this._offset, length = this._items.length; i <= length; i++) {
      if (this._items[i] === item) {
        return true;
      }
    }
    return false;
  },
  dequeue: function() {
    if (this.count) {
      var item = this._items[this._offset];
      if (++this._offset * 2 >= this._items.length) {
        _cleanQueue(this);
      }
      this.count--;
      return item;
    }
    return undefined;
  },
  enqueue: function(item) {
    this._items.push(item);
    this.count++;
  },
  getEnumerator: function() {
    if (this._offset != 0) {
      _cleanQueue(this);
    }
    return new Enumerator(this._items);
  },
  peek: function() {
    return this._items.length ? this._items[this._offset] : undefined;
  }
}

// String

function string(arg1, arg2) {
  if (typeof arg2 == 'number') {
    return arg2 > 1 ? new Array(arg2 + 1).join(arg1) : arg1;
  }
  return Array.prototype.join.call(arguments, '');
}

function emptyString(s) {
  return !s || !s.length;
}

function whitespace(s) {
  return emptyString(s) || !s.replace(/^\s*/, '').length;
}

function compareStrings(s1, s2, ignoreCase) {
  s1 = s1 || '', s2 = s2 || '';
  ignoreCase ? (s1 = s1.toUpperCase(), s2 = s2.toUpperCase()) : 0;
  return (s1 === s2) ? 0 : (s1 < s2) ? -1 : 1;
}

var _formatPlaceHolderRE = /(\{[^\}^\{]+\})/g;
var _formatters = {};

function format(cultureOrFormat) {
  var culture = neutralCulture;
  var format = cultureOrFormat;
  var values = Array.prototype.slice.call(arguments, 1);

  if (cultureOrFormat.constructor != String) {
    culture = cultureOrFormat;
    format = values[0];
    values = values.slice(1);
  }

  return format.replace(_formatPlaceHolderRE,
    function(str, match) {
      var index = parseInt(match.substr(1), 10);
      var value = values[index];
      if (!isValue(value)) {
        return '';
      }

      var formatter = _formatters[typeName(value)];
      if (formatter) {
        var formatSpec = '';
        var formatIndex = match.indexOf(':');
        if (formatIndex > 0) {
          formatSpec = match.substring(formatIndex + 1, match.length - 1);
        }
        if (formatSpec && (formatSpec != 'i')) {
          return formatter(value, formatSpec, culture);
        }
      }
      return culture == neutralCulture ? value.toString() : value.toLocaleString();
    });
}

function trim(s, tc) {
  if (tc || !String.prototype.trim) {
    tc = tc ? tc.join('') : null;
    var r = tc ? new RegExp('^[' + tc + ']+|[' + tc + ']+$', 'g') : /^\s+|\s+$/g;
    return s.replace(r, '');
  }
  return s.trim();
}
function trimStart(s, tc) {
  var r = tc ? new RegExp('^[' + tc.join('') + ']+') : /^\s+/;
  return s.replace(r, '');
}
function trimEnd(s, tc) {
  var r = tc ? new RegExp('[' + tc.join('') + ']+$') : /\s+$/;
  return s.replace(r, '');
}
function startsWith(s, prefix) {
  if (emptyString(prefix)) {
    return true;
  }
  if (emptyString(s) || (prefix.length > s.length)) {
    return false;
  }
  return s.substr(0, prefix.length) == prefix;
}
function endsWith(s, suffix) {
  if (emptyString(suffix)) {
    return true;
  }
  if (emptyString(s) || (suffix.length > s.length)) {
    return false;
  }
  return s.substr(-suffix.length) == suffix;
}
function padLeft(s, totalWidth, ch) {
  return (s.length < totalWidth) ? string(ch || ' ', totalWidth - s.length) + s : s;
}
function padRight(s, totalWidth, ch) {
  return (s.length < totalWidth) ? s + string(ch || ' ', totalWidth - s.length) : s;
}
function removeString(s, index, count) {
  if (!count || ((index + count) > s.length)) {
    return s.substr(0, index);
  }
  return s.substr(0, index) + s.substr(index + count);
}
function insertString(s, index, value) {
  if (!value) {
    return s;
  }
  if (!index) {
    return value + s;
  }
  return s.substr(0, index) + value + s.substr(index);
}
function replaceString(s, oldValue, newValue) {
  return s.split(oldValue).join(newValue || '');
}

// Delegate Functionality

function _bindList(fnList) {
  var d = function() {
    var args = arguments;
    var result = null;
    for (var i = 0, l = fnList.length; i < l; i++) {
      result = args.length ? fnList[i].apply(null, args) : fnList[i].call(null);
    }
    return result;
  };
  d._fnList = fnList;
  return d;
}

function bind(fn, o) {
  if (!o) {
    return fn;
  }

  var name = null;
  fn = typeof fn == 'string' ? o[name = fn] : fn;

  var cache = name ? o.$$b || (o.$$b = {}) : null;
  var binding = cache ? cache[name] : null;

  if (!binding) {
    // Create a function that invokes the specified function, in the
    // context of the specified object.
    binding = function() {
      return fn.apply(o, arguments);
    };

    if (cache) {
      cache[name] = binding;
    }
  }
  return binding;
}

function bindAdd(binding, value) {
  if (!binding) {
    return value;
  }
  if (!value) {
    return binding;
  }

  var fnList = [].concat(binding._fnList || binding, value);
  return _bindList(fnList);
}

function bindSub(binding, value) {
  if (!binding) {
    return null;
  }
  if (!value) {
    return binding;
  }

  var fnList = binding._fnList || [binding];
  var index = fnList.indexOf(value);
  if (index >= 0) {
    if (fnList.length == 1) {
      return null;
    }

    fnList = index ? fnList.slice(0, index).concat(fnList.slice(index + 1)) : fnList.slice(1);
    return _bindList(fnList);
  }
  return binding;
}


function bindExport(fn, multiUse, name, root) {
  // Generate a unique name if one is not specified
  name = name || '__' + (new Date()).valueOf();

  // If unspecified, exported bindings go on the global object
  // (so they are callable using a simple identifier).
  root = root || global;

  var exp = {
    name: name,
    detach: function() {
      root[name] = _nop;
    },
    dispose: function() {
      try { delete root[name]; } catch (e) { root[name] = undefined; }
    }
  };

  // Multi-use bindings are exported directly; for the rest a stub is exported, and the stub
  // first auto-disposes, and then invokes the actual binding.
  root[name] = multiUse ? fn : function() {
    exp.dispose();
    return fn.apply(null, arguments);
  };

  return exp;
}

// EventArgs

function EventArgs() {
}
EventArgs.Empty = new EventArgs();

function CancelEventArgs() {
  this.cancel = false;
}

// Contracts

function IDisposable() { }
function IEnumerable() { }
function IEnumerator() { }
function IObserver() { }
function IApplication() { }
function IContainer() { }
function IObjectFactory() { }
function IEventManager() { }
function IInitializable() { }

// StringBuilder

function StringBuilder(s) {
  this._parts = isValue(s) && s !== '' ? [s] : [];
  this.isEmpty = this._parts.length == 0;
}
var StringBuilder$ = {
  append: function(s) {
    if (isValue(s) && s !== '') {
      this._parts.push(s);
      this.isEmpty = false;
    }
    return this;
  },

  appendLine: function(s) {
    this.append(s);
    this.append('\r\n');
    this.isEmpty = false;
    return this;
  },

  clear: function() {
    this._parts = [];
    this.isEmpty = true;
  },

  toString: function(s) {
    return this._parts.join(s || '');
  }
};

// Observable

var _observerStack = [];
var _observerRegistration = {
  dispose: function() {
    _observerStack.pop();
  }
}
function _captureObservers(observers) {
  var registeredObservers = _observerStack;
  var observerCount = registeredObservers.length;

  if (observerCount) {
    observers = observers || [];
    for (var i = 0; i < observerCount; i++) {
      var observer = registeredObservers[i];
      if (observers.indexOf(observer) < 0) {
        observers.push(observer);
      }
    }
    return observers;
  }
  return null;
}
function _invalidateObservers(observers) {
  for (var i = 0, len = observers.length; i < len; i++) {
    observers[i].invalidateObserver();
  }
}

function Observable(v) {
  this._v = v;
  this._observers = null;
}
var Observable$ = {

  getValue: function() {
    this._observers = _captureObservers(this._observers);
    return this._v;
  },
  setValue: function(v) {
    if (this._v !== v) {
      this._v = v;

      var observers = this._observers;
      if (observers) {
        this._observers = null;
        _invalidateObservers(observers);
      }
    }
  }
};
Observable.registerObserver = function(o) {
  _observerStack.push(o);
  return _observerRegistration;
}


function ObservableCollection(items) {
  this._items = items || [];
  this._observers = null;
}
var ObservableCollection$ = {

  get_item: function (index) {
    this._observers = _captureObservers(this._observers);
    return this._items[index];
  },
  set_item: function(index, item) {
    this._items[index] = item;
    this._updated();
  },
  get_length: function() {
    this._observers = _captureObservers(this._observers);
    return this._items.length;
  },
  add: function(item) {
    this._items.push(item);
    this._updated();
  },
  clear: function() {
    this._items.clear();
    this._updated();
  },
  contains: function(item) {
    return this._items.indexOf(item) >= 0;
  },
  getEnumerator: function() {
    this._observers = _captureObservers(this._observers);
    // TODO: Change this
    return this._items.getEnumerator();
  },
  indexOf: function(item) {
    return this._items.indexOf(item);
  },
  insert: function(index, item) {
    this._items.insert(index, item);
    this._updated();
  },
  remove: function(item) {
    if (this._items.remove(item)) {
      this._updated();
      return true;
    }
    return false;
  },
  removeAt: function(index) {
    this._items.splice(index, 1);
    this._updated();
  },
  toArray: function() {
    return this._items;
  },
  _updated: function() {
    var observers = this._observers;
    if (observers) {
      this._observers = null;
      _invalidateObservers(observers);
    }
  }
}

// Task

function Task(result) {
  this._continuations = result !== undefined ?
                          (this.status = 'done', null) :
                          (this.status = 'pending', []);
  this.result = result;
  this.error = null;
}
var Task$ = {
  get_completed: function() {
    return this.status != 'pending';
  },
  changeWith: function(continuation) {
    var task = new Task();
    this.continueWith(function(t) {
      var error = t.error;
      var result;
      if (!error) {
        try {
          result = continuation(t);
        }
        catch (e) {
          error = e;
        }
      }
      _updateTask(task, result, error);
    });
    return task;
  },
  continueWith: function(continuation) {
    if (this._continuations) {
      this._continuations.push(continuation);
    }
    else {
      var self = this;
      setTimeout(function() { continuation(self); }, 0);
    }
    return this;
  },
  done: function(callback) {
    return this.continueWith(function(t) {
      if (t.status == 'done') {
        callback(t.result);
      }
    });
  },
  fail: function(callback) {
    return this.continueWith(function(t) {
      if (t.status == 'failed') {
        callback(t.error);
      }
    });
  },
  then: function(doneCallback, failCallback) {
    return this.continueWith(function(t) {
      t.status == 'done' ? doneCallback(t.result) : failCallback(t.error);
    });
  }
};

function _updateTask(task, result, error) {
  if (task.status == 'pending') {
    if (error) {
      task.error = error;
      task.status = 'failed';
    }
    else {
      task.result = result;
      task.status = 'done';
    }

    var continuations = task._continuations;
    task._continuations = null;

    for (var i = 0, c = continuations.length; i < c; i++) {
      continuations[i](task);
    }
  }
}

function _joinTasks(tasks, any) {
  tasks = toArray(tasks);

  var count = tasks.length;

  var interval = 0;
  if ((count > 1) && (typeof tasks[0] == 'number')) {
    interval = tasks[0];
    tasks = tasks.slice(1);
    count--;
  }
  if (Array.isArray(tasks[0])) {
    tasks = tasks[0];
    count = tasks.length;
  }

  var joinTask = new Task();
  var seen = 0;

  function continuation(t) {
    if (joinTask.status == 'pending') {
      seen++;
      if (any) {
        _updateTask(joinTask, t);
      }
      else if (seen == count) {
        _updateTask(joinTask, true);
      }
    }
  }

  function timeout() {
    if (joinTask.status == 'pending') {
      if (any) {
        _updateTask(joinTask, null);
      }
      else {
        _updateTask(joinTask, false);
      }
    }
  }

  if (interval != 0) {
    setTimeout(timeout, interval);
  }

  for (var i = 0; i < count; i++) {
    tasks[i].continueWith(continuation);
  }

  return joinTask;
}
Task.all = function() {
  return _joinTasks(arguments, false);
}
Task.any = function() {
  return _joinTasks(arguments, true);
}
Task.delay = function(timeout) {
  var timerTask = new Task();

  setTimeout(function() {
    _updateTask(timerTask, true);
  }, timeout);

  return timerTask;
}

function deferred(result) {
  var task = new Task(result);

  return {
    task: task,
    resolve: function(result) {
      _updateTask(task, result);
    },
    reject: function(error) {
      _updateTask(task, null, (error || new Error()));
    }
  };
}

// Culture

var neutralCulture = {
  name: '',
  // numberFormat
  nf: {
    nan: 'NaN',           // naNSymbol
    neg: '-',             // negativeSign
    pos: '+',             // positiveSign
    negInf: '-Infinity',  // negativeInfinityText
    posInf: 'Infinity',   // positiveInfinityText
    gw: [3],              // numberGroupSizes
    dd: 2,                // numberDecimalDigits
    ds: '.',              // numberDecimalSeparator
    gs: ',',             // numberGroupSeparator

    per: '%',             // percentSymbol
    perGW: [3],           // percentGroupSizes
    perDD: 2,             // percentDecimalDigits
    perDS: '.',           // percentDecimalSeparator
    perGS: ',',           // percentGroupSeparator
    perPP: '{0} %',       // percentPositivePattern
    perNP: '-{0} %',      // percentNegativePattern

    cur: '$',             // currencySymbol
    curGW: [3],           // currencyGroupSizes
    curDD: 2,             // currencyDecimalDigits
    curDS: '.',           // currencyDecimalSeparator
    curGS: ',',           // currencyGroupSeparator
    curNP: '(${0})',      // currencyNegativePattern
    curPP: '${0}'         // currencyPositivePattern
  },
  // dateFormat
  dtf: {
    am: 'AM',             // amDesignator
    pm: 'PM',             // pmDesignator

    ds: '/',              // dateSeparator
    ts: ':',              // timeSeparator

    gmt: 'ddd, dd MMM yyyy HH:mm:ss \'GMT\'',   // gmtDateTimePattern
    uni: 'yyyy-MM-dd HH:mm:ssZ',                // universalDateTimePattern
    sort: 'yyyy-MM-ddTHH:mm:ss',                // sortableDateTimePattern
    dt: 'dddd, MMMM dd, yyyy h:mm:ss tt',       // dateTimePattern

    ld: 'dddd, MMMM dd, yyyy',                  // longDatePattern
    sd: 'M/d/yyyy',                             // shortDatePattern

    lt: 'h:mm:ss tt',                           // longTimePattern
    st: 'h:mm tt',                              // shortTimePattern

    day0: 0,                                    // firstDayOfWeek
    day: ['Sunday', 'Monday', 'Tuesday',
          'Wednesday', 'Thursday',
          'Friday', 'Saturday'],                // dayNames
    sday: ['Sun', 'Mon', 'Tue', 'Wed',
           'Thu', 'Fri', 'Sat'],                // shortDayNames
    mday: ['Su', 'Mo', 'Tu', 'We',
           'Th', 'Fr', 'Sa'],                   // minimizedDayNames

    mon: ['January', 'February', 'March',
          'April', 'May', 'June', 'July',
          'August', 'September', 'October',
          'November', 'December', ''],          // monthNames
    smon: ['Jan', 'Feb', 'Mar', 'Apr',
           'May', 'Jun', 'Jul', 'Aug',
           'Sep', 'Oct', 'Nov', 'Dec', '']      // shortMonthNames
  }
};

var currentCulture = { name: 'en-us', dtf: neutralCulture.dtf, nf: neutralCulture.nf };

// Formatting Helpers

function _commaFormatNumber(number, groups, decimal, comma) {
  var decimalPart = null;
  var decimalIndex = number.indexOf(decimal);
  if (decimalIndex > 0) {
    decimalPart = number.substr(decimalIndex);
    number = number.substr(0, decimalIndex);
  }

  var negative = number.startsWith('-');
  if (negative) {
    number = number.substr(1);
  }

  var groupIndex = 0;
  var groupSize = groups[groupIndex];
  if (number.length < groupSize) {
    return decimalPart ? number + decimalPart : number;
  }

  var index = number.length;
  var s = '';
  var done = false;
  while (!done) {
    var length = groupSize;
    var startIndex = index - length;
    if (startIndex < 0) {
      groupSize += startIndex;
      length += startIndex;
      startIndex = 0;
      done = true;
    }
    if (!length) {
      break;
    }

    var part = number.substr(startIndex, length);
    if (s.length) {
      s = part + comma + s;
    }
    else {
      s = part;
    }
    index -= length;

    if (groupIndex < groups.length - 1) {
      groupIndex++;
      groupSize = groups[groupIndex];
    }
  }

  if (negative) {
    s = '-' + s;
  }
  return decimalPart ? s + decimalPart : s;
}

_formatters['Number'] = function(number, format, culture) {
  var nf = culture.nf;
  var s = '';
  var precision = -1;

  if (format.length > 1) {
    precision = parseInt(format.substr(1));
  }

  var fs = format.charAt(0);
  switch (fs) {
    case 'd': case 'D':
      s = parseInt(Math.abs(number)).toString();
      if (precision != -1) {
        s = padLeft(s, precision, '0');
      }
      if (number < 0) {
        s = '-' + s;
      }
      break;
    case 'x': case 'X':
      s = parseInt(Math.abs(number)).toString(16);
      if (fs == 'X') {
        s = s.toUpperCase();
      }
      if (precision != -1) {
        s = padLeft(s, precision, '0');
      }
      break;
    case 'e': case 'E':
      if (precision == -1) {
        s = number.toExponential();
      }
      else {
        s = number.toExponential(precision);
      }
      if (fs == 'E') {
        s = s.toUpperCase();
      }
      break;
    case 'f': case 'F':
    case 'n': case 'N':
      if (precision == -1) {
        precision = nf.dd;
      }
      s = number.toFixed(precision).toString();
      if (precision && (nf.ds != '.')) {
        var index = s.indexOf('.');
        s = s.substr(0, index) + nf.ds + s.substr(index + 1);
      }
      if ((fs == 'n') || (fs == 'N')) {
        s = _commaFormatNumber(s, nf.gw, nf.ds, nf.gs);
      }
      break;
    case 'c': case 'C':
      if (precision == -1) {
        precision = nf.curDD;
      }
      s = Math.abs(number).toFixed(precision).toString();
      if (precision && (nf.curDS != '.')) {
        var index = s.indexOf('.');
        s = s.substr(0, index) + nf.curDS + s.substr(index + 1);
      }
      s = _commaFormatNumber(s, nf.curGW, nf.curDS, nf.curGS);
      if (number < 0) {
        s = String.format(culture, nf.curNP, s);
      }
      else {
        s = String.format(culture, nf.curPP, s);
      }
      break;
    case 'p': case 'P':
      if (precision == -1) {
        precision = nf.perDD;
      }
      s = (Math.abs(number) * 100.0).toFixed(precision).toString();
      if (precision && (nf.perDS != '.')) {
        var index = s.indexOf('.');
        s = s.substr(0, index) + nf.perDS + s.substr(index + 1);
      }
      s = _commaFormatNumber(s, nf.perGW, nf.perDS, nf.perGS);
      if (number < 0) {
        s = String.format(culture, nf.perNP, s);
      }
      else {
        s = String.format(culture, nf.perPP, s);
      }
      break;
  }

  return s;
}


var _dateFormatRE = /'.*?[^\\]'|dddd|ddd|dd|d|MMMM|MMM|MM|M|yyyy|yy|y|hh|h|HH|H|mm|m|ss|s|tt|t|fff|ff|f|zzz|zz|z/g;

_formatters['Date'] = function(dt, format, culture) {
  if (format == 'iso') {
    return dt.toISOString();
  }
  else if (format.charAt(0) == 'i') {
    var fnName = 'String';
    switch (format) {
      case 'id': fnName = 'DateString'; break;
      case 'it': fnName = 'TimeString'; break;
    }
    return culture == neutralCulture ? dt['to' + fnName]() : dt['toLocale' + fnName]();
  }

  var dtf = culture.dtf;

  if (format.length == 1) {
    switch (format) {
      case 'f': format = dtf.ld + ' ' + dtf.st; break;
      case 'F': format = dtf.dt; break;

      case 'd': format = dtf.sd; break;
      case 'D': format = dtf.ld; break;

      case 't': format = dtf.st; break;
      case 'T': format = dtf.lt; break;

      case 'g': format = dtf.sd + ' ' + dtf.st; break;
      case 'G': format = dtf.sd + ' ' + dtf.lt; break;

      case 'R': case 'r':
        dtf = neutralCulture.dtf;
        format = dtf.gmt;
        break;
      case 'u': format = dtf.uni; break;
      case 'U':
        format = dtf.dt;
        dt = new Date(dt.getUTCFullYear(), dt.getUTCMonth(), dt.getUTCDate(),
                      dt.getUTCHours(), dt.getUTCMinutes(), dt.getUTCSeconds(), dt.getUTCMilliseconds());
        break;

      case 's': format = dtf.sort; break;
    }
  }

  if (format.charAt(0) == '%') {
    format = format.substr(1);
  }

  var sb = new StringBuilder();

  _dateFormatRE.lastIndex = 0;
  while (true) {
    var index = _dateFormatRE.lastIndex;
    var match = _dateFormatRE.exec(format);

    sb.append(format.slice(index, match ? match.index : format.length));
    if (!match) {
      break;
    }

    var fs = match[0];
    var part = fs;
    switch (fs) {
      case 'dddd':
        part = dtf.day[dt.getDay()];
        break;
      case 'ddd':
        part = dtf.sday[dt.getDay()];
        break;
      case 'dd':
        part = padLeft(dt.getDate().toString(), 2, '0');
        break;
      case 'd':
        part = dt.getDate();
        break;
      case 'MMMM':
        part = dtf.mon[dt.getMonth()];
        break;
      case 'MMM':
        part = dtf.smon[dt.getMonth()];
        break;
      case 'MM':
        part = padLeft((dt.getMonth() + 1).toString(), 2, '0');
        break;
      case 'M':
        part = (dt.getMonth() + 1);
        break;
      case 'yyyy':
        part = dt.getFullYear();
        break;
      case 'yy':
        part = padLeft((dt.getFullYear() % 100).toString(), 2, '0');
        break;
      case 'y':
        part = (dt.getFullYear() % 100);
        break;
      case 'h': case 'hh':
        part = dt.getHours() % 12;
        if (!part) {
          part = '12';
        }
        else if (fs == 'hh') {
          part = padLeft(part.toString(), 2, '0');
        }
        break;
      case 'HH':
        part = padLeft(dt.getHours().toString(), 2, '0');
        break;
      case 'H':
        part = dt.getHours();
        break;
      case 'mm':
        part = padLeft(dt.getMinutes().toString(), 2, '0');
        break;
      case 'm':
        part = dt.getMinutes();
        break;
      case 'ss':
        part = padLeft(dt.getSeconds().toString(), 2, '0');
        break;
      case 's':
        part = dt.getSeconds();
        break;
      case 't': case 'tt':
        part = (dt.getHours() < 12) ? dtf.am : dtf.pm;
        if (fs == 't') {
          part = part.charAt(0);
        }
        break;
      case 'fff':
        part = padLeft(dt.getMilliseconds().toString(), 3, '0');
        break;
      case 'ff':
        part = padLeft(dt.getMilliseconds().toString(), 3).substr(0, 2);
        break;
      case 'f':
        part = padLeft(dt.getMilliseconds().toString(), 3).charAt(0);
        break;
      case 'z':
        part = dt.getTimezoneOffset() / 60;
        part = ((part >= 0) ? '-' : '+') + Math.floor(Math.abs(part));
        break;
      case 'zz': case 'zzz':
        part = dt.getTimezoneOffset() / 60;
        part = ((part >= 0) ? '-' : '+') + padLeft(Math.floor(Math.abs(part)).toString(), 2, '0');
        if (fs == 'zzz') {
          part += dtf.ts + padLeft(Math.abs(dt.getTimezoneOffset() % 60).toString(), 2, '0');
        }
        break;
      default:
        if (part.charAt(0) == '\'') {
          part = part.substr(1, part.length - 2).replace(/\\'/g, '\'');
        }
        break;
    }
    sb.append(part);
  }

  return sb.toString();
}

// Type System

var _modules = {};

var _classMarker = 'class';
var _interfaceMarker = 'interface';

function createType(typeName, typeInfo, typeRegistry) {
  // The typeInfo is either an array of information representing
  // classes and interfaces, or an object representing enums and resources
  // or a function, representing a record factory.

  if (Array.isArray(typeInfo)) {
    var type = typeInfo[0];

    // A class is minimally the class type and an object representing
    // its prototype members, and optionally the base type, and references
    // to interfaces implemented by the class.
    if (typeInfo.length >= 2) {
      var baseType = typeInfo[2];
      if (baseType) {
        // Chain the prototype of the base type (using an anonymous type
        // in case the base class is not creatable, or has side-effects).
        var anonymous = function() {};
        anonymous.prototype = baseType.prototype;
        type.prototype = new anonymous();
        type.prototype.constructor = type;
      }

      // Add the type's prototype members if there are any
      typeInfo[1] && extend(type.prototype, typeInfo[1]);

      type.$base = baseType || Object;
      type.$interfaces = typeInfo.slice(3);
      type.$type = _classMarker;
    }
    else {
      type.$type = _interfaceMarker;
    }

    type.$name = typeName;
    return typeRegistry[typeName] = type;
  }

  return typeInfo;
}

function isClass(fn) {
  return fn.$type == _classMarker;
}

function isInterface(fn) {
  return fn.$type == _interfaceMarker;
}

function typeOf(instance) {
  var ctor;

  // NOTE: We have to catch exceptions because the constructor
  //       cannot be looked up on native COM objects
  try {
    ctor = instance.constructor;
  }
  catch (ex) {
  }
  return ctor || Object;
}

function type(s) {
  var nsIndex = s.indexOf('.');
  var ns = nsIndex > 0 ? _modules[s.substr(0, nsIndex)] : global;
  var name = nsIndex > 0 ? s.substr(nsIndex + 1) : s;

  return ns ? ns[name] : null;
}

var _typeNames = [
  Number, 'Number',
  String, 'String',
  Boolean, 'Boolean',
  Array, 'Array',
  Date, 'Date',
  RegExp, 'RegExp',
  Function, 'Function'
];
function typeName(type) {
  if (!(type instanceof Function)) {
    type = type.constructor;
  }
  if (type.$name) {
    return type.$name;
  }
  if (type.name) {
    return type.name;
  }
  for (var i = 0, len = _typeNames.length; i < len; i += 2) {
    if (type == _typeNames[i]) {
      return _typeNames[i + 1];
    }
  }
  return 'Object';
}

function canAssign(type, otherType) {
  // Checks if the specified type is equal to otherType,
  // or is a parent of otherType

  if ((type == Object) || (type == otherType)) {
    return true;
  }
  if (type.$type == _classMarker) {
    var baseType = otherType.$base;
    while (baseType) {
      if (type == baseType) {
        return true;
      }
      baseType = baseType.$base;
    }
  }
  else if (type.$type == _interfaceMarker) {
    var baseType = otherType;
    while (baseType) {
      var interfaces = baseType.$interfaces;
      if (interfaces && (interfaces.indexOf(type) >= 0)) {
        return true;
      }
      baseType = baseType.$base;
    }
  }
  return false;
}

function instanceOf(type, instance) {
  // Checks if the specified instance is of the specified type

  if (!isValue(instance)) {
    return false;
  }

  if ((type == Object) || (instance instanceof type)) {
    return true;
  }

  var instanceType = typeOf(instance);
  return canAssign(type, instanceType);
}

function canCast(instance, type) {
  return instanceOf(type, instance);
}

function safeCast(instance, type) {
  return instanceOf(type, instance) ? instance : null;
}

function module(name, implementation, exports) {
  var registry = _modules[name] = { $name: name };

  if (implementation) {
    for (var typeName in implementation) {
      createType(typeName, implementation[typeName], registry);
    }
  }

  var api = {};
  if (exports) {
    for (var typeName in exports) {
      api[typeName] = createType(typeName, exports[typeName], registry);
    }
  }

  return api;
}


  return extend(module('ss', null, {
      IDisposable: [ IDisposable ],
      IEnumerable: [ IEnumerable ],
      IEnumerator: [ IEnumerator ],
      IObserver: [ IObserver ],
      IApplication: [ IApplication ],
      IContainer: [ IContainer ],
      IObjectFactory: [ IObjectFactory ],
      IEventManager: [ IEventManager ],
      IInitializable: [ IInitializable ],
      EventArgs: [ EventArgs, { } ],
      CancelEventArgs: [ CancelEventArgs, { }, EventArgs ],
      StringBuilder: [ StringBuilder, StringBuilder$ ],
      Stack: [ Stack, Stack$ ],
      Queue: [ Queue, Queue$ ],
      Observable: [ Observable, Observable$ ],
      ObservableCollection: [ ObservableCollection, ObservableCollection$, null, IEnumerable ],
      Task: [ Task, Task$ ]
    }), {
      version: '0.8',

      isValue: isValue,
      value: value,
      extend: extend,
      keys: keys,
      keyCount: keyCount,
      keyExists: keyExists,
      clearKeys: clearKeys,
      enumerate: enumerate,
      array: toArray,
      remove: removeItem,
      boolean: parseBoolean,
      regexp: parseRegExp,
      number: parseNumber,
      date: parseDate,
      truncate: truncate,
      now: now,
      today: today,
      compareDates: compareDates,
      error: error,
      string: string,
      emptyString: emptyString,
      whitespace: whitespace,
      format: format,
      compareStrings: compareStrings,
      startsWith: startsWith,
      endsWith: endsWith,
      padLeft: padLeft,
      padRight: padRight,
      trim: trim,
      trimStart: trimStart,
      trimEnd: trimEnd,
      insertString: insertString,
      removeString: removeString,
      replaceString: replaceString,
      bind: bind,
      bindAdd: bindAdd,
      bindSub: bindSub,
      bindExport: bindExport,
      deferred: deferred,

      module: module,
      modules: _modules,

      isClass: isClass,
      isInterface: isInterface,
      typeOf: typeOf,
      type: type,
      typeName: typeName,
      canCast: canCast,
      safeCast: safeCast,
      canAssign: canAssign,
      instanceOf: instanceOf,

      culture: {
        neutral: neutralCulture,
        current: currentCulture
      },

      fail: fail
    });
  }


  function _export() {
    var ss = _ss();
    typeof exports == 'object' ? ss.extend(exports, ss) : global.ss = ss;
  }

  global.define ? global.define('ss', [], _ss) : _export();
})(this);
