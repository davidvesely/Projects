
var commonModule = angular.module('common', ['ngRoute','ui.bootstrap']);

// non-SPA views will use Angular controllers created on the appMainModule

var appMainModule = angular.module('appMain', ['common']);

// SPA-views will attach to their own module and use their own data-ng-app and nested controllers

var reserveCarModule = angular.module('reserveCar', ['common'])
    .config(function ($routeProvider, $locationProvider) {
        $routeProvider.when(CarRental.rootPath + 'customer/reserve', { templateUrl: CarRental.rootPath + 'Templates/Reserve.html', controller: 'ReserveViewModel' });
        $routeProvider.when(CarRental.rootPath + 'customer/reserve/carlist', { templateUrl: CarRental.rootPath + 'Templates/CarList.html', controller: 'CarListViewModel' });
        $routeProvider.otherwise({ redirectTo: CarRental.rootPath + 'customer/reserve' });
        $locationProvider.html5Mode(true);
    });

var accountRegisterModule = angular.module('accountRegister', ['common'])
    .config(function ($routeProvider, $locationProvider) {
        $routeProvider.when(CarRental.rootPath + 'account/register/step1', { templateUrl: CarRental.rootPath + 'Templates/RegisterStep1.html', controller: 'AccountRegisterStep1ViewModel' });
        $routeProvider.when(CarRental.rootPath + 'account/register/step2', { templateUrl: CarRental.rootPath + 'Templates/RegisterStep2.html', controller: 'AccountRegisterStep2ViewModel' });
        $routeProvider.when(CarRental.rootPath + 'account/register/step3', { templateUrl: CarRental.rootPath + 'Templates/RegisterStep3.html', controller: 'AccountRegisterStep3ViewModel' });
        $routeProvider.when(CarRental.rootPath + 'account/register/confirm', { templateUrl: CarRental.rootPath + 'Templates/RegisterConfirm.html', controller: 'AccountRegisterConfirmViewModel' });
        $routeProvider.otherwise({ redirectTo: CarRental.rootPath + 'account/register/step1' });
        $locationProvider.html5Mode(true);
    });

// services attached to the commonModule will be available to all other Angular modules (above)

commonModule.directive('jqdatepicker', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.datepicker({
                changeYear: "true",
                changeMonth: true,
                dateFormat: 'mm-dd-yyyy',
                showOtherMonths: true,
                showButtonPanel: true,
                onClose: function (selectedDate) {
                    scope.dateTtime = selectedDate + "T" + scope.time;
                    scope.$apply();
                }
            });
        }
    };
})

commonModule.factory('viewModelHelper', function ($http, $q) {
    return CarRental.viewModelHelper($http, $q);
});

(function (cr) {
    var initialId;
    cr.initialId = initialId;
}(window.CarRental));

(function (cr) {
    var initialState;
    cr.initialState = initialState;
}(window.CarRental));

//(function (cr) {
//    var rootPath;
//    cr.rootPath = rootPath;
//}(window.CarRental));

(function (cr) {
    var mustEqual = function (val, other) {
        return val == other();
    }
    cr.mustEqual = mustEqual;
}(window.CarRental));

(function (cr) {
    var viewModelHelper = function ($http, $q) {
        
        var self = this;

        self.modelIsValid = ko.observable(true);
        self.modelErrors = ko.observableArray();
        self.isLoading = ko.observable(false);

        self.statePopped = false;
        self.stateInfo = {};

        self.apiGet = function (uri, data, success, failure, always) {
            self.isLoading = true;
            self.modelIsValid = true;
            $http.get(CarRental.rootPath + uri, data)
                .then(function (result) {
                    success(result);
                    if (always != null)
                        always();
                    self.isLoading = false;
                }, function (result) {
                    if (failure == null) {
                        if (result.status != 400)
                            self.modelErrors = [result.status + ':' + result.statusText + ' - ' + result.data.Message];
                        else
                            self.modelErrors = [result.data.Message];
                        self.modelIsValid = false;
                    }
                    else
                        failure(result);
                    if (always != null)
                        always();
                    self.isLoading = false;
                });
        }

        self.apiPost = function (uri, data, success, failure, always) {
            self.isLoading = true;
            self.modelIsValid = true;
            $http.post(CarRental.rootPath + uri, data)
                .then(function (result) {
                    success();
                    if (always != null)
                        always();
                    self.isLoading = false;
                }, function (result) {
                    if (failure == null) {
                        if (result.status != 400)
                            self.modelErrors = [result.status + ':' + result.statusText + ' - ' + result.data.Message];
                        else
                            self.modelErrors = [result.data.Message];
                        self.modelIsValid = false;
                    }
                    else
                        failure(result);
                    if (always != null)
                        always();
                    isLoading = false;
                });
        }

        self.PropertyRule = function (propertyName, rules) {
            var self = this;
            self.PropertyName = propertyName;
            self.Rules = rules;
        };

        self.validateModel = function (model, propertyRules) {
            var errors = [];
            var props = Object.keys(model);
            for (var i = 0; i < props.length; i++) {
                var prop = props[i];
                for (var j = 0; j < propertyRules.length; j++) {
                    var propertyRule = propertyRules[j];
                    if (prop == propertyRule.PropertyName) {
                        var rules = propertyRule.Rules;
                        if (rules.hasOwnProperty('required')) {
                            if (model[prop].trim() == '') {
                                errors.push(getMessage(rules.required));
                            }
                        }
                        if (rules.hasOwnProperty('pattern')) {
                            var regExp = new RegExp(rules.pattern.value);
                            if (regExp.exec(model[prop].trim()) == null) {
                                errors.push(getMessage(rules.pattern));
                            }
                        }
                        if (rules.hasOwnProperty('minLength')) {
                            var minLength = rules.minLength.value;
                            if (model[prop].trim().length < minLength) {
                                errors.push(getMessage(rules.minLength));
                            }
                        }
                    }
                }
            }

            model['errors'] = errors;
            model['isValid'] = (errors.length == 0);
        }

        var getMessage = function (rule) {
            var message = '';
            if (rule.hasOwnProperty('message'))
                message = rule.message;
            else
                message = prop + ' is invalid.';
            return message;
        }

        //self.pushUrlState = function (code, title, id, url) {
        //    self.stateInfo = { State: { Code: code, Id: id }, Title: title, Url: CarRental.rootPath + url };
        //}

        //self.handleUrlState = function (initialState) {
        //    if (!self.statePopped) {
        //        if (initialState != '') {
        //            history.replaceState(self.stateInfo.State, self.stateInfo.Title, self.stateInfo.Url);
        //            // we're past the initial nav state so from here on everything should push
        //            initialState = '';
        //        }
        //        else {
        //            history.pushState(self.stateInfo.State, self.stateInfo.Title, self.stateInfo.Url);
        //        }
        //    }
        //    else
        //        self.statePopped = false; // only actual popping of state should set this to true

        //    return initialState;
        //}

        return this;
    }
    cr.viewModelHelper = viewModelHelper;
}(window.CarRental));
