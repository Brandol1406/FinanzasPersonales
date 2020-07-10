var toastcount = 1;
var doToast = function (title, message, icon, text, autohide) {
    let toastName = "t" + toastcount;
    let time = (new Date()).toLocaleTimeString("es-DO");
    toastcount++;
    let template = `<div class="toast" data-autohide="${autohide}" data-animation="true" data-delay="10000" id="${toastName}">
            <div class="toast-header ${text}" style="color:white;">
                <span class="octicon ${icon}"></span>&nbsp;
                <strong class="mr-auto">${title}</strong>&nbsp;
                <small>${time}</small>
                <button type="button" class="ml-2 mb-1 close" data-dismiss="toast" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
                </div>
                <div class="toast-body">
                    ${message}
                </div>
            </div>`;
    $("#toastContainer").append(template);
    $('#' + toastName).toast("show");
    $('#' + toastName).on('hidden.bs.toast', function () {
        $('#' + toastName).remove();
    });
};
var toaster = {
    success: (title, message, dimisable = true) => doToast(title, message, "octicon-check", "bg-success", dimisable),
    info: (title, message, dimisable = true) => doToast(title, message, "octicon-info", "bg-info", dimisable),
    error: (title, message, dimisable = true) => doToast(title, message, "octicon-x", "bg-danger", dimisable),
    warning: (title, message, dimisable = true) => doToast(title, message, "octicon-alert", "bg-warning", dimisable)
};