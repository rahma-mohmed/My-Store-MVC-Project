var dtble;
$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtble = $("#mytable").DataTable({
        "ajax": {
            "url": "/Admin/Order/GetData",
        },

        "columns": [
            { "data": "id" },
            { "data": "applicationUser.name" },
            { "data": "applicationUser.phoneNumber" },
            { "data": "applicationUser.email" },
            { "data": "orderStatus" },
            { "data": "totalPrice" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a href="/Admin/Order/Details?orderid=${data}" class="btn btn-warning">Details</a>                        `
                }
            }
        ]
    });
}
