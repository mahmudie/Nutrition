function ErrIndicatorsForm(model) {
    var self = this;
    self.options = ko.observableArray();
    self.all = ko.observableArray();
    self.state = ko.observable(true);
    self.sending = ko.observable(false);
    uri = "/api/errindicators/";
    ko.extenders.numeric = function(target, precision) {
    var result = ko.pureComputed({
        read: target, 
        write: function(newValue) {
            var current = target(),
                roundingMultiplier = Math.pow(10, precision),
                newValueAsNum = Number(newValue)<0 ? 0 : +newValue,
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
    self.form = {
        ErnmrId: model.ernmrId,
        IndicatorName: ko.observable(),
        IndicatorId: ko.observable(),
        Male: ko.observable(0).extend({ numeric: 0 }),
        Female: ko.observable(0).extend({ numeric: 0 }),
        UserName: model.userName
    };


    if (!('contains' in String.prototype)) {
        String.prototype.contains = function (str, startIndex) {
            return -1 !== String.prototype.indexOf.call(this, str, startIndex);
        };
    }

    function ajaxHelper(uri, method, data) {
        return $.ajax({
                type: method,
                url: uri,
                dataType: 'json',
                contentType: 'application/json',
                data: data ? JSON.stringify(data) : null
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(" something went wrong", "error");
            });
    }



    self.addForm = function (data, event) {
        self.state(false);
        $.get($(event.target).attr('href')).done(function () {
            self.state(true);
        }).success(function () {
            loadData();
        })
            .fail(function () {
                $.notify(" something went wrong", "error");
                self.state(true);

            });
    };

    self.dropdownText = ko.computed(function () {
        var text = String(self.form.IndicatorName());
        var text = text.toLocaleLowerCase().replace(/\s/g, '');
        if (text.contains("pregnant"))
            return 2;
        if (text.contains("mothers"))
            return 3;
        if (text === "")
            return -1;
        else
            return 0;

    });



    self.delete = function (item) {
        var token = $("input[name=__RequestVerificationToken]").val();
        $.ajax({
            url: uri + item.indicatorId + 'ernmrid=' + item.ernmrId,
                type: 'DELETE',
                headers: {
                    "csrftoken": token
                },
                contentType: 'application/json'
            }).fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(" something went wrong", "error");
            }).success(function () {
                $.notify("Deleted.", "error");
            })
            .done(function () {
                loadData();
            });
    };

    function loadData() {
        ajaxHelper(uri + self.form.ErnmrId, "GET").done(function (data) {
            self.all(data);
            self.sending(false);       
        });

    }
    self.edit = function (item) {
        self.form.IndicatorId = item.indicatorId;
        self.form.Male(item.male);
        self.form.Female(item.female);
        self.form.IndicatorName(item.indicatorName);
        var validator = $("#form").validate();
        $('#form').find('.field-validation-error span').each(function () {
            validator.settings.success($(this));
        });
        validator.resetForm();
        $('#update').modal('show');

    };
    self.update = function (form) {
        if (!$(form).valid())
            return false;
        self.sending(true);
        var token = form["__RequestVerificationToken"].value;
        $.ajax({
            url: uri + self.form.IndicatorId + "ernmrid=" + self.form.ErnmrId,
                type: 'Put',
                headers: {
                    "csrftoken": token
                },
                contentType: 'application/json',
                data: ko.toJSON(self.form)
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(" something went wrong", "error");
                $('#update').modal('hide');
                self.sending(false);
            }).success(function () {
                $.notify("Successfully Saved.", "success");
            })
            .done(function () {
                loadData();
                $('#update').modal('hide');
            });
    };

    loadData();
}