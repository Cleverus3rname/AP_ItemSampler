/// <binding ProjectOpened='all, watch' />
'use strict';

const lessFiles = {
    'Client/css/about.css': 'Client/Styles/about.less',
    'Client/css/home.css': 'Client/Styles/home.less',
    'Client/css/item.css': 'Client/Styles/item.less',
    'Client/css/nav.css': 'Client/Styles/nav.less',
    'Client/css/search.css': 'Client/Styles/search.less',
    'Client/css/site.css': 'Client/Styles/site.less'};


module.exports = function (grunt) {
    grunt.initConfig({
     

   

        less: {
            development: {
                files: lessFiles,
                sourceMap: true
            },
            production: {
                files: lessFiles
            }
        },

        

     

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

    grunt.loadNpmTasks('grunt-contrib-less');
    grunt.loadNpmTasks('grunt-version');



};
