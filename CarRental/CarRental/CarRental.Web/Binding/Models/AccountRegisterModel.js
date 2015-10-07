
(function (cr) {
    var AccountRegisterModelStep1 = function () {

        var self = this;

        self.FirstName = '';
        self.LastName = '';
        self.Address = '';
        self.City = '';
        self.State = '';
        self.ZipCode = '';
    }
    cr.AccountRegisterModelStep1 = AccountRegisterModelStep1;
}(window.CarRental));

(function (cr) {
    var AccountRegisterModelStep2 = function () {

        var self = this;

        self.LoginEmail = '';
        self.Password = '';
        self.PasswordConfirm = '';
        //self.LoginEmail = ko.observable("").extend({
        //    required: { message: "Login email is required" },
        //    email: { message: "Login is not a valid email" }
        //});
        //self.Password = ko.observable("").extend({
        //    required: { message: "Password is required" },
        //    minLength: { message: "Password must be at least 6 characters", params: 6 }
        //});
        //self.PasswordConfirm = ko.observable("").extend({
        //    validation: { validator: CarRental.mustEqual, message: "Password do not match", params: self.Password }
        //});
    }
    cr.AccountRegisterModelStep2 = AccountRegisterModelStep2;
}(window.CarRental));

(function (cr) {
    var AccountRegisterModelStep3 = function () {

        var self = this;

        self.CreditCard = '';
        self.ExpDate = '';
    }
    cr.AccountRegisterModelStep3 = AccountRegisterModelStep3;
}(window.CarRental));
