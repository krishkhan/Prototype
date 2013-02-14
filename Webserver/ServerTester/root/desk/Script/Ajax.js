
var http_request = false;

function AjaxManager() {

    var delegate;
    var path;
    var endTransaction;


    this.SendRequest = function (callback, _path, async) {
        delegate = callback;
        path = _path;
        http_request.abort();
        var date = new Date();
        var token = date.getMinutes() + "_" + date.getMilliseconds();
        http_request.open('POST', path + "__" + token, true);
        if (http_request.onreadystatechange == null)
            http_request.onreadystatechange = this.OnStateChange;
        http_request.send(null);
    }

    this.OnStateChange = function () {
        if (http_request.readyState == 4) {
            if (http_request.status == 200) {
                delegate(http_request.responseText, path);
            } else {
                // alert('There was a problem with the request !!!');
            }
        }
    }
    if (window.XMLHttpRequest) { // Mozilla, Safari,...
        http_request = new XMLHttpRequest();
    } else if (window.ActiveXObject) {
        try {
            http_request = new ActiveXObject("Msxml2.XMLHTTP");
        } catch (e) {
            try {
                http_request = new ActiveXObject("Microsoft.XMLHTTP");
            } catch (e) {
            }
        }
    }

    if (http_request != false) {
        http_request.onreadystatechange = this.OnStateChange;
    }
    else {
        alert("error");
    }
}

var ajax = new AjaxManager();


function NormalizeString(msg) {

    msg = msg.replace("%20", " ");
    return msg;
}