var dataTable;

$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get('status');
    loadDataTable(status);
});

function loadDataTable(status) {
    dataTable = $('#tblBookings').DataTable({
        "ajax": {
            url: `/booking/getall${status ? '?status=' + status : ''}`,
            type: 'GET',
            dataSrc: 'data',  // Matches the 'data' key in the API response
            error: function (xhr, error, thrown) {
                console.error("Error fetching data:", error);
                console.error("Response text:", xhr.responseText);
                alert("Failed to load data. Check console for details.");
            }
        },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'phone', "width": "10%" },
            { data: 'email', "width": "15%" },
            { data: 'status', "width": "10%" },
            {
                data: 'checkInDate', "width": "10%", render: function (data) {
                    // Format checkInDate to a readable format
                    return data ? new Date(data).toLocaleDateString() : '';
                }
            },
            { data: 'nights', "width": "10%" },
            {
                data: 'totalCost',
                render: function (data) {
                    return data != null ? $.fn.dataTable.render.number(',', '.', 2).display(data) : '0.00';
                },
                "width": "10%"
            },
            {
                data: 'id',
                render: function (data) {
                    return `<a href="/booking/BookingDetails?bookingId=${data}" class="btn btn-outline-warning mx-2">
                        <i class="bi bi-pencil-square"></i> Details
                    </a>`;
                }
            }
        ]
    });
}
