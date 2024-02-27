const { src, dest, series, watch } = require('gulp');
const sass = require('gulp-sass')(require('node-sass'));

function compileSass() {
    return src('src/scss/*.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(dest('dist/css'));
}

function watchSass() {
    watch('src/scss/**/*.scss', compileSass);
}

exports.default = series(compileSass, watchSass);
