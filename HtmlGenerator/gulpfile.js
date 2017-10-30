﻿"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    htmlmin = require("gulp-htmlmin"),
    //uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    del = require("del"),
    order = require("gulp-order"),
    uglify = require("gulp-uglify");

var paths = {
  webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/";
paths.libs = paths.webroot + "lib/";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src(
        
        [   paths.libs + "requirejs/require.js",
            paths.js + "hopscotch/**/*.js",
            paths.js + "blockly/**/*.js",
            paths.js + "**/*.js",
            "!" + paths.minJs],
        { base: "." })
        .pipe(concat(paths.concatJsDest))
        //.pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});
gulp.task("series:first", function () {
    console.log('first task! <-----');
})
gulp.task("series:second", ["series:first"], function () {
    console.log('second task! <-----');
});

gulp.task("min", ["min:js", "min:css"]);