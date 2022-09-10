let userId = null;
let currentRoles, newRole = null;


function setUserId(id) {
    userId = id;
    
}

function deleteUser() {
    $.ajax({
        url: '/users/delete/' + userId,
        type: 'DELETE',
        dataType: 'json',
        success: function (response) {
           window.location.reload();
        },
        error: function () {}
    })
}

function getUserRoles(id,roles) {
    currentRoles = roles;
    $("#currentRoles").val(roles);
    userId = id;
}

function assignUser() {
    newRole = $('#roles').val();
    
    $.ajax({
        url: '/users/assign/' + userId + '?assign_role=' + newRole,
        type:"POST",
        dataType: 'json',
        success: function () {
            window.location.reload();
        }
    })
}

function revokeUserRole() {
    var 
}