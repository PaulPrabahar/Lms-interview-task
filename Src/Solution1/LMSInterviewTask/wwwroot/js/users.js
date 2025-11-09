$(init);
function init() {
    load();
    $("#btnAdd").on("click", () => $("#formBox").toggleClass("d-none"));
    $("#btnSave").on("click", save);
}
async function load() {
    const res = await apiGet("/users");
    const rows = res.user || [];   // 👈 extract array from object
    const $tb = $("#tbl tbody").empty();

    rows.forEach(r =>
        $tb.append(`<tr>
            <td>${r.empCode || ""}</td>
            <td>${r.fullName}</td>
            <td>${r.email}</td>
        </tr>`));
}
async function save() {
    const body = { empCode: $("#uCode").val(), fullName: $("#uName").val(), email: $("#uEmail").val() };
    await apiPost("/users", body);
    $("#formBox").addClass("d-none");
    await load();
}
