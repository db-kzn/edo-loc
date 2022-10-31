; (function () {
    "use strict";

    if (window.cadesplugin) { return; }

    let pluginObject;
    let plugin_resolved = 0;

    let isFireFoxExtensionLoaded = false;
    let cadesplugin_loaded_event_recieved = false;

    let isOpera = false;
    let isFireFox = false;
    let isSafari = false;
    let isYandex = false;

    let plugin_resolve;
    let plugin_reject;

    let cadesplugin = new Promise(function (resolve, reject) {
        plugin_resolve = resolve;
        plugin_reject = reject;
    });

    cadesplugin.JSModuleVersion = "2.3.3";
    cadesplugin.current_log_level = cadesplugin.LOG_LEVEL_ERROR;

    if (!window.cadesplugin_load_timeout) { window.cadesplugin_load_timeout = 20000; }
    setTimeout(check_load_timeout, window.cadesplugin_load_timeout);

    cadesplugin.async_spawn = async_spawn;
    cadesplugin.CreateObjectAsync = CreateObjectAsync;
    cadesplugin.ReleasePluginObjects = ReleasePluginObjects;

    cadesplugin.set = set_pluginObject;
    cadesplugin.set_log_level = set_log_level;
    cadesplugin.getLastError = getLastError;
    cadesplugin.is_capilite_enabled = is_capilite_enabled;

    window.cadesplugin = cadesplugin;

    check_plugin_working();

    function async_spawn(generatorFunc) {
        function continuer(verb, arg) {
            var result;
            try {
                result = generator[verb](arg);
            } catch (err) {
                return Promise.reject(err);
            }
            if (result.done) {
                return result.value;
            } else {
                return Promise.resolve(result.value).then(onFulfilled, onRejected);
            }
        }
        var generator = generatorFunc(Array.prototype.slice.call(arguments, 1));
        var onFulfilled = continuer.bind(continuer, "next");
        var onRejected = continuer.bind(continuer, "throw");
        return onFulfilled();
    }
    function CreateObjectAsync(name) { return pluginObject.CreateObjectAsync(name); }
    function ReleasePluginObjects() { return cpcsp_chrome_nmcades.ReleasePluginObjects(); }

    function set_pluginObject(obj) { pluginObject = obj; }
    function set_log_level(level) {
        if (level === cadesplugin.LOG_LEVEL_DEBUG) {
            cpcsp_console_log(cadesplugin.LOG_LEVEL_INFO, "cadesplugin_api.js: log_level = DEBUG");
        } else if (level === cadesplugin.LOG_LEVEL_INFO) {
            cpcsp_console_log(cadesplugin.LOG_LEVEL_INFO, "cadesplugin_api.js: log_level = INFO");
        } else if (level === cadesplugin.LOG_LEVEL_ERROR) {
            cpcsp_console_log(cadesplugin.LOG_LEVEL_INFO, "cadesplugin_api.js: log_level = ERROR");
        } else {
            cpcsp_console_log(cadesplugin.LOG_LEVEL_ERROR, "cadesplugin_api.js: Incorrect log_level: " + level);
            return;
        }

        if (IsNativeMessageSupported()) { return; }

        cadesplugin.current_log_level = level;

        if (cadesplugin.current_log_level === cadesplugin.LOG_LEVEL_DEBUG) {
            window.postMessage("set_log_level=debug", "*");
        } else if (cadesplugin.current_log_level === cadesplugin.LOG_LEVEL_INFO) {
            window.postMessage("set_log_level=info", "*");
        } else if (cadesplugin.current_log_level === cadesplugin.LOG_LEVEL_ERROR) {
            window.postMessage("set_log_level=error", "*");
        }
    }

    function getLastError(e) { return (!e.message) ? e : (e.message.number) ? e.message + " (0x" + decimalToHexString(e.number) + ")" : e.message; }
    function is_capilite_enabled() { return ((typeof (cadesplugin.EnableInternalCSP) !== 'undefined') && cadesplugin.EnableInternalCSP); };

    function check_load_timeout() {
        if (plugin_resolved === 1) { return; }
        if (isFireFox && !isFireFoxExtensionLoaded) { show_firefox_missing_extension_dialog(); }

        plugin_resolved = 1;
        plugin_reject("Истекло время ожидания загрузки плагина");
    }
    function show_firefox_missing_extension_dialog() {
        if (!window.cadesplugin_skip_extension_install) {
            var ovr = document.createElement('div');
            ovr.id = "cadesplugin_ovr";
            ovr.style = "visibility: hidden; position: fixed; left: 0px; top: 0px; width:100%; height:100%; background-color: rgba(0,0,0,0.7)";
            ovr.innerHTML = "<div id='cadesplugin_ovr_item' style='position:relative; max-width:400px; margin:100px auto; background-color:#fff; border:2px solid #000; padding:10px; text-align:center; opacity: 1; z-index: 1500'>" +
                "<button id='cadesplugin_close_install' style='float: right; font-size: 10px; background: transparent; border: 1; margin: -5px'>X</button>" +
                "<p>Для работы КриптоПро ЭЦП Browser plugin на данном сайте необходимо расширение для браузера. Убедитесь, что оно у Вас включено или установите его." +
                "<p><a href='https://www.cryptopro.ru/sites/default/files/products/cades/extensions/firefox_cryptopro_extension_latest.xpi'>Скачать расширение</a></p>" +
                "</div>";
            document.getElementsByTagName("Body")[0].appendChild(ovr);
            document.getElementById("cadesplugin_close_install").addEventListener('click', function () {
                plugin_loaded_error("Плагин недоступен");
                document.getElementById("cadesplugin_ovr").style.visibility = 'hidden';
            });
            ovr.addEventListener('click', function () {
                plugin_loaded_error("Плагин недоступен");
                document.getElementById("cadesplugin_ovr").style.visibility = 'hidden';
            });
            ovr.style.visibility = "visible";
        }
    }

    function check_plugin_working() {
        return (IsNativeMessageSupported())
            ? load_extension()
            : plugin_loaded_error("Браузер не поддерживается");
    }
    function load_extension() {
        if (isFireFox || isSafari) { return nmcades_api_onload(); }

        let fileref = document.createElement('script');
        fileref.setAttribute("type", "text/javascript");
        fileref.setAttribute("src",
            (isOpera || isYandex)
                ? "chrome-extension://epebfcehmdedogndhlcacafjaacknbcm/nmcades_plugin_api.js"
                : "chrome-extension://iifchhfnnmpdbibifmljnfjhpififfog/nmcades_plugin_api.js"
        );
        fileref.onerror = plugin_loaded_error;
        fileref.onload = nmcades_api_onload;
        document.getElementsByTagName("head")[0].appendChild(fileref);
    }
    function nmcades_api_onload() {
        if (!isFireFox && !isSafari && window.cadesplugin_extension_loaded_callback) { window.cadesplugin_extension_loaded_callback(); }

        window.postMessage("cadesplugin_echo_request", "*");
        window.addEventListener("message", function (event) {
            if (typeof (event.data) !== "string" || !event.data.match("cadesplugin_loaded") || cadesplugin_loaded_event_recieved) { return; }

            if (isFireFox || isSafari) {
                // Для Firefox, Сафари вместе с сообщением cadesplugin_loaded прилетает url для загрузки nmcades_plugin_api.js
                var url = event.data.substring(event.data.indexOf("url:") + 4);
                if (!url.match("^(moz|safari)-extension://[a-zA-Z0-9/_-]+/nmcades_plugin_api.js$")) {
                    cpcsp_console_log(cadesplugin.LOG_LEVEL_ERROR, "Bad url \"" + url + "\" for load CryptoPro Extension for CAdES Browser plug-in");
                    plugin_loaded_error();
                    return;
                }
                var fileref = document.createElement('script');
                fileref.setAttribute("type", "text/javascript");
                fileref.setAttribute("src", url);
                fileref.onerror = plugin_loaded_error;
                fileref.onload = firefox_or_safari_nmcades_onload;
                document.getElementsByTagName("head")[0].appendChild(fileref);
            } else {
                cpcsp_chrome_nmcades.check_chrome_plugin(plugin_loaded, plugin_loaded_error);
            }
            cadesplugin_loaded_event_recieved = true;
        }, false);
    }
    function firefox_or_safari_nmcades_onload() {
        if (window.cadesplugin_extension_loaded_callback) { window.cadesplugin_extension_loaded_callback(); }
        isFireFoxExtensionLoaded = true;
        cpcsp_chrome_nmcades.check_chrome_plugin(plugin_loaded, plugin_loaded_error);
    }
    function plugin_loaded() {
        plugin_resolved = 1;
        plugin_resolve();
    }
    function plugin_loaded_error(msg) {
        if (typeof (msg) === 'undefined' || typeof (msg) === 'object') { msg = "Плагин недоступен"; }
        plugin_resolved = 1;
        plugin_reject(msg);
    }

    function decimalToHexString(number) {
        if (number < 0) { number = 0xFFFFFFFF + number + 1; }
        return number.toString(16).toUpperCase();
    }
    function cpcsp_console_log(level, msg) {
        if (typeof (console) === 'undefined') { return; }

        if (level <= cadesplugin.current_log_level) {
            if (level === cadesplugin.LOG_LEVEL_DEBUG) {
                console.log("DEBUG: %s", msg);
            } else if (level === cadesplugin.LOG_LEVEL_INFO) {
                console.info("INFO: %s", msg);
            } else if (level === cadesplugin.LOG_LEVEL_ERROR) {
                console.error("ERROR: %s", msg);
            }
        }
    }
    function check_browser() {
        var ua = navigator.userAgent, tem, M = ua.match(/(opera|yabrowser|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];

        if (/trident/i.test(M[1])) {
            tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
            return { name: 'IE', version: (tem[1] || '') };
        }

        if (M[1] === 'Chrome') {
            tem = ua.match(/\b(OPR|Edg|YaBrowser)\/(\d+)/);
            if (tem !== null) { return { name: tem[1].replace('OPR', 'Opera'), version: tem[2] }; }
        }

        M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];

        if ((tem = ua.match(/version\/(\d+)/i)) !== null) { M.splice(1, 1, tem[1]); }

        return { name: M[0], version: M[1] };
    }
    function IsNativeMessageSupported() {
        let browserSpecs = check_browser();

        if (browserSpecs.name === 'Edg') { return true; }
        if (browserSpecs.name === 'YaBrowser') { isYandex = true; return true; }
        if (browserSpecs.name === 'Chrome') { return (browserSpecs.version >= 42); }
        if (browserSpecs.name === 'Opera') { isOpera = true; return (browserSpecs.version >= 33); }
        if (browserSpecs.name === 'Firefox') { isFireFox = true; return (browserSpecs.version >= 52); }
        if (browserSpecs.name === 'Safari') { isSafari = true; return (browserSpecs.version >= 12); }

        return false;
    }
}());
