'use strict';

$(document).ready(() => {
    initDates();
    fillOptions();
    $("#btnProcesar").click(() => procesar());
});
function procesar() {
    let data = getFormObject("#frmParams");
    let tbody = $("#tGastos").find("tbody");
    tbody.html(`<tr><td><strong>Cargando...</strong></td><td></td><td></td><td><div class="spinner-border ml-auto" role="status" aria-hidden="true"></div></td></tr>`);
    useAjax("/Gastos/GetGastos", JSON.stringify(data), d => {
        let result = JSON.parse(d);
        $("#total").html(result.total.toLocaleString("es-DO"));
        $("#numero").html(result.numero);
        tbody.html('');
        if (result.list.length > 0) {
            result.list.forEach((g, i) => {
                tbody.append(`<tr><td>${g.categoriaNombre}</td><td>${g.justificacion}</td><td>${new Date(g.fecha).toLocaleDateString("es-DO")}</td><td>${g.valor}</td></tr>`);
            });
        }
        else {
            tbody.append(`<tr><td>${'No hay datos para mostrar'}</td><td></td><td></td><td></td></tr>`);
        }
    });
}
function fillOptions() {
    useAjax("/Gastos/GetCategorias", null, d => {
        fillDropDown("#categoria", d, 'id_cat', 'Nombre');
    });
}
function initDates() {
    var date = new Date();
    var date2 = new Date();
    date2.setMonth((new Date()).getMonth() - 1);
    var day = ("0" + date.getDate()).slice(-2);
    var month = ("0" + (date.getMonth() + 1)).slice(-2);

    var day2 = ("0" + date2.getDate()).slice(-2);
    var month2 = ("0" + (date2.getMonth() + 1)).slice(-2);
    
    var desde = document.querySelector('#desde');
    desde.value = date2.getFullYear() + "-" + (month2) + "-" + (day2);
    var hasta = document.querySelector('#hasta');
    hasta.value = date.getFullYear() + "-" + (month) + "-" + (day);
}

