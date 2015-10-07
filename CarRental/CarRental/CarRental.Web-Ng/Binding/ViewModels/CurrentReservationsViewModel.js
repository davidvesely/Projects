
appMainModule.controller("CurrentReservationsViewModel", function ($scope, $http, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.reservations = [];
    $scope.init = false; // used so view doesn't sit on "no open reservations" while load takes place

    $scope.loadOpenReservations = function () {
        viewModelHelper.apiGet('api/reservation/getopen', null,
            function (result) {
                for (var i = 0; i < result.data.length; i++) {
                    result.data['CancelRequest'] = false;
                    $scope.reservations = result.data;
                }
                $scope.init = true;
            });
    }

    $scope.requestCancelReservation = function (reservation) {
        reservation.CancelRequest = true;
    }

    $scope.undoCancelRequest = function (reservation) {
        reservation.CancelRequest = false;
    }

    $scope.cancelReservation = function (reservation) {
        viewModelHelper.apiPost('api/reservation/cancel', reservation.ReservationId,
            function (result) {
                var index = $scope.reservations.indexOf(reservation);
                if (index > -1)
                    $scope.reservations.splice(index, 1);
            });
    }

    $scope.loadOpenReservations();
});
