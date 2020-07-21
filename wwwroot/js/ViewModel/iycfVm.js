function FormViewModel(nmrid) {

    var self = this;
    self.dataArray = ko.observableArray();
    self.enabled = ko.observable(true);
    self.sending = ko.observable(false);
    self.Report = ko.observable();
    ko.extenders.numeric = function(target, precision) {
    var result = ko.pureComputed({
        read: target, 
        write: function(newValue) {
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
    var uri = "/api/iycf";
    self.model = {
        Nmrid: nmrid,
        Iycfid: ko.observable(),
        MChildU5months: ko.observable(0).extend({ numeric: 0 }),
        MChild524months: ko.observable(0).extend({ numeric: 0 }),
        Pregnanatwomen: ko.observable(0).extend({ numeric: 0 }),
        Firstvisit: ko.observable(0).extend({ numeric: 0 }),
        Revisit: ko.observable(0).extend({ numeric: 0 }),
        ReferIn: ko.observable(0).extend({ numeric: 0 }),
        ReferOut: ko.observable(0).extend({ numeric: 0 }),
        CauseShortName: ko.observable(),
        UserName: ko.observable()
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

    self.newForm = function (data, event) {
        self.enabled(false);
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + "/new/" + [self.model.Nmrid], 'Post', null, token).success(function (data) {
            getdataArray();
            self.enabled(true);
        }).done(function () {
            self.enabled(true);
            getdataArray();
        });
    };
    self.delete = function (item) {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + '/' + item.iycfid + 'nmrid=' + self.model.Nmrid, 'DELETE', null, token).success(function () {
            $.notify("deleted.", "error");
            getdataArray();
        });
    }

    self.edit = function (data) {
        self.model.Iycfid = data.iycfid;
        self.model.MChildU5months(data.mChildU5months);
        self.model.MChild524months(data.mChild524months);
        self.model.Pregnanatwomen(data.pregnanatwomen);
        self.model.Firstvisit(data.firstvisit);
        self.model.Revisit(data.revisit);
        self.model.ReferIn(data.referIn);
        self.model.ReferOut(data.referOut);
        self.model.CauseShortName(data.causeShortName);
        self.model.UserName(data.userName);
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
        ajaxHelper(uri + "/" + self.model.Iycfid + "nmrid=" + self.model.Nmrid, 'PUT', ko.toJS(self.model), token)
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
    self.total = ko.computed(function () {
        var sum = 0;
        var sum5 = 0;
        var sum1 = 0;
        var sum6 = 0;
        var sum2 = 0;
        var sum7 = 0;
        var sum3 = 0;
        var sum4 = 0;

        ko.utils.arrayForEach(self.dataArray(), function (report) {
            sum += Number(report.mChild524months + report.mChildU5months + report.pregnanatwomen);
            sum1 += Number(report.mChildU5months);
            sum2 += Number(report.mChild524months);
            sum3 += Number(report.pregnanatwomen);
            sum4 += Number(report.firstvisit);
            sum5 += Number(report.revisit);
            sum6 += Number(report.referIn);
            sum7 += Number(report.referOut);
        });
        return {
            sum: [sum, sum1, sum2, sum3, sum4, sum5, sum6, sum7]
        };
    });

    function getdataArray() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + "/" + [self.model.Nmrid], 'GET', null, token).done(function (data) {
            self.dataArray(data);
            self.sending(false);
        });
    }
    getdataArray();

}