'use strict';
let GastosList = [];

$(document).ready(() => {
    $("#presSelector").change(() => { loadResumenCuentas(); loadCharts(); });
    fillOptions();
    setCurrentPres();
    loadResumenCuentas();
    loadCharts();
});
function fillOptions() {
    useAjax("/Presupuestos/GetAll", null, d => {
        fillDropDown("#presSelector", JSON.parse(d), 'id_pres', 'Representacion');
    }, "GET", false);
}
function setCurrentPres() {
    useAjax("/Presupuestos/GetCurrent", null, d => {
        if (d.Success) {
            $("#presSelector").val(d.Data.id_pres);
        } else {
            toaster.warning("Advertencia", "No hay un presupuesto activo. Los presupuestos creados no estan entre la fecha actual", false);
        }
    }, "POST", false);
}
function loadResumenCuentas() {
    var params = {
        id_pres: Number($("#presSelector").val())
    };
    useAjax("/Home/GetResumenCuentas", JSON.stringify(params), RenderBars, "POST", true);
}
//Render bars
function RenderBars (d) {
    if (d.Success) {
        if (d.Data.List.length > 0) {
            var content = "";
            $("#tP").html(d.Data.TotalLimites.toLocaleString('es-DO'));
            $("#tG").html(d.Data.TotalGastado.toLocaleString('es-DO'));
            let tg = d.Data.TotalLimites - d.Data.TotalGastado;
            $("#tD").html(tg.toLocaleString('es-DO'));

            GastosList = [];
            GastosList.push(['Task', 'Hours per Day']);
            d.Data.List.forEach((e, i) => {
                let barTemplate = `<div class='border border-light bg-light' style='margin-bottom:8px; padding:4px;'>
                                            <span><strong>${e.Categoria}</strong><span>
                                            <div class="progress" style="height: 40px;">
                                                <div class="progress-bar ${GetBgOfPercent(e.Porcentaje)}" role="progressbar" style="width: ${e.PorcentajeString}%;" aria-valuenow="25" aria-valuemin="0" aria-valuemax="100">${e.PorcentajeString}%</div>
                                            </div>
                                            <span>Límite: ${e.Limite}, Gastado: ${e.Gastado}, Disponible: ${(e.Limite - e.Gastado)}<span>
                                        </div>`;
                content += barTemplate;
                GastosList.push([e.Categoria, e.Gastado]);
            });
            $("#resumenBody").html(content);
        }
        else {
            $("#resumenBody").html("<span>No hay datos para mostrar</span>");
            $("#piechart").html("<span>No hay datos para mostrar</span>");
            $("#tP").html(0);
            $("#tG").html(0);
            $("#tD").html(0);
        }
    }
}
function GetBgOfPercent(p) {
    if (p >= 0 && p < 26)
        return "bg-success";
    if (p >= 26 && p < 51)
        return "bg-info";
    if (p >= 51 && p < 76)
        return "bg-warning";
    if (p >= 76 && p < 1000)
        return "bg-danger";
}
// Load google charts
function loadCharts() {
    google.charts.load('current', { 'packages': ['corechart'] });
    google.charts.setOnLoadCallback(drawChart);
}
// Draw the chart and set the chart values
function drawChart() {
    var data = google.visualization.arrayToDataTable(GastosList);

    // Optional; add a title and set the width and height of the chart
    var options = { 'title': 'Gastado', 'width': 450, 'height': 400 };

    // Display the chart inside the <div> element with id="piechart"
    var chart = new google.visualization.PieChart(document.getElementById('piechart'));
    chart.draw(data, options);
}