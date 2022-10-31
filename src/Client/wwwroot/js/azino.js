; (function () {
    "use strict";

    if (window.azino) { return; }

    const { cadesplugin } = window;

    // App Constants
    const certTypes = CertTypes();

    // CryptoPro Constants
    const CP =
    {
        CADESCOM: CadesCom(),
        CAPICOM: CapiCom(),
        OBJ: Obj()
    };

    const _certs = [];
    const _user = {
        _cert: null,
        _thumbprint: ""
    }

    // CryptoAPI Objects
    const _ = {
        oAbout: null,
        oCerts: [],
        oStore: null
    };

    window.azino =
    {
        _store: {
            _certs: _certs,
            _user: _user
        },

        GetCryptoAbout: GetCryptoAbout,
        GetCertList: GetCertList,

        TokenCheck: TokenCheck,
        SignCadesBES: SignCadesBES,

        Console: Console,
        LinkOpen: (link) => window.open(link, "_blank", "comma,delimited,list,of,window,features"),

        SaveFile: SaveFile,
        Download: Download,
        PlayAudio: PlayAudio,
        ScrollToBottom: ScrollToBottom
    }

    // MAIN FUNCTION

    function GetCryptoAbout() {
        const jsModuleVersion = (cadesplugin && cadesplugin.JSModuleVersion) ? cadesplugin.JSModuleVersion : "";

        if (!jsModuleVersion) { return BadResponse("Не установлен КриптоПро ЭЦП Browser plug-in"); }

        const cryptoState = {
            cadesPluginApiVersion: jsModuleVersion,
            browserPluginVersion: "",
            cspVersion: "",
            cspName: ""
        };

        console.log()

        return (new Promise(function (resolve, reject) {
            cadesplugin.async_spawn(function* (args) {

                try {
                    if (!_.oAbout) { _.oAbout = yield cadesplugin.CreateObjectAsync(CP.OBJ.About) }
                    const oAbout = _.oAbout;

                    cryptoState.cspName = yield oAbout.CSPName(80);
                    cryptoState.cspVersion = yield (yield oAbout.CSPVersion("", 80)).toString();
                    cryptoState.browserPluginVersion = yield (yield oAbout.PluginVersion).toString();
                } catch (e) {
                    return args[1]("Ошибка чтения данных о CSP")
                }

                console.log("About Crypto CSP :", cryptoState);

                return args[0](cryptoState);
            }, resolve, reject);
        })).then(r => { return Ok(r); }).catch(e => { return BadResponse(e); });
    }
    function GetCertList(resetCache = false) {
        if (!resetCache && _certs && _certs.length > 0) { return Ok(_certs); }

        _certs.length = 0;
        _.oCerts.length = 0;

        return (new Promise(function (resolve, reject) {
            cadesplugin.async_spawn(function* (args) {
                let oStore;

                try {
                    if (!_.oStore) {
                        _.oStore = yield cadesplugin.CreateObjectAsync(CP.OBJ.Store);
                        if (!_.oStore) { return args[1]("Хранилище сертификатов не найдено"); }
                    }
                    oStore = _.oStore;

                    yield oStore.Open();
                } catch (ex) { return args[1]("Ошибка открытия хранилища сертификатов"); }

                let oStoreCerts;
                let oStoreCertsCount;

                try {
                    oStoreCerts = yield oStore.Certificates;
                    oStoreCertsCount = yield oStoreCerts.Count;
                } catch (ex) { return args[1]("Ошибка чтения сертификатов из хранилища"); }

                for (let i = 1; i <= oStoreCertsCount; i++) {
                    let oCert;
                    let cert = NewCert();

                    try {
                        oCert = yield oStoreCerts.Item(i);
                    } catch (ex) {
                        //return args[1]("Ошибка получения сертификата из хранилища");
                        continue;
                    }

                    try {
                        cert.ix = i - 1;
                        cert.fromContainer = false;

                        let pk = yield oCert.PublicKey();
                        let alg = yield pk.Algorithm;

                        cert.publicKey = {
                            length: yield pk.Length,
                            key: yield (yield pk.EncodedKey).Value(),
                            parameters: yield (yield pk.EncodedParameters).Value(),
                            algorithm: yield alg.FriendlyName,
                            oid: yield alg.Value
                        }

                        FillIssuer(cert.issuer, yield oCert.IssuerName);
                        FillSubject(cert.subject, yield oCert.SubjectName);
                        cert.type = (cert.subject.innLe) ? certTypes.company : (cert.subject.org) ? certTypes.organization : certTypes.personal;

                        cert.serialNumber = yield oCert.SerialNumber;
                        cert.thumbprint = yield oCert.Thumbprint;

                        cert.fromDate = yield oCert.ValidFromDate;
                        cert.tillDate = yield oCert.ValidToDate;
                        cert.version = yield oCert.Version;

                        cert.hasPrivateKey = yield oCert.HasPrivateKey();
                        cert.isValid = yield (yield oCert.IsValid()).Result;
                        cert.isCorrect = (!!cert.hasPrivateKey && !!cert.isValid && !!cert.subject.snils && !!cert.thumbprint);

                        cert.hasError = false;
                    } catch (ex) {
                        //return args[1]("Ошибка получения свойств сертификата из хранилища");
                        cert.hasError = true;
                    }

                    _certs.push(cert);
                    _.oCerts.push(oCert);
                }

                yield oStore.Close();
                return args[0](_certs);
            }, resolve, reject);
        })).then(r => { return Ok(r); }).catch(e => { return BadResponse(e); });
    }

    function TokenCheck(thumbprint, ix = 0) {
        return (new Promise(function (resolve, reject) {
            cadesplugin.async_spawn(function* (args) {
                let oCertificate = null;

                if (_.oCerts) {
                    if (!ix) {
                        for (let i = 0; i < _certs.length; i++) {
                            if (_certs[i].Thumbprint === thumbprint) {
                                ix = i;
                                break;
                            }
                        }
                    }

                    if (ix) { oCertificate = _.oCerts[ix]; }
                }

                let oStore;

                if (!oCertificate) {

                    try {
                        if (!_.oStore) {
                            _.oStore = yield cadesplugin.CreateObjectAsync(CP.OBJ.Store);
                            if (!_.oStore) { return args[1]("Хранилище сертификатов не найдено"); }
                        }

                        oStore = _.oStore;

                        yield oStore.Open(); //CP.CAPICOM.CURRENT_USER_STORE, CP.CAPICOM.MY_STORE, CP.CAPICOM.STORE_OPEN_MAXIMUM_ALLOWED
                    } catch (ex) { return args[1]("Ошибка открытия хранилища сертификатов"); }

                    let Certificates = yield oStore.Certificates;
                    let oStoreCertsCount = yield Certificates.Count;

                    for (var i = 1; i <= oStoreCertsCount; i++) {
                        let cert;

                        try { cert = yield Certificates.Item(i); }
                        catch (ex) {
                            return args[1](("Ошибка при получении сертификата: " + cadesplugin.getLastError(ex)));
                        }

                        try {
                            var ctp = yield cert.Thumbprint;
                            if (thumbprint === ctp) {
                                oCertificate = cert;
                                break;
                            }
                        } catch (ex) {
                            return args[1]("Ошибка при получении отпечатка: " + cadesplugin.getLastError(ex));
                        }
                    }
                }

                console.log("SignIn Cert: ", oCertificate);
                var tokenСonnected = false;

                if (oCertificate !== null) {
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

                if (oStore) { yield oStore.Close(); }
                return args[0](tokenСonnected);
            }, resolve, reject); //cadesplugin.async_spawn
        })).then(r => { return Ok(r); }).catch(e => { return BadResponse(e); });
    }
    function SignCadesBES(thumbprint, dataInBase64) {
        return SignCreate(thumbprint, dataInBase64)
            .then(signedMessage => { return Ok(signedMessage); })
            .catch(e => { return BadResponse(e); });
    };

    function SignCreate(thumbprint, dataInBase64, docTitle = "") {
        return new Promise(function (resolve, reject) {
            cadesplugin.async_spawn(function* (args) {
                var StartTime = Date.now();

                let oStore;

                try {
                    if (!_.oStore) { _.oStore = yield cadesplugin.CreateObjectAsync(CP.OBJ.Store); }
                    oStore = _.oStore;

                    if (!oStore) { return args[1]("Хранилище сертификатов не найдено"); }

                    yield oStore.Open(CP.CAPICOM.CURRENT_USER_STORE, CP.CAPICOM.MY_STORE, CP.CAPICOM.STORE_OPEN_MAXIMUM_ALLOWED);
                } catch (ex) { return args[1]("Ошибка открытия хранилища сертификатов"); }

                let Certificates = yield oStore.Certificates;
                let certCnt = yield Certificates.Count;
                let oCertificate;

                for (var i = 1; i <= certCnt; i++) {
                    let cert;

                    try { cert = yield Certificates.Item(i); }
                    catch (ex) {
                        alert("Ошибка при получении сертификата: ", i, cadesplugin.getLastError(ex));
                        continue;
                    }

                    try {
                        var ctp = yield cert.Thumbprint;
                        if (thumbprint === ctp) {
                            oCertificate = cert;
                            break;
                        }
                    } catch (ex) {
                        alert("Ошибка при получении отпечатка: ", i, cadesplugin.getLastError(ex));
                    }
                }

                var RunTime = Date.now();
                var oTimeNow = new Date();

                var oSigningTimeAttr = yield cadesplugin.CreateObjectAsync(CP.OBJ.Attribute);
                yield oSigningTimeAttr.propset_Name(CP.CAPICOM.AUTHENTICATED_ATTRIBUTE_SIGNING_TIME);
                yield oSigningTimeAttr.propset_Value(oTimeNow);

                var oDocumentNameAttr = yield cadesplugin.CreateObjectAsync(CP.OBJ.Attribute);
                yield oDocumentNameAttr.propset_Name(CP.CADESCOM.AUTHENTICATED_ATTRIBUTE_DOCUMENT_NAME);
                yield oDocumentNameAttr.propset_Value(docTitle);

                var oSigner = yield cadesplugin.CreateObjectAsync(CP.OBJ.Signer);
                yield oSigner.propset_Certificate(oCertificate);
                yield oSigner.propset_CheckCertificate(true);

                var oAuthAttrs = yield oSigner.AuthenticatedAttributes2;
                yield oAuthAttrs.Add(oSigningTimeAttr);
                yield oAuthAttrs.Add(oDocumentNameAttr);

                var oSignedData = yield cadesplugin.CreateObjectAsync(CP.OBJ.SignedData);
                yield oSignedData.propset_ContentEncoding(CP.CADESCOM.BASE64_TO_BINARY);
                yield oSignedData.propset_Content(dataInBase64);

                var sSignedMessage = "";
                try {
                    sSignedMessage = yield oSignedData.SignCades(oSigner, CP.CADESCOM.CADES_BES, true);
                } catch (err) {
                    let mes = cadesplugin.getLastError(err);
                    alert("Failed to create signature. Error: " + mes);
                    return args[1](mes);
                }

                yield oStore.Close();

                var EndTime = Date.now();

                console.warn("Время выполнения: " + (EndTime - StartTime) + " мс : " + (RunTime - StartTime));

                return args[0](sSignedMessage);
            }, resolve, reject);
        });
    }
    function Verify(sSignedMessage, dataInBase64) {
        return new Promise(function (resolve, reject) {
            cadesplugin.async_spawn(function* (args) {
                var oSignedData = yield cadesplugin.CreateObjectAsync(CP.OBJ.SignedData);
                try {
                    // Значение свойства ContentEncoding должно быть задано
                    // до заполнения свойства Content
                    yield oSignedData.propset_ContentEncoding(CP.CADESCOM.BASE64_TO_BINARY);
                    yield oSignedData.propset_Content(dataInBase64);
                    yield oSignedData.VerifyCades(sSignedMessage, CP.CADESCOM.CADES_BES, true);
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

    // HELPERS

    function Ok(data) {
        return {
            succeed: true,
            data: data,
            message: ""
        }
    }
    function BadResponse(message) {
        return {
            succeed: false,
            data: null,
            message: message || "Ошибка"
        };
    }
    function Console(value, tag = "|> ", type = "info") {
        var log = (type === "info") ? console.info :
            (type === "error") ? console.error :
                (type === "debug") ? console.debug :
                    (type === "warn") ? console.warn : console.log;

        log(tag, value)
    }

    function FillIssuer(issuer, name) {
        let a = SplitCertName(name);

        issuer.name = a.CN || "";
        issuer.inn = a.INN || "";
        issuer.ogrn = a.OGRN || "";
        issuer.eMail = a.E || "";
    }
    function FillSubject(subject, name) {
        let a = SplitCertName(name);

        subject.surname = a.SN || "";
        subject.givenName = a.G || "";
        subject.title = a.T || "";
        subject.inn = a.INN || a["ИНН"] || a["OID.1.2.643.3.131.1.1"] || "";
        subject.snils = a.SNILS || a["СНИЛС"] || a["OID.1.2.643.100.3"] || "";

        subject.org = (a.O) ? a.O : "";
        subject.ogrn = a.OGRN || a["ОГРН"] || a["OID.1.2.643.100.1"] || "";
        subject.innLe = a.INNLE || a["ИНН ЮЛ"] || a["OID.1.2.643.100.4"] || "";

        subject.name = a.CN || "";
        subject.eMail = a.E || "";

        subject.street = "";
        subject.locality = a.L || "";
        subject.state = a.S || "";
        subject.country = a.C || "";
    }
    function SplitCertName(str) {
        let s = str.split(', ');
        let a = {};

        for (let i = 0; i < s.length; i++) {
            let k = s[i].split('=');

            if (k[0]) {
                a[k[0]] = (k[1]) ? k[1].replace(/(^")|("$)/g, '').replace(/""/g, "\"").trim() : "";
            }
        }
        return a;
    }

    function SaveFile(filename, contentType, content) {
        console.log("File: ", filename, "\n", content);

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
    function Download(options) {
        var fileUrl = "data:" + options.mimeType + ";base64," + options.byteArray;

        fetch(fileUrl)
            .then(response => response.blob())
            .then(blob => {
                var link = window.document.createElement("a");
                link.href = window.URL.createObjectURL(blob, { type: options.mimeType });
                link.download = options.fileName;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            });
    }
    function PlayAudio(elementName) {
        document.getElementById(elementName).play();
    }
    function ScrollToBottom(elementName) {
        let element = document.getElementById(elementName);
        element.scrollTop = element.scrollHeight - element.clientHeight;
    }

    // CONSTANTS

    function CertTypes() {
        return Object.freeze({
            undefined: 0,
            personal: 1,
            organization: 2,
            company: 3
        });
    }
    function CadesCom() {
        return {
            AUTHENTICATED_ATTRIBUTE_DOCUMENT_NAME: 1,
            BASE64_TO_BINARY: 1,
            CADES_BES: 1
        }
    }
    function CapiCom() {
        return {
            AUTHENTICATED_ATTRIBUTE_SIGNING_TIME: 0,
            CURRENT_USER_STORE: 2,
            MY_STORE: "My",
            STORE_OPEN_MAXIMUM_ALLOWED: 2
        }
    }
    function Obj() {
        return {
            About: "CAdESCOM.About",
            Attribute: "CADESCOM.CPAttribute",
            SignedData: "CAdESCOM.CadesSignedData",
            Signer: "CAdESCOM.CPSigner",
            Store: "CAdESCOM.Store",
        };
    }

    // FACTORIES

    function NewCert() {
        const cert = {
            ix: 0,
            fromContainer: false,
            hasError: false,

            thumbprint: "",
            serialNumber: "",
            type: 0,

            fromDate: null,
            tillDate: null,
            version: 0,

            provider: "",
            privateKeyLink: "",
            hasPrivateKey: false,

            isValid: false,
            isCorrect: false,

            publicKey: {
                algorithm: "",
                oid: "",
                key: "",
                parameters: "",
                length: 0
            },
            subject: {
                surname: "",
                givenName: "",
                title: "",
                inn: "",
                snils: "",

                org: "",
                ogrn: "",
                innLe: "",

                name: "",
                eMail: "",

                street: "",
                locality: "",
                state: "",
                country: ""
            },
            issuer: {
                name: "",      // CN
                inn: "",       // INN
                ogrn: "",      // OGRN
                eMail: ""      // E
            }
        };

        return cert;
    }
}());
