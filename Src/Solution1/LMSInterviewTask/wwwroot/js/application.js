let types = [];

$(init);

async function init() {
    try {
        await loadUsers("#u");
        await loadPeriods("#p");

        // ✅ backend: GET /leavetypes  → { leaveTypes: [...] }
        const resTypes = await apiGet("/leavetypes");
        types = Array.isArray(resTypes) ? resTypes : (resTypes.leaveTypes || []);
        const $t = $("#t").empty();
        types.forEach(x => $t.append(`<option value="${x.id}">${x.code} - ${x.name}</option>`));

        $("#avail").on("click", showAvail);
        $("#apply").on("click", apply);
        $("#u,#p").on("change", loadApps);

        // optional: auto-load table if both selects have a value
        await loadApps();
    } catch (e) {
        console.error("Init error:", e);
    }
}

async function loadUsers(sel) {
    // ✅ backend: GET /users → { user: [...] }
    const res = await apiGet("/users");
    const rows = Array.isArray(res) ? res : (res.user || []);
    const $s = $(sel).empty().append(`<option value="">-- User --</option>`);
    rows.forEach(u => $s.append(`<option value="${u.id}">${u.fullName}</option>`));
}

async function loadPeriods(sel) {
    // ✅ backend: GET /period (singular) → { user: [...] }
    const res = await apiGet("/period");
    const rows = Array.isArray(res) ? res : (res.user || []);
    const $s = $(sel).empty().append(`<option value="">-- Period --</option>`);
    rows.forEach(p => $s.append(`<option value="${p.id}">${p.name}</option>`));
}

async function showAvail() {
    const userId = +$("#u").val(), periodId = +$("#p").val(), typeId = +$("#t").val();
    if (!userId || !periodId || !typeId) { msg("Pick user/period/type"); return; }

    // ✅ backend: GET /applications/available-balance?userId=&periodId=&typeId=
    const res = await apiGet("/balanceleave", { userId, periodId, typeId });
    const b = res.balances?.[0];
    $("#info")
        .toggleClass("d-none", !b)
        .text(b ? `${b.typeCode}: Alloc ${b.allocatedDays}, Used ${b.usedDays}, Avail ${b.availableDays}` : "No allocation");
}

async function apply() {
    const userId = +$("#u").val(), periodId = +$("#p").val(), typeId = +$("#t").val(), daysRequested = +$("#d").val();
    if (!userId || !periodId || !typeId || !daysRequested) { msg("Fill all"); return; }

    // ✅ backend: POST /applications  with { userId, periodId, typeId, daysRequested }
    await apiPost("/leaveapplication", { userId, periodId, typeId, daysRequested });
    $("#d").val(""); $("#info").addClass("d-none");
    await loadApps();
}

async function loadApps() {
    const userId = +$("#u").val(), periodId = +$("#p").val();
    if (!userId || !periodId) return;

    // ✅ backend: GET /applications?userId=&periodId= → { leaveApplication: [...] }
    const res = await apiGet("/leaveapplication", { userId, periodId });
    const rows = res.leaveApplication || [];
    const $tb = $("#tbl tbody").empty();

    rows.forEach(a => {
        const t = types.find(x => x.id === a.typeId);
        $tb.append(`<tr>
      <td>${t?.code || a.typeId}</td>
      <td>${a.daysRequested}</td>
      <td>${a.status}</td>
      <td>${a.decisionNote || ""}</td>
      <td>${(a.createdAt || "").toString().substring(0, 19)}</td>
    </tr>`);
    });
}