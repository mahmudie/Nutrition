function FormViewModel(model) {
    var self = this;
    self.rep = ko.observableArray();
    self.enabled = ko.observable(true);
    self.enabledstock = ko.observable(true);
    self.sending = ko.observable(false);
    self.loaded = ko.observable(true);
    var uri = "/api/samin";
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
        Otptfuid: ko.observable(),
        TMale: ko.observable(0).extend({ numeric: 0 }),
        TFemale: ko.observable(0).extend({ numeric: 0 }),
        RefOut: ko.observable(0).extend({ numeric: 0 }),
        Muac115: ko.observable(0).extend({ numeric: 0 }),
        Z3score: ko.observable(0).extend({ numeric: 0 }),
        Cured: ko.observable(0).extend({ numeric: 0 }),
        NonCured: ko.observable(0).extend({ numeric: 0 }),
        Defaulters: ko.observable(0).extend({ numeric: 0 }),
        Defaultreturn: ko.observable(0).extend({ numeric: 0 }),
        Death: ko.observable(0).extend({ numeric: 0 }),
        Totalbegin: ko.observable(0).extend({ numeric: 0 }),
        Odema: ko.observable(0).extend({ numeric: 0 }),
        Fromsfp: ko.observable(0).extend({ numeric: 0 }),
        Fromscotp: ko.observable(0).extend({ numeric: 0 }),
        ageGroup: ko.observable(0).extend({ numeric: 0 }),
        UserName:""
    }
    self.stocks = ko.observableArray();
    self.sinStock = {
        Nmrid:model.nmrid,
        SstockId: ko.observable(),
        Openingbalance:ko.observable(0).extend({ numeric: 0 }),
        Used:ko.observable(0).extend({ numeric: 0 }),
        Received:ko.observable(0).extend({ numeric: 0 }),
        Expired:ko.observable(0).extend({ numeric: 0 }),
        Damaged: ko.observable(0).extend({ numeric: 0 }),
        Loss: ko.observable(0).extend({ numeric: 0 }),
        Item:"",
        UserName: model.userName,
    }
    self.partialForm={
    IalsKwashiorkor: ko.observable(model.ialsKwashiorkor),
    IalsMarasmus: ko.observable(model.ialsMarasmus),
    IawgKwashiorkor: ko.observable(model.iawgKwashiorkor),
    IawgMarasmus: ko.observable(model.iawgMarasmus),
    Nmrid:model.nmrid
    }
    self.questions = {
        Nmrid:model.nmrid,
        Bnaqid:model.bnaqid,
        IpdRutfstockOutWeeks: ko.observable(model.ipdRutfstockOutWeeks).extend({ numeric: 0 }),
        IpdAdmissionsByChws: ko.observable(model.ipdAdmissionsByChws).extend({ numeric: 0 }),
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
                     url: '/Bnaq/edit/'+self.questions.Nmrid+'/'+1,
                     type: 'POST',
                     contentType: 'application/x-www-form-urlencoded',
                     data: ko.toJS(self.questions)
                 })
                 .success(function(){
                    $("#send").notify("Successfully Saved.","success");
                    ajaxHelper("/api/samin/partial/"+[self.reports.Nmrid], 'PUT', ko.toJS(self.partialForm), token).success(function (data) {
                        $("#send").notify("Successfully Saved.","success");
                     }).fail(function () {
                         $("#send").notify(" something went wrong","error");
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
        ajaxHelper(uri + '/' + item.otptfuid + 'id2=' + item.nmrid, 'DELETE',null,token)
            .success(function (data) {
                self.rep.remove(item);
                $.notify("Deleted","error");
            });
    }

    function ajaxHelper(uri, method, data,token) {
        return $.ajax({
            type: method,
            url: uri,
            headers: { "csrftoken": token },
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        })
            .fail(function (jqXHR, textStatus, errorThrown) {
                 $.notify(" something went wrong","error");
            });
    }
    self.tbegin=ko.observable();
    function calctotal(item){
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri+"/total/"+item.otptfuid+"nmrid="+item.nmrid, 'get',null, token)
            .done(function (data) {
                if(data>0){
                self.reports.Totalbegin(data);
                self.tbegin(data);}
                else{
                     self.tbegin(data);
                }
            })
    }
    function getReports() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + '/find/' + [self.reports.Nmrid], 'GET',null,token).done(function (data) {
            self.rep(data);
            self.sending(false);

        });
    }

    function getStocks() {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + '/stock/' + [self.sinStock.Nmrid], 'GET', null, token).done(function (data) {
            self.stocks(data);
            self.sending(false);
        });
    }
    self.checkNum = ko.computed(function () {
        var length = self.rep().length;
        return length;

    });
    self.checkInputs = ko.computed(function () {
        var female = self.reports.TFemale()?parseInt(self.reports.TFemale()):0;
        var male = self.reports.TMale()?parseInt(self.reports.TMale()):0;
        var zscore = self.reports.Z3score()?parseInt(self.reports.Z3score()):0;
        var muac = self.reports.Muac115() ? parseInt(self.reports.Muac115()) : 0;
        var odema = self.reports.Odema() ? parseInt(self.reports.Odema()) : 0;

        return (zscore + muac + odema == female + male );

    });
    self.checkOut = ko.computed(function () {
        var female = self.reports.TFemale()?parseInt(self.reports.TFemale()):0;
        var male = self.reports.TMale()?parseInt(self.reports.TMale()):0;

        var cured = self.reports.Cured()?parseInt(self.reports.Cured()):0;
        var noncured = self.reports.NonCured()?parseInt(self.reports.NonCured()):0;
        var death = self.reports.Death() ? parseInt(self.reports.Death()) : 0;
        var defaultreturn = self.reports.Defaultreturn() ? parseInt(self.reports.Defaultreturn()) : 0;
        var fromsfp = self.reports.Fromsfp() ? parseInt(self.reports.Fromsfp()) : 0;
        var fromscotp = self.reports.Fromscotp() ? parseInt(self.reports.Fromscotp()) : 0;
        var dflt = self.reports.Defaulters() ? parseInt(self.reports.Defaulters()) : 0;
        var refout = self.reports.RefOut() ? parseInt(self.reports.RefOut()) : 0;
        var balance = self.reports.Totalbegin() ? parseInt(self.reports.Totalbegin()) : 0;
        

        return (cured + noncured + death + dflt + refout <= female + male + balance + defaultreturn + fromsfp + fromscotp);

    });

    self.edit = function (item) {
        self.enabled(false);
        map(item);
        calctotal(item);
        $.get("/Samin/edit/"+item.otptfuid+'?nmrid='+item.nmrid, function (d) {
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
        self.sending(true);
        var token = form["__RequestVerificationToken"].value;
        ajaxHelper(uri + "/" + self.reports.Otptfuid + "nmrid=" + [self.reports.Nmrid], 'PUT', ko.toJS(self.reports), token).success(function (data) {
            getReports();
            $('#edit').modal('hide');
            $.notify("Successfully Saved.","success");
        }).fail(function () {
            $('#edit').modal('hide');
            self.sending(false);
            $.notify(" something went wrong","error");
        });
    }
    self.newReport = function (data, event) {
        self.enabled(false);
        $.get($(event.target).attr('href')).done(function (d) {
            self.enabled(true);
            getReports();
            getStocks();
        }).fail(function () {
            self.enabled(true);
        })
    };

    self.editStock = function (item) {
        self.enabledstock(false);
        mapStock(item);
        $.get("/Samin/EditStock/" + item.sstockId + "?nmrid=" + item.nmrid, function (d) {
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
        ajaxHelper(uri + "/stock/" + self.sinStock.SstockId + "nmrid=" + [self.sinStock.Nmrid], 'PUT', ko.toJS(self.sinStock), token).success(function (data) {
            getStocks();
           $.notify("Successfully Saved.","success");
            $('#editStock').modal('hide');
        }).fail(function () {
            $('#editStock').modal('hide');
            $("#send").notify(" something went wrong","error");
            self.sending(false);
        });

    }

    self.stockAthand =function (item) {
       var sum= item.openingbalance + item.received -(item.used + item.expired + item.damaged + item.loss);
        return sum.toFixed(2);
    };
    self.deleteStock = function (item) {
        var token = $("input[name=__RequestVerificationToken]").val();
        ajaxHelper(uri + '/stock/' + item.sstockId + 'nmrid=' + item.nmrid, 'DELETE', null, token)
            .success(function (data) {
                self.stocks.remove(item);
                $.notify("Deleted","error");
            });
    };
    self.total = ko.pureComputed(function () {
        var sum1= 0; var sum2 = 0;var sum3 = 0; var sum4 = 0;var sum5 = 0; var sum6 = 0;
        var sum7 = 0; var sum8 = 0; var sum9 = 0; var sum10 = 0; var sum11 = 0; var sum12 = 0;
        var sum13 = 0; var sum14 = 0;
        var exit=0
        var ending=0
        ko.utils.arrayForEach(self.rep(), function (report) {
            sum1+= Number(report.totalbegin);
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
        });
        ending= (sum1+sum5 + sum6 + sum7 + sum8 + sum9) - (sum10 + sum11 + sum12 + sum13 + sum14);
        exit=sum10 + sum11 + sum12 + sum13 + sum14;
        return {
            sum: [sum1, sum2, sum3, sum4, sum5, sum6, sum5 + sum6, sum7, sum8, sum9, sum7 + sum8 + sum9,
                sum5 + sum6 + sum7 + sum8 + sum9, sum10, sum11, sum12, sum13, sum14,exit,
               ending,Math.floor((sum10 / exit) * 100)?Math.floor((sum10 / exit) * 100)+' %':0,Math.floor((sum11 / exit) * 100)?Math.floor((sum11 / exit) * 100)+' %':0
               ,Math.floor((sum12 / exit) * 100)?Math.floor((sum12 / exit) * 100)+ ' %':0
               ,Math.floor((sum13 / ending) * 100)?Math.floor((sum13 / ending) * 100) + ' %':0,Math.floor((sum14 / exit) * 100)?Math.floor((sum14 / exit) * 100)+'%':0]
        };
    });
    function map(data) {
        self.reports.Death(data.death);
        self.reports.Muac115(data.muac115);
        self.reports.TMale(data.tMale);
        self.reports.TFemale(data.tFemale);
        self.reports.Defaulters(data.defaulters);
        self.reports.Cured(data.cured);
        self.reports.Odema(data.odema);
        self.reports.NonCured(data.nonCured);
        self.reports.Fromsfp(data.fromsfp);
        self.reports.Z3score(data.z3score);
        self.reports.Fromscotp(data.fromscotp);
        self.reports.RefOut(data.refOut);
        self.reports.Defaultreturn(data.defaultreturn);
        self.reports.Totalbegin(data.totalbegin);
        self.reports.Otptfuid = data.otptfuid;
        self.reports.UserName=data.userName
    }
    function mapStock(data) {
        self.sinStock.SstockId = data.sstockId;
        self.sinStock.Item = data.item;
        self.sinStock.Openingbalance(data.openingbalance);
        self.sinStock.Used(data.used);
        self.sinStock.Received(data.received);
        self.sinStock.Damaged(data.damaged);
        self.sinStock.Expired(data.expired);
        self.sinStock.Loss(data.loss);
    }
    getReports();
    getStocks();
}