const API_BASE = "";

function apiGet(url, qs = {}) {
    return $.getJSON(API_BASE + url, qs);
}

function apiPost(url, body) {
    return $.ajax({
        url: API_BASE + url,
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(body)
    });
}

function msg(text) { console.log(text); } // swap with toasts later