require.config({
    baseurl: '/scripts/',
    paths: {
        'angular': '../themes/RICONS-2015/js/angularjs/angular',
        'ngStorage': '../themes/RICONS-2015/js/angularjsngStorage',
        'ngCookies': '../themes/RICONS-2015/js/angularjs/angular-cookies',
        'ui-router': '../themes/RICONS-2015/js/angularjs/angular-ui-router',
        'bootstrap': '../themes/login/js/libs/bootstrap',
        'service': '../themes/login/js/services/service',
        'homeCtrl': '../themes/login/js/controllers/homeCtrl',
        'accountCtrl': '../themes/login/js/controllers/accountCtrl',
        'filter': '../themes/login/js/filters/filter',
    },
    shim: {
        ngStorage: {
            deps: ['angular'],
            exports: 'angular'
        },
        ngCookies: {
            deps: ['angular'],
            exports: 'angular'
        },
        'ui-router': {
            deps: ['angular'],
            exports: 'angular'
        },
        angular: {
            exports: 'angular'
        },
        bootstrap: {
            deps: ['jquery']
        }
    },
    deps: ['app']
});

require([
    "app",
    "filter",
    "bootstrap",
    "homeCtrl",
    "accountCtrl"
],

    function (app) {
        //bootstrapping app
        app.init();
    }
);