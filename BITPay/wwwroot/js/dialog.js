function showConfirmMessage(title, message, callback) {
    Swal.fire({
        title: title,
        text: message,
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: '<i class="fa fa-check"></i> Yes',
        cancelButtonText: '<i class="fa fa-times"></i> No',
        confirmButtonClass: 'btn-success btn m-r-5',
        cancelButtonClass: 'btn-danger btn m-l-5',
        buttonsStyling: false
    }).then((result) => {
        if (result.value) {
            callback();
        }
    });
}

function showProgress() {
    swal.fire({
        title: '<i class="fa fa-spin fa-spinner fa-3x"></i>',
        html: '<h4>Processing.....</h4>',
        footer: 'Please wait....',
        showConfirmButton: false
    });
}

function showErrorMessage(title, message) {
    Swal.fire({
        title: title,
        text: message,
        type: 'error',
        confirmButtonText: '<i class="fa fa-check"></i> Okay',
        confirmButtonClass: 'btn-success btn',
        buttonsStyling: false
    });
}

function showSuccessMessage(title, message) {
    Swal.fire({
        title: title,
        text: message,
        type: 'success',
        showConfirmButton: false,
        timer: 1500
    });
}