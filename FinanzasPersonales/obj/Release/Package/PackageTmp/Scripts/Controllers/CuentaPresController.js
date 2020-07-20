'use strict';

let dataTable = null;

$(document).ready(() => {
    $("#btnAgregar").click(() => newItem());
    $("#btnGuardar").click(() => Save());
    $("#btnActualizar").click(() => Update());
    $("#presSelector").change(() => { setPres(); LoaddataTable();});
    fillOptions();
    setCurrentPres();
    setPres();
    LoaddataTable();
});
function fillOptions() {
    useAjax("/Categorias/GetAll", null, d => {
        fillDropDown("#id_cat", JSON.parse(d), 'id_cat', 'Nombre');
    }, "GET");
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
function setPres() {
    $("#id_pres").val($("#presSelector").val());
}
function clear() {
    clearForm("#frmModel", "id_pres,id_cuent");
}
function Save() {
    var data = getFormObject("#frmModel");
    useAjax("/CuentaPres/Create", JSON.stringify(data), d => {
        if (d.Success) {
            clear();
            $('#modelModal').modal("hide");
            LoaddataTable();
            toaster.success("Éxito", "Se ha registrado con éxito");
        }
        ManageModelErrors(d.Data);
    });
}
function Update() {
    var data = getFormObject("#frmModel");
    useAjax("/CuentaPres/Edit", JSON.stringify(data), d => {
        if (d.Success) {
            clear();
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
            { data: "id_cuent" },
            { data: "Categoria" },
            { data: "Limite" },
            {
                data: "id_cuent",
                render: function (data, type) {
                    return `<a class='btn btn-info' onclick='edit(${data}, this)' title="Editar"><span class='octicon octicon-pencil'></span></a>&nbsp;` +
                        `<a class='btn btn-danger' onclick='confirm(${data}, this)' title="Eliminar"><span class='octicon octicon-trashcan'></span></a>`;
                }
            }
        ];
    dataTable = loadTable("#tData", "/CuentaPres/GetAll/" + $("#presSelector").val(), columns, [[0, "desc"]], d => {
        if (d.length > 0) {
            let sum = 0;
            for (var i = 0; i < d.length; i++) {
                sum += d[i].Limite;
            }
            $("#total").html(sum.toLocaleString("es-DO"));
        }
        else {
            $("#total").html("0.00");
        }
    });
}
function newItem() {
    $("#btnGuardar").show();
    $("#btnActualizar").hide();
    clear();
    $('#modelModal').modal("show");
}
function edit(id, elem) {
    $("#btnGuardar").hide();
    $("#btnActualizar").show();
    clearForm("#frmModel");
    useAjax("/CuentaPres/Get", "{id: " + id + "}", (d) => {
        bindObjectToForm("#frmModel", d);
        $('#modelModal').modal("show");
    });
}
function confirm(id) {
    $('#confirmModal').modal("show");
    document.querySelector("#btnEliminar").onclick = function () { destroy(id); };
}
function destroy(id) {
    useAjax("/CuentaPres/Delete", "{id: " + id + "}", (d) => {
        $('#confirmModal').modal("hide");
        if (d.Success) {
            LoaddataTable();
            toaster.success("Éxito", "Se ha eliminado con éxito");
        } else {
            toaster.warning("Aviso", d.Data);
        } 
    });
}
