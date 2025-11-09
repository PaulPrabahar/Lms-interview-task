let types = [];
$(init);
async function init() {
    await loadUsers("#ddlUser");
    await loadPeriods("#ddlPeriod");

    const res = await apiGet("/leavetypes");    // be tolerant here too
    types = Array.isArray(res) ? res : (res.leaveTypes || res.types || res.user || []);

    $("#btnLoad").on("click", loadAllocations);
}

async function loadUsers(sel) {
    const res = await apiGet("/users");          // returns { user: [...] }
    const rows = Array.isArray(res) ? res : (res.user || res.users || []);
    const $s = $(sel).empty().append(`<option value="">-- User --</option>`);
    rows.forEach(u => $s.append(`<option value="${u.id}">${u.fullName}</option>`));
}
async function loadPeriods(sel) {
    const res = await apiGet("/period");         // returns { user: [...] } per your Postman
    const rows = Array.isArray(res) ? res : (res.user || res.periods || res.period || []);
    const $s = $(sel).empty().append(`<option value="">-- Period --</option>`);
    rows.forEach(p => $s.append(`<option value="${p.id}">${p.name}</option>`));
}
async function loadAllocations() {
    const userId = +$("#ddlUser").val(), periodId = +$("#ddlPeriod").val();
    if (!userId || !periodId) { msg("Pick user & period"); return; }
    const allocs = await apiGet("/allocations", { userId, periodId });
    const map = {}; allocs.userLeave?.forEach(a => map[a.typeId] = a.daysAllocated);

    const $tb = $("#tbl tbody").empty();
    types.forEach(t => {
        const val = map[t.id] ?? 0;
        $tb.append(`<tr data-type="${t.id}">
      <td>${t.code}</td><td>${t.name}</td>
      <td><input class="form-control form-control-sm inp" type="number" min="0" step="0.5" value="${val}"/></td>
      <td><button class="btn btn-sm btn-primary btn-save">Save</button></td>
    </tr>`);
    });

    $("#tbl").off("click", ".btn-save").on("click", ".btn-save", async function () {
        const $tr = $(this).closest("tr");
        const typeId = +$tr.data("type");
        const daysAllocated = +($tr.find(".inp").val() || 0);
        await apiPost("/allocations", { userId, periodId, typeId, daysAllocated });
        msg("Saved");
    });
}
