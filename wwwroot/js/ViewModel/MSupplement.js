function MsForm(model) {
    var self = this;
    self.options = ko.observableArray();
    self.all = ko.observableArray();
    self.state = ko.observable(true);
    self.sending = ko.observable(false);
    uri = "/api/msform/";
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
        Nmrid: model.nmrid,
        Mnitems: ko.observable(),
        Mnid: ko.observable(),
        chu2m: ko.observable(0).extend({ numeric: 0 }),
        chu2f: ko.observable(0).extend({ numeric: 0 }),
        Remarks: ko.observable(),
        refbyCHW: ko.observable(0).extend({ numeric: 0 }),
        UserName: model.userName
    }


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
    }
    self.dropdownText = ko.computed(function () {
        var text = String(self.form.Mnitems());
        var text = text.toLocaleLowerCase().replace(/\s/g, '');
        if (text.contains("vita"))
            return 1;
        if (text.contains("ferroussulfatedrop"))
            return 2;
        if (text.contains("ferroussulfate200"))
            return 3;
        if (text.contains("multiplemicronutrientspowder"))
            return 4;
        if (text.contains("zinctab"))
            return 4;
        if (text == "")
            return -1;
        else
            return 0;

    });



    self.delete = function (item) {
        var token = $("input[name=__RequestVerificationToken]").val();
        $.ajax({
                url: uri + item.mnid + 'nmrid=' + item.nmrid,
                type: 'DELETE',
                headers: {
                    "csrftoken": token
                },
                contentType: 'application/json',
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
        ajaxHelper(uri + self.form.Nmrid, "GET").done(function (data) {
            self.all(data);
            self.sending(false);       
        });

    }
    self.edit = function (item) {
        self.form.Mnid = item.mnid;
        self.form.chu2m(item.chu2m);
        self.form.chu2f(item.chu2f);
        self.form.Remarks(item.remarks);
        self.form.refbyCHW(item.refbyCHW);
        self.form.Mnitems(item.mnitems);
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
                url: uri + self.form.Mnid + "nmrid=" + self.form.Nmrid,
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
    self.totalw = ko.computed(function () {
        var sum = 0;
        ko.utils.arrayForEach(self.all(), function (report) {
            sum += Number(report.chu2m);
        });
        return sum;
    });

    self.totalc = ko.computed(function () {
        var sum = 0;
        ko.utils.arrayForEach(self.all(), function (report) {
            sum += Number(report.chu2f);
        });
        return sum;
    });

    self.totalp = ko.computed(function () {
        var sum = 0;
        ko.utils.arrayForEach(self.all(), function (report) {
            sum += Number(report.refbyCHW);
        });
        return sum;
    });

    loadData();
}