
appMainModule.controller("CurrentReservationsViewModel", function ($scope, $http, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.reservations = [];

    $scope.loadOpenReservations = function () {
        viewModelHelper.apiGet('api/reservation/getopen', null,
            function (result) {
                for (var i = 0; i < result.data.length; i++) {
                    result.Data['CancelRequest'] = false;
                    $scope.reservations = result.Data;
                }
            });
    }

    $scope.requestCancelReservation = function (reservation) {
        reservation.CancelRequest = true;
    }

    $scope.undoCancelRequest = function (reservation) {
        reservation.CancelRequest = false;
    }

    $scope.cancelReservation = function (reservation) {
        viewModelHelper.apiPost('api/reservation/cancel', { '': reservation.ReservationId },
            function (result) {
                $scope.reservations.remove(reservation);
            });
    }

    $scope.loadOpenReservations();
});
