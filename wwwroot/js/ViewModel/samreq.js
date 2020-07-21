function FormViewModel(id) {

    var self = this;
    self.dataArray = ko.observableArray();
    self.enabled = ko.observable(true);
    self.sending = ko.observable(false);
    self.Report = ko.observable();
    ko.extenders.numeric = function (target, precision) {
        var result = ko.pureComputed({
            read: target,
            write: function (newValue) {
                var current = target(),
                    roundingMultiplier = Math.pow(10, precision),
                    newValueAsNum = isNaN(newValue) ? 0 : +newValue,
                    valueToWrite = Math.round(newValueAsNum * roundingMultiplier) / roundingMultiplier;

                if (valueToWrite !== current) {
                    target(valueToWrite);
                } else {
                    if (newValue !== current) {
                        target.notifySubscribers(valueToWrite);
                    }
                }
            }
        }).extend({ notify: 'always' });

        result(target());

        return result;
    };
    var uri = "/report";
    self.model = {
        Rid: id,
        Id: id,
        CurrentBalance: ko.observable(0).extend({ numeric: 0 }),
        Adjustment: ko.observable(0).extend({ numeric: 0 }),
        AdjustmentComment: ko.observable(),

    }

    function ajaxHelper(uri, method, data, token) {
        return $.ajax({
            type: method,
            url: uri,
            headers: token ? {
                "csrftoken": token
            } : null,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(" something went wrong", "error");
            });
    }

    if (!('contains' in String.prototype)) {
        String.prototype.contains = function (str, startIndex) {
            return -1 !== String.prototype.indexOf.call(this, str, startIndex);
        };
    }



    self.edit = function (data) {
        self.model.Id = data.id;
        self.model.Adjustment(data.adj);
        self.model.CurrentBalance(data.balance);
        self.model.AdjustmentComment(data.comment);
        var validator = $("#form").validate();
        $('#form').find('.field-validation-error span').each(function () {
            validator.settings.success($(this));
        });
        validator.resetForm();
        $('#Edit').modal('show');
    };


    self.editRecord = function (form) {
        if (!$(form).valid())
            return false;
        self.sending(true);
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + "/edit/" + self.model.Id, 'POST', ko.toJS(self.model), token)
            .success(function (item) {
                getdataArray();
                $('#Edit').modal('hide');
                $.notify("Successfully Saved.", "success");
            })
            .complete(function () {
                $('#Edit').modal('hide');
                self.sending(false);
            });
    }


    function getdataArray() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + "/get/" + [self.model.Rid], 'GET', null, token).done(function (data) {
            self.dataArray(data);
            self.sending(false);
        });
    }
    getdataArray();

}