
var reserveCarModule = angular.module('reserveCar', ['common'])
    .config(function ($routeProvider, $locationProvider) {
        $routeProvider.when(CarRental.rootPath + 'customer/reservecar', { templateUrl: CarRental.rootPath + 'Templates/Reserve.html', controller: 'ReserveViewModel' });
        $routeProvider.when(CarRental.rootPath + 'customer/reservecar/carlist', { templateUrl: CarRental.rootPath + 'Templates/CarList.html', controller: 'CarListViewModel' });
        $routeProvider.otherwise({ redirectTo: CarRental.rootPath + 'customer/reservecar' });
        $locationProvider.html5Mode(true);
    });

reserveCarModule.controller("ReserveCarViewModel", function ($scope, $window, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.reserveCarModel = new CarRental.ReserveCarModel();

    $scope.previous = function () {
        $window.history.back();
    }
});

// this is a child controller of ReserveCarViewModel
reserveCarModule.controller("ReserveViewModel", function ($scope, $http, $location, viewModelHelper, validator) {

    viewModelHelper.modelIsValid = true;
    viewModelHelper.modelErrors = [];

    var reserveCarModelRules = [];

    var setupRules = function () {
        reserveCarModelRules.push(new validator.PropertyRule("PickupDate", {
            required: { message: "Pickup date is required" }
        }));
        reserveCarModelRules.push(new validator.PropertyRule("ReturnDate", {
            required: { message: "Return date is required" }
        }));
    }

    $scope.submit = function () {
        if ($scope.reserveCarModel.PickupDate != null && $scope.reserveCarModel.PickupDate != '')
            $scope.reserveCarModel.PickupDate = moment($scope.reserveCarModel.PickupDate).format('MM-DD-YYYY');
        else
            $scope.reserveCarModel.PickupDate = '';
        if ($scope.reserveCarModel.ReturnDate != null && $scope.reserveCarModel.ReturnDate != '')
            $scope.reserveCarModel.ReturnDate = moment($scope.reserveCarModel.ReturnDate).format('MM-DD-YYYY');
        else
            $scope.reserveCarModel.ReturnDate = '';
        
        validator.ValidateModel($scope.reserveCarModel, reserveCarModelRules);
        viewModelHelper.modelIsValid = $scope.reserveCarModel.isValid;
        viewModelHelper.modelErrors = $scope.reserveCarModel.errors;
        if (viewModelHelper.modelIsValid) {
            $scope.reserveCarModel.initialized = true; // cannot rely on null when object is created in parent viewmodel
            $location.path(CarRental.rootPath + 'customer/reservecar/carlist');
        }
        else
            viewModelHelper.modelErrors = $scope.reserveCarModel.errors;
    }

    $scope.openPickup = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.openedPickup = true;
    }

    $scope.openReturn = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.openedReturn = true;
    }

    setupRules();
});

// this is a child controller of ReserveCarViewModel
reserveCarModule.controller("CarListViewModel", function ($scope, $http, $location, viewModelHelper) {

    viewModelHelper.modelIsValid = true;
    viewModelHelper.modelErrors = [];

    $scope.viewMode = 'carlist'; // carlist, success
    $scope.cars = [];
    $scope.reservationNumber = '';
    $scope.init = false; // used so view doesn't sit on "no available cars" while load takes place

    $scope.availableCars = function () {
        if (!$scope.reserveCarModel.initialized) {
            $location.path(CarRental.rootPath + 'customer/reservecar');
        }
        else {
            viewModelHelper.apiGet('api/reservation/availablecars/' + $scope.reserveCarModel.PickupDate + '/' + $scope.reserveCarModel.ReturnDate, null,
                function (result) {
                    $scope.cars = result.data;
                    $scope.init = true;
                });
        }
    }

    $scope.selectCar = function (car) {
        var model = { PickupDate: $scope.reserveCarModel.PickupDate, ReturnDate: $scope.reserveCarModel.ReturnDate, Car: car.CarId };
        viewModelHelper.apiPost('api/reservation/reservecar', model,
            function (result) {
                $scope.reservationNumber = result.data.ReservationId;
                $scope.viewMode = 'success';
            });
    }

    $scope.availableCars();
});
