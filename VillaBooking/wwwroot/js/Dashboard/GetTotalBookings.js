$(document).ready(function () {
    loadTotalBookingRadialChart();
});

function loadTotalBookingRadialChart() {
    $(".chart-spinner").show();

    $.ajax({
        url: "/Dashboard/GetTotalBookingRadialChartData", 
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            // Update total count
            document.querySelector("#spanTotalBookingCount").innerHTML = 6;

            // Update section for the count in the current month (adjusted field)
            var sectionCurrentCount = document.createElement("span");
            if (data.hasRatioIncrease) {  // Adjusted from data.hasRatioIncreased
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i> <span> ' + data.increasDeacreasAmount + '</span>'; // Adjusted field name
            }
            else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i> <span> ' + data.increasDeacreasAmount + '</span>'; // Adjusted field name
            }

            // Append current count and "since last month"
            document.querySelector("#sectionBookingCount").append(sectionCurrentCount);
            document.querySelector("#sectionBookingCount").append(" since last month");

            // Load the radial bar chart (ensure you have this function defined)
            loadRadialBarChart("totalBookingsRadialChart", data);

            $(".chart-spinner").hide();
        },
        error: function (xhr, status, error) {
            console.error("AJAX error: " + status + ": " + error);
            $(".chart-spinner").hide();
        }
    });
}