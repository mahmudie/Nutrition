function forms(model) {
    var self = this;
    self.success = ko.observableArray();
    self.error = ko.observableArray();
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
        Nmrid:model.nmrid,
        UserName:model.userName,
        GirlsScreened: ko.observable(model.girlsScreened).extend({ numeric: 0 }),
        BoysScreened: ko.observable(model.boysScreened).extend({ numeric: 0 }),
        Plwreported: ko.observable(model.plwreported).extend({ numeric: 0 }),

    }

    self.validateAndSave = function (form) {
        if (!$(form).valid())
            return false;
        // include the anti forgery token
        self.form.__RequestVerificationToken = form['__RequestVerificationToken'].value;
        $.ajax({
            url: '/Bnaq/edit/'+self.form.Nmrid+'/'+4,
            type: 'POST',
            contentType: 'application/x-www-form-urlencoded',
            data: ko.toJS(self.form)
        })
        .success(function(){
           $("#send").notify("Successfully Saved.","success");
        })
        .error(function(){
            $("#send").notify(" something went wrong","error");
    })
    };

}