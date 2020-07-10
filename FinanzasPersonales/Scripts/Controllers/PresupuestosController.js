'use strict';

let dataTable = null;

$(document).ready(() => {
    $("#btnAgregar").click(() => newItem());
    $("#btnGuardar").click(() => Save());
    $("#btnActualizar").click(() => Update());
    LoaddataTable();
});
function Save() {
    var data = getFormObject("#frmModel");
    useAjax("/Presupuestos/Create", JSON.stringify(data), d => {
        if (d.Success) {
            clearForm("#frmModel");
            $('#modelModal').modal("hide");
            LoaddataTable();
            toaster.success("Éxito", "Se ha registrado con éxito");
        }
        ManageModelErrors(d.Data);
    });
}
function Update() {
    var data = getFormObject("#frmModel");
    useAjax("/Presupuestos/Edit", JSON.stringify(data), d => {
        if (d.Success) {
            clearForm("#frmModel");
            $('#modelModal').modal("hide");
            LoaddataTable();
            toaster.success("Éxito", "Se ha actualizado con éxito");
        }
        ManageModelErrors(d.Data);
    });
}
function LoaddataTable() {
    if (dataTable !== null)
        dataTable.destroy();
    let columns =
        [
            { data: "id_pres" },
            { data: "Desde", render: (d, t) => new Date(d).toLocaleDateString("es-DO")},
            { data: "Hasta", render: (d, t) => new Date(d).toLocaleDateString("es-DO") },
            { data: "Nombre" },
            {
                data: "id_pres",
                render: function (data, type) {
                    return `<a class='btn btn-info' onclick='edit(${data}, this)' title="Editar"><span class='octicon octicon-pencil'></span></a>&nbsp;` +
                        `<a class='btn btn-danger' onclick='confirm(${data}, this)' title="Eliminar"><span class='octicon octicon-trashcan'></span></a>`;
                }
            }
        ];
    dataTable = loadTable("#tData", "/Presupuestos/GetAll", columns, [[0, "desc"]]);
}
function newItem() {
    $("#btnGuardar").show();
    $("#btnActualizar").hide();
    clearForm("#frmModel");
    $('#modelModal').modal("show");
}
function edit(id, elem) {
    $("#btnGuardar").hide();
    $("#btnActualizar").show();
    clearForm("#frmModel");
    useAjax("/Presupuestos/Get", "{id: " + id + "}", (d) => {
        bindObjectToForm("#frmModel", d);
        $('#modelModal').modal("show");
    });
}
function confirm(id) {
    $('#confirmModal').modal("show");
    document.querySelector("#btnEliminar").onclick = function () { destroy(id); };
}
function destroy(id) {
    useAjax("/Presupuestos/Delete", "{id: " + id + "}", (d) => {
        $('#confirmModal').modal("hide");
        if (d.Success) {
            LoaddataTable();
            toaster.success("Éxito", "Se ha eliminado con éxito");
        }
        else {
            toaster.warning("Advertencia", d.Data);
        }
    });
}
