"use strict"
var gulp = require('gulp');
var ts = require('gulp-typescript');
var tsProject = ts.createProject('tsconfig.json');
var del = require('del');
var srcmap = require('gulp-sourcemaps');
var path = require('path');

var projectRoot = path.resolve(__dirname);
var srcRoot = path.resolve(projectRoot, 'src');
var config = {
    paths: {
        project: {
            root: projectRoot
        },
        extension: {
            root: srcRoot
        }
    }
}

gulp.task('ext:compile-src', (done) => {

    console.log('compile src ' + config.paths.project.root);

    return gulp.src([
                config.paths.project.root + '/src/**/*.ts',
                config.paths.project.root + '/src/**/*.js'])
                .pipe(srcmap.init())
                .pipe(tsProject())
                .on('error', function() {
                    if (process.env.BUILDMACHINE) {
                        done('Extension Tests failed to build. See Above.');
                        process.exit(1);
                    }
                })
                .pipe(srcmap.write('.', {
                   sourceRoot: function(file){ return file.cwd + '/src'; }
                }))
                .pipe(gulp.dest('out/src/'));
});

gulp.task('ext:compile', gulp.series('ext:compile-src'));

gulp.task('clean', function (done) {
    return del('out', done);
});

gulp.task('compile', gulp.series('clean', 'ext:compile'));

gulp.task('watch', function(){
    return gulp.watch(config.paths.project.root + '/src/**/*', gulp.series('compile'))
});
