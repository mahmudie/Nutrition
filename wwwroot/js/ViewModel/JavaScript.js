function forms() {
    var self = this;
    self.form = {
        DistName: ko.observable(),
        DistCode:ko.observable(),
        DistNameDari: ko.observable(),
        DistNamePashto: ko.observable(),
        ProvCode: ko.observable()
    }
    self.validateAndSave = function (form) {
        if (!$(form).valid())
            return false;
        // include the anti forgery token
        self.form.__RequestVerificationToken = form["__RequestVerificationToken"].value;
        $.ajax({
            url: 'Create',
            type: 'post',
            contentType: 'application/x-www-form-urlencoded',
            data: ko.toJS(self.form)
        })
        .success(self.successfulSave)
        .error(self.errorSave)
        .complete(function () { });
    };
    self.successfulSave = function () {
        $('.body-content').html('<div class="alert alert-success"><strong>Success!</strong> The new district has been saved.</div>');
        setTimeout(function () { location.href = './'; }, 1000);
    };
    self.errorSave = function () {
        $('.body-content').html('<div class="alert alert-danger"><strong>Error!</strong> There was an error creating district.</div>');
    };
}