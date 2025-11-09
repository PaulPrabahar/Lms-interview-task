$(init);
function init() {
    load();
    $("#btnAdd").on("click", () => $("#formBox").toggleClass("d-none"));
    $("#btnSave").on("click", save);
}
async function load() {
    const res = await apiGet("/period");  // get full response object
    const rows = res.user || [];           // unwrap "user" array

    const $tb = $("#tbl tbody").empty();
    rows.forEach(r => {
        const start = (r.startDate || "").substring(0, 10);
        const end = (r.endDate || "").substring(0, 10);
        $tb.append(`<tr><td>${r.name}</td><td>${start}</td><td>${end}</td></tr>`);
    });
}
async function save() {
    const body = {
        name: $("#pName").val(),
        startDate: new Date($("#pStart").val()).toISOString(),
        endDate: new Date($("#pEnd").val()).toISOString()
    };
    await apiPost("/period", body);
    $("#formBox").addClass("d-none");
    await load();
}
