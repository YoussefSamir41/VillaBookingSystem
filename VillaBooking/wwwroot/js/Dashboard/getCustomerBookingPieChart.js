var staticData = {
    series: [30, 70],
    labels: ["New Customer Bookings", "Returning Customer Bookings"]
};

//function loadCustomerBookingPieChart() {
//    $(".chart-spinner").show();

//    $.ajax({
//        url: "/Dashboard/GetBookingPieChartData",
//        type: 'GET',
//        dataType: 'json',
//        success: function (data) { // Change from staticData to data
//            if (data && data.series && data.labels) {
//                loadPieChart("customerBookingsPieChart", data); // Use the correct variable 'data'
//            } else {
//                console.error("Invalid data received: ", data); // Log 'data' here
//                $(".chart-spinner").hide();
//            }
//        },
//        error: function (xhr, status, error) {
//            console.error("Error fetching data: ", error);
//            $(".chart-spinner").hide();
//        }
//    });
//}

function loadPieChart(id, staticData) {
    // Check if 'data' is not null or undefined and if 'series' and 'labels' exist and are arrays
    if (staticData && Array.isArray(staticData.series) && Array.isArray(staticData.labels)) {
        var chartColors = getChartColorsArray(id);
        var options = {
            colors: chartColors,
            series: staticData.series,  // Ensure these exist in 'data'
            labels: staticData.labels,  // Ensure these exist in 'data'
            chart: {
                width: 380,
                type: 'pie',
            },
            stroke: {
                show: false
            },
            legend: {
                position: 'bottom',
                horizontalAlign: 'center',
                labels: {
                    colors: "#fff",
                    useSeriesColors: true
                },
            },
        };

        // Create the chart only if the data is valid
        var chart = new ApexCharts(document.querySelector("#" + id), options);
        chart.render();
    } else {
        // Log an error if the data is not valid
        console.error("Invalid data received: ", staticData);
    }
}
