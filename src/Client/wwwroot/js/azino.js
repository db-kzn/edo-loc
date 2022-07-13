(function () {
    if (!!window.azino) { return; }
    const cadesplugin = window.cadesplugin;

    let pluginAvailable = null;

    cadesplugin
        .then(() => { pluginAvailable = true; })
        .catch(() => { pluginAvailable = false; });

    var CADESCOM_CADES_BES = 1;
    var CADESCOM_BASE64_TO_BINARY = 1;

    var CAPICOM_MY_STORE = "My";
    var CAPICOM_CURRENT_USER_STORE = 2;
    var CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED = 2;

    var CAPICOM_AUTHENTICATED_ATTRIBUTE_SIGNING_TIME = 0;
    var CADESCOM_AUTHENTICATED_ATTRIBUTE_DOCUMENT_NAME = 1;

    window.azino = {};

    const azino = window.azino;
    const certTypes = Object.freeze({
        undefined: 0,
        personal: 1,
        organization: 2,
        company: 3
    });

    azino.about = null;
    azino.store = null;
    azino.license = null;

    azino.signer = null;
    azino.cx509privateKey = null;

    azino.certs = [];
    azino.userCert = null;
    azino.userThumbprint = null;

    azino.Console = (value, tag = "=> ") => console.log(tag, value);
    azino.LinkOpen = (link) => window.open(link, "_blank", "comma,delimited,list,of,window,features");

    azino.CadesPluginApiVersion = () => {
        if (!cadesplugin) { return ""; }

        return (!cadesplugin.JSModuleVersion) ? "" : cadesplugin.JSModuleVersion;
    }
    azino.BrowserPluginVersion = () => {
        if (!pluginAvailable) { return ""; }

        return GetAbout().then(function (about) {
            if (typeof (about.PluginVersion) != "undefined") {
                return about.PluginVersion.then(function (version) {
                    return Promise.all([
                        version.MajorVersion,
                        version.MinorVersion,
                        version.BuildVersion
                    ]).then(v => {
                        return `${v[0]}.${v[1]}.${v[2]}`;
                    });
                });
            }

            if (typeof (about.Version) == "undefined") { return ""; }

            return about.Version.then(function (version) {
                return version;
            });
        }).catch(function (e) { return ""; });
    }
    azino.CspVersion = () => {
        if (!pluginAvailable) { return ""; }

        return GetAbout().then(function (about) {
            if (typeof (about.CSPVersion) == "undefined") { return ""; }

            return about.CSPVersion("", 80).then(function (version) {
                return Promise.all([
                    version.MajorVersion,
                    version.MinorVersion,
                    version.BuildVersion
                ]).then(v => {
                    return `${v[0]}.${v[1]}.${v[2]}`;
                });
            });
        }).catch(function (e) { return ""; });;
    }
    azino.CspName = () => {
        if (!pluginAvailable) { return ""; }

        //return "";

        return GetAbout().then(function (about) {
            if (typeof (about.CSPName) == "undefined") { return ""; }

            return about.CSPName(80).then(function (name) {
                return name;
            });
        }).catch(function (e) { return ""; });;
    }
    azino.CspLicenses = () => {
        if (!pluginAvailable) { return ""; }

        return GetLicense().then(function (license) {
            if (!license) { return ["", "", ""]; }

            return Promise.all([
                license.ValidTo(0),
                license.ValidTo(1),
                license.ValidTo(2)
            ]).then(l => {
                console.log("Licenses: ", l);
                return l;
            });

        }).catch(function (e) { return ""; });;
    }

    azino.GetCertDetails = (thumbprint) => {
        if (!pluginAvailable) { return null; }

        GetCertByThumbprint(thumbprint).then(cert => {
            console.log(cert);
        });
    }

    azino.GetCerts = () => {
        azino.certs.length = 0;

        let result = GetArrayOfCertPromises().then(a => {
            return Promise.all(a.map(c => CreateCert(c).then(cert => {
                return Promise.all([
                    c.IssuerName,
                    c.SubjectName,
                    c.Thumbprint,
                    c.SerialNumber,

                    c.ValidFromDate,
                    c.ValidToDate,
                    c.Version,

                    Promise.resolve({ "provider": "", "privateKeyLink": "" })
                    //cert.hasPrivateKey ? ResolvePrivateKey(c.PrivateKey)
                    //    : Promise.resolve({ "provider": "", "privateKeyLink": "" })

                ]).then(ccc => {
                    cert.linkObj = c;
                    console.log("CERT:", ccc);

                    let issuer = ccc[0];
                    cert.issuer = {
                        "name": Extract(issuer, 'CN='),
                        "inn": Extract(issuer, 'INN='),
                        "ogrn": Extract(issuer, 'OGRN='),
                        "eMail": Extract(issuer, 'E='),

                        "commonName": "",
                        "organization": "",
                        "street": "",
                        "locality": "",
                        "state": "",
                        "country": ""
                    };

                    let subject = ccc[1];
                    cert.subject = {
                        //user
                        "surname": Extract(subject, 'SN='),
                        "givenName": Extract(subject, 'G='),
                        "title": Extract(subject, ' T='),
                        "inn": Extract(subject, 'INN=') || Extract(subject, 'ИНН=') || Extract(subject, 'OID.1.2.643.3.131.1.1='),
                        "snils": Extract(subject, 'SNILS=') || Extract(subject, 'СНИЛС=') || Extract(subject, 'OID.1.2.643.100.3='),
                        // org
                        "org": OrgNameFormat(Extract(subject, 'O=')),
                        "ogrn": Extract(subject, 'OGRN=') || Extract(subject, 'ОГРН=') || Extract(subject, 'OID.1.2.643.100.1='),
                        "innLe": Extract(subject, 'INNLE=') || Extract(subject, 'ИНН ЮЛ=') || Extract(subject, 'OID.1.2.643.100.4='),
                        // cert
                        "name": Extract(subject, 'CN='),
                        "eMail": Extract(subject, 'E='),
                        // address
                        "street": "",
                        "locality": "",
                        "state": "",
                        "country": ""
                    };

                    cert.thumbprint = ccc[2];
                    cert.serialNumber = ccc[3];
                    cert.fromDate = new Date(Date.parse(ccc[4])).toLocaleDateString("ru-RU");
                    cert.tillDate = new Date(Date.parse(ccc[5])).toLocaleDateString("ru-RU");

                    //cert.version = ccc[6];
                    cert.version = (!!cert.subject.innLe) ? certTypes.company : (!!cert.subject.org) ? certTypes.organization : certTypes.personal;
                    //cert.isOrgCert = !!cert.subject.innLe; => cert.version => cert.type

                    cert.provider = ""; //ccc[7].provider;
                    cert.privateKeyLink = ""; // ccc[7].privateKeyLink;
                    cert.isCorrect = (!!cert.hasPrivateKey && !!cert.isValid && !!cert.subject.snils && !!cert.thumbprint); //

                    azino.certs.push(cert);
                    console.log("-->", cert);

                    return cert;
                });
            })));
        });

        result.then(r => console.log("Result: ", r));
        return result;
    }

    azino.TokenCheck = (thumbprint) => {
        return new Promise(function (resolve, reject) {
            cadesplugin.async_spawn(function* (args) {
                var oStore = yield cadesplugin.CreateObjectAsync("CAdESCOM.Store");
                yield oStore.Open(CAPICOM_CURRENT_USER_STORE, CAPICOM_MY_STORE, CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);

                var Certificates = yield oStore.Certificates;
                var certCnt = yield Certificates.Count;
                var oCertificate = null;

                for (var i = 1; i <= certCnt; i++) {
                    let cert;

                    try { cert = yield Certificates.Item(i); }
                    catch (ex) {
                        alert("Ошибка при получении сертификата: ", i, cadesplugin.getLastError(ex));
                        continue;
                    }

                    try {
                        var ctp = yield cert.Thumbprint;
                        if (thumbprint == ctp) {
                            oCertificate = cert;
                            break;
                        }
                    } catch (ex) {
                        alert("Ошибка при получении отпечатка: ", i, cadesplugin.getLastError(ex));
                    }
                }

                console.log("SignIn Cert: ", oCertificate);
                var tokenСonnected = false;

                if (oCertificate != null) {
                    var pk = yield oCertificate.PrivateKey;
                    if (pk) {
                        try {
                            console.log("Private Key: ", pk);
                            var field = yield pk.UniqueContainerName;
                            console.log("PK Field: ", field);
                            tokenСonnected = !!field;
                        } catch (ex) {
                            alert("Ошибка доступа к закрытому ключу: ", cadesplugin.getLastError(ex));
                        }
                    }
                }

                return args[0](tokenСonnected);
            }, resolve, reject); //cadesplugin.async_spawn
        });
    }

    azino.SignCadesBES = (thumbprint, dataInBase64) => {
        //if (!pluginAvailable) { return ""; }
        //cadesplugin.set_log_level(4);
        //console.log("\n thumbprint: ", thumbprint, "\n dataInBase64: ", dataInBase64);

        return SignCreate(thumbprint, dataInBase64)
            .then(signedMessage => { return signedMessage; })
            .catch(() => { return ""; });

        //.then(
        //function (signedMessage) {
        //    console.log("signature :", signedMessage);

        //    Verify(signedMessage, dataInBase64).then(
        //        function () { console.log("Signature verified"); },
        //        function (err) { console.log("ERROR: ", err); }
        //    );
        //},
        //    function (err) { console.log("ERROR: ", err); }
        //);
    };

    azino.SaveFile = (filename, contentType, content) => {
        console.log("SIGN: ", filename, "\n", content);

        // Create the URL
        const file = new File([content], filename, { type: contentType });
        const exportUrl = URL.createObjectURL(file);

        // Create the <a> element and click on it
        const a = document.createElement("a");
        document.body.appendChild(a);
        a.href = exportUrl;
        a.download = filename;
        a.target = "_blank";
        a.click();

        // We don't need to keep the object URL, let's release the memory
        // On older versions of Safari, it seems you need to comment this line...
        URL.revokeObjectURL(exportUrl);
    }

    function SignCreate(thumbprint, dataInBase64, docTitle = "") {
        return new Promise(function (resolve, reject) {
            cadesplugin.async_spawn(function* (args) {
                var oStore = yield cadesplugin.CreateObjectAsync("CAdESCOM.Store");
                yield oStore.Open(CAPICOM_CURRENT_USER_STORE, CAPICOM_MY_STORE, CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);

                var Certificates = yield oStore.Certificates;
                var certCnt = yield Certificates.Count;
                var oCertificate;

                for (var i = 1; i <= certCnt; i++) {
                    let cert;

                    try { cert = yield Certificates.Item(i); }
                    catch (ex) {
                        alert("Ошибка при получении сертификата: ", i, cadesplugin.getLastError(ex));
                        continue;
                    }

                    try {
                        var ctp = yield cert.Thumbprint;
                        if (thumbprint == ctp) {
                            oCertificate = cert;
                            break;
                        }
                    } catch (ex) {
                        alert("Ошибка при получении отпечатка: ", i, cadesplugin.getLastError(ex));
                    }
                }

                var oSigningTimeAttr = yield cadesplugin.CreateObjectAsync("CADESCOM.CPAttribute");
                yield oSigningTimeAttr.propset_Name(CAPICOM_AUTHENTICATED_ATTRIBUTE_SIGNING_TIME);
                var oTimeNow = new Date();
                yield oSigningTimeAttr.propset_Value(oTimeNow);

                var oDocumentNameAttr = yield cadesplugin.CreateObjectAsync("CADESCOM.CPAttribute");
                yield oDocumentNameAttr.propset_Name(CADESCOM_AUTHENTICATED_ATTRIBUTE_DOCUMENT_NAME);
                yield oDocumentNameAttr.propset_Value(docTitle);

                var oSigner = yield cadesplugin.CreateObjectAsync("CAdESCOM.CPSigner");
                yield oSigner.propset_Certificate(oCertificate);
                yield oSigner.propset_CheckCertificate(true);

                var oAuthAttrs = yield oSigner.AuthenticatedAttributes2;
                yield oAuthAttrs.Add(oSigningTimeAttr);
                yield oAuthAttrs.Add(oDocumentNameAttr);

                var oSignedData = yield cadesplugin.CreateObjectAsync("CAdESCOM.CadesSignedData");
                yield oSignedData.propset_ContentEncoding(CADESCOM_BASE64_TO_BINARY);
                yield oSignedData.propset_Content(dataInBase64);

                var sSignedMessage = "";
                try {
                    sSignedMessage = yield oSignedData.SignCades(oSigner, CADESCOM_CADES_BES, true);
                } catch (err) {
                    e = cadesplugin.getLastError(err);
                    alert("Failed to create signature. Error: " + e);
                    return args[1](e);
                }

                yield oStore.Close();
                return args[0](sSignedMessage);
            }, resolve, reject); //cadesplugin.async_spawn
        });
    }

    function Verify(sSignedMessage, dataInBase64) {
        return new Promise(function (resolve, reject) {
            cadesplugin.async_spawn(function* (args) {
                var oSignedData = yield cadesplugin.CreateObjectAsync("CAdESCOM.CadesSignedData");
                try {
                    // Значение свойства ContentEncoding должно быть задано
                    // до заполнения свойства Content
                    yield oSignedData.propset_ContentEncoding(CADESCOM_BASE64_TO_BINARY);
                    yield oSignedData.propset_Content(dataInBase64);
                    yield oSignedData.VerifyCades(sSignedMessage, CADESCOM_CADES_BES, true);
                }
                catch (err) {
                    var e = cadesplugin.getLastError(err);
                    alert("Failed to verify signature. Error: " + e);
                    return args[1](e);
                }
                return args[0]();
            }, resolve, reject);
        });
    }

    function OrgNameFormat(raw) {
        if (typeof raw != "string") { return ""; }

        let len = raw.length;
        if (len === 0) { return ""; }

        let rawName = (raw[0] === '"' && raw[len - 1] === '"') ? raw.slice(1, -1) : raw;
        let orgName = rawName.replace(/\"\"/g, "\"");

        console.log("ORG NAME:\n ", raw, ";\n ", rawName, ";\n ", orgName);

        //let rawLen = rawName.length;
        //if (rawLen > 2) {
        //    // const pairOfQuotes = '""';
        //    // const openingQuotes = '«'; // https://unicode-table.com/ru/sets/quotation-marks/
        //    // const closingQuotes = '»'; // 
            
        //    //let first = orgName.indexOf('""');
        //    //let last = orgName.lastIndexOf('""');

        //    //if (first != last && first != -1 && last != -1) {
        //    //    orgName = rawName.substring(0, first) + rawName.substring(first + 2, last);            
        //    //}
        //}

        return orgName;
    }

    function GetCertByThumbprint(thumbprint) {
        if (!!azino.userCert && azino.userThumbprint === thumbprint) { return Promise.resolve(azino.userCert); }

        return GetArrayOfCertPromises().then(arr => {
            azino.userCert = arr.find(c => {
                return c.Thumbprint.then(t => t === thumbprint);
            });

            if (!!azino.userCert) { azino.userThumbprint = thumbprint; }

            return azino.userCert;
        })
    }

    function GetArrayOfCertPromises() {
        return GetStore().then(store => {
            return store.Open().then(() => {
                return store.Certificates.then(storeCerts => {
                    return storeCerts.Count.then(count => {
                        let promiseCerts = [];

                        for (let i = 1; i <= count; i++) {
                            promiseCerts[i - 1] = storeCerts.Item(i);
                        }

                        return Promise.all(promiseCerts).then(arr => {
                            store.Close();
                            return arr;
                        })
                    })
                })
            })
        }).catch(function () { return []; });
    }

    function CreateCert(a) {
        return Promise.all([
            a.IsValid(),
            a.HasPrivateKey(),
            a.PublicKey()
        ]).then(c => {
            return Promise.all([
                c[0].Result,
                c[1],
                c[2].Algorithm.then(alg => { return alg.FriendlyName; })
            ]).then(cc => {
                return {
                    "isValid": cc[0],
                    "hasPrivateKey": cc[1],
                    "algorithm": cc[2]
                };
            });
        }).catch(e => {
            return {
                "isValid": false,
                "hasPrivateKey": false,
                "algorithm": ""
            };
        })
    }

    function GetStore() {
        if (azino.store) { return Promise.resolve(azino.store); }

        if (!pluginAvailable) { return Promise.reject(""); }

        return cadesplugin.CreateObjectAsync("CAdESCOM.Store")
            .then(function (store) {
                azino.store = store;
                return store;
            });
    }

    function GetAbout() {
        if (azino.about) { return Promise.resolve(azino.about); }

        if (!pluginAvailable) { return Promise.reject(""); }

        return cadesplugin.CreateObjectAsync("CAdESCOM.About")
            .then(about => {
                azino.about = about;
                return about;
        });
    }

    function GetLicense() {
        if (azino.license) { return Promise.resolve(azino.license); }

        if (!pluginAvailable) { return Promise.reject(""); }

        return cadesplugin.CreateObjectAsync("CAdESCOM.CPLicense")
            .then(license => {
                azino.license = license;
                return license;
            });
    }

    function Extract(from, what) {
        let result = "";
        let begin = from.indexOf(what);
        let start = begin + what.length;

        if (what == "E=" && begin > 0) {
            begin = from.indexOf(" E=");
            start = begin + 3;
        }

        if (begin >= 0) {
            let end = from.indexOf(', ', begin);

            while (end > 0) {
                if (CheckQuotes(from.substr(start, end - start))) { break; }
                
                end = from.indexOf(', ', end + 1);
            }

            result = (end < 0) ? from.substr(start) : from.substr(start, end - start);
        }

        return result;
    }

    function CheckQuotes(str) {
        let result = 0;

        for (let i = 0; i < str.length; i++) {
            if (str[i] === '"') { result++; }
        }

        return !(result % 2);
    }

    //function GetCX509PrivateKey() {
    //    if (azino.cx509privateKey) { return Promise.resolve(azino.cx509privateKey); }

    //    if (!pluginAvailable) { return Promise.reject(""); }

    //    return cadesplugin.CreateObjectAsync("CAdESCOM.CX509PrivateKey")
    //        .then(cx509privateKey => {
    //            azino.cx509privateKey = cx509privateKey;
    //            return cx509privateKey;
    //        });
    //}
    //function ResolvePrivateKey(privateKey) {
    //    return privateKey.then(pk => {
    //        let providerName = pk.ProviderName
    //            .then(pn => { return pn; })
    //            .catch(e => { return e.message });

    //        let uniqueContainerName = pk.UniqueContainerName
    //            .then(ucn => { return ucn; })
    //            .catch(e => { return e.message });

    //        return Promise.all([
    //            providerName,
    //            uniqueContainerName
    //        ]).then(r => {
    //            return {
    //                "provider": r[0],
    //                "privateKeyLink": r[1]
    //            };
    //        });
    //    })
    //}
}());