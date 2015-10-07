
appMainModule.controller("MyAccountViewModel", function ($scope, $http, viewModelHelper, validator) {

    $scope.viewMode = ''; // account, success
    $scope.accountModel = null;
    $scope.viewModelHelper = viewModelHelper;

    var accountModelRules = [];

    var setupRules = function () {
        accountModelRules.push(new validator.PropertyRule("FirstName", {
            required: { message: "First name is required" }
        }));
        accountModelRules.push(new validator.PropertyRule("LastName", {
            required: { message: "Last name is required" }
        }));
        accountModelRules.push(new validator.PropertyRule("Address", {
            required: { message: "Address is required" }
        }));
        accountModelRules.push(new validator.PropertyRule("City", {
            required: { message: "City is required" }
        }));
        accountModelRules.push(new validator.PropertyRule("State", {
            required: { message: "State is required" }
        }));
        accountModelRules.push(new validator.PropertyRule("ZipCode", {
            required: { message: "Zip code is required" },
            pattern: { message: "Zip code is in invalid format", params: /^\d{5}$/ }
        }));
        accountModelRules.push(new validator.PropertyRule("CreditCard", {
            required: { message: "Credit card is required" },
            pattern: { message: "Credit card is in invalid format", params: /^\d{16}$/ }
        }));
        accountModelRules.push(new validator.PropertyRule("ExpDate", {
            required: { message: "Expiration date is required" },
            pattern: { message: "Expiration date is in invalid format", params: /^(0[1-9]|1[0-2])\/[0-9]{2}$/ }
        }));
    }

    $scope.initialize = function () {
        viewModelHelper.apiGet('api/customer/account', null,
            function (result) {
                result.data.ExpDate = result.data.ExpDate.substring(0, 2) + "/" + result.data.ExpDate.substring(2, 4);
                $scope.accountModel = result.data;
                $scope.viewMode = 'account';
            });
    }
        
    $scope.save = function () {
        validator.ValidateModel($scope.accountModel, accountModelRules);
        viewModelHelper.modelIsValid = $scope.accountModel.isValid;
        viewModelHelper.modelErrors = $scope.accountModel.errors;
        if (viewModelHelper.modelIsValid) {
            viewModelHelper.apiPost('api/customer/account', $scope.accountModel,
                function (result) {
                    $scope.viewMode = 'success';
                });
        }
        else
            viewModelHelper.modelErrors = accountModel.errors;
    }

    var validationErrors = function () {
        var errors = [];
        for (var i = 0; i < propertyBag.length; i++) {
            if (propertyBag[i].Invalid) {
                errors.push(propertyBag[i].PropertyName);
            }
        }
        return errors;
    }

    $scope.validate = function (field, invalid) {
         for (var i = 0; i < propertyBag.length; i++)
         {
             if (propertyBag[i].PropertyName == field) {
                 propertyBag[i].Invalid = invalid;
                 break;
             }
         }
    }
    
    setupRules();
    $scope.initialize();

});
