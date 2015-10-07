
var accountRegisterModule = angular.module('accountRegister', ['common'])
    .config(function ($routeProvider, $locationProvider) {
        $routeProvider.when(CarRental.rootPath + 'account/register/step1', { templateUrl: CarRental.rootPath + 'Templates/RegisterStep1.html', controller: 'AccountRegisterStep1ViewModel' });
        $routeProvider.when(CarRental.rootPath + 'account/register/step2', { templateUrl: CarRental.rootPath + 'Templates/RegisterStep2.html', controller: 'AccountRegisterStep2ViewModel' });
        $routeProvider.when(CarRental.rootPath + 'account/register/step3', { templateUrl: CarRental.rootPath + 'Templates/RegisterStep3.html', controller: 'AccountRegisterStep3ViewModel' });
        $routeProvider.when(CarRental.rootPath + 'account/register/confirm', { templateUrl: CarRental.rootPath + 'Templates/RegisterConfirm.html', controller: 'AccountRegisterConfirmViewModel' });
        $routeProvider.otherwise({ redirectTo: CarRental.rootPath + 'account/register/step1' });
        $locationProvider.html5Mode(true);
    });

accountRegisterModule.controller("AccountRegisterViewModel", function ($scope, $http, $location, $window, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;

    $scope.accountModelStep1 = new CarRental.AccountRegisterModelStep1();
    $scope.accountModelStep2 = new CarRental.AccountRegisterModelStep2();
    $scope.accountModelStep3 = new CarRental.AccountRegisterModelStep3();

    $scope.previous = function () {
        $window.history.back();
    }
});

accountRegisterModule.controller("AccountRegisterStep1ViewModel", function ($scope, $http, $location, viewModelHelper, validator) {

    viewModelHelper.modelIsValid = true;
    viewModelHelper.modelErrors = [];

    var accountModelStep1Rules = [];

    validator.TestProp = 'miguel';

    var setupRules = function () {
        accountModelStep1Rules.push(new validator.PropertyRule("FirstName", {
            required: { message: "First name is required" }
        }));
        accountModelStep1Rules.push(new validator.PropertyRule("LastName", {
            required: { message: "Last name is required" }
        }));
        accountModelStep1Rules.push(new validator.PropertyRule("Address", {
            required: { message: "Address is required" }
        }));
        accountModelStep1Rules.push(new validator.PropertyRule("City", {
            required: { message: "City is required" }
        }));
        accountModelStep1Rules.push(new validator.PropertyRule("State", {
            required: { message: "State is required" }
        }));
        accountModelStep1Rules.push(new validator.PropertyRule("ZipCode", {
            required: { message: "Zip code is required" },
            pattern: { message: "Zip code is in invalid format", params: /^\d{5}$/ }
        }));
    }

    $scope.step2 = function () {
        validator.ValidateModel($scope.accountModelStep1, accountModelStep1Rules);
        viewModelHelper.modelIsValid = $scope.accountModelStep1.isValid;
        viewModelHelper.modelErrors = $scope.accountModelStep1.errors;
        if (viewModelHelper.modelIsValid) {
            viewModelHelper.apiPost('api/account/register/validate1', $scope.accountModelStep1,
                function (result) {
                    $scope.accountModelStep1.Initialized = true;
                    $location.path(CarRental.rootPath + 'account/register/step2');
                });
        }
        else
            viewModelHelper.modelErrors = $scope.accountModelStep1.errors;
    }

    setupRules();
});

accountRegisterModule.controller("AccountRegisterStep2ViewModel", function ($scope, $http, $location, viewModelHelper, validator) {

    if (!$scope.accountModelStep1.Initialized) {
        // got to this controller before going through step 1
        $location.path(CarRental.rootPath + 'account/register/step1');
    }

    viewModelHelper.modelIsValid = true;
    viewModelHelper.modelErrors = [];

    var accountModelStep2Rules = [];

    var setupRules = function () {
        accountModelStep2Rules.push(new validator.PropertyRule("LoginEmail", {
            required: { message: "Login Email is required" }
        }));
        accountModelStep2Rules.push(new validator.PropertyRule("Password", {
            required: { message: "Password is required" },
            minLength: { message: "Password must be at least 6 characters", params: 6 }
        }));
        accountModelStep2Rules.push(new validator.PropertyRule("PasswordConfirm", {
            required: { message: "Password confirmation is required" },
            custom: {
                validator: CarRental.mustEqual,
                message: "Password do not match",
                params: function () { return $scope.accountModelStep2.Password; } // must be function so it can be obtained on-demand
            }
        }));
    }

    $scope.step3 = function () {
        validator.ValidateModel($scope.accountModelStep2, accountModelStep2Rules);
        viewModelHelper.modelIsValid = $scope.accountModelStep2.isValid;
        viewModelHelper.modelErrors = $scope.accountModelStep2.errors;
        if (viewModelHelper.modelIsValid) {
            viewModelHelper.apiPost('api/account/register/validate2', $scope.accountModelStep2,
                function (result) {
                    $scope.accountModelStep2.Initialized = true;
                    $location.path(CarRental.rootPath + 'account/register/step3');
                });
        }
        else
            viewModelHelper.modelErrors = $scope.accountModelStep2.errors;
    }

    setupRules();
});

accountRegisterModule.controller("AccountRegisterStep3ViewModel", function ($scope, $http, $location, viewModelHelper, validator) {

    if (!$scope.accountModelStep2.Initialized) {
        // got to this controller before going through step 2
        $location.path(CarRental.rootPath + 'account/register/step2');
    }

    var accountModelStep3Rules = [];

    var setupRules = function () {
        accountModelStep3Rules.push(new validator.PropertyRule("CreditCard", {
            required: { message: "Credit Card # is required" },
            pattern: { message: "Credit card is in invalid format (16 digits)", params: /^\d{16}$/ }
        }));
        accountModelStep3Rules.push(new validator.PropertyRule("ExpDate", {
            required: { message: "Expiration Date is required" },
            pattern: { message: "Expiration Date is in invalid format (MM/YY)", params: /^(0[1-9]|1[0-2])\/[0-9]{2}$/ }
        }));
    }

    $scope.confirm = function () {
        validator.ValidateModel($scope.accountModelStep3, accountModelStep3Rules);
        viewModelHelper.modelIsValid = $scope.accountModelStep3.isValid;
        viewModelHelper.modelErrors = $scope.accountModelStep3.errors;
        if (viewModelHelper.modelIsValid) {
            viewModelHelper.apiPost('api/account/register/validate3', $scope.accountModelStep3,
                function (result) {
                    $scope.accountModelStep3.Initialized = true;
                    $location.path(CarRental.rootPath + 'account/register/confirm');
                });
        }
        else
            viewModelHelper.modelErrors = $scope.accountModelStep3.errors;
    }

    setupRules();
});

accountRegisterModule.controller("AccountRegisterConfirmViewModel", function ($scope, $http, $location, $window, viewModelHelper) {

    if (!$scope.accountModelStep3.Initialized) {
        // got to this controller before going through step 3
        $location.path(CarRental.rootPath + 'account/register/step3');
    }

    $scope.createAccount = function () {

        var accountModel;

        accountModel = $.extend(accountModel, $scope.accountModelStep1);
        accountModel = $.extend(accountModel, $scope.accountModelStep2);
        accountModel = $.extend(accountModel, $scope.accountModelStep3);

        viewModelHelper.apiPost('api/account/register', accountModel,
            function (result) {
                //$location.path(CarRental.rootPath);
                $window.location.href = CarRental.rootPath;
            });
    }
});
