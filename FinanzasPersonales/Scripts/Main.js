function getFormObject(form) {
    var obj = {};
    $(form).find("input, select, textarea").each(function () {
        obj[this.id] = this.value;
    });
    return obj;
}
function clearForm(form, fieldsToIgnore) {
    fieldsToIgnore = fieldsToIgnore || "";
    $(form).find("input, select, textarea").each(function () {
        if (fieldsToIgnore.indexOf(this.id) === -1) {
            if (this.tagName === "SELECT")
                $(this).val(0);
            else
                $(this).val("");
            var label = $("#" + this.id + "-msg");
            if (label.length > 0) {
                label.remove();
            }
        }
    });
}
function bindObjectToForm(form, object) {
    for (p in object) {
        let elem = $(form).find("#" + p);
        if (elem.attr('type') === "date") {
            let seconds = Number(object[p].replace(/\//g, "").replace("Date(", "").replace(")", ""));
            var date = new Date(seconds);
            var day = ("0" + date.getDate()).slice(-2);
            var month = ("0" + (date.getMonth() + 1)).slice(-2);

            elem.val(date.getFullYear() + "-" + (month) + "-" + (day));
        } else {
            elem.val(object[p]);
        }
    }
}
function useAjax(url, data, onSuccess, method = "POST", async = true) {
    let configs = {
        type: method,
        url: "/App" + url,
        async: async,
        data: data,
        contentType: "application/json; charset=utf-8",
        success: function (data, textStatus, jqXHR) {
            onSuccess(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            document.write(jqXHR.responseText);
        }
    };
    $.ajax(configs);
}
function ManageModelErrors(errArray) {
    errArray.forEach(err => {
        if (err.Errors.length > 0) {
            let errSpan = `<span class='text-danger' id='${err.Key}-msg'>${err.Errors[0].ErrorMessage}</span>`;
            if (document.querySelector("#" + err.Key + "-msg") === null)
                $("#" + err.Key).after(errSpan);
        } else {
            $('#' + err.Key + '-msg').remove();
        }
    });
}
function fillDropDown(select, data, valueMember, displayMember) {
    data.forEach(e => {
        let optionTemplate = `<option value='${e[valueMember]}'>${e[displayMember]}</option>`;
        $(select).append(optionTemplate);
    });
}

function loadTable(table, url, columns, order = [[0, "desc"]]) {
    return $(table).DataTable({
        ajax: { url: "/App" + url, dataSrc: "" },
        columns: columns,
        order: order
    });
}