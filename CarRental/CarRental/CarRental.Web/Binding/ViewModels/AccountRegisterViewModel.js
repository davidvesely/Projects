
accountRegisterModule.controller("AccountRegisterViewModel", function ($scope, $http, $location, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;

    $scope.accountModelStep1 = null;
    $scope.accountModelStep2 = null;
    $scope.accountModelStep3 = null;

    $scope.previous = function () {
        window.location.back(); // ???????
    }
});

accountRegisterModule.controller("AccountRegisterViewModelStep1", function ($scope, $http, $location, viewModelHelper) {

    $scope.accountModelStep1 = new CarRental.AccountRegisterModelStep1();

    var accountModelStep1Rules = [];

    var setupRules = function () {
        accountModelStep1Rules.push(new viewModelHelper.PropertyRule("FirstName", {
            required: { message: "First name is required" }
        }));
        accountModelStep1Rules.push(new viewModelHelper.PropertyRule("LastName", {
            required: { message: "Last name is required" }
        }));
        accountModelStep1Rules.push(new viewModelHelper.PropertyRule("Address", {
            required: { message: "Address is required" }
        }));
        accountModelStep1Rules.push(new viewModelHelper.PropertyRule("City", {
            required: { message: "City is required" }
        }));
        accountModelStep1Rules.push(new viewModelHelper.PropertyRule("State", {
            required: { message: "State is required" }
        }));
        accountModelStep1Rules.push(new viewModelHelper.PropertyRule("ZipCode", {
            required: { message: "Zip code is required" }
        }));
    }

    // TODO: maybe can use $location to go "back" instead of view nagivating back

    $scope.step2 = function () {
        viewModelHelper.validateModel($scope.accountModelStep1, accountModelStep1Rules);
        viewModelHelper.modelIsValid = $scope.accountModelStep1.isValid;
        viewModelHelper.modelErrors = $scope.accountModelStep1.errors;
        if (viewModelHelper.modelIsValid) {
            viewModelHelper.apiPost('api/account/register/validate1', $scope.accountModelStep1,
                function (result) {
                    $location.path('/account/register/step2');
                });
        }
        else
            viewModelHelper.modelErrors = accountModelStep1.errors;
    }

    setupRules();
});

accountRegisterModule.controller("AccountRegisterViewModelStep2", function ($scope, $http, $location, viewModelHelper) {

    if ($scope.accountModelStep1 == null) {
        // got to this controller before going through step 1
        $location.path('/account/register/step1');
    }

    $scope.accountModelStep2 = new CarRental.AccountRegisterModelStep2();

    var accountModelStep2Rules = [];

    var setupRules = function () {
        accountModelStep2Rules.push(new viewModelHelper.PropertyRule("FirstName", {
            required: { message: "First name is required" }
        }));
    }

    $scope.step3 = function () {
        viewModelHelper.validateModel($scope.accountModelStep2, accountModelStep2Rules);
        viewModelHelper.modelIsValid = $scope.accountModelStep2.isValid;
        viewModelHelper.modelErrors = $scope.accountModelStep2.errors;
        if (viewModelHelper.modelIsValid) {
            viewModelHelper.apiPost('api/account/register/validate2', $scope.accountModelStep2,
                function (result) {
                    $location.path('/account/register/step3');
                });
        }
        else
            viewModelHelper.modelErrors = accountModelStep2.errors;
    }
});

accountRegisterModule.controller("AccountRegisterViewModelStep3", function ($scope, $http, $location, viewModelHelper) {

    if ($scope.accountModelStep2 == null) {
        // got to this controller before going through step 2
        $location.path('/account/register/step2');
    }

    $scope.accountModelStep3 = new CarRental.AccountRegisterModelStep3();

    var accountModelStep3Rules = [];

    var setupRules = function () {
        accountModelStep3Rules.push(new viewModelHelper.PropertyRule("FirstName", {
            required: { message: "First name is required" }
        }));
    }

    $scope.confirm = function () {
        viewModelHelper.validateModel($scope.accountModelStep3, accountModelStep3Rules);
        viewModelHelper.modelIsValid = $scope.accountModelStep3.isValid;
        viewModelHelper.modelErrors = $scope.accountModelStep3.errors;
        if (viewModelHelper.modelIsValid) {
            viewModelHelper.apiPost('api/account/register/validate3', $scope.accountModelStep3,
                function (result) {
                    $location.path('/account/register/confirm');
                });
        }
        else
            viewModelHelper.modelErrors = accountModelStep3.errors;
    }
});

accountRegisterModule.controller("AccountRegisterViewModelConfirm", function ($scope, $http, $location, viewModelHelper) {

    if ($scope.accountModelStep3 == null) {
        // got to this controller before going through step 3
        $location.path('/account/register/step3');
    }

    $scope.viewMode = 'confirm'; // confirm, welcome
    
    self.createAccount = function (model) {

        var accountModel;

        accountModel = $.extend(accountModel, $scope.accountModelStep1);
        accountModel = $.extend(accountModel, $scope.accountModelStep2);
        accountModel = $.extend(accountModel, $scope.accountModelStep3);

        self.viewModelHelper.apiPost('api/account/register', accountModel,
            function (result) {
                $scope.viewMode = 'welcome';
            });
    }
});
