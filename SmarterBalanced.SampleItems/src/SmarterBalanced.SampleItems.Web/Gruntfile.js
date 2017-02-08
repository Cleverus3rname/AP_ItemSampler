/// <binding ProjectOpened='all, watch' />
'use strict';

const lessFiles = {
    'wwwroot/css/nav.css': 'Styles/nav.less',
    'wwwroot/css/HomePage.css': 'Styles/HomePage.less'
};

module.exports = function (grunt) {
    grunt.initConfig({
        clean: ["wwwroot/scripts/*", "temp/"],

        // TODO: Minify JS eventually
        //uglify: {
        //    all: {
        //        src: ["wwwroot/scripts/*.js"],
        //        dest: "wwwroot/scripts/*.min.js"
        //    }
        //},

        less: {
            development: {
                files: lessFiles
            },
            production: {
                files: lessFiles
            }
        },
        
        cssmin: {
            options: {
                shorthandCompacting: false,
                roundingPrecision: -1
            },
            target: {
                files: {
                    "wwwroot/css/site.min.css": ["wwwroot/css/site.css"],
                    "wwwroot/css/HomePage.min.css": ["wwwroot/css/HomePage.css"],
                    "wwwroot/css/ItemPage.min.css": ["wwwroot/css/ItemPage.css"],
                    "wwwroot/css/Navbar.min.css": ["wwwroot/css/Navbar.css"],
                    "wwwroot/css/SearchPage.min.css": ["wwwroot/css/SearchPage.css"]
                }
            }
        },

        ts: {
            default: {
                tsconfig: {
                    tsconfig: 'Scripts/tsconfig.json',
                },
            },
        },

        watch: {
            files: ["Styles/*.less"],
            tasks: ["less"],
        },
        
        karma: {
            unit: {
                configFile: 'Scripts/karma.conf.js'
            }
        }
        //TODO: add watch for css.min, issue with watch.

    });

    grunt.loadNpmTasks('grunt-contrib-clean');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-ts');
    grunt.loadNpmTasks('grunt-karma');
    grunt.loadNpmTasks('grunt-contrib-less');

    grunt.registerTask("all", ['clean', 'ts', 'cssmin', 'less']); //,'uglify']); // TODO: Minify JS eventually
    grunt.registerTask("tsrecompile", ['clean', 'ts']);

};