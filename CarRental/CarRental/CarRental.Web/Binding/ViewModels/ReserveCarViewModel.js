
reserveCarModule.controller("ReserveCarViewModel", function ($scope, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.reserveCarModel = new CarRental.ReserveCarModel();
    $scope.castro = { propA: 'A', propB: 'B' };
});

// this is a child controller of ReserveCarViewModel
reserveCarModule.controller("ReserveViewModel", function ($scope, $http, $location, viewModelHelper) {

    var reserveCarModelRules = [];

    var setupRules = function () {
        reserveCarModelRules.push(new viewModelHelper.PropertyRule("PickupDate", {
            required: { message: "Pickup date is required" }
        }));
        reserveCarModelRules.push(new viewModelHelper.PropertyRule("ReturnDate", {
            required: { message: "Return date is required" }
        }));
    }

    $scope.reserveCarModel.initialized = true; // cannot rely on null when object is created in parent viewmodel
    
    $scope.submit = function () {
        if ($scope.reserveCarModel.PickupDate != null && $scope.reserveCarModel.PickupDate != '')
            $scope.reserveCarModel.PickupDate = moment($scope.reserveCarModel.PickupDate).format('MM/DD/YYYY');
        else
            $scope.reserveCarModel.PickupDate = '';
        if ($scope.reserveCarModel.ReturnDate != null && $scope.reserveCarModel.ReturnDate != '')
            $scope.reserveCarModel.ReturnDate = moment($scope.reserveCarModel.ReturnDate).format('MM/DD/YYYY');
        else
            $scope.reserveCarModel.ReturnDate = '';
        
        viewModelHelper.validateModel($scope.reserveCarModel, reserveCarModelRules);
        viewModelHelper.modelIsValid = $scope.reserveCarModel.isValid;
        viewModelHelper.modelErrors = $scope.reserveCarModel.errors;
        if (viewModelHelper.modelIsValid) {
            $location.path(CarRental.rootPath + 'customer/reserve/carlist');
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

    $scope.viewMode = 'carlist'; // carlist, success
    $scope.cars = [];
    $scope.reservationNumber = '';
    
    $scope.availableCars = function () {
        if (!$scope.reserveCarModel.initialized) {
            // cannot rely on null here because if object is newd-up in reserveViewModel, it will not carry into here
            // got to this controller before going through step 1
            $location.path(CarRental.rootPath + 'customer/reserve');
        }
        viewModelHelper.apiGet('api/reservation/availablecars',
                { pickupDate: $scope.reserveCarModel.PickupDate, returnDate: $scope.reserveCarModel.ReturnDate },
            function (result) {
                $scope.cars = result.data;
            });    
    }

    $scope.selectCar = function (car) {
        var model = { PickupDate: $scope.reserveCarModel.PickupDate, ReturnDate: $scope.reserveCarModel.ReturnDate, Car: car.CarId() };
        viewModelHelper.apiPost('api/reservation/reservecar', model,
            function (result) {
                $scope.reservationNumber = result.data.ReservationId;
                $scope.viewMode = 'success';
            });
    }

    $scope.availableCars();
});
