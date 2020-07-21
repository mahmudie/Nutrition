function FormViewModel(model) {
    var self = this;
    self.rep = ko.observableArray();
    self.enabled = ko.observable(true);
    self.enabledstock = ko.observable(true);
    self.sending = ko.observable(false);
    self.loaded = ko.observable(true);
    var uri = "/api/opdmam";
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
    self.reports = {
        Nmrid: model.nmrid,
        Mamid: ko.observable(),
        TMale: ko.observable(0).extend({ numeric: 0 }),
        TFemale: ko.observable(0).extend({ numeric: 0 }),
        Absents: ko.observable(0).extend({ numeric: 0 }),
        Muac12: ko.observable(0).extend({ numeric: 0 }),
        Muac23: ko.observable(0).extend({ numeric: 0 }),
        Zscore23: ko.observable(0).extend({ numeric: 0 }),
        Cured: ko.observable(0).extend({ numeric: 0 }),
        ReferIn: ko.observable(0).extend({ numeric: 0 }),
        NonCured: ko.observable(0).extend({ numeric: 0 }),
        Defaulters: ko.observable(0).extend({ numeric: 0 }),
        Deaths: ko.observable(0).extend({ numeric: 0 }),
        Totalbegin: ko.observable(0).extend({ numeric: 0 }),
        Transfers: ko.observable(0).extend({ numeric: 0 }),
        AgeGroup: ko.observable(),
        UserName:model.userName,
    }
    self.stocks = ko.observableArray();
    self.sinStock = {

        Nmrid: model.nmrid,
        Item: ko.observable(),
        StockId: ko.observable(),
        OpeningBalance: ko.observable(0).extend({ numeric: 0 }),
        QuantityReceived: ko.observable(0).extend({ numeric: 0 }),
        QuantityDistributed: ko.observable(0).extend({ numeric: 0 }),
        QuantityTransferred: ko.observable(0).extend({ numeric: 0 }),
        QuantityReferin: ko.observable(0).extend({ numeric: 0 }),
        Losses: ko.observable(0).extend({ numeric: 0 }),
        QuantityReturned: ko.observable(0).extend({ numeric: 0 }),
        ExpectedRecepients: ko.observable(0).extend({ numeric: 0 }),
        UserName:model.userName,

    }
    self.partialForm = {
        SfpAwg: ko.observable(model.sfpAwg),
        SfpAls: ko.observable(model.sfpAls),
    }
    self.questions = {
        Nmrid:model.nmrid,
        Bnaqid:model.bnaqid,
        MamRusfstockoutWeeks: ko.observable(model.mamRusfstockoutWeeks).extend({ numeric: 0 }),
        MamAddminsionByChws: ko.observable(model.mamAddminsionByChws).extend({ numeric: 0 }),
    }
    self.partialRecord = function (form) {
        var x = document.getElementById("questions");
        if (!$(form).valid())
            return false;
        if (!$(x).valid())
            return false;
        var token = $("input[name=__RequestVerificationToken]").val();
        
        self.questions.__RequestVerificationToken = token;
             $.ajax({
                     url: '/Bnaq/edit/'+self.questions.Nmrid+'/'+3,
                     type: 'POST',
                     contentType: 'application/x-www-form-urlencoded',
                     data: ko.toJS(self.questions)
                 })
                 .success(function(){
                    $("#send").notify("Successfully Saved.","success");
                    ajaxHelper("/api/opdmam/partial/" + [self.reports.Nmrid], 'PUT', ko.toJS(self.partialForm), token).success(function (data) {
                        $("#send").notify("Successfully Saved.", "success");
                    }).fail(function () {
                        $("#send").notify(" something went wrong", "error");
                    });                    
                 })
                 .error(function(){
                     $("#send").notify(" something went wrong","error");
             })



    }

    if (!('contains' in String.prototype)) {
        String.prototype.contains = function (str, startIndex) {
            return -1 !== String.prototype.indexOf.call(this, str, startIndex);
        };
    }

    self.delete = function (item) {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + '/' + item.mamid + 'id2=' + item.nmrid, 'DELETE', null, token)
            .success(function (data) {
                self.rep.remove(item);
                $.notify("Deleted", "error");
            });
    }

    function ajaxHelper(uri, method, data, token) {
        return $.ajax({
                type: method,
                url: uri,
                headers: {
                    "csrftoken": token
                },
                dataType: 'json',
                contentType: 'application/json',
                data: data ? JSON.stringify(data) : null
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                $.notify(" something went wrong", "error");
            });
    }

    function getReports() {
        self.loaded(false);
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + '/find/' + [self.reports.Nmrid], 'GET', null, token).done(function (data) {
            self.rep(data);
            self.loaded(true);
            self.sending(false);
        });
    }

    function getStocks() {
        self.loaded(false);
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + '/stock/' + [self.sinStock.Nmrid], 'GET', null, token).done(function (data) {
            self.stocks(data);
            self.loaded(true);
            self.sending(false);
        });
    }
    self.checkNum = ko.computed(function () {
        var length = self.rep().length;
        return length;

    });
    self.checkInputs = ko.computed(function () {
        var female = self.reports.TFemale() ? parseInt(self.reports.TFemale()) : 0;
        var male = self.reports.TMale() ? parseInt(self.reports.TMale()) : 0;
        var zscore = self.reports.Zscore23() ? parseInt(self.reports.Zscore23()) : 0;
        var muac12 = self.reports.Muac12() ? parseInt(self.reports.Muac12()) : 0;
        var muac23 = self.reports.Muac23() ? parseInt(self.reports.Muac23()) : 0;
        return (zscore + muac12 + muac23  == female + male);
    })


    
    self.checkOut = ko.computed(function () {
        var female = self.reports.TFemale()?parseInt(self.reports.TFemale()):0;
        var male = self.reports.TMale()?parseInt(self.reports.TMale()):0;

        var cured = self.reports.Cured()?parseInt(self.reports.Cured()):0;
        var noncured = self.reports.NonCured()?parseInt(self.reports.NonCured()):0;
        var death = self.reports.Deaths()?parseInt(self.reports.Deaths()):0;
        var dflt = self.reports.Defaulters() ? parseInt(self.reports.Defaulters()) : 0;
        var refout = self.reports.Transfers() ? parseInt(self.reports.Transfers()) : 0;
        var balance = self.reports.Totalbegin() ? parseInt(self.reports.Totalbegin()) : 0;
        var refin = self.reports.ReferIn() ? parseInt(self.reports.ReferIn()) : 0;

        return (cured + noncured + death + dflt + refout) <= (female + male + refin + balance);

    });

        self.checkOut2 = ko.computed(function () {
        var female = self.reports.Muac23()?parseInt(self.reports.Muac23()):0;

        var cured = self.reports.Cured()?parseInt(self.reports.Cured()):0;
        var noncured = self.reports.NonCured()?parseInt(self.reports.NonCured()):0;
        var death = self.reports.Deaths()?parseInt(self.reports.Deaths()):0;
        var dflt = self.reports.Defaulters() ? parseInt(self.reports.Defaulters()) : 0;
        var refout = self.reports.Transfers() ? parseInt(self.reports.Transfers()) : 0;
        var balance = self.reports.Totalbegin() ? parseInt(self.reports.Totalbegin()) : 0;
        var refin = self.reports.ReferIn() ? parseInt(self.reports.ReferIn()) : 0;

            return (cured + noncured + death + dflt + refout) <= (balance + female + refin);

    });



    self.edit = function (item) {
        self.enabled(false);
        map(item);
        calctotal(item);
        $.get("/mamview/edit/" + item.mamid + '?nmrid=' + item.nmrid, function (d) {
            $('#popup').html(d);
            $('#edit').modal('show');
            ko.applyBindings(self, document.getElementById('edit'));
            self.enabled(true);
        }).fail(function () {
            self.enabled(true);
        });

    };
    self.editRecord = function (form) {
        if (!$(form).valid())
            return false;
        if(self.dropdownText()==1){
        self.sending(true);
        var token = form["__RequestVerificationToken"].value;
        self.reports.TFemale(parseInt(self.reports.Muac23()));
        ajaxHelper(uri + "/" + self.reports.Mamid + "nmrid=" + [self.reports.Nmrid], 'PUT', ko.toJS(self.reports), token).success(function (data) {
            getReports();
            $.notify(" successfully saved.", "success");
            $('#edit').modal('hide');

        }).fail(function () {
            $('#edit').modal('hide');
            self.sending(false);
            $.notify(" something went wrong", "error");
        });
       }
       if(self.dropdownText()==2){
        self.sending(true);
        var token = form["__RequestVerificationToken"].value;
        ajaxHelper(uri + "/" + self.reports.Mamid + "nmrid=" + [self.reports.Nmrid], 'PUT', ko.toJS(self.reports), token).success(function (data) {
            getReports();
            self.sending(false);
            $.notify(" successfully saved.", "success");
            $('#edit').modal('hide');

        }).fail(function () {
            $('#edit').modal('hide');
            self.sending(false);
            $.notify(" something went wrong", "error");
        });
        }
    }
    self.newReport = function (data, event) {
        self.enabled(false);
        $.get($(event.target).attr('href')).success(function (d) {
            self.enabled(true);
            getReports();
            getStocks();
        }).fail(function () {
            self.enabled(true);
        });
    };

    self.editStock = function (item) {
        self.enabledstock(false);
        mapStock(item);
        $.get("/mamview/EditStock/" + item.stockId + "?nmrid=" + item.nmrid, function (d) {
            $('#popup').html(d);
            $('#editStock').modal('show');
            ko.applyBindings(self, document.getElementById('editStock'));
            self.enabledstock(true);
        }).fail(function () {
            self.enabledstock(true);
        });

    };
    self.editstockRecord = function (form) {
        if (!$(form).valid())
            return false;
        self.sending(true);
        var token = form["__RequestVerificationToken"].value;
        ajaxHelper(uri + "/stock/" + self.sinStock.StockId + "nmrid=" + [self.sinStock.Nmrid], 'PUT', ko.toJS(self.sinStock), token).success(function (data) {
            getStocks();
            $.notify(" successfully saved.", "success");
            $('#editStock').modal('hide');

        }).fail(function () {
            $('#editStock').modal('hide');
            self.sending(false);
            $.notify(" something went wrong", "error");
        });

    }

    self.stockAthand = function (item) {
        var sum = (item.expectedRecepients * item.weight) - (item.openingBalance + item.quantityReceived - item.quantityDistributed -
            item.quantityTransferred - item.quantityReturned - item.losses);
        return sum;
    };
    self.closingBalance = function (item) {
        var sum = (item.openingBalance + item.quantityReceived - item.quantityDistributed -
            item.quantityTransferred - item.quantityReturned - item.losses);
        return sum;
    };
    self.deleteStock = function (item) {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + '/stock/' + item.stockId + 'nmrid=' + item.nmrid, 'DELETE', null, token)
            .success(function (data) {
                self.stocks.remove(item);
                $.notify("Deleted", "error");
            });
    };
    self.total = ko.pureComputed(function () {
        var totalbegin = 0;
        var sum2 = 0;
        var sum3 = 0;
        var sum4 = 0;
        var sum5 = 0;
        var sum6 = 0;
        var sum7 = 0;
        var sum8 = 0;
        var sum9 = 0;
        var sum10 = 0;
        var sum11 = 0;
        var sum12 = 0;
        var sum13 = 0;
        var sum14 = 0;
        var s1 = 0;
        var s2 = 0;
        var s3 = 0;
        var s4 = 0;
        var s5 = 0;
        var s6 = 0;
        var s7 = 0;
        var s8 = 0;
        var e1 = 0;
        var e2 = 0;
        ko.utils.arrayForEach(self.rep(), function (report) {
            totalbegin += Number(report.totalbegin);
            sum2 += Number(report.zscore23);
            sum3 += Number(report.muac12);
            sum4 += Number(report.muac23);
            sum5 += Number(report.tFemale);
            sum6 += Number(report.tMale);
            sum7 += Number(report.referIn);
            sum8 += Number(report.absents);
            sum9 += Number(report.cured);
            sum10 += Number(report.deaths);
            sum11 += Number(report.defaulters);
            sum12 += Number(report.transfers);
            sum13 += Number(report.nonCured);
            if (report.ageGroup.toLocaleLowerCase().contains("children")) {
                s1 += Number(report.cured);
                s2 += Number(report.deaths);
                s3 += Number(report.defaulters);
                s4 += Number(report.nonCured);
                e1 += Number(report.cured + report.defaulters + report.deaths + report.transfers + report.nonCured);
            }
            if (report.ageGroup.toLocaleLowerCase().contains("women")) {
                s5 += Number(report.cured);
                s6 += Number(report.deaths);
                s7 += Number(report.defaulters);
                s8 += Number(report.nonCured);
                e2 += Number(report.cured + report.defaulters + report.deaths + report.transfers + report.nonCured);
            }

        });
        return {
            sum: [totalbegin, sum2, sum3, sum4, sum6, sum5, sum5 + sum6, sum7, sum5 + sum6 + sum7, sum8, sum9, sum10,
                sum11, sum12, sum13, sum9 + sum10 + sum11 + sum13 + sum12, (totalbegin + sum5 + sum6 + sum7) - (sum8 + sum10 + sum11 + sum12), (totalbegin + sum5 + sum6 + sum7) - (sum9 + sum10 + sum13 + sum11 + sum12), , Math.floor((s1 / e1) * 100) ? Math.floor((s1 / e1) * 100) + ' %' : 0, Math.floor((s2 / e1) * 100) ? Math.floor((s2 / e1) * 100) + ' %' : 0, Math.floor((s3 / e1) * 100) ? Math.floor((s3 / e1) * 100) + ' %' : 0, Math.floor((s4 / e1) * 100) ? Math.floor((s4 / e1) * 100) + ' %' : 0, Math.floor((s5 / e2) * 100) ? Math.floor((s5 / e2) * 100) + ' %' : 0, Math.floor((s6 / e2) * 100) ? Math.floor((s6 / e2) * 100) + ' %' : 0, Math.floor((s7 / e2) * 100) ? Math.floor((s7 / e2) * 100) + ' %' : 0, Math.floor((s8 / e2) * 100) ? Math.floor((s8 / e2) * 100) + ' %' : 0
            ]
        };
    });
    self.dropdownText = ko.computed(function () {
        var text = String(self.reports.AgeGroup());
        var text = text.toLocaleLowerCase().replace(/\s/g, '');
        if (text.contains("pregnantwomen"))
            return 1;
        if (text.contains("lactatingwomen"))
            return 1;
        if (text.contains("children"))
            return 2;
        if (text == "")
            return -1;
        else
            return 0;
    });
    self.tbegin = ko.observable();

    function calctotal(item) {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + "/total/" + item.mamid + "nmrid=" + item.nmrid, 'get', null, token)
            .done(function (data) {
                if (data > 0) {
                    self.reports.Totalbegin(data);
                    self.tbegin(data);
                } else {
                    self.tbegin(data);
                }
            })
    }

    function map(data) {
        self.reports.Deaths(data.deaths);
        self.reports.AgeGroup(data.ageGroup);
        self.reports.Muac12(data.muac12);
        self.reports.Muac23(data.muac23);
        self.reports.TMale(data.tMale);
        self.reports.Transfers(data.transfers);
        self.reports.TFemale(data.tFemale);
        self.reports.Defaulters(data.defaulters);
        self.reports.Cured(data.cured);
        self.reports.NonCured(data.nonCured);
        self.reports.ReferIn(data.referIn);
        self.reports.Zscore23(data.zscore23);
        self.reports.Totalbegin(data.totalbegin);
        self.reports.Absents(data.absents);
        self.reports.Mamid = data.mamid;
    }

    function mapStock(data) {
        self.sinStock.StockId = data.stockId;
        self.sinStock.Item(data.item);
        self.sinStock.OpeningBalance(data.openingBalance);
        self.sinStock.QuantityReceived(data.quantityReceived);
        self.sinStock.QuantityDistributed(data.quantityDistributed);
        self.sinStock.QuantityTransferred(data.quantityTransferred);
        self.sinStock.QuantityReferin(data.quantityReferin);
        self.sinStock.Losses(data.losses);
        self.sinStock.QuantityReturned(data.quantityReturned);
        self.sinStock.ExpectedRecepients(data.expectedRecepients);
    }
    getReports();
    getStocks();
}