var dataTable;

$(function () {
    var url = window.location.search.toLowerCase();
    if (url.includes("approved")) {
        loadDataTable("approved");
    } else if (url.includes("pending")) {
        loadDataTable("pending");
    } else if (url.includes("readyforpickup")) {
        loadDataTable("readyforpickup");
    }
    else if (url.includes("completed")) {
        loadDataTable("completed");
    } else if (url.includes("cancelled")) {
        loadDataTable("cancelled");
    } else {
        loadDataTable("all");
    }

});
function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        order: [[0, "desc"]],
        //"ajax": {
        //    "url": "/Order/GetAll?status=" + status,
        //    "beforeSend": function (xhr) {
        //        const token = getJwtFromCookie();
        //        console.log("JWTToken from cookie:", token);
        //        if (token) {
        //            xhr.setRequestHeader("Authorization", "Bearer " + token);
        //        }
        //    }
        //},
        ajax: function (data, callback, settings) {
            const status = new URLSearchParams(window.location.search).get("status") || "all";
            const token = getJwtFromCookie();
            console.log("Calling /Order/GetAll with status:", status);
            console.log("JWT Token:", token);

            $.ajax({
                url: "/Order/GetAll?status=" + status,
                type: "GET",
                headers: {
                    "Authorization": "Bearer " + token

                },
                success: function (response) {
                    console.log("Sending Authorization Header:", token);

                    callback(response); // Load data into DataTable
                },
                error: function (xhr, status, error) {
                    console.error("DataTable load failed:", error);
                }
            });
        },
        "columns": [
            { "data": "orderHeaderId", "width": "5%" },
            { "data": "name", "width": "20%" },
            { "data": "email", "width": "25%" },
            { "data": "phone", "width": "15%" },
            { "data": "orderTime", "width": "15%" },
            { "data": "orderTotal", "width": "10%" },
            { "data": "status", "width": "10%" },
            {
                "data": "orderHeaderId",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                                <a href="/Order/OrderDetail?orderId=${data}" class="btn btn-primary mx-2">
                                    <i class="bi bi-pencil-square"></i>
                                </a>
                            </div>`;
                }, "width": "10%"

            }

        ]
    });

}

//function getJwtFromCookie(cookieName = "JWTToken") {
//    const cookies = document.cookie.split(';');
//    for (let cookie of cookies) {
//        const [name, value] = cookie.trim().split('=');
//        if (name === cookieName) {
//            return decodeURIComponent(value);
//        }
//    }
//    return null;
//}

function getJwtFromCookie(cookieName = "JWTToken") {
    const match = document.cookie.match(new RegExp('(^| )' + cookieName + '=([^;]+)'));
    return match ? decodeURIComponent(match[2]) : null;
}
