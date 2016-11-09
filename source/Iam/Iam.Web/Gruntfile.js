/// <binding BeforeBuild='build:css, build:js' />
module.exports = function (grunt) {

    grunt.initConfig({
        concat: {
            css: {
                src: [
                    "lib/bootstrap/dist/css/bootstrap.min.css",
                    "lib/components-font-awesome/css/font-awesome.min.css"
                ],
                dest: "content/styles.min.css"
            },
            js: {
                options: {
                    separator: ";"
                },
                src: [
                    "lib/jquery/dist/jquery.min.js",
                    "lib/bootstrap/dist/js/bootstrap.min.js",
                    "lib/angularjs/angular.min.js",
                    "lib/underscore/underscore.min.js",
                    "scripts/encoder.min.js"
                ],
                dest: "scripts/scripts.min.js"
            }
        },
        copy: {
            fonts: {
                expand: true,
                cwd: "lib/components-font-awesome/fonts/",
                src: "*",
                dest: "fonts/"
            }
        }
    });
    
    grunt.loadNpmTasks("grunt-contrib-concat");
    grunt.loadNpmTasks("grunt-contrib-copy");
    
    grunt.registerTask("build:js", ["concat:js"]);
    grunt.registerTask("build:css", ["concat:css", "copy:fonts"]);

};