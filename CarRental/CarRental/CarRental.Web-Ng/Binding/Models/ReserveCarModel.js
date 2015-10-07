
(function (cr) {
    var ReserveCarModel = function () {

        var self = this;

        self.initialized = false;
        self.PickupDate = '';
        self.ReturnDate = '';
    }
    cr.ReserveCarModel = ReserveCarModel;
}(window.CarRental));
