﻿var dtble;
$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtble = $("#mytable").DataTable({
        "ajax": {
            "url": "/Admin/Product/GetData",
        },

        "columns": [
            { "data": "name" },
            { "data": "description" },
            { "data": "price" },
            { "data": "category.name" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div style="display:flex;align-items:center">
                        <a href="/Admin/Product/Edit/${data}" class="btn btn-success">Edit</a>
                        <a onClick=DeleteItem("/Admin/Product/DeleteProduct/${data}") class="btn btn-danger">Delete</a>
                        </div>
                        `
                }
            }
        ]
    });
}

function DeleteItem(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        dtble.ajax.reload();
                        toastr["success"](data.message);
                    }
                    else {
                        toastr["error"](data.message);
                    }
                }
            });

            Swal.fire({
                title: "Deleted!",
                text: "Your file has been deleted.",
                icon: "success"
            });
        }
    });
}