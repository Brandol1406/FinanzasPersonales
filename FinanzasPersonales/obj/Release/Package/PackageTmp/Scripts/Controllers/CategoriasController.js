'use strict';

let CategoriasTable = null;

$(document).ready(() => {
    $("#btnAgregar").click(() => newItem());
    $("#btnGuardar").click(() => Save());
    $("#btnActualizar").click(() => Update());
    LoaddataTable();
});
function Save() {
    var data = getFormObject("#frmModel");
    useAjax("/Categorias/Create", JSON.stringify(data), d => {
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
    useAjax("/Categorias/Edit", JSON.stringify(data), d => {
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
    if (CategoriasTable !== null)
        CategoriasTable.destroy();
    let columns =
        [
            { data: "id_cat" },
            { data: "Nombre" },
            {
                data: "id_cat",
                render: function (data, type) {
                    return `<a class='btn btn-info' onclick='edit(${data}, this)' title="Editar"><span class='octicon octicon-pencil'></span></a>&nbsp;` +
                        `<a class='btn btn-danger' onclick='confirm(${data}, this)' title="Eliminar"><span class='octicon octicon-trashcan'></span></a>`;
                }
            }
        ];
    CategoriasTable = loadTable("#tData", "/Categorias/GetAll", columns, [[0, "desc"]]);
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

    useAjax("/Categorias/Get", "{id: " + id + "}", (d) => {
        bindObjectToForm("#frmModel", d);
        $('#modelModal').modal("show");
    });
}
function confirm(id) {
    $('#confirmModal').modal("show");
    document.querySelector("#btnEliminar").onclick = function () { destroy(id); };
}
function destroy(id) {
    useAjax("/Categorias/Delete", "{id: " + id + "}", (d) => {
        console.log(d);
        $('#confirmModal').modal("hide");
        if (d.Success) {
            LoaddataTable();
            toaster.success("Éxito", "Se ha eliminado con éxito");
        }
        else {
            toaster.warning("Aviso", d.Data);
        }
    });
}
