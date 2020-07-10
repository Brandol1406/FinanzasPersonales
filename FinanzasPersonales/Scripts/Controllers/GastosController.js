'use strict';

let gastosTable = null;

$(document).ready(() => {
    fillOptions();
    $("#btnAgregar").click(() => newGasto());
    $("#btnGuardar").click(() => SaveGasto());
    $("#btnActualizar").click(() => UpdateGasto());
    LoadGastosTable();
});
function SaveGasto() {
    var data = getFormObject("#frmGasto");
    useAjax("/Gastos/Create", JSON.stringify(data), d => {
        if (d.Success) {
            clearForm("#frmGasto");
            $('#gastoModal').modal("hide");
            LoadGastosTable();
            toaster.success("Éxito", "Se ha registrado con éxito");
        }
        ManageModelErrors(d.Data);
    });
}
function UpdateGasto() {
    var data = getFormObject("#frmGasto");
    useAjax("/Gastos/Edit", JSON.stringify(data), d => {
        if (d.Success) {
            clearForm("#frmGasto");
            $('#gastoModal').modal("hide");
            LoadGastosTable();
            toaster.success("Éxito", "Se ha actualizado con éxito");
        }
        ManageModelErrors(d.Data);
    });
}
function fillOptions() {
    useAjax("/Categorias/GetAll", null, d => {
        fillDropDown("#categoria", JSON.parse(d), 'id_cat', 'Nombre');
    }, "GET");
}
function LoadGastosTable() {
    if (gastosTable !== null)
        gastosTable.destroy();

    let dateOptions = { year: 'numeric', month: 'numeric', day: 'numeric' };

    let columns =
        [
            { data: "categoriaNombre" },
            { data: "justificacion" },
            { data: "fecha", render: (d, t) => new Date(d).toLocaleDateString("es-DO", dateOptions) },
            { data: "valor" },
            {
                data: "id_gasto",
                render: function (data, type) {
                    return `<a class='btn btn-info' onclick='edit(${data}, this)' title="Editar"><span class='octicon octicon-pencil'></span></a>&nbsp;` +
                        `<a class='btn btn-danger' onclick='confirm(${data}, this)' title="Eliminar"><span class='octicon octicon-trashcan'></span></a>`;
                }
            }
        ];
    gastosTable = loadTable("#tGastos", "/Gastos/GetGastos", columns, [[2, "desc"]]);
}
function newGasto() {
    $("#btnGuardar").show();
    $("#btnActualizar").hide();
    clearForm("#frmGasto");
    $('#gastoModal').modal("show");
}
function edit(id, elem) {
    $("#btnGuardar").hide();
    $("#btnActualizar").show();
    clearForm("#frmGasto");
    useAjax("/Gastos/Get", "{id: " + id + "}", (d) => {
        bindObjectToForm("#frmGasto", d);
        $('#gastoModal').modal("show");
    });
}
function confirm(id) {
    $('#confirmModal').modal("show");
    document.querySelector("#btnEliminar").onclick = function () { destroy(id); };
}
function destroy(id) {
    useAjax("/Gastos/Delete", "{id: " + id + "}", (d) => {
        $('#confirmModal').modal("hide");
        LoadGastosTable();
        toaster.success("Éxito", "Se ha eliminado con éxito");
    });
}
