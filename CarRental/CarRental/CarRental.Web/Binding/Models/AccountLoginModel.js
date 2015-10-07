
(function (cr) {
    var AccountLoginModel = function () {

        var self = this;

        self.LoginEmail = '';
        self.Password = '';
        self.RememberMe = false;
    }
    cr.AccountLoginModel = AccountLoginModel;
}(window.CarRental));
