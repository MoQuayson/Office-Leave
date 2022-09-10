let leaveId = null;
function setLeaveId(id) {
    leaveId = id;
}

function deleteLeaveApplication() {
    $.ajax({
        url: "/leave-applications/delete/" + leaveId,
        type: "DELETE",
        dataType: "json",
        success: function (response) {
            window.location.reload();
        },
        error: function (err) {
            console.log(err)
        }
    });
}