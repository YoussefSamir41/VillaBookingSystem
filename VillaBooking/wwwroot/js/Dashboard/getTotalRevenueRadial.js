$(document).ready(function () {
    loadRevenueRadialChart();
});

function loadRevenueRadialChart() {
    $(".chart-spinner").show(); // Show spinner

    $.ajax({
        url: "/Dashboard/GetRevenueChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            // Check if the response contains the expected data
            if (!data || !data.Series || typeof data.totalCount === "undefined") {
                console.error("Invalid data received:", data);
                document.querySelector("#sectionRevenueCount").innerHTML = "No revenue data available.";
                $(".chart-spinner").hide(); // Hide spinner
                return;
            }

            // Set the total revenue count
            document.querySelector("#spanTotalRevenueCount").innerHTML = data.totalCount;

            const sectionRevenueCount = document.querySelector("#sectionRevenueCount");
            sectionRevenueCount.innerHTML = ""; // Clear previous content

            // Determine if revenue increased or decreased
            const sectionCurrentCount = document.createElement("span");
            if (data.HasRatioIncrease) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML =
                    '<i class="bi bi-arrow-up-right-circle me-1"></i> <span>' + data.CurrentRevenue + '</span>';
            } else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML =
                    '<i class="bi bi-arrow-down-right-circle me-1"></i> <span>' + data.CurrentRevenue + '</span>';
            }

            // Append to the section
            sectionRevenueCount.append(sectionCurrentCount);
            sectionRevenueCount.append(" since last month");

            // Load the radial bar chart
            loadRadialBarChart("totalRevenueRadialChart", data.Series);

            $(".chart-spinner").hide(); // Hide spinner
        },
        error: function (xhr, status, error) {
            console.error("Error fetching revenue data:", error);
            document.querySelector("#sectionRevenueCount").innerHTML = "Failed to load revenue data.";
            $(".chart-spinner").hide(); // Hide spinner
        }
    });
}
