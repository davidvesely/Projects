
appMainModule.controller("AccountChangePasswordViewModel", function ($scope, $http, viewModelHelper, validator) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.passwordModel = new CarRental.AccountChangePasswordModel();
    $scope.viewMode = 'changepw'; // changepw, success
    //$scope.loginEmail = '';
    
    var passwordModelRules = [];

    var setupRules = function () {
        passwordModelRules.push(new validator.PropertyRule("OldPassword", {
            required: { message: "Password is required" },
            minLength: { message: "Password must be at least 6 characters", value: 6 }
        }));
        passwordModelRules.push(new validator.PropertyRule("NewPassword", {
            required: { message: "New password is required" },
            minLength: { message: "Old Password must be at least 6 characters", value: 6 }
        }));
    }

    $scope.changePassword = function () {
        validator.ValidateModel($scope.passwordModel, passwordModelRules);
        viewModelHelper.modelIsValid = $scope.passwordModel.isValid;
        viewModelHelper.modelErrors = $scope.passwordModel.errors;
        if (viewModelHelper.modelIsValid) {
            //$scope.passwordModel['loginEmail'] = $scope.loginEmail;
            viewModelHelper.apiPost('api/account/changepw', $scope.passwordModel,
                function (result) {
                    $scope.viewMode = 'success';
                });
        }
        else
            viewModelHelper.modelErrors = passwordModel.errors;
    }

    setupRules();
});
