function FormViewModel(model, user) {
    var self = this;
    self.user = ko.observable(user);
    self.rep = ko.observableArray();
    self.opds = ko.observableArray();
    self.opdstocks = ko.observableArray();
    self.samouts = ko.observableArray();
    self.bna = ko.observableArray();
    var uri = "/api/samin";
    self.stocks = ko.observableArray();
    self.samoutstocks = ko.observableArray();
    self.questions = ko.observableArray();
    self.error = ko.observable();
    self.success = ko.observable();
    self.dataArray = ko.observableArray();

    self.all = ko.observableArray();
    self.checkNum = ko.computed(function () {
        var length = self.rep().length;
        return length;

    });
    self.owner = function (name) {
        if (name === self.user()) {
            return true;

        } else {
            return false;
        }
    };
    function ajaxHelper(uri, method, data, token) {
        self.error('');
        self.success('');
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
                self.error(errorThrown);
            });
    }

    function getReports() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + '/admin/' + [model.nmrid], 'GET', null, token).done(function (data) {
            self.rep(data);
        });
    }

    function getMS() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper('/api/msform/admin/' + [model.nmrid], 'GET', null, token).done(function (data) {
            self.all(data);
        });
    }

    self.sendMessage = function (form) {
        var para = $('#Nmrid').val();
        var url = "/feedback/create/" + para;
        $.ajax({
            type: "POST",
            url: url,
            data: $("#message").serialize(), // serializes the form's elements.
            success: function (data) {
                $.notify("sent.", "success");
                getQuestions();
                $("#message").trigger("reset");
            },
        }).fail(function () {
            $.notify("failed.", "warning");

        }
        );

    }
    self.deleteMessage = function (data) {
        var url = "/feedback/delete/" + data.id;
        $.ajax({
            type: "GET",
            url: url,
            success: function () {
                $.notify("deleted.", "success");
                getQuestions();
            },
        }).fail(function () {
            $.notify("failed to delete.", "warning");

        }
        );

    }

    function getopds() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper('/api/opdmam/admin/' + [model.nmrid], 'GET', null, token).done(function (data) {
            self.opds(data);
        });
    }

    function getopdstocks() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper('/api/opdmam/adminstock/' + [model.nmrid], 'GET', null, token).done(function (data) {
            self.opdstocks(data);
        });
    }
    self.closingBalance = function (item) {
        var sum = (item.openingBalance + item.quantityReceived - item.quantityDistributed -
            item.quantityTransferred - item.quantityReturned - item.losses);
        return sum;
    };

    self.stockAthandopd = function (item) {
        var sum = (item.expectedRecepients * item.weight) - (item.openingBalance + item.quantityReceived - item.quantityDistributed -
            item.quantityTransferred - item.quantityReturned - item.losses);
        return sum;
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

    function getSamouts() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper('/api/samout/admin/' + [model.nmrid], 'GET', null, token).done(function (data) {
            self.samouts(data);
        });
    }

    function getQuestions() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper('/api/samout/que/' + [model.nmrid], 'GET', null, token).done(function (data) {
            self.questions(data);
        });
    }

    function getStocks() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + '/adminstock/' + [model.nmrid], 'GET', null, token).done(function (data) {
            self.stocks(data);
        });
    }

    function getsamoutStocks() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper('/api/samout/adminstock/' + [model.nmrid], 'GET', null, token).done(function (data) {
            self.samoutstocks(data);
        });
    }
    self.stockAthand = function (item) {
        var sum = (item.openingbalance + item.received) - (item.used + item.expired + item.damaged + item.loss);
        return sum.toFixed(2);
    };

    function getdataArray() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper("/api/iycf/admin/" + [model.nmrid], 'GET', null, token).done(function (data) {
            self.dataArray(data);
        });
    }
    self.totaliycf = ko.computed(function () {
        var sum1 = 0;
        var sum6 = 0;
        var sum2 = 0;
        var sum7 = 0;
        var sum3 = 0;
        var sum = 0;
        var sum4 = 0;
        var sum5 = 0;

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
    self.total = ko.pureComputed(function () {
        var sum1 = 0;
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
        var exit = 0
        var ending = 0
        ko.utils.arrayForEach(self.rep(), function (report) {
            if (!report.ageGroup.toLocaleLowerCase().contains("total")) {
                sum1 += Number(report.totalbegin);
                sum2 += Number(report.odema);
                sum3 += Number(report.z3score);
                sum4 += Number(report.muac115);
                sum5 += Number(report.tMale);
                sum6 += Number(report.tFemale);
                sum7 += Number(report.fromscotp);
                sum8 += Number(report.fromsfp);
                sum9 += Number(report.defaultreturn);
                sum10 += Number(report.cured);
                sum11 += Number(report.death);
                sum12 += Number(report.defaulters);
                sum13 += Number(report.refOut);
                sum14 += Number(report.nonCured);
            }
        });
        ending = (sum1 + sum5 + sum6 + sum7 + sum8 + sum9) - (sum10 + sum11 + sum12 + sum13 + sum14);
        exit = sum10 + sum11 + sum12 + sum13 + sum14;
        return {
            sum: [sum1, sum2, sum3, sum4, sum5, sum6, sum5 + sum6, sum7, sum8, sum9, sum7 + sum8 + sum9,
                sum5 + sum6 + sum7 + sum8 + sum9, sum10, sum11, sum12, sum13, sum14, exit,
                ending, Math.floor((sum10 / exit) * 100) ? Math.floor((sum10 / exit) * 100) + ' %' : 0, Math.floor((sum11 / exit) * 100) ? Math.floor((sum11 / exit) * 100) + ' %' : 0, Math.floor((sum12 / exit) * 100) ? Math.floor((sum12 / exit) * 100) + ' %' : 0, Math.floor((sum13 / ending) * 100) ? Math.floor((sum13 / ending) * 100) + ' %' : 0, Math.floor((sum14 / exit) * 100) ? Math.floor((sum14 / exit) * 100) + '%' : 0
            ]
        };
    });
    self.totalout = ko.pureComputed(function () {
        var sum1 = 0;
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
        var exit = 0
        var ending = 0
        ko.utils.arrayForEach(self.samouts(), function (report) {
            if (!report.ageGroup.toLocaleLowerCase().contains("total")) {
                sum1 += Number(report.totalbegin);
                sum2 += Number(report.odema);
                sum3 += Number(report.z3score);
                sum4 += Number(report.muac115);
                sum5 += Number(report.tMale);
                sum6 += Number(report.tFemale);
                sum7 += Number(report.fromscotp);
                sum8 += Number(report.fromsfp);
                sum9 += Number(report.defaultreturn);
                sum10 += Number(report.cured);
                sum11 += Number(report.death);
                sum12 += Number(report.defaulters);
                sum13 += Number(report.refOut);
                sum14 += Number(report.nonCured);
            }
        });
        ending = (sum1 + sum5 + sum6 + sum7 + sum8 + sum9) - (sum10 + sum11 + sum12 + sum13 + sum14);
        exit = sum10 + sum11 + sum12 + sum13 + sum14;
        return {
            sum: [sum1, sum2, sum3, sum4, sum5, sum6, sum5 + sum6, sum7, sum8, sum9, sum7 + sum8 + sum9,
                sum5 + sum6 + sum7 + sum8 + sum9, sum10, sum11, sum12, sum13, sum14, exit,
                ending, Math.floor((sum10 / exit) * 100) ? Math.floor((sum10 / exit) * 100) + ' %' : 0, Math.floor((sum11 / exit) * 100) ? Math.floor((sum11 / exit) * 100) + ' %' : 0, Math.floor((sum12 / exit) * 100) ? Math.floor((sum12 / exit) * 100) + ' %' : 0, Math.floor((sum13 / ending) * 100) ? Math.floor((sum13 / ending) * 100) + ' %' : 0, Math.floor((sum14 / exit) * 100) ? Math.floor((sum14 / exit) * 100) + '%' : 0
            ]
        };
    });
    if (!('contains' in String.prototype)) {
        String.prototype.contains = function (str, startIndex) {
            return -1 !== String.prototype.indexOf.call(this, str, startIndex);
        };
    }
    self.totalopd = ko.pureComputed(function () {
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
        ko.utils.arrayForEach(self.opds(), function (report) {
            if (!report.ageGroup.toLocaleLowerCase().contains("total")) {
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
            }
        });
        return {
            sum: [totalbegin, sum2, sum3, sum4, sum6, sum5, sum5 + sum6, sum7, sum5 + sum6 + sum7, sum8, sum9, sum10,
                sum11, sum12, sum13, sum9 + sum10 + sum11 + sum13 + sum12, (totalbegin + sum5 + sum6 + sum7) - (sum8 + sum10 + sum11 + sum12), (totalbegin + sum5 + sum6 + sum7) - (sum9 + sum10 + sum13 + sum11 + sum12), , Math.floor((s1 / e1) * 100) ? Math.floor((s1 / e1) * 100) + ' %' : 0, Math.floor((s2 / e1) * 100) ? Math.floor((s2 / e1) * 100) + ' %' : 0, Math.floor((s3 / e1) * 100) ? Math.floor((s3 / e1) * 100) + ' %' : 0, Math.floor((s4 / e1) * 100) ? Math.floor((s4 / e1) * 100) + ' %' : 0, Math.floor((s5 / e2) * 100) ? Math.floor((s5 / e2) * 100) + ' %' : 0, Math.floor((s6 / e2) * 100) ? Math.floor((s6 / e2) * 100) + ' %' : 0, Math.floor((s7 / e2) * 100) ? Math.floor((s7 / e2) * 100) + ' %' : 0, Math.floor((s8 / e2) * 100) ? Math.floor((s8 / e2) * 100) + ' %' : 0
            ]
        };
    });
    getReports();
    getSamouts();
    getStocks();
    getsamoutStocks();
    getMS();
    getopds();
    getopdstocks();
    getdataArray();
    getQuestions();
}