module.exports = {
    publicPath: '',
    devServer: {
        open: 'chrome',
    },
    css: {
        loaderOptions: {
            sass: {
                additionalData: `
                @import "@/styles/imports/_colors.scss";
            `,
            },
        },
    },
};
