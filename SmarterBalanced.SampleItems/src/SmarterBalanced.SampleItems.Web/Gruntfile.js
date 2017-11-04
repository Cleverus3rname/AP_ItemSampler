/// <binding ProjectOpened='all, watch' />
'use strict';

module.exports = function (grunt) {
    grunt.initConfig({
     
        version: {
            package: {
                src: ['package.json']
            },
            csproj: {
                options: {
                    prefix: '<VersionPrefix>'
                },
                src: [
                    '../SmarterBalanced.SampleItems.Core/SmarterBalanced.SampleItems.Core.csproj',
                    '../SmarterBalanced.SampleItems.Dal/SmarterBalanced.SampleItems.Dal.csproj',
                    '../SmarterBalanced.SampleItems.Test/SmarterBalanced.SampleItems.Test.csproj',
                    'SmarterBalanced.SampleItems.Web.csproj'
                ]
            }
        }
    });

    grunt.loadNpmTasks('grunt-version');

};
